using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The creature genome catalog stays shippable: every authored body validates clean, fits the
    /// Quest bone budget, builds through the REAL skinning+gait cores without error, and stays in
    /// its triangle budget. Adding a genome to ForgeBodyLibrary.Specs() automatically puts it
    /// under all of these gates.
    /// </summary>
    public class ForgeBodyLibraryTests
    {
        [Test]
        public void EveryGenome_ValidatesClean_AndFitsTheBoneBudget()
        {
            foreach (var spec in ForgeBodyLibrary.Specs())
            {
                var body = spec.Value();
                Assert.IsEmpty(body.Validate(), spec.Key + " must validate clean");
                Assert.LessOrEqual(body.BoneCount(), ForgeCreatureBody.MaxBones, spec.Key);
                Assert.AreEqual(spec.Key, body.bodyId, "asset id and bodyId must agree");
                Object.DestroyImmediate(body);
            }
        }

        [Test]
        public void EveryGenome_BuildsAndAnimates_ThroughTheRealCores()
        {
            foreach (var spec in ForgeBodyLibrary.Specs())
            {
                var body = spec.Value();
                var r = ForgeSkinnedBuilder.Build(body);
                try
                {
                    Assert.AreEqual(body.BoneCount(), r.bones.Length, spec.Key);
                    Assert.Greater(r.mesh.vertexCount, 0, spec.Key);
                    Assert.LessOrEqual(r.mesh.triangles.Length / 3, body.budgetTris, spec.Key);
                    Assert.AreEqual(r.mesh.subMeshCount, r.paletteSlots.Length, spec.Key);

                    // The motor must accept it without throwing and fill every bone slot.
                    var q = new Quaternion[body.BoneCount()];
                    ForgeGaitMotor.Evaluate(body, 0.4f, 1f, q);
                    for (int i = 0; i < q.Length; i++)
                        Assert.AreEqual(1f, Quaternion.Dot(q[i], q[i]), 1e-3f,
                            spec.Key + " bone " + i + " not normalized");
                }
                finally
                {
                    Object.DestroyImmediate(r.skeletonRoot);
                    Object.DestroyImmediate(body);
                }
            }
        }
    }
}
