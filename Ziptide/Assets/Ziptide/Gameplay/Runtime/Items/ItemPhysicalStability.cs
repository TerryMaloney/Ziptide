using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Fits an item's root BoxCollider to its visible mesh hierarchy after runtime construction is complete.
    /// ItemFactory creates primitive roots first and ForgeVisualApplier may then replace the look with a
    /// metre-authored hierarchy whose bounds do not match the old primitive shell. A small physical skin
    /// keeps the visible item above walkable surfaces instead of letting the mesh settle through the floor.
    /// LineRenderer aim rays and TextMesh labels are intentionally excluded.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ItemPhysicalStability : MonoBehaviour
    {
        private const float SupportPaddingWorld = 0.012f;

        private void Start()
        {
            FitNow(gameObject);
        }

        /// <summary>Re-fit now; public so EditMode regression tests can prove the runtime calculation.</summary>
        public static bool FitNow(GameObject root)
        {
            if (root == null) return false;
            var box = root.GetComponent<BoxCollider>();
            if (box == null) return false;

            var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            Bounds localBounds = default;
            bool found = false;

            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;

                Bounds world = renderer.bounds;
                Vector3 min = world.min;
                Vector3 max = world.max;
                for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                {
                    Vector3 worldCorner = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z);
                    Vector3 localCorner = root.transform.InverseTransformPoint(worldCorner);
                    if (!found)
                    {
                        localBounds = new Bounds(localCorner, Vector3.zero);
                        found = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(localCorner);
                    }
                }
            }

            if (!found) return false;

            Vector3 scale = root.transform.lossyScale;
            Vector3 padding = new Vector3(
                SupportPaddingWorld / Mathf.Max(0.0001f, Mathf.Abs(scale.x)),
                SupportPaddingWorld / Mathf.Max(0.0001f, Mathf.Abs(scale.y)),
                SupportPaddingWorld / Mathf.Max(0.0001f, Mathf.Abs(scale.z)));

            box.center = localBounds.center;
            box.size = new Vector3(
                Mathf.Max(0.01f, localBounds.size.x + padding.x * 2f),
                Mathf.Max(0.01f, localBounds.size.y + padding.y * 2f),
                Mathf.Max(0.01f, localBounds.size.z + padding.z * 2f));
            return true;
        }
    }
}
