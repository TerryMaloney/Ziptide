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

        private void Awake()
        {
            if (!ForgeVisualApplier.TryApply(gameObject, recipeId)) return;

            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                if (r.transform == transform) continue;
                if (IsKept(r.transform)) continue;
                r.enabled = false; // renderer only — colliders on primitive pieces keep working
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
