#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// THE NOTHING-SHIPS-INVISIBLE GATE (Test Day 1: gardens/mechanics existed in packs but Terry
    /// saw none). For the audited scene's WorldPackDefinition: every gardens/sockets/mines entry
    /// must have physical presence the player can find — for gardens that means planter pads in
    /// the scene (the runtime attaches to them). WARNINGS for now (blockers after the content
    /// stabilizes) so the succession window can't be broken by a content gap.
    /// </summary>
    public static class WorldContentAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            var pack = UnityEditor.AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(
                "Assets/Ziptide/Content/Worlds/Packs/" + report.sceneName + "_WorldPack.asset");
            if (pack == null) return;

            int planters = 0;
            foreach (var t in Object.FindObjectsOfType<Transform>())
                if (t != null && t.name.StartsWith("Planter")) planters++;

            int gardens = pack.gardens != null ? pack.gardens.Count : 0;
            if (gardens > 0 && planters == 0)
                report.Warning("WORLD_CONTENT_GARDEN_UNANCHORED",
                    "Pack '" + pack.packId + "' defines " + gardens + " garden(s) but the scene has no " +
                    "Planter pads — the runtime plots will float on nothing the player can recognize.");

            int sockets = pack.sockets != null ? pack.sockets.Count : 0;
            int socketObjects = 0;
            foreach (var s in Object.FindObjectsOfType<Ziptide.Gameplay.BuildSocketRuntime>())
                if (s != null) socketObjects++;
            if (sockets > 0 && socketObjects == 0)
                report.Warning("WORLD_CONTENT_SOCKET_MISSING",
                    "Pack '" + pack.packId + "' defines " + sockets + " build socket(s) but the scene " +
                    "contains no BuildSocketRuntime — the mechanic is unreachable in this world.");
        }
    }
}
#endif
