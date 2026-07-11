# FH-S07 IMPLEMENTATION LOG — HOME / W000 FIRST-HOUR SURFACES

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“crush that out”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CODE + UNITY CI GREEN — BAKE / DEVICE EVIDENCE PENDING; FILE CLAIM RELEASED  
**Envelope:** `docs/first_hour/envelopes/FH-S07-HOME-W000-SURFACES.json`

## Dependencies

- ✅ `FH-X01-CONTRACT-ASSET`
- ✅ `FH-X02-PROGRESSION-CORE`
- ✅ `FH-S01-OBSERVATION`
- ✅ `FH-S02-HOLSTER`

## Delivered behavior

### Cold boot and profile choice

- `_Boot` now presents a diegetic **New Game / Continue / Settings** surface before first-world travel.
- Continue appears only when `SaveFileStore` can recover a valid main or backup profile.
- `SaveSystem.HasExistingProfile` is a read-only validity query.
- `SaveSystem.StartNewProfile()` is the explicit New Game command and reuses the existing serializer and atomic writer.
- Settings never consumes the one-shot travel choice.
- New and Continue travel exactly once through the existing `TravelCoordinator.TravelTo(..., skipGate: true)` path.
- No second save singleton, serializer, boot state, scene loader, or travel owner was added.

### Locked comfort presets

- Added device-level `ComfortSettings` using `PlayerPrefs`, not `PlayerProfile`; a New Game does not erase body/device comfort.
- Standard is the install default.
- Cozy / Standard / Bold resolve to the locked dial table in `docs/design/COMFORT_AND_ACCESSIBILITY.md`.
- The diegetic comfort console delegates to existing owners:
  - `LocomotionDirector` for turn mode and slide/dash configuration;
  - `ComfortVignette` for aperture strength;
  - `ZiplineRuntime` for the comfort speed ceiling.
- No rig transform is moved and no second vignette, locomotion provider, slide, dash, or zipline system exists.
- Flight-related locked dials are stored in the resolved table for the existing flight owner to consume when that protected seam is assigned; FH-S07 did not silently broaden its protected-file scope.

### W000 surfaces

- Added one named grabbable bunk keepsake: `bunk_keepsake`.
- Added a first-destination helm exposing only `W001_ToxicCity`.
- The helm changes only `ShipCastOffRuntime`'s existing destination value; `TryLaunch()`, `LaunchSequence()`, repair arming, PUNCH IT, and the sole `TravelCoordinator` call remain unchanged.
- Added idempotent `FirstHourSurfaceAuthor` for the actual generated W000 scene:
  - `Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity`
  - `__FIRST_HOUR_COMFORT_CONSOLE`
  - `__FIRST_HOUR_BUNK_OBJECT`
  - `__FIRST_HOUR_FIRST_HELM`
- No `.unity` or `.prefab` YAML was hand-edited.
- Primitive surfaces are deliberate safe fallbacks; Picasso's art/Forge/water files were untouched.

## Protected-owner proof

Only the announced additive seams changed:

- `BootLoader.Start()` delegates cold-boot choice and retains the one travel path.
- `SaveSystem` remains the sole live profile and disk owner.
- `LocomotionDirector.ApplyProfile()` remains the ordinary locomotion owner; `ApplyComfortSettings()` is additive.
- `ShipCastOffRuntime.TryLaunch()` and `LaunchSequence()` remain the sole launch/travel sequence.

No replacement owner or duplicated state was introduced.

## Commit and CI proof

- claim: `2640fbcdff5ee3ded681bc620c2773b01a4ae9f3`
- commit 1, boot/profile: `4ba6e650b00f01ce6e69b7b3f3b7ca97762d62e6`
  - CI green run `29164626077`
- commit 2, locked comfort table/console: `6f088d8663eb7184306981eb30ae8b1e8eb51507`
- test-only guard correction: `3f7754b40a65ff6778b4652416a4aaaabea58450`
  - CI green run `29164968854`, 906/906 tests
- commit 3, W000 bunk/helm/author: `5d3c3c164671334f281593e74658b7c1b5523260`
- test-only author-harness correction: `e51dce54b984184139812c89ac344c86f2b6f39a`
  - final CI green run `29165306485`
  - Unity EditMode: `success`
  - project-contract reports: `success`
  - Android: `skipped` as expected for an ordinary branch push
  - final total: 910/910 tests passed
- circuit breaker: two reds out of three, both test-harness/source-guard defects; runtime behavior passed. Final green landed before the breaker.

## Required one-time bake

Run either:

- Unity menu: `Ziptide → First Hour → Author W000 Surfaces`
- batchmode execute method: `Ziptide.Editor.FirstHourSurfaceAuthor.AuthorW000`

Then commit the generated change to:

- `Ziptide/Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity`

The author is idempotent: rerunning it must retain exactly one of each stable marker.

## Required headset evidence

1. Cold boot shows New Game / Settings with no save; Continue appears only after a valid save exists.
2. Settings opens Cozy / Standard / Bold without beginning travel.
3. Standard is default; Cozy gives stronger vignette, 45° snap, neutral slide and slower ziplines; Bold gives smooth 120° turn and light vignette.
4. Starting New Game does not erase the chosen comfort preset.
5. W000 presents the comfort console as the first obvious interaction.
6. The named bunk keepsake is visible and grabbable once.
7. The helm exposes only W001/Toxic City.
8. Selecting W001 does not travel immediately; existing PUNCH IT and the repaired-coupler gate still own departure.
9. No nausea, rig jump, duplicate menu, direct scene load, or duplicated W000 marker.

## Diagnostics

- `ZIPTIDE: HOME_HUB_READY continue=<bool>`
- `ZIPTIDE: HOME_HUB_CHOICE choice=<new|continue|settings>`
- `ZIPTIDE: COMFORT_PRESET preset=<cozy|standard|bold>`
- `ZIPTIDE: FIRST_HOUR_BUNK_GRAB id=bunk_keepsake`
- `ZIPTIDE: FIRST_HELM_SELECTED dest=W001_ToxicCity`
- `ZIPTIDE: FIRST_HOUR_SURFACES_AUTHORED scene=W000_DriftIn castoff=<bool>`
- `ZIPTIDE: FIRST_HOUR_SURFACES_SAVED scene=W000_DriftIn`

## Closure

FH-S07's protected/shared file claim is released. Code and Unity CI are green. The envelope remains device-yellow until the author is run and Terry passes the bake/headset checklist. FH-S08 final orchestration remains blocked by its full dependency and device-evidence requirements; do not skip those gates.