#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors the four bot-difficulty assets into Resources/Bots (rookie/regular/veteran/nightmare)
    /// from the pure presets — create-only, so once they exist the ASSETS are the tunable truth and
    /// Terry dials a fight by editing numbers, not code. Build-wired like the other authors.
    /// </summary>
    public static class BotProfileAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Bots";

        [MenuItem("Ziptide/Worlds/Author Bot Profiles (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Bot Profiles",
                made + " profile asset(s) created under " + Folder + " (existing ones untouched).", "OK");
        }

        public static int EnsureAllAuthored()
        {
            int made = 0;
            made += Ensure("rookie", BotProfileData.Rookie);
            made += Ensure("regular", BotProfileData.Regular);
            made += Ensure("veteran", BotProfileData.Veteran);
            made += Ensure("nightmare", BotProfileData.Nightmare);
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static int Ensure(string id, BotProfileData d)
        {
            string path = Folder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<BotProfileDefinition>(path) != null) return 0;
            Directory.CreateDirectory(Folder);
            var def = ScriptableObject.CreateInstance<BotProfileDefinition>();
            def.aimErrorDegrees = d.AimErrorDegrees;
            def.reactionSeconds = d.ReactionSeconds;
            def.leadTargets = d.LeadTargets;
            def.dodgeChance = d.DodgeChance;
            def.coverDiscipline = d.CoverDiscipline;
            def.peekSeconds = d.PeekSeconds;
            def.hideSeconds = d.HideSeconds;
            def.retreatBelowHP = d.RetreatBelowHP;
            def.fireCooldownScale = d.FireCooldownScale;
            def.strafeFlipSeconds = d.StrafeFlipSeconds;
            def.engageStandoff = d.EngageStandoff;
            def.engageBand = d.EngageBand;
            def.fireRange = d.FireRange;
            def.rushRange = d.RushRange;
            def.searchSeconds = d.SearchSeconds;
            def.repositionEvery = d.RepositionEvery;
            AssetDatabase.CreateAsset(def, path);
            Debug.Log("[Ziptide] Authored bot profile " + path);
            return 1;
        }
    }
}
#endif
