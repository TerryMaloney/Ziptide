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

            // THE CATCH (docs/design/THE_CATCH.md §6). The rings are an orbital cargo arrestor — the
            // catching end of the mass driver on the Moss's horizon — and four lines are the whole
            // explanation. Said once on arrival, so the corridor stops being scenery the moment the
            // player first sees it.
            Enter("enter_the_catch", ZiptideConstants.SceneSpaceLane,
                  "Catch corridor's still lit. Nobody has thrown a pod up here in years and the lamps are still running.");
            Cue("CATCH_DEAD_RING",
                "Catch Three is dark. That has been dark a while.");
            Cue("CATCH_OVERRUN",
                "Everything that missed the catch ends up out here. Us too, if you get careless.");
            Cue("CATCH_THE_FIND",
                "That is not cargo.");

            // The peak of the first hour is a two-handed gesture nobody explains. Owning both
            // halves and never discovering they join would end the hour with the best beat still
            // in a holster, so ArtifactJoinRuntime waits — and only then says this.
            Cue("ARTIFACT_JOIN_HINT",
                "Both of them out. One in each hand — I want to see what they do when they can reach.");
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
            Flag("react_ship_refit", "SHIP_REFIT",
                 "You changed the ship. I noticed before you finished. I notice everything about the ship.");
            Flag("react_conquest", "CONQUEST_ATTACKED",
                 "The Wardens will notice this.");
            Flag("react_conquest_mission", "CONQUEST_MISSION_FLOWN",
                 "You could have let the dice decide. You went yourself instead. I'm noting a pattern.");

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
            // THE FIRST HOUR — the 15 teaching lines (docs/first_hour/first_hour_beats.json).
            //
            // These use the Cue trigger: they NEVER fire on a world load or a flag. The first-hour
            // director asks for one BY ID only after the beat's hesitation interval has elapsed, so a
            // player who just does the thing hears nothing at all. That is the design law — the game
            // teaches by waiting, not by narrating. Register stays RILL's: dry, brief, never a manual
            // page, never the word "button" where a physical noun will do.
            // ═══════════════════════════════════════════════════════════════════════════════════════
            void Cue(string id, string text) =>
                L.Add(new RillLine { id = id, trigger = RillTrigger.Cue, key = id, text = text });

            Cue("TUT_LOOK_RILL",        "Over here. Small, floating, opinionated — that is me.");
            Cue("TUT_COMFORT_CONSOLE",  "The console by the bunk sets how you move. Pick what your stomach likes; you can change it whenever.");
            Cue("TUT_MOVE_QUARTERS",    "Walk it off. The compartment is small and nothing in it bites.");
            Cue("TUT_GRAB_BUNK_OBJECT", "Your papers are on the bunk. Reach out and take them — properly, with your hand.");
            Cue("TUT_HOLSTER_ITEM",     "Put it on your hip. Anything holstered comes with us; anything loose stays where the world left it.");
            Cue("TUT_INTERACT_HELM",    "The helm is lit. Tell it where we are going.");

            // THE ARMOURY (⚖ Terry: the ship is where weapons live, and the ramp does not open until
            // one is on your belt). Two lines, not one, because the two failures are different and a
            // player stuck on the second one has already done what the first line asked. RILL says
            // what is wrong, never which button — the rack is a physical noun and so is your hip.
            // THE SPINE (docs/storyboard/THEMATIC_SPINE.md §3.1). Both are ordinary technician talk on
            // first hearing and knives on the second, once the player knows Cal built the Shell and
            // then went inside it. Neither line is a hint; both are literally true and useful in the
            // moment, which is the whole technique — a line that only makes sense later is a wink.
            Cue("SPINE_RELAY_DONE",     "Good. That's one more piece of it working.");
            Cue("SPINE_HALVES_JOINED",  "Two pieces. Somebody went to trouble to make sure it took two.");

            Cue("ARM_YOURSELF",         "Rack by the hatch. Take something before the ramp opens — out there is not a place you go empty-handed.");
            Cue("BELT_IT",              "In your fist does you no good on a ladder. Put it on your hip, then the ramp is yours.");
            Cue("TUT_PUNCH_IT",         "Coupler is green. When you are ready — punch it.");
            Cue("TUT_ACCEPT_FIRST_JOB", "The kiosk has our contract. Take it and we are working.");
            Cue("TUT_SCAN_FAULT",       "Scan it. Your wrist will show you what is wrong faster than I can describe it.");
            Cue("TUT_REPAIR_ACCESS",    "Panel first. Nothing inside is live until you open it.");
            Cue("TUT_SHOOT_PRACTICE",   "That target is there to be discharged. Better here than somewhere it matters.");
            Cue("TUT_OBSERVE_CREATURE", "Do not close. Watch it — everything out here tells you what it is about to do, if you let it.");
            Cue("TUT_COUNTER_CREATURE", "You saw its tell. Answer it. We disable; we do not kill.");
            Cue("TUT_ZIPLINE",          "Take the line. It is quicker than the long way round, and the long way round is flooded.");
            Cue("TUT_RETURN_HOME",      "That is the contract closed. The ship is where we left it.");

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // THE BOUNDS LADDER — what RILL says when you fly somewhere you should not.
            //
            // ⚖ Terry, 2026-07-29: "there should be a variety of responses from Rill so it doesn't
            // get too repetitive and stale." So: FOUR lines per pool, and FlightBoundsVoiceCore
            // WALKS each pool (a coprime stride) instead of rolling dice — every line is spent
            // before any repeats. She also only speaks on a RISE, so holding station near a wreck
            // is silent; getting worse is what earns a line.
            //
            // ADV = "you should know"; the ship still does exactly what you told it.
            // COR = "I am taking some of it back"; the correction has already started as she speaks.
            // Ids come from FlightBoundsVoiceCore.LineId, never from a literal — the pools and the
            // picker cannot drift apart.
            //
            // Register: RILL does not scold and does not panic. She is a forty-thousand-year-old
            // instrument reporting a fact she finds mildly disappointing.
            // ═══════════════════════════════════════════════════════════════════════════════════════
            void Bounds(FlightBoundKind kind, FlightBoundLevel level, params string[] texts)
            {
                for (int i = 0; i < texts.Length; i++)
                    Cue(FlightBoundsVoiceCore.LineId(kind, level, i), texts[i]);
            }

            // CORRIDOR — off the swept lane. Advisory FOREVER by design: wandering is allowed, and
            // she says so. There is no CORRIDOR_COR pool because nothing ever pushes you back.
            Bounds(FlightBoundKind.Corridor, FlightBoundLevel.Advisory,
                "You are outside the swept lane. Nothing wrong with that. Nothing swept out here either.",
                "Off the corridor. The rings are behind your shoulder when you want them again.",
                "Open water. The tenders do not clear this side, so whatever you hit, you found yourself.",
                "Wander if you like. I will keep the line lit for when you are done.");

            // STRUCTURE — a hull. This is the "too close to a building" case, in orbit's dialect.
            Bounds(FlightBoundKind.Structure, FlightBoundLevel.Advisory,
                "Close on the hull. It will not move for you — it stopped being able to some time ago.",
                "That is somebody's cargo. Was. Give it a metre.",
                "You are inside its tumble. Wait for the far side of the roll before you commit.",
                "Traffic. Dead traffic, but it still has corners.");
            Bounds(FlightBoundKind.Structure, FlightBoundLevel.Correcting,
                "Too close. Taking a metre back — you can have it again in a moment.",
                "I am walking us off that. Do not fight me for two seconds.",
                "That was going to be paint. Correcting.",
                "Easing us out. Whatever is on the other side of it will still be there.");

            // GATE — inside a catch ring's slab but off its axis. The bore is 12 metres; the truss
            // is not a suggestion.
            Bounds(FlightBoundKind.Gate, FlightBoundLevel.Advisory,
                "You are off centre in the bore. Twelve metres is generous until it is not.",
                "Mind the rim. That coil housing is live even when the lamps are not.",
                "Left of the throat. The ring does not care, but the truss will.",
                "Centre it. A gate you clip is a gate that gets logged, and I get the letter.");
            Bounds(FlightBoundKind.Gate, FlightBoundLevel.Correcting,
                "Centring us. You were going to wear the rim.",
                "Nudging to the throat — hold the stick where it is.",
                "That line clips. Fixed. Fly the middle of the hole.",
                "Taking the bore. This one I am not asking about.");

            // GROUND — no floor exists in orbit, so these wait for the atmospheric legs. Authored
            // now so the pool is never empty the first time a lane turns the Ground class on.
            Bounds(FlightBoundKind.Ground, FlightBoundLevel.Advisory,
                "You are low. The ground out here is mostly rust and it is not soft rust.",
                "Losing height. I would like some of it back before the roofs start.",
                "That is close to the deck. Anything down there that matters is already on my list.",
                "Low. Not dangerous yet. Say the word and it will be.");
            Bounds(FlightBoundKind.Ground, FlightBoundLevel.Correcting,
                "Lifting us. Whatever you were looking at, look at it from higher.",
                "Too low. Buying altitude with the throttle — you will feel it.",
                "I am pulling us up. This is the part where you thank me later.",
                "Deck alarm. Climbing. We can discuss it at height.");

            // DEEP — the outer sphere. Past it there is genuinely nothing, so this is the one tier
            // that ends in a hard hold.
            Bounds(FlightBoundKind.Deep, FlightBoundLevel.Advisory,
                "You are a long way out. There is nothing further — I mean that literally.",
                "The charts stop about here. Not out of caution. Out of content.",
                "Deep. Turn whenever you like; I will not say it twice for a while.",
                "Far side of everything. The Moss is that way, behind you, still throwing.");
            Bounds(FlightBoundKind.Deep, FlightBoundLevel.Correcting,
                "That is the edge. Bringing us round — there is nothing out there to reach.",
                "Turning us back. I would rather do it gently than have you find the wall.",
                "Far enough. Coming about.",
                "Holding here. Out is not a direction any more.");

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // THE ARTIFACT THREAD — "The Key That Knew You" (FIRST_HOUR_DIRECTORS_CUT §2).
            //
            // Direction, not final VO. Two contracts, two halves, one signer; the question gets
            // planted and left unanswered, because it is the engine for every act after this one.
            // Nobody says "it knew YOU" — RILL only ever says it rewrote its fitting to the SHIP.
            // The Transmission layer stays sealed, so a player who replays this hour after finishing
            // the game should get chills. That is the test of the line.
            // ═══════════════════════════════════════════════════════════════════════════════════════
            Flag("artifact_find", ZiptideFlags.ARTIFACT_HALF_A,
                 "Scrap... scrap... hull plate... wait. That one's not scrap. That's not supposed to be — anywhere.");
            CalFlag("cal_artifact_find", ZiptideFlags.ARTIFACT_HALF_A,
                    "So what are you supposed to be?");

            Flag("artifact_second", ZiptideFlags.ARTIFACT_HALF_B,
                 "The Dockmaster called that a paperweight. It has a fracture face. Cal — it has the SAME fracture face.");
            CalFlag("cal_artifact_second", ZiptideFlags.ARTIFACT_HALF_B,
                    "Two contracts. One signer. I'm choosing not to think about that yet.");

            Flag("artifact_joined", ZiptideFlags.ARTIFACT_JOINED,
                 "That's a heading. It goes... back the way we came.");
            CalFlag("cal_artifact_joined", ZiptideFlags.ARTIFACT_JOINED,
                    "That's OUR berth.");

            Flag("artifact_key_seated", ZiptideFlags.KEY_SEATED,
                 "It's not a beacon. It's a key. And it already knows the lock — it rewrote its own fitting. To OURS.");

            Flag("artifact_first_ziptide", ZiptideFlags.FIRST_ZIPTIDE_RIDDEN,
                 "I have no chart for where we just went. I have a depth reading. That is all I have.");
            CalFlag("cal_first_ziptide", ZiptideFlags.FIRST_ZIPTIDE_RIDDEN,
                    "Then we're the first ones to write one. Come on.");

            // The signer. Planted in hour one, explained in none of it.
            Flag("vex_noticed", "VEX_NOTICED",
                 "Two contracts. Two halves. One signer. Cal — who is Vex Bootstrapper?");
            CalFlag("cal_vex_noticed", "VEX_NOTICED",
                    "Somebody with terrible taste in paperweights.");

            // ── First-hour flag reactions: the tutorial's own beats had NO voice at all ──────────────
            // W000 grants TUTORIAL_COMPLETE / FIRST_TRAVEL / C1_W001_RILL_BOOT and W001 grants its
            // arrival and completion flags; none of them said anything. The whole point of W000 is
            // these moments.
            Flag("react_tutorial_complete", ZiptideFlags.TUTORIAL_COMPLETE,
                 "Coupler holds. Ship answers. Whatever we were before this morning, we are a working crew now.");
            Flag("react_first_travel", ZiptideFlags.FIRST_TRAVEL,
                 "First crossing logged. However that felt — it does not stop feeling like that.");
            Flag("react_w001_arrived", ZiptideFlags.C1_W001_ARRIVED,
                 "Toxic Venice. Do not drink anything, do not fall in anything, and mind the tower — it leans on purpose.");
            Flag("react_w001_job", ZiptideFlags.C1_W001_JOB_COMPLETE,
                 "Relay is up. Half this district gets light back tonight because you opened a panel.");
            Flag("react_first_holster", ZiptideFlags.FIRST_HOLSTER,
                 "Good. That comes with us now.");
            Flag("react_first_drone", ZiptideFlags.FIRST_DRONE_DOWN,
                 "Down, not destroyed. It will be somebody's spare parts and nobody's grave.");
            Flag("react_first_job", ZiptideFlags.FIRST_JOB_COMPLETE,
                 "Paid. I have logged it. I have also logged that you did it in the correct order, which I did not expect.");

            // The tutorial's final beat — the W000→W001 launch — had no line of its own and fell
            // through to the generic wildcard pool.
            Gate("gate_toxiccity", ZiptideConstants.SceneToxicCity,
                 "First one. Eyes open — the tide does not ask twice.", once: true);

            CalGate("cal_gate_toxiccity", ZiptideConstants.SceneToxicCity,
                    "Eyes open. Right. Sure. Absolutely.", once: true);
            CalFlag("cal_react_tutorial_complete", ZiptideFlags.TUTORIAL_COMPLETE,
                    "A working crew. One of us has a ship and the other one has opinions. Let's see how far that gets us.");
            CalEnter("cal_w001_arrival", ZiptideConstants.SceneToxicCity,
                     "It leans ON PURPOSE. Great. Love that. Really settling in already.");

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
            CalFlag("cal_react_refit", "SHIP_REFIT",
                    "New wings, same us. Try to keep up.");
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
