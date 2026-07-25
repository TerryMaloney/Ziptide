# GI-MSPEC-001 — EQUIPMENT FRAME BLANK + ASSEMBLER SQUARE SOCKET

**Status:** draft measured-contract extraction. Geometry, interaction, and authority are defined; numerical dimensions remain intentionally unresolved pending reference-plate measurement and headset proxy testing. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-001
asset_id_proposed: component.equipment_frame_blank.mk1
display_name_working: Equipment Frame
asset_class: refined_industrial_component
status: draft
world_or_ship_context: ship production chain
slot_or_mount_role: assembler-square-industrial-socket
implementation_authority: none
```

## 1. Governing authority

- `docs/project_art_plan/PROMPT_TEST_05_FINAL_KEEPER_VERDICT_ORE_PROCESSOR_MK1.md`
- `docs/project_art_plan/PROMPT_TEST_04_FINAL_KEEPER_VERDICT_ASSEMBLER_MK1.md`
- `docs/project_art_plan/GROWING_INVENTION_CONCEPT_KEEPER_INDEX.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`

**Controlling image class:** dedicated Equipment Frame Blank prop/integration sheet showing source drawer, gloved hand, keyed edge, and Assembler seating.

**Non-authoritative:** generated inch labels, exact thickness, mass, text, stamps, and decorative markings.

## 2. Locked role

> The Equipment Frame Blank is the reusable asymmetrical square industrial body accepted by the Assembler Mk I. It supplies structure, mounting channels, and keyed orientation but no biological behavior.

It is not a finished tool, weapon body, generic plate, inventory bag, or biological component.

## 3. Locked dominant geometry

- open square/rectangular frame rather than solid plate;
- one visibly asymmetric keyed edge;
- keyed edge includes a coarse tooth/channel pattern readable by touch and silhouette;
- four reinforced corner regions;
- two long mounting channels on opposite rails;
- repairable visible fasteners;
- no round dominant silhouette;
- no biological material;
- one orientation clearly reads as `keyed edge toward socket key`;
- frame must remain distinguishable from Structural Plate Blank at thumbnail size.

## 4. Orientation contract

```yaml
forward_axis: normal to open frame plane, toward player while held
up_axis: keyed-edge orientation determined by Assembler plate
primary_grab_origin: one non-keyed side rail
secondary_grab_origin: opposite side rail, optional
socket_insertion_axis: down/forward into waist-height cradle, finalized by machine layout
keyed_orientation_feature: coarse toothed edge plus one offset/chamfered corner
resting_orientation: flat in output drawer and Assembler cradle
```

The keyed edge alone must prevent a visually plausible 180-degree mis-insertion. If the final machine socket remains rotationally ambiguous, add one offset corner or rail stop rather than relying on labels.

## 5. Numerical measurement plan

Do not adopt generator dimensions.

Required before dimension lock:

1. measure the accepted prop sheet against the depicted gloved hand using a consistent reference method;
2. compare with the Assembler player-eye socket image;
3. create three simple proxy frames covering small, medium, and large one-hand handling candidates;
4. test standing adult, seated adult, and young-child reach/grip on headset;
5. choose the smallest size that preserves the keyed silhouette, comfortable one-hand handling, and readable socket match;
6. record final width, height, depth, rail thickness, key-tooth pitch, insertion depth, and tolerance.

```yaml
bounds_target_m: TBD
mass_target_kg: TBD
mass_gameplay_class: light-one-hand
scale_tolerance_percent: TBD after proxy test
maximum_required_reach_m: inherited from Assembler station spec
```

## 6. Assembler socket contract

### Socket category

- always the left industrial input in the approved Assembler grammar;
- unmistakably asymmetrical square/open recess;
- cannot resemble the round biological socket;
- future catalyst position remains covered and irrelevant.

### Physical acceptance geometry

- recessed frame cradle supports all four reinforced corners;
- one matching keyed rail/tooth receiver;
- one offset stop or corner feature prevents rotation error;
- insertion does not require fine finger alignment;
- component can be seated with one hand;
- final seated depth leaves enough visible frame silhouette to confirm category;
- no pinch zone between frame and guard;
- player can retrieve the component before commit.

### Valid seating cues

Use all applicable cues:

- visible final depth;
- physical snap or detent;
- short haptic confirmation;
- mechanical latch/indicator movement;
- brief audio cue;
- spoken/caption phrase where accessibility narration is active: `Equipment Frame seated.`

Do not use green light alone.

### Invalid seating cues

- frame stops before seated depth;
- no commit-lever availability;
- gentle return/eject bias rather than trapping the component;
- spoken/caption phrase: `Turn the frame until the keyed edge matches.`

Wrong-category item phrase: `Place an Equipment Frame in the square socket.`

## 7. Interaction sequence

1. retrieve frame from inventory or Ore Processor output drawer;
2. grab one side rail;
3. approach the Assembler square socket;
4. align coarse keyed edge;
5. seat until mechanical detent confirms;
6. release component;
7. component remains stable with guard open;
8. cancel before commit returns/removes the frame without loss;
9. guard closure and valid biological input enable commit;
10. after completion, the frame no longer exists as a separate loose item and is represented by the finished equipment.

Steps 8–10 require economy/save ownership review before implementation.

## 8. Collider and grab target

Proposed strategy for later implementation review:

- one simple outer frame collider envelope;
- central opening remains non-colliding unless needed for hand passage;
- keyed edge uses simplified stepped collider, not every visual tooth;
- one primary grab volume along a non-keyed rail;
- optional second grab volume for accessibility;
- no mesh collider unless a later audit proves it necessary;
- socket matching should use stable keyed anchors/categories, not free physics alone.

## 9. Material and Quest class

```yaml
asset_tier: repeated-near
primary_materials:
  - worn structural steel
  - repairable fasteners
secondary_materials:
  - darker mounting-channel inserts
emissive_states: none
lod_count_target: low complexity; exact count governed by Forge intake
material_slots_max: target 1-2
collider_strategy: primitive/compound
instancing_requirement: yes where repeated
```

The keyed silhouette and reinforced corners must survive the lowest in-hand LOD used during normal play.

## 10. Accessibility contract

- **spoken name:** `Equipment Frame.`
- **short function:** `The metal body for a field utility.`
- **extended function:** `Place it in the square Assembler socket.`
- **icon:** asymmetrical square open frame with one toothed/keyed edge;
- **silhouette nickname:** `the square frame`;
- **non-color category cue:** open square frame shape and toothed edge;
- **success phrase:** `Equipment Frame seated.`
- **invalid orientation phrase:** `Turn the frame until the keyed edge matches.`

## 11. Required diagrams before review-ready

- front/side/top clean silhouette;
- keyed-edge close-up;
- gloved-hand hold pose;
- Ore Processor output drawer pose;
- Assembler approach alignment;
- fully seated frame;
- wrong rotation stop;
- wrong-category rejection;
- guard-open retrieval clearance;
- simple collider envelope.

## 12. Device questions

Terry/headset review must answer:

1. Is the frame comfortable and obvious to grab with one hand?
2. Is the keyed edge recognizable without reading?
3. Can a seated player see both edge and socket while aligning?
4. Can a six-year-old match the frame after one demonstration?
5. Does the frame remain stable when released before guard closure?
6. Is retrieval easy if the wrong component is placed?
7. Does the object feel unnecessarily large or heavy?
8. Does the square category remain distinct beside the round biological component?

## 13. Stop conditions

Do not mark implementation-ready when:

- exact dimensions come only from generated labels;
- keyed orientation remains rotationally ambiguous;
- child/seated alignment requires wrist strain or deep reach;
- collider requires high-detail mesh collision;
- frame can enter the round biological socket;
- visual category is lost at Quest in-hand distance;
- save/cancel semantics are being invented in this art contract.
