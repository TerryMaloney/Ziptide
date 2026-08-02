# HERO SHIP PRODUCTION PACKET

**Prepared by:** Fable 5 (Reasonbox), senior technical direction pass, 2026-07-17.
**Branch:** `fable/hero-ship-production-packet` (documentation only; draft PR; DO NOT MERGE before the Quest checkpoint result).
**Status:** execution-ready packet for POST-recovery implementation. Nothing here authorizes runtime work before the checkpoint verdict and recovery exit.
**Authority context:** `docs/recovery/RECOVERY_PROGRAM.md` (freeze), `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md` (authorized candidate `2b158b4` — untouched by this branch), `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md` (proof lanes referenced throughout), `docs/PROJECT_COMPLETION_ROADMAP.md` §5 (the ship goal this packet operationalizes).

---

## 1. CURRENT REPOSITORY TRUTH — every ship artifact, verified in source 2026-07-17

Verification method: files read on this branch's base (current `terry-local-wip`). "Device verdict" rows quote Terry's 2026-07-14 headset pass (pre-recovery build). Roadmap claims were checked against source; divergences are called out.

### 1.1 Data layer
| Artifact | Path | State |
|---|---|---|
| `ShipDefinition` | `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/ShipDefinition.cs` | FUNCTIONAL data class: `shipId` (default `rustbucket_scavenger`), `displayName`, `hullSize` (5×3×12 m), `cockpitSeatLocalPos`, `boardingDoorLocalPos`, flight feel (`cruiseSpeed` 25, `boostMultiplier` 2.5, `turnRateDegrees` 30, `flightVignette` 0.6), `slots: List<ShipSlotDef>` (slotId/slotKind/acceptedItemIds/localPos), `reachablePackIds`. |
| `ShipLoadoutCore` | `Content/Runtime/Ship/ShipLoadoutCore.cs` | FUNCTIONAL pure loadout math (chassis/engine/wings → resolved stats). Pinned by `Tests/EditMode/ShipLoadoutCoreTests.cs`. |
| `ShipLocker` | `Core/Runtime/ShipLocker.cs` | FUNCTIONAL profile-flag persistence for equipped chassis/engine/wings/name. Pinned by `ShipLockerTests.cs`. |
| Chassis presets | consumed by `ShipRefit` (`ShipChassisPreset`) | FUNCTIONAL: six silhouette presets reproportion the shared hull skeleton. |

### 1.2 Hull and visual identity
| Artifact | Path | State |
|---|---|---|
| `ShipHullBuilder` | `Editor/Patching/ShipHullBuilder.cs` | FUNCTIONAL but INTERIM BY ITS OWN CONTRACT: ~19-part procedural silhouette (tapered fuselage, raked nose, canopy, nacelles w/ glowing exhausts, wings, tail fin, landing struts, port/starboard nav lights, dorsal antenna, belly cargo pod), sized to the bounding box the boarding station expects. Header explicitly invites replacement: "Picasso's LLM asset-forge hull mesh replaces the whole assembly through the same parent when it lands." **Device verdict (2026-07-14): "the ship is a series of blocks. thats not a ship"** — the verdict stands against this hull; the interim silhouette did not clear the bar. |
| `ShipRefit` | `Gameplay/Runtime/World/ShipRefit.cs` | FUNCTIONAL: runtime re-proportioning per chassis preset (named hull parts; deck/door offsets keyed off unchanged root), livery tint (closes the `CosmeticKind.ShipLivery` seam), journey decals along the port flank, bow nameplate. Idempotent; hangar re-runs it live. |
| Forge path | `docs/design/SHIP_FORGE_AND_CUSTOMIZATION.md`, `Editor/Patching/ForgeBaker.cs`, `ForgeRecipeLibrary.cs` | The bake pipeline (mesh+maps+material+prefab → `Resources/ForgeBaked`, regenerated every build) is FUNCTIONAL and already produces creature/prop/plant looks. **No ship hull recipe exists yet** — this is the missing piece the hero ship needs. |

### 1.3 Boarding, helm, cast-off (the W000 flow — inside the golden slice)
| Artifact | Path | State |
|---|---|---|
| `ShipBoardingStation` | `Gameplay/Runtime/World/ShipBoardingStation.cs` | FUNCTIONAL S1 per `docs/systems/SHIPS.md`: boarding panel → cockpit-deck TELEPORT (never parenting), helm lists destinations story-gated via `WorldGating`, travel only through `TravelCoordinator.TravelTo`, disembark panel. Logs `ZIPTIDE: SHIP_BOARD / SHIP_DISEMBARK / SHIP_DEPART`. Serialized fields assigned by `CityBuilder` at patch time. |
| `ShipBoardingPresentationGuard` | `Gameplay/Runtime/World/ShipBoardingPresentationGuard.cs` | FUNCTIONAL (recovery-era): head-position-driven activation of cockpit/hangar/Quarters UI so the exterior arrival view isn't cluttered. Explicitly a local component, not a state owner. |
| `FirstDestinationHelmRuntime` | `Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs` | FUNCTIONAL, golden-slice-scoped: minimal helm exposing ONLY ToxicCity; delegates to `ShipCastOffRuntime`; never calls `TravelCoordinator` itself. |
| `ShipCastOffRuntime` | `Gameplay/Runtime/Story/ShipCastOffRuntime.cs` | FUNCTIONAL with ONE KNOWN DEVICE DEFECT. PUNCH IT launch with arming gate: `armingMachineId = "gate_coupler"`, resolves the scene's `RepairableMachine` by `MachineId`, arms via pure `CastOffArming` (law: no gate configured → armed; machine absent → armed — never strand). Logs `FLIGHT_BLOCKED reason=unarmed`, plus a full diag line with `gate=/repaired=`. **Device verdict: "I repaired the gate coupler and it still says i need to repair the gate coupler and i cant punch it"** — a repaired-state false negative (candidates: `RepairableMachine.IsRepaired` never flipping on device, stale `_armingMachine` cache, or repair completion UX misreading). Reproduce via the logged diag before changing code. |
| `CastOffArming` | `Gameplay/Runtime/Story/CastOffArming.cs` | FUNCTIONAL pure decision, pinned by `Tests/EditMode/CastOffArmingTests.cs`. The pure law is fail-open; the defect above is therefore in the RUNTIME wiring or machine state, not this core. |
| W000 wiring | `Editor/Patching/CityBuilder.cs:381` (attaches `ShipCastOffRuntime` to the W000_DriftIn berth ship), `FirstHourSurfaceAuthor.cs` (W000 surface markers), `WorldLayoutLibrary.BuildW000DriftIn` | FUNCTIONAL; W000_DriftIn is a generated world and part of the certified golden route. |

### 1.4 Quarters / hangar / progression surfaces
| Artifact | Path | State |
|---|---|---|
| `QuartersRoom` | `Gameplay/Runtime/World/QuartersRoom.cs` | FUNCTIONAL plumbing, CONTENT-STARVED by design note: three cosmetic bays (weapon skins / ship livery / trails+emblems) through pure `CosmeticLocker`; "no cosmetics are authored yet, so every bay shows the supply-drop stub." Host-agnostic (no ship references). |
| `QuartersPhotoWall`, `QuartersCameraFeature` | `Gameplay/Runtime/World/`, `Gameplay/Runtime/Photo/` | FUNCTIONAL; camera injection is recovery-gated (`RecoveryFeatureId.QuartersCameraInjector`). |
| `HangarBayRuntime` | `Gameplay/Runtime/World/HangarBayRuntime.cs` | FUNCTIONAL refit surface: chassis(6)/engine/wings tile rows + ship name + live holo-readout of resolved `ShipLoadoutCore` stats; equips via `ShipLocker`; re-runs `ShipRefit` live on the berth hull; first refit sets `SHIP_REFIT` story flag. |
| `ShipRefit` livery/decals | see 1.2 | FUNCTIONAL but the cosmetics catalog is empty (Quarters note above) — livery equip paths exist with nothing to equip. |

### 1.5 Flight (outside the golden slice; separate device campaign)
| Artifact | Path | State |
|---|---|---|
| `FlightModel` | `Content/Runtime/Flight/FlightModel.cs` | FUNCTIONAL pure core, comfort laws by construction (no smooth yaw; pitch clamps; transient-only barrel roll; signed throttle w/ reverse fraction; strafe; soft-wall lane). Pinned by `FlightModelTests.cs`. |
| `FlightInputCore` | `Ship/Runtime/FlightInputCore.cs` | FUNCTIONAL pure input shaping (snap-yaw latch w/ hold-to-repeat, signed throttle/strafe). Pinned by `FlightInputCoreTests.cs`. |
| `ShipFlightRuntime` | `Ship/Runtime/ShipFlightRuntime.cs` | SOURCE/CORE proven, DEVICE-UNPROVEN: helm-seat flight, world-moves-around-static-rig render, `ParamsFrom(ShipDefinition/ShipStats)` with comfort clamps, live loadout resolve via `ShipLocker`. **Device verdict: "turning in 'your ship' is super glitchy"** — snap-yaw application to the world transform is the prime suspect; needs the R1-style visual/PlayMode evidence before re-tuning. |
| `FlightCourseCore` + `ScenePatcherSpaceLane` | `Ship/Runtime/`, `Editor/Patching/ScenePatcherSpaceLane.cs` | FUNCTIONAL ring-course trial scene (`returnScene = W000_DriftIn`). Pinned by `FlightCourseCoreTests.cs`, `ShipFlightParamsTests.cs`. |
| `SpaceCombatCore` / `SpaceTargetRuntime` | `Ship/Runtime/` | CORE-proven armor-only non-lethal ship combat (disable → salvage; ships never explode). Out of hero-ship scope; listed for completeness. |
| `FlightSignals` | `Core/Runtime/FlightSignals.cs` | Present (recovery-era coordination surface). Treat as the flight lane's event contract; verify before extending. |
| `VehicleRuntime` | `Ship/Runtime/VehicleRuntime.cs` | Ground vehicles (pitch-locked FlightModel). Out of scope here. |

### 1.6 Duplicates / dead / uncertain
- **Two helm surfaces exist by design layering, not by accident:** `FirstDestinationHelmRuntime` (golden, minimal, W000-only) and `ShipBoardingStation`'s own destination helm (full list). Post-recovery these MUST be consolidated into one helm implementation with a "first-hour restriction" mode, or the R2 "one owner per responsibility" rule is violated the moment both ship. → PR-3.
- **W004_BroadcastTomb "different ship" (device verdict: "also looks like shit"):** berth ships are built per-layout by `CityBuilder.BuildShipyard`; W004's layout carries its own berth. UNCERTAIN whether W004 renders an older hull variant or the same interim hull at different proportions. Resolve during PR-1 by auditing every `ShipyardBerthDef` consumer — one hero hull must serve every berth.
- **No dead ship code found.** `SpaceLane`/combat are live-but-unexposed (not in golden scenes). Nothing ship-related was deleted by recovery; exposure is scene-lock based (GoldenSlice = `_Boot`/`W000_DriftIn`/`ToxicCity`), plus `RecoveryRuntimeGate` for injectors (no ship-specific feature id exists — ship code rides scene exposure).
- **Roadmap-vs-source check:** `PROJECT_COMPLETION_ROADMAP.md` §5 says "Ship definitions, boardable shell, helm, quarters, customization, modules, liveries, decals, fly-out presentation and SpaceLane code exist." Source confirms ALL of these EXCEPT fly-out presentation: S2 (engine audio + shake + starfield) is **still the "layers onto Depart later" comment** in `ShipBoardingStation`; `ShipCastOffRuntime` has a rails take-off sequence but the S2 cinematic-fly-out-as-presentation around travel is not implemented as described. Roadmap overstates by one item.

---

## 2. REUSE / REPAIR / RETIRE MATRIX

| Piece | Disposition | Why |
|---|---|---|
| `ShipDefinition` + `ShipSlotDef` | **Preserve unchanged** | Complete data contract; every hero-ship dimension is already a field. Extend only by adding fields (additive-save-field law). |
| `ShipLoadoutCore`, `ShipLocker`, their tests | **Preserve unchanged** | Pure, tested, profile-persisted. The hero ship changes what the numbers LOOK like, not the math. |
| `ShipBoardingStation` | **Extend behind its current owner** | The teleport/gating/travel spine is contract-correct (TravelCoordinator-only, no parenting). Hero work re-skins its geometry and panels; the callbacks stay. |
| `ShipBoardingPresentationGuard` | **Preserve unchanged** | Recovery-vetted; exactly the head-gated activation the denser hero interior needs. |
| `FirstDestinationHelmRuntime` | **Retire after proven replacement** (PR-3) | Merge into the single helm with a first-hour restriction mode; keep until the consolidated helm passes the same PlayMode route the golden slice pinned (43/43 suite must stay green through the swap). |
| `ShipCastOffRuntime` | **Repair** (PR-4) | The arming false negative is a device-proven defect. The pure core is fail-open and correct; fix the runtime machine-state wiring and add a PlayMode test reproducing the failure class. |
| `CastOffArming` + tests | **Preserve unchanged** | Pure law is right; do not touch while repairing the wiring. |
| `ShipHullBuilder` | **Presentation-only replacement** (PR-1) | Its own header defines the succession contract: baked hero mesh replaces the assembly "through the same parent." Keep the builder as the fallback body (visible-fallback policy: a missing bake must still produce a flyable, boardable ship — but never ship a candidate build showing the fallback). |
| `ShipRefit` | **Extend behind its current owner** (PR-1/PR-6) | Re-target chassis/livery/decal/nameplate application from scaled primitives to the baked hero mesh's material/attachment points. The refit CONTRACT (profile → visible hull) is exactly right. |
| `HangarBayRuntime` | **Extend behind its current owner** (PR-6) | The refit surface works; hero work upgrades what equipping visibly does. |
| `QuartersRoom` | **Preserve unchanged**; author content (PR-6) | Room plumbing is done; it needs authored `CosmeticDefinition` assets, not code. |
| `FlightModel` / `FlightInputCore` / cores + tests | **Preserve unchanged** | Comfort laws live in the math; device complaints target the runtime application layer, not the model. |
| `ShipFlightRuntime` | **Repair** (PR-7) | "Super glitchy turning" on device. Diagnose with R1 visual/PlayMode evidence FIRST (snap application, frame pacing, vignette latching); then bounded fix. |
| `SpaceCombatCore` / `SpaceTargetRuntime` / `ScenePatcherSpaceLane` | **Preserve unchanged, keep unexposed** | Not part of hero-ship scope; they inherit the hero hull whenever flight re-enters a device campaign. |
| W004 berth ship | **Repair via PR-1's berth audit** | One hull identity everywhere; per-world variation comes from livery/decals, not divergent geometry. |

---

## 3. CANONICAL OWNERSHIP MAP (unchanged and non-negotiable)

Per `docs/recovery/CANONICAL_OWNER_DECISIONS.md` and `RECOVERY_VERIFICATION_SYSTEM.md` §4:

- **`TravelCoordinator`** is the only travel owner. The helm SELECTS; `TravelCoordinator.TravelTo` MOVES. The hero ship adds zero new travel paths. (Locked contract #1, `CLAUDE.md`.)
- **`PlayerRigPersistence`** owns the persistent rig. Boarding/seating is teleportation of the rig, NEVER parenting to the hull, never direct camera movement (`docs/systems/SHIPS.md` guardrail; `SPACEFLIGHT_PHYSICS.md` law — fly the hull, never the deck; in-flight the world moves around the static rig).
- **`PlayerInputSessionGuard`** owns input session state across travel (the rb36 lesson). Seat-lock/helm interactions must observe it, not re-enable/disable input actions themselves.
- **`SaveSystem`** owns profile/disk. Ship persistence stays inside the existing seams: `ShipLocker` profile flags and additive `PlayerProfile` fields with neutral defaults.
- **`BootLoader` / `_Boot`** owns startup. The hero ship introduces no bootstrap, no `RuntimeInitializeOnLoadMethod`, no new singleton. Any new component is scene-content attached by patchers/authors.
- **No duplicate world-selection implementation:** after PR-3 exactly one helm class serves both first-hour and full modes.

Every PR in §6 lists these as forbidden-to-modify. New work REQUESTS these owners (calls their public APIs) or OBSERVES them; it never competes.

---

## 4. HERO SHIP PRODUCT DEFINITION

The single canonical hull: **the Rustbucket** (`shipId: rustbucket_scavenger` — the id already in the data). One ship, owned by the player from minute zero, that is simultaneously HOME, WORLD SELECTOR, PROGRESSION SINK, and (later) FLIGHT VESSEL.

- **Exterior silhouette & scale:** keep the authored `hullSize` 5×3×12 m footprint (berths, boarding door and cockpit offsets already key off it). Identity requirements from roadmap §5: recognizable nose, visible propulsion, landing/berth logic, human-scale door. Direction: a working salvage tug — asymmetric utility (port-side boarding spine, belly cargo pod, dorsal sensor mast), aviation nav-light convention preserved from the interim hull. Exact style is Terry's call (§8 Q1) — the packet mandates ONE silhouette readable at 150 m and at 2 m.
- **Docking/landing:** unchanged berth contract (`ShipyardBerthDef` in world layouts; `CityBuilder.BuildShipyard`). Landing struts must visually carry the hull's weight (struts + skid contact shadows).
- **Boarding path:** the existing `ShipBoardingStation` door-panel → cockpit-deck teleport, re-dressed: a physical ramp/hatch at `boardingDoorLocalPos` so the teleport reads as "I went inside," not "I got moved."
- **Cockpit:** seated helm at `cockpitSeatLocalPos`; world-space TMP destination panel (readability law from the roadmap: arm's-length readable); the PUNCH IT control and the arming state (coupler status) visible ON the console, not floating text.
- **Quarters:** the existing `QuartersRoom` parks one aft of the cockpit (already does); hero pass gives it walls that match the hull interior instead of a free-standing box.
- **Cargo/workbench:** the belly cargo pod becomes the visible economy surface — salvage crates appear as cargo fills (data-driven dressing; no new economy code).
- **Upgrade surfaces:** `HangarBayRuntime` tiles stay; equipping engine/wings/chassis must visibly change the baked hull (nacelle swap, wing profile, silhouette preset) via `ShipRefit`'s re-targeted application.
- **Windows/exterior treatment:** canopy is the ONE budgeted transparent surface; everything else emissive/opaque (Quest budget §5).
- **RILL/story locations:** RILL's ship lines already exist (`RillLineAuthor`: `enter_w000`, `cal_w000`); the hero interior gives RILL a physical speaker point on the console.
- **World-selection flow:** one helm (PR-3), story-gated by `WorldGating.MeetsRequirements`, first-hour mode = ToxicCity only (current golden behavior preserved).
- **Visible progression:** journey decals + livery + chassis changes on the hull (all existing `ShipRefit` seams); the ship is "a wearable save file" (ShipRefit's own words).
- **Ground travel vs fly-out vs free flight:** stage-locked per `docs/systems/SHIPS.md` — S1 boarding/travel (exists), S2 fly-out presentation AROUND `TravelCoordinator` travel (PR-5), S4 true flight only in dedicated flight scenes (`SpaceLane`), never scene-streaming.

---

## 5. QUEST PERFORMANCE CONTRACT (all numbers PROPOSED — baseline on first artifact run, then ratchet)

Scene-level law stays `PerfBudgetAuditRules` (tris 150k target/400k cap; renderers 900/2500; materials 25/60; realtime lights 1/3 per scene). The hero ship gets its OWN sub-budget so it can never eat a world's budget:

| Axis | Proposed budget (exterior + interior combined) |
|---|---|
| Triangles | ≤ 45,000 (LOD0); LOD1 ≤ 15,000; LOD2/impostor ≤ 2,000 |
| Renderers | ≤ 120 after bake (the 19-part builder + refit parts today are the ceiling reference; baked mesh should LOWER renderer count) |
| Unique materials | ≤ 12 (hull set + canopy + emissives + decal atlas) |
| Draw calls | ≤ 40 attributable to the ship at LOD0 (measure via the R1 perf artifact, not guessed) |
| Transparent surfaces | ≤ 2 (canopy, helm holo) |
| Realtime lights | 0 (emissive materials only — nav lights, exhausts, console) |
| Particle systems | ≤ 2 concurrent, budgeted through `VfxLibrary` caps (≤64 particles/system) if used at all |
| Collision | ≤ 20 primitive colliders exterior + interior walkable set; NO mesh colliders on the hull skin; boarding/deck colliders unchanged from `ShipBoardingStation`'s expectations |
| Audio sources | ≤ 4 (engine idle, console, RILL point, ambience) |
| LOD behavior | LOD1 at ~35 m, LOD2/impostor at ~120 m (proposed; tune from the Linux perf artifact + device pass) |

Gate: extend the existing audit (a ship-scoped rule in `Editor/Audit/`, WARN first run, BLOCK after baseline — the `PerfBudgetAuditRules` promotion pattern).

---

## 6. EXACT IMPLEMENTATION SEQUENCE — six bounded PRs

Proof lanes named per `RECOVERY_VERIFICATION_SYSTEM.md` §5: **CI** (EditMode + patch/audit), **Contract Scan**, **PlayMode Observation**, **Golden Android**, **Clean Package Proof**, **Quest**.
Global forbidden set for EVERY PR: `TravelCoordinator`, `PlayerRigPersistence`, `PlayerInputSessionGuard`, `SaveSystem`, `BootLoader`, `.github/workflows/*` (except where a PR explicitly adds an audit rule via the normal review), `Packages/*`, `ProjectSettings/*`, the checkpoint documents, and any generated evidence.

**PR-1 — The hero hull bake + berth audit (presentation only).**
Purpose: one designed hull mesh set through the Forge bake pipeline, replacing the interim assembly through `ShipHullBuilder`'s parent contract; audit all `ShipyardBerthDef` consumers (incl. W004) onto the same hull.
Files: `Editor/Patching/ForgeRecipeLibrary.cs` (or a dedicated `ShipHullRecipeLibrary`), `Editor/Patching/ShipHullBuilder.cs` (attach-baked-else-fallback), `Editor/Patching/CityBuilder.cs` (berth audit only), new EditMode tests (recipe validates, named-part contract preserved for ShipRefit).
Acceptance: EditMode green; patch/audit green; PlayMode 43/43 untouched; **visual artifact:** golden-viewpoint captures of the berth ship in W000 + a photo-booth turnaround; Golden Android build. Quest gate: silhouette read at 150 m/2 m in the PR-1 device campaign (§7).
Rollback: the fallback builder path — reverting the recipe returns the interim hull with zero wiring changes.

**PR-2 — Interior dress pass (cockpit, quarters shell, cargo).**
Purpose: interior walls/console/ramp so boarding reads as entering a vehicle; `QuartersRoom` gains hull-matched enclosure; cargo pod shows economy state.
Files: the hull recipe's interior modules, `ShipBoardingStation` geometry-build section (panel/console dressing ONLY — no logic), `ShipRefit` attachment points.
Acceptance: CI + PlayMode + visual captures (helm panel readability check via the existing UI spatial evidence class); Golden Android. Quest: interior walkthrough + panel readability.
Rollback: interior modules are additive children — remove to revert.

**PR-3 — ONE helm.**
Purpose: consolidate `FirstDestinationHelmRuntime` + `ShipBoardingStation` helm into a single destination surface with a `firstHourOnly` restriction mode; retire the tutorial helm after the PlayMode route proves the replacement.
Files: `ShipBoardingStation.cs` (helm section), `FirstDestinationHelmRuntime.cs` (retire last), `FirstHourSurfaceAuthor.cs` (marker rewire), PlayMode test additions to the existing 43-test suite.
Acceptance: **the full clean-lane suite must stay green (43/43 + additions)** — this PR touches the golden route. Contract Scan green (no new owner). Golden Android. Quest: the W000→ToxicCity selection flow unchanged in feel.
Rollback boundary: keep the tutorial helm class in-tree until one full green device campaign after the swap.

**PR-4 — Cast-off arming repair (the coupler false negative).**
Purpose: fix "repaired but still blocked." Diagnose from device evidence FIRST (`FLIGHT_BLOCKED reason=unarmed`, the `gate=/armed=/repaired=` diag line); suspects: `RepairableMachine.IsRepaired` persistence on device, stale `_armingMachine` resolution, repair-completion signaling.
Files: `ShipCastOffRuntime.cs`, `RepairableMachine.cs`; a PlayMode test that repairs the machine and asserts the gate arms (the failure CLASS, not the implementation); `CastOffArming.cs` stays untouched.
Acceptance: CI + PlayMode (new test) + Golden Android; Quest: repair → PUNCH IT succeeds.
Rollback: arming gate is fail-open by law — worst case revert leaves launch armed, never stranded.

**PR-5 — S2 fly-out presentation.**
Purpose: the missing cinematic layer AROUND travel (seat-lock beat, engine audio, camera-safe shake, window starfield, then `TravelCoordinator.TravelTo`) tuned by `ShipDefinition.flightVignette`; comfort-first, reports external motion via the existing `ComfortVignette.ReportExternalMotion` seam.
Files: `ShipBoardingStation.cs`/`ShipCastOffRuntime.cs` depart paths, one new presentation component (scene-content, no bootstrap), `AudioDirector` consumption only.
Acceptance: CI + PlayMode (travel route stays green) + visual capture; Golden Android. Quest: comfort verdict on the fly-out.
Rollback: presentation flag off → instant depart (current behavior).

**PR-6 — Progression made visible (refit re-target + starter cosmetics).**
Purpose: `ShipRefit` applies chassis/engine/wings/livery/decals onto the baked hull's attachment points; author a starter `CosmeticDefinition` set (3 liveries minimum) so Quarters bays stop showing stubs; hangar equip = visible hull change.
Files: `ShipRefit.cs`, `HangarBayRuntime.cs` (display only), a create-only `CosmeticAuthor` extension, EditMode tests for preset→attachment mapping.
Acceptance: CI + visual captures per chassis preset (6 turnarounds); Golden Android. Quest: equip in hangar → see the change on the berth.
Rollback: refit falls back to current scale-based application.

**PR-7 (flight campaign opener — only after PR-1..6 and a green device pass) — Flight turn feel.**
Purpose: diagnose-then-fix "super glitchy turning" in `ShipFlightRuntime` with PlayMode + visual evidence before touching values (snap application timing, world-transform stepping, vignette latch).
Files: `ShipFlightRuntime.cs`, possibly `FlightInputCore.cs` cadence constants; cores' tests extended.
Acceptance: CI + a recorded flight-path PlayMode observation + device campaign in `SpaceLane_Trial`. Explicit Quest gate — flight ships nothing without a device PASS.
Rollback: flight stays unexposed (not in any golden/candidate scene list) until green.

---

## 7. FIRST DEVICE CAMPAIGN (the shortest session that proves the hero ship)

Run AFTER PR-1+2 land (PR-3..6 add their own beats). 10–14 minutes, on the then-current certified candidate profile:

1. **Approach (2 min):** from W000 spawn, look at the berth from ~30 m — does the silhouette read as a vehicle with a nose, engines, and a door? Walk a full circle — struts, nav lights, no interpenetrating geometry.
2. **Board (2 min):** ramp/hatch panel → cockpit deck. Verdict: "did I go inside?" Deck collision solid; no clipping into hull walls; `SHIP_BOARD` in log.
3. **Interior walk (2 min):** cockpit → quarters → cargo. Physical scale verdict (ceiling height, corridor width vs arm span).
4. **Panel readability (2 min):** helm console at seated arm's length — every destination row legible without leaning; coupler/arming state visible; no mirrored text (the R1 facing checks should have caught this pre-device).
5. **Select + travel (2 min):** pick ToxicCity, confirm; travel completes through the crest; `SHIP_DEPART dest=` logged; input alive on arrival (PlayerInputSessionGuard evidence class).
6. **Return + state (2 min):** travel back; ship still berthed, boarding still works, no duplicate ships, save survives quit/resume.
7. **Comfort/pacing throughout:** no judder during boarding teleports; 72 Hz hold near the ship (frame-pacing sample from the device log; compare against the Linux perf artifact).

Evidence: full logcat + the standard screenshot set; verdicts recorded per beat in the runbook style (✅/❌ + one line).

---

## 8. OPEN QUESTIONS (genuinely Terry's — everything else is decided above)

1. **Silhouette taste:** the six `ShipChassisPreset` shapes exist as proportion sets. Which direction is THE Rustbucket — boxy tug, raked interceptor, or heavy hauler? (One reference image or a "more like X" sentence is enough; the bake starts from it.)
2. **Scale feel:** is the current 5×3×12 m footprint right on device, or should the hero ship feel bigger inside? (Changing `hullSize` moves berth/door/seat data — cheap in data, decide once.)
3. **Helm posture:** seated cast-off (current) or standing console? Affects comfort defaults and console height.
4. **Interior mood:** warm lived-in scavenger vs cold utility? Drives the interior palette in PR-2.
5. **The name:** keep "Rustbucket" as the canonical display name?

---

## 9. HANDOFF

- **Branch:** `fable/hero-ship-production-packet` — this document only. Draft PR open; DO NOT MERGE before the Quest checkpoint verdict.
- **Files created:** `docs/post_recovery/HERO_SHIP_PRODUCTION_PACKET.md` (this file).
- **Evidence inspected:** `docs/systems/SHIPS.md`; `docs/design/SHIP_SYSTEM.md`/`SHIP_FORGE_AND_CUSTOMIZATION.md`/`SPACEFLIGHT_PHYSICS.md`/`CONTROLS_AND_FLIGHT.md`/`SPACE_COMBAT.md`; `docs/PROJECT_COMPLETION_ROADMAP.md` §5; `docs/recovery/` (program, verification system, checkpoint, exposure manifest, `RecoveryFeatureId.cs`); source: `ShipDefinition.cs`, `ShipLoadoutCore.cs`, `ShipLocker.cs`, `ShipHullBuilder.cs`, `ShipRefit.cs`, `HangarBayRuntime.cs`, `QuartersRoom.cs`, `ShipBoardingStation.cs`, `ShipBoardingPresentationGuard.cs`, `FirstDestinationHelmRuntime.cs`, `ShipCastOffRuntime.cs`, `CastOffArming.cs`, `FlightModel.cs`, `FlightInputCore.cs`, `ShipFlightRuntime.cs`, `SpaceCombatCore.cs`, `ScenePatcherSpaceLane.cs`, `CityBuilder.cs` (W000/berth wiring), `FirstHourSurfaceAuthor.cs`, `RillLineAuthor.cs`, plus all seven ship/flight EditMode test suites.
- **Unresolved:** §8's five taste questions; the W004 berth-hull variant question (§1.6, resolved inside PR-1's audit); exact perf numbers (all labeled proposed, to be baselined from the first R1 perf artifact).
- **Recommended first implementation PR after recovery exits:** **PR-1 (hero hull bake + berth audit)** — it is presentation-only, fully rollback-safe behind the fallback builder, unblocks every other PR's visual identity, and directly answers the standing device verdict that triggered this packet.
