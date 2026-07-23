# ZIPTIDE OFFLINE PRODUCTION QUEUE — 2026-07-23, revision 2

**Purpose:** continue meaningful ZIPTIDE production while Terry is away from the Windows/Quest setup.
This is not a license to disturb the recovery experiment. The exact authorized headset artifact remains
`recovery-golden-apk-c45b1a2…` from Golden Android run `29786604008`.

## Boundary

The project has two simultaneous truths:

1. **The recovery-critical device route is frozen** until Terry tests the exact authorized artifact.
2. **The whole project is not frozen.** Deterministic tooling, data contracts, visual specifications,
   mission catalogs, content intake, and other work that cannot alter that device route may continue.

Do not modify or replace before the headset verdict:

- XR rig/input lifecycle;
- weapon scale, pose, attach transforms, colliders, or hand interaction;
- coupler height, visibility, release, selection, or power sequence;
- `PUNCH IT`, ToxicCity travel, arrival, or post-travel input restoration;
- scenes or a new APK presented as the recovery candidate.

The first back-at-computer action remains `docs/testing/HEADSET_RETRY_C45B1A2.md`.

---

## Completed in this offline production round

### 1. Fast evidence and failure handling

- `tools/factory_governance_gate.py`
- `tools/dev_preflight.ps1`
- `.github/workflows/fast-preflight.yml`
- downloadable `fast-preflight-evidence-<sha>` artifact
- `tools/check_dev_capabilities.ps1`
- exact headset retry card
- recovery fast-path and lifecycle/evidence-lock rules

The fast workflow found a stale dev-menu test. Runtime authority had correctly moved F2, headset gesture,
and ADB backup to `DevWarpBoard`; the test still inspected retired `DevMenu`. The test now follows the
authoritative owner and guards against summon ownership returning to the retired canvas.

### 2. Concept art converted into build contracts

- `docs/project_art_plan/DEVICE_GATE_PRODUCTION_PACK.md`
- `docs/project_art_plan/concept_intake_manifest.json`
- `tools/concept_intake_gate.py`
- `tools/tests/test_concept_intake_gate.py`

Build-ready specifications exist for:

- Artifact Key;
- RILL Orb;
- Ziptide Gate lifecycle;
- MK2 Architect living yacht.

The Artifact Key is now **intake-ready**. Its exact keeper is
`concepts/artifact_key/key_two_half_states_v1.png`, with separated / within-two-inches arcing / joined
flowing states. The MK2 bedroom keeper remains explicitly missing; its room and bunk contract is already
protected below.

### 3. Concept-versus-build comparison core

- `tools/reference_plate.py`
- `tools/tests/test_reference_plate_gate.py`

The tool takes a keeper PNG and booth-render PNG and produces a deterministic standalone SVG comparison
plate plus optional JSON metadata. This is the non-Unity core of Forge booth reference mode. Unity
integration remains post-device-gate runtime work.

### 4. Space mission loop converted into enforceable data

- `docs/design/space_mission_catalog.json`
- `tools/space_mission_catalog_gate.py`
- `tools/tests/test_space_mission_catalog_gate.py`

The ten canon mission types are machine-readable. The gate protects the core five, pacifist completion,
no timers, relay→Signal consequence, The Approach's unique endgame role, and the locked flight scope:
arcade, no EVA v1, no punitive fuel, soft-lock, HOTAS optional, combat as optional spice.

### 5. Ground-to-space celestial consistency

- `docs/design/celestial_system_catalog.json`
- `tools/celestial_system_gate.py`
- `tools/tests/test_celestial_system_gate.py`

W001 ground sky and Moss orbital space share one proposed system truth: one warm star, ringed parent
giant, sibling grey moon, shared starfield/galactic band, object palette, and matching bearings. Names,
exact colors, angular sizes, and keeper-derived sun direction remain author-required, not guessed.

### 6. MK2 interior made structurally buildable

- `docs/project_art_plan/mk2_room_socket_manifest.json`
- `tools/mk2_room_manifest_gate.py`
- `tools/tests/test_mk2_room_manifest_gate.py`

The MK2 has an enforceable seven-room order, more than fifty stable gameplay/route sockets, four required
bunk sockets, fixed-versus-skinnable identity zones, automation/fabricator/drive-heart/airlock contracts,
and child-reachable essential cockpit controls.

### 7. Unified offline readiness evidence

- `tools/offline_readiness_report.py`
- `tools/tests/test_offline_readiness_report_gate.py`
- `Builds/Reports/offline_readiness.json` in Fast Preflight artifacts

The report distinguishes deterministic failure, report-only warning, awaiting CI, and awaiting Terry's
device. Temporary subreports remain inside `Builds/Reports` to satisfy repository path-safety rules.
Warnings remain visible without becoming false blockers; error/blocker findings still stop the lane.

### 8. Seven-state gate lifecycle contract

- `docs/project_art_plan/gate_lifecycle_catalog.json`
- `tools/gate_lifecycle_gate.py`
- `tools/tests/test_gate_lifecycle_gate.py`

Dormant → waking → open → traversal → exit/reform → collapse → entrainment surge is now validated data.
TravelCoordinator retains transition ownership; the XR rig retains camera ownership. Camera motion,
forced FOV, full-field strobe, early input restoration, excessive particles, and normal-state transparent
coverage are gated.

### 9. First Moss-orbit POI palette

- `docs/design/space_poi_catalog.json`
- `tools/space_poi_catalog_gate.py`
- `tools/tests/test_space_poi_catalog_gate.py`

Six POIs cover every core mission without runtime spawning:

- prior-waker smallcraft;
- disabled salvage tug;
- debris ribbon;
- orbital repair relay;
- stranded civilian ship;
- Ziptide gate approach lane.

Each declares mission compatibility, pacifist access, budget/LOD band, salvage, story chance, ownership,
hazards, and stable sockets. Relay repair preserves the Signal hook; prior-waker wrecks preserve the
ordered log thread.

### 10. Launch and re-entry implementation packet

- `docs/design/launch_transition_catalog.json`
- `tools/launch_transition_gate.py`
- `tools/tests/test_launch_transition_gate.py`

W001→Moss orbit and Moss orbit→W001 are exact data routes. The packet validates liftoff/ascent/veil/
arrival and re-entry veil/descent/touchdown; pulls all celestial values from the shared catalog; permits
scene-load hold only inside the one-to-two-second plasma veil; and restores control only after floor,
horizon, and input stability.

### 11. RILL visual progression contract

- `docs/project_art_plan/rill_visual_state_catalog.json`
- `tools/rill_visual_state_gate.py`
- `tools/tests/test_rill_visual_state_gate.py`

The catalog maps all nine exact `RillMemoryState` values rather than inventing a second emotion runtime.
It records current presenter colors, explicitly names the six-state amber-default collapse, protects the
repair panel and independent iris/channels, defines personal-space and glitch comfort rails, and keeps
endgame visual treatments proposed/author-required until reviewed.

---

## Current evidence state

- All new direct validators are included in Fast Preflight.
- The full `test_*_gate.py` suite is captured in a downloadable artifact on failure.
- The latest source is newer than the last durable Unity/audit verdict; that is **awaiting CI**, not a
  gameplay failure.
- `first_hour_binding` currently reports five non-blocking evidence warnings because exact source tokens
  moved in `BootLoader` and `RepairableMachine`. The current owners are known; reconcile the evidence
  inventory before ratcheting that report to strict.
- The exact Quest route remains **awaiting device**, not failed.

---

## Next safe queue — priority order

### N1 — Reconcile first-hour binding evidence

**Goal:** update the evidence inventory from retired exact tokens to the current source owners without
changing runtime behavior.

**Known drift:** `BootLoader` now logs/uses `destination`; `RepairableMachine` now uses the explicit
`State.Off → State.On → State.Broken` state machine and `PowerOn()` / `BreakDown()`.

**Acceptance:** `first_hour_binding_gate.py --strict` passes; no runtime file is changed merely to satisfy
an old token.

### N2 — Main CI queue optimization

**Goal:** stop superseded commits from consuming simultaneous Unity runs and make cheap preflight a hard
prerequisite for expensive Unity jobs.

**Acceptance:** same durable source-verdict semantics; stale verdict writes remain refused; newer branch
commits cancel obsolete Unity runs; Unity does not start when deterministic Python governance is red.

This is a shared/protected workflow edit. Keep it one focused change after Fast Preflight is confirmed.

### N3 — SunRig interface and authored-data packet

**Goal:** define the non-runtime interface between the celestial catalog and future `SunRig`.

**Acceptance:** primary/secondary key-light roles, disc bearing, hull terminator, glare comfort caps,
multi-sun support, and ground/orbit parity all consume shared celestial IDs rather than Moss-specific
hardcoding.

### N4 — Mission catalog translator design

**Goal:** map mission JSON into existing `JobDefinition`/step/economy/flag contracts without implementing
runtime spawning.

**Acceptance:** every catalog step has one existing or proposed translator; rewards route through
`ProfileEconomy`; relay repair owns Signal; no duplicate mission engine is proposed.

### N5 — POI spawn-definition packet

**Goal:** map the six POI families into future world-pack/spawn definitions.

**Acceptance:** stable IDs, seed rules, per-scene caps, LOD bands, sockets, mission compatibility, and
salvage tables; runtime spawning remains unauthorized until the device gate clears.

### N6 — Missing MK2 bedroom keeper and measured room spec

**Goal:** complete the only missing room reference without changing the seven-room layout.

**Acceptance:** four bunk pods, privacy shutters, personal cubbies/keepsakes, child-readable circulation,
fixed identity zones, and comparison against the MK1 bunk wall. Until image capability is used, the
manifest remains `keeper-missing-required` rather than pretending the lounge is the bedroom.

---

## Work that begins only after the exact headset pass

1. Integrate `reference_plate.py` into ForgePhotoBooth.
2. Build the Artifact Key end-to-end.
3. Implement `SunRig` from the celestial catalog.
4. Compose the full Moss space vista into `SpaceLane_Trial`.
5. Implement the launch plasma veil through `TravelCoordinator`.
6. Stop for a comfort/presence headset verdict.
7. Add parallax bodies, restrained speed VFX/audio, POIs, and the mission catalog translator.

The order buys the largest visible and experiential improvements before floating origin, large-scale
combat, atmospheric free flight, or walk-the-flying-ship interior physics.

---

## Back-at-computer list

1. Run `./tools/check_dev_capabilities.ps1` from PowerShell.
2. Keep/install the exact `recovery-golden-apk-c45b1a2…` artifact; do not replace it with current head.
3. Follow `docs/testing/HEADSET_RETRY_C45B1A2.md` twice in the same install.
4. Save checkpoint/logcat evidence for any failure.
5. Only after the verdict, rebase current `terry-local-wip`, run `./tools/dev_preflight.ps1`, and resume
   the post-device sequence above.
