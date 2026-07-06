using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE II P3 skinning-core contract. The one that matters most is
    /// PosingABone_MovesExactlyItsVertices: it CPU-skins vertices through the bindposes
    /// (v' = bone.localToWorld * bindpose * v) and proves the bind-pose law holds — the classic
    /// skinning trap caught in EditMode, before any scene or device is involved.
    /// </summary>
    public class ForgeSkinnedBuilderTests
    {
        private static ForgeCreatureBody SampleBody()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "test_walker";
            b.palette = new[] { new Color(0.1f, 0.1f, 0.12f), new Color(0.4f, 0.3f, 0.2f),
                new Color(0.3f, 0.8f, 0.95f), new Color(1f, 0.3f, 0.2f) };
            b.coreParts = new[]
            {
                new ForgePart { name = "Torso", op = ForgeOp.SphereSection, bevel = 1f, segments = 10,
                    smooth = true, size = new Vector3(0.5f, 0.3f, 0.6f), position = new Vector3(0f, 0.4f, 0f), paletteSlot = 1 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "LegFront", attachLocal = new Vector3(0.2f, 0.35f, 0.15f),
                    chainDirection = new Vector3(0.5f, -1f, 0.1f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.06f, 0.2f, 0.07f), paletteSlot = 0 },
                        new ForgeLimbSegment { size = new Vector3(0.05f, 0.22f, 0.06f), paletteSlot = 0 },
                    }
                },
                new ForgeLimb
                {
                    name = "Tail", attachLocal = new Vector3(0f, 0.4f, -0.3f),
                    chainDirection = new Vector3(0f, 0.1f, -1f), role = GaitRole.Tail,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.05f, 0.18f, 0.05f), paletteSlot = 1 },
                        new ForgeLimbSegment { size = new Vector3(0.035f, 0.16f, 0.035f), paletteSlot = 1 },
                    }
                },
            };
            return b;
        }

        [Test]
        public void Build_IsDeterministic()
        {
            var a = ForgeSkinnedBuilder.Build(SampleBody());
            var b = ForgeSkinnedBuilder.Build(SampleBody());
            try
            {
                CollectionAssert.AreEqual(a.mesh.vertices, b.mesh.vertices, "vertices diverged");
                CollectionAssert.AreEqual(a.mesh.triangles, b.mesh.triangles, "triangles diverged");
                CollectionAssert.AreEqual(a.mesh.bindposes, b.mesh.bindposes, "bindposes diverged");
                Assert.AreEqual(a.mesh.boneWeights.Length, b.mesh.boneWeights.Length);
                for (int i = 0; i < a.mesh.boneWeights.Length; i++)
                    Assert.AreEqual(a.mesh.boneWeights[i].boneIndex0, b.mesh.boneWeights[i].boneIndex0);
            }
            finally
            {
                Object.DestroyImmediate(a.skeletonRoot);
                Object.DestroyImmediate(b.skeletonRoot);
            }
        }

        [Test]
        public void Skeleton_MatchesTheGenome_AndStaysInBudget()
        {
            var body = SampleBody();
            var r = ForgeSkinnedBuilder.Build(body);
            try
            {
                // root + LegFront 2 segs ×2 (mirrored) + Tail 2 segs = 7
                Assert.AreEqual(7, body.BoneCount());
                Assert.AreEqual(body.BoneCount(), r.bones.Length, "bones must match the genome");
                Assert.LessOrEqual(r.bones.Length, ForgeCreatureBody.MaxBones);
                Assert.AreEqual(r.bones.Length, r.mesh.bindposes.Length, "one bindpose per bone");
                Assert.IsEmpty(body.Validate());
            }
            finally { Object.DestroyImmediate(r.skeletonRoot); }
        }

        [Test]
        public void EveryVertex_IsRigidWeighted_ToAValidBone_AndLimbsOwnVerts()
        {
            var r = ForgeSkinnedBuilder.Build(SampleBody());
            try
            {
                var counts = new int[r.bones.Length];
                foreach (var w in r.mesh.boneWeights)
                {
                    Assert.AreEqual(1f, w.weight0, 1e-5f, "rigid weights only");
                    Assert.That(w.boneIndex0, Is.InRange(0, r.bones.Length - 1));
                    counts[w.boneIndex0]++;
                }
                Assert.Greater(counts[0], 0, "core verts on the root bone");
                for (int i = 1; i < counts.Length; i++)
                    Assert.Greater(counts[i], 0, "bone " + i + " owns no vertices — a segment built empty");
            }
            finally { Object.DestroyImmediate(r.skeletonRoot); }
        }

        [Test]
        public void PosingABone_MovesExactlyItsVertices()
        {
            var r = ForgeSkinnedBuilder.Build(SampleBody());
            try
            {
                int limbBone = 1; // first limb segment
                Vector3 limbVert = Vector3.zero, coreVert = Vector3.zero;
                int limbIdx = -1, coreIdx = -1;
                var verts = r.mesh.vertices;
                var weights = r.mesh.boneWeights;
                for (int i = 0; i < weights.Length; i++)
                {
                    if (limbIdx < 0 && weights[i].boneIndex0 == limbBone) { limbIdx = i; limbVert = verts[i]; }
                    if (coreIdx < 0 && weights[i].boneIndex0 == 0) { coreIdx = i; coreVert = verts[i]; }
                }
                Assert.GreaterOrEqual(limbIdx, 0);
                Assert.GreaterOrEqual(coreIdx, 0);

                Vector3 SkinNow(int vi)
                {
                    int b = weights[vi].boneIndex0;
                    return (r.bones[b].localToWorldMatrix * r.mesh.bindposes[b]).MultiplyPoint3x4(verts[vi]);
                }

                // At bind pose, CPU skinning must reproduce the authored vertex exactly.
                Assert.Less((SkinNow(limbIdx) - limbVert).magnitude, 1e-4f, "bind pose must be identity for its verts");

                // Rotate the limb bone: its vertex moves, the core vertex does not.
                r.bones[limbBone].Rotate(30f, 0f, 0f, Space.Self);
                Assert.Greater((SkinNow(limbIdx) - limbVert).magnitude, 0.005f, "limb vert must follow its bone");
                Assert.Less((SkinNow(coreIdx) - coreVert).magnitude, 1e-4f, "core vert must NOT follow a limb bone");
            }
            finally { Object.DestroyImmediate(r.skeletonRoot); }
        }

        [Test]
        public void Uvs_StayInsideTheAtlas_AndBudgetHolds()
        {
            var body = SampleBody();
            var r = ForgeSkinnedBuilder.Build(body);
            try
            {
                foreach (var uv in r.mesh.uv)
                {
                    Assert.That(uv.x, Is.InRange(0f, 1f));
                    Assert.That(uv.y, Is.InRange(0f, 1f));
                }
                Assert.LessOrEqual(r.mesh.triangles.Length / 3, body.budgetTris);
            }
            finally { Object.DestroyImmediate(r.skeletonRoot); }
        }

        [Test]
        public void Validate_CatchesTheBoneBudget()
        {
            var body = SampleBody();
            body.limbs[0].segments = new[]
            {
                new ForgeLimbSegment(), new ForgeLimbSegment(), new ForgeLimbSegment(),
                new ForgeLimbSegment(), new ForgeLimbSegment(), new ForgeLimbSegment(),
            }; // 6 ×2 mirrored + tail 2 + root = 15 > 12
            Assert.IsTrue(body.Validate().Exists(i => i.Contains("bone count")),
                "over-boned genomes must fail Validate");
        }
    }
}
