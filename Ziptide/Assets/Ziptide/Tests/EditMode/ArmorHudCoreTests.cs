using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Combat gained stakes today and the player had no way to see their armor. These pin the readout,
    /// because the failures here are not cosmetic: a permanent tint is eye strain in VR, and a "hurt"
    /// state the player cannot tell from a "one more hit kills you" state will make them trade when
    /// they should run.
    /// </summary>
    public class ArmorHudCoreTests
    {
        [Test]
        public void AHealthyPlayer_SeesNothingAtAll()
        {
            // THE rule. A permanent overlay is eye strain, and worse, it destroys the signal — if
            // something is always there, its arrival means nothing.
            Assert.AreEqual(0f, ArmorHudCore.Opacity01(1f), 0.0001f);
            Assert.AreEqual(0f, ArmorHudCore.CurrentOpacity(1f, broken: false, timeSeconds: 3f), 0.0001f);
        }

        [Test]
        public void TheVignette_DeepensAsArmorDrains()
        {
            float full = ArmorHudCore.Opacity01(1f);
            float most = ArmorHudCore.Opacity01(0.75f);
            float half = ArmorHudCore.Opacity01(0.5f);
            float last = ArmorHudCore.Opacity01(0.1f);

            Assert.Less(full, most);
            Assert.Less(most, half);
            Assert.Less(half, last);
        }

        [Test]
        public void TheVignette_NeverBlindsThePlayer()
        {
            // Even fully drained it stays translucent. An opaque overlay in VR is not a warning, it is
            // a reason to take the headset off.
            for (float a = 0f; a <= 1f; a += 0.05f)
                Assert.Less(ArmorHudCore.Opacity01(a), 0.6f, "armor01=" + a);
        }

        [Test]
        public void Broken_ReadsDifferentlyFromMerelyHurt()
        {
            // Three signals carry the state at once — brightness, hue and motion — so a colour-blind
            // player still reads it, and so "broken" can never be mistaken for "a bit hurt".
            ArmorHudCore.Tint(0.3f, broken: false, out float hr, out float hg, out float hb);
            ArmorHudCore.Tint(0f, broken: true, out float br, out float bg, out float bb);
            Assert.Less(bg, hg, "broken is the redder of the two — hue carries the state");
            Assert.Greater(hr, 0.5f, "both states stay warm; it is the green/blue that shift");
            Assert.Greater(br, 0.5f);
            Assert.Less(bb, hb, "…and broken drains the blue further still");

            // …and it MOVES, which hurt does not. Sampled a quarter-period apart (1/1.6 Hz ≈ 0.625 s),
            // which is the trough-to-peak of the pulse rather than two points that happen to be close.
            float p1 = ArmorHudCore.CurrentOpacity(0f, broken: true, timeSeconds: 0f);
            float p2 = ArmorHudCore.CurrentOpacity(0f, broken: true, timeSeconds: 0.156f);
            Assert.Greater(System.Math.Abs(p2 - p1), 0.05f, "the broken state visibly pulses");
        }

        [Test]
        public void TheBrokenPulse_StaysWithinItsBand_AndNeverVanishes()
        {
            for (float t = 0f; t < 6f; t += 0.017f)
            {
                float o = ArmorHudCore.BrokenPulse01(t);
                Assert.GreaterOrEqual(o, ArmorHudCore.BrokenPulseMin - 0.001f);
                Assert.LessOrEqual(o, ArmorHudCore.BrokenPulseMax + 0.001f);
            }
        }

        [Test]
        public void TheBrokenPulse_IsUrgent_ButNotAStrobe()
        {
            // Fast flashing is a seizure risk and a comfort failure. Keep it in the "heartbeat" band.
            Assert.Greater(ArmorHudCore.BrokenPulseHz, 0.8f, "slower than this reads as ambient, not urgent");
            Assert.Less(ArmorHudCore.BrokenPulseHz, 3f, "faster than this is a strobe");
        }

        [Test]
        public void BrokenAlwaysShows_EvenThoughArmorIsZeroEitherWay()
        {
            // Armor 0 and broken are the same number but not the same state; the readout must not
            // collapse them.
            float hurtAtZero = ArmorHudCore.CurrentOpacity(0f, broken: false, timeSeconds: 1f);
            float brokenAtZero = ArmorHudCore.CurrentOpacity(0f, broken: true, timeSeconds: 1f);
            Assert.Greater(hurtAtZero, 0f);
            Assert.Greater(brokenAtZero, 0f);
        }

        [Test]
        public void OutOfRangeArmor_IsClamped_NotCrashedOrInverted()
        {
            Assert.AreEqual(0f, ArmorHudCore.Opacity01(2f), 0.0001f, "over-full is still healthy");
            Assert.AreEqual(ArmorHudCore.MaxWoundedOpacity, ArmorHudCore.Opacity01(-1f), 0.0001f);
        }
    }
}
