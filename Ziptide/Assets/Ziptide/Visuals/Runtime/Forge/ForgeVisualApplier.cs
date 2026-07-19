using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Runtime consumption seam for Forge assets: swaps an item's primitive LOOK for its generated
    /// mesh. A look, never a stat — colliders, Rigidbody, and grab components are untouched (same law
    /// as cosmetics), and a missing recipe is a graceful no-op (the primitive stays, warning logged;
    /// the audit's FORGE_RECIPE_MISSING blocker catches it at build time). Meshes build once per
    /// recipeId and are cached — zero committed binaries, zero per-frame allocations (SkyVista's
    /// bake-on-entry contract, in 3D). Called by ItemFactory.Create when a definition carries a
    /// forgeRecipeId; scene-authored ItemRuntime instances also reapply their serialized definition at
    /// Awake so stale build-generated visuals cannot survive into a source-only PlayMode checkout.
    ///
    /// IMPORTANT SIZE CONTRACT: ItemFactory's primitive root scale is the intended collider/body size.
    /// A Forge recipe is already authored in Unity metres. Leaving that small non-uniform primitive
    /// scale on the root scales the forged mesh a second time and produces centimetre-sized weapons.
    /// Before mounting the Forge look, this class transfers the old root scale into the BoxCollider and
    /// restores a unit-scale item root. Reapplication is idempotent.
    ///
    /// IMPORTANT AIM CONTRACT: every handheld item's authored forward axis is local +Z. Forge sockets
    /// may position the Grip, but may not pitch it away from the controller's forward axis. The prior
    /// +45 degree socket rotated the item -45 degrees in-hand and made the muzzle point into the sky.
    /// </summary>
    public static class ForgeVisualApplier
    {
        public const string VisualChildName = "ForgeVisual";
        private static readonly Dictionary<string, Mesh> MeshCache = new Dictionary<string, Mesh>();

        public static bool TryApply(GameObject item, string recipeId)
        {
            if (item == null || string.IsNullOrEmpty(recipeId)) return false;

            var recipe = Resources.Load<ForgeRecipeDefinition>("Forge/" + recipeId);
            if (recipe == null)
            {
                Debug.LogWarning("ZIPTIDE: FORGE_RECIPE_NOT_FOUND id=" + recipeId + " item=" + item.name);
                return false;
            }

            // Park the primitive look (root renderer only — children like rails/indicators keep theirs).
            var rootRenderer = item.GetComponent<MeshRenderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;

            bool normalizedScale = NormalizeItemRootScale(item, out Vector3 colliderSize);

            var existing = item.transform.Find(VisualChildName);
            GameObject vis = existing != null ? existing.gameObject : new GameObject(VisualChildName);
            vis.transform.SetParent(item.transform, false);
            vis.transform.localPosition = Vector3.zero;
            vis.transform.localRotation = Quaternion.identity;
            vis.transform.localScale = Vector3.one;

            // ForgeVisual owns its entire child hierarchy. Build patching can serialize a baked
            // prefab(Clone) whose material lives under the regenerated/gitignored ForgeBaked tree.
            // A source-only PlayMode checkout then resolves that old child renderer with a null material.
            // Clear every previously-owned child before choosing the currently available baked/fallback
            // representation. Deactivate first so deferred runtime destruction cannot expose one stale
            // frame to rendering or the R1.9 surface audit.
            int removedChildren = ClearOwnedChildren(vis.transform);

            // E1.4: prefer the BAKED look (single textured material, build-time ForgeBaker output).
            // The runtime flat-color mesh stays as the dev fallback when no bake shipped.
            var baked = Resources.Load<GameObject>("ForgeBaked/" + recipeId + "/prefab");
            int tris;
            if (baked != null)
            {
                RemoveFallbackComponents(vis);
                var look = Object.Instantiate(baked, vis.transform, false);
                var lookMf = look.GetComponent<MeshFilter>();
                tris = lookMf != null && lookMf.sharedMesh != null ? lookMf.sharedMesh.triangles.Length / 3 : 0;
            }
            else
            {
                if (!MeshCache.TryGetValue(recipeId, out var mesh) || mesh == null)
                {
                    mesh = ForgeMesh.Build(recipe);
                    MeshCache[recipeId] = mesh;
                }
                var mf = vis.GetComponent<MeshFilter>();
                if (mf == null) mf = vis.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;

                var mr = vis.GetComponent<MeshRenderer>();
                if (mr == null) mr = vis.AddComponent<MeshRenderer>();
                mr.enabled = true;
                mr.sharedMaterials = ForgeMaterials.ForRecipe(recipe);
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Quest budget
                tris = mesh.triangles.Length / 3;
            }

            // Snap existing socket-named children (Grip = XR attach, Muzzle = ray origin) to the
            // recipe's positions so the generated shape and interaction points agree. Grip rotation is
            // canonical identity: item local +Z must match the controller/interactor forward direction.
            if (recipe.sockets != null)
                foreach (var s in recipe.sockets)
                {
                    if (s == null || string.IsNullOrEmpty(s.name)) continue;
                    var child = item.transform.Find(s.name);
                    if (child == null) continue;
                    child.localPosition = s.localPosition;
                    child.localRotation = s.name == "Grip"
                        ? Quaternion.identity
                        : Quaternion.Euler(s.localEuler);
                }

            Debug.Log("ZIPTIDE: FORGE_APPLIED id=" + recipeId + " item=" + item.name
                + " baked=" + (baked != null) + " tris=" + tris
                + " replacedChildren=" + removedChildren
                + " normalizedScale=" + normalizedScale
                + " collider=" + colliderSize.ToString("F3"));
            return true;
        }

        /// <summary>
        /// Move ItemFactory's primitive dimensions from Transform scale into the root BoxCollider, then
        /// restore a unit-scale root so metre-authored Forge geometry is not scaled twice. A previously
        /// normalized item has unit root scale plus a non-unit collider and is left unchanged.
        /// </summary>
        private static bool NormalizeItemRootScale(GameObject item, out Vector3 colliderSize)
        {
            var box = item.GetComponent<BoxCollider>();
            Vector3 rootScale = item.transform.localScale;
            bool rootIsUnit = Approximately(rootScale, Vector3.one);
            bool colliderAlreadyCarriesSize = box != null && !Approximately(box.size, Vector3.one);

            if (rootIsUnit && colliderAlreadyCarriesSize)
            {
                colliderSize = box.size;
                return false;
            }

            colliderSize = PositiveSize(rootScale);
            if (box != null) box.size = colliderSize;
            item.transform.localScale = Vector3.one;
            return !rootIsUnit;
        }

        private static Vector3 PositiveSize(Vector3 value)
        {
            return new Vector3(
                Mathf.Max(0.01f, Mathf.Abs(value.x)),
                Mathf.Max(0.01f, Mathf.Abs(value.y)),
                Mathf.Max(0.01f, Mathf.Abs(value.z)));
        }

        private static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) < 0.0001f
                && Mathf.Abs(a.y - b.y) < 0.0001f
                && Mathf.Abs(a.z - b.z) < 0.0001f;
        }

        private static int ClearOwnedChildren(Transform visualRoot)
        {
            int removed = visualRoot.childCount;
            for (int i = visualRoot.childCount - 1; i >= 0; i--)
            {
                GameObject child = visualRoot.GetChild(i).gameObject;
                child.SetActive(false);
                DestroyOwned(child);
            }
            return removed;
        }

        private static void RemoveFallbackComponents(GameObject visualRoot)
        {
            var oldMr = visualRoot.GetComponent<MeshRenderer>();
            if (oldMr != null)
            {
                oldMr.enabled = false;
                DestroyOwned(oldMr);
            }
            var oldMf = visualRoot.GetComponent<MeshFilter>();
            if (oldMf != null) DestroyOwned(oldMf);
        }

        private static void DestroyOwned(Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Object.Destroy(value);
            else Object.DestroyImmediate(value);
        }
    }
}
