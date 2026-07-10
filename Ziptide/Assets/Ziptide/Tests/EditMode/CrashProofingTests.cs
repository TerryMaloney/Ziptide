using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using Ziptide.Core;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The ship-killer sweep's gates (Terry: "we can build all the worlds we want but if everything
    /// breaks we're screwed"): ① the atomic save store survives every interruption shape we can
    /// simulate — truncated main, missing main, corrupt-both; ② TryDeserialize tells corrupt from
    /// fine (incl. the hollow-parse trap); ③ SCENE COVERAGE — every scene the game can travel to
    /// must exist in Build Settings, because LoadScene on a missing scene silently no-ops.
    /// </summary>
    public class CrashProofingTests
    {
        // ── ① The atomic store: corrupt-and-recover on real temp files ──────
        private string _dir;
        private string P(string name) => Path.Combine(_dir, name);

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "ziptide_saveproof_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        [TearDown]
        public void TearDown()
        {
            try { Directory.Delete(_dir, true); } catch { /* temp cleanup best-effort */ }
        }

        private static bool LooksUsable(string text) => text != null && text.StartsWith("GOOD");

        [Test]
        public void WriteAtomic_KeepsThePreviousVersionAsBackup()
        {
            string path = P("profile.json");
            SaveFileStore.WriteAtomic(path, "GOOD v1");
            SaveFileStore.WriteAtomic(path, "GOOD v2");
            Assert.AreEqual("GOOD v2", File.ReadAllText(path));
            Assert.AreEqual("GOOD v1", File.ReadAllText(SaveFileStore.BakPath(path)), "previous version demoted, never lost");
            Assert.IsFalse(File.Exists(SaveFileStore.TmpPath(path)), "no tmp litter");
        }

        [Test]
        public void CorruptMain_RecoversFromBackup()
        {
            string path = P("profile.json");
            SaveFileStore.WriteAtomic(path, "GOOD v1");
            SaveFileStore.WriteAtomic(path, "GOOD v2");
            File.WriteAllText(path, "GARB");   // the battery died mid-write
            string best = SaveFileStore.ReadBestVersion(path, LooksUsable, out bool fromBackup);
            Assert.AreEqual("GOOD v1", best, "the previous good version comes back");
            Assert.IsTrue(fromBackup);
        }

        [Test]
        public void MissingEverything_ReturnsNull_NeverThrows()
        {
            string best = SaveFileStore.ReadBestVersion(P("nothing.json"), LooksUsable, out bool fromBackup);
            Assert.IsNull(best);
            Assert.IsFalse(fromBackup);

            SaveFileStore.WriteAtomic(P("both.json"), "GOOD");
            File.WriteAllText(P("both.json"), "GARB");            // main corrupt, no bak yet
            best = SaveFileStore.ReadBestVersion(P("both.json"), LooksUsable, out _);
            Assert.IsNull(best, "corrupt main with no backup = honest null (fresh start), not a crash");
        }

        // ── ② TryDeserialize knows corrupt from fine ─────────────────────────
        [Test]
        public void TryDeserialize_TellsCorruptFromFine()
        {
            var real = ProfileSerializer.NewProfile();
            real.SetFlag("TEST_FLAG");
            string good = ProfileSerializer.Serialize(real);

            Assert.IsTrue(ProfileSerializer.TryDeserialize(good, out var parsed));
            Assert.IsTrue(parsed.HasFlag("TEST_FLAG"));

            Assert.IsFalse(ProfileSerializer.TryDeserialize(null, out _));
            Assert.IsFalse(ProfileSerializer.TryDeserialize("", out _));
            Assert.IsFalse(ProfileSerializer.TryDeserialize("not json at all", out _));
            Assert.IsFalse(ProfileSerializer.TryDeserialize("{}", out _),
                "a hollow parse (no playerId) is corruption, not a profile");
            // The public Deserialize contract is unchanged: garbage still yields a fresh profile.
            Assert.IsNotNull(ProfileSerializer.Deserialize("not json at all"));
        }

        // ── ③ Scene coverage: every travel target exists in Build Settings ──
        // Pending first bake (Terry's runbook items) — allowed to be absent; delete an entry here
        // once its scene is baked + committed so the gate tightens behind it.
        private static readonly HashSet<string> PendingFirstBake = new HashSet<string>
        {
            "Cavern_TestLab", "W011_Undercroft", "SpaceLane_Trial",
        };

        [Test]
        public void EveryTravelTarget_IsInBuildSettings()
        {
            var inBuild = new HashSet<string>();
            foreach (var s in EditorBuildSettings.scenes)
                if (s.enabled) inBuild.Add(Path.GetFileNameWithoutExtension(s.path));

            var required = new List<string>
            {
                ZiptideConstants.SceneBoot, ZiptideConstants.SceneW000, ZiptideConstants.SceneSandbox,
            };
            foreach (var w in ConquestGalaxy.ChapterOneTwoSeeds())
                required.Add(w.SceneName);   // the war table can travel to EVERY one of these

            var missing = new List<string>();
            foreach (var scene in required)
                if (!inBuild.Contains(scene) && !PendingFirstBake.Contains(scene))
                    missing.Add(scene);

            Assert.IsEmpty(missing,
                "Travel targets missing from Build Settings (LoadScene would silently no-op):\n  " +
                string.Join("\n  ", missing) +
                "\nAdd the scene to Build Settings (or, for a genuinely unbaked world, to PendingFirstBake with a runbook step).");
        }
    }
}
