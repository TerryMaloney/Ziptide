# TEST DAY 1 RESPONSE — the Feel & Clarity program (Terry's ❌ list → the new working queue)

> **STATUS 2026-07-06 — WAVE 1 + 2.1 SHIPPED, all CI-green:** 2.1 movement `2fd06ab` · 1.7 HUD
> `62f2d03` · 1.2 grip `d20ec75` · 1.3 tracer + 1.10 subtitle `3b29fd6` · 1.5 z-fight `488076b` ·
> 1.4 SPAWN_AT diagnostic `87516c1` · 1.9 beacon `1174ad9` · 1.6 incoming-fire lines `983ed2c` ·
> 1.8 board labels `1ba42d1`. **Open:** 1.1 + 1.4 close after Terry's next logcat (`ITEM_SPAWN` /
> `SPAWN_AT` lines name the culprits). **Next for any operator: Wave 2.2 (laser sights), then
> Wave 3 top-down (3.1 PUNCH-IT flight is the crown jewel), then 4.1 gardens audit.**

## Context

Terry completed the first full 11-world headset pass (2026-07-05, build `874c905` locally rebuilt).
**Wins confirmed on-device:** full environments render and read, W005 swarm movement is a "major
leap," hammer feel is great in PvP, all worlds reachable, gating works. **His ❌/feel list below
becomes project priority #2 by standing rules** (after CI health). Fable 5 access ends within
~a day, so every item is written as a self-contained envelope an Opus/Sonnet operator can execute
from the boards. Buckets are ordered by (player impact × cheapness). Recon anchors (verified in
code): sprint EXISTS in `Gameplay/Runtime/Locomotion/DashLocomotion.cs` (L3 stick-click, 1.9× of
1.75 m/s base = ~3.3 m/s — too slow, and Terry didn't discover it); gardens ARE seeded into packs
(`WorldStubGenerator.cs:281` adds `GardenSpawnDefinition`) so the failure is runtime spawn or
discoverability; PvP HUD canvas lives in `Multiplayer/Runtime/Net/PvpNetHub.cs`; arena weapons come
from `Editor/Patching/ArenaWeaponAuthor.cs` + `Gameplay/Runtime/Weapons/*Runtime.cs`.

## WAVE 1 — QUICK FIXES (each ≈1 commit, CI-verifiable, do first in any order)

| # | Terry's words | Fix envelope | Seam |
|---|---|---|---|
| 1.1 | "laser guns are suuuper tiny" | Author real sizes on PrismBeam/SonicThumper/StaticNet item defs (compare `DefaultTaserDartGun`); add an audit warning `ITEM_SCALE_SUSPECT` for grabbables < ~12 cm | `ArenaWeaponAuthor.cs` / item library + `ItemFactory` |
| 1.2 | "guns aim at whatever angle you picked them up at" | Every gun gets a Grip socket pose snapped on grab (the taser's +45° convention, applied via the shipped `ForgeVisualApplier`/attach-transform pattern) — one helper, applied in `ItemFactory.Create` for ALL guns | `ItemFactory.cs` + item defs |
| 1.3 | "grey weapon shows nothing shooting out" | Add tracer (LineRenderer flash) + muzzle emissive pulse to the hitscan fire path so every shot is visible | the weapon fire runtime (`*Runtime.cs` fire method) |
| 1.4 | "stuck under the level in matchboard" | Arena spawn Y = sample ground height + the torso-height overlap check the worlds already use (port the WorldStubGenerator spawn fix to the arena patcher) | arena patcher / `__SPAWN_PLAYER` markers |
| 1.5 | "glass city ground = white noise at start" | Z-fighting at W003 spawn: raise POI pad/platform ~4 cm above terrain or shrink coplanar overlap; audit rule `COPLANAR_AT_SPAWN` (warning) | `WorldLayoutLibrary` W003 + `WorldPoiBuilder`/`WorldStubGenerator` |
| 1.6 | "getting hit by something I can't tell what" (W005) | Give every damage source a visible tell: hazard volumes get a particle/tinted zone + `HAZARD_HIT src=` log line; HUD damage-direction flash | hazard system + player damage handler |
| 1.7 | "PvP HUD is massive, in my face" | Scale world-space HUD to ~0.6 m wide at ~1.6 m, anchor low-forward (not camera-locked center); one `HudPlacement` helper reused by all HUDs | `PvpNetHub.cs` canvas creation |
| 1.8 | "matchboard menu text runs on top of other text" | Fix stacked-label layout (dynamic row height / VerticalLayoutGroup) in the matchboard UI | matchboard/Tidefront UI code |
| 1.9 | "idk where the kiosk is" | `ObjectiveBeacon`: floating marker + off-screen edge arrow pointing at the current objective target (kiosk first); reusable for any "go here" | new small runtime + kiosk registration |
| 1.10 | "subtitle placement" | Anchor RILL subtitle lower (−25° pitch, 1.8 m), fade after read | `SubtitleText.cs` / `RillCompanion.cs` |

## WAVE 2 — THE FEEL LEAP (movement + aiming; Terry's "Fortnite-level" directive)

- **2.1 Movement v2** (`LocomotionProfile` + `DashLocomotion`): base 3.0 m/s, sprint 5.5–6.0 m/s
  (hold-to-sprint on L3 AND auto-sprint-forward option), accel snappier, add jump on A (small,
  grounded-checked — the fall-safety net already exists), strafe full-speed. All values in the
  PROFILE asset (data, tunable per Terry feedback without code). Add a boot-time hint line +
  runbook row documenting controls. LOCO_STATE logs sprint state.
- **2.2 Aim v2 — every gun**: (a) grip-pose fix from 1.2 is the foundation; (b) add a subtle laser
  sight + dot reticle on all guns (cheap LineRenderer, theme-colored) so aiming reads instantly;
  (c) OPTIONAL follow-up envelope: "aim-assist cone" ala Quest shooters (small angular snap toward
  targets under the reticle) — data-driven per weapon, off by default in PvP. ("Aim in/out like
  Xbox" is interpreted as: consistent grip + visible aim line + assist — the VR equivalents of
  stick aiming. If Terry meant scope zoom, that's a later envelope: hold-trigger-half = 1.5× zoom
  vignette.)

## WAVE 3 — SYSTEMS (bigger envelopes, still single-lane)

- **3.1 Flight v1 "PUNCH IT"** (story lane, P4b): wire the EXISTING pure `FlightModel` (tests
  already green) to the ship: fuel-cell socket on the console arms it (socket runtime exists —
  Terry already put the cell in!), then a big PUNCH IT button launches a rails take-off (world
  transition with velocity feel per `SPACEFLIGHT_PHYSICS.md`; never parent the rig to the hull).
  Deliverable: cell in → console lights → PUNCH IT → leave W000 with thrust feel.
- **3.2 Destruction v2** (MP lane): replace uniform blocks with pre-fractured wedge/plate chunks
  (Forge `Wedge` op — no new assets needed) at ~60% current size; chunks fly with impulse +
  despawn; budget-capped per arena. "CoD Siege breakaway, better mechanically."
- **3.3 HUD & "How to play" system** (shared UI lane): one `GameHud` style (small, low-anchored,
  from 1.7) + a diegetic **rules board** at each mode's entrance (matchboard, arenas, shell game):
  3-panel "how this mode works" using the DevMenu canvas patterns. Kill all text-overlap layouts.
- **3.4 Building read** (art lane): building "lines that don't make sense" = module seams —
  E5.1 building modules with real textures (already boarded) fixes this properly; quick win first:
  align module UV/panel spacing so seams land on floor lines (grammar spacing tweak).

## WAVE 4 — CONTENT SURFACING (things built but invisible)

- **4.1 Gardens/build sockets audit**: packs contain gardens (`WorldStubGenerator:281`) but Terry
  saw none → add `WORLD_CONTENT` audit: every pack's gardens/sockets MUST resolve to a spawned
  runtime object in the built scene (blocker if silently dropped); then a beacon (1.9) on the first
  garden plot in W005 + runbook row.
- **4.2 Detail density pass** (art lane): "big open expanses need more detail" → scatter density
  bump + E5.3 flora/props envelopes (already boarded) — after E1.4 so new props ship textured.
- **4.3 "Blocky is ok for now"** — confirmed roadmap: FORGE II P2 (geometry ops) + E1.4 (textures
  on device) are the fidelity fixes; no new work needed, just sequencing (art board unchanged).

## Board updates (same commit as plan execution start)
- `PRIORITIES.md` rev 7: Wave 1 = #2 (after CI), Wave 2 = #3, flight 3.1 keeps P4b slot, rest slot in.
- `docs/SPRINT.md` (story: 3.1, 1.5, 1.6, 1.9), `SPRINT_MULTIPLAYER.md` (1.4, 1.7, 1.8, 3.2, 3.3),
  `SPRINT_ART.md` (3.4, 4.2 after E1.4), HANDOFF `dddd` with Terry's full verbatim feedback.
- `TERRY_RUNBOOK.md`: new §2o re-test checklist (one row per wave item, with its logcat tag).

## Verification
- Every envelope: CI green (tests for pure parts: locomotion profile values, spawn-height, fracture
  math, beacon targeting) + the audit gates extended per item (1.1, 1.5, 4.1 add rules).
- Device truth: Terry's next test day runs runbook §2o; each item has a named logcat tag to
  confirm (`LOCO_STATE sprint=`, `HAZARD_HIT src=`, `BEACON_TARGET`, `ARENA_SPAWN_OK`...).
- Art items verified through the photo loop (booth) before device.
