# GI-MSPEC-002 — CIRCUIT BRIDGE MK I + LOW-VOLTAGE RELAY TARGET

**Status:** draft measured interaction contract. Equipment body authority is established; numerical dimensions, target spacing, voltage fiction, world placement, and implementation remain unresolved. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-002
asset_id_proposed: equipment.circuit_bridge.mk1
display_name_working: Circuit Bridge
asset_class: field_equipment_and_target_pair
status: draft
world_or_ship_context: provisional wet industrial ruin
slot_or_mount_role: one belt slot / flush relay target
implementation_authority: none
```

## 1. Governing authority

- `docs/project_art_plan/PROMPT_TEST_08_FIRST_PASS_VERDICT_CIRCUIT_BRIDGE_MK1.md`
- `docs/project_art_plan/PROMPT_TEST_08_FINAL_COMPOSITE_KEEPER_VERDICT_CIRCUIT_BRIDGE_MK1.md`
- `docs/project_art_plan/PROMPT_TEST_07_COMPONENTS_AND_NURSERY_KEEPER_VERDICT_RELAY_REED.md`
- `docs/project_art_plan/CONCEPT_ART_PROMPT_PLAYBOOK_SUPPLEMENT_UTILITY_WEAPON_DRIFT.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`

### Reference authority split

- latest Circuit Bridge production sheet: body, service view, belt role, broad flat silhouette, side clamps, top lever, rear docking face, attached state;
- dedicated Conductive Vein component sheet: biological insert anatomy;
- rain-wet scenes: environment mood only;
- both handheld player-eye scenes: rejected;
- this document: target geometry, pose, clearance, interaction sequence, and failure-state planning;
- future headset proxy: final reach, alignment, release, and readability verdict.

## 2. Locked role

> The Circuit Bridge Mk I docks flush over one approved broken low-voltage relay gap, routes current through one protected biological path, remains attached without being held, and frees both hands for the actual mechanical task.

It is not a gun, taser, battery, remote hacker, universal key, projectile, grappling device, combat-disable system, or high-voltage bypass.

## 3. Equipment silhouette contract

- broad and flat;
- wider than tall;
- compact one-slot belt body;
- no long forward axis;
- no lower pistol grip;
- no trigger location;
- no muzzle, barrel, stock, sight rail, forward prongs, or beam emitter;
- one top carry/belt loop;
- one large fold-over top bridge lever;
- two short side/rear docking clamps;
- flat rear docking surface;
- guarded central biological route;
- mechanical connected/disconnected indicator.

At thumbnail size, the object should read as a repair box, patch module, or relay cover.

## 4. Coordinate and orientation contract

```yaml
forward_axis: normal from flat rear docking face toward equipment front
up_axis: top bridge lever side
primary_grab_origin: top/belt loop or broad side edge while undocked
secondary_grab_origin: opposite side edge, optional
belt_mount_origin: rear/top loop; final orientation subject to belt clearance test
socket_insertion_axis: equipment rear face toward relay target face
keyed_orientation_feature: asymmetric clamp spacing plus offset rear contact windows
resting_orientation: broad face outward on belt; flat face against relay when docked
hands_after_commit: both-free
```

## 5. Numerical measurement plan

No generated dimension is authoritative.

Before dimension lock:

1. measure the production sheet relative to the depicted hand and belt;
2. build broad-flat proxy volumes in small/medium/large candidates;
3. test belt clearance beside the other two equipment categories;
4. test one-hand retrieval and two-hand optional alignment;
5. test flush docking from standing and seated positions;
6. determine target lug spacing only after a comfortable module size exists;
7. confirm top lever can be operated without wrist collision against cabinet;
8. confirm both hands can clear the device immediately after commit.

```yaml
bounds_target_m: TBD
mass_target_kg: TBD
mass_gameplay_class: light-one-hand
maximum_required_reach_m: TBD by target placement plate
contact_spacing_m: TBD; target derives from accepted module, not generator art
clamp_travel_m: TBD
lever_arc_degrees: TBD
```

## 6. Relay target contract

The target is a paired equipment interface, not generic world decoration.

### Valid target geometry

- one broad flat cabinet or machine surface;
- two separated fixed relay lugs/rails;
- two recessed low-voltage contact windows behind/inside the lugs;
- one asymmetric spacing or key feature that prevents upside-down docking;
- enough flat perimeter for the module rear face;
- enough side clearance for both clamps;
- enough top clearance for lever movement;
- one analog gauge or mechanical response visible from player position;
- target remains local and physically reachable.

### Invalid target geometry

- exposed high-voltage bus;
- damaged/missing lug;
- flooded/unsafe cabinet where game rules prohibit use;
- unrelated power socket;
- combat target;
- remote terminal;
- surface without both seating points;
- orientation outside safe player reach.

### Target symbol

Use a simple physical matching mark based on two separated contacts joined by a broad bridge shape. It may support recognition but cannot replace keyed geometry.

## 7. Locked interaction sequence

### Stowed

- module occupies one belt slot;
- clamps retracted;
- top lever up/locked;
- biological route inactive;
- mechanical indicator shows disconnected through position, not color alone.

### Alignment

1. player grips top loop or broad side edge;
2. rear face approaches matching relay surface;
3. keyed rear windows align with target lugs;
4. ghost arrows or floating UI are not required;
5. optional subtle snap guidance begins only at close range.

### Seating

1. rear face contacts flat target surface;
2. both clamps are manually or mechanically closed around fixed lugs;
3. each side confirms independently;
4. top lever remains mechanically unavailable until both sides seat;
5. wrong/partial seating gently refuses commit.

### Commit/routing

1. player lowers large top lever;
2. lever closes a guarded internal route;
3. restrained violet-white pulse travels only inside Conductive Vein channel;
4. target gauge moves from zero to safe low reading;
5. local mechanism becomes available;
6. equipment remains attached;
7. both hands are free.

### Release

1. player returns nearby mechanism to safe state if required by gameplay owner;
2. lift bridge lever;
3. internal pulse ends;
4. analog gauge returns or indicates residual state;
5. short discharge-safe mechanical delay, if required, is shown physically;
6. clamps unlock;
7. module is removed and returned to belt.

Exact timing and power fiction require gameplay/architecture approval.

## 8. Mechanical and non-color state cues

| State | Mechanical cue | Visual/effect cue | Audio/haptic placeholder |
|---|---|---|---|
| disconnected | lever up, clamps open, indicator left | route dark | light neutral detent |
| one side seated | one clamp closed | one contact window aligned | single short tick |
| fully seated | both clamps closed, lever unlocked | route visible but inactive | two-part seating confirmation |
| routing | lever down, indicator right | one internal traveling pulse, gauge moves | contained low hum + lever haptic |
| invalid | lever remains locked | no route activity | soft refusal + spoken/caption help |
| high voltage | clamps or key refuse full seat | hazard symbol/mechanical shutter | distinct refusal, no active hum |
| safe to release | lever up, gauge safe, indicator neutral | route dark | release-ready click |

Color may supplement but never replace these states.

## 9. Conductive Vein insert contract

- graphite-brown irregular segmented biological strands;
- copper paths under smoky sheath;
- protected behind transparent or heavy-mesh window;
- visible layered biological ends only in power-off service state;
- replaceable through keyed service latch;
- not exposed during docking or routing;
- no open-air arc;
- no full-window decorative lightning storm;
- active pulse follows one clear left-to-right route.

Insert replacement is a service/maintenance interaction, not normal target use. Exact durability and depletion remain unapproved.

## 10. Belt and body-clearance contract

The bridge must be tested alongside the Moss Filter Rig and Grip Clamp category silhouettes.

Required checks:

- one belt slot only;
- no overlap with adjacent slots at neutral stance;
- top lever cannot snag while stowed;
- clamps remain retracted and guarded on belt;
- broad face does not strike thigh/hip during locomotion;
- seated posture does not bury the grab point;
- device can be returned one-handed;
- silhouette remains visibly different from Grip Clamp and Filter Rig.

## 11. Collider and attachment strategy

Proposed later-review approach:

- simple broad body collider;
- guarded lever collider/interaction handle;
- one collider per simplified clamp;
- flat rear docking plane;
- keyed target anchors for left/right seat;
- attachment resolved through deterministic snap/anchor state rather than unconstrained physics;
- physics may support approach feel but does not own final seated state;
- no mesh collider unless later evidence requires it.

## 12. Accessibility contract

- **spoken name:** `Circuit Bridge.`
- **short function:** `Connects one broken low-voltage power gap.`
- **extended function:** `Dock it flat over the matching relay gap, close both side clamps, then lower the top lever. It stays attached while you work.`
- **caption twin:** `Docked one-slot utility for a broken low-voltage relay.`
- **icon:** broad flat box spanning two separated contacts;
- **silhouette nickname:** `the power bridge box`;
- **invalid phrase:** `This bridge does not fit here.`
- **partial-seat phrase:** `Close both side clamps.`
- **unsafe phrase:** `Voltage too high. Bridge refused.`
- **success phrase:** `Bridge connected.`
- **release phrase:** `Bridge safe to remove.`

A six-year-old should be able to describe the sequence as: `Put the box over the two power parts, close it, and pull the top lever.`

## 13. Quest asset target

```yaml
asset_tier: hero-near / repeated utility
lod_count_target: exact count governed by Forge intake
material_slots_max: target 2-3 including guarded biological window
texture_resolution_targets: TBD by asset tier
alpha_usage: avoid broad layered transparency; one contained window only
shader_family: mobile-compatible opaque plus constrained transparent/mesh route
collider_strategy: primitive/compound
rig_or_animation_bones_max: none expected; lever/clamps as transforms
particle_or_vfx_needs: one restrained internal route, preferably shader/mesh-based
```

The protected route must remain readable without high-cost particles or excessive transparent overdraw.

## 14. Required measured plates before review-ready

1. front/side/top equipment dimensions;
2. belt-mounted clearance beside body and adjacent slots;
3. rear docking face and keyed windows;
4. relay target front/side dimensions;
5. left/right clamp approach and closed positions;
6. top lever arc and hand clearance;
7. standing player alignment pose;
8. seated player alignment pose;
9. fully docked hands-free state;
10. nearby second-task reach while module remains attached;
11. invalid orientation refusal;
12. high-voltage/wrong-target refusal;
13. release and safe-removal state;
14. collider/anchor plate.

## 15. Headset test questions

1. Does the stowed module read as a repair utility rather than a weapon?
2. Can it be retrieved without accidental adjacent-slot grabs?
3. Can a seated player align both clamps without leaning excessively?
4. Is the top lever reachable after flush docking?
5. Are both seating points visible or inferable during placement?
6. Does the device remain stable when the player lets go?
7. Are both hands genuinely free for the nearby task?
8. Is the internal pulse visible without excessive brightness?
9. Do clamp/lever/gauge states communicate without color?
10. Can a young player understand wrong target versus wrong orientation?
11. Is release obvious and safe-feeling?
12. Does any pose, silhouette, effect, or sound reintroduce gun/taser language?

## 16. Stop conditions

Do not mark implementation-ready if:

- target geometry is generic or undefined;
- module must be held or aimed during routing;
- any active effect crosses open air;
- forward contacts resemble a muzzle/taser;
- clamps or lever are too small for reliable VR use;
- seated/child reach fails;
- belt footprint collides with body or neighboring slots;
- state depends only on violet light;
- high-voltage/wrong-target refusal is not physically legible;
- final design requires another concept-generation pass rather than measurable correction;
- runtime or save behavior would be invented by this art/interaction spec.
