using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins per-world difficulty scaling (COMBAT_HEALTH_PLAN Phase C): a world's tier turns creatures
    /// tougher without changing the unified damage language. Pure/headless.
    /// </summary>
    public class DifficultyScaleTests
    {
        [Test]
        public void Tier0_IsNeutral()
        {
            Assert.AreEqual(1.0f, DifficultyScale.HealthMult(0));
            Assert.AreEqual(1.0f, DifficultyScale.DamageMult(0));
            Assert.AreEqual(4, DifficultyScale.ScaledHealth(4f, 0));
        }

        [Test]
        public void HarderTiers_ScaleUp_Monotonically()
        {
            Assert.Greater(DifficultyScale.HealthMult(1), DifficultyScale.HealthMult(0));
            Assert.Greater(DifficultyScale.HealthMult(2), DifficultyScale.HealthMult(1));
            Assert.Greater(DifficultyScale.DamageMult(2), DifficultyScale.DamageMult(0));
        }

        [Test]
        public void ScaledHealth_RoundsToWholeNumbers()
        {
            Assert.AreEqual(6, DifficultyScale.ScaledHealth(4f, 1));   // 4 * 1.5
            Assert.AreEqual(8, DifficultyScale.ScaledHealth(4f, 2));   // 4 * 2.0
            Assert.AreEqual(40, DifficultyScale.ScaledHealth(20f, 2)); // the warden on a capstone world
        }

        [Test]
        public void OutOfRangeTiers_Clamp()
        {
            Assert.AreEqual(DifficultyScale.HealthMult(0), DifficultyScale.HealthMult(-3));
            Assert.AreEqual(DifficultyScale.HealthMult(DifficultyScale.MaxTier), DifficultyScale.HealthMult(99));
        }

        [Test]
        public void ScaledValues_AreNeverZero()
        {
            Assert.GreaterOrEqual(DifficultyScale.ScaledHealth(0.1f, 0), 1);
            Assert.GreaterOrEqual(DifficultyScale.ScaledDamage(0.1f, 0), 1);
        }
    }
}
