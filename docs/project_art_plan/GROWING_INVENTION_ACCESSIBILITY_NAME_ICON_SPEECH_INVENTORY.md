# GROWING / INVENTION ACCESSIBILITY — NAME, ICON, SPEECH, AND PHYSICAL-CUE INVENTORY

**Status:** canonical design inventory for child-readable and non-text-dependent presentation across the current growing/invention concept lane. Docs-only and freeze-compatible. Names marked `working` or `provisional` are not story/progression canon.

**Purpose:** ensure every important seed, plant output, refined material, machine, equipment item, socket, and interaction state can be recognized by silhouette, heard aloud, captioned, and operated without relying on fluent reading or color alone.

## 1. Accessibility law

Every player-facing item or station must have:

1. one stable short display name;
2. one spoken-name line;
3. one short spoken-function line;
4. a caption twin carrying the same meaning;
5. one dominant silhouette or shape category;
6. one large icon describable in plain language;
7. physical placement or socket grammar;
8. active/invalid/complete cues duplicated beyond color;
9. narration behavior that does not chatter continuously while scrolling;
10. language simple enough for a young player while remaining useful to adults.

Generated labels and symbols in concept images are never authoritative.

## 2. Speech pattern

### Name readout

Use the shortest stable player-facing name.

Examples:

- `Moss Filter Seed.`
- `Filter Organ.`
- `Moss Filter Rig.`

### Short function readout

Target one simple sentence, generally 3–10 words.

Examples:

- `Grows material for filtering gear.`
- `Lets you enter light toxic areas.`
- `Holds loose parts while you repair them.`

### Extended function readout

Available through deliberate help/focus, not every hover.

Examples:

- `Place this seed in a grow plot. Harvest the pods when the plant matures.`
- `Dock this module over a broken low-voltage relay gap, then lower the lever.`

### Caption twin

Caption must preserve the spoken meaning rather than adding hidden instructions unavailable to non-audio players.

## 3. Focus and repetition behavior — design targets

Exact timing remains subject to UI/accessibility implementation review.

- do not speak immediately during fast menu traversal;
- use a short focus dwell before speaking the item name;
- speak the function only after longer focus or deliberate help action;
- suppress repeat name narration when moving back and forth rapidly;
- allow replay on demand;
- stop or duck speech when the player exits the menu/context;
- machine slots may announce category before item name when empty;
- invalid placement should state the category needed, not blame the player;
- no important instruction should exist only in speech.

## 4. Shape-category grammar

| Category | Dominant shape | Meaning |
|---|---|---|
| seed / grow input | asymmetric puck or keyed capsule | place into matching grow socket |
| raw biological output | rounded/organic family-specific form | harvest, then stabilize |
| refined biological component | round or family-keyed insert | goes into Assembler biological socket |
| raw ore/salvage | irregular chunk | goes into Ore Processor tray |
| industrial stock | square/open frame, plate, strip, or bar | recipe-ready industrial material |
| equipment frame | asymmetrical square/open frame | goes into Assembler square socket |
| one-slot field equipment | compact rectangular belt module | choose one belt slot and use in world |
| future catalyst | triangle | covered and unavailable until taught |

Shape is primary. Color and text are secondary.

## 5. Filter-family inventory

### Moss Filter Seed

- **name status:** working/approved family direction;
- **spoken name:** `Moss Filter Seed.`
- **short function:** `Grows material for filtering gear.`
- **extended function:** `Place it in the matching grow socket. Harvest the Filter Organs when it matures.`
- **caption twin:** `Grows Filter Organs for filtering gear.`
- **icon:** notched porous puck with three visible honeycomb openings;
- **silhouette nickname:** `the holey seed puck`;
- **physical cue:** asymmetric notch aligns with grow-socket key;
- **non-color cue:** socket cannot fully seat in the wrong rotation; seated state uses mechanical drop/snap.

### Moss Filter plant

- **spoken name:** `Moss Filter.`
- **short function:** `Filters toxic particles and grows Filter Organs.`
- **extended function:** `Its membrane leaves trap toxic particles. Harvest the hanging organs when they are ready.`
- **icon:** layered membrane leaf over a porous circle;
- **silhouette nickname:** `the filter-leaf plant`;
- **ready cue:** harvest organ changes size/position and releases with a physical twist/snap, not color alone.

### Filter Organ

- **spoken name:** `Filter Organ.`
- **short function:** `Raw material for filtering equipment.`
- **extended function:** `Stabilize it in the BioRefiner before assembly.`
- **icon:** round honeycomb organ with moss rim;
- **silhouette nickname:** `the sponge pod`;
- **socket category:** raw biological tray;
- **non-color cue:** porous face and organic collar remain tactile/visible.

### Refined Filter Membrane

- **spoken name:** `Filter Membrane.`
- **short function:** `A stable filtering component.`
- **extended function:** `Combine it with an Equipment Frame in the Assembler.`
- **icon:** three thin layered membranes inside a round rim;
- **silhouette nickname:** `the layered round filter`;
- **socket category:** round biological Assembler socket.

### Moss Filter Rig Mk I

- **spoken name:** `Moss Filter Rig.`
- **short function:** `Lets you enter light toxic areas.`
- **extended function:** `Equip it in one belt slot. It protects you only in approved low-grade contaminated spaces.`
- **caption twin:** `One-slot utility for light toxic areas.`
- **icon:** compact square module with one round porous cartridge;
- **silhouette nickname:** `the belt filter`;
- **active cue:** mechanical clean/blocked indicator plus restrained internal pulse;
- **invalid cue:** target-zone warning must use symbol, audio/caption, and mechanical state—not only color.

## 6. Grip-family inventory

Names and world placement remain provisional except where separately approved.

### Gripvine Seed

- **spoken name:** `Gripvine Seed.`
- **short function:** `Grows material that holds to wet surfaces.`
- **icon:** asymmetric two-lobed clamp puck;
- **silhouette nickname:** `the clamp seed`;
- **physical cue:** clamp-shaped key fits only the matching grow socket.

### Grip Pad

- **spoken name:** `Grip Pad.`
- **short function:** `Sticks firmly to wet stone and metal.`
- **extended function:** `Stabilize it before using it in equipment.`
- **icon:** pale flattened pad pressed against a dark surface;
- **silhouette nickname:** `the sticky pad`;
- **non-color cue:** pad visibly flattens and microhook face changes contact shape under pressure.

### Grip Fiber

- **spoken name:** `Grip Fiber.`
- **short function:** `A strong living fiber for high-load parts.`
- **icon:** three irregular braided biological strands with visible cut end;
- **silhouette nickname:** `the living rope`;
- **warning:** avoid ordinary-rope iconography; preserve biological segmentation and cut anatomy.

### Refined Grip Surface Insert

- **spoken name:** `Grip Surface.`
- **short function:** `A stable gripping component.`
- **icon:** round pale contact face with dark fibrous rim;
- **socket category:** round biological Assembler socket.

### Cistern Grip Clamp Mk I

- **spoken name:** `Grip Clamp.`
- **short function:** `Holds loose parts while you repair them.`
- **extended function:** `Press it against wet stone or metal, lock the lever, then use your free hand to finish the repair.`
- **caption twin:** `Temporarily braces one loose repair part.`
- **icon:** loose panel beside a clamp, then the same panel held flat;
- **silhouette nickname:** `the sticky clamp`;
- **non-color states:** lever up/down, jaw open/closed, pad flat/compressed, mechanical lock marker;
- **forbidden read:** grappling gun, traversal launcher, weapon, permanent building tool.

## 7. Conduct-family inventory

Exact species/world/progression names remain provisional.

### Relay Reed Seed

- **spoken name:** `Relay Reed Seed.`
- **short function:** `Grows material that carries low-voltage power.`
- **icon:** split cylinder with two offset contact ridges;
- **silhouette nickname:** `the split seed`;
- **physical cue:** ridges align with an insulated keyed socket.

### Relay Reed plant

- **spoken name:** `Relay Reed.`
- **short function:** `Routes low-voltage current through living veins.`
- **icon:** two forked reed tips joined by one internal route;
- **silhouette nickname:** `the wire reed`;
- **active cue:** one pulse travels along one internal path; no whole-body glow.

### Conductive Vein

- **spoken name:** `Conductive Vein.`
- **short function:** `A living path for low-voltage current.`
- **extended function:** `Stabilize it before using it in a bridge module.`
- **icon:** segmented strand with one visible internal copper path;
- **silhouette nickname:** `the living wire`;
- **warning:** must not look like ordinary store-bought cable.

### Charge Nodule

- **spoken name:** `Charge Nodule.`
- **short function:** `Stores a small controlled electrical charge.`
- **extended function:** `Use only in protected equipment made for stored charge.`
- **icon:** asymmetric smoky pod with one internal spiral and torn collar;
- **silhouette nickname:** `the spiral pod`;
- **warning:** no light-bulb, battery, ammunition, or weapon-cartridge read.

### Refined Conductive Vein Insert

- **spoken name:** `Conductive Insert.`
- **short function:** `A stable low-voltage routing component.`
- **icon:** protected segmented route inside a keyed biological holder;
- **socket category:** biological Assembler holder; final geometry pending measured spec.

### Circuit Bridge Mk I

- **spoken name:** `Circuit Bridge.`
- **short function:** `Connects one broken low-voltage power gap.`
- **extended function:** `Dock it flat over the matching relay gap, close both side clamps, then lower the top lever. It stays attached while you work.`
- **caption twin:** `Docked one-slot utility for a broken low-voltage relay.`
- **icon:** broad flat box spanning two separated contacts;
- **silhouette nickname:** `the power bridge box`;
- **non-color states:** clamps open/closed, lever up/down, mechanical connected indicator, analog target response;
- **invalid phrase:** `This bridge does not fit here.`
- **unsafe/high-voltage phrase:** `Voltage too high. Bridge refused.`
- **success phrase:** `Bridge connected.`
- **forbidden read:** gun, taser, remote hacking tool, universal key, battery pack.

## 8. Industrial component inventory

### Equipment Frame Blank

- **spoken name:** `Equipment Frame.`
- **short function:** `The metal body for a field utility.`
- **extended function:** `Place it in the square Assembler socket.`
- **icon:** asymmetrical square open frame with keyed edge;
- **silhouette nickname:** `the square frame`;
- **socket category:** square industrial Assembler socket;
- **non-color cue:** keyed edge prevents wrong orientation.

### Structural Plate Blank

- **spoken name:** `Structural Plate.`
- **short function:** `A strong flat part for machines and equipment.`
- **icon:** reinforced square plate;
- **status:** vocabulary only; early-game exposure unapproved.

### Conductive Strip

- **spoken name:** `Conductive Strip.`
- **short function:** `A flexible metal path for powered equipment.`
- **icon:** flat bent strip with two contact ends;
- **status:** vocabulary only; avoid confusion with Conductive Vein through square industrial framing and metallic edge language.

### Dense Anchor Bar

- **spoken name:** `Anchor Bar.`
- **short function:** `A heavy part for high-load machinery.`
- **icon:** short thick bar with reinforced ends;
- **status:** vocabulary only.

## 9. Machine inventory

### Grow Tray / Nursery

- **spoken name:** `Grow Tray.`
- **short function:** `Plant seeds and harvest useful biology.`
- **empty-slot phrase:** `Choose a seed for this plot.`
- **covered-slot phrase:** `This plot is not available yet.`
- **ready phrase:** `Ready to harvest.`
- **icon:** seed puck entering a rectangular plot;
- **physical cues:** keyed seed socket, covered inactive plot, reachable harvest points, analog growth state.

### BioRefiner Mk I

- **spoken name:** `BioRefiner.`
- **short function:** `Makes raw biology stable for crafting.`
- **empty-input phrase:** `Place one raw biological item in the tray.`
- **ready phrase:** `Tray seated. Close the lock.`
- **commit phrase:** `Ready to stabilize.`
- **complete phrase:** `Refined material ready.`
- **invalid phrase:** `This item does not fit the BioRefiner.`
- **icon:** organic item entering a guarded chamber, stable round insert leaving a separate tray;
- **non-color cues:** tray position, latch position, lever availability, gauge motion, separate output drawer.

### Ore Processor Mk I

- **spoken name:** `Ore Processor.`
- **short function:** `Turns ore and salvage into standard metal parts.`
- **empty-input phrase:** `Place one ore or salvage piece in the tray.`
- **ready phrase:** `Tray seated. Lock the crossbar.`
- **commit phrase:** `Ready to process.`
- **complete phrase:** `Industrial part ready.`
- **invalid phrase:** `This material does not fit the Ore Processor.`
- **icon:** irregular chunk entering a heavy chamber, square frame leaving the lower drawer;
- **non-color cues:** tray, crossbar, high-resistance handle, pressure gauge, separate drawer.

### Assembler Mk I

- **spoken name:** `Assembler.`
- **short function:** `Joins one metal frame and one biological component.`
- **square-slot phrase:** `Place an Equipment Frame here.`
- **round-slot phrase:** `Place a refined biological component here.`
- **covered-catalyst phrase:** `This socket is not available yet.`
- **ready phrase:** `Both components seated. Close the guard.`
- **commit phrase:** `Ready to assemble.`
- **complete phrase:** `Equipment ready.`
- **invalid phrase:** `That component belongs in a different socket.`
- **icon:** square plus round joined inside a guarded cradle;
- **non-color cues:** socket shape, component seating depth, guard position, lever lock, arm position, same-cradle output.

## 10. Shared error language

Use neutral, actionable phrasing.

Prefer:

- `Turn the seed until the notch matches.`
- `Place a biological item in the round socket.`
- `Close the guard before pulling the lever.`
- `The output drawer is blocked.`
- `This target is not safe for the Circuit Bridge.`

Avoid:

- `Wrong.`
- `Invalid action.`
- `User error.`
- long technical explanations during active interaction;
- color-only directions such as `use the green slot`.

## 11. Icon evaluation gates

An icon passes only when:

- recognizable at small physical-card/menu scale;
- distinguishable in monochrome;
- understandable without generated text;
- family identity remains visible;
- no icon unintentionally resembles a weapon or hazard category;
- square industrial, round biological, asymmetric seed, and triangle catalyst categories remain distinct;
- a young player can match item to socket after one demonstration.

## 12. Open decisions requiring later review

- final localization-safe display names;
- exact narration voice and event IDs;
- focus dwell and repeat-suppression timing;
- whether item name and function use separate buttons/actions;
- exact symbol artwork and glyph integration;
- subtitle/caption placement in VR;
- tutorial sequence and first exposure order;
- child/seated reach measurements;
- audio/haptic twins;
- which provisional families/items become gameplay canon.

## 13. Runtime boundary

This inventory defines presentation requirements only. It does not authorize UI, voice, audio, item, machine, recipe, save, scene, input, or accessibility implementation before the relevant recovery and architecture gates.
