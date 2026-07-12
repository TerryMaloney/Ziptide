#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// FORGE III F3.9 — THE CONFORMANCE GATE. The "quality stays EVEN across lanes and model
    /// generations" machine (Terry's exact ask: EVENNESS). Every visible Renderer in a shipped world
    /// scene must trace to a KNOWN art provenance — a Forge-applied look (the "ForgeVisual" child), a
    /// registered kit module (<see cref="ForgeModuleLook"/>), the SkyVista rig, a water/grounding/
    /// practical author, or an explicit whitelist entry (each line carries its WHY). Everything else is
    /// ART_UNCONFORMED — a bare, un-styled renderer that a lesser-model lane left unfinished.
    ///
    /// THE RATCHET (one direction only):
    ///  • Reported as a WARN with a per-world COUNT so Terry watches the number shrink.
    ///  • When a world reaches 0 unconformed it earns a place in <see cref="ConformanceLockedWorlds"/>.
    ///  • For a LOCKED world, ANY regression (a bare untraceable renderer reappearing) is a BLOCKER —
    ///    so a world we already polished cannot quietly ship unfinished visuals again.
    ///
    /// Starts WARN-only for every world; nothing is locked yet (the list is empty). Wired into
    /// WorldAuditRunner per scene, wrapped in try/catch, so a classifier miss can never block a build.
    /// The classification and ratchet decisions are pure static helpers so CI unit-tests them directly.
    /// </summary>
    public static class ArtConformanceAuditRules
    {
        /// <summary>Worlds proven at 0 unconformed renderers — regression here is a BLOCKER.
        /// Add a scene name ONLY after the audit reports 0 for it (the ratchet, one direction).</summary>
        public static readonly string[] ConformanceLockedWorlds = new string[0];

        /// <summary>Whitelist path (scene:objectName per line; each entry line REQUIRES a WHY comment).</summary>
        public const string WhitelistPath = "Assets/Ziptide/Editor/Audit/art_conformance_whitelist.txt";

        /// <summary>GameObject names that ARE finished art provenance on their own — author output that
        /// carries no marker MonoBehaviour. Matched against the renderer's own name or any ancestor.</summary>
        private static readonly string[] ProvenanceNames =
        {
            ForgeVisualApplier.VisualChildName, // "ForgeVisual" — every Forge mesh + creature look
            GroundShadow.ChildName,             // "GroundShadow" — F3.4 blob shadow
            "Foam",                             // F3.3 water foam band
        };

        public static void Run(SceneAuditReport report)
        {
            HashSet<string> whitelist = LoadWhitelistKeys();

            int total = 0, unconformed = 0;
            var firstFew = new List<string>();
            foreach (var r in Object.FindObjectsOfType<Renderer>())
            {
                if (r == null) continue;
                total++;
                if (IsConformed(r.transform, whitelist, report.sceneName)) continue;
                unconformed++;
                if (firstFew.Count < 8) firstFew.Add(PathOf(r.transform));
            }

            if (total == 0) return; // nothing to judge (e.g. a data-only scene)

            bool locked = IsWorldLocked(report.sceneName);
            if (unconformed > 0)
            {
                string examples = firstFew.Count > 0 ? "  e.g. " + string.Join(", ", firstFew) : "";
                string msg = unconformed + " of " + total + " renderer(s) have no known art provenance " +
                    "(Forge look / kit module / SkyVista / water / grounding / practical / whitelist)." +
                    examples + "  Give each a Forge recipe, or add a whitelist line WITH its WHY.";
                if (ShouldBlock(unconformed, locked))
                    report.Blocker("ART_UNCONFORMED", "LOCKED world regressed — " + msg);
                else
                    report.Warning("ART_UNCONFORMED", msg);
            }
            else if (!locked)
            {
                // 0 unconformed but not yet locked — surface it so Terry knows this world is ratchet-ready.
                report.Warning("ART_CONFORMED_READY",
                    "0 unconformed of " + total + " renderer(s) — this world is ready to be added to " +
                    "ArtConformanceAuditRules.ConformanceLockedWorlds (the ratchet).");
            }
            // 0 unconformed AND locked = perfect: no finding at all.
        }

        /// <summary>Does the renderer's ancestry trace to a known art provenance?</summary>
        public static bool IsConformed(Transform t, ISet<string> whitelistKeys, string sceneName)
        {
            for (var p = t; p != null; p = p.parent)
            {
                string n = p.name;
                for (int i = 0; i < ProvenanceNames.Length; i++)
                    if (n == ProvenanceNames[i]) return true;
                if (whitelistKeys != null && whitelistKeys.Contains(sceneName + ":" + n)) return true;
            }
            return HasMarkerInParents(t);
        }

        /// <summary>A provenance MonoBehaviour anywhere up the parent chain marks the whole subtree as art.</summary>
        private static bool HasMarkerInParents(Transform t)
        {
            return t.GetComponentInParent<ForgeModuleLook>(true) != null
                || t.GetComponentInParent<SkyVistaRig>(true) != null
                || t.GetComponentInParent<ZiptideWater>(true) != null
                || t.GetComponentInParent<PracticalLight>(true) != null;
        }

        /// <summary>The ratchet decision: a bare renderer only BLOCKS in a world we already locked at 0.</summary>
        public static bool ShouldBlock(int unconformed, bool worldLocked)
        {
            return unconformed > 0 && worldLocked;
        }

        public static bool IsWorldLocked(string sceneName)
        {
            return IsWorldLocked(sceneName, ConformanceLockedWorlds);
        }

        /// <summary>Testable overload — locked membership against an injected list.</summary>
        public static bool IsWorldLocked(string sceneName, IEnumerable<string> lockedList)
        {
            if (lockedList == null || string.IsNullOrEmpty(sceneName)) return false;
            foreach (var w in lockedList)
                if (w == sceneName) return true;
            return false;
        }

        /// <summary>Parse whitelist lines into "scene:objectName" keys. Every entry line MUST carry a
        /// '#' WHY comment after the key, or it is REJECTED (returned via <paramref name="rejected"/>)
        /// so no one can slip in an unexplained exemption. Full-line comments and blanks are ignored.</summary>
        public static HashSet<string> ParseWhitelistKeys(IEnumerable<string> lines, out List<string> rejected)
        {
            var keys = new HashSet<string>();
            rejected = new List<string>();
            if (lines == null) return keys;

            foreach (var raw in lines)
            {
                if (raw == null) continue;
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue; // blank or full comment

                int hash = line.IndexOf('#');
                if (hash < 0) { rejected.Add(line); continue; }      // no WHY comment → reject

                string key = line.Substring(0, hash).Trim();
                string why = line.Substring(hash + 1).Trim();
                if (key.Length == 0 || why.Length == 0 || key.IndexOf(':') <= 0)
                {
                    rejected.Add(line);                               // malformed key or empty WHY → reject
                    continue;
                }
                keys.Add(key);
            }
            return keys;
        }

        private static HashSet<string> LoadWhitelistKeys()
        {
            try
            {
                if (!System.IO.File.Exists(WhitelistPath))
                    return new HashSet<string>();
                var keys = ParseWhitelistKeys(System.IO.File.ReadAllLines(WhitelistPath), out var rejected);
                foreach (var bad in rejected)
                    Debug.LogWarning("ZIPTIDE: ART_CONFORMANCE_WHITELIST_REJECTED line=\"" + bad +
                                     "\" (needs 'scene:object  # WHY').");
                return keys;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("ZIPTIDE: ART_CONFORMANCE_WHITELIST_ERROR " + ex.Message);
                return new HashSet<string>();
            }
        }

        private static string PathOf(Transform t)
        {
            var sb = new System.Text.StringBuilder(t.name);
            for (var p = t.parent; p != null; p = p.parent)
                sb.Insert(0, p.name + "/");
            return sb.ToString();
        }
    }
}
#endif
