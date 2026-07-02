using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Warden escalation contract (GAME_PLAN M3; CREATURE_DESIGN §Wardens): lawful, never an ambush —
    /// Dormant until the Signal rises, Warn always precedes Pursue, backing off de-escalates, the
    /// Ch.6 ally flag overrides everything. Pure, headless.
    /// </summary>
    public class WardenStateTests
    {
        [Test]
        public void Tier0_IsAlwaysDormant_EvenPointBlank()
        {
            var w = new WardenState();
            w.Tick(1f, signalTier: 0, dist: 0.5f, isAlly: false);
            Assert.AreEqual(WardenMode.Dormant, w.Mode);
        }

        [Test]
        public void Tier1_WatchesInRange_NeverWarns()
        {
            var w = new WardenState { WatchRange = 12f, WarnRange = 4.5f };
            w.Tick(1f, 1, dist: 8f, isAlly: false);
            Assert.AreEqual(WardenMode.Watch, w.Mode);
            w.Tick(5f, 1, dist: 1f, isAlly: false); // point blank at tier 1 — still only watching
            Assert.AreEqual(WardenMode.Watch, w.Mode);
        }

        [Test]
        public void Tier2_WarnAlwaysPrecedesPursue_AndCrowdingEscalates()
        {
            var w = new WardenState { WarnRange = 4.5f, WarnSeconds = 2f };
            w.Tick(0.5f, 2, dist: 3f, isAlly: false);
            Assert.AreEqual(WardenMode.Warn, w.Mode, "the warning is the telegraph — never straight to Pursue");
            Assert.Greater(w.WarnProgress, 0f);

            w.Tick(1.6f, 2, dist: 3f, isAlly: false); // stood ground through the window
            Assert.AreEqual(WardenMode.Pursue, w.Mode);
        }

        [Test]
        public void BackingOff_DeEscalates()
        {
            var w = new WardenState { WatchRange = 12f, WarnRange = 4.5f, WarnSeconds = 2f };
            w.Tick(1f, 2, dist: 3f, isAlly: false);
            Assert.AreEqual(WardenMode.Warn, w.Mode);
            w.Tick(0.1f, 2, dist: 8f, isAlly: false); // stepped back
            Assert.AreEqual(WardenMode.Watch, w.Mode, "lawful, not vindictive");
            Assert.AreEqual(0f, w.WarnProgress);
        }

        [Test]
        public void AllyFlag_OverridesEverything()
        {
            var w = new WardenState();
            w.Tick(5f, 3, dist: 0.5f, isAlly: true);
            Assert.AreEqual(WardenMode.Ally, w.Mode);
        }

        [Test]
        public void OutOfWatchRange_IsDormant()
        {
            var w = new WardenState { WatchRange = 12f };
            w.Tick(1f, 2, dist: 30f, isAlly: false);
            Assert.AreEqual(WardenMode.Dormant, w.Mode);
        }
    }
}
