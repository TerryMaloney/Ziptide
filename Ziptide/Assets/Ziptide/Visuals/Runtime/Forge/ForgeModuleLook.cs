using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// E5.1 — the runtime skin swap for KIT MODULES (walls today), riding the exact item pattern:
    /// the scene ships the structured PRIMITIVE module (safe to serialize, looks right in the editor),
    /// and on device this component swaps in the recipe's baked textured mesh via
    /// <see cref="ForgeVisualApplier"/>. Because a kit module's look lives in CHILD renderers (the
    /// applier only parks the root's), this also hides the primitive children on success — except
    /// whitelisted ones (the interior-mapped window Pane, a real shader feature the recipe can't bake).
    /// A look, never a stat: colliders stay on hidden pieces, so walls block exactly as before.
    /// Missing recipe/bake = graceful no-op — the primitive kit simply stays.
    /// </summary>
    public class ForgeModuleLook : MonoBehaviour
    {
        [Tooltip("Forge recipe that skins this module (Resources/Forge/<id>; baked preferred).")]
        public string recipeId = "";

        [Tooltip("Child names whose renderers SURVIVE the swap (e.g. the interior-mapped Pane).")]
        public string[] keepChildren = { "Pane" };

        [Tooltip("FORGE III F3.8 part 2: per-instance brightness jitter (±this, from a position hash) " +
                 "so identical modules differ slightly down a street. MaterialPropertyBlock — batching-safe. " +
                 "0 = off.")]
        [Range(0f, 0.2f)] public float tintJitter = 0.05f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            if (!ForgeVisualApplier.TryApply(gameObject, recipeId)) return;

            // Per-instance tint: a slightly-off-white _BaseColor via property block, so a row of the
            // SAME wall module isn't a copy-paste. MPB keeps the shared baked material batchable.
            if (tintJitter > 0f)
            {
                float j = 1f + (Hash01(transform.position) * 2f - 1f) * tintJitter;
                var tint = new Color(j, j, j, 1f);
                var mpb = new MaterialPropertyBlock();
                foreach (var r in GetComponentsInChildren<Renderer>(true))
                {
                    if (!IsUnderForgeVisual(r.transform)) continue; // only the swapped-in look
                    r.GetPropertyBlock(mpb);
                    mpb.SetColor(BaseColorId, tint);
                    r.SetPropertyBlock(mpb);
                }
            }

            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                if (r.transform == transform) continue;
                if (IsKept(r.transform)) continue;
                r.enabled = false; // renderer only — colliders on primitive pieces keep working
            }
        }

        private static bool IsUnderForgeVisual(Transform t)
        {
            for (var p = t; p != null; p = p.parent)
                if (p.name == ForgeVisualApplier.VisualChildName) return true;
            return false;
        }

        /// <summary>Deterministic 0..1 from a world position (quantised so tiny drifts don't flicker).</summary>
        private static float Hash01(Vector3 p)
        {
            unchecked
            {
                int x = Mathf.RoundToInt(p.x * 4f), y = Mathf.RoundToInt(p.y * 4f), z = Mathf.RoundToInt(p.z * 4f);
                uint h = (uint)(x * 374761393 + y * 668265263 + z * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177u;
                return ((h ^ (h >> 16)) & 0xFFFFFF) / (float)0x1000000;
            }
        }

        private bool IsKept(Transform t)
        {
            // Anything under the applier's ForgeVisual child IS the new look; whitelisted names
            // (and their subtrees) survive too.
            for (var p = t; p != null && p != transform.parent; p = p.parent)
            {
                if (p.name == ForgeVisualApplier.VisualChildName) return true;
                if (keepChildren != null)
                    foreach (var k in keepChildren)
                        if (!string.IsNullOrEmpty(k) && p.name == k) return true;
                if (p == transform) break;
            }
            return false;
        }
    }
}
