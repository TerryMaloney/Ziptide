using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// GROW-A-GARDEN, the world-object layer (Quality Bar P3 — the Roblox-or-better bar starts here).
    /// Binds a <see cref="PlotState"/> in this world's <see cref="WorldState"/> through the pure,
    /// CI-tested <see cref="GardenService"/> backend:
    ///
    ///   EMPTY   — select the soil to plant this planter's seed (GARDEN_PLANT)
    ///   GROWING — the plant VISIBLY grows (scale + color follow GrowthProgress; offline time counts —
    ///             ProfileEconomy resolves idle growth on world entry)
    ///   READY   — glow + readout flips; select to harvest: yield credits the profile (GARDEN_HARVEST)
    ///             and the plot resets to EMPTY for replanting.
    ///
    /// Spawned by JobDirector from <see cref="GardenSpawnDefinition"/> pack data at the HarvestGrove
    /// POI's planter pads (the pads themselves are WorldPoiBuilder geometry). v1 harvests with the
    /// implicit field-gloves tool (Resources/Garden/field_gloves); dedicated tend/harvest tools arrive
    /// with the tool-chest system and slot straight into GardenService.Tend — no changes here.
    /// </summary>
    public class GardenPlotRuntime : MonoBehaviour
    {
        private static readonly Color SoilColor = new Color(0.30f, 0.22f, 0.16f);
        private static readonly Color SproutColor = new Color(0.40f, 0.65f, 0.35f);
        private static readonly Color ReadyColor = new Color(0.55f, 0.95f, 0.45f);
        private static readonly Color TextColor = new Color(0.75f, 0.92f, 0.75f);

        private GardenSpawnDefinition _def;
        private string _worldId;
        private PlantDefinition _plant;
        private ToolDefinition _gloves;
        private GameObject _plantVisual;
        private Renderer _plantRenderer;
        private TextMesh _readout;
        private float _refreshTimer;

        /// <summary>Build + bind. Called by JobDirector right after AddComponent (runtime only).</summary>
        public void Init(GardenSpawnDefinition def, string worldId)
        {
            _def = def ?? new GardenSpawnDefinition();
            _worldId = worldId;
            _plant = Resources.Load<PlantDefinition>("Garden/" + _def.plantId);
            _gloves = Resources.Load<ToolDefinition>("Garden/field_gloves");
            if (_plant == null)
                Debug.LogWarning("ZIPTIDE: GARDEN_PLANT_DEF_MISSING id=" + _def.plantId);
            Build();
        }

        private void Build()
        {
            // The interactive soil bed — selects drive the whole loop.
            var soil = GameObject.CreatePrimitive(PrimitiveType.Cube);
            soil.name = "Soil";
            soil.transform.SetParent(transform, false);
            soil.transform.localPosition = new Vector3(0f, 0.0f, 0f);
            soil.transform.localScale = new Vector3(1.0f, 0.12f, 0.8f);
            Paint(soil, SoilColor);
            var interactable = soil.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => OnSelected());

            _plantVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _plantVisual.name = "Plant";
            var pc = _plantVisual.GetComponent<Collider>();
            if (pc != null) Destroy(pc);
            _plantVisual.transform.SetParent(transform, false);
            Paint(_plantVisual, SproutColor);
            _plantRenderer = _plantVisual.GetComponent<Renderer>();
            _plantVisual.SetActive(false);

            var readoutGo = new GameObject("Readout");
            _readout = readoutGo.AddComponent<TextMesh>();
            _readout.characterSize = 0.025f;
            _readout.fontSize = 48;
            _readout.anchor = TextAnchor.MiddleCenter;
            _readout.alignment = TextAlignment.Center;
            _readout.color = TextColor;
            readoutGo.transform.SetParent(transform, false);
            readoutGo.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        }

        private PlotState Plot()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return null;
            var world = profile.GetWorld(_worldId, createIfMissing: true);
            if (world.plots == null) return null;
            return world.plots.Find(p => p != null && p.plotId == _def.id && !p.harvested);
        }

        private static long Now() => System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private void OnSelected()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || _plant == null) return;
            var world = profile.GetWorld(_worldId, createIfMissing: true);
            var plot = Plot();

            if (plot == null)
            {
                var planted = GardenService.Plant(world, _plant, Now());
                if (planted != null)
                {
                    planted.plotId = _def.id;
                    Debug.Log("ZIPTIDE: GARDEN_PLANT plot=" + _def.id + " plant=" + _plant.id +
                              " grow=" + _plant.growSeconds + "s");
                }
                return;
            }

            if (plot.IsReady(Now()))
            {
                var result = GardenService.Harvest(profile, plot, _plant, _gloves, Now());
                if (result.Success)
                {
                    // Keep the save lean: the harvested plot entry is done — remove it so replanting
                    // doesn't accumulate dead entries across sessions.
                    world.plots.Remove(plot);
                    Debug.Log("ZIPTIDE: GARDEN_HARVEST plot=" + _def.id + " plant=" + _plant.id +
                              " mult=" + result.yieldMultiplier.ToString("F2") + " entries=" + result.yieldEntries);
                }
                else
                {
                    Debug.Log("ZIPTIDE: GARDEN_HARVEST_BLOCKED plot=" + _def.id + " reason=" + result.status +
                              (_gloves == null ? " (field_gloves asset missing)" : ""));
                }
            }
            // Growing: selection is a no-op for now — tend tools land with the tool chest.
        }

        private void Update()
        {
            _refreshTimer -= Time.deltaTime;
            if (_refreshTimer > 0f) return;
            _refreshTimer = 0.5f; // readout + growth visuals tick at 2Hz — plenty for a plant

            var plot = Plot();
            long now = Now();
            string plantName = _plant != null ? _plant.DisplayName.Replace('_', ' ') : _def.plantId;

            if (plot == null)
            {
                if (_plantVisual != null) _plantVisual.SetActive(false);
                SetReadout("[ " + plantName + " ]\n< select soil to plant >");
            }
            else if (!plot.IsReady(now))
            {
                float t = (float)plot.GrowthProgress(now);
                ShowPlant(t, false);
                long remain = (long)plot.growSeconds - (now - plot.plantedAtUnix);
                if (remain < 0) remain = 0;
                SetReadout(plantName + "  " + Mathf.RoundToInt(t * 100f) + "%\n" + FormatTime(remain) + " to harvest");
            }
            else
            {
                ShowPlant(1f, true);
                SetReadout(plantName + "  READY\n< select soil to harvest >");
            }

            var cam = Camera.main;
            if (cam != null && _readout != null)
                _readout.transform.rotation = Quaternion.LookRotation(_readout.transform.position - cam.transform.position);
        }

        private void ShowPlant(float growth, bool ready)
        {
            if (_plantVisual == null) return;
            _plantVisual.SetActive(true);
            float h = Mathf.Lerp(0.08f, 0.65f, growth);
            float w = Mathf.Lerp(0.06f, 0.22f, growth);
            _plantVisual.transform.localScale = new Vector3(w, h, w);
            _plantVisual.transform.localPosition = new Vector3(0f, 0.06f + h * 0.5f, 0f);
            if (_plantRenderer != null && _plantRenderer.material != null)
            {
                Color c = ready
                    ? ReadyColor * (0.85f + 0.15f * Mathf.Sin(Time.time * 5f)) // ready shimmer
                    : Color.Lerp(SproutColor * 0.7f, SproutColor, growth);
                if (_plantRenderer.material.HasProperty("_BaseColor")) _plantRenderer.material.SetColor("_BaseColor", c);
                else _plantRenderer.material.color = c;
            }
        }

        private void SetReadout(string text)
        {
            if (_readout != null) _readout.text = text;
        }

        private static string FormatTime(long seconds)
        {
            if (seconds >= 3600) return (seconds / 3600) + "h " + ((seconds % 3600) / 60) + "m";
            if (seconds >= 60) return (seconds / 60) + "m " + (seconds % 60) + "s";
            return seconds + "s";
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
