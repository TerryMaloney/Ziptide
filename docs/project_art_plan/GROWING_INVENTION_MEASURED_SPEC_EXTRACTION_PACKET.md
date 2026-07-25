# GROWING / INVENTION MEASURED-SPEC EXTRACTION PACKET

**Status:** canonical pre-build template for converting approved concept keepers into measurable, implementation-reviewable asset contracts. Docs-only and freeze-compatible. Completing a measured spec does not authorize runtime, scene, prefab, imported asset, recipe, save, or APK changes.

**Applies to:** Moss Filter, Cistern Gripvine, Relay Reed, BioRefiner Mk I, Assembler Mk I, Ore Processor Mk I, Equipment Frame Blank, Moss Filter Rig Mk I, Cistern Grip Clamp Mk I, Circuit Bridge Mk I, and future growing/invention assets.

## 1. Extraction law

A keeper image controls only the decisions explicitly assigned to it. Measured-spec extraction must:

1. cite the governing design and keeper documents;
2. list every composite-reference authority split;
3. distinguish measured facts from estimated targets;
4. resolve scale, reach, handling, sockets, moving parts, state cues, and Quest constraints;
5. identify unknowns that require Terry/headset review;
6. avoid inventing gameplay, recipe, story, or implementation behavior not already authorized;
7. preserve stable proposed IDs even when display names remain provisional;
8. fail closed when a concept cannot support safe or readable interaction.

## 2. Required header

```yaml
spec_id: GI-MSPEC-<asset>
asset_id_proposed: <stable-machine-readable-id>
display_name_working: <name>
asset_class: species | harvest_component | refined_component | machine | equipment | interaction_target | accessibility_card
status: draft | review-ready | Terry-approved | implementation-ready | superseded
world_or_ship_context: <location or provisional>
slot_or_mount_role: <none | belt-1 | station | wall-target | grow-plot | socket-category>
keeper_documents: []
source_image_records: []
controlling_reference_split: []
governing_gameplay_documents: []
implementation_authority: none | pending-freeze-lift | approved-slice
owner_lane: <art/design/architecture/runtime/accessibility>
```

## 3. Authority matrix

For each decision, name exactly one controlling authority or mark unresolved.

| Decision class | Controlling source | Status | Notes |
|---|---|---|---|
| dominant silhouette | | | |
| material/anatomy grammar | | | |
| wild scale | | | |
| ship scale | | | |
| held/belt scale | | | |
| player-eye interaction | | | |
| socket geometry | | | |
| service/open state | | | |
| active-state effect | | | |
| environment placement | | | |
| icon/symbol | | | |
| spoken name/function | | | |
| recipe/progression | | | governed outside art spec |
| Quest performance class | | | |

No later image automatically supersedes an earlier reference. Explicitly document every override.

## 4. Universal dimensions and scale

Use metres and kilograms internally. Record intended tolerances rather than false precision.

```yaml
bounds_target_m:
  width:
  height:
  depth:
scale_tolerance_percent:
mass_target_kg:
mass_gameplay_class: weightless-ui | light-one-hand | heavy-one-hand | two-hand | fixed
player_reference:
  adult_standing:
  adult_seated:
  child_approx_6:
minimum_clearance_m:
minimum_readable_distance_m:
maximum_required_reach_m:
```

Required checks:

- recognizable at thumbnail/icon size;
- recognizable at Quest world distance;
- readable at hand/face distance when held;
- no critical control outside seated-child reach target;
- no clipping through body, belt, station, or environment at expected poses;
- mass and inertia class match interaction expectations;
- no concept-art dimension is adopted without review.

## 5. Coordinate and orientation contract

```yaml
forward_axis:
up_axis:
primary_grab_origin:
secondary_grab_origin:
belt_mount_origin:
station_mount_origin:
socket_insertion_axis:
keyed_orientation_feature:
resting_orientation:
```

Include diagrams for:

- front/side/top;
- world upright;
- held neutral pose;
- belt pose;
- inserted/docked pose;
- service/open pose;
- collider envelope.

## 6. Material and visual grammar

```yaml
primary_materials: []
secondary_materials: []
biological_layers: []
wear_language: []
repair_language: []
emissive_states: []
forbidden_material_drift: []
material_instances_target:
texture_sets_target:
```

State emissives must identify:

- location;
- meaning;
- maximum brightness intent;
- whether the cue is duplicated mechanically, spatially, audibly, and haptically;
- inactive appearance;
- fault appearance.

Generated text, invented logos, and corrupted glyphs are never material authority.

## 7. Quest asset budget target

Final budgets must align with current Forge/Quest policy at intake. This template records the requested class without overriding existing caps.

```yaml
asset_tier: hero-near | repeated-near | world-mid | background | ui-physical
lod_count_target:
triangle_targets:
  lod0:
  lod1:
  lod2:
material_slots_max:
texture_resolution_targets: []
alpha_usage:
shader_family:
lightmap_or_probe_plan:
collider_strategy:
rig_or_animation_bones_max:
particle_or_vfx_needs:
audio_emitters:
instancing_requirement:
```

Required review:

- no hidden high-cost interior geometry except service states that are actually visible;
- repeated plants use variants/instancing rather than unique hero cost;
- translucent biological surfaces receive an explicit mobile rendering plan;
- emissive activity is restrained and state-driven;
- collision remains simple enough for stable VR grabbing and placement.

## 8. Species specification block

Use for Filter, Grip, Conduct, and later biological families.

```yaml
lifecycle_forms:
  seed:
  sprout:
  juvenile:
  mature_wild:
  mature_ship:
shared_anatomy_rules: []
root_or_anchor_grammar:
functional_tell:
harvest_outputs: []
harvest_attachment_points: []
harvest_regrowth_rule: unresolved
growth_clearance_m:
plot_socket_geometry:
plot_orientation_key:
wind_or_secondary_motion:
```

Required diagrams:

- lifecycle silhouettes at one common scale;
- wild versus ship cultivar;
- seed/socket keyed relation;
- harvest output attached and detached;
- reachable harvest height;
- collision/root footprint;
- one useful trait demonstration.

Do not let cultivation art override the approved compact ship scale merely because a return-state image overgrew the plant.

## 9. Component specification block

Use for raw harvests and refined biological/industrial components.

```yaml
source_species_or_machine:
raw_or_refined:
accepted_socket_category:
keyed_edge_or_contact_geometry:
handedness:
one_hand_grab:
stackability:
inventory_representation:
service_replacement_role:
damage_or_spent_state:
```

Cross-machine components must be shown:

1. at source output;
2. beside a gloved hand;
3. at destination socket;
4. fully seated;
5. rejected in wrong orientation or wrong socket where useful.

## 10. Machine specification block

```yaml
machine_role:
raw_inputs: []
outputs: []
normal_operation_sequence: []
service_sequence: []
input_surface:
output_surface:
commit_control:
cancel_return_control:
interlock_conditions: []
fault_conditions: []
power_off_service_state:
child_seated_reach_plan:
```

Required state plates:

- idle;
- loading;
- seated/ready;
- locked;
- active;
- complete;
- cancel/return;
- invalid input;
- interruption recovery;
- power-off service state.

Machine hard rules:

- input and output openings must remain distinguishable;
- dangerous mechanisms are inaccessible during normal use;
- service-open views never imply safe live operation;
- future catalyst/upgrade sockets remain covered until taught;
- machine role does not drift into another station’s responsibility;
- text is secondary to shape and physical state.

## 11. Field-equipment specification block

```yaml
belt_slots_consumed:
primary_grab_pose:
secondary_support_pose:
mount_or_dock_target:
activation_control:
release_control:
replaceable_component:
active_duration_or_condition: governed elsewhere
hands_after_commit: both-free | one-free | occupied
non_weapon_utility_check:
  dominant_axis: local | forward
  active_pose: docked | held-neutral | aimed
  effect_path: internal | contact-contained | open-air
  primary_control: lever | latch | crank | trigger-like
  child_thumbnail_read: utility | ambiguous | weapon
```

Required views:

- front/side/top;
- belt mounted;
- neutral held pose;
- service/component replacement;
- target alignment;
- fully seated/attached;
- active state;
- release state;
- invalid target response.

Automatic failure:

- gun/taser silhouette for a non-weapon utility;
- open-air beam or arc;
- trigger-like control;
- required aiming pose;
- active interaction occupies both hands when the gameplay role requires repair work;
- key state communicated only by color.

## 12. Interaction-target specification

Equipment cannot be measured correctly without its destination.

```yaml
target_id_proposed:
target_class:
valid_target_geometry:
invalid_target_geometry:
docking_lugs_or_socket:
contact_spacing:
approach_clearance:
player_stance_space:
seated_access:
mechanical_response:
analog_response:
audio_response:
haptic_response:
safety_refusal_cases: []
```

For Circuit Bridge, measured target plates are mandatory before implementation. The equipment production sheet is insufficient by itself to define cabinet lugs, contact windows, seating tolerance, release clearance, and player hand positions.

## 13. Accessibility and child-readability block

```yaml
spoken_name:
spoken_function_short:
spoken_function_extended:
caption_twin:
icon_description:
silhouette_nickname:
non_color_state_cues: []
focus_readout_delay_ms: unresolved
repeat_suppression_rule: unresolved
error_phrase:
success_phrase:
```

Every interactable or inventory item must support:

- stable short name;
- one-sentence spoken purpose;
- caption twin;
- unique silhouette;
- icon not dependent on fine detail;
- state cues that do not rely only on color;
- no unsolicited repeated narration while scrolling;
- child-readable physical placement and controls.

## 14. Audio/haptic event placeholders

Names remain provisional until the canonical audio event seam is confirmed.

```yaml
audio_events:
  focus_name:
  focus_function:
  grab:
  seat_valid:
  seat_invalid:
  commit:
  active_loop:
  complete:
  cancel_return:
  fault:
haptic_events:
  hover:
  seat:
  lock:
  active:
  complete:
  invalid:
```

No concept image authorizes an audio implementation. This block ensures later visual/mechanical states have planned twins.

## 15. Save and atomicity questions

The measured spec must identify, without solving outside its authority:

- which states need persistence;
- whether inserted components are returned on cancel;
- interruption point before/after commit;
- duplicate-output prevention;
- spent/used component handling;
- world-return state ownership;
- migration impact if IDs change.

Answers are routed to save/economy/architecture owners before implementation.

## 16. Required review checklist

A spec becomes `review-ready` only when:

- [ ] all keeper authorities are cited;
- [ ] dimensions are targets with stated tolerances;
- [ ] adult, seated, and child reach are represented;
- [ ] orientation and sockets are unambiguous;
- [ ] active, fault, cancel, and release states are shown;
- [ ] non-color accessibility cues are defined;
- [ ] Quest asset class and collider strategy are proposed;
- [ ] composite-reference splits are explicit;
- [ ] generated text/logos/dimensions are excluded;
- [ ] unknown gameplay/recipe/save questions are routed rather than invented;
- [ ] no protected recovery/runtime file has been changed;
- [ ] headset-specific questions are listed for later device review.

## 17. Initial extraction order

1. Equipment Frame Blank and Assembler square socket;
2. Moss Filter Rig and one contamination target;
3. Moss Filter seed/ship cultivar/Filter Organ;
4. BioRefiner interaction surfaces;
5. Ore Processor interaction surfaces;
6. Cistern Grip Clamp and one brace target;
7. Cistern Gripvine components/ship cultivar;
8. Relay Reed components/ship cultivar;
9. Circuit Bridge production body plus mandatory relay-target plate;
10. Assembler full machine measured spec after both accepted equipment recipes are reconciled.

This order resolves shared component contracts before attempting the largest stations.

## 18. Stop conditions

Stop extraction and return to design when:

- keeper references conflict without an authority split;
- dimensions make seated/child reach impossible;
- an equipment target does not define physical seating;
- a non-weapon utility still requires aiming or open-air effects;
- a station exposes dangerous active mechanisms;
- Quest budget cannot preserve the dominant visual grammar;
- the spec would need to invent gameplay, save, recipe, or story behavior;
- the exact headset recovery/freeze boundary would be crossed.
