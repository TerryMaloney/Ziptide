using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The veil's job is to make leaving and returning to a planet feel like distance. Its
    /// CONTRACT, though, is a safety one: a presentation on the travel path must never be able to
    /// strand the player. These tests pin the schedule (build → cut → clear), the bounds
    /// (intensity always 0..1, always zero once done), and the two rules that keep travel safe —
    /// the cut lead is shorter than the veil, and nothing survives the hard cap.
    /// </summary>
    public sealed class AtmosphereVeilCoreTests
    {
        [Test]
        public void EveryLegFitsUnderTheHardCap()
        {
            Assert.Less(AtmosphereVeilCore.TotalSeconds(VeilLeg.Ascent), AtmosphereVeilCore.HardCapSeconds);
            Assert.Less(AtmosphereVeilCore.TotalSeconds(VeilLeg.Reentry), AtmosphereVeilCore.HardCapSeconds);
        }

        [Test]
        public void CutLead_LandsInsideTheAscentPeak_NeverAfterTheVeilEnds()
        {
            Assert.Greater(AtmosphereVeilCore.CutLeadSeconds, AtmosphereVeilCore.AscentBuildSeconds,
                "cutting before the burn peaks would swap the scene mid-build");
            Assert.Less(AtmosphereVeilCore.CutLeadSeconds, AtmosphereVeilCore.TotalSeconds(VeilLeg.Ascent),
                "a lead past the veil's own length would make travel wait on nothing");
            Assert.AreEqual(VeilPhase.Peak,
                AtmosphereVeilCore.PhaseAt(VeilLeg.Ascent, AtmosphereVeilCore.CutLeadSeconds));
        }

        [Test]
        public void Ascent_BuildsFromNothingToFull()
        {
            Assert.AreEqual(0f, AtmosphereVeilCore.Intensity(VeilLeg.Ascent, 0f));
            Assert.Less(AtmosphereVeilCore.Intensity(VeilLeg.Ascent, 0.3f),
                AtmosphereVeilCore.Intensity(VeilLeg.Ascent, 0.9f));
            Assert.AreEqual(1f,
                AtmosphereVeilCore.Intensity(VeilLeg.Ascent, AtmosphereVeilCore.AscentBuildSeconds + 0.1f),
                0.001f);
        }

        [Test]
        public void Reentry_StartsHotAndClears()
        {
            Assert.AreEqual(1f, AtmosphereVeilCore.Intensity(VeilLeg.Reentry, 0.1f), 0.001f);
            float mid = AtmosphereVeilCore.ReentryPeakSeconds + AtmosphereVeilCore.ReentryClearSeconds * 0.5f;
            float late = AtmosphereVeilCore.ReentryPeakSeconds + AtmosphereVeilCore.ReentryClearSeconds * 0.9f;
            Assert.Greater(AtmosphereVeilCore.Intensity(VeilLeg.Reentry, mid),
                AtmosphereVeilCore.Intensity(VeilLeg.Reentry, late));
        }

        [Test]
        public void IntensityIsAlwaysBounded()
        {
            foreach (VeilLeg leg in new[] { VeilLeg.Ascent, VeilLeg.Reentry })
                for (float t = -1f; t < 8f; t += 0.02f)
                {
                    float v = AtmosphereVeilCore.Intensity(leg, t);
                    Assert.GreaterOrEqual(v, 0f, leg + " at t=" + t);
                    Assert.LessOrEqual(v, 1f, leg + " at t=" + t);
                }
        }

        [Test]
        public void PastTheEnd_TheViewIsAlwaysClear()
        {
            foreach (VeilLeg leg in new[] { VeilLeg.Ascent, VeilLeg.Reentry })
            {
                Assert.AreEqual(0f, AtmosphereVeilCore.Intensity(leg, AtmosphereVeilCore.TotalSeconds(leg)));
                Assert.AreEqual(0f, AtmosphereVeilCore.Intensity(leg, AtmosphereVeilCore.HardCapSeconds));
                Assert.AreEqual(0f, AtmosphereVeilCore.Intensity(leg, 99f));
                Assert.AreEqual(VeilPhase.Done, AtmosphereVeilCore.PhaseAt(leg, 99f));
            }
        }

        [Test]
        public void PhasesRunInOrder()
        {
            Assert.AreEqual(VeilPhase.Build, AtmosphereVeilCore.PhaseAt(VeilLeg.Ascent, 0.1f));
            Assert.AreEqual(VeilPhase.Peak,
                AtmosphereVeilCore.PhaseAt(VeilLeg.Ascent, AtmosphereVeilCore.AscentBuildSeconds + 0.05f));
            Assert.AreEqual(VeilPhase.Peak, AtmosphereVeilCore.PhaseAt(VeilLeg.Reentry, 0.1f));
            Assert.AreEqual(VeilPhase.Clear,
                AtmosphereVeilCore.PhaseAt(VeilLeg.Reentry, AtmosphereVeilCore.ReentryPeakSeconds + 0.05f));
        }

        [Test]
        public void ReentryVeilMatchesTheArrivalBeatsBudget()
        {
            // The reentry act was specced at ReentryArrivalCore.PresentationSeconds before the
            // visual existed; the veil must still fit that budget or the beat and its presentation
            // drift apart.
            Assert.LessOrEqual(AtmosphereVeilCore.TotalSeconds(VeilLeg.Reentry),
                ReentryArrivalCore.PresentationSeconds + 0.001f);
        }

        [Test]
        public void NegativeTimeIsSafe()
        {
            Assert.AreEqual(0f, AtmosphereVeilCore.Intensity(VeilLeg.Ascent, -5f));
            Assert.AreEqual(VeilPhase.Build, AtmosphereVeilCore.PhaseAt(VeilLeg.Ascent, -5f));
        }
    }
}
