# ⏳ ASYNC TRAVEL — the design (Fable endgame #4, 2026-07-10; Opus implements against this)

**Problem:** `SceneManager.LoadScene` in TravelCoordinator is synchronous — as worlds get richer,
the main thread stalls mid-tide (a HITCH inside a VR fade: dropped frames at the worst moment).
Not a crash; a comfort failure that grows with the richness bar.

**Design (minimal, honoring every travel law):**
1. Replace the one load call with `LoadSceneAsync(sceneName, LoadSceneMode.Single)`,
   `allowSceneActivation = false`. THE ZIPTIDE crest plays as today; activation flips ONLY when
   (a) `progress >= 0.9f` AND (b) the crest has fully covered vision — the load hides inside the
   moment that already exists for it. Single mode keeps today's semantics exactly (no additive
   rework, _Boot stays DontDestroyOnLoad-persistent as it is now).
2. Timeout guard: if 0.9 never arrives in 20s, log `TRAVEL_TIMEOUT`, activate anyway (never wedge).
3. NOTHING else moves: pre-flight guard, autosave, rig prep, arrival flow, `_travelling` gate —
   the diff is ~15 lines inside `TravelCoroutine` only.
4. Verify: EditMode can't load scenes — device gate is Terry's: watch `HEALTH` dropped-frames
   across 5 travels before/after; the runbook step ships with the change.

**⛔ Report-only law:** travel file. The implementing session states this diff in HANDOFF, gets
Terry's go (he has already approved the DESIGN direction in the crash-proofing arc), ships it
alone in one commit, and watches CI + the next device pass. Nothing else rides in that commit.
