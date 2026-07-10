using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Forge geometry contract: every op emits finite, in-range, outward-facing triangles;
    /// builds are seed-free deterministic; mirrors are symmetric; flat vs smooth shading differ;
    /// budgets count honestly. Pure — no scene, no GPU.
    /// </summary>
    public class ForgeMeshTests
    {
        private static ForgePart Part(ForgeOp op)
        {
            var p = new ForgePart { op = op, size = new Vector3(0.2f, 0.3f, 0.4f), segments = 8 };
            if (op == ForgeOp.BeveledBox) p.bevel = 0.02f;
            if (op == ForgeOp.SphereSection) p.bevel = 1f;
            if (op == ForgeOp.Tube) p.wallThickness = 0.03f;
            if (op == ForgeOp.Lathe) p.profile = new[] { new Vector2(0.4f, 0f), new Vector2(1f, 0.4f), new Vector2(0.2f, 1f) };
            // SweepSpline requires its curve the same way Lathe requires its profile (2..4 pts).
            if (op == ForgeOp.SweepSpline) p.spline = new[]
                { Vector3.zero, new Vector3(0.03f, 0.06f, 0f), new Vector3(0f, 0.12f, 0.02f) };
            return p;
        }

        private static ForgeRecipeDefinition Recipe(params ForgePart[] parts)
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "test_recipe";
            r.palette = new[] { Color.red, Color.blue };
            r.parts = parts;
            return r;
        }

        [Test]
        public void EveryOp_EmitsValidIndexedGeometry()
        {
            foreach (ForgeOp op in System.Enum.GetValues(typeof(ForgeOp)))
            {
                var g = ForgeMesh.BuildPart(Part(op));
                Assert.Greater(g.vertices.Count, 0, op + " emitted no vertices");
                Assert.Greater(g.triangles.Count, 0, op + " emitted no triangles");
                Assert.AreEqual(0, g.triangles.Count % 3, op + " triangle list not divisible by 3");
                foreach (int idx in g.triangles)
                    Assert.IsTrue(idx >= 0 && idx < g.vertices.Count, op + " index out of range");
                foreach (var v in g.vertices)
                    Assert.IsTrue(!float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z)
                        && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z),
                        op + " produced a non-finite vertex");
            }
        }

        [Test]
        public void ConvexOps_FaceOutward()
        {
            // For centered convex shapes, every face normal must point away from the origin.
            foreach (var op in new[] { ForgeOp.BeveledBox, ForgeOp.Cylinder, ForgeOp.Wedge, ForgeOp.SphereSection })
            {
                var g = ForgeMesh.BuildPart(Part(op));
                int inward = 0;
                for (int t = 0; t < g.triangles.Count; t += 3)
                {
                    Vector3 a = g.vertices[g.triangles[t]], b = g.vertices[g.triangles[t + 1]], c = g.vertices[g.triangles[t + 2]];
                    Vector3 centroid = (a + b + c) / 3f;
                    Vector3 n = Vector3.Cross(b - a, c - a);
                    if (centroid.sqrMagnitude > 1e-8f && Vector3.Dot(n, centroid) < -1e-7f) inward++;
                }
                Assert.AreEqual(0, inward, op + " has " + inward + " inward-facing triangles");
            }
        }

        [Test]
        public void Build_IsDeterministic()
        {
            var r = Recipe(Part(ForgeOp.BeveledBox), Part(ForgeOp.Cylinder), Part(ForgeOp.GreebleStrip));
            var a = ForgeMesh.Build(r);
            var b = ForgeMesh.Build(r);
            CollectionAssert.AreEqual(a.vertices, b.vertices);
            CollectionAssert.AreEqual(a.triangles, b.triangles);
            CollectionAssert.AreEqual(a.normals, b.normals);
        }

        [Test]
        public void MirrorX_ProducesSymmetricBounds_AndDoublesTris()
        {
            var off = Part(ForgeOp.BeveledBox);
            off.position = new Vector3(0.3f, 0.1f, 0f);
            var plain = Recipe(off);
            int plainTris = ForgeMesh.CountTriangles(plain);

            var mirroredPart = Part(ForgeOp.BeveledBox);
            mirroredPart.position = new Vector3(0.3f, 0.1f, 0f);
            mirroredPart.mirrorX = true;
            var mirrored = Recipe(mirroredPart);
            Assert.AreEqual(plainTris * 2, ForgeMesh.CountTriangles(mirrored));

            var mesh = ForgeMesh.Build(mirrored);
            Bounds bnds = mesh.bounds;
            Assert.AreEqual(-bnds.min.x, bnds.max.x, 0.001f, "mirrored bounds not symmetric about X");
        }

        [Test]
        public void MirroredGeometry_StillFacesOutward()
        {
            var p = Part(ForgeOp.Cylinder);
            p.position = new Vector3(0.3f, 0f, 0f);
            p.mirrorX = true;
            var mesh = ForgeMesh.Build(Recipe(p));
            var verts = mesh.vertices;
            var tris = mesh.triangles;
            int inward = 0;
            for (int t = 0; t < tris.Length; t += 3)
            {
                Vector3 a = verts[tris[t]], b = verts[tris[t + 1]], c = verts[tris[t + 2]];
                Vector3 centroid = (a + b + c) / 3f;
                // Compare against the nearer instance center (±0.3 on X).
                Vector3 center = centroid.x >= 0f ? new Vector3(0.3f, 0f, 0f) : new Vector3(-0.3f, 0f, 0f);
                Vector3 n = Vector3.Cross(b - a, c - a);
                if (Vector3.Dot(n, centroid - center) < -1e-7f) inward++;
            }
            Assert.AreEqual(0, inward, "mirrored copy has flipped-inward faces — winding not corrected");
        }

        [Test]
        public void FlatShading_ExplodesVerts_SmoothShares()
        {
            var flat = Part(ForgeOp.Cylinder);
            var smooth = Part(ForgeOp.Cylinder);
            smooth.smooth = true;
            var flatMesh = ForgeMesh.Build(Recipe(flat));
            var smoothMesh = ForgeMesh.Build(Recipe(smooth));
            Assert.Greater(flatMesh.vertexCount, smoothMesh.vertexCount);
            Assert.AreEqual(flatMesh.triangles.Length, smoothMesh.triangles.Length);
        }

        [Test]
        public void CountTriangles_MatchesBuiltMesh()
        {
            var r = Recipe(Part(ForgeOp.BeveledBox), Part(ForgeOp.Tube), Part(ForgeOp.Wedge));
            var mesh = ForgeMesh.Build(r);
            Assert.AreEqual(ForgeMesh.CountTriangles(r), mesh.triangles.Length / 3);
        }

        [Test]
        public void Submeshes_MatchUsedPaletteSlots()
        {
            var a = Part(ForgeOp.BeveledBox); a.paletteSlot = 0;
            var b = Part(ForgeOp.Cylinder); b.paletteSlot = 1;
            var mesh = ForgeMesh.Build(Recipe(a, b));
            Assert.AreEqual(2, mesh.subMeshCount);
            CollectionAssert.AreEqual(new List<int> { 0, 1 }, ForgeMesh.UsedPaletteSlots(Recipe(a, b)));
        }

        [Test]
        public void BuildSingle_CollapsesSubmeshes_KeepingEveryTriangle()
        {
            var a = Part(ForgeOp.BeveledBox); a.paletteSlot = 0;
            var b = Part(ForgeOp.Cylinder); b.paletteSlot = 1;
            var r = Recipe(a, b);
            var single = ForgeMesh.BuildSingle(r);
            Assert.AreEqual(1, single.subMeshCount, "E1.3: one textured material = one submesh");
            Assert.AreEqual(ForgeMesh.CountTriangles(r), single.triangles.Length / 3,
                "the collapse must not drop or duplicate triangles");
        }

        [Test]
        public void Normals_AreUnitLength()
        {
            var r = Recipe(Part(ForgeOp.SphereSection), Part(ForgeOp.Lathe));
            r.parts[0].smooth = true;
            var mesh = ForgeMesh.Build(r);
            foreach (var n in mesh.normals)
                Assert.AreEqual(1f, n.magnitude, 0.01f, "non-unit normal");
        }

        // ── Validation ──────────────────────────────────────────────────────

        [Test]
        public void Validate_AcceptsAWellFormedRecipe()
        {
            var r = Recipe(Part(ForgeOp.BeveledBox));
            r.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle" }
            };
            Assert.IsEmpty(r.Validate());
        }

        [Test]
        public void Validate_RejectsTheFootguns()
        {
            var r = Recipe(Part(ForgeOp.BeveledBox));
            r.recipeId = "";
            Assert.IsNotEmpty(r.Validate(), "empty id");

            r = Recipe(Part(ForgeOp.BeveledBox));
            r.parts[0].paletteSlot = 5; // palette has 2
            Assert.IsNotEmpty(r.Validate(), "palette slot out of range");

            r = Recipe(Part(ForgeOp.BeveledBox));
            r.surfaceFamily = "MadeUpFamily";
            Assert.IsNotEmpty(r.Validate(), "unknown family");

            r = Recipe(Part(ForgeOp.BeveledBox));
            r.parts[0].size = new Vector3(9f, 0.1f, 0.1f);
            Assert.IsNotEmpty(r.Validate(), "oversized part");

            r = Recipe(Part(ForgeOp.Lathe));
            r.parts[0].profile = new[] { new Vector2(1f, 0f) };
            Assert.IsNotEmpty(r.Validate(), "one-point lathe profile");

            r = Recipe(Part(ForgeOp.BeveledBox));
            r.sockets = new[] { new ForgeSocket { name = "Grip" } };
            Assert.IsNotEmpty(r.Validate(), "Grip without Muzzle");

            r = Recipe(Part(ForgeOp.BeveledBox));
            r.schemaVersion = 99;
            Assert.IsNotEmpty(r.Validate(), "unknown schema version");
        }

        [Test]
        public void PaletteLaw_AlienOrigamiIsStrict_SalvageIsNot()
        {
            Assert.IsTrue(ForgePalettes.IsAllowed(ForgePalettes.FamilySalvage, new Color(0.5f, 0.25f, 0.1f)));
            Assert.IsTrue(ForgePalettes.IsAllowed(ForgePalettes.FamilyAlienOrigami, new Color(0.05f, 0.05f, 0.06f)), "matte black");
            Assert.IsTrue(ForgePalettes.IsAllowed(ForgePalettes.FamilyAlienOrigami, new Color(0.1f, 0.5f, 0.55f)), "deep teal");
            Assert.IsTrue(ForgePalettes.IsAllowed(ForgePalettes.FamilyAlienOrigami, new Color(0.8f, 0.6f, 0.2f)), "gold");
            Assert.IsFalse(ForgePalettes.IsAllowed(ForgePalettes.FamilyAlienOrigami, new Color(0.8f, 0.1f, 0.1f)), "red is off-canon for Origami");
        }
    }
}
