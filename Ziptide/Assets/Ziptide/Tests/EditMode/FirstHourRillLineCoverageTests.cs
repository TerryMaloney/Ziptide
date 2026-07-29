using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The first hour shipped with THREE authored lines covering both of its worlds, and none of the
    /// 15 teaching lines the beat contract names. FH-S08's acceptance is literally "15 line IDs
    /// resolve", so this test reads the contract and the author and refuses to let them drift apart.
    /// </summary>
    public sealed class FirstHourRillLineCoverageTests
    {
        private static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));

        private static string ReadRepoFile(string relative)
        {
            string path = Path.Combine(RepoRoot, relative.Replace('/', Path.DirectorySeparatorChar));
            Assert.That(File.Exists(path), Is.True, "Missing: " + relative);
            return File.ReadAllText(path);
        }

        /// <summary>Every rillLine id the beat contract declares, read straight out of the JSON.</summary>
        private static List<string> ContractLineIds()
        {
            string json = ReadRepoFile("docs/first_hour/first_hour_beats.json");
            // Only the rillLine blocks — the beats also carry TUT_* names as taught-verb flags, and
            // those are not deliverable lines.
            var ids = new List<string>();
            foreach (Match m in Regex.Matches(json,
                         "\"rillLine\"\\s*:\\s*\\{[^}]*?\"id\"\\s*:\\s*\"(TUT_[A-Z0-9_]+)\"",
                         RegexOptions.Singleline))
            {
                string id = m.Groups[1].Value;
                if (!ids.Contains(id)) ids.Add(id);
            }
            return ids;
        }

        [Test]
        public void EveryTeachingLineInTheContract_IsAuthored()
        {
            var ids = ContractLineIds();
            Assert.AreEqual(15, ids.Count,
                "the contract declares exactly one teaching line per core verb");

            string author = ReadRepoFile("Ziptide/Assets/Ziptide/Editor/Patching/RillLineAuthor.cs");
            foreach (string id in ids)
                StringAssert.Contains("Cue(\"" + id + "\"", author,
                    "teaching line " + id + " is named by the beat contract but never authored");
        }

        [Test]
        public void TeachingLinesUseTheCueTrigger_SoAConfidentPlayerNeverHearsThem()
        {
            string author = ReadRepoFile("Ziptide/Assets/Ziptide/Editor/Patching/RillLineAuthor.cs");
            StringAssert.Contains("trigger = RillTrigger.Cue", author);
            // A teaching line must not be reachable by simply loading the world or gaining a flag.
            StringAssert.DoesNotContain("Enter(\"TUT_", author);
            StringAssert.DoesNotContain("Flag(\"TUT_", author);
        }

        [Test]
        public void TheTutorialsOwnBeats_HaveAVoice()
        {
            string author = ReadRepoFile("Ziptide/Assets/Ziptide/Editor/Patching/RillLineAuthor.cs");
            // W000 exists to grant these three flags and, before this, said nothing when it did.
            StringAssert.Contains("ZiptideFlags.TUTORIAL_COMPLETE,", author);
            StringAssert.Contains("ZiptideFlags.FIRST_TRAVEL,", author);
            StringAssert.Contains("ZiptideFlags.C1_W001_ARRIVED,", author);
            // The W000 -> W001 launch is the tutorial's final beat; it used to fall through to the
            // generic wildcard gate pool.
            StringAssert.Contains("gate_toxiccity", author);
        }

        [Test]
        public void TryGetById_FindsALineAndRejectsMissingOrBlankIds()
        {
            var lib = ScriptableObject.CreateInstance<RillLineLibrary>();
            lib.lines.Add(new RillLine { id = "TUT_LOOK_RILL", trigger = RillTrigger.Cue, text = "Over here." });

            Assert.IsTrue(lib.TryGetById("TUT_LOOK_RILL", out RillLine found));
            Assert.AreEqual("Over here.", found.text);

            Assert.IsFalse(lib.TryGetById("TUT_NOT_A_LINE", out RillLine missing));
            Assert.IsNull(missing);
            Assert.IsFalse(lib.TryGetById(null, out _));
            Assert.IsFalse(lib.TryGetById(string.Empty, out _));

            Object.DestroyImmediate(lib);
        }

        [Test]
        public void CueLines_DoNotLeakIntoWorldEnterOrGateCollection()
        {
            var lib = ScriptableObject.CreateInstance<RillLineLibrary>();
            lib.lines.Add(new RillLine { id = "TUT_PUNCH_IT", trigger = RillTrigger.Cue, key = "TUT_PUNCH_IT", text = "Punch it." });
            lib.lines.Add(new RillLine { id = "enter", trigger = RillTrigger.WorldEnter, key = "TUT_PUNCH_IT", text = "Hello." });

            var into = new List<RillLine>();
            lib.Collect(RillTrigger.WorldEnter, "TUT_PUNCH_IT", into);

            Assert.AreEqual(1, into.Count, "a Cue line must only ever be delivered by id");
            Assert.AreEqual("enter", into[0].id);

            Object.DestroyImmediate(lib);
        }
    }
}
