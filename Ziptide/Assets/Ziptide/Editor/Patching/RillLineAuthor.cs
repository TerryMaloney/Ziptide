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
    /// Authors Resources/Story/RillLines.asset — RILL's AND Cal's subtitle lines (GAME_PLAN M1; Cal
    /// joined 2026-07-06, soul pass 2). CODE IS THE SOURCE OF TRUTH: the 12 canonical arc beats
    /// (MASTER_BUILD_PLAN §5.2, exact lines) + one entry line per authored world, register matched to
    /// RILL's memory state at that point (Dormant = terse/functional, Stirring = questions —
    /// STORY_BIBLE). Beats for worlds that don't exist yet (W013+) are authored NOW and simply fire
    /// when those flags start being granted (M5) — content and systems grow together. Cal's lines live
    /// in their own section at the bottom (CalEnter/CalFlag/CalGate) so the two voices stay easy to
    /// tell apart on the page; neither has a VO clip yet — see docs/VOICE_PIPELINE.md before recording.
    /// TO CHANGE A LINE: edit it here; the asset regenerates every build. Wired into BuildAndroid next
    /// to the other data authors; also runnable from the menu.
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
            // Cal's half of the conversation (STORY_BIBLE §3b, soul pass 2026-07-06). Same trigger/key
            // convention as RILL's above; speaker="CAL" is the only difference, so a Cal line placed right
            // after the RILL line it answers plays as one exchange (Collect returns list order). Cal has
            // NO VO CAST YET (see docs/VOICE_PIPELINE.md) — voClip stays null, subtitles carry every line.
            void CalEnter(string id, string scene, string text) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.WorldEnter, key = scene, text = text, speaker = "CAL" });
            void CalFlag(string id, string flag, string text) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.FlagSet, key = flag, text = text, speaker = "CAL" });
            void CalGate(string id, string destSceneOrStar, string text, bool once) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.GateDeparture, key = destSceneOrStar, text = text, once = once, speaker = "CAL" });

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
            Enter("enter_w000", "W000_DriftIn",
                  "...there you are. I have been awake for six minutes and I already have opinions. The coupler is down; your papers are by the bunk. Shall we?");
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
            // The Undercroft (1.4g — W011's cave layer; scene by ScenePatcherCavern).
            Enter("enter_w011b", "W011_Undercroft",
                  "The Hum is louder down here. It is not an echo. Echoes answer you — this is leading.");
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
            Flag("react_first_release", ZiptideFlags.FIRST_RELEASE,
                 "Dropped gear stays where it falls — and stays behind when we travel. The holster on your hip keeps it with us.");

            // ── The ship (M4) — the companion has opinions about home ───────────────────────────────
            Flag("react_ship_board", "SHIP_FIRST_BOARD",
                 "Home. Or the nearest thing either of us has to one.");
            Flag("react_quarters", "QUARTERS_FIRST_VISIT",
                 "Your quarters. The Guild manifest calls this compartment 'storage.' I disagree.");

            // ── THE ZIPTIDE gate lines — RILL rides the tide with you ───────────────────────────────
            // Wildcard pool (key "*", NOT once): one is picked at random each crossing, so the gate
            // keeps a voice without repeating itself every time. Register: dry, 40,000 years old,
            // still secretly delighted by the crossing.
            void Gate(string id, string destSceneOrStar, string text, bool once) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.GateDeparture,
                                     key = destSceneOrStar, text = text, once = once });

            Gate("gate_any_brace", "*", "Brace. The tide has us.", once: false);
            Gate("gate_any_hold", "*", "Gate is open. Hold on to what you are holding.", once: false);
            Gate("gate_any_first", "*", "Every crossing still feels like the first one. For both of us.", once: false);
            Gate("gate_any_delight", "*", "Forty thousand years, and this part still delights me.", once: false);
            Gate("gate_any_count", "*", "Crossing. Do not count the pillars — you will lose track on purpose.", once: false);

            // Destination-specific gate lines (say once, before the generic pool takes over).
            Gate("gate_w002", "W002_DryCistern", "Next stop: a hole in the ground with paperwork.", once: true);
            Gate("gate_w005", "W005_OxidizedCanopy", "Green on the other side. Try not to repair anything that is alive.", once: true);
            Gate("gate_w012", "W012_MarasLastJump", "Mara crossed from here last. Watch the sky when we land.", once: true);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // CAL'S LINES — the other half of the conversation (STORY_BIBLE §3b, soul pass 2, 2026-07-06).
            // Appended after all of RILL's above so, for any shared trigger+key, RILL's entry (earlier
            // list index) always plays first and Cal's answers it — one exchange, not two monologues.
            // Register follows the same Dormant→Integrated arc as RILL's own lines; Cal is dry and
            // competence-under-pressure per STORY_BIBLE §3 ("oh, NOW you work"), breaks into a real
            // question only a few times a chapter, and always deflects back into a joke afterward — the
            // "joke, then a real moment, then a joke" rhythm, never the reverse.
            // ═══════════════════════════════════════════════════════════════════════════════════════

            // ── Ch.0-2 world-entry banter (answers RILL's enter_w0xx lines above) ────────────────────
            CalEnter("cal_w000", "W000_DriftIn", "Six minutes and you already have opinions. That tracks.");
            CalEnter("cal_w002", "W002_DryCistern", "So the pumps died forty years ago and nobody updated the file. Sounds about right for this job.");
            CalEnter("cal_w004", "W004_BroadcastTomb", "Long before the Guild got here — or long before you did?");
            CalEnter("cal_w005", "W005_OxidizedCanopy", "If it minds, it can file a complaint. We've got a contract.");
            CalEnter("cal_w007", "W007_SableStation", "Good. I prefer people who prefer things.");
            CalEnter("cal_w008", "W008_SealedArchive", "You keep saying that like it's new information about yourself.");
            CalEnter("cal_w009", "W009_Chitinwall", "Everything out here either grew wrong or grew on purpose. I'm losing track of which is worse.");
            CalEnter("cal_w011", "W011_TheHum", "First before what, RILL?");
            CalEnter("cal_w011b", "W011_Undercroft", "Leading where? ...You know what, keep that one. I'd rather find out.");
            CalEnter("cal_w012", "W012_MarasLastJump", "Yeah. Me too.");

            // ── Ch.3-7 flag reactions (paired with RILL's canonical beats above; dormant until W013+ ship) ──
            CalFlag("cal_react_shard", ZiptideFlags.C3_W013_MEMORY_SHARD,
                    "That's not weird at all. RILL, that is not weird even a little.");
            CalFlag("cal_react_refusal", ZiptideFlags.C3_W019_RILL_REFUSED,
                    "Okay. I trust you. I'd like to still trust you in an hour, so — anything you want to tell me first?");
            CalFlag("cal_react_color", ZiptideFlags.C4_W024_COLOR_NAMED,
                    "Forty thousand years and you land on a color. I'd have picked a word. You're braver than I gave you credit for.");
            CalFlag("cal_react_standoff", ZiptideFlags.C5_W037_WARDEN_STANDOFF,
                    "RILL. Do you know why?");
            CalFlag("cal_react_pattern", ZiptideFlags.C6_W039_PATTERN_WARNING,
                    "My memories, or yours?");
            CalFlag("cal_react_named", ZiptideFlags.C6_W051_RILL_NAMED,
                    "Then tell me. I've been waiting a while to stop calling you 'the drone.'");
            CalFlag("cal_react_confess", "W053_COMPLETE",
                    "RILL — you don't have to finish that. Not tonight.");
            CalFlag("cal_react_revelation", ZiptideFlags.C8_W062_REVELATION,
                    "Out. Not in. Say that again, slower.");
            CalFlag("cal_react_branch", ZiptideFlags.C12_W063_BRANCH,
                    "Whatever I choose in there — I need you to know I heard everything you didn't say too.");

            // ── Endgame ending reactions (answer RILL's beat12_end* lines above, exact per-ending) ────
            CalFlag("cal_end_a", ZiptideFlags.C12_W063_ENDING_A,
                    "...Okay. Okay. I'll come back. I don't care what the manifest says about return trips.");
            CalFlag("cal_end_b", ZiptideFlags.C12_W063_ENDING_B,
                    "Then let's go find out together.");
            CalFlag("cal_end_c", ZiptideFlags.C12_W063_ENDING_C,
                    "I'll remember for both of us, then. Deal?");
            CalFlag("cal_end_d", ZiptideFlags.C12_W063_ENDING_D,
                    "The first of us. Yeah. I believe that. I think I always kind of did.");

            // ── THE ZIPTIDE — Cal's occasional line in the wildcard gate pool (mixed in with RILL's) ──
            CalGate("cal_gate_any_pillars", "*", "Please tell me the pillars have names.", once: false);
            CalGate("cal_gate_any_howdoyou", "*", "I still don't know how you make a tide out of light. I've stopped asking.", once: false);
            CalGate("cal_gate_w007", "W007_SableStation", "Sable's turf. Manners, please.", once: true);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // COMPANION MEMORY — RILL brings something back up, unprompted, gate crossings later
            // (docs/systems/COMPANION_MEMORY.md, added 2026-07-06). Each watches a flag already granted
            // above; `crossings` is how many gate departures after that flag is first noticed before the
            // line fires on its own. This is what makes RILL feel like she has a throughline of thought
            // that keeps running whether or not Cal's looking, not just a stimulus-response reaction.
            // ═══════════════════════════════════════════════════════════════════════════════════════
            void FollowUp(string id, string flag, string text, int crossings) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.FollowUp, key = flag, text = text, crossingsDelay = crossings });
            void CalFollowUp(string id, string flag, string text, int crossings) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.FollowUp, key = flag, text = text,
                                     crossingsDelay = crossings, speaker = "CAL" });

            FollowUp("followup_cargo", ZiptideFlags.C1_W004_RILL_ASKED_CARGO,
                "I never received an answer about the cargo. I have stopped expecting one. That is new, too.", crossings: 9);
            FollowUp("followup_cage", ZiptideFlags.C2_CONTAINMENT_REVEALED,
                "I have had time to think about the cage. I keep arriving at the same word: deliberate. I did not expect to end up there.", crossings: 7);
            CalFollowUp("cal_followup_cage", ZiptideFlags.C2_CONTAINMENT_REVEALED,
                "You've had 'a while' to think about a lot of things. I'm starting to worry you're outpacing me on all of them.", crossings: 8);
            FollowUp("followup_refused", ZiptideFlags.C3_W019_RILL_REFUSED,
                "You never asked me again why I refused. I have been waiting. I think I am relieved you didn't.", crossings: 6);

            return L;
        }
    }
}
#endif
