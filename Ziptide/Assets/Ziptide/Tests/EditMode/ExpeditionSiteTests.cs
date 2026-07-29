using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE EXPEDITION (⚖ Terry): half B is out on the tidal flats, reached by vehicle through a
    /// breach in the sea wall. The failure mode that would quietly kill this leg is geometric — a
    /// ramp placed against solid wall, or a site standing off the edge of the generated flat — so
    /// these tests pin the site against the SAME ring numbers RingCityBuilder builds from, and pin
    /// the resonance tell's promise that it never leaves anything dark.
    /// </summary>
    public sealed class ExpeditionSiteTests
    {
        private static RingCityDef Rings()
        {
            // The committed ToxicCity ring values.
            return new RingCityDef
            {
                enabled = true,
                seaWallRadius = 126f,
                seaWallBreachCount = 2,
                outskirtsRadius = 190f,
                harbourAzimuthDegrees = 180f,
                harbourArcDegrees = 46f,
            };
        }

        [Test]
        public void Site_StandsOnTheTidalFlat_OutsideTheWallAndInsideTheOutskirts()
        {
            var r = Rings();
            float radius = new Vector2(FlatsSiteAuthor.SitePosition.x, FlatsSiteAuthor.SitePosition.z).magnitude;
            Assert.Greater(radius, r.seaWallRadius,
                "the whole point is that half B is OUTSIDE the wall");
            Assert.Less(radius, r.outskirtsRadius,
                "past the outskirts there is no generated ground to stand the wreck on");
        }

        [Test]
        public void BreachRamp_LandsOnAnActualGapInTheWall()
        {
            var r = Rings();
            float deg = FlatsSiteAuthor.BreachAzimuthDegrees(r);

            // Recompute the builder's breach bearings independently: the ramp must coincide with
            // one of them, or the drive out ends at a wall.
            const int segments = 48;
            bool matches = false;
            for (int b = 0; b < r.seaWallBreachCount; b++)
            {
                int seg = (int)(segments * ((b + 0.5f) / r.seaWallBreachCount) + 7) % segments;
                if (Mathf.Abs(Mathf.DeltaAngle(seg / (float)segments * 360f, deg)) < 0.01f) matches = true;
            }
            Assert.IsTrue(matches, "the ramp must sit at a breach the wall builder actually leaves");
        }

        [Test]
        public void BreachRamp_IsNotInsideTheHarbourMouth()
        {
            var r = Rings();
            float deg = FlatsSiteAuthor.BreachAzimuthDegrees(r);
            float from = r.harbourAzimuthDegrees - r.harbourArcDegrees * 0.5f;
            float to = r.harbourAzimuthDegrees + r.harbourArcDegrees * 0.5f;
            Assert.IsFalse(deg >= from && deg <= to,
                "a 'breach' inside the harbour mouth is just the harbour — the expedition needs its own way out");
        }

        [Test]
        public void BreachChosen_IsTheOneFacingTheSite()
        {
            var r = Rings();
            float deg = FlatsSiteAuthor.BreachAzimuthDegrees(r);
            Assert.Less(Mathf.Abs(Mathf.DeltaAngle(deg, FlatsSiteAuthor.SiteAzimuthDegrees)), 90f,
                "leaving the city should point roughly where the drive is going");
        }

        [Test]
        public void BreachAzimuth_SurvivesANullRingDefinition()
        {
            Assert.DoesNotThrow(() => FlatsSiteAuthor.BreachAzimuthDegrees(null));
        }

        // ── The resonance tell ────────────────────────────────────────────────────

        [Test]
        public void ResonanceTell_BlacksOutThenComesFullyBack()
        {
            Assert.AreEqual(1f, ResonanceTellCore.Power(-0.1f), "nothing is dimmed before the grab");
            Assert.AreEqual(0f, ResonanceTellCore.Power(0.1f), "the first beat is dead flat");
            Assert.AreEqual(1f, ResonanceTellCore.Power(ResonanceTellCore.DurationSeconds),
                "the tell must always hand power back");
            Assert.AreEqual(1f, ResonanceTellCore.Power(60f), "and stay handed back");
        }

        [Test]
        public void ResonanceTell_RecoversSmoothly_NeverSnaps()
        {
            float prev = 0f;
            for (float t = ResonanceTellCore.BlackoutSeconds; t < ResonanceTellCore.DurationSeconds; t += 0.05f)
            {
                float p = ResonanceTellCore.Power(t);
                Assert.GreaterOrEqual(p, prev - 0.001f, "recovery must be monotonic");
                Assert.LessOrEqual(p, 1f);
                prev = p;
            }
        }

        [Test]
        public void ResonanceTell_IsShortEnoughToReadAsInterference()
        {
            Assert.Less(ResonanceTellCore.DurationSeconds, 3f,
                "longer than this and a dead vehicle reads as a bug, not a tell");
            Assert.Less(ResonanceTellCore.BlackoutSeconds, ResonanceTellCore.DurationSeconds);
            Assert.IsFalse(ResonanceTellCore.IsActive(ResonanceTellCore.DurationSeconds + 0.01f));
        }
    }
}
