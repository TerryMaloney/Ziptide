using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// De-garble playback contract (GAME_PLAN M1): every clarity tier renders a distinct, non-empty
    /// text; the register arc lands (static → addressed → you → I → the name). Pure, headless.
    /// </summary>
    public class TransmissionTextTests
    {
        [Test]
        public void EveryTier_RendersDistinctNonEmptyText()
        {
            var seen = new System.Collections.Generic.HashSet<string>();
            for (int tier = 0; tier <= TransmissionProgress.TierMax; tier++)
            {
                string text = TransmissionText.Render(tier);
                Assert.IsFalse(string.IsNullOrEmpty(text), "tier " + tier + " is empty");
                Assert.IsTrue(seen.Add(text), "tier " + tier + " duplicates another tier's text");
            }
        }

        [Test]
        public void RegisterArc_LandsAtTheRightTiers()
        {
            StringAssert.Contains("degraded", TransmissionText.Render(0));            // pure static
            StringAssert.Contains("addressed to", TransmissionText.Render(1));        // professional
            StringAssert.Contains("you", TransmissionText.Render(2));                 // confessional
            StringAssert.Contains("I did this", TransmissionText.Render(3));          // intimate
            StringAssert.Contains("Cal", TransmissionText.Render(TransmissionProgress.TierMax)); // the name
        }

        [Test]
        public void OutOfRangeTiers_ClampSafely()
        {
            Assert.AreEqual(TransmissionText.Render(TransmissionProgress.TierMax), TransmissionText.Render(99));
            Assert.AreEqual(TransmissionText.Render(0), TransmissionText.Render(-3));
        }
    }
}
