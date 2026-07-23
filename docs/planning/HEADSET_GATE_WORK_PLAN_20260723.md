# HEADSET-GATE WORK PLAN — 2026-07-23

## Decision

ZIPTIDE is **not globally blocked**, but the recovery-critical runtime chain is device-gated.

The exact Golden artifact `recovery-golden-apk-c45b1a2…` remains the only authorized headset build
until Terry runs `docs/testing/HEADSET_RETRY_C45B1A2.md`. Do not change or re-diagnose XR input,
weapon transforms/scale, coupler interaction, or the `PUNCH IT` travel chain without new headset
evidence.

Other work may advance only when it cannot invalidate, obscure, or replace that device verdict.

---

## 1. Work that is safe before Terry returns

### A. Process and evidence tooling — ACTIVE

- Fast local governance preflight: `tools/factory_governance_gate.py`.
- Regression tests: `tools/tests/test_factory_governance_gate.py`.
- One-command Windows wrapper: `tools/dev_preflight.ps1`.
- Mandatory operator procedure: `docs/FAST_PREFLIGHT.md` + `docs/OPERATOR_START_HERE.md`.
- Exact device card: `docs/testing/HEADSET_RETRY_C45B1A2.md`.

These changes do not alter the Golden gameplay source.

### B. Concept-to-build paper work — SAFE

Picasso/Fable's concept wave is mature enough to convert into measured build specifications without
opening Unity runtime scope. The current capability audit already defines the route:

`keeper concept → measured visual spec → recipe/intake plan → booth comparison → gates → headset`.

Safe deliverables while device-gated:

1. **Artifact key visual spec** — proportions, grip/face-distance scale, material zones, three states,
   frozen-current channels, arc sockets, collider envelope, Forge part decomposition.
2. **RILL visual spec** — orb proportions, repair panel, eye/iris, emissive channels, four `RillState`
   mappings, close-face texel requirement.
3. **Gate lifecycle visual table** — dormant, waking, open, traversal, collapse, entrainment states;
   transparent-coverage and comfort constraints.
4. **Warden capital distance-band spec** — P3 silhouette first; near-pass promotion criteria only.
5. **MK2 build intake sheet** — fixed identity zones versus skinnable zones, 25–30 m scale, LOD bands,
   seven-room interior list, reconfigure transforms, graft hardpoints, and Tier-C intake requirements.
6. **Missing MK2 bedroom keeper** — generate/file only when image capability is available; until then
   retain the canonical requirement for dedicated bunks.

No one should turn 2D concept files into gameplay prefabs ad hoc. Every asset enters through the
Forge/WC-5/applier contracts.

### C. Planning reconciliation — SAFE

The June GPT planning files on the older default-branch stream are references, not current authority.
Their durable ideas are already represented in the active architecture:

- recipes/registries instead of hardcoded content;
- tiny scene shells and patcher-driven generation;
- compact first-world onboarding instead of an oversized prelude;
- named modular regions, vehicle-width routes, and a dormant gate endpoint.

Do not cherry-pick that stream wholesale. Reuse only ideas that survive comparison with the current
WorldSpec/factory, story bible, Moss concepts, and Recovery freeze.

---

## 2. Runtime work that waits for the headset verdict

Do not start these against the active gameplay branch before the exact Golden retry:

- any XR rig/input lifecycle edit;
- weapon attach transforms, scale, colliders, or hand pose changes;
- coupler height, visibility, interaction, selection, or release behavior;
- `PUNCH IT`, ToxicCity arrival, or travel/input restoration changes;
- a replacement APK presented as the recovery test;
- broad scene changes that make Terry's observed route differ from `c45b1a2`.

This is not fear of building. It is experimental control: the current device test must answer one
bounded question against one proven artifact.

---

## 3. Post-headset fork

### If the exact Golden route passes

1. Record the device verdict and close the weapon/coupler/recovery checkpoint.
2. Rebase/pull current `terry-local-wip`; run `.\tools\dev_preflight.ps1`; require ordinary CI green.
3. Begin the **highest-return visual/space sequence**, one envelope at a time:

#### Envelope V1 — Booth reference-plate mode

**Goal:** every procedural/converted asset renders beside its keeper concept in one CI artifact.

**Acceptance:** deterministic comparison image; no runtime dependency; unit coverage for reference
selection/layout; existing booth output unchanged when no reference is assigned.

#### Envelope V2 — Artifact key pipeline tutorial

**Goal:** build the smallest close-range hero asset end-to-end using the full concept acceptance loop.

**Acceptance:** measured spec, Tier-B recipe, separated material zones, three readable states,
conformance/budget gates, comparison plate, then headset verdict.

#### Envelope S1 — Felt-space composition

Build in the audited impact-per-cost order:

1. `SunRig` — real directional key light + disc/flare relationship + hull terminator; data supports
   multiple suns but Moss begins with one warm sun.
2. Space-vista composition — full 360 starfield/nebula and shared celestial truth in
   `SpaceLane_Trial`.
3. Launch plasma veil — smooth diegetic mask around `TravelCoordinator`; no strobe, no camera motion.

After S1, stop for a device comfort/presence verdict before adding open-volume complexity.

#### Envelope S2 — Space depth and presence

- `DistantBodyRig` for receding launch world and moon parallax;
- restrained speed particulate and engine/hull audio;
- first debris/derelict POI palette and cockpit tractor-salvage interaction.

#### Envelope S3 — Mission loop

Author the core five as data-driven space jobs:

- salvage run;
- derelict discovery/lore dive;
- ferry/passage contract;
- distress/rescue;
- relay/seal repair that raises Signal.

All remain pacifist-viable and route rewards through existing economy/flags.

Atmospheric free flight, dock-only landing, faction missions, open-volume floating origin, combat
expansion, and walk-the-flying-MK2 interior follow only after those smaller slices prove themselves.

### If any Golden route item fails

1. Preserve the exact artifact and attach checkpoint/log evidence.
2. Enter `docs/recovery/RECOVERY_DEBUG_FAST_PATH.md`.
3. Pin one failed surface and one owner/property before editing.
4. Do not combine weapon, coupler, travel, and input corrections in one candidate.
5. Require the same verification ladder again before authorizing a replacement headset artifact.

---

## 4. Picasso/Fable review — what is now real planning authority

### MK2 ship

- Canon exterior and full seven-view set are approved.
- It is a 25–30 m multi-deck living yacht, not a tiny fighter.
- Signature: solid iridescent grown-alloy airframe, cyan frozen-current circulation, gill nacelles,
  detached wingtip vanes, dorsal greenhouse, and visible salvaged grabber/plating graft.
- Cal's fixed identity anchors remain across skins; hull plates, channel color, and trim are swap zones.
- Interior authority: cockpit, lounge/galley, dedicated bedroom/bunks, greenhouse, drive-core,
  workshop/fabricator, airlock. The existing blueprint omits the dedicated bedroom and must not be
  treated as complete.
- Gameplay contracts remain `ShipDefinition`/coupler/`TravelCoordinator`; the MK2 is a visual and
  capability upgrade, not a parallel travel engine.

### Space-flight loop

- Flight verb exists and is comfort-tuned; environment/place is the major built gap.
- Locked posture: arcade, not Newtonian; virtual-grab base; HOTAS optional; no punitive fuel; no EVA
  in v1; soft-lock targeting; combat is optional spice.
- Navigation/story/failure are now designed: diegetic beacon/RILL bearings, transmissions in flight,
  distress auto-tow rather than harsh game-over, dock autopilot, gentle boundaries/terrain assist.
- Mission types come from the story bible rather than filler: wrecks are prior breakout attempts,
  passage credits are the leash, and relay repair wakes the Signal.

---

## 5. Capability upgrade that would materially accelerate future work

The highest-value connection is still a **writable Windows development machine** with:

- the current repo checkout and authenticated `git`/`gh`;
- Unity 2022.3.62f3 batchmode available to scripts;
- Python 3 and PowerShell;
- Android SDK/ADB and the authorized Quest connection;
- permission to run local EditMode, patch/audit, build/install, and logcat commands.

That turns the loop into:

`edit → fast preflight → local targeted test → CI proof → install → checkpoint/logcat`

instead of API whole-file edits plus waiting for every GitHub-hosted Unity result. It would reduce
latency and clerical risk, but it would not replace exact-SHA CI/Recovery/Golden/device evidence.

## Current recommendation

Until Terry is at the computer, keep the authorized Golden artifact frozen, finish only paper/tooling
work, and do not create a new gameplay APK. The first hands-on action is the exact headset card; the
first post-pass build is booth-reference tooling followed by the artifact key and the three-part
felt-space slice.
