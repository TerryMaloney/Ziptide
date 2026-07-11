# AUDIODIRECTOR TRANSITION / RELEASE CLEANUP LOG

**Owner:** GPT-5.6 Thinking, runtime-health workstream  
**Authorized by:** `CURRENT_EXECUTION_CHECKLIST.md` §7 item 7, 2026-07-11  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — narrow cleanup in progress

## Recheck result

The queue row is **not stale**. Existing ownership is mostly correct:

- `AudioDirector` is a guarded `DontDestroyOnLoad` singleton;
- it subscribes once to `SceneManager.sceneLoaded` and unsubscribes in `OnDestroy`;
- `RuntimeHealthMonitor` runs `Resources.UnloadUnusedAssets()` after world loads and counts `AudioClip` objects;
- `ResourceDisciplineTests` already ratchets runtime-created resources.

However, the two persistent `AudioSource`s keep old `clip` references after `Stop()`, so the travel janitor cannot unload those clips. Rapid profile changes can also leave overlapping crossfade/fade coroutines controlling the same two sources. A silent world fades only the current source and may retain/continue the other source from an interrupted crossfade.

## Exact protected scope

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs`
- `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs` + `.meta`
- this log and checklist/excellence closure after green

## Approved behavior-preserving fix

1. Keep one tracked transition coroutine; cancel it before reusing either source.
2. Stop and clear the inactive source before assigning the next clip.
3. After crossfade, stop and set the old source’s `clip = null`.
4. On silent/disabled profiles, cancel transition, immediately clear the inactive source, fade the active source, then clear its clip.
5. On destruction, cancel transition and clear both sources before releasing the singleton.
6. Preserve the public singleton, scene-loaded ownership, profile lookup, looping, target volume, crossfade duration and all scene/travel architecture.

## Collision / stop rules

- Do not touch audio assets, profiles, world packs, scene YAML, adaptive-audio design, ambience generation, VO, travel or RuntimeHealthMonitor.
- Do not create a second director, janitor, mixer or resource registry.
- One issue-sized runtime edit plus focused lifecycle tests.
- Three CI reds triggers the circuit breaker.
