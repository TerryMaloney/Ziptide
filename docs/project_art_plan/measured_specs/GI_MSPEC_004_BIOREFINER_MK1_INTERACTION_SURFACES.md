# GI-MSPEC-004 — BIOREFINER MK I INTERACTION SURFACES

**Status:** draft measured machine contract. Role, safe loading grammar, dominant controls, cassette model, and output separation are established. Numerical dimensions, cassette inventory, processing timings, yields, persistence, exact ship placement, and implementation remain unresolved. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-004
asset_id_proposed: machine.bio_refiner.mk1
display_name_working: BioRefiner
asset_class: machine
status: draft
world_or_ship_context: ship production area
slot_or_mount_role: fixed waist-height station
implementation_authority: none
```

## 1. Governing authority

- `docs/project_art_plan/PROMPT_TEST_03_BIOREFINER_MK1_VERDICT.md`
- `docs/project_art_plan/PROMPT_TEST_03_BIOREFINER_MK1_FINAL_PASS_VERDICT.md`
- `docs/project_art_plan/GROWING_INVENTION_CONCEPT_KEEPER_INDEX.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`
- growing/invention reconciliation and future recipe/data authorities

### Reference authority split

- installed interaction keeper: player-eye layout, removable external tray, guarded chamber, dominant lever, cancel/return control, output tray, gauges, work light;
- production/body sheet: cabinet silhouette, maintenance side, hoses, patches, service decomposition, adult/seated scale;
- input/output sheet: transformation logic only;
- dedicated family/component keepers: actual raw and refined biological anatomy;
- generated hopper-loading, branding, labels, readings, machine text, and mixed-cycle outputs: non-authoritative;
- this document: measurable interaction surfaces, category grammar, state machine, clearances, and review gates.

## 2. Locked role

> The BioRefiner accepts one raw biological output in a removable keyed tray, locks it inside a guarded chamber, stabilizes it through one installed process cassette, and returns one durable recipe-ready biological material through a separate output tray.

It does not:

- assemble finished equipment;
- process ore or salvage metal;
- accept multiple simultaneous biological items in Mk I;
- expose rollers, heat surfaces, gears, or tools during normal play;
- consume an item before the player deliberately commits;
- silently choose an unrelated process;
- function as a conveyor or factory router.

## 3. Machine silhouette and zone layout

The machine is a compact, worn, fixed salvage cabinet with five visually distinct zones:

1. **external removable input tray** — asymmetrical, keyed, one raw biological item;
2. **guarded processing chamber** — transparent or mesh-protected observation, no hand access;
3. **cassette bay/identifier** — one installed process family visible mechanically;
4. **dominant control cluster** — one process lever plus one cancel/return control;
5. **separate output tray** — lower and differently shaped from input.

Secondary gauges and maintenance surfaces must not compete with the five primary zones.

## 4. Coordinate and installation contract

```yaml
forward_axis: outward from player-facing chamber/control face
up_axis: work-lamp/chamber-top direction
station_mount_origin: fixed floor/deck or reinforced bench base
input_tray_insertion_axis: toward guarded chamber
output_tray_motion_axis: outward/downward from separate lower bay
process_lever_primary_axis: large deliberate arc, direction TBD by reach plate
cancel_control_location: separated from commit lever, reachable before commit
service_access_side: side/rear, outside normal player interaction volume
mass_gameplay_class: fixed
```

Machine placement must preserve:

- unobstructed approach for standing and seated players;
- room to remove, load, and reinsert the input tray;
- no wall or adjacent machine collision with lever arc;
- clear view of latch, chamber, cassette state, gauges, and output;
- service panels outside the normal hand path;
- no need to lean over an open hazard.

## 5. Numerical measurement plan

Do not adopt generator dimensions.

Required proxy study:

1. choose standing/seated shared work-surface height range;
2. measure child hand and seated adult reach to tray handle, latch, lever, cancel, and output;
3. test three tray widths against Filter Organ, Grip Pad, Grip Fiber bundle, Conductive Vein bundle, and future family outputs without becoming a universal bin;
4. determine maximum single input bounds and reject/eject behavior;
5. test tray travel and chamber door/latch without pinch zones;
6. test process-lever force and arc using a low physical-motion proxy;
7. confirm output tray can be reached without crossing the input or active chamber;
8. verify machine footprint leaves a clear ship circulation path.

```yaml
machine_bounds_m: TBD
player_work_surface_height_m: TBD
input_tray_bounds_m: TBD
input_tray_travel_m: TBD
output_tray_bounds_m: TBD
output_tray_travel_m: TBD
latch_force_n: TBD
process_lever_force_n: TBD
process_lever_arc_degrees: TBD
maximum_required_reach_m: TBD
minimum_front_clearance_m: TBD
minimum_side_clearance_m: TBD
```

## 6. Input tray contract

- removable and visibly asymmetrical;
- accepts exactly one raw biological output in Mk I;
- family-agnostic enough for approved raw biology but not visually identical to Ore Processor tray;
- soft/organic support geometry avoids implying crushing before commit;
- one coarse keyed edge prevents wrong orientation;
- large handle usable with one hand;
- tray can be loaded completely outside the machine;
- item remains retrievable until commit;
- tray cannot fully enter if item exceeds allowed envelope;
- valid seating uses physical depth, detent, haptic and audio—not light alone;
- invalid content states the required category without destroying or trapping the item.

Working invalid phrase: `This machine needs a raw biological material.`

## 7. Guarded chamber and interlock contract

Normal operation requires:

1. input tray fully seated;
2. chamber guard/door closed;
3. heavy mechanical latch locked;
4. one compatible cassette installed;
5. no service panel open;
6. process lever physically unavailable until all conditions pass.

During processing:

- no hand collider can reach moving/heat areas;
- visible motion remains behind guard;
- transparent material must not cause unacceptable Quest overdraw;
- one raw item maps to one declared transformation result;
- process resolves atomically after commit;
- interruption cannot duplicate or delete materials.

Service-open state:

- power mechanically disconnected;
- rollers/tools visible only here;
- process lever locked out;
- distinct physical service posture prevents confusion with normal loading;
- service state is not required for ordinary player crafting in Mk I unless later explicitly approved.

## 8. Cassette contract

The cassette system explains why one machine can stabilize different material families without becoming magical.

Each cassette requires:

- one large silhouette/icon readable without labels;
- keyed physical bay preventing incompatible orientation;
- installed state visible before loading/commit;
- mechanical identification in addition to color;
- stable process family, not a hidden random outcome;
- no cassette swap after commit;
- exact cassette inventory and acquisition governed by progression data, not this art spec.

Current concept examples—not recipe canon:

- membrane stabilization;
- grip-surface curing;
- tendon stabilization;
- conductive-vein stabilization.

## 9. Output tray contract

- separate lower opening from input;
- different handle and silhouette from input tray;
- remains closed/empty until cycle completes;
- presents one stable refined component;
- output anatomy comes from dedicated refined-component keeper/spec;
- no mixed raw and refined material in one tray;
- player can retrieve with one hand;
- completion state uses drawer motion, latch release, analog gauge, sound and restrained light;
- if inventory is full or player leaves, owned output persists under architecture/save law rather than disappearing.

## 10. Machine state model

### Idle

- chamber open or unlocked;
- input tray removable;
- output tray empty/closed;
- process lever unavailable;
- cassette identity visible.

### Input seated

- tray at final depth;
- item category confirmed;
- guard can close;
- cancel/return remains available.

### Locked / ready

- guard and latch positions visibly changed;
- lever becomes available;
- analog ready indicator moves;
- optional spoken/caption: `Ready to refine.`

### Working

- lever remains committed;
- guarded internal motion;
- analog gauge movement;
- restrained family-appropriate effect;
- cancel no longer destroys or duplicates state; interruption policy handled atomically.

### Complete

- tools stop;
- guard remains safe;
- output drawer releases;
- one completion haptic/audio cue;
- optional spoken/caption: `Refined material ready.`

### Invalid / fault

- lever remains unavailable;
- item returns or stays retrievable;
- mechanical fault position plus non-color cue;
- no blame language;
- exact fault cause available through deliberate help.

## 11. Accessibility contract

Working language:

- spoken name: `BioRefiner.`
- short function: `Makes plant materials ready for inventions.`
- empty input cue: `Place one raw biological material in the tray.`
- wrong category: `This tray needs a raw biological material.`
- cassette mismatch: `This process does not match that material.`
- pre-commit cancel: `Material returned.`
- complete: `Refined material ready.`

Icon grammar:

- machine icon: guarded chamber with organic input entering and stable round output leaving;
- input tray icon: rounded organic form;
- cassette icon: large family-specific process shape;
- output tray icon: stable round/family-keyed insert;
- state cues duplicated through tray/latch/lever/drawer position.

Narration must use dwell/replay rules and avoid reading every gauge or incidental label.

## 12. Quest performance and asset questions

Resolve before implementation-ready status:

- cabinet mesh/LOD and material-slot targets;
- number of animated parts;
- guard transparency strategy and fallback;
- hoses static versus skinned;
- gauge implementation and batching;
- cassette variant count;
- raw/refined component visibility and culling;
- particle/emissive/audio budgets;
- collider count and pinch-zone simplification;
- whether service-open internals are a separate mesh/state or non-playable reference only;
- distance at which small controls simplify.

## 13. Save, interruption, and ownership questions

Architecture review must define:

- transaction record before commit;
- atomic input consumption/output creation;
- recovery after app suspend, overlay, quit, travel, or crash;
- output persistence if left in tray;
- cancel behavior before commit;
- cassette ownership/configuration persistence;
- no duplication through repeated lever interaction;
- no owned material loss through travel/quit;
- relationship to future automation without changing Mk I manual interaction.

## 14. BioRefiner back-half pilot gate

The handoff recommends the BioRefiner as the first concept-to-built pipeline pilot because `MachineType.BioRefiner` reportedly already exists and the cabinet is Forge-friendly. This spec does **not** activate that pilot.

The pilot begins only after:

1. M0 headset proof passes and relevant freezes lift;
2. keeper binaries and provenance are archived;
3. this spec receives Terry and architecture review;
4. proxy scale/reach plate passes;
5. a machine-readable keeper sidecar exists;
6. one representative raw/refined pair is selected;
7. Forge recipe/asset lane is assigned;
8. built-vs-keeper comparison method is prepared;
9. headset interaction verdict is part of the definition of done.

## 15. Terry/headset questions

- Is the input tray obviously different from the output tray?
- Can a young or seated player load, latch, commit, cancel and collect without leaning into the machine?
- Does the machine feel biological-processing rather than ore-crushing or equipment-assembling?
- Are state changes understandable with generated labels removed?
- Is the dominant lever deliberate without requiring excessive force or reach?
- Is the guarded process satisfying to watch without becoming visually noisy?
- Can the selected raw item and resulting refined item be traced as one clear cycle?

## 16. Implementation gate

Status remains `draft` until archive, scale proxies, interaction plate, cassette/category choice, atomicity design, Quest budgets, M0/freeze clearance, owner assignment, Forge intake plan, and device-verdict plan are complete.
