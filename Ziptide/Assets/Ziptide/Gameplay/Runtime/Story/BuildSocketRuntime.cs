using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// MECHANICAL BUILDING v1 (Quality Bar P3 — the tycoon seed). A build socket on the MachineSite
    /// plinth: select it, pay the credit cost, and a REAL extractor rig rises on the spot — a live
    /// <see cref="MiningRigRuntime"/> bound to a MineState in this world's save, exactly like an
    /// authored mine. Leave the world, come back: your machine is still there, still producing
    /// (ProfileEconomy resolves idle accrual). Spawned by JobDirector from
    /// <see cref="BuildSocketSpawnDefinition"/> pack data.
    /// Logs ZIPTIDE: SOCKET_BUILD id=… cost=… / SOCKET_BLOCKED reason=poor.
    /// Free-form grid building is post-launch; the persistence + pay-to-place loop lands here.
    /// </summary>
    public class BuildSocketRuntime : MonoBehaviour
    {
        private static readonly Color FrameColor = new Color(0.25f, 0.28f, 0.34f);
        private static readonly Color HoloColor = new Color(0.35f, 0.75f, 0.95f);
        private static readonly Color TextColor = new Color(0.70f, 0.90f, 1f);

        private BuildSocketSpawnDefinition _def;
        private string _worldId;
        private GameObject _holo;
        private TextMesh _readout;
        private bool _built;

        /// <summary>Build + bind. Called by JobDirector right after AddComponent (runtime only).</summary>
        public void Init(BuildSocketSpawnDefinition def, string worldId)
        {
            _def = def ?? new BuildSocketSpawnDefinition();
            _worldId = worldId;
            BuildVisuals();

            // Already built on a previous visit? The MineState is the truth — raise the rig again.
            var world = World();
            if (world != null && world.mines != null &&
                world.mines.Find(m => m != null && m.machineId == _def.id) != null)
                RaiseRig();
        }

        private WorldState World()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            return profile != null ? profile.GetWorld(_worldId, createIfMissing: true) : null;
        }

        private void BuildVisuals()
        {
            // Socket frame — the selectable base.
            var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "SocketFrame";
            frame.transform.SetParent(transform, false);
            frame.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            frame.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            Paint(frame, FrameColor);
            var interactable = frame.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => TryBuild());

            // Hologram ghost of the machine-to-be — reads as "something goes here".
            _holo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _holo.name = "Holo";
            var hc = _holo.GetComponent<Collider>();
            if (hc != null) Destroy(hc);
            _holo.transform.SetParent(transform, false);
            _holo.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            _holo.transform.localScale = new Vector3(0.7f, 1.1f, 0.7f);
            Paint(_holo, HoloColor * 0.5f);

            var readoutGo = new GameObject("Readout");
            _readout = readoutGo.AddComponent<TextMesh>();
            _readout.characterSize = 0.025f;
            _readout.fontSize = 48;
            _readout.anchor = TextAnchor.MiddleCenter;
            _readout.alignment = TextAlignment.Center;
            _readout.color = TextColor;
            readoutGo.transform.SetParent(transform, false);
            readoutGo.transform.localPosition = new Vector3(0f, 1.8f, 0f);
        }

        private void TryBuild()
        {
            if (_built) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            var world = World();
            if (profile == null || world == null) return;

            if (profile.GetResource("credits") < _def.buildCost)
            {
                Debug.Log("ZIPTIDE: SOCKET_BLOCKED id=" + _def.id + " reason=poor need=" + _def.buildCost);
                if (_readout != null) _readout.text = "NEED " + _def.buildCost + " CR";
                return;
            }

            RewardRouter.TrySpend(profile, LedgerSource.UpgradeCost, "credits", _def.buildCost,
                reason: "build_" + _def.id, worldId: _worldId);
            // The built machine IS a mine save-entry — MiningRigRuntime.Init binds/creates it and the
            // idle economy picks it up like any authored extractor.
            RaiseRig();
            Debug.Log("ZIPTIDE: SOCKET_BUILD id=" + _def.id + " cost=" + _def.buildCost +
                      " resource=" + _def.resourceId);
        }

        private void RaiseRig()
        {
            _built = true;
            if (_holo != null) _holo.SetActive(false);

            var rigGo = new GameObject("BuiltRig_" + _def.id);
            rigGo.transform.SetParent(transform, false);
            rigGo.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            rigGo.AddComponent<MiningRigRuntime>().Init(new MineSpawnDefinition
            {
                id = _def.id,
                resourceId = _def.resourceId,
                ratePerSecond = _def.ratePerSecond,
                storageCap = _def.storageCap,
                localPosition = Vector3.zero
            }, _worldId);
        }

        private void Update()
        {
            if (_readout == null) return;
            if (_built)
            {
                _readout.text = "";
                return;
            }
            // Idle hologram shimmer + offer text.
            if (_holo != null)
            {
                float pulse = 0.35f + 0.15f * Mathf.Sin(Time.time * 2.2f);
                var r = _holo.GetComponent<Renderer>();
                if (r != null && r.material != null)
                {
                    Color c = HoloColor * pulse;
                    if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", c);
                    else r.material.color = c;
                }
            }
            _readout.text = "BUILD: " + _def.resourceId.Replace('_', ' ') + " extractor\n" +
                            _def.buildCost + " CR\n< select base to build >";
            var cam = Camera.main;
            if (cam != null)
                _readout.transform.rotation = Quaternion.LookRotation(_readout.transform.position - cam.transform.position);
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
