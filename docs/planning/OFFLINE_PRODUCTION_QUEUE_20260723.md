# ZIPTIDE OFFLINE PRODUCTION QUEUE — 2026-07-23

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
- downloadable `fast-python-gates-<sha>` failure artifact
- `tools/check_dev_capabilities.ps1`
- exact headset retry card
- recovery fast-path and lifecycle/evidence-lock rules

A stale dev-menu test was discovered by the new fast workflow. The source correctly assigned F2,
headset gesture, and ADB backup to `DevWarpBoard`; the test still inspected retired `DevMenu`. The test
now follows the authoritative owner and guards against ownership returning to the retired canvas.

### 2. Concept art converted into build contracts

- `docs/project_art_plan/DEVICE_GATE_PRODUCTION_PACK.md`
- `docs/project_art_plan/concept_intake_manifest.json`
- `tools/concept_intake_gate.py`
- `tools/tests/test_concept_intake_gate.py`

Build-ready specifications now exist for:

- Artifact Key;
- RILL Orb;
- Ziptide Gate lifecycle;
- MK2 Architect living yacht.

The manifest verifies keeper/source/spec links. Unknowns remain explicit rather than guessed: the
standalone Artifact Key keeper filename is unresolved, and the MK2 bedroom keeper is still missing.

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

W001 ground sky and Moss orbital space now share one proposed system truth: one warm star, ringed parent
giant, sibling grey moon, shared starfield/galactic band, object palette, and matching bearings. Names,
exact colors, angular sizes, and keeper-derived sun direction remain author-required, not guessed.

### 6. MK2 interior made structurally buildable

- `docs/project_art_plan/mk2_room_socket_manifest.json`
- `tools/mk2_room_manifest_gate.py`
- `tools/tests/test_mk2_room_manifest_gate.py`

The MK2 now has an enforceable seven-room order, more than fifty stable gameplay/route sockets, four
required bunk sockets, fixed-versus-skinnable identity zones, automation/fabricator/drive-heart/airlock
contracts, and child-reachable essential cockpit controls.

---

## Next safe queue — priority order

### O1 — Offline readiness report

**Goal:** one command/report that runs every cheap repository gate and identifies READY / BLOCKED /
AWAITING-DEVICE surfaces without launching Unity.

**Inputs:** the factory, concept, mission, celestial, MK2, continuity, and first-hour validators.

**Acceptance:** deterministic JSON; a failing sub-gate names its tool and findings; the report explicitly
marks the exact headset route as awaiting device rather than failed.

### O2 — Artifact Key intake closure

**Goal:** finish the smallest hero-asset intake packet without building the runtime asset.

**Work:** locate or author the exact standalone keeper reference; lock measured proportions against that
keeper; split grip/join/arc/coupler sockets; define three-state comparison captures.

**Acceptance:** concept manifest advances from `spec-ready-concept-unresolved` to `intake-ready` only when
the exact keeper path exists and all build-acceptance markers pass.

### O3 — RILL state-material contract

**Goal:** turn the four keeper mood states into a small machine-readable state table.

**Work:** iris state, channel color/intensity, amber structural warning, movement cue, personal-space
behavior, and accessibility-safe luminance limits.

**Acceptance:** all states map to existing `RillState`; no duplicate emotion runtime; no strobing state;
repair panel and eye remain protected across skins/LODs.

### O4 — Gate lifecycle contract data

**Goal:** express dormant→waking→open→traversal→exit→collapse→entrainment as validated state data.

**Acceptance:** every state names visual layers, duration bounds, transparent coverage, particle cap,
comfort behavior, and travel ownership; no state moves the camera or restores control early.

### O5 — Space POI and salvage palette

**Goal:** define the first Moss-orbit content palette that gives missions places to happen.

**Work:** small prior-waker wreck, salvage tug, debris ribbon, repair relay, distress ship, and one gate
approach lane; all reuse approved faction/material families.

**Acceptance:** each POI declares mission compatibility, distance/LOD band, salvage outputs, log/story
chance, faction ownership, and Quest budget class. No runtime spawning until the device gate clears.

### O6 — Launch-transition implementation packet

**Goal:** convert the launch/atmosphere documents into an exact build envelope for later runtime work.

**Acceptance:** one data contract for ascent phases, planet-recede reveal, plasma-veil timing, control
ownership, comfort limits, audio cues, failure recovery, and required logs. It must consume the celestial
catalog rather than repeat sun/body direction data.

### O7 — Main CI queue optimization

**Goal:** stop superseded commits from consuming simultaneous Unity runs and make cheap preflight a hard
prerequisite for expensive Unity jobs.

**Acceptance:** same source-verdict semantics, stale verdict writes still refused, newer branch commits
cancel obsolete runs, and Unity never starts when deterministic Python governance is red.

This workflow edit is valuable but shared/protected. It should be one focused commit after the fast
suite is green, with no unrelated source changes.

---

## Work that begins only after the exact headset pass

1. Integrate `reference_plate.py` into ForgePhotoBooth.
2. Build the Artifact Key end-to-end.
3. Implement `SunRig` from the celestial catalog.
4. Compose the full Moss space vista into `SpaceLane_Trial`.
5. Implement the launch plasma veil through `TravelCoordinator`.
6. Stop for a comfort/presence headset verdict.
7. Add parallax bodies, restrained speed VFX/audio, POIs, and the mission catalog translator.

The order deliberately buys the largest visible and experiential improvements before floating origin,
large-scale combat, atmospheric free flight, or walk-the-flying-ship interior physics.

---

## Back-at-computer list

1. Run `./tools/check_dev_capabilities.ps1` from PowerShell.
2. Keep/install the exact `recovery-golden-apk-c45b1a2…` artifact; do not replace it with current head.
3. Follow `docs/testing/HEADSET_RETRY_C45B1A2.md` twice in the same install.
4. Save checkpoint/logcat evidence for any failure.
5. Only after the verdict, rebase the current branch, run `./tools/dev_preflight.ps1`, and resume the
   post-device sequence above.
