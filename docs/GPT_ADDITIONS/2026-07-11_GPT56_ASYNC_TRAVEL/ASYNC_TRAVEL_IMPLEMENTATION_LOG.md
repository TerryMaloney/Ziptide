# ASYNC TRAVEL IMPLEMENTATION LOG

**Owner:** GPT-5.6 Thinking, general Story/Ship integration  
**Authorized by:** Terry, 2026-07-11 (“let’s start continuing to check off stuff”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — protected travel-file implementation in progress  
**Design:** `docs/design/ASYNC_TRAVEL.md`

## Exact protected scope

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - only the destination-load block inside `TravelCoroutine`;
  - preserve pre-flight, autosave, rig/inventory preparation, `_travelling`, post-load teleport/XRI/restore, `TRAVEL_OK`, and first-hour publication order;
  - keep the no-coordinator fallback synchronous and unchanged.
- `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs`
  - update the existing source guard to pin one fallback synchronous call and one owned async coroutine call;
  - assert activation is held, the 0.9 readiness threshold exists, timeout is 20 seconds, and `TRAVEL_TIMEOUT` is logged.
- This implementation log and the current checklist/runbook at closure.

## Approved diff

Replace the single owned coroutine `SceneManager.LoadScene(sceneName)` with:

1. `LoadSceneAsync(sceneName, LoadSceneMode.Single)`;
2. `allowSceneActivation = false`;
3. wait until progress reaches 0.9 after the existing crest-cover wait;
4. after 20 seconds, log `ZIPTIDE: TRAVEL_TIMEOUT` and permit activation anyway;
5. set `allowSceneActivation = true` and wait for completion;
6. continue the existing post-load flow unchanged.

## Collision rules

- Do not touch Picasso `Visuals/**`, Forge, water, art authors/audits, or `SPRINT_ART.md`.
- Do not create a second travel owner, loader, fade, rig state, save path, inventory path, or timeout singleton.
- Do not change `ZiptideGateEffect` choreography.
- This task ships alone; no unrelated gameplay changes ride in the commit.
- Three CI reds triggers the circuit breaker.
