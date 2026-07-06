using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE II P3+P4 consumption seam for CREATURES — what <see cref="ForgeVisualApplier"/> is to
    /// items. If an authored <see cref="ForgeCreatureBody"/> exists at Resources/Forge/Bodies/&lt;id&gt;,
    /// this builds the skinned walking body under a "ForgeVisual" child (skeleton + one
    /// SkinnedMeshRenderer with per-palette-slot materials, eye slot emissive) and binds a
    /// <see cref="ForgeCreatureAnimator"/> so the gait follows the host's own movement. A LOOK,
    /// never a stat: colliders, CreatureRuntime, and behaviors are untouched. Missing body =
    /// graceful false (most creatures aren't forged yet — the caller keeps its primitives).
    /// Logs FORGE_CREATURE_APPLIED.
    /// </summary>
    public static class ForgeCreatureVisualApplier
    {
        public const string VisualChildName = "ForgeVisual";
        private static readonly Dictionary<Color, Material> EmissiveCache = new Dictionary<Color, Material>();

        public static bool TryApply(GameObject host, string creatureId)
        {
            if (host == null || string.IsNullOrEmpty(creatureId)) return false;

            var body = Resources.Load<ForgeCreatureBody>("Forge/Bodies/" + creatureId);
            if (body == null) return false; // not forged yet — primitives carry the look

            var existing = host.transform.Find(VisualChildName);
            if (existing != null) Object.Destroy(existing.gameObject);

            var vis = new GameObject(VisualChildName);
            vis.transform.SetParent(host.transform, false);

            var r = ForgeSkinnedBuilder.Build(body);
            r.skeletonRoot.transform.SetParent(vis.transform, false);

            var smr = vis.AddComponent<SkinnedMeshRenderer>();
            smr.bones = r.bones;
            smr.sharedMesh = r.mesh;
            smr.rootBone = r.bones[0];
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Quest budget

            var mats = new Material[r.paletteSlots.Length];
            for (int i = 0; i < r.paletteSlots.Length; i++)
            {
                int slot = r.paletteSlots[i];
                Color c = body.palette != null && slot < body.palette.Length
                    ? body.palette[slot]
                    : Color.magenta; // loud fallback — Validate() should have caught this
                mats[i] = slot == body.eyePaletteSlot ? Emissive(c) : ForgeMaterials.Mat(c);
            }
            smr.sharedMaterials = mats;

            vis.AddComponent<ForgeCreatureAnimator>().Bind(body, r.bones);

            Debug.Log("ZIPTIDE: FORGE_CREATURE_APPLIED id=" + creatureId
                + " bones=" + r.bones.Length + " tris=" + r.mesh.triangles.Length / 3);
            return true;
        }

        /// <summary>The eye must GLOW — a shared emissive variant per color (small cache; eyes
        /// reuse palette colors across a family).</summary>
        private static Material Emissive(Color c)
        {
            if (EmissiveCache.TryGetValue(c, out var cached) && cached != null) return cached;
            var baseMat = ForgeMaterials.Mat(c);
            if (baseMat == null) return null;
            var mat = new Material(baseMat) { name = baseMat.name + "_Eye" };
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", c * 2.2f);
            EmissiveCache[c] = mat;
            return mat;
        }
    }
}
