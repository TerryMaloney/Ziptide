using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE II E1.1 contract: every part owns a disjoint UV-atlas island, all UVs land inside
    /// their island (texel ownership for the E1.2 rasterizer), island size tracks surface area
    /// (uniform texel density), cylindrical wrap seams are fixed per-face, and the mesh carries
    /// tangents for the coming normal maps. Pure — no scene, no GPU.
    /// </summary>
    public class ForgeUVTests
    {
        private static ForgePart Part(ForgeOp op, Vector3 size)
        {
            var p = new ForgePart { op = op, size = size, segments = 10 };
            if (op == ForgeOp.SphereSection) p.bevel = 1f;
            if (op == ForgeOp.Tube) p.wallThickness = size.x * 0.15f;
            if (op == ForgeOp.Lathe) p.profile = new[] { new Vector2(0.5f, 0f), new Vector2(1f, 0.5f), new Vector2(0.3f, 1f) };
            return p;
        }

        private static ForgeRecipeDefinition Recipe(params ForgePart[] parts)
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "uv_test";
            r.palette = new[] { Color.red };
            r.parts = parts;
            return r;
        }

        [Test]
        public void Islands_AreDisjoint_AndInsideTheAtlas()
        {
            var r = Recipe(
                Part(ForgeOp.BeveledBox, new Vector3(0.2f, 0.3f, 0.4f)),
                Part(ForgeOp.Cylinder, new Vector3(0.1f, 0.5f, 0.1f)),
                Part(ForgeOp.SphereSection, new Vector3(0.2f, 0.2f, 0.2f)),
                Part(ForgeOp.Wedge, new Vector3(0.05f, 0.05f, 0.08f)),
                Part(ForgeOp.GreebleStrip, new Vector3(0.1f, 0.05f, 0.3f)),
                Part(ForgeOp.Tube, new Vector3(0.15f, 0.1f, 0.15f)));
            var islands = ForgeUV.ComputeIslands(r);
            Assert.AreEqual(r.parts.Length, islands.Count);
            for (int i = 0; i < islands.Count; i++)
            {
                var a = islands[i].rect;
                Assert.IsTrue(a.xMin >= 0f && a.yMin >= 0f && a.xMax <= 1f && a.yMax <= 1f,
                    "island " + i + " escapes the atlas: " + a);
                for (int j = i + 1; j < islands.Count; j++)
                    Assert.IsFalse(a.Overlaps(islands[j].rect),
                        "islands " + i + " and " + j + " overlap");
            }
        }

        [Test]
        public void IslandSize_TracksSurfaceArea()
        {
            // Part B has 4x the linear size of part A (16x area) — island edge should be ~4x,
            // and never worse than 2x off uniform texel density.
            var small = Part(ForgeOp.BeveledBox, new Vector3(0.05f, 0.05f, 0.05f));
            var large = Part(ForgeOp.BeveledBox, new Vector3(0.2f, 0.2f, 0.2f));
            var islands = ForgeUV.ComputeIslands(Recipe(small, large));
            float eSmall = islands[0].rect.width;
            float eLarge = islands[1].rect.width;
            float ratio = eLarge / eSmall; // ideal 4 (sqrt of 16x area)
            Assert.Greater(ratio, 2f, "large part starved of texels");
            Assert.Less(ratio, 8f, "small part starved of texels");
        }

        [Test]
        public void AllMeshUVs_LandInsideTheirPartsIsland()
        {
            var box = Part(ForgeOp.BeveledBox, new Vector3(0.2f, 0.1f, 0.3f));
            box.bevel = 0.01f;
            var cyl = Part(ForgeOp.Cylinder, new Vector3(0.1f, 0.4f, 0.1f));
            cyl.paletteSlot = 0;
            var r = Recipe(box, cyl);
            var islands = ForgeUV.ComputeIslands(r);
            var mesh = ForgeMesh.Build(r);
            var uvs = mesh.uv;
            Assert.AreEqual(mesh.vertexCount, uvs.Length, "every vertex needs a UV");

            // Part order = emission order (no mirrors here): count verts per part by rebuilding.
            int boxVerts = ForgeMesh.BuildPart(box).triangles.Count; // exploded: 1 vert per index
            const float eps = 1e-4f;
            for (int i = 0; i < uvs.Length; i++)
            {
                Rect isl = islands[i < boxVerts ? 0 : 1].rect;
                Assert.IsTrue(uvs[i].x >= isl.xMin - eps && uvs[i].x <= isl.xMax + eps
                    && uvs[i].y >= isl.yMin - eps && uvs[i].y <= isl.yMax + eps,
                    "uv " + i + " " + uvs[i] + " escaped island " + isl);
            }
        }

        [Test]
        public void CylinderSeam_IsFixedPerFace()
        {
            var cyl = Part(ForgeOp.Cylinder, new Vector3(0.2f, 0.3f, 0.2f));
            var r = Recipe(cyl);
            var islands = ForgeUV.ComputeIslands(r);
            float islandW = islands[0].rect.width;
            var mesh = ForgeMesh.Build(r);
            var uvs = mesh.uv;
            var tris = mesh.triangles;
            for (int t = 0; t < tris.Length; t += 3)
            {
                float u0 = uvs[tris[t]].x, u1 = uvs[tris[t + 1]].x, u2 = uvs[tris[t + 2]].x;
                float span = Mathf.Max(u0, Mathf.Max(u1, u2)) - Mathf.Min(u0, Mathf.Min(u1, u2));
                Assert.Less(span, islandW * 0.6f,
                    "triangle at " + t + " spans the wrap seam (u-span " + span + ")");
            }
        }

        [Test]
        public void MirroredPart_ReusesItsIsland()
        {
            // The mirrored instance flips winding (b↔c), so vertex ORDER differs — the contract is
            // that both copies use the SAME island with the SAME UV set, not positional identity.
            var p = Part(ForgeOp.BeveledBox, new Vector3(0.1f, 0.1f, 0.1f));
            p.position = new Vector3(0.2f, 0f, 0f);
            p.mirrorX = true;
            var r = Recipe(p);
            var islands = ForgeUV.ComputeIslands(r);
            Assert.AreEqual(1, islands.Count, "a mirrored part is ONE island, not two");
            var isl = islands[0].rect;
            var mesh = ForgeMesh.Build(r);
            var uvs = mesh.uv;
            int half = uvs.Length / 2;

            var first = new List<Vector2>();
            var second = new List<Vector2>();
            const float eps = 1e-4f;
            for (int i = 0; i < uvs.Length; i++)
            {
                Assert.IsTrue(uvs[i].x >= isl.xMin - eps && uvs[i].x <= isl.xMax + eps
                    && uvs[i].y >= isl.yMin - eps && uvs[i].y <= isl.yMax + eps,
                    "mirrored-part uv escaped the shared island: " + uvs[i]);
                (i < half ? first : second).Add(uvs[i]);
            }
            System.Comparison<Vector2> byXY = (a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y);
            first.Sort(byXY);
            second.Sort(byXY);
            for (int i = 0; i < half; i++)
            {
                Assert.AreEqual(first[i].x, second[i].x, 1e-5f, "mirrored UV set diverged (x) at " + i);
                Assert.AreEqual(first[i].y, second[i].y, 1e-5f, "mirrored UV set diverged (y) at " + i);
            }
        }

        [Test]
        public void UVs_AreDeterministic_AndTangentsExist()
        {
            var r = Recipe(
                Part(ForgeOp.BeveledBox, new Vector3(0.2f, 0.3f, 0.4f)),
                Part(ForgeOp.Lathe, new Vector3(0.15f, 0.3f, 0.15f)));
            var a = ForgeMesh.Build(r);
            var b = ForgeMesh.Build(r);
            CollectionAssert.AreEqual(a.uv, b.uv);
            Assert.AreEqual(a.vertexCount, a.tangents.Length, "tangents missing (normal maps need them)");
        }

        [Test]
        public void SmoothParts_AlsoGetContainedUVs()
        {
            var dome = Part(ForgeOp.SphereSection, new Vector3(0.2f, 0.2f, 0.2f));
            dome.smooth = true;
            var r = Recipe(dome);
            var islands = ForgeUV.ComputeIslands(r);
            var mesh = ForgeMesh.Build(r);
            var isl = islands[0].rect;
            const float eps = 1e-4f;
            foreach (var uv in mesh.uv)
                Assert.IsTrue(uv.x >= isl.xMin - eps && uv.x <= isl.xMax + eps
                    && uv.y >= isl.yMin - eps && uv.y <= isl.yMax + eps, "smooth uv escaped " + uv);
        }
    }
}
