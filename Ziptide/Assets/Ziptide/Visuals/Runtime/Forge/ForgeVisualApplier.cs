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
    /// forgeRecipeId; also snaps existing socket-named children (Grip/Muzzle) to the recipe's poses,
    /// which carries the +45° Quest grip tilt (ASSET_SWAP_PIPELINE.md §4).
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

            var existing = item.transform.Find(VisualChildName);
            GameObject vis = existing != null ? existing.gameObject : new GameObject(VisualChildName);
            vis.transform.SetParent(item.transform, false);
            vis.transform.localPosition = Vector3.zero;
            vis.transform.localRotation = Quaternion.identity;
            vis.transform.localScale = Vector3.one;

            // E1.4: prefer the BAKED look (single textured material, build-time ForgeBaker output).
            // The runtime flat-color mesh stays as the dev fallback when no bake shipped.
            var baked = Resources.Load<GameObject>("ForgeBaked/" + recipeId + "/prefab");
            int tris;
            if (baked != null)
            {
                for (int i = vis.transform.childCount - 1; i >= 0; i--)
                    Object.Destroy(vis.transform.GetChild(i).gameObject);
                var oldMf = vis.GetComponent<MeshFilter>();
                if (oldMf != null) Object.Destroy(oldMf);
                var oldMr = vis.GetComponent<MeshRenderer>();
                if (oldMr != null) Object.Destroy(oldMr);
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
                mr.sharedMaterials = ForgeMaterials.ForRecipe(recipe);
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Quest budget
                tris = mesh.triangles.Length / 3;
            }

            // Snap existing socket-named children (Grip = XR attach, Muzzle = ray origin) to the
            // recipe's poses so the generated shape and the interaction points agree.
            if (recipe.sockets != null)
                foreach (var s in recipe.sockets)
                {
                    if (s == null || string.IsNullOrEmpty(s.name)) continue;
                    var child = item.transform.Find(s.name);
                    if (child == null) continue;
                    child.localPosition = s.localPosition;
                    child.localRotation = Quaternion.Euler(s.localEuler);
                }

            Debug.Log("ZIPTIDE: FORGE_APPLIED id=" + recipeId + " item=" + item.name
                + " baked=" + (baked != null) + " tris=" + tris);
            return true;
        }
    }
}
