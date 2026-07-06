using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    public class RillLineTests
    {
        [Test]
        public void FormatSubtitle_DefaultSpeaker_IsRill()
        {
            var line = new RillLine { id = "x", text = "Systems nominal." };
            Assert.AreEqual("RILL: Systems nominal.", line.FormatSubtitle());
        }

        [Test]
        public void FormatSubtitle_CalSpeaker_PrefixesCal()
        {
            var line = new RillLine { id = "cal_x", text = "Does that happen a lot?", speaker = "CAL" };
            Assert.AreEqual("CAL: Does that happen a lot?", line.FormatSubtitle());
        }

        [Test]
        public void Collect_MatchesRegardlessOfSpeaker()
        {
            var lib = ScriptableObject.CreateInstance<RillLineLibrary>();
            lib.lines.Add(new RillLine { id = "a", trigger = RillTrigger.WorldEnter, key = "W001", text = "Hi", speaker = "RILL" });
            lib.lines.Add(new RillLine { id = "b", trigger = RillTrigger.WorldEnter, key = "W001", text = "Hi back", speaker = "CAL" });

            var into = new List<RillLine>();
            lib.Collect(RillTrigger.WorldEnter, "W001", into);

            Assert.AreEqual(2, into.Count);
            Assert.AreEqual("RILL", into[0].speaker);
            Assert.AreEqual("CAL", into[1].speaker);
        }
    }
}
