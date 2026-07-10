using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE DANGLING-HANDLER RATCHET (crash-proofing sweep, follow-up). A destroyed object still
    /// subscribed to a static event (or SceneManager.sceneLoaded) becomes a dangling handler —
    /// MissingReferenceException minutes after the real cause, the nastiest crash class to debug.
    /// Today the codebase is CLEAN (every subscriber unsubscribes, verified 2026-07-10); this gate
    /// keeps it that way:
    ///   · Static events are DISCOVERED from the source (no hand-kept list to go stale) — any
    ///     runtime file that subscribes must also unsubscribe, or sit on the ledger with a reason.
    ///   · Same rule for SceneManager.sceneLoaded. Static one-shot boot hooks (a static method
    ///     subscribed once for app lifetime — nothing to dangle) are the legitimate exemption.
    /// </summary>
    public class EventHygieneTests
    {
        // Reviewed static-lifetime subscribers: a STATIC method hooked once at boot can never
        // dangle (no instance to die). Anything else added here needs the same justification.
        private static readonly HashSet<string> SceneLoadedLedger = new HashSet<string>
        {
            "Gameplay/Runtime/World/ConquestMissionRuntime.cs",  // [RuntimeInitializeOnLoadMethod] static hook
            "Gameplay/Runtime/Enemies/EcologyDirector.cs",       // static boot hook (Reasonbox 4.3)
            "Core/Runtime/GamePool.cs",                          // static boot hook
        };

        private static readonly HashSet<string> StaticEventLedger = new HashSet<string>
        {
            // empty today — the codebase is clean; keep it that way
        };

        private static readonly Regex StaticEventDecl =
            new Regex(@"static\s+event\s+[^;=]+?(\w+)\s*;", RegexOptions.Compiled);

        [Test]
        public void EveryStaticEventSubscriber_AlsoUnsubscribes()
        {
            var files = RuntimeSources();

            // 1. Discover every static event name from the source itself.
            var eventNames = new HashSet<string>();
            foreach (var kv in files)
                foreach (Match m in StaticEventDecl.Matches(kv.Value))
                    eventNames.Add(m.Groups[1].Value);
            Assert.IsNotEmpty(eventNames, "discovery broke — the project has static events");

            // 2. Every subscriber file shows the matching unsubscribe.
            var offenders = new List<string>();
            foreach (var name in eventNames)
                foreach (var kv in files)
                {
                    if (!kv.Value.Contains(name + " +=")) continue;
                    if (kv.Value.Contains(name + " -=")) continue;
                    if (StaticEventLedger.Contains(kv.Key)) continue;
                    offenders.Add(kv.Key + "  (subscribes " + name + ", never unsubscribes)");
                }

            Assert.IsEmpty(offenders,
                "Dangling-handler risk — static-event subscriptions with no matching -=:\n  " +
                string.Join("\n  ", offenders) +
                "\nUnsubscribe in OnDestroy, or (ONLY for a static method hooked once at boot) add " +
                "the file to the ledger in EventHygieneTests with the justification.");
        }

        [Test]
        public void EverySceneLoadedSubscriber_AlsoUnsubscribes_OrIsAStaticBootHook()
        {
            var offenders = new List<string>();
            var ledgerHits = new HashSet<string>();
            foreach (var kv in RuntimeSources())
            {
                if (!kv.Value.Contains("sceneLoaded +=")) continue;
                if (kv.Value.Contains("sceneLoaded -=")) continue;
                if (SceneLoadedLedger.Contains(kv.Key)) { ledgerHits.Add(kv.Key); continue; }
                offenders.Add(kv.Key);
            }
            Assert.IsEmpty(offenders,
                "sceneLoaded subscriptions with no unsubscribe (dangling-handler risk):\n  " +
                string.Join("\n  ", offenders));

            var stale = new List<string>();
            foreach (var entry in SceneLoadedLedger)
                if (!ledgerHits.Contains(entry)) stale.Add(entry);
            Assert.IsEmpty(stale, "Ledger entries no longer needed — delete them (the ratchet only tightens):\n  " +
                string.Join("\n  ", stale));
        }

        private static Dictionary<string, string> RuntimeSources()
        {
            string root = Path.Combine(Application.dataPath, "Ziptide");
            var result = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string norm = file.Replace('\\', '/');
                if (norm.Contains("/Editor/") || norm.Contains("/Tests/")) continue;
                string rel = norm.Substring(norm.IndexOf("Assets/Ziptide/") + "Assets/Ziptide/".Length);
                result[rel] = File.ReadAllText(file);
            }
            return result;
        }
    }
}
