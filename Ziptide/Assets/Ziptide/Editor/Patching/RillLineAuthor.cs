#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors Resources/Story/RillLines.asset — RILL's subtitle lines (GAME_PLAN M1). CODE IS THE
    /// SOURCE OF TRUTH: the 12 canonical arc beats (MASTER_BUILD_PLAN §5.2, exact lines) + one entry
    /// line per authored world, register matched to RILL's memory state at that point (Dormant =
    /// terse/functional, Stirring = questions — STORY_BIBLE). Beats for worlds that don't exist yet
    /// (W013+) are authored NOW and simply fire when those flags start being granted (M5) — content
    /// and systems grow together. TO CHANGE A LINE: edit it here; the asset regenerates every build.
    /// Wired into BuildAndroid next to the other data authors; also runnable from the menu.
    /// </summary>
    public static class RillLineAuthor
    {
        private const string AssetPath = "Assets/Ziptide/Resources/Story/RillLines.asset";

        [MenuItem("Ziptide/Story/Author RILL Lines")]
        public static void EnsureAuthored()
        {
            string dir = Path.GetDirectoryName(AssetPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var lib = AssetDatabase.LoadAssetAtPath<RillLineLibrary>(AssetPath);
            if (lib == null)
            {
                lib = ScriptableObject.CreateInstance<RillLineLibrary>();
                AssetDatabase.CreateAsset(lib, AssetPath);
            }

            lib.lines = BuildLines();
            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] RILL line library authored: " + lib.lines.Count + " lines → " + AssetPath);
        }

        private static List<RillLine> BuildLines()
        {
            var L = new List<RillLine>();
            void Enter(string id, string scene, string text) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.WorldEnter, key = scene, text = text });
            void Flag(string id, string flag, string text) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.FlagSet, key = flag, text = text });

            // ── The 12 canonical arc beats (MASTER_BUILD_PLAN §5.2 — exact lines; VO priority) ──────
            Enter("beat01_boot", "ToxicCity", "Systems nominal. I think.");                              // 1. W001
            Flag("beat02_cargo", ZiptideFlags.C1_W004_RILL_ASKED_CARGO,
                 "What are you carrying that requires containment?");                                    // 2. W004
            Flag("beat03_misidentify", ZiptideFlags.C2_W009_RILL_MISIDENTIFIED,
                 "I know this—wait, I do not. That is new.");                                            // 3. W009
            Flag("beat04_shard", ZiptideFlags.C3_W013_MEMORY_SHARD,
                 "I was here. Or something that used to be me was here.");                               // 4. W013
            Flag("beat05_refusal", ZiptideFlags.C3_W019_RILL_REFUSED,
                 "I cannot help you break this seal. Ask me why.");                                      // 5. W019
            Flag("beat06_color", ZiptideFlags.C4_W024_COLOR_NAMED,
                 "That is the color I have been trying to name for 40,000 years.");                      // 6. W024
            Flag("beat07_unprompted", ZiptideFlags.C4_W028_NO_JOB,
                 "Some worlds do not want to be completed. This is one of them.");                       // 7. W028
            Flag("beat08_staredown", ZiptideFlags.C5_W037_WARDEN_STANDOFF,
                 "It recognizes me. Interesting.");                                                      // 8. W037
            Flag("beat09_pattern", ZiptideFlags.C6_W039_PATTERN_WARNING,
                 "It is not spreading toward you. It is spreading toward your memories.");               // 9. W039
            Flag("beat10_name", ZiptideFlags.C6_W051_RILL_NAMED,
                 "I have carried a designation for 40,000 years. Today I would rather have a name.");    // 10. W051
            Flag("beat11_revelation", ZiptideFlags.C8_W062_REVELATION,
                 "The Architects did not build this to keep stories in. They built it to keep something out."); // 11. W062
            Flag("beat12_endA", ZiptideFlags.C12_W063_ENDING_A,
                 "The network needs a witness. I was built for this. Go home, Cal.");                    // 12. endings
            Flag("beat12_endB", ZiptideFlags.C12_W063_ENDING_B,
                 "Wherever you are crossing to — I am coming with you.");
            Flag("beat12_endC", ZiptideFlags.C12_W063_ENDING_C,
                 "Let me forget. Some memories are heavier than the cage that held them.");
            Flag("beat12_endD", ZiptideFlags.C12_W063_ENDING_D,
                 "I understand the Pattern now. It was never the enemy. It was the first of us.");

            // ── Chapter 1–2 world-entry lines (register: Dormant = terse; Stirring = questions) ─────
            Enter("enter_w002", "W002_DryCistern", "Cistern registry says these pumps died forty years ago. Contract says otherwise.");
            Enter("enter_w003", "W003_GlassShelf", "Wind advisory. The baffles are down. That is the whole briefing.");
            Enter("enter_w004", "W004_BroadcastTomb", "The broadcast spine is dark. Something here was transmitting long before the Guild arrived.");
            Enter("enter_w005", "W005_OxidizedCanopy", "The canopy is growing through the machines. Do you think it minds us fixing them?");
            Enter("enter_w006", "W006_MirrorFlats", "No lifesigns. No wind. Why does an empty world need this much light?");
            Enter("enter_w007", "W007_SableStation", "This station is not on my charts. The person running it prefers it that way.");
            Enter("enter_w008", "W008_SealedArchive", "An archive that was sealed from the inside. I have questions. I suspect it has answers.");
            Enter("enter_w009", "W009_Chitinwall", "The wall is not architecture. It grew. Stay near the pylons.");
            Enter("enter_w010", "W010_TidalArray", "The tide here does not follow the moon. It follows something else. I am still counting what.");
            Enter("enter_w011", "W011_TheHum", "Listen. That sound is in the rock, not the machines. It was here first.");
            Enter("enter_w012", "W012_MarasLastJump", "Mara filed a flight plan straight out of the system. I want to watch. I need to watch.");

            // ── Key flag reactions inside Ch.1–2 (the fragment + the capstone) ──────────────────────
            Flag("react_fragment_t1", ZiptideFlags.FRAGMENT_T1_FOUND,
                 "That recording is addressed to the watchers. It is mostly static. Keep it anyway.");
            Flag("react_containment", ZiptideFlags.C2_CONTAINMENT_REVEALED,
                 "Her ship did not fail. The sky refused it. Cal — it's a cage.");
            Flag("react_signal1", ZiptideFlags.SIGNAL_THRESHOLD_1,
                 "Did you feel that? Every gate on the network just... inhaled.");
            Flag("react_signal2", ZiptideFlags.SIGNAL_THRESHOLD_2,
                 "The Signal again. Stronger. It is not random — it answers you.");

            return L;
        }
    }
}
#endif
