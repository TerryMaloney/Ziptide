# ASYNC TRAVEL IMPLEMENTATION LOG

**Owner:** GPT-5.6 Thinking, general Story/Ship integration  
**Authorized by:** Terry, 2026-07-11 (“let’s start continuing to check off stuff”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ CODE + UNITY CI GREEN — DEVICE FRAME-PACING COMPARISON PENDING; FILE CLAIM RELEASED  
**Design:** `docs/design/ASYNC_TRAVEL.md`

## Exact protected scope

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - only the destination-load block inside `TravelCoroutine`;
  - pre-flight, autosave, rig/inventory preparation, `_travelling`, post-load teleport/XRI/restore, `TRAVEL_OK`, and first-hour publication order remain owned by the same path;
  - the no-coordinator fallback remains synchronous and unchanged.
- `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs`
  - pins one fallback synchronous call and one owned async coroutine call;
  - pins activation hold, the 0.9 readiness threshold, 20-second timeout, `TRAVEL_TIMEOUT`, and post-load order.

## Shipped behavior

The single owned coroutine destination load now:

1. calls `LoadSceneAsync(sceneName, LoadSceneMode.Single)` after the existing crest-cover wait;
2. sets `allowSceneActivation = false`;
3. waits for Unity progress `>= 0.9f` using unscaled time;
4. logs `ZIPTIDE: TRAVEL_TIMEOUT` after 20 seconds if normal readiness never arrives;
5. permits activation and waits for `isDone`;
6. returns to the unchanged post-load frame, rig teleport, XRI wiring/readiness, inventory restoration, `TRAVEL_OK`, and first-hour completion path.

If Unity unexpectedly returns no async operation, travel fails explicitly with `reason=async_load_not_started`, releases `_travelling`, and does not continue into a fake successful arrival.

## Ownership preserved

Unchanged:

- `TravelCoordinator.TravelTo` remains the sole gameplay travel API;
- `_Boot` redirect and missing-build-scene pre-flight;
- fallback autosave/rig preparation/direct load;
- Ziptide departure and arrival choreography;
- profile and inventory save timing;
- XRI readiness and inventory restoration;
- one-shot first-hour travel event order;
- no new singleton, loader, fade, save state, rig state, or scene architecture.

Picasso `Visuals/**`, Forge, water, art authors/audits, and `SPRINT_ART.md` were untouched.

## Commit and CI proof

- claim: `47419204c513bdb3408962fab3e25cfb18ac40c1`
- runtime: `c85f76301ff1b9b6bd0cca04e729f5d4f2166075`
- contract tests: `c7b5d524b710b767a071264d6a1c552136bf08a0`
- CI run: `29167548682`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: skipped as expected for an ordinary branch push
- overall: `GREEN`
- circuit-breaker reds: `0/3`

## Terry device evidence

During the next consolidated headset pass:

1. travel the same representative route five times before judging anything else;
2. watch the white crest: it may remain visible while the destination prepares, but the old frozen-world hitch should be gone or reduced;
3. confirm arrival tide, spawn teleport, locomotion, hands/rays, and holstered inventory restore exactly as before;
4. inspect `HEALTH`/frame logs for dropped-frame spikes across five travels;
5. confirm no `TRAVEL_TIMEOUT` during ordinary loads;
6. if `TRAVEL_TIMEOUT` appears, confirm travel still completes instead of remaining wedged;
7. confirm cold boot still skips the world-to-world gate effect and reaches W000 normally.

## Closure

The protected travel-file claim is released. The implementation is code/CI green and remains device-yellow until Terry compares frame pacing and confirms the full travel restoration path in-headset.
