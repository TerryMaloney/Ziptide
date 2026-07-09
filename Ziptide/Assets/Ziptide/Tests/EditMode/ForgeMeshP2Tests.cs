using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P2 geometry richness (FORGE_II_QUALITY_LEAP §P2): Capsule / Frustum / Torus / SweepSpline /
    /// OrganicBlob + the taper/bend/noise modifiers. Same invariants the original ops are held to:
    /// non-empty, deterministic, outward-wound — pure math, no scene.
    /// </summary>
    public class ForgeMeshP2Tests
    {
        // ── helpers ──────────────────────────────────────────────────────────

        private static ForgeMesh.PartGeometry Geo(ForgePart p) => ForgeMesh.BuildPart(p);

        /// <summary>Star-convex-from-centroid outwardness (capsule/frustum/blob are star-convex).</summary>
        private static void AssertOutwardFromCentroid(ForgeMesh.PartGeometry g, string label)
        {
            Vector3 centroid = Vector3.zero;
            foreach (var v in g.vertices) centroid += v;
            centroid /= g.vertices.Count;
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]], b = g.vertices[g.triangles[t + 1]], c = g.vertices[g.triangles[t + 2]];
                Vector3 fn = Vector3.Cross(b - a, c - a);
                if (fn.sqrMagnitude < 1e-12f) continue; // degenerate seam tris are allowed
                Vector3 outDir = (a + b + c) / 3f - centroid;
                Assert.Greater(Vector3.Dot(fn.normalized, outDir.normalized), 0f,
                    label + " face at tri " + t + " winds inward");
            }
        }

        private static void AssertDeterministic(ForgePart p, string label)
        {
            var g1 = Geo(p);
            var g2 = Geo(p);
            Assert.AreEqual(g1.vertices.Count, g2.vertices.Count, label);
            for (int i = 0; i < g1.vertices.Count; i++)
                Assert.AreEqual(g1.vertices[i], g2.vertices[i], label + " vert " + i);
        }

        private static Bounds BoundsOf(ForgeMesh.PartGeometry g)
        {
            var b = new Bounds(g.vertices[0], Vector3.zero);
            foreach (var v in g.vertices) b.Encapsulate(v);
            return b;
        }

        // ── Capsule ──────────────────────────────────────────────────────────

        [Test]
        public void Capsule_BuildsOutward_AndSpansItsHeight()
        {
            var p = new ForgePart { op = ForgeOp.Capsule, size = new Vector3(0.2f, 0.6f, 0.2f), segments = 8 };
            var g = Geo(p);
            Assert.Greater(g.triangles.Count, 0);
            AssertOutwardFromCentroid(g, "capsule");
            var b = BoundsOf(g);
            Assert.AreEqual(0.6f, b.size.y, 1e-4f, "total height includes the dome caps");
            Assert.AreEqual(0.2f, b.size.x, 1e-3f);
            AssertDeterministic(p, "capsule");
        }

        [Test]
        public void Capsule_SphereDegenerate_HasNoDuplicateEquator()
        {
            // height == diameter → pure sphere; the builder must skip the duplicate equator ring.
            var p = new ForgePart { op = ForgeOp.Capsule, size = new Vector3(0.3f, 0.3f, 0.3f), segments = 8 };
            var g = Geo(p);
            AssertOutwardFromCentroid(g, "capsule-sphere");
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]], b = g.vertices[g.triangles[t + 1]], c = g.vertices[g.triangles[t + 2]];
                Assert.Greater(Vector3.Cross(b - a, c - a).sqrMagnitude, 1e-14f, "zero-area tri at " + t);
            }
        }

        // ── Frustum ──────────────────────────────────────────────────────────

        [Test]
        public void Frustum_TruncatedCone_Outward()
        {
            var p = new ForgePart { op = ForgeOp.Frustum, size = new Vector3(0.3f, 0.4f, 0.12f), segments = 10 };
            var g = Geo(p);
            AssertOutwardFromCentroid(g, "frustum");
            var b = BoundsOf(g);
            Assert.AreEqual(0.3f, b.size.x, 1e-3f, "bottom diameter rules the width");
            AssertDeterministic(p, "frustum");
        }

        [Test]
        public void Frustum_ZeroTop_IsATrueCone()
        {
            var p = new ForgePart { op = ForgeOp.Frustum, size = new Vector3(0.3f, 0.4f, 0f), segments = 8 };
            var g = Geo(p);
            AssertOutwardFromCentroid(g, "cone");
            // Apex: exactly one vertex at the very top.
            int apexCount = 0;
            foreach (var v in g.vertices) if (Mathf.Abs(v.y - 0.2f) < 1e-5f) apexCount++;
            Assert.AreEqual(1, apexCount, "a cone has a single apex vertex");
        }

        // ── Torus ────────────────────────────────────────────────────────────

        [Test]
        public void Torus_WindsOutward_AgainstTheAnalyticNormal()
        {
            float majorR = 0.15f, minorR = 0.04f;
            var p = new ForgePart { op = ForgeOp.Torus, size = new Vector3(majorR * 2f, minorR * 2f, 0.01f), segments = 12 };
            var g = Geo(p);
            Assert.Greater(g.triangles.Count, 0);
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]], b = g.vertices[g.triangles[t + 1]], c = g.vertices[g.triangles[t + 2]];
                Vector3 fc = (a + b + c) / 3f;
                Vector3 onRing = new Vector3(fc.x, 0f, fc.z);
                if (onRing.sqrMagnitude < 1e-10f) continue;
                Vector3 tubeCenter = onRing.normalized * majorR;
                Vector3 analytic = (fc - tubeCenter).normalized;
                Vector3 fn = Vector3.Cross(b - a, c - a);
                if (fn.sqrMagnitude < 1e-12f) continue;
                Assert.Greater(Vector3.Dot(fn.normalized, analytic), 0f, "torus face " + t + " winds inward");
            }
            var bounds = BoundsOf(g);
            Assert.AreEqual((majorR + minorR) * 2f, bounds.size.x, 1e-3f, "outer diameter = major + minor");
            AssertDeterministic(p, "torus");
        }

        // ── SweepSpline ──────────────────────────────────────────────────────

        [Test]
        public void SweepSpline_StraightDown_MatchesCylinderWinding()
        {
            var p = new ForgePart
            {
                op = ForgeOp.SweepSpline, size = new Vector3(0.08f, 0.1f, 0.1f), segments = 8,
                spline = new[] { new Vector3(0f, 0.4f, 0f), new Vector3(0f, -0.4f, 0f) }
            };
            var g = Geo(p);
            Assert.Greater(g.triangles.Count, 0);
            // Wall faces (not caps) must point radially out from the Y axis.
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]], b = g.vertices[g.triangles[t + 1]], c = g.vertices[g.triangles[t + 2]];
                Vector3 fn = Vector3.Cross(b - a, c - a);
                if (fn.sqrMagnitude < 1e-12f) continue;
                fn.Normalize();
                if (Mathf.Abs(fn.y) > 0.9f) continue; // caps
                Vector3 fc = (a + b + c) / 3f;
                Vector3 radial = new Vector3(fc.x, 0f, fc.z).normalized;
                Assert.Greater(Vector3.Dot(fn, radial), 0f, "sweep wall face " + t + " winds inward");
            }
            AssertDeterministic(p, "sweep");
        }

        [Test]
        public void SweepSpline_CurvedTentacle_FollowsItsControlPoints_AndTapersByProfile()
        {
            var p = new ForgePart
            {
                op = ForgeOp.SweepSpline, size = new Vector3(0.1f, 0.1f, 0.1f), segments = 8,
                spline = new[] { Vector3.zero, new Vector3(0.2f, 0.3f, 0f), new Vector3(0.5f, 0.35f, 0.1f) },
                profile = new[] { new Vector2(0.06f, 0f), new Vector2(0.04f, 0f), new Vector2(0.01f, 0f) }
            };
            var g = Geo(p);
            var b = BoundsOf(g);
            Assert.Greater(b.max.x, 0.45f, "the tube reaches its far control point");
            Assert.Greater(b.max.y, 0.3f, "the tube arcs upward");

            // Per-point radii: rings near t=1 must be much thinner than rings near t=0.
            Vector3 endC = new Vector3(0.5f, 0.35f, 0.1f);
            float maxDistNearEnd = 0f, maxDistNearStart = 0f;
            foreach (var v in g.vertices)
            {
                if ((v - endC).magnitude < 0.05f) maxDistNearEnd = Mathf.Max(maxDistNearEnd, (v - endC).magnitude);
                if (v.magnitude < 0.08f) maxDistNearStart = Mathf.Max(maxDistNearStart, v.magnitude);
            }
            Assert.Greater(maxDistNearStart, maxDistNearEnd, "tip is thinner than root");
        }

        // ── OrganicBlob ──────────────────────────────────────────────────────

        [Test]
        public void OrganicBlob_PlainSphere_Outward_AndClosed()
        {
            var p = new ForgePart { op = ForgeOp.OrganicBlob, size = new Vector3(0.3f, 0.4f, 0.3f), segments = 10 };
            var g = Geo(p);
            AssertOutwardFromCentroid(g, "blob");
            var b = BoundsOf(g);
            Assert.AreEqual(0.4f, b.size.y, 1e-3f, "ellipsoid Y extent");
            AssertDeterministic(p, "blob");
        }

        // ── Modifiers ────────────────────────────────────────────────────────

        [Test]
        public void Taper_ShrinksTheTop_LeavesTheBottom()
        {
            var p = new ForgePart { op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.4f, 0.2f), taper = 0.5f };
            var g = Geo(p);
            float topMax = 0f, botMax = 0f;
            foreach (var v in g.vertices)
            {
                if (v.y > 0.19f) topMax = Mathf.Max(topMax, Mathf.Abs(v.x));
                if (v.y < -0.19f) botMax = Mathf.Max(botMax, Mathf.Abs(v.x));
            }
            Assert.AreEqual(0.1f, botMax, 1e-4f, "base untouched");
            Assert.AreEqual(0.05f, topMax, 1e-3f, "top scaled by (1 - taper)");
        }

        [Test]
        public void Bend_CurvesTheTopTowardPlusZ_BaseStaysPut()
        {
            var p = new ForgePart { op = ForgeOp.BeveledBox, size = new Vector3(0.1f, 0.6f, 0.1f), bendDegrees = 90f };
            var g = Geo(p);
            var b = BoundsOf(g);
            Assert.Greater(b.max.z, 0.2f, "a 90-degree bend swings the top well into +Z");
            Assert.Less(b.max.y, 0.3f, "arcing shortens the vertical reach");
            float minY = b.min.y;
            Assert.AreEqual(-0.3f, minY, 1e-3f, "the base slice does not move");
        }

        [Test]
        public void Noise_DisplacesWithinAmplitude_AndIsSeedStable()
        {
            var basePart = new ForgePart { op = ForgeOp.OrganicBlob, size = new Vector3(0.3f, 0.3f, 0.3f), segments = 10, smooth = true };
            var noisy = new ForgePart
            {
                op = ForgeOp.OrganicBlob, size = new Vector3(0.3f, 0.3f, 0.3f), segments = 10, smooth = true,
                noiseAmplitude = 0.02f, noiseFrequency = 9f, noiseSeed = 7
            };
            var g0 = Geo(basePart);
            var g1 = Geo(noisy);
            float maxDelta = 0f;
            for (int i = 0; i < g0.vertices.Count; i++)
                maxDelta = Mathf.Max(maxDelta, (g1.vertices[i] - g0.vertices[i]).magnitude);
            Assert.Greater(maxDelta, 0.002f, "noise actually displaces");
            Assert.LessOrEqual(maxDelta, 0.021f, "never beyond the amplitude");

            AssertDeterministic(noisy, "noisy blob");
            var other = new ForgePart
            {
                op = ForgeOp.OrganicBlob, size = new Vector3(0.3f, 0.3f, 0.3f), segments = 10, smooth = true,
                noiseAmplitude = 0.02f, noiseFrequency = 9f, noiseSeed = 8
            };
            var g2 = Geo(other);
            bool anyDiff = false;
            for (int i = 0; i < g1.vertices.Count && !anyDiff; i++)
                anyDiff = (g1.vertices[i] - g2.vertices[i]).sqrMagnitude > 1e-10f;
            Assert.IsTrue(anyDiff, "a different seed gives different bumps");
        }

        [Test]
        public void Noise_OnAHardEdgedBox_NeverCracksTheSeams()
        {
            // Quad-built ops duplicate corner verts; welded normals must displace duplicates identically.
            var p = new ForgePart
            {
                op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.2f, 0.2f),
                noiseAmplitude = 0.03f, noiseFrequency = 11f, noiseSeed = 3
            };
            var g = Geo(p);
            var byPos = new Dictionary<Vector3Int, Vector3>();
            foreach (var v in g.vertices)
            {
                var k = new Vector3Int(Mathf.RoundToInt(v.x * 100000f), Mathf.RoundToInt(v.y * 100000f), Mathf.RoundToInt(v.z * 100000f));
                byPos[k] = v; // if duplicates diverged, they'd land in different buckets — checked below
            }
            // A cracked box would have MORE unique positions than the noiseless one.
            var clean = Geo(new ForgePart { op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.2f, 0.2f) });
            var cleanPos = new HashSet<Vector3Int>();
            foreach (var v in clean.vertices)
                cleanPos.Add(new Vector3Int(Mathf.RoundToInt(v.x * 100000f), Mathf.RoundToInt(v.y * 100000f), Mathf.RoundToInt(v.z * 100000f)));
            Assert.LessOrEqual(byPos.Count, cleanPos.Count, "noise split welded corners apart (cracks)");
        }

        // ── Validation + hash guardrails ─────────────────────────────────────

        private static ForgeRecipeDefinition Recipe(params ForgePart[] parts)
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "p2_test";
            r.palette = new[] { Color.gray };
            r.parts = parts;
            return r;
        }

        [Test]
        public void Validate_AcceptsAConeTop_RejectsABadSpline_AndBadModifiers()
        {
            var cone = Recipe(new ForgePart { op = ForgeOp.Frustum, size = new Vector3(0.2f, 0.3f, 0f) });
            Assert.IsEmpty(cone.Validate(), "a zero top diameter is a legal cone");

            var badSpline = Recipe(new ForgePart { op = ForgeOp.SweepSpline, size = new Vector3(0.1f, 0.1f, 0.1f) });
            Assert.IsTrue(badSpline.Validate().Exists(i => i.Contains("spline")), "missing spline must fail");

            var strayField = Recipe(new ForgePart { op = ForgeOp.BeveledBox, size = Vector3.one * 0.1f, spline = new[] { Vector3.zero, Vector3.one } });
            Assert.IsTrue(strayField.Validate().Exists(i => i.Contains("SweepSpline")), "spline on a box is an authoring mistake");

            var badTaper = Recipe(new ForgePart { op = ForgeOp.BeveledBox, size = Vector3.one * 0.1f, taper = 2f });
            Assert.IsTrue(badTaper.Validate().Exists(i => i.Contains("taper")));

            var badNoise = Recipe(new ForgePart { op = ForgeOp.BeveledBox, size = Vector3.one * 0.1f, noiseAmplitude = 3f });
            Assert.IsTrue(badNoise.Validate().Exists(i => i.Contains("noiseAmplitude")));

            var fatTorus = Recipe(new ForgePart { op = ForgeOp.Torus, size = new Vector3(0.1f, 0.2f, 0.05f) });
            Assert.IsTrue(fatTorus.Validate().Exists(i => i.Contains("Torus")), "minor >= major must fail");
        }

        [Test]
        public void ContentHash_IsStableForOldAssets_AndSensitiveToModifiers()
        {
            var plain = Recipe(new ForgePart { op = ForgeOp.BeveledBox, size = Vector3.one * 0.1f });
            string h1 = plain.ComputeContentHash();
            Assert.AreEqual(h1, plain.ComputeContentHash(), "hash is deterministic");

            var modded = Recipe(new ForgePart { op = ForgeOp.BeveledBox, size = Vector3.one * 0.1f, taper = 0.3f });
            Assert.AreNotEqual(h1, modded.ComputeContentHash(), "a modifier changes the look, so it changes the hash");
        }

        [Test]
        public void NewOps_FlowThroughTheFullMeshBuild()
        {
            var r = Recipe(
                new ForgePart { op = ForgeOp.Capsule, size = new Vector3(0.1f, 0.3f, 0.1f), smooth = true },
                new ForgePart { op = ForgeOp.Torus, size = new Vector3(0.2f, 0.05f, 0.01f), position = new Vector3(0f, 0.2f, 0f) },
                new ForgePart
                {
                    op = ForgeOp.SweepSpline, size = new Vector3(0.05f, 0.1f, 0.1f), smooth = true,
                    spline = new[] { Vector3.zero, new Vector3(0.1f, 0.2f, 0f), new Vector3(0.3f, 0.25f, 0.05f) }
                });
            Assert.IsEmpty(r.Validate());
            var mesh = ForgeMesh.Build(r);
            Assert.Greater(mesh.vertexCount, 0);
            Assert.AreEqual(mesh.vertexCount, mesh.uv.Length, "every vertex owns an atlas UV");
            Assert.Greater(ForgeMesh.CountTriangles(r), 0);
        }
    }
}
