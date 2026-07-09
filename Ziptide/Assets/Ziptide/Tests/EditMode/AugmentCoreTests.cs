using NUnit.Framework;
using Ziptide.Gameplay;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Tests.EditMode
{
    /// <summary>A4.5/4.6/4.7 pure layer: the augment clock, the slot rule, the effect hooks, the
    /// shared charge pool, and the locator's tiers/afterglow/Sixth-Sense scale. Headless.</summary>
    public class AugmentCoreTests
    {
        [TearDown]
        public void ResetStatics() => AugmentEffects.ResetAll(); // never leak a hook into another test

        // ── AugmentClock ─────────────────────────────────────────────────────
        [Test]
        public void Clock_ActiveThenCooldown_ThenReadyAgain()
        {
            var c = new AugmentClock(cooldownSeconds: 20, durationSeconds: 2);
            Assert.IsTrue(c.TryActivate(100));
            Assert.IsTrue(c.IsActive(101), "1s in — effect running");
            Assert.IsFalse(c.IsActive(102.5), "2.5s in — effect over");
            Assert.IsFalse(c.TryActivate(105), "cooldown runs AFTER the effect (until t=122)");
            Assert.IsFalse(c.TryActivate(121.9));
            Assert.IsTrue(c.TryActivate(122.1), "ready again");
        }

        [Test]
        public void Clock_ZeroDuration_GoesStraightToCooldown()
        {
            var c = new AugmentClock(cooldownSeconds: 8, durationSeconds: 0); // Surge Dash shape
            Assert.IsTrue(c.TryActivate(10));
            Assert.IsFalse(c.IsActive(10.01), "instant burst — never 'active'");
            Assert.IsFalse(c.TryActivate(17.9));
            Assert.IsTrue(c.TryActivate(18.1));
        }

        [Test]
        public void Clock_ReadyProgress_FillsOverTheCooldown()
        {
            var c = new AugmentClock(cooldownSeconds: 10, durationSeconds: 0);
            c.TryActivate(0);
            Assert.AreEqual(0.5f, c.ReadyProgress(5), 0.01f);
            Assert.AreEqual(1f, c.ReadyProgress(10.1), 0.01f);
        }

        // ── AugmentLoadout ───────────────────────────────────────────────────
        [Test]
        public void Loadout_OneActiveOnePassive_SwapReturnsTheDisplaced()
        {
            var l = new AugmentLoadout();
            Assert.IsNull(l.Equip(AugmentKind.Active, "surge_dash"));
            Assert.IsNull(l.Equip(AugmentKind.Passive, "sure_step"));
            Assert.AreEqual("surge_dash", l.Equip(AugmentKind.Active, "bubble_guard"),
                "equipping a second active SWAPS — slot scarcity is the balance");
            Assert.AreEqual("bubble_guard", l.ActiveId);
            Assert.AreEqual("sure_step", l.PassiveId, "the passive slot is untouched");
            Assert.IsTrue(l.Has("sure_step"));
            Assert.IsFalse(l.Has("surge_dash"), "displaced = no longer equipped");
        }

        // ── SharedChargePool (A4.6 heart) ────────────────────────────────────
        [Test]
        public void SharedPool_BothHands_DrainTheSamePool()
        {
            var p = new SharedChargePool(maxCharges: 2, rechargeSeconds: 1.5);
            Assert.IsTrue(p.TryFire(0));   // left hand
            Assert.IsTrue(p.TryFire(0.1)); // right hand — same pool
            Assert.IsFalse(p.TryFire(0.2), "two hands, TWO shots total — dual-wield adds no DPS");
            Assert.IsTrue(p.CanFire(1.8), "recharged together");
        }

        // ── LocatorState A4.7 ────────────────────────────────────────────────
        [Test]
        public void Locator_Tiers_CarryTheSpecTimings()
        {
            Assert.AreEqual(60.0, LocatorState.ForTier(1).CooldownSeconds, 1e-6);
            Assert.AreEqual(0.0, LocatorState.ForTier(1).AfterglowSeconds, 1e-6, "tier 1 = no trail");
            Assert.AreEqual(45.0, LocatorState.ForTier(2).CooldownSeconds, 1e-6);
            Assert.AreEqual(8.0, LocatorState.ForTier(2).AfterglowSeconds, 1e-6);
            Assert.AreEqual(30.0, LocatorState.ForTier(3).CooldownSeconds, 1e-6);
        }

        [Test]
        public void Locator_Afterglow_OpensOnPing_AndFades()
        {
            var s = new LocatorState(holdSeconds: 1.0, cooldownSeconds: 10.0, afterglowSeconds: 8.0);
            Assert.IsFalse(s.InAfterglow(0));
            // Hold to the ping.
            bool pinged = false;
            for (double t = 0; t <= 1.05 && !pinged; t += 0.05) pinged = s.Tick(t, 0.05, held: true);
            Assert.IsTrue(pinged);
            Assert.IsTrue(s.InAfterglow(5), "5s later the trail still glows");
            Assert.Greater(s.AfterglowRemaining(2), s.AfterglowRemaining(7), "and it FADES");
            Assert.IsFalse(s.InAfterglow(12), "expired");
        }

        [Test]
        public void Locator_SixthSense_HalvesTheCooldown_AtPingTime()
        {
            var s = new LocatorState(holdSeconds: 1.0, cooldownSeconds: 10.0);
            AugmentEffects.LocatorCooldownScale = 0.5f;
            bool pinged = false;
            for (double t = 0; t <= 1.05 && !pinged; t += 0.05) pinged = s.Tick(t, 0.05, held: true);
            Assert.IsTrue(pinged);
            Assert.IsFalse(s.Tick(4.0, 0.05, held: true), "still cooling at 4s...");
            s.Tick(6.2, 0.05, held: false); // ping fired at ~1s + 5s scaled cooldown → ready ~6s
            Assert.IsFalse(s.OnCooldown, "ready at ~6s, not ~11 — Sixth Sense halved it");
        }

        // ── AugmentEffects hooks ─────────────────────────────────────────────
        [Test]
        public void Effects_ResetAll_RestoresEveryDefault()
        {
            AugmentEffects.WeaponCooldownScale = 0.5f;
            AugmentEffects.SlowResistance = 0.5f;
            AugmentEffects.LocatorCooldownScale = 0.5f;
            AugmentEffects.MagnetReachMeters = 3f;
            AugmentEffects.ResetAll();
            Assert.AreEqual(1f, AugmentEffects.WeaponCooldownScale);
            Assert.AreEqual(0f, AugmentEffects.SlowResistance);
            Assert.AreEqual(1f, AugmentEffects.LocatorCooldownScale);
            Assert.AreEqual(0f, AugmentEffects.MagnetReachMeters);
        }
    }
}
