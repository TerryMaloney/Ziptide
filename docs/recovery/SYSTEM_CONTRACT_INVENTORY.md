# ZIPTIDE SYSTEM CONTRACT INVENTORY

**Status:** R0 IN PROGRESS  
**Machine-readable source:** [`system_contracts.json`](system_contracts.json)  
**Program:** [`RECOVERY_PROGRAM.md`](RECOVERY_PROGRAM.md)

## 1. What this inventory is

This is not another feature checklist. It records who owns each runtime responsibility, how it starts, what persists, what it creates during runtime, what proof actually exists, what conflicts with it, and whether it is allowed into the recovery candidate.

The initial pass covers **26 critical systems**. It is intentionally incomplete until the remaining repository-wide scans listed in §6 are performed.

## 2. Initial architectural conclusion

ZIPTIDE has strong pure cores and generators but weak composition ownership.

The most important current facts are:

1. **The persistent XR rig is a service container.** `PlayerRigPersistence` owns or ensures XRI wiring, fall safety, boot hold, belt, credits HUD, RILL, ping, quick swap, avatar and stun receiver. A defect in this class or its scene-adoption logic affects every world.
2. **Runtime presentation is fragmented.** Home Hub, developer menu, travel doors, Quarters, Tidefront table and credits HUD each implement independent placement, text, materials, interaction binding and facing.
3. **Item presentation has no single contract.** Physical envelope, hand grip, holster pose, muzzle direction, impact axis and shooter-ignore behavior are distributed across definitions, factory defaults, sockets and weapon runtimes.
4. **Global scene presentation has multiple active paths.** Legacy sky, SkyVista, grade, fog, practical-light runtime quads and Forge fallbacks can combine in ways that isolated tests do not reproduce.
5. **The world pipeline validates structure more strongly than runtime experience.** Scene patching and audits run before or without the complete runtime-created composition visible to the player.
6. **Pure gameplay cores can be good while their surfaces are unusable.** Conquest is the clearest example: a substantial deterministic simulation exists behind an unreadable runtime table.
7. **The project’s previous completion vocabulary was invalid.** Many entries marked complete or excellent have only CORE/PATCHED/APK proof and have failed or remain untested at PLAYMODE/VISUAL/QUEST levels.

## 3. Exposure decision for the recovery candidate

### Golden path

These must be made coherent and proven:

- boot flow;
- persistent XR rig and interaction manager;
- Home Hub;
- TravelCoordinator;
- save/profile lifecycle;
- one tool/weapon path;
- one belt/holster path;
- one grounded creature and non-lethal resolution;
- one job/reward path;
- one canonical ship use/boarding moment;
- audio/runtime-health support required by that path.

### Hidden prototypes

These remain in the repository but are excluded from the initial recovery build:

- Quarters room presentation;
- Tidefront/Conquest table presentation;
- PvP/Photon surfaces;
- ship flight unless a stationary ship-use moment cannot prove the loop;
- melee/hammer implementations;
- zipline;
- unverified practical-light halo/pool quads;
- broad world and arena selection;
- all nonessential worlds and catalog systems.

### Replace or merge before exposure

- travel-door UI;
- persistent credits HUD;
- item presentation defaults;
- sky/grade ownership;
- ship presentation roots;
- repair/objective ownership;
- any remaining duplicate menu/input path.

## 4. Critical contract clusters

### A. Boot, rig and travel

Current candidate chain:

`BootLoader → HomeHubRuntime → TravelCoordinator → PlayerRigPersistence teleport/XRI/inventory restore`

This is the core of R3, but it has no PlayMode round-trip proof. It also creates/binds runtime interaction surfaces before the current test pipeline observes them.

Required R1 tests:

- boot remains stable while idle;
- exactly one rig/camera/XRI manager exists;
- Home Hub tiles bind to the live manager;
- one selection causes one travel;
- fake tracked-head offset is preserved;
- spawn settles before locomotion/fall checks resume;
- inventory/save restoration completes;
- return travel leaves no duplicate persistent services.

### B. Runtime UI and HUD

Current independent implementations:

- `HomeHubRuntime` — primitive board/tiles/TextMesh;
- `DevWarpBoard` — primitive board/tiles/TextMesh;
- `DevMenu` — retained TMP Canvas diagnostic with independent EventSystem repair logic;
- `WorldTravelStation` — doors and opposite-face labels;
- `CreditsHud` — persistent camera-relative TextMesh;
- `QuartersRoom` — multiple primitive bays and TextMesh controls;
- `ConquestTableRuntime` — all map/control/catalog text at once.

R2 requires one diegetic panel contract that owns:

- panel backing and visible front;
- text bounds, width, wrap and clipping;
- layout groups/pages;
- target collider and interaction-manager binding;
- contrast and distance;
- snapshot-test metadata;
- whether a panel is fixed, head-relative or world-relative.

### C. Item, holster and weapon presentation

Current split:

- item definition data;
- `ItemFactory` fallback scale and 45-degree gun grip;
- Forge visual child;
- XRI attach transform;
- belt socket acceptance;
- no independent holster pose;
- weapon-specific muzzle and projectile behavior;
- no complete shooter/self-ignore contract;
- separate hammer/thumper implementations.

R2 requires one item presentation schema containing:

- physical envelope;
- right/left grip;
- right/left holster;
- muzzle position and forward axis;
- impact axis;
- owner/self collision exclusions;
- visual fallback shipping classification.

### D. World and visual composition

The intended chain is valuable and should remain:

`WorldSpec → compiler/validator → layout/pack → patcher/builders → generated scene → runtime visual systems`

The problem is the last step. Runtime sky bodies, grade volumes, practical-light quads, HUD/UI and visual fallbacks are not sufficiently represented in the authored-scene audit.

R1 must capture the post-`Awake`/`Start` hierarchy and screenshots. R2 must establish one active scene-global visual owner and prevent prototype fallbacks from silently entering the candidate build.

### E. Pure core versus player-facing translator

Preserve these cores unless later evidence contradicts them:

- WorldSpec validation/generation cores;
- save/profile serialization;
- economy/reward core;
- PvP mode and bot cores;
- conquest simulation;
- Forge recipe/mesh/texture pipeline;
- deterministic terrain, lot, building, POI and scatter cores;
- story canon/data.

Their current translators and presentation surfaces do not inherit the same presumption.

## 5. Immediate R0 decisions already supported by evidence

1. **No broad rewrite.** Pure cores, data and generators are valuable.
2. **No continuation of the old roadmap.** Expansion remains frozen.
3. **PlayMode infrastructure is no longer optional.** The previous feasibility pass correctly avoided hiding a new CI lane inside an unrelated task; recovery now makes it an explicit reviewed R1 deliverable.
4. **Terry cannot remain the first integration test.** Quest becomes a checkpoint after automated runtime and visual evidence.
5. **Feature flags/exposure control are required.** Hidden prototypes must not appear in the golden slice merely because they exist in scenes or persistent bootstraps.
6. **Warnings are debt, not success.** The current audit has 55 warnings and zero blockers; green cannot be interpreted as player-ready.
7. **Visual provenance is not visual quality.** Forge and conformance tooling remain useful, but candidate acceptance requires integrated snapshots and device proof.

## 6. Remaining R0 repository scans

The machine inventory marks these as open:

1. Complete `RuntimeInitializeOnLoadMethod` and bootstrap census.
2. Complete `DontDestroyOnLoad` and persistent-object census.
3. All direct `SceneManager.Load*` calls and travel bypasses.
4. All input actions/button bindings and menu-open paths, including the remaining Y+B behavior.
5. All runtime-created `GameObject`, primitive, `TextMesh`, TMP Canvas, EventSystem, camera and Volume objects.
6. Event producer/consumer graph for boot, travel, repair, jobs, rewards, creature resolution, ship and first hour.
7. Save-field reader/writer ownership graph.
8. All visible fallback/prototype renderers and how they enter scenes.
9. Global `RenderSettings`, post-processing, fog, sky and lighting owners.
10. Exact build-settings/scene exposure list.
11. Exact source paths currently marked unresolved in `system_contracts.json`.
12. Reconciliation of sprint/checklist claims against proof levels.

## 7. R0 exit report format

R0 will close with:

- canonical owner table;
- duplicate-owner table;
- unowned responsibility table;
- feature exposure manifest;
- proof-level ledger;
- event/save graphs;
- proposed R1 file changes and CI job design;
- bounded migration packets for R2;
- explicit list of systems preserved, hidden, merged, replaced or deleted from runtime.

No broad runtime implementation begins before that report is reviewable.
