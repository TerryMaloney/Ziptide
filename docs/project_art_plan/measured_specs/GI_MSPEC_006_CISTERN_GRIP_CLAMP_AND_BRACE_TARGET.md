# GI-MSPEC-006 — CISTERN GRIP CLAMP MK I + BRACE TARGET

**Status:** draft measured equipment/target contract. Equipment role, one-slot silhouette, biological compression behavior, tension lever, hands-free brace result, and wet-surface context are established. Numerical dimensions, allowable load, jaw travel, force, surface rules, target placement, recipe IDs, and implementation remain unresolved. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-006
asset_id_proposed: equipment.cistern_grip_clamp.mk1
display_name_working: Grip Clamp
asset_class: field_equipment_and_target_pair
status: draft
world_or_ship_context: Dry Cistern concept context; exact progression authority pending
slot_or_mount_role: one belt slot / local brace target
implementation_authority: none
```

## 1. Governing authority

- `docs/project_art_plan/PROMPT_TEST_02_CISTERN_GRIP_FAMILY_FINAL_KEEPERS.md`
- `docs/project_art_plan/PROMPT_TEST_06_FINAL_KEEPER_VERDICT_CISTERN_GRIP_CLAMP_MK1.md`
- `docs/project_art_plan/measured_specs/GI_MSPEC_001_EQUIPMENT_FRAME_AND_ASSEMBLER_SOCKET.md`
- `docs/project_art_plan/GROWING_INVENTION_CONCEPT_KEEPER_INDEX.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`

### Reference authority split

- production/equipment sheet: body, biological pad, belt loop, keyed jaw, tension lever, service/pad-replacement view;
- Dry Cistern player-eye scene: wet-surface bracing moment and free-hand utility;
- dedicated Grip-family component sheet: Grip Surface Insert anatomy;
- chain schematic: logic only;
- equipment card: icon and before/after grammar only;
- exact dimensions, force, target eligibility, recipe, IDs, world placement, collider, and persistence: unresolved;
- this document: target pair, clearances, load/failure model, pose, reach, and validation gates.

## 2. Locked role

> The Cistern Grip Clamp Mk I temporarily braces one small repair component, cable coupling, tool, pipe bracket, or loose industrial panel against an approved wet stone or metal surface so the player can use the other hand for the actual task.

It is not:

- a grappling or traversal device;
- a remote tether;
- a weapon or combat restraint;
- a permanent building system;
- a universal physics-freeze tool;
- a heavy structural support for large machinery;
- a substitute for authored target geometry.

## 3. Equipment silhouette and component grammar

- compact rectangular one-slot salvage-metal body;
- one dominant round refined Grip Surface Insert;
- pale microhook contact face;
- dark olive fibrous biological rim;
- restrained amber compression nodes only under load;
- one large tension lever;
- one keyed mounting jaw/support geometry;
- one obvious mechanical locked/unlocked indicator;
- one belt loop;
- one service latch for biological insert replacement;
- no barrel, trigger, long axis, launch mechanism, hook projectile, or weapon pose.

At thumbnail size, it should read as `the clamp box with the round grip pad`.

## 4. Coordinate and handling contract

```yaml
forward_axis: outward from biological pad face
up_axis: belt-loop / indicator side
primary_grab_origin: body side opposite tension lever
secondary_grab_origin: lever or rear frame only during placement, optional
belt_mount_origin: rear/top loop; final position by three-slot belt clearance test
brace_contact_axis: pad normal toward approved support surface
jaw_alignment_axis: parallel to loose component edge or target bracket
service_open_axis: pad cradle opens away from active lever
resting_orientation: pad face outward or slightly downward on belt
mass_gameplay_class: light-one-hand
hands_after_commit: one-free minimum; two-free where target geometry supports it
```

The equipment must not require the player to hold the full device under load after lock.

## 5. Numerical measurement plan

No generated dimension or apparent force is authoritative.

Required proxy study:

1. measure keeper relative to hand and belt;
2. create small/medium/large body proxies;
3. test retrieval from all belt slots without lever/body collision;
4. test pad face visibility during alignment;
5. test jaw alignment on vertical, angled, and limited horizontal approved targets;
6. determine minimum pad size that visibly deforms without making device oversized;
7. test lever reach and arc while device is against a wall/column;
8. establish a low real-world lever force with strong haptic/audio/load animation;
9. determine jaw travel and object-thickness bands only after target prototypes exist;
10. test release under no-load and loaded-fiction states without snapback;
11. verify standing, seated, and young-child approach and hand clearance.

```yaml
bounds_target_m: TBD
mass_target_kg: TBD
pad_diameter_m: TBD
pad_compression_visual_range_m: TBD
jaw_travel_m: TBD
target_object_thickness_range_m: TBD
lever_force_n: TBD
lever_arc_degrees: TBD
maximum_required_reach_m: TBD
minimum_hand_clearance_m: TBD
fictional_load_class: small-repair-component only
```

## 6. Brace target pair contract

The target must be authored as a paired **loose component + stable support surface** interaction.

### Stable support surface requirements

- wet stone, painted/rusted industrial metal, or other approved high-friction fiction surface;
- broad enough for the biological pad;
- target normal and approach angle reachable;
- no active hazard at hand position;
- clear visual indication that the surface can receive temporary bracing through geometry/material context, not quest glow;
- no reliance on arbitrary placement anywhere in the world.

### Loose component requirements

- small enough for the clamp's declared load class;
- one readable edge, bracket, jaw point, or keyed support location;
- visibly slips, swings, falls, or will not stay aligned before bracing;
- becomes stable after lock;
- leaves one meaningful follow-up action for the player's free hand;
- returns to authored final state after repair rather than relying permanently on the clamp.

### Valid examples

- loose maintenance plate aligned over a pump opening;
- pipe brace held against wet column while coupling is seated;
- cable junction housing held while connector is turned;
- small salvage component stabilized for scanning/removal;
- temporary switch linkage held in position while another control is used.

### Invalid examples

- player locomotion anchor;
- climbing handhold;
- enemy restraint;
- large beam, door, bridge, or structural machinery;
- arbitrary loose prop with no authored follow-up;
- target requiring the player to stand over a drop with no safe position;
- hot/electrified/sharp surface not designed for hand-safe access;
- permanent construction placement.

## 7. Locked interaction sequence

### Stowed

- one belt slot;
- lever open/unloaded;
- jaw retracted or neutral;
- pad inactive;
- mechanical indicator unlocked.

### Approach and alignment

1. player identifies loose component and support surface;
2. one safe observation position shows the slipping/misalignment problem;
3. player retrieves clamp by body grip;
4. pad faces stable support surface;
5. keyed jaw aligns with loose component edge/bracket;
6. optional close-range snap guidance begins only near valid geometry;
7. no distant beam, line, or floating arrow is required.

### Seating

1. pad contacts support surface;
2. jaw contacts component target;
3. both contact conditions confirm independently;
4. device holds neutral but not locked;
5. lever becomes available only when geometry is valid.

### Commit / loaded

1. player pulls large tension lever;
2. jaw and body close/load visibly;
3. biological pad flattens against support;
4. amber compression nodes activate locally;
5. mechanical indicator changes position;
6. short haptic/audio cue confirms load;
7. device remains attached;
8. at least one hand becomes free for the real task.

### Release

1. follow-up repair/action reaches safe state;
2. player operates release/unload control;
3. amber nodes fade after mechanical load releases;
4. lever returns without violent snapback;
5. jaw opens;
6. device can be removed and returned to belt;
7. authored repaired component remains in its final state where appropriate.

## 8. Invalid and overload behavior

- invalid surface: device cannot seat; no amber load cue;
- missing component jaw point: lever unavailable;
- excessive object size/load: mechanical refusal before commit;
- movement after seating but before commit: gentle disengage/return;
- interruption after commit: device and target restore a deterministic safe state;
- no damage or resource consumption from invalid attempt;
- no physics explosion, launched prop, or player knockback;
- optional spoken/caption: `That part is too heavy for this clamp.`
- target eligibility must be authored/data-driven and auditable.

## 9. Biological pad service contract

Whether pad replacement is playable in Mk I remains unresolved.

If playable:

- service latch is large and reachable;
- tension state must be fully unloaded before opening;
- insert has keyed round cradle and cannot seat backward;
- dedicated Grip Surface Insert keeper controls anatomy;
- removal/insertion uses simple gross hand motion;
- invalid insert is refused without trapping;
- owned insert persists through save/travel.

If not playable in Mk I, the service view remains build/decomposition reference only.

## 10. Accessibility contract

Working language:

- spoken name: `Grip Clamp.`
- short function: `Holds loose parts while you repair them.`
- extended help: `Place the round pad on a stable surface and the jaw on the loose part. Pull the lever to hold it.`
- invalid surface: `This clamp needs a stable wet stone or metal surface.`
- overload: `That part is too heavy for this clamp.`
- complete seating: `Clamp ready.`
- locked: `Part secured.`

Icon grammar:

- equipment icon: round pad plus lever holding two surfaces together;
- before/after card: slipping rectangle → same rectangle held flush;
- valid load uses lever, jaw, pad deformation, haptic, sound, and amber nodes;
- no meaning depends on amber color alone.

## 11. Comfort and safety

- no required high real force;
- no rapid repeated lever pumping;
- no pinch point between hand and target;
- no required wrist rotation against a wall beyond comfortable range;
- no forced reach above shoulder or below unsafe crouch height;
- target placement supports seated alternative;
- pad deformation and load sound sell force;
- release cannot snap device into player;
- invalid target does not grab unexpectedly;
- target near deep shaft requires stable standing/seated platform and guardrail-safe reach.

## 12. Quest asset/performance questions

Resolve:

- equipment mesh/LOD/material targets;
- pad deformation implementation;
- amber node state method;
- lever/jaw animation parts;
- collider count and contact geometry;
- target-pair authoring component/data;
- physics versus authored pose responsibility;
- haptic/audio budget;
- belt attachment and occlusion;
- wet-surface material readability without expensive effects;
- reduced-motion and reduced-flash behavior.

## 13. Save and atomicity questions

Architecture must define:

- clamp ownership and belt state persistence;
- attached-device persistence across save/quit/travel;
- safe recovery if target unloads while clamp is attached;
- whether clamp auto-returns to Lost & Found/travel recovery under owned-item law;
- target final-state persistence after repair;
- no item duplication through attach/detach loops;
- no prop launch or broken world state after interruption.

## 14. Measured target plate requirements

Before implementation approval, create at least two plates:

### Plate A — straightforward vertical panel

- standing and seated approach;
- pad area, jaw point, lever clearance;
- loose and braced states;
- free-hand repair location;
- release path.

### Plate B — wet pipe/column brace

- angled component and curved support approximation;
- guardrail/platform safety;
- hand clearance;
- reach band;
- invalid/overload example.

## 15. Terry/headset questions

- Does it read as a clamp rather than a weapon or grappling device?
- Can the pad and jaw be aligned without awkward wrist angles?
- Does lever motion feel deliberate but not strenuous?
- Is the free-hand payoff immediately obvious?
- Is pad deformation visible from player eye position?
- Can a young player distinguish stable support from invalid scenery?
- Does release feel safe and predictable?
- Is one-slot carrying cost justified by useful world opportunities?

## 16. Implementation gate

Status remains `draft` until binary/provenance archive, proxy dimensions, two target plates, reach/force tests, target data ownership, save/recovery law, Quest budgets, exact progression approval, M0/freeze clearance, lane assignment, and device-verdict plan are complete.
