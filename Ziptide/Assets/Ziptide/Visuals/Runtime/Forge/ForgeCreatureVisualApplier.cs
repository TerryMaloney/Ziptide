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

            // CREATURE TEXTURE BAKE: prefer the build-time skin (albedo/normal/wear atlas baked from
            // the SAME synthetic recipe the mesh's UVs came from). The EYE keeps its live emissive
            // material so the ForgeBodyTell channels keep working. Missing bake = flat palette look.
            var baked = Resources.Load<Material>("ForgeBaked/body_" + creatureId + "/material");
            var mats = new Material[r.paletteSlots.Length];
            var baseColors = new Color[r.paletteSlots.Length];
            int eyeIndex = -1;
            for (int i = 0; i < r.paletteSlots.Length; i++)
            {
                int slot = r.paletteSlots[i];
                Color c = body.palette != null && slot < body.palette.Length
                    ? body.palette[slot]
                    : Color.magenta; // loud fallback — Validate() should have caught this
                if (slot == body.eyePaletteSlot) { eyeIndex = i; baseColors[i] = c; mats[i] = Emissive(c); }
                else if (baked != null) { baseColors[i] = Color.white; mats[i] = baked; } // tint base = white over the atlas
                else { baseColors[i] = c; mats[i] = ForgeMaterials.Mat(c); }
            }
            smr.sharedMaterials = mats;

            vis.AddComponent<ForgeCreatureAnimator>().Bind(body, r.bones);
            // The tell bridge: behaviors drive gameplay reads (eye states, freeze tints, husk clones)
            // through this instead of their now-hidden primitive parts.
            vis.AddComponent<ForgeBodyTell>().Init(smr, eyeIndex, baseColors);

            // FORGE III F3.4 — a blob shadow at the creature's base so it reads as standing on the
            // ground, not floating. Parented to the HOST (not the animated visual) so it stays put
            // as the body bobs/walks.
            float footprint = Mathf.Max(r.mesh.bounds.extents.x, r.mesh.bounds.extents.z);
            GroundShadow.Attach(host, footprint * 0.9f);

            Debug.Log("ZIPTIDE: FORGE_CREATURE_APPLIED id=" + creatureId
                + " bones=" + r.bones.Length + " tris=" + r.mesh.triangles.Length / 3
                + " baked=" + (baked != null));
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
