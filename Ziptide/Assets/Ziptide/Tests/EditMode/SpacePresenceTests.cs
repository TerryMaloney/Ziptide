using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// "Space as a place" — the two instruments that answer Terry's question about the space leg
    /// ("how do we figure out where we're going") and the window that fixes a space game whose
    /// first ten minutes had no space in them. Both are pure underneath, so both are provable here.
    /// </summary>
    public sealed class SpacePresenceTests
    {
        // ── The helm compass ribbon ────────────────────────────────────────────────

        [Test]
        public void Bearing_IsZeroWhenTheTargetIsDeadAhead()
        {
            float b = CompassRibbonCore.BearingDegrees(Vector3.forward, Vector3.zero, new Vector3(0f, 0f, 50f));
            Assert.AreEqual(0f, b, 0.01f);
        }

        [Test]
        public void Bearing_SignsLeftNegativeAndRightPositive()
        {
            Assert.Less(CompassRibbonCore.BearingDegrees(Vector3.forward, Vector3.zero, new Vector3(-30f, 0f, 30f)), 0f);
            Assert.Greater(CompassRibbonCore.BearingDegrees(Vector3.forward, Vector3.zero, new Vector3(30f, 0f, 30f)), 0f);
        }

        [Test]
        public void Bearing_IgnoresHeight_SoTheRibbonStaysOneAxis()
        {
            float flat = CompassRibbonCore.BearingDegrees(Vector3.forward, Vector3.zero, new Vector3(20f, 0f, 20f));
            float high = CompassRibbonCore.BearingDegrees(Vector3.forward, Vector3.zero, new Vector3(20f, 90f, 20f));
            Assert.AreEqual(flat, high, 0.01f);
        }

        [Test]
        public void RibbonOffset_IsClampedToTheStrip()
        {
            Assert.AreEqual(0f, CompassRibbonCore.RibbonOffset(0f), 0.001f);
            Assert.AreEqual(1f, CompassRibbonCore.RibbonOffset(179f), 0.001f);
            Assert.AreEqual(-1f, CompassRibbonCore.RibbonOffset(-179f), 0.001f);
            Assert.AreEqual(0.5f, CompassRibbonCore.RibbonOffset(CompassRibbonCore.HalfFieldDegrees * 0.5f), 0.001f);
        }

        [Test]
        public void Behind_IsItsOwnState_NotAPinnedMarker()
        {
            Assert.IsFalse(CompassRibbonCore.IsBehind(60f));
            Assert.IsTrue(CompassRibbonCore.IsBehind(175f));
            // And it must be visually distinct, or "turn around" reads as "keep turning".
            Assert.AreNotEqual(CompassRibbonCore.MarkerColor(175f), CompassRibbonCore.MarkerColor(60f));
        }

        [Test]
        public void DegenerateBearings_AreSafe()
        {
            Assert.AreEqual(0f, CompassRibbonCore.BearingDegrees(Vector3.zero, Vector3.zero, Vector3.zero));
            Assert.AreEqual(0f, CompassRibbonCore.BearingDegrees(Vector3.up, Vector3.zero, Vector3.up * 5f));
        }

        // ── The porthole ──────────────────────────────────────────────────────────

        [Test]
        public void Porthole_BakeIsDeterministic()
        {
            var a = new byte[32 * 32 * 4];
            var b = new byte[32 * 32 * 4];
            PortholeStarfieldCore.Bake(a, 32, 32, 909, 0.8f);
            PortholeStarfieldCore.Bake(b, 32, 32, 909, 0.8f);
            CollectionAssert.AreEqual(a, b);
        }

        [Test]
        public void Porthole_StarCountRisesWithDensity()
        {
            var sparse = new byte[64 * 64 * 4];
            var dense = new byte[64 * 64 * 4];
            PortholeStarfieldCore.Bake(sparse, 64, 64, 5, 0.1f);
            PortholeStarfieldCore.Bake(dense, 64, 64, 5, 1f);
            Assert.Less(PortholeStarfieldCore.CountBright(sparse, 64, 64, 120),
                PortholeStarfieldCore.CountBright(dense, 64, 64, 120));
        }

        [Test]
        public void Porthole_IsNeverBlank_AndNeverBlownOut()
        {
            var px = new byte[64 * 64 * 4];
            PortholeStarfieldCore.Bake(px, 64, 64, 11, 0.85f);
            int bright = PortholeStarfieldCore.CountBright(px, 64, 64, 120);
            Assert.Greater(bright, 0, "a window onto space with no stars is a black rectangle");
            Assert.Less(bright, 64 * 64 / 2, "the view is a starfield, not a white wall");
        }

        [Test]
        public void Porthole_AlphaIsAlwaysOpaque()
        {
            var px = new byte[16 * 16 * 4];
            PortholeStarfieldCore.Bake(px, 16, 16, 3, 0.5f);
            for (int i = 0; i < 16 * 16; i++)
                Assert.AreEqual(255, px[i * 4 + 3]);
        }

        [Test]
        public void Porthole_RejectsBadBuffersWithoutThrowing()
        {
            Assert.DoesNotThrow(() => PortholeStarfieldCore.Bake(null, 8, 8, 1, 0.5f));
            Assert.DoesNotThrow(() => PortholeStarfieldCore.Bake(new byte[4], 8, 8, 1, 0.5f));
            Assert.DoesNotThrow(() => PortholeStarfieldCore.Bake(new byte[16 * 4], 0, 0, 1, 0.5f));
        }
    }
}
