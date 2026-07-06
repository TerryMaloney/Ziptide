using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE II P3 — the SKINNING CORE. Turns a <see cref="ForgeCreatureBody"/> into one
    /// rigid-weighted skinned mesh + its bone skeleton: root bone carries the core parts, each
    /// limb segment carries its own bone (placed at the segment's UPPER joint, +Y down the chain,
    /// so the P4 motor rotates a knee by rotating one bone). Every vertex is 100% one bone
    /// (rigid weights — reads crisp, skins cheap). Deterministic: same body → identical vertices,
    /// weights, and bind poses. THIS FILE IS THE HARD MATH — wiring it to CreatureBehaviorBase
    /// is the easy follow-up (HANDOFF jjjj envelope).
    ///
    /// Bind-pose law (the classic skinning trap, spelled out): mesh vertices are authored in
    /// ROOT-LOCAL space at build pose; bindpose[i] = bones[i].worldToLocalMatrix at that pose
    /// (root at identity), so bone-space verts = bindpose * vertex. Move/rotate a bone afterwards
    /// and exactly its verts follow. Never recompute bindposes after posing.
    /// </summary>
    public static class ForgeSkinnedBuilder
    {
        public struct Result
        {
            public Mesh mesh;              // vertices in root space, boneWeights + bindposes set
            public Transform[] bones;      // [0] = root; then limb segments in declaration order
            public GameObject skeletonRoot;// owns the bone hierarchy; caller parents + adds SMR
            public int[] paletteSlots;     // palette slot per submesh — materials in this order
        }

        /// <summary>Build mesh + skeleton. Caller owns the returned skeletonRoot GameObject.</summary>
        public static Result Build(ForgeCreatureBody body)
        {
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            // One submesh per used palette slot (sorted), so each slot gets its own material —
            // the same contract ForgeMesh.Build gives recipes.
            var trisBySlot = new SortedDictionary<int, List<int>>();
            var weights = new List<BoneWeight>();

            // ── Skeleton ────────────────────────────────────────────────────
            var rootGo = new GameObject("Skeleton_" + (body != null ? body.bodyId : "null"));
            var bones = new List<Transform> { rootGo.transform };

            // ── Synthetic recipe = every part, so ForgeUV can allocate islands the normal way ──
            var parts = new List<ForgePart>();
            var partBone = new List<int>();   // parallel: which bone owns each part
            var partPose = new List<Matrix4x4>(); // world (=root-local) pose per part

            if (body != null && body.coreParts != null)
                foreach (var p in body.coreParts)
                {
                    if (p == null) continue;
                    parts.Add(p);
                    partBone.Add(0);
                    partPose.Add(Matrix4x4.TRS(p.position, Quaternion.Euler(p.eulerRotation),
                        p.scale == Vector3.zero ? Vector3.one : p.scale));
                }

            if (body != null && body.limbs != null)
                foreach (var limb in body.limbs)
                {
                    if (limb == null || limb.segments == null) continue;
                    BuildLimbChain(limb, mirrored: false, rootGo.transform, bones, parts, partBone, partPose);
                    if (limb.mirrorX)
                        BuildLimbChain(limb, mirrored: true, rootGo.transform, bones, parts, partBone, partPose);
                }

            // The eye: a small emissive dome on the core (bone 0).
            if (body != null && body.eyeRadius > 0.001f)
            {
                var eye = new ForgePart
                {
                    name = "Eye", op = ForgeOp.SphereSection, bevel = 1f, segments = 8, smooth = true,
                    size = Vector3.one * (body.eyeRadius * 2f), position = body.eyeLocal,
                    paletteSlot = body.eyePaletteSlot
                };
                parts.Add(eye);
                partBone.Add(0);
                partPose.Add(Matrix4x4.TRS(eye.position, Quaternion.identity, Vector3.one));
            }

            // Island allocation reuses the proven E1.1 packer via a synthetic recipe.
            var synthetic = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            synthetic.recipeId = body != null ? body.bodyId : "body";
            synthetic.parts = parts.ToArray();
            var islands = ForgeUV.ComputeIslands(synthetic);
            var islandByPart = new Dictionary<int, Rect>();
            foreach (var isl in islands) islandByPart[isl.partIndex] = isl.rect;

            // ── Geometry: exploded flat shading (crisp chitin read), rigid weight per part ──
            for (int pi = 0; pi < parts.Count; pi++)
            {
                var part = parts[pi];
                var local = ForgeMesh.BuildPart(part);
                var proj = ForgeUV.ProjectionFor(part.op);
                Bounds lb = LocalBounds(local);
                Rect island = islandByPart.TryGetValue(pi, out var r) ? r : new Rect(0f, 0f, 1f, 1f);
                Matrix4x4 pose = partPose[pi];
                var bw = new BoneWeight { boneIndex0 = partBone[pi], weight0 = 1f };
                if (!trisBySlot.TryGetValue(part.paletteSlot, out var tris))
                    trisBySlot[part.paletteSlot] = tris = new List<int>();

                for (int t = 0; t < local.triangles.Count; t += 3)
                {
                    int a = local.triangles[t], b = local.triangles[t + 1], c = local.triangles[t + 2];
                    Vector3 la = local.vertices[a], lbv = local.vertices[b], lc = local.vertices[c];
                    Vector3 localFn = FaceNormal(la, lbv, lc);
                    ForgeUV.ProjectTriangle(proj, la, lbv, lc, localFn, lb, out var ua, out var ub, out var uc);

                    Vector3 va = pose.MultiplyPoint3x4(la);
                    Vector3 vb = pose.MultiplyPoint3x4(lbv);
                    Vector3 vc = pose.MultiplyPoint3x4(lc);
                    Vector3 fn = FaceNormal(va, vb, vc);

                    int i0 = verts.Count;
                    verts.Add(va); verts.Add(vb); verts.Add(vc);
                    normals.Add(fn); normals.Add(fn); normals.Add(fn);
                    uvs.Add(ForgeUV.ToAtlas(ua, island));
                    uvs.Add(ForgeUV.ToAtlas(ub, island));
                    uvs.Add(ForgeUV.ToAtlas(uc, island));
                    weights.Add(bw); weights.Add(bw); weights.Add(bw);
                    tris.Add(i0); tris.Add(i0 + 1); tris.Add(i0 + 2);
                }
            }

            Object.DestroyImmediate(synthetic);

            // ── Mesh assembly + bind poses ───────────────────────────────────
            var mesh = new Mesh { name = "ForgeBody_" + (body != null ? body.bodyId : "null") };
            if (verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            var paletteSlots = new int[Mathf.Max(1, trisBySlot.Count)];
            mesh.subMeshCount = paletteSlots.Length;
            int sub = 0;
            foreach (var kv in trisBySlot) // SortedDictionary — deterministic slot order
            {
                paletteSlots[sub] = kv.Key;
                mesh.SetTriangles(kv.Value, sub);
                sub++;
            }
            mesh.boneWeights = weights.ToArray();

            var bindposes = new Matrix4x4[bones.Count];
            for (int i = 0; i < bones.Count; i++)
                bindposes[i] = bones[i].worldToLocalMatrix * rootGo.transform.localToWorldMatrix;
            mesh.bindposes = bindposes;
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            return new Result { mesh = mesh, bones = bones.ToArray(), skeletonRoot = rootGo,
                paletteSlots = paletteSlots };
        }

        /// <summary>One chain: bones at each joint (+Y along the chain), a box segment per bone.</summary>
        private static void BuildLimbChain(ForgeLimb limb, bool mirrored, Transform root,
            List<Transform> bones, List<ForgePart> parts, List<int> partBone, List<Matrix4x4> partPose)
        {
            Vector3 dir = limb.chainDirection.sqrMagnitude > 1e-6f ? limb.chainDirection.normalized : Vector3.down;
            Vector3 attach = limb.attachLocal;
            if (mirrored) { dir.x = -dir.x; attach.x = -attach.x; }
            // Bone +Y points ALONG dir (down the chain) — the P4 motor's contract.
            Quaternion chainRot = Quaternion.FromToRotation(Vector3.up, dir);

            Vector3 joint = attach;
            Transform parent = root;
            string tag = limb.name + (mirrored ? "_M" : "");
            for (int s = 0; s < limb.segments.Length; s++)
            {
                var seg = limb.segments[s];
                float len = Mathf.Max(0.01f, seg.size.y);

                var boneGo = new GameObject("Bone_" + tag + "_" + s);
                boneGo.transform.SetParent(parent, false);
                boneGo.transform.position = joint;          // world == root-local (root at identity)
                boneGo.transform.rotation = chainRot;
                bones.Add(boneGo.transform);
                parent = boneGo.transform;

                Vector3 center = joint + dir * (len * 0.5f);
                parts.Add(new ForgePart
                {
                    name = tag + "_Seg" + s, op = ForgeOp.BeveledBox, bevel = 0.004f,
                    size = seg.size, paletteSlot = seg.paletteSlot
                    // position/rotation live in partPose, not the part (the part is LOCAL geometry)
                });
                partBone.Add(bones.Count - 1);
                partPose.Add(Matrix4x4.TRS(center, chainRot, Vector3.one));

                joint += dir * len;
            }
        }

        private static Bounds LocalBounds(ForgeMesh.PartGeometry g)
        {
            if (g.vertices.Count == 0) return new Bounds(Vector3.zero, Vector3.one * 0.001f);
            var b = new Bounds(g.vertices[0], Vector3.zero);
            for (int i = 1; i < g.vertices.Count; i++) b.Encapsulate(g.vertices[i]);
            return b;
        }

        private static Vector3 FaceNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 n = Vector3.Cross(b - a, c - a);
            return n.sqrMagnitude > 1e-14f ? n.normalized : Vector3.up;
        }
    }
}
