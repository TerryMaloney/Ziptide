using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A single named item waiting to be salvaged out of a wreck — THE FIND
    /// (`docs/design/FIRST_HOUR_DIRECTORS_CUT.md` §2.1).
    ///
    /// Why this is not just a spawned item sitting in the scene: the lane's content root MOVES past
    /// the pilot (the rig never does), so an item authored as a physics object out there would be
    /// dragged through space by its parent. This marker rides the lane like every other prop, and
    /// only materialises the real grabbable when the player is close enough to take it — at which
    /// point it is released into world space, standing still, where they can actually grab it.
    ///
    /// It owns no economy: salvage payment stays with the existing space-salvage loop. This is the
    /// story object, not the payout.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpaceSalvageItemRuntime : MonoBehaviour
    {
        /// <summary>How close the player must get before the find becomes a real object.</summary>
        private const float MaterialiseRange = 6f;
        private const float ScanIntervalSeconds = 0.25f;

        [SerializeField] private string itemId = "artifact_half_a";
        [SerializeField] private string grantsFlag = ZiptideFlags.ARTIFACT_HALF_A;

        private float _nextScanAt;
        private bool _materialised;

        /// <summary>Author entry point (public Init — the no-reflection law).</summary>
        public void Init(string id, string flag = null)
        {
            if (!string.IsNullOrEmpty(id)) itemId = id;
            if (!string.IsNullOrEmpty(flag)) grantsFlag = flag;
        }

        private void Start()
        {
            // Already found on a previous session: do not offer it twice. The flag is the truth, and
            // the item itself travelled home in the player's holster.
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && !string.IsNullOrEmpty(grantsFlag) && profile.HasFlag(grantsFlag))
            {
                _materialised = true;
                Debug.Log("ZIPTIDE: SALVAGE_FIND_ALREADY_TAKEN id=" + itemId);
                return;
            }

            BuildBeacon();
        }

        /// <summary>
        /// A soft glow so the find is discoverable from the course without a HUD marker. The player
        /// should notice something odd out there and choose to go look — being told to is a different,
        /// worse beat.
        /// </summary>
        private void BuildBeacon()
        {
            var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glow.name = "FindGlow";
            glow.transform.SetParent(transform, false);
            glow.transform.localScale = Vector3.one * 0.45f;
            var collider = glow.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = glow.GetComponent<Renderer>();
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) return;

            var colour = new Color(0.45f, 0.85f, 0.82f);
            var material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", colour);
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", colour * 2.2f);
            }
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            if (_materialised || Time.unscaledTime < _nextScanAt) return;
            _nextScanAt = Time.unscaledTime + ScanIntervalSeconds;

            Camera cam = Camera.main;
            if (cam == null) return;
            if (Vector3.Distance(cam.transform.position, transform.position) > MaterialiseRange) return;

            _materialised = true;

            // Released into WORLD space: the lane content is moving, and a grabbable that slides away
            // from the hand while you reach for it is the worst possible version of this moment.
            GameObject item = ItemFactory.Create(itemId, transform.position);
            if (item == null)
            {
                Debug.LogWarning("ZIPTIDE: SALVAGE_FIND_FAILED id=" + itemId
                    + " — no ItemDefinition with that id in Resources/Items.");
                return;
            }

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && !string.IsNullOrEmpty(grantsFlag)) profile.SetFlag(grantsFlag);

            Debug.Log("ZIPTIDE: SALVAGE_FIND id=" + itemId + " flag=" + grantsFlag);

            Transform glow = transform.Find("FindGlow");
            if (glow != null) Destroy(glow.gameObject);
        }
    }
}
