using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.3c / BUILDING_INTERIORS — the interior loot cache: a small banded crate that pays
    /// scrap through the ONE economy path (<see cref="RewardRouter"/>, source
    /// <see cref="LedgerSource.Salvage"/>) when grabbed, pops, and despawns. This is why a room is
    /// worth walking into.
    ///
    /// Patch-time contract: InteriorBuilder calls <see cref="Init"/> in the EDITOR — fields and the
    /// visual children serialize into the scene, but interactable wiring can NOT (editor-added
    /// UnityEvent listeners don't survive into play mode), so the grab arms itself at runtime in
    /// Start(). Grab volume exists before the interactable (gotcha #6). The grant lands on the live
    /// SaveSystem profile, which travel-autosave persists.
    /// </summary>
    public class SalvageCacheRuntime : MonoBehaviour
    {
        [Tooltip("Resource paid on open (RewardRouter id).")]
        public string resourceId = "scrap";
        [Tooltip("Amount paid on open.")]
        public double amount = 6;

        private bool _opened;

        /// <summary>Patch-time build (public Init — the no-reflection law): sets the payload and
        /// creates the serialized visual. Safe to skip for runtime spawns — Start() self-heals.</summary>
        public void Init(string resource, double pay)
        {
            if (!string.IsNullOrEmpty(resource)) resourceId = resource;
            if (pay > 0) amount = pay;
            EnsureVisual();
        }

        private void Start()
        {
            EnsureVisual();

            // Arm the grab at RUNTIME (listeners never serialize). Collider precedes interactable.
            var body = transform.Find("CacheBody");
            if (body == null) return;
            if (body.GetComponent<XRSimpleInteractable>() != null) return;
            var grab = body.gameObject.AddComponent<XRSimpleInteractable>();
            grab.selectEntered.AddListener(_ => Open());
        }

        private void EnsureVisual()
        {
            if (transform.Find("CacheBody") != null) return;

            // Body: a squat crate with a glow band so it reads as loot across the room.
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "CacheBody";
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(0.42f, 0.3f, 0.32f);
            body.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            ItemFactory.ApplyURPColor(body, new Color(0.28f, 0.26f, 0.22f));

            var band = GameObject.CreatePrimitive(PrimitiveType.Cube);
            band.name = "GlowBand";
            var bandCol = band.GetComponent<Collider>();
            if (bandCol != null)
            {
                if (Application.isPlaying) Destroy(bandCol);
                else DestroyImmediate(bandCol);
            }
            band.transform.SetParent(transform, false);
            band.transform.localScale = new Vector3(0.44f, 0.055f, 0.34f);
            band.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            ItemFactory.ApplyURPColor(band, new Color(0.35f, 0.95f, 0.75f)); // salvage teal

            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Open()
        {
            if (_opened) return;
            _opened = true;

            double granted = GrantTo(SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,
                resourceId, amount);
            Debug.Log("ZIPTIDE: SALVAGE_CACHE resource=" + resourceId + " amount=" + amount +
                      " granted=" + granted.ToString("F0"));
            StartCoroutine(PopAndDespawn());
        }

        /// <summary>Pure grant seam (EditMode-tested): the cache pays through RewardRouter with the
        /// Salvage source. Null profile = 0 (a cache can never crash a boot).</summary>
        public static double GrantTo(PlayerProfile profile, string resource, double pay)
        {
            if (profile == null || string.IsNullOrEmpty(resource) || pay <= 0) return 0;
            return RewardRouter.Grant(profile, LedgerSource.Salvage, resource, pay,
                reason: "salvage_cache", relatedId: "");
        }

        private IEnumerator PopAndDespawn()
        {
            // A quick scale-punch — the "got it" beat — then gone.
            float t = 0f;
            Vector3 baseScale = transform.localScale;
            while (t < 0.18f)
            {
                t += Time.deltaTime;
                float k = 1f + 0.5f * Mathf.Sin((t / 0.18f) * Mathf.PI);
                transform.localScale = baseScale * k;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
