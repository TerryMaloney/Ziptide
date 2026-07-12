using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE LEAK RATCHET (runtime-health layer). Unity never auto-destroys runtime-created
    /// Materials/Textures/AudioClips — a file that creates them and never Destroys anything is a
    /// leak candidate. This gate scans the runtime source: every creator file must either show
    /// destroy discipline or sit on the VISIBLE exemption ledger below with a reason. Adding a new
    /// creator file without cleanup turns CI red and points here — the debt can only shrink.
    /// (The orphans the ledger files DO make are swept by RuntimeHealthMonitor's post-travel
    /// janitor; the ledger documents that that's a decision, not an accident.)
    /// Plus: the FrameStats pure-core contract.
    /// </summary>
    public class ResourceDisciplineTests
    {
        // Reviewed 2026-07-10: these create resources with scene/app lifetime (shared factory mats,
        // persistent rigs) and rely on the travel janitor to sweep what falls out of use. Shrink me.
        private static readonly HashSet<string> ExemptionLedger = new HashSet<string>
        {
            "Core/Runtime/RuntimeMaterialFixer.cs",          // boot-time repair pass, app lifetime
            "Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs",
            "Gameplay/Runtime/Inventory/BeltRig.cs",         // one persistent rig per session
            "Gameplay/Runtime/Items/ItemFactory.cs",         // THE factory — mats live with their items
            "Gameplay/Runtime/Pvp/PvpBot.cs",
            "Gameplay/Runtime/Story/ChoiceStation.cs",
            "Gameplay/Runtime/Story/TransmissionConsole.cs",
            "Gameplay/Runtime/Weapons/GravityGunRuntime.cs",
            "Gameplay/Runtime/Weapons/GunLaserSight.cs",
            "Visuals/Runtime/Forge/ForgeMaterials.cs",       // registry-cached, deliberately shared
            "Visuals/Runtime/Grounding/GroundShadow.cs",     // ONE shared blob-shadow mat/tex/quad, app lifetime
        };

        private static readonly string[] CreatorMarkers = { "new Material(", "new Texture2D(", "AudioClip.Create(" };

        [Test]
        public void EveryRuntimeResourceCreator_CleansUp_OrIsOnTheLedger()
        {
            string root = Path.Combine(Application.dataPath, "Ziptide");
            var offenders = new List<string>();
            var ledgerHits = new HashSet<string>();

            foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string norm = file.Replace('\\', '/');
                if (norm.Contains("/Editor/") || norm.Contains("/Tests/")) continue;
                string text = File.ReadAllText(file);
                bool creates = false;
                foreach (var marker in CreatorMarkers)
                    if (text.Contains(marker)) { creates = true; break; }
                if (!creates) continue;

                string rel = norm.Substring(norm.IndexOf("Assets/Ziptide/") + "Assets/Ziptide/".Length);
                if (text.Contains("Destroy(")) continue;          // shows discipline
                if (ExemptionLedger.Contains(rel)) { ledgerHits.Add(rel); continue; }
                offenders.Add(rel);
            }

            Assert.IsEmpty(offenders,
                "New runtime resource creator(s) with no Destroy discipline:\n  " +
                string.Join("\n  ", offenders) +
                "\nEither Destroy what you create (OnDestroy is the usual home) or add the file to " +
                "the ExemptionLedger in ResourceDisciplineTests WITH a reviewed reason.");

            // The ledger may only shrink — stale entries are removed so the debt count stays honest.
            var stale = new List<string>();
            foreach (var entry in ExemptionLedger)
                if (!ledgerHits.Contains(entry)) stale.Add(entry);
            Assert.IsEmpty(stale, "Ledger entries no longer needed (file gained cleanup or moved) — " +
                "delete them so the ratchet stays tight:\n  " + string.Join("\n  ", stale));
        }

        // ── FrameStats pure-core contract ────────────────────────────────────
        [Test]
        public void FrameStats_MathHolds()
        {
            var s = new FrameStats(100);
            for (int i = 0; i < 90; i++) s.Push(10f);
            for (int i = 0; i < 10; i++) s.Push(20f);
            Assert.AreEqual(100, s.Count);
            Assert.AreEqual(11f, s.AverageMs, 0.01f);
            Assert.AreEqual(20f, s.WorstMs, 0.001f);
            Assert.AreEqual(10, s.DroppedFrames(13.9f), "the ten 20ms frames blow the 72Hz budget");
            Assert.AreEqual(20f, s.PercentileMs(0.99f), 0.001f);
            Assert.AreEqual(50f, s.OnePercentLowFps, 0.5f, "1% low reads the slow tail as FPS");
        }

        [Test]
        public void FrameStats_RingWraps_AndResets()
        {
            var s = new FrameStats(50);
            for (int i = 0; i < 200; i++) s.Push(5f);   // wraps 4×
            Assert.AreEqual(50, s.Count);
            Assert.AreEqual(5f, s.AverageMs, 0.001f);
            s.Push(50f);                                  // one spike lands in the ring
            Assert.AreEqual(1, s.DroppedFrames(13.9f));
            s.Reset();
            Assert.AreEqual(0, s.Count);
            Assert.AreEqual(0f, s.AverageMs);
            s.Push(-3f);                                  // garbage in → ignored
            Assert.AreEqual(0, s.Count);
        }
    }
}
