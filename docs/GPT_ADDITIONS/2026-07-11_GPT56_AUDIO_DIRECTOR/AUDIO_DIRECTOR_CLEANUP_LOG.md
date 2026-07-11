# AUDIODIRECTOR TRANSITION / RELEASE CLEANUP LOG

**Owner:** GPT-5.6 Thinking, runtime-health workstream  
**Authorized by:** `CURRENT_EXECUTION_CHECKLIST.md` §7 item 7, 2026-07-11  
**Branch:** `terry-local-wip`  
**Status:** ✅ UNITY CI GREEN — FILE CLAIM RELEASED

## Recheck result

The queue row was **not stale**. Existing ownership was mostly correct:

- `AudioDirector` was already a guarded `DontDestroyOnLoad` singleton;
- it subscribed once to `SceneManager.sceneLoaded` and unsubscribed in `OnDestroy`;
- `RuntimeHealthMonitor` already ran `Resources.UnloadUnusedAssets()` after world loads and counted `AudioClip` objects;
- `ResourceDisciplineTests` already ratcheted runtime-created resources.

The remaining defect was narrower: both persistent `AudioSource`s kept stopped clips referenced, preventing the travel janitor from unloading those clips. Interrupted profile changes could also leave overlapping fade coroutines controlling the same two sources, and a silent world could leave the inactive source retained.

## Delivered fix

`AudioDirector` now:

1. tracks exactly one transition coroutine;
2. cancels the active transition before either source is reused;
3. stops and clears the inactive source before assigning a new clip;
4. clears the retired source’s clip after a completed crossfade;
5. immediately clears the inactive source when entering silence;
6. clears the active source after its fade to silence;
7. cancels transitions and clears both sources during destruction;
8. preserves the singleton, scene-load subscription, world-pack profile lookup, loop flag, volume and configured crossfade duration.

No audio assets, profiles, world packs, scene YAML, adaptive-audio design, ambience generation, VO, travel system or runtime janitor changed.

## Verification

Added `AudioDirectorLifecycleTests` covering:

- `StopAndClear` releases the `AudioClip` reference and resets volume;
- null safety;
- one subscription / one unsubscription;
- one tracked transition owner;
- cancel-before-reuse ordering;
- retired-source release after crossfade;
- both source releases on silence and destruction.

## Proof

- implementation + tests commit: `e8d18d67732032d79a2806f1e517d19d1aa9330d`
- Unity CI run: `29170675599`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: skipped as expected for ordinary branch CI
- overall: `GREEN`
- circuit-breaker reds: `0/3`

The live branch later advanced through unrelated water/first-hour documentation commits. `e8d18d6` remains the merge-base ancestor of the current branch, so the cleanup is still present and was not overwritten.

## Handoff — Did / Next / Heads-up / Commits

**Did:** removed the final persistent clip-retention seam and serialized all music transitions through one coroutine owner. The existing post-travel janitor can now unload retired world music instead of being defeated by stopped source references.

**Next:** §7 item 8 is only eligible after checking whether the current Unity CI environment can run a small PlayMode scaffold reliably. Do not begin by building a large travel integration test; first prove a minimal PlayMode test can enter/exit cleanly without scene YAML, Quest hardware or timing flakiness.

**Heads-up:** code-green is not a long-session memory verdict. During Terry’s consolidated Quest travel soak, `ZIPTIDE: HEALTH_SWEEP ... clips=` and total memory should remain flat across repeated worlds. Any continued clip growth should be diagnosed from the census before adding another audio owner.

**Commits:** claim `dbda48f`; implementation/tests `e8d18d6`; CI run `29170675599`; this closure stamp.

## Closure

The `AudioDirector` unload/disposal hardening row is closed at code/CI maturity. The protected-file claim is released.
