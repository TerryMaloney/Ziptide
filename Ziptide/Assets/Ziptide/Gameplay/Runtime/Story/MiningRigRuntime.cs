using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A placed extractor made VISIBLE (GAME_PLAN M2): binds a <see cref="MineState"/> in this world's
    /// <see cref="WorldState"/> — the existing idle backend (`ProfileEconomy` already resolves offline
    /// accrual on entry, ECON_RESOLVE) — accrues live while you watch, shows the stored amount, and
    /// pays out to the profile when you select the hopper (the credits/resources HUD moves in front of
    /// you). Spawned by JobDirector from <see cref="MineSpawnDefinition"/> pack data.
    /// Logs ZIPTIDE: MINE_COLLECT id=… amt=…
    /// </summary>
    public class MiningRigRuntime : MonoBehaviour
    {
        private static readonly Color BodyColor = new Color(0.20f, 0.22f, 0.26f);
        private static readonly Color HopperColor = new Color(0.30f, 0.55f, 0.45f);
        private static readonly Color HopperFlash = new Color(0.55f, 0.95f, 0.75f);

        private MineSpawnDefinition _def;
        private string _worldId;
        private MineState _mine;
        private TextMesh _readout;
        private Renderer _hopper;
        private float _flashUntil;

        /// <summary>Build + bind. Called by the spawner right after AddComponent (runtime only).</summary>
        public void Init(MineSpawnDefinition def, string worldId)
        {
            _def = def ?? new MineSpawnDefinition();
            _worldId = worldId;
            Build();
            BindMineState();
        }

        private void BindMineState()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return;
            var world = profile.GetWorld(_worldId, createIfMissing: true);
            _mine = world.mines.Find(m => m != null && m.machineId == _def.id);
            if (_mine == null)
            {
                _mine = new MineState
                {
                    machineId = _def.id,
                    resourceId = _def.resourceId,
                    ratePerSecond = _def.ratePerSecond,
                    storageCap = _def.storageCap,
                    stored = 0,
                    lastResolvedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                world.mines.Add(_mine);
            }
            else
            {
                // The definition is the source of truth for TUNING; the save keeps only progress.
                _mine.resourceId = _def.resourceId;
                _mine.ratePerSecond = _def.ratePerSecond;
                _mine.storageCap = _def.storageCap;
            }
        }

        private void Build()
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "RigBody";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            body.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            Paint(body, BodyColor);

            var drill = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            drill.name = "Drill";
            var dc = drill.GetComponent<Collider>(); if (dc != null) Destroy(dc);
            drill.transform.SetParent(transform, false);
            drill.transform.localPosition = new Vector3(0f, 0.25f, 0.5f);
            drill.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            drill.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            Paint(drill, HopperColor);

            // The hopper — select it to collect the stored yield.
            var hopper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hopper.name = "Hopper";
            hopper.transform.SetParent(transform, false);
            hopper.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            hopper.transform.localScale = new Vector3(0.55f, 0.3f, 0.55f);
            Paint(hopper, HopperColor);
            _hopper = hopper.GetComponent<Renderer>();
            var interactable = hopper.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => Collect());

            var readoutGo = new GameObject("Readout");
            _readout = readoutGo.AddComponent<TextMesh>();
            _readout.characterSize = 0.03f;
            _readout.fontSize = 48;
            _readout.anchor = TextAnchor.MiddleCenter;
            _readout.alignment = TextAlignment.Center;
            _readout.color = HopperFlash;
            readoutGo.transform.SetParent(transform, false);
            readoutGo.transform.localPosition = new Vector3(0f, 1.8f, 0f);
        }

        private void Update()
        {
            // Live accrual while present (idle accrual across sessions is ProfileEconomy's job).
            if (_mine == null) BindMineState();
            if (_mine != null)
            {
                _mine.stored += _mine.ratePerSecond * Time.deltaTime;
                if (_mine.storageCap > 0 && _mine.stored > _mine.storageCap) _mine.stored = _mine.storageCap;
                _mine.lastResolvedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (_readout != null)
                {
                    _readout.text = _def.resourceId.Replace('_', ' ') + "\n"
                        + System.Math.Floor(_mine.stored) + (_mine.storageCap > 0 ? " / " + _mine.storageCap : "")
                        + "\n< select hopper to collect >";
                }
            }

            var cam = Camera.main;
            if (cam != null && _readout != null)
                _readout.transform.rotation = Quaternion.LookRotation(_readout.transform.position - cam.transform.position);

            if (_hopper != null && _flashUntil > 0f && Time.time > _flashUntil)
            {
                _flashUntil = 0f;
                Tint(_hopper, HopperColor);
            }
        }

        private void Collect()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || _mine == null) return;
            double amt = ProfileEconomy.CollectMine(profile, _mine);
            if (amt <= 0) return;
            if (_hopper != null) { Tint(_hopper, HopperFlash); _flashUntil = Time.time + 0.4f; }
            Debug.Log("ZIPTIDE: MINE_COLLECT id=" + _def.id + " amt=" + System.Math.Floor(amt) +
                      " resource=" + _def.resourceId);
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

        private static void Tint(Renderer r, Color color)
        {
            if (r == null || r.material == null) return;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
            else if (r.material.HasProperty("_Color")) r.material.color = color;
        }
    }
}
