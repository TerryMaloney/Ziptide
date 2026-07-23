# DEVICE-GATE PRODUCTION PACK — concept art converted into build contracts

**Status:** implementation-ready paper work. No runtime, scene, prefab, shader, input, coupler, weapon,
or travel files are changed by this document. The authorized `c45b1a2` headset artifact remains frozen.

This pack converts the current Picasso/Fable concept canon into explicit inputs for Forge, WC-5,
booth comparison, audits, and later headset verdicts. Numeric dimensions below are **VR engineering
targets**, not claims that the concept pixels were photogrammetrically measured. A builder may tune
within the listed bands, but moving outside them requires an amended spec rather than an ad hoc scene fix.

---

# ASSET: ARTIFACT KEY

## Visual identity

The key is a two-part Architect object whose halves are clearly related to the coupler, RILL, the gate,
and the MK2 drive-heart. It must read as ancient precision rather than a fantasy crystal or ordinary
metal key. The primary tell is the frozen-current channel pattern: energy looks caught in branching
motion under a thin stone-metal skin.

Protect these tells:

- two independently useful halves with a deliberate S-like joining seam;
- dark stone-metal body with restrained amber structural nodes;
- cyan current channels that remain visible without turning the whole object into a glow stick;
- joined state creates one continuous channel network rather than hiding the seam;
- no generic teeth, key ring, sword grip, or USB-device silhouette.

## Scale and ergonomics

Engineering target:

- joined overall length: **0.26–0.30 m**;
- each half: **0.14–0.17 m** along its longest axis;
- graspable body thickness: **0.032–0.048 m**;
- minimum finger clearance around intended grip: **0.018 m**;
- joined center of mass stays within **0.035 m** of the seam center;
- readable at face distance **0.25–0.55 m** and at arm distance **0.55–0.90 m**.

Each half needs a natural one-hand grip with no razor edge crossing the palm. The joined object may be
held from either half. It must never auto-scale when picked up.

## Material zones

1. **Architect body:** dense dark stone-metal, low-to-medium roughness, subtle iridescent edge response.
2. **Frozen-current channels:** cyan emissive mask beneath the surface; emission is local, not full-body.
3. **Structural nodes:** sparse amber insets at seam anchors and alignment points.
4. **Repair/history marks:** extremely restrained; this is old, not rusty or scavenger-built.
5. **Join face:** darker recessed material so separation remains legible when energy is off.

The body and channel mask must remain separate material/control zones. Skin or state changes may alter
channel color/intensity, but not erase the joining seam.

## State vocabulary

### Separated

- both halves stable and independently grabbable;
- current channels breathe at low intensity;
- join faces show a faint directional alignment cue;
- no continuous arc spam.

### Near alignment

- begins inside a **0.05 m** proximity envelope when orientation is valid;
- two to four short, bounded arc filaments bridge the gap;
- haptic cue is a single soft tick plus a restrained alignment hum;
- invalid orientation may flicker once, then settle; it must not strobe.

### Joined

- physical seam remains visible;
- current flows continuously across both halves;
- amber seam nodes lock once, then return to steady state;
- joined form exposes a stable coupler/drive-heart socket transform.

## Interaction and collider envelope

- one convex grasp collider per half, fitted to the visible grip body;
- no collider may extend more than **0.012 m** outside visible geometry;
- join sensor is separate from the grasp collider and cannot steal selection;
- joined assembly uses one authoritative rigidbody owner;
- release must not inject throw velocity merely because the join state changed;
- sockets: `KeyHalfA_Join`, `KeyHalfB_Join`, `KeyJoined_Coupler`, `ArcSocket_0..3`;
- logs: `ZIPTIDE: ARTIFACT_KEY_STATE separated|aligning|joined`.

## Forge decomposition and budget

Target Tier **B**, with a Tier-C replacement seam permitted later.

Suggested decomposition:

- two tapered body shells;
- two recessed join-face inserts;
- six to ten channel strips/mask islands per half;
- three to five amber structural nodes per half;
- optional shallow panel/inset detail, never loose greeble noise.

Hero target: **4k–9k triangles joined**, two material families plus one controlled emissive family,
one 1k texture set or equivalent atlas allocation. LOD1 may reduce channel geometry to baked masks;
LOD2 becomes a silhouette-preserving body with one emissive seam.

## Build acceptance

- silhouette reads as one designed two-part object in black fill;
- both halves fit an adult and child-sized virtual hand without appearing miniature;
- separated, near, and joined states are distinguishable with emission disabled in screenshots;
- joined channel network crosses the seam correctly;
- comparison plate includes front, side, join-face, in-hand, and three-state views;
- conformance, material split, collider envelope, particle cap, and held-item scale gates pass;
- final close is Terry's headset verdict against the keeper reference.

---

# ASSET: RILL ORB

## Visual identity

RILL is a close companion, not a generic floating UI ball. The keeper direction is a compact repaired
Architect orb: a precise spherical body, one expressive recessed eye/lens, branching frozen-current
channels, and an unmistakable riveted repair panel that proves she has history.

Protect these tells:

- nearly spherical primary mass, with asymmetry coming from repair and channel work;
- one recessed eye/lens with a readable iris, not a flat glowing decal;
- repair panel remains visible in every skin and mood;
- cyan/amber Architect language matches the key and MK2 without making RILL emotionless;
- no humanoid mouth, cartoon eyebrows, propeller, wings, or weapon silhouette.

## Scale and presence

Engineering target:

- body diameter: **0.28–0.34 m**;
- eye/lens visible diameter: **22–30%** of body diameter;
- repair panel occupies **12–20%** of visible surface area;
- default conversation distance from headset: **0.65–0.95 m**;
- minimum comfort distance: **0.45 m**; she must not drift through the player's face;
- default eye line: **0.05–0.18 m** below the player's eye height when conversing.

RILL may compress her personal-space orbit in cramped interiors, but she never scales down to fit.

## Material zones

1. **Main shell:** dark grown stone-metal or iridescent Architect alloy, medium roughness.
2. **Eye housing:** darker recessed ring with a clear lens and separately authored iris texture.
3. **Current channels:** thin emissive network controlled by mood and narrative state.
4. **Repair panel:** fixed warmer metal, visible fasteners, slight mismatch from the shell.
5. **Micro-wear:** limited to the repair area and contact edges; never global rust.

The iris, channel emission, shell, and repair panel must be independently addressable.

## Mood/state vocabulary

Map to the existing `RillState` contract rather than inventing a second emotion system.

### Calm / attentive

- cyan iris at medium value;
- channel emission low and steady;
- slow breathing pulse, no constant bobbing frenzy.

### Curious / delighted

- iris opens slightly and shifts cyan toward teal;
- localized channel wave travels toward the eye;
- one small orbit lean or tilt, then returns to neutral.

### Concerned / warning

- iris tightens; amber appears at structural nodes;
- channel rhythm becomes slower and heavier, not flashing red;
- body holds more still to make the warning readable.

### Glitched / memory pressure

- controlled cyan-to-violet discontinuity across selected channels;
- brief iris desynchronization and one bounded pose hitch;
- never rapid full-frame flicker, random teleporting, or continuous audio distortion.

## Motion and interaction

- movement vocabulary: follow, converse, inspect target, warning hold, recall/return;
- body orientation follows the eye target with critically damped motion;
- repair panel should occasionally rotate into view, not always hide behind the orb;
- collision is non-blocking to player locomotion but prevents wall burial;
- selectable surfaces are explicit; the entire visual shell is not a giant accidental button;
- logs: `ZIPTIDE: RILL_VISUAL_STATE <state>` and `ZIPTIDE: RILL_PERSONAL_SPACE_CLAMP`.

## Forge decomposition and budget

Target Tier **A/B**:

- primary sphere/shell;
- recessed eye socket and lens;
- iris plane/mesh behind lens;
- repair-panel shell plus fasteners;
- channel detail from emissive/normal bake with selective geometry only at hero distance.

Hero target: **5k–10k triangles**, maximum three material families, 1k face-distance texture allocation
with the iris receiving protected texel density. LOD1 bakes fasteners and minor channel relief; LOD2
preserves orb, eye, repair-panel color block, and one channel emission mask.

## Build acceptance

- RILL is recognizable from silhouette plus eye/repair-panel color blocks at five meters;
- repair panel appears in all skins and LODs;
- four mood states remain distinct in a muted-color accessibility capture;
- no mood uses rapid luminance reversal or full-body strobe;
- personal-space clamp prevents face intersection through the full conversation route;
- comparison plate includes neutral turnaround, eye close-up, repair-panel close-up, four moods,
  conversation distance, and dark-scene readability;
- conformance, face-distance texel, material-zone, particle, and companion motion gates pass.

---

# ASSET: ZIPTIDE GATE LIFECYCLE

## Visual identity

The gate is a standing wall/crest of photon-fluid and entrained matter, not a flat portal disc. Its
surface language is a frozen ocean wave held at the moment before breaking: sculpted crest planes,
visible trough depth, cyan-white internal flow, amber Architect structure, and loose rim matter being
pulled into ordered motion.

Protect these tells:

- wave/crest silhouette with directional flow;
- visible thickness and trough depth from side angles;
- photon-fluid surface rather than smoke, fire, glass, or a generic circular vortex;
- entrainment at the rim: nearby loose pieces dissolve into streams and become part of the gate;
- the inside-traversal beat can become abstract, but entry and exit stay spatially grounded.

## Scale and composition

Gate size is world-authored, but the first hero gate must satisfy:

- clear opening at least **3.0 m wide × 3.2 m high** for player and compact vehicle readability;
- crest thickness visually reads between **0.35–0.80 m**;
- safe interaction apron extends at least **2.0 m** from the entry face;
- no required control is placed above **0.95 m** unless duplicated at child-reachable height;
- total transparent screen coverage remains below **35%** in the normal open state.

## Material zones

1. **Photon-fluid body:** layered normals and depth gradient on sculpted crest/trough geometry.
2. **Internal current:** cyan-white emissive flow traveling through the wave body.
3. **Architect frame/seals:** sparse amber structural anchors and glyph channels.
4. **Entrained matter:** authored rim fragments using dissolve masks and bounded particle streams.
5. **Traversal shell:** separate full-view effect used only during the transition window.

The photon-fluid body and traversal shell are separate systems so normal operation never becomes a
permanent full-screen transparent effect.

## Lifecycle states

### Dormant

- frame/seals visible; photon-fluid absent or reduced to a thin pooled seam;
- one slow amber diagnostic path indicates the device is real but inactive;
- environmental audio carries the state without a blinking UI button.

### Waking

- current climbs the frame into the trough;
- crest grows in one readable direction over **1.0–2.5 seconds**;
- loose rim fragments begin restrained entrainment;
- no instant white flash or camera impulse.

### Open / breathing

- stable wave silhouette with slow breathing displacement;
- internal flow is directional and continuous;
- rim spray stays within the authored particle cap;
- opening remains visually navigable and does not pulse the whole field of view.

### Traversal

- entry contact pulls local current toward the player/ship;
- transition shell closes around the view with a smooth luminance ramp;
- camera remains owned by the rig; the effect moves around the player;
- scene load is hidden inside the existing travel contract.

### Exit / reform

- destination gate opens outward before the traversal shell clears;
- spatial horizon and floor return before control is restored;
- effect cannot expose one frame of unlit/empty scene.

### Collapse

- crest drains toward anchors rather than exploding;
- rim fragments settle or finish dissolving;
- collapse has a minimum readable duration of **0.75 seconds** unless an emergency story beat overrides it.

### Entrainment surge

- a bounded story state, not the default idle;
- larger loose objects break into authored ribbons before joining the current;
- object silhouettes remain recognizable for the first half of the dissolve;
- surge cannot exceed particle/transparent coverage rails or trigger rapid flicker.

## Comfort and safety rails

- no repeated full-field luminance reversals;
- no forced camera roll, translation, shake, or FOV pulse;
- traversal shell fade-in and fade-out each use at least **0.25 seconds**;
- high-frequency normal motion is clamped at close range;
- audio carries intensity when visual intensity must be comfort-capped;
- seated and child-height players receive the same readable activation path.

## Build acceptance

- all seven lifecycle states are captured in one deterministic state sheet;
- open gate passes front, side, and oblique thickness reads;
- dormant state is clearly inactive without relying on English text;
- transition route never exposes empty scene or restores controls before floor/horizon stability;
- transparent coverage, particle count, shader safety, travel ownership, and comfort gates pass;
- device close requires one normal traversal and one repeated traversal without visual or input drift.

---

# ASSET: MK2 ARCHITECT LIVING YACHT

## Visual identity

The MK2 is the approved 25–30 m Architect-derived living yacht. It is not a tiny fighter and not a
translucent energy blob. The airframe must pass a black-silhouette ship test: sharp nose, forward
teardrop canopy, long substantial fuselage, swept manta/delta wings, twin gill nacelles, and a clear
rear thrust direction.

Protect these tells:

- solid iridescent grown-alloy body, roughly seventy percent physical hull;
- cyan frozen-current circulation under a skin-thin surface layer;
- detached force-field-held wingtip vanes as accents, never the whole body;
- dorsal greenhouse visible through a long canopy;
- salvaged grabber arm and welded plating on a real belly/flank hardpoint;
- Cal's lantern, bird, keepsakes, worn textiles, and practical controls remain inside every skin;
- gill-like nacelle intakes and contained cyan plasma bloom, never open flame.

## Scale and layout

Engineering target:

- overall length: **25–30 m**;
- maximum width in flight state: **15–20 m**;
- walkable interior clear height: **2.1–2.5 m**;
- primary corridor clear width: **1.1–1.5 m**;
- cockpit control reach band: **0.55–1.15 m**, with essential controls duplicated at child-reachable height;
- every room has a return sightline, landmark, or signed route to prevent interior disorientation.

Canonical front-to-back zones:

1. cockpit;
2. crew lounge and galley;
3. dedicated bedroom/sleeping quarters with bunk pods;
4. dorsal greenhouse;
5. central drive-core/heart;
6. fabricator/workshop bay;
7. side airlock/boarding module.

The existing blueprint is incomplete until the dedicated bedroom/bunk zone is represented.

## Fixed identity zones

These do not disappear when a skin changes:

- grabber arm and its jury-mounted hardpoint;
- welded scavenger plating patch;
- drive-heart and artifact-key S-seam;
- lantern, bird trinket, taped notes/photos, worn blankets, books, kettle/thermos;
- chunky physical cockpit controls required for VR readability;
- greenhouse gameplay sockets and fabricator interaction sockets.

## Skinnable zones

- exterior grown-alloy plate material;
- frozen-current channel color/pattern within approved contrast limits;
- trim and structural-node material;
- room shell panels, concealed lighting, upholstery shell, bunk shutters;
- greenhouse planter shell and watering-can shell;
- non-gameplay holographic presentation layer.

A skin changes the shell, not collision, room coordinates, interaction sockets, save IDs, or Cal's
fixed identity anchors.

## Reconfigure vocabulary

### Flight state

- plates drawn close to the airframe;
- wingtip vanes aligned for a clean thrust read;
- greenhouse and drive-heart protected;
- grabber arm stowed inside its hardpoint envelope.

### Work/bloom state

- selected panels separate along circulatory seams;
- greenhouse and drive-heart become more visible;
- grabber arm deploys;
- detached vanes move to a stable work geometry;
- all movement is authored transform animation with validated swept volumes, not free physics.

Transition must be interrupt-safe: loading a save or changing scenes resolves to one canonical state,
never half-open transforms.

## LOD and intake bands

Tier **C hero**, with a permanent Forge-built B+ bridge/distant version.

- **H0 interior/berth hero:** reviewed hero mesh and face-distance materials; room shells separate from gameplay sockets.
- **H1 exterior near:** full silhouette, canopy/greenhouse, graft, nacelles, channels, simplified small detail.
- **P2 mid-distance:** silhouette, canopy color block, graft mass, engine bloom, major channel paths.
- **P3 far/distant:** Forge B+ hull, solid silhouette, two nacelle lights, greenhouse strip, one graft asymmetry.
- **impostor:** only where world vista distance makes parallax negligible.

WC-5 intake must record provenance/license, source units, forward/up axes, material-zone map, LOD source,
triangle/material/texture counts, pivot hierarchy, collision ownership, and the exact visual-child swap
point used by `ForgeVisualApplier`/ship visual application.

## Interaction and contract boundaries

- gameplay remains owned by `ShipDefinition`, coupler, existing flight model, and `TravelCoordinator`;
- imported visual children never become authorities for travel, save, seat, room, or damage state;
- room shell swaps preserve stable socket IDs and navigation volumes;
- the rig is never parented to a moving decorative mesh;
- logs: `ZIPTIDE: SHIP_VISUAL_VARIANT`, `ZIPTIDE: SHIP_RECONFIGURE`, and intake provenance ID.

## Build acceptance

- black-fill silhouette reads as a substantial ship from front, side, top, and rear;
- a human scale figure and full seven-zone interior fit without cheating the 25–30 m envelope;
- flight/work states have validated swept volumes and deterministic end states;
- fixed identity zones survive at least three radically different skin mockups;
- greenhouse, graft, teardrop canopy, gill nacelles, and detached vanes survive H1 and P2;
- P3 Forge bridge is visually compatible with the Tier-C hero and remains the permanent distant LOD;
- WC-5 provenance, budget, material-zone, skin invariance, room-socket, and conformance gates pass;
- booth output includes hero reference comparison, turnaround, scale cutaway, reconfigure pair,
  cockpit, each interior zone, and P2/P3 distance captures;
- final close requires berth, cockpit, and in-flight headset verdicts.
