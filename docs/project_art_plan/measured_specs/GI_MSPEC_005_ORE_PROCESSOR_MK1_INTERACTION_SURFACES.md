# GI-MSPEC-005 — ORE PROCESSOR MK I INTERACTION SURFACES

**Status:** draft measured machine contract. Heavy-machine identity, removable ore tray, guarded compression chamber, crossbar, process handle, cassette model, separate output drawer, and industrial-output categories are established. Numerical dimensions, force values, output yields, exact cassettes, ship placement, persistence, and implementation remain unresolved. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-005
asset_id_proposed: machine.ore_processor.mk1
display_name_working: Ore Processor
asset_class: machine
status: draft
world_or_ship_context: ship production area
slot_or_mount_role: fixed floor-mounted station with waist-height controls
implementation_authority: none
```

## 1. Governing authority

- `docs/project_art_plan/PROMPT_TEST_05_FINAL_KEEPER_VERDICT_ORE_PROCESSOR_MK1.md`
- `docs/project_art_plan/measured_specs/GI_MSPEC_001_EQUIPMENT_FRAME_AND_ASSEMBLER_SOCKET.md`
- `docs/project_art_plan/GROWING_INVENTION_CONCEPT_KEEPER_INDEX.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`
- growing/invention and future mining/economy authorities

### Reference authority split

- production/body sheet: heavy low silhouette, structural ribs, guarded chamber, crossbar, process handle, service decomposition;
- installed ship-context sheet: player approach, reachable control placement, surrounding industrial atmosphere;
- output vocabulary sheet: category silhouettes only;
- Equipment Frame Blank sheet: first cross-machine industrial component authority;
- exact raw ores, cassettes, labels, dimensions, yields, recipes, and machine branding: unresolved/non-authoritative;
- this document: interaction surfaces, safety, state model, category contracts, proxy questions, and gates.

## 2. Locked role

> The Ore Processor accepts one raw mineral chunk or salvage-metal input in a removable keyed tray, locks it inside a fully guarded heavy-processing chamber with one installed material cassette, and returns one standardized recipe-ready industrial component through a separate lower output drawer.

It does not:

- refine biological materials;
- assemble final equipment;
- expose crushing, cutting, heat, or compression mechanisms in normal play;
- output finished weapons or tools;
- accept conveyor feed in Mk I;
- become a magical universal fabricator;
- use the same opening for input and output.

## 3. Dominant machine and interaction zones

The Ore Processor must remain distinguishable from BioRefiner and Assembler at thumbnail distance.

Primary zones:

1. **large asymmetric removable ore tray** — raw irregular material;
2. **opaque/guarded heavy chamber** — force application hidden from hands;
3. **mechanical crossbar** — visible lock across chamber;
4. **high-resistance process handle** — deliberate force verb;
5. **cassette identifier/bay** — process family;
6. **separate lower output drawer** — standardized industrial stock;
7. **analog pressure/state gauges** — force response.

The machine should read as heavy compression and standardization through structure and operation—not through exposed danger.

## 4. Coordinate and installation contract

```yaml
forward_axis: outward from tray/chamber/control face
up_axis: chamber-top / gauge direction
station_mount_origin: fixed reinforced deck/floor base
input_tray_insertion_axis: into guarded chamber
crossbar_motion_axis: lateral or vertical across chamber face; final choice by reach plate
process_handle_primary_axis: large deliberate pull/push arc, final direction TBD
output_drawer_motion_axis: outward from distinct lower bay
service_access_side: rear/side, outside normal interaction volume
mass_gameplay_class: fixed
```

Installation must provide:

- heavy floor footprint without blocking ship circulation;
- all player controls at reachable waist height;
- standing and seated approach;
- clear two-hand option for raw heavy-looking objects without requiring actual high physical force;
- no overhead reach for tray, crossbar, handle, cassette, or drawer;
- enough clearance for full crossbar and handle travel;
- output retrieval without reaching across the chamber.

## 5. Numerical measurement plan

No generated measurements are authoritative.

Proxy sequence:

1. establish common ship work-surface/reach bands with BioRefiner and Assembler;
2. build small/medium/large machine footprint proxies preserving heavy silhouette;
3. test tray size against approved raw-input envelope candidates without making it a general inventory bin;
4. test tray loading with one and two hands from standing and seated positions;
5. test crossbar motion with minimal shoulder elevation;
6. test a high-resistance *fiction* using controlled haptic/audio and moderate physical handle force rather than real strenuous motion;
7. determine safe handle arc and return behavior;
8. test lower output drawer reach for seated child proxy;
9. verify clear distinction between input tray, cassette bay, and output drawer;
10. align Equipment Frame Blank output scale with GI-MSPEC-001.

```yaml
machine_bounds_m: TBD
input_tray_bounds_m: TBD
input_tray_travel_m: TBD
raw_input_bounds_max_m: TBD
crossbar_travel_m: TBD
crossbar_force_n: TBD
process_handle_force_n: TBD
process_handle_arc_degrees: TBD
output_drawer_bounds_m: TBD
output_drawer_travel_m: TBD
maximum_required_reach_m: TBD
minimum_front_clearance_m: TBD
```

## 6. Raw input tray contract

- large asymmetrical keyed tray;
- supports one raw mineral or salvage-metal input;
- visually and physically different from BioRefiner organic tray;
- coarse supports and wear plates communicate heavy material;
- input can be placed while tray is fully outside chamber;
- no hand enters chamber during loading;
- large handle and edge geometry support one- or two-hand placement;
- tray cannot fully seat if item exceeds envelope or category is invalid;
- item remains retrievable until commit;
- invalid items are refused/ejected gently and never destroyed;
- no small loose fragments required in Mk I unless a later recipe authority explicitly permits them.

Working invalid phrase: `This machine needs raw ore or salvage metal.`

## 7. Chamber, crossbar, and safety interlock

Commit requires:

1. valid raw input;
2. tray fully seated;
3. chamber fully guarded/closed;
4. crossbar locked at final mechanical position;
5. compatible cassette installed;
6. service access closed;
7. process handle unlocked only after all checks pass.

During processing:

- all moving/force/heat surfaces inaccessible;
- chamber remains opaque or safely guarded;
- pressure communicated with gauge, structure sound, restrained amber working state, and mechanical movement;
- no exposed molten material;
- no violent camera shake;
- force fantasy does not require dangerous real-world exertion;
- process resolves atomically.

Service-open state:

- power mechanically disconnected;
- internal wear plates/press structures may be visible;
- no active handle or crossbar operation;
- visually distinct from normal play;
- optional/deferred unless implementation requires service gameplay.

## 8. Cassette and output vocabulary contract

The cassette explains one core machine producing different standardized stock.

Current output categories are vocabulary, not all early-game unlocks:

- Structural Plate Blank;
- Equipment Frame Blank;
- Conductive Strip;
- Dense Anchor Bar.

Every cassette/output pair requires:

- stable category silhouette;
- physical cassette identifier readable without text;
- output that fits one downstream contract;
- no random output;
- no finished equipment;
- dedicated output definition before implementation;
- progression authority determining which are available.

W000/W001 should expose only the minimum industrial output required by the approved first invention.

## 9. Output drawer contract

- distinct lower location and rectangular/industrial silhouette;
- cannot be confused with input tray;
- locked/closed during working state;
- presents exactly one standardized output after completion;
- Equipment Frame Blank output must match GI-MSPEC-001 keyed geometry;
- drawer motion plus mechanical latch, gauge release, sound, and restrained light confirm completion;
- output remains owned/persistent if left behind;
- drawer cannot create duplicates through repeated opening;
- no hot/exposed surface at player contact.

## 10. State model

### Idle

- input tray removable;
- crossbar open;
- handle locked/unavailable;
- output drawer closed/empty;
- cassette identity visible.

### Loaded

- valid raw item seated;
- tray can slide inward;
- cancel/return available.

### Locked / ready

- tray fully inside;
- crossbar visibly closed;
- gauge and mechanical indicator move;
- handle becomes available;
- optional spoken/caption: `Ready to process.`

### Working

- handle committed;
- guarded force cycle;
- analog pressure movement;
- restrained amber state;
- no player access to moving parts;
- interruption resolves through atomic transaction rule.

### Complete

- pressure returns to safe state;
- crossbar/handle reset sequence becomes available;
- output drawer releases;
- one standardized component present;
- optional spoken/caption: `Industrial material ready.`

### Invalid / fault

- handle remains locked;
- tray/item remains retrievable;
- mechanical refusal and short haptic/audio cue;
- help states category or alignment needed;
- no destructive test cycle.

## 11. Accessibility contract

Working language:

- spoken name: `Ore Processor.`
- short function: `Makes ore and salvage ready for inventions.`
- empty input: `Place one raw ore or salvage piece in the tray.`
- wrong category: `This machine needs raw ore or salvage metal.`
- lock reminder: `Close the crossbar before processing.`
- complete: `Industrial material ready.`

Icon/shape grammar:

- machine icon: irregular chunk entering guarded press, square stock leaving;
- input tray: irregular-chunk symbol;
- crossbar: unmistakable lock line across chamber;
- output drawer: square/open industrial stock symbol;
- force state duplicated through handle/crossbar/gauge position, haptics, and sound;
- amber light is secondary.

## 12. Comfort and physical-effort law

The machine may *look* heavy without demanding unsafe force.

- process handle uses moderate real force and large travel readability;
- haptic resistance, mechanical sound, gauge lag, and animation sell effort;
- no repeated rapid pumping;
- no overhead pull;
- seated alternative uses the same reachable handle;
- optional two-hand grip cannot be mandatory where child reach would fail;
- no required crouch beneath the machine;
- no violent rebound when handle releases.

## 13. Quest performance and asset questions

Resolve:

- body mesh/LOD/material targets;
- hydraulic hose treatment;
- gauge count and animation method;
- chamber guard material;
- cassette variant strategy;
- crossbar, handle, tray, and drawer animation parts;
- collider count and safe envelopes;
- dust/particle/amber state budget;
- audio and haptic budget;
- service interior separate mesh/state;
- occlusion and footprint in ship room;
- raw/output prop LOD and batching.

## 14. Save and atomicity questions

Architecture must define:

- pre-commit transaction state;
- atomic raw-input consumption and output creation;
- overlay/suspend/quit/travel recovery;
- output persistence in drawer;
- cassette configuration persistence;
- no duplicate cycles through handle spam;
- no lost owned materials;
- future automation adapter boundary without bypassing Mk I manual contract.

## 15. Terry/headset questions

- Does it read as a heavy industrial processor without looking unsafe?
- Are input, crossbar, process handle, cassette, and output obvious without labels?
- Can a seated adult and young player reach all controls comfortably?
- Does the handle feel deliberate without real strain?
- Is the Equipment Frame Blank recognizable when it exits?
- Is the machine clearly different from BioRefiner and Assembler?
- Is the process satisfying without exposed crushing or excessive effects?

## 16. Implementation gate

Status remains `draft` until keeper/provenance archive, proxy dimensions, effort/reach tests, one cassette/output choice, GI-MSPEC-001 alignment, atomicity design, Quest budgets, M0/freeze clearance, owner assignment, and device-verdict plan are complete.
