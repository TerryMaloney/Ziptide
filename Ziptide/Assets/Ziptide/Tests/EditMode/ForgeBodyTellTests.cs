using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The tell-bridge contract (FORGE II P3 close): behaviors route gameplay reads through
    /// ForgeBodyTell; no forged body → every static returns false/null so the primitive path runs
    /// unchanged. State is tracked in fields, so this is testable headless without shaders.
    /// </summary>
    public class ForgeBodyTellTests
    {
        [Test]
        public void NoForgedBody_EveryStaticFallsBack()
        {
            var host = new GameObject("bare");
            try
            {
                Assert.IsFalse(ForgeBodyTell.TrySetEye(host, Color.red));
                Assert.IsFalse(ForgeBodyTell.TrySetBodyTint(host, Color.gray));
                Assert.IsFalse(ForgeBodyTell.TryClearBodyTint(host));
                Assert.IsNull(ForgeBodyTell.TryCloneStatue(host, Color.gray));
            }
            finally { Object.DestroyImmediate(host); }
        }

        private static (GameObject host, ForgeBodyTell tell) Rig(int eyeIndex)
        {
            var host = new GameObject("host");
            var vis = new GameObject(ForgeCreatureVisualApplier.VisualChildName);
            vis.transform.SetParent(host.transform, false);
            var tell = vis.AddComponent<ForgeBodyTell>();
            tell.Init(null, eyeIndex, new[] { new Color(0.2f, 0.5f, 0.9f), Color.gray });
            return (host, tell);
        }

        [Test]
        public void EyeTell_TracksState_AndStartsAtThePaletteColor()
        {
            var (host, tell) = Rig(eyeIndex: 0);
            try
            {
                Assert.IsTrue(tell.HasEye);
                Assert.AreEqual(new Color(0.2f, 0.5f, 0.9f), tell.EyeColor, "initial = palette eye color");
                Assert.IsTrue(ForgeBodyTell.TrySetEye(host, Color.red));
                Assert.AreEqual(Color.red, tell.EyeColor);
            }
            finally { Object.DestroyImmediate(host); }
        }

        [Test]
        public void BodyTint_SetsAndClears()
        {
            var (host, tell) = Rig(eyeIndex: 0);
            try
            {
                Assert.IsTrue(ForgeBodyTell.TrySetBodyTint(host, Color.gray));
                Assert.IsTrue(tell.IsBodyTinted);
                Assert.AreEqual(Color.gray, tell.BodyTint);
                Assert.IsTrue(ForgeBodyTell.TryClearBodyTint(host));
                Assert.IsFalse(tell.IsBodyTinted, "clear restores the palette (the un-freeze)");
            }
            finally { Object.DestroyImmediate(host); }
        }

        [Test]
        public void EyelessBody_RefusesEyeTells_SoThePrimitiveFallbackRuns()
        {
            var (host, tell) = Rig(eyeIndex: -1);
            try
            {
                Assert.IsFalse(tell.HasEye);
                Assert.IsFalse(ForgeBodyTell.TrySetEye(host, Color.red),
                    "an eyeless genome must NOT swallow the tell — the behavior needs its fallback");
            }
            finally { Object.DestroyImmediate(host); }
        }

        [Test]
        public void CloneStatue_ClonesTheForgeVisual_AndOnlyThat()
        {
            var (host, _) = Rig(eyeIndex: 0);
            var unforged = new GameObject("unforged");
            GameObject statue = null;
            try
            {
                statue = ForgeBodyTell.TryCloneStatue(host, Color.gray);
                Assert.IsNotNull(statue, "a forged host yields a husk clone");
                Assert.AreEqual("__ForgeHusk", statue.name);
                Assert.IsNull(statue.GetComponentInChildren<ForgeBodyTell>(), "the husk carries no live tell");
                Assert.IsNull(ForgeBodyTell.TryCloneStatue(unforged, Color.gray));
            }
            finally
            {
                Object.DestroyImmediate(host);
                Object.DestroyImmediate(unforged);
                if (statue != null) Object.DestroyImmediate(statue);
            }
        }
    }
}
