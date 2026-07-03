using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Subtitle word-wrap contract (Quality Bar P0.3): no output line exceeds the width, words are
    /// never split, hard newlines survive, and degenerate inputs are safe. Pure, headless.
    /// </summary>
    public class SubtitleTextTests
    {
        private static void AssertNoLineExceeds(string wrapped, int max)
        {
            foreach (string line in wrapped.Split('\n'))
                Assert.LessOrEqual(line.Length, max, "line too wide: '" + line + "'");
        }

        [Test]
        public void LongLine_WrapsWithinWidth_AndKeepsEveryWord()
        {
            string text = "I have been awake for six minutes and I already have opinions about this world.";
            string wrapped = SubtitleText.Wrap(text, 38);
            AssertNoLineExceeds(wrapped, 38);
            Assert.AreEqual(text, wrapped.Replace('\n', ' '), "words lost or reordered");
        }

        [Test]
        public void ShortLine_IsUntouched()
        {
            Assert.AreEqual("Systems nominal. I think.", SubtitleText.Wrap("Systems nominal. I think.", 38));
        }

        [Test]
        public void HardNewlines_SurviveAsBreaks()
        {
            string wrapped = SubtitleText.Wrap("line one\nline two", 38);
            Assert.AreEqual("line one\nline two", wrapped);
        }

        [Test]
        public void OverlongSingleWord_StaysIntactOnItsOwnLine()
        {
            string wrapped = SubtitleText.Wrap("a supercalifragilisticexpialidocious word", 10);
            StringAssert.Contains("supercalifragilisticexpialidocious", wrapped);
            Assert.IsFalse(wrapped.Contains("supercalifragilistic\n"), "word was split");
        }

        [Test]
        public void DegenerateInputs_AreSafe()
        {
            Assert.AreEqual("", SubtitleText.Wrap(null));
            Assert.AreEqual("", SubtitleText.Wrap(""));
            Assert.AreEqual("abc", SubtitleText.Wrap("abc", 0)); // nonsense width = no-op
        }
    }
}
