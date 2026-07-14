# ZIPTIDE Recovery Exposure Manifest

**Machine source:** [`recovery_exposure_manifest.json`](recovery_exposure_manifest.json)  
**Golden destination:** `ToxicCity` / W001

## Why W001 is the target

W002 may be easier to isolate, but it is not the intended first real destination. The recovery program exists to make the real first-hour path coherent:

`_Boot → W000 → W001/ToxicCity`

Choosing W002 would create a demonstration path that bypasses the transition and integration failures Terry actually encountered. W001 is therefore selected with its known debt exposed, not hidden.

## GoldenSlice scenes

Only these scenes are allowed in the recovery candidate:

1. `_Boot`
2. `W000_DriftIn`
3. `ToxicCity`

The current project enables 24 scenes. R1 must generate the reduced recovery build list through code/build tooling; no scene YAML is edited and no source scene is deleted.

## GoldenSlice automatic owners

Allowed support:

- SaveSystem
- persistent player rig
- TravelCoordinator
- AudioDirector
- AmbienceDirector
- ComfortVignette
- RuntimeHealthMonitor
- FirstHourObservationAdapter

Conditional:

- EcologyDirector only after one named golden creature has authoring/contact/grounding PlayMode proof.

Forbidden:

- DebugHUD automatic overlay
- name-based XR camera enforcer
- RuntimeInputEnabler
- RuntimeMaterialFixer
- unconditional VR boot diagnostics
- Conquest mission injector
- DevWarpBoard unless the Diagnostic profile explicitly enables it
- PvP progression bootstrap
- Quarters camera injector
- Photon NetBootstrap

## Player-facing scope

Allowed:

- Home Hub;
- required W000 first-hour surface;
- one bounded W001 arrival/job area;
- one tool and holster path;
- one creature and non-lethal resolution;
- one job/reward/spend path;
- one ship boarding/use moment;
- minimal contextual status required to finish that loop.

Hidden:

- always-on credits HUD;
- broad world-door wall;
- Quarters;
- Tidefront table;
- PvP/Photon surfaces;
- zipline;
- free flight;
- hammer/melee pair;
- non-golden scenes;
- unverified practical halo/pool quads;
- legacy striped planet presentation;
- visible runtime material fallback.

This is exposure control, not deletion. FullDevelopment preserves source access while migration proceeds; it is not an acceptance build.
