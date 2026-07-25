# GI-MSPEC-003 — MOSS FILTER RIG MK I + LOW-GRADE CONTAMINATION TARGET

**Status:** draft measured interaction contract. Equipment silhouette, family grammar, one-slot role, and the requirement for a visible world payoff are established. Numerical dimensions, exposure timing, resource consumption, activation method, hazard thresholds, exact W001 placement, recipe IDs, and implementation remain unresolved. Docs-only and freeze-compatible.

```yaml
spec_id: GI-MSPEC-003
asset_id_proposed: equipment.moss_filter_rig.mk1
display_name_working: Moss Filter Rig
asset_class: field_equipment_and_environment_target
status: draft
world_or_ship_context: W001 The Moss / Toxic City candidate
slot_or_mount_role: one belt slot / low-grade contamination access
implementation_authority: none
```

## 1. Governing authority

- `docs/growing/GROWING_INVENTION_LOOP_DECISION_RECONCILIATION.md`
- `docs/production/W000_W001_MODEL_BAND_PRODUCTION_PACKET.md`
- `docs/production/W001_IDENTITY_DECISION_BRIEF.md`
- `docs/project_art_plan/GROWING_INVENTION_CONCEPT_KEEPER_INDEX.md`
- `docs/project_art_plan/GROWING_INVENTION_MEASURED_SPEC_EXTRACTION_PACKET.md`
- `docs/project_art_plan/GROWING_INVENTION_ACCESSIBILITY_NAME_ICON_SPEECH_INVENTORY.md`
- dedicated compact belt-mounted Moss Filter Rig keeper image selected in the Filter-family concept pass
- dedicated Filter Organ / refined membrane component references

### Reference authority split

- compact equipment body, belt role, replaceable round biological cartridge, service/open view: dedicated Moss Filter Rig keeper;
- Filter-family anatomy and material grammar: Moss Filter lifecycle and Filter Organ keepers;
- contamination fiction, W001 mood, and route role: W001 identity and production documents;
- exact contaminated-space placement and route consequence: future W001 WorldSpec and first-hour route plate;
- exact dimensions, belt origin, air coupling, activation behavior, collider, hazard logic, duration, and depletion: unresolved;
- generated chest harness, mask, labels, logos, and embedded machine-sheet outputs: rejected/non-authoritative.

## 2. Locked role

> The Moss Filter Rig Mk I is a compact one-slot belt utility that visibly enables the player to enter or interact with one class of **low-grade contaminated space** that is unsafe without filtering support.

The item must create an observable option—not a hidden percentage bonus.

It is not:

- permanent poison immunity;
- a full respirator suit, chest harness, backpack, or helmet replacement;
- a combat gas weapon;
- a universal environmental bypass;
- a substitute for higher-tier atmosphere or vacuum equipment;
- an excuse to remove readable hazard telegraphing.

## 3. Equipment silhouette and component contract

- broad compact rectangular salvage-metal body;
- one dominant round living Filter Membrane cartridge;
- battered frame, repairable fasteners, and one visible belt attachment;
- one suit-air coupling or equipment-interface feature, exact routing unresolved;
- cartridge uses warm off-white porous/honeycomb tissue with muted moss-green biological rim;
- restrained cyan activity only while filtering or confirming a valid state;
- one large VR-friendly service latch for cartridge access;
- one mechanical clean/blocked/ready state that does not rely on color;
- no weapon grip, muzzle, mask hose dominating the silhouette, or chest-scale body.

At thumbnail size, a young player should recognize it as `the filter box with the round plant part`.

## 4. Coordinate and handling contract

```yaml
forward_axis: outward from visible cartridge face
up_axis: service-latch / belt-loop orientation
primary_grab_origin: non-cartridge side rail or top edge
secondary_grab_origin: optional opposite edge for service handling
belt_mount_origin: rear or top loop; final location subject to three-slot belt clearance test
service_open_axis: cartridge door/latch opens away from player hand
cartridge_insertion_axis: normal into round cartridge cradle
keyed_orientation_feature: one asymmetric biological rim notch plus matching cradle stop
resting_orientation: cartridge face outward on belt
mass_gameplay_class: light-one-hand
```

The cartridge must not plausibly seat backward or rotate freely into an unreadable state.

## 5. Numerical measurement plan

No concept-art dimension is authoritative.

Before dimension lock:

1. measure the keeper relative to depicted belt and hand;
2. create small, medium, and large broad-body proxy volumes;
3. test one-hand retrieval from all three belt positions;
4. test collision with torso, hands, adjacent weapon/tool slots, seated posture, and crouch;
5. test cartridge service at waist/chest height without fine finger alignment;
6. choose the smallest body preserving the round biological silhouette and readable mechanical indicator;
7. determine air-coupling fiction only after belt and suit-interface layout are reconciled;
8. verify a six-year-old proxy can identify and operate the main latch without excessive reach or force.

```yaml
bounds_target_m: TBD
mass_target_kg: TBD
scale_tolerance_percent: TBD
belt_clearance_envelope_m: TBD
cartridge_diameter_m: TBD
cartridge_insertion_depth_m: TBD
service_latch_force_n: TBD
maximum_required_reach_m: TBD
```

## 6. Low-grade contamination target contract

The target is an authored environmental condition paired with the equipment, not an invisible damage volume with no visual explanation.

### Valid target presentation

Use several compatible cues:

- visible suspended particulates, vapor, residue, or contaminated airflow;
- environmental source such as leaking vent, canal intake, damaged filtration duct, contaminated corridor, or toxic pocket;
- stable boundary/readability at player approach distance;
- nearby material response, dead plant life, corrosion, warning shape, airflow motion, or NPC/environmental context;
- clear low-grade class distinct from lethal, vacuum, underwater, or high-temperature hazards;
- reachable optional reward, route, story trace, repair point, salvage, or shortcut beyond the boundary;
- one safe observation position before commitment;
- no reliance on written warning text alone.

### Invalid targets

- vacuum or decompression;
- fully submerged travel;
- high-temperature fire zone;
- untelegraphed instant damage volume;
- combat cloud fired by an enemy;
- generic poison status unrelated to environment;
- mandatory route with no recovery path;
- target so large that the rig becomes permanent background equipment;
- target whose only feedback is a HUD number.

### Target scale classes — provisional

- **interaction pocket:** player reaches into/briefly occupies a contaminated area to scan, retrieve, or repair;
- **short traversal pocket:** player crosses one limited contaminated corridor or alcove;
- **vent/intake service target:** rig enables operation beside contaminated machinery.

W001 should choose one primary class and at most one supporting class for the first-hour proof.

## 7. Activation and state model — unresolved decision gate

The concept evidence does not yet prove whether Mk I should be passive while equipped, manually activated, or automatically couple when entering a valid target.

Measured interaction review must compare:

### Candidate A — equipped passive

- lowest input burden;
- strong for young players;
- risk: hidden state and weak ceremony.

### Candidate B — one physical enable control

- clearer deliberate choice;
- supports audio/haptic ceremony;
- risk: extra interaction while approaching a hazard.

### Candidate C — automatic target coupling with visible acknowledgement

- maintains readability and low friction;
- risk: target-specific magic unless coupling fiction is explicit.

No implementation may select an activation model until the first-hour interaction plate and Terry review choose one.

## 8. Required state communication

### Stowed / unavailable

- belt silhouette visible;
- no active cyan flow;
- mechanical indicator at rest;
- short name available through focus narration.

### Ready / equipped

- mechanical indicator changes position;
- subtle haptic/audio confirmation;
- restrained biological activity allowed;
- no constant loud breathing or visual obstruction.

### Valid contamination boundary

- environment communicates hazard before entry;
- item or suit gives one non-color-only acknowledgement;
- optional spoken/caption cue on first encounter only: `Light contamination. Filter gear can help.`

### Filtering active

- restrained internal membrane movement or pulse;
- environmental audio changes and/or suit-air response;
- no full-screen filter effect required;
- player can tell the item is doing work without reading a meter.

### Invalid/high-grade target

- no activation or lock-in;
- mechanical refusal plus short haptic/audio cue;
- optional spoken/caption cue: `This filter is not strong enough.`

### Blocked/spent/fault state

Whether Mk I depletes, clogs, cools down, or remains reusable is not decided. If any limited-use rule is later approved, the state must be mechanical and recoverable without silently destroying the item.

## 9. Accessibility contract

Working player-facing language:

- spoken name: `Moss Filter Rig.`
- short function: `Lets you enter light toxic areas.`
- extended help: `Equip it before entering light contamination. It does not protect against stronger hazards.`
- caption twin must preserve the same meaning;
- icon: compact rectangular box with one large round porous cartridge and three particles becoming one clean airflow line;
- silhouette nickname: `the round filter box`;
- category: one-slot field equipment;
- no important meaning only in cyan/green coloration;
- first valid boundary may use one RILL/helper line, then suppress repetition.

## 10. Quest and asset-budget questions

Before implementation-ready status, resolve:

- body and cartridge mesh/LOD targets;
- whether membrane motion is shader, bones, blend shape, or static state swap;
- transparent material cost at hand distance;
- emissive-area cap and bloom dependence;
- collider count for body, latch, cartridge, and belt mount;
- whether cartridge is physically removable in v1 or service-view only;
- audio loop cost and spatialization;
- hazard particulate budget and overdraw;
- target-zone effect fallback for reduced-motion/visual-accessibility settings.

## 11. Save and atomicity questions

The visual concept does not authorize persistence behavior. Architecture review must define:

- whether equipment ownership and belt selection persist separately;
- whether a removable cartridge is inventory state or visual-only;
- what happens if save/quit occurs inside a valid contamination target;
- safe relocation/recovery behavior on load;
- whether an active limited-use state persists or resets;
- no owned item may disappear through travel/quit.

## 12. W001 proof plate requirements

Before implementation approval, create one measured route plate showing:

1. safe approach position;
2. first readable hazard cue distance;
3. belt retrieval/equip pose;
4. standing and seated entry line;
5. contamination boundary depth;
6. useful action or reward beyond the boundary;
7. retreat path;
8. first-time narration/caption timing;
9. no collision with nearby architecture;
10. post-use return to normal belt state.

## 13. Terry/headset questions

- Does the rig read as one-slot equipment rather than part of the suit?
- Can the item be retrieved comfortably from every allowed belt position?
- Is the round cartridge recognizable at world and hand distance?
- Is the contamination boundary obvious without a HUD warning?
- Does activation require too many steps for the first useful invention?
- Can a young player understand why one toxic area is valid and a stronger hazard is not?
- Is the payoff meaningful enough to justify carrying the item?

## 14. Implementation gate

This spec remains `draft` until:

1. source keeper binaries are durably archived;
2. activation model is selected;
3. proxy dimensions and belt clearance pass standing/seated/child tests;
4. one W001 contamination target plate is approved;
5. exact recipe/progression authority is reconciled;
6. Quest asset/effect budgets are assigned;
7. recovery M0 passes and the relevant freeze is explicitly lifted;
8. architecture assigns the approved data/runtime slice;
9. device verification plan is written before implementation.
