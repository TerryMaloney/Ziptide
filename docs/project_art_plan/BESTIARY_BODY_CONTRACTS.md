# BESTIARY BODY CONTRACTS — the pre-Tripo passport for each approved creature

**Written 2026-07-28 (T-Dog lane).** One entry per creature whose concept sheet is approved.
Block A must be complete **before** a model is generated; Blocks B/C before rig/animation.
Card spec: `CREATURE_ROSTER_AND_PROMPT_QUEUE.md` §5. Intake registration:
`concept_intake_manifest.json` (gate: `tools/concept_intake_gate.py`).

> **Registration note:** `concept_intake_gate.py` verifies every `conceptPaths` entry exists on
> disk. Manifest entries for the three sheets below are therefore **deliberately not added yet** —
> adding them before the PNGs are committed would turn CI red. See §0 for the exact one-step
> action when the images land.

---

## 0. WHEN THE IMAGES REACH THE REPO (do this once, at the computer)

**24 sheets exist** (21 creatures/machines + 3 plants) and none are committed yet — they are on
Terry's phone. The gate (`tools/concept_intake_gate.py:214`) verifies every registered
`conceptPaths` file exists on disk, so registration cannot precede the commit.

1. Save creature sheets into `concepts/bestiary_ch1/` and plant sheets into `concepts/flora_ch1/`
   using the exact filenames in the passport headings below (`<id>_sheet_v1.png`).
2. `git add` + commit the PNGs.
3. Add one `assets` entry per sheet to `docs/project_art_plan/concept_intake_manifest.json`:
   `status: "intake-ready"` · `specPath: "docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md"` ·
   `conceptPaths` → the committed PNG · `requiredMarkers` → the four headings every passport
   below uses: `## Scale and ergonomics`, `## State vocabulary`,
   `## Interaction and collider envelope`, `## Build acceptance`.
4. Run `python3 tools/concept_intake_gate.py` — must pass before pushing.

**Outstanding sheets (not yet generated):** `sump_tender` (peaceful canal filter-giant) ·
`glass_reed` (fibre plant).

**⚠ ID drift to settle before any model is imported:** concept sheets say
`creature_cistern_swarmer` / `creature_glass_tendril`; shipped assets say `swarm_bug` / `tendril`.
Pick one set now.

---

# ASSET: HUSK-MOLTER

## Scale and ergonomics
Body ~0.9 m long × 0.55 m wide × 0.30 m tall in shell (large-dog footprint). Wall-mounted at
**0.8–1.8 m** height so it sits inside adult AND child gaze cones. Molted soft body ~0.7 m long,
much lower profile. Threat-height band: **wall/low**. Reads at 0.6 m and 10 m.

## State vocabulary
Idle (clamped to wall, near-motionless) · idle-break (single plate flex) · locomotion (slow
wall-crawl) · turn-in-place · notice (plates lift ~15°, audible shell-tick) · approach: does NOT
approach — it holds and molts · telegraph: rear seam splits ~0.3 s before the squirt-out ·
apex: molt burst · whiff/recover: soft body scrambles · additive flinch · stun-loop (soft body
curls) · downed (curled, violet glow out).
**Counter (CI-required):** scan-then-confirm — hit the *moving* one, or gravity-grab it before the
molt completes. The husk fools the scanner exactly once.
**Weakened tell:** post-molt it is permanently soft — no second shell.

## Interaction and collider envelope
Colliders: shell = single box proxy; soft body = capsule; legs excluded. **Sockets:** gaze (head
band) · weak-point ×1 (exposed dorsal ridge of the soft body, only valid post-molt) · grab handle
×1 (mid-thorax of the soft body, sized for a controller grip, positioned away from the face) ·
feet ×12 (wall anchors) · VFX mount (rear seam) · audio mount (shell) · carry handle (soft body
thorax, reuse the grab handle).
**Separable part — THE HUSK:** its own mesh, its own LOD, persists in the world as a prop. Needs
**cap geometry at the rear seam and a hollow interior** (the sheet provides the interior view).
Missing-part silhouette: the soft body alone must still read as the same species.
Grabbable: soft body yes, husk yes (as a prop). Ragdoll: soft body only. Pushable: husk yes.

## Build acceptance
Silhouette triad (armoured-on-wall / mid-molt / curled-disabled) all reachable by one rig without
self-intersection. The empty husk must be convincingly hollow from the underside view. Violet glow
confined to a single material region on the soft body's flanks. Two-distance read confirmed.

---

# ASSET: DREDGE-BULL

## Scale and ergonomics
~4.0 m long × 2.2 m wide at the plate × 1.7 m tall. **Head plate top edge ~1.7 m** — at adult eye
level, above a child's. Threat-height band: **eye/body**. Closest approach to headset: **1.2 m**
(it is a charger; it must never end a charge inside personal space — it overshoots past the
player). Enters arm's reach: only when winded/disabled.

## State vocabulary
Idle (grazing, plate down, slow) · idle-break (snort/shake) · locomotion (heavy plod) ·
turn-in-place (**deliberately slow — this is the design**) · notice (head lifts, plate angles up,
low bellow) · approach (line-up shuffle) · **telegraph: legs brace back, plate lowers, body coils
— 0.8 s** (whole-body answer per the VR timing rule: the player must sidestep) · apex (charge) ·
**whiff/recover: impact stagger, head up, VENT EXPOSED** · additive flinch · stun-loop (winded,
vent pulsing) · downed (flat on belly, plate resting on ground).
**Counter (CI-required):** make it miss so it hits a wall/wreck; then strike the pleated vent
behind the head plate during the winded window.
**Weakened tell:** one foreleg drags, the plate visibly sags to that side.

## Interaction and collider envelope
Colliders: plate = box (must be solid — it is a shield); body = capsule; legs = capsules.
**Sockets:** gaze (small dorsal eyes) · **weak-point ×1 (the vent, behind the plate — only
exposed in winded/stunned)** · grab handle ×1 (rear haunch, low, away from the plate and the
face) · tether point (base of tail, for the salvage lasso) · feet ×4 · VFX mount (plate leading
edge, for impact sparks and mud) · audio mount (chest) · carry handle: **none — too heavy to
carry**; it is salvaged in place.
Separable parts: plate detaches as a salvage part (needs cap geometry at the skull join).
Grabbable: no (mass). Pushable: no. Ragdoll: no — use authored downed pose.

## Build acceptance
Front plate must occlude the vent completely in neutral and expose it completely when winded —
verify from three registered views. Charge-telegraph silhouette must be unmistakable **from
behind and from the side**, not just head-on. Human scale figure matches 4 m length.

---

# ASSET: WARDEN SENTINEL

## Scale and ergonomics
**3.0 m tall**, humanoid, slender. Eye at ~2.8 m — well above both eye heights, so it looms;
the chest aperture sits at **~1.7 m**, i.e. at adult eye level and reachable-looking, which is
correct because the aperture is the thing the player must read. Threat-height band: **overhead**.
Closest approach to headset: **1.5 m minimum** — see the ⚠ below. Enters arm's reach: never.

## State vocabulary
Idle (passive, limbs folded high, chest sealed, eye dim — reads as architecture) · idle-break
(slow head rotation) · locomotion (measured stride) · turn-in-place · **notice: eye brightens +
single sterile chime** · **warning (unique to this species, and the whole point): chest aperture
irises open, flat white light spills, wide stable stance — held 1.0 s before any pursuit** ·
approach (pursuit stride, chest half-open) · telegraph (aperture widens, 0.7 s) · apex
(containment field projection — non-lethal) · whiff/recover (aperture irises shut, 1.2 s
vulnerable) · additive flinch · stun-loop · downed (kneeling, folded to the ground, aperture
closed, eye black).
**Counter (CI-required):** it always warns first — leave its enforcement zone during the warning
hold, or strike the open aperture during the post-projection recovery window.
**Weakened tell:** one leg stiffens; the stride becomes asymmetric and audibly uneven.

## Interaction and collider envelope
Colliders: torso capsule, head sphere, limb capsules. **Sockets:** gaze (the single eye) ·
**weak-point ×1 (the open chest aperture — valid only while irised open)** · grab handle ×1
(forearm — the only part that ever comes within reach) · feet ×2 · VFX mount (aperture centre) ·
audio mount (head) · carry handle (torso, for the downed body — it is carryable).
Separable parts: none at v1 (a maintained machine does not shed).
Grabbable: forearm only. Ragdoll: no — authored kneel. Pushable: no.

## Build acceptance
Aperture iris must be a real modelled mechanism readable at 0.6 m (the sheet's detail panels are
the reference). Kneeling downed pose must be get-up-able. Surface must stay unbroken — no
greebles, no rust. Sterile white is the ONLY emissive.

## ⚠ Kid-comfort flag (⚖ Terry)
The sheet came back **humanoid** rather than the specced four folding limbs. Two consequences:
**(1) Production win** — a humanoid rig auto-rigs and retargets far more easily, which materially
helps the Tripo pipeline. Recommend keeping it.
**(2) Fear risk** — a silent, faceless, 3 m humanoid that approaches you is significantly more
intimidating in VR than a bird-legged machine, and this is an E/E10 family title. Mitigations
already in the design: it warns before acting, it never enters arm's reach, it is unarmed, and it
kneels when disabled. Recommend holding the **1.5 m minimum approach** as a hard rule for this
species and reviewing it with a child tester before it ships in a required encounter.

---
---

# PART 2 — COMPACT PASSPORTS (the remaining 18 creatures + 3 plants)

Same four required headings, condensed. Every entry states the fields that **gate the mesh**:
scale, locomotion, the silhouette set the rig must reach, sockets, separable parts, and whether
the disabled pose is reachable. Blocks B/C/D detail lives in `CREATURE_ROSTER_AND_PROMPT_QUEUE.md`.

---

# ASSET: WITNESS-MITE

## Scale and ergonomics
~0.35 m across × 0.15 m tall (large crab / softball). Ground-level — **below the gaze cone**, so
it MUST announce itself by sound (Block D field 30). Threat-height band: floor. Reads at 0.6 m;
at 10 m it is a rock, which is the point.

## State vocabulary
Frozen (plates clamped, nodules dull — reads as stone) · moving (plates fanned on pale membrane,
seams glowing cold blue-white) · notice: none — it has no notice state, it simply freezes ·
disabled (plates half-open, slack, glow out).
**Counter (CI-required):** keep it in view to hold it frozen, wrist-scan to pin it, then
gravity-toss. **⚠ Tier exemption required** — "moves only when unwatched" collides with
observation-based culling; pin it at T1 within 20 m regardless of visibility.

## Interaction and collider envelope
Collider: single low box. **Sockets:** gaze (shell nodule ring) · weak-point ×1 (underside feeding
aperture, valid only while moving/plates open) · grab handle ×1 (shell rim — it is small enough to
pick up whole) · feet ×6 · VFX + audio mounts (seam ring) · carry handle = the grab handle.
Separable parts: none. Grabbable: yes, whole. Ragdoll: no. Pushable: yes.

## Build acceptance
The frozen pose must be convincingly inanimate at 3 m — if a player can tell it is alive while
frozen, the mechanic is dead. Plate-fan must be a real modelled articulation, not a texture change.
Blue-white glow confined to the seam membrane material region.

---

# ASSET: LIGHT-GRAZER

## Scale and ergonomics
**Two sizes, same species:** small ~0.5 m furled, large ~2.5 m swollen with vanes spread. Four
long folding legs. Threat-height band: body→overhead depending on size. Closest approach: 1.0 m.

## State vocabulary
Ambient: drifting/grazing in darkness, slowly inflating · small furled form (opaque, wrinkled) ·
large swollen form (translucent, filaments lit) · mid-shrink (vanes half-collapsing, skin
puckering — the recoil-from-light beat) · disabled (deflated, slack on the ground, glow out).
**Counter (CI-required):** Prism Beam — light shrinks it to disable size.
**Weakened tell:** it cannot re-inflate; stays at the small furled silhouette.

## Interaction and collider envelope
Collider: capsule scaled with size state. **Sockets:** gaze (none — no eyes; use body centre) ·
weak-point ×1 (the dark core, visible through the membrane only at large size) · grab handle ×1
(leg joint, small form only) · feet ×4 · VFX mount (core) · audio mount (sac) · carry handle
(small form only).
**Variants: size is the mechanic.** Decide now — blendshape (preferred: continuous inflate) vs
separate meshes. Blendshape keeps one rig and lets the shrink be animated rather than swapped.
Grabbable: small form yes. Ragdoll: no. Pushable: yes.

## Build acceptance
Both sizes must read as unmistakably the same animal — identical leg count, vane structure, core.
The inflate must be a single continuous deformation, not a mesh swap. Translucency must survive
Quest's shader budget (see perf budget).

---

# ASSET: TETHER-SWARM

## Scale and ergonomics
5–7 bodies, each ~0.2 m, spread across a ~3 m formation; braided cord ~2.5 m. Threat-height band:
eye level (they fly). Closest approach: 0.8 m per body.

## State vocabulary
Ambient: bodies drift and re-converge, cord slackens and tautens · alert (formation tightens) ·
attack (bodies dart, cord stays anchored) · **cord severed (the whole mechanic): bodies drift
apart, filaments dark, glow drains outward from the cut** · disabled (colony settled, inert).
**Counter (CI-required):** sever the cord, not the bodies. Killing individuals does nothing.

## Interaction and collider envelope
**⚠ Architecture note: this is ONE object, not N agents.** At T2+ it must be a single instanced
cloud/particle representation, not seven creatures. **Sockets:** weak-point ×1 = **the cord
midpoint** (the only real target) · tether point (cord ends) · per-body VFX mounts · audio mount
(cord). No grab handle on bodies; grab handle ×1 on the cord.
**Separable part — THE SEVERED CORD:** two halves, each needing cap geometry at the cut.
Grabbable: cord only. Ragdoll: no. Pushable: bodies yes.

## Build acceptance
The cord must be the brightest, thickest, most deliberate element from every angle — if a player
ever shoots a body first, the read has failed. Bodies must be cheap enough to instance.

---

# ASSET: ARCHITECT LATTICE-KEEPER

## Scale and ergonomics
**4.0 m tall**, tripod stance, broad flat crown. Threat-height band: overhead. Closest approach:
1.5 m. Enters arm's reach: never — it is indifferent, not hostile.

## State vocabulary
Dormant (arms folded, channels dark — reads as a monument) · **ambient: maintenance — arms partly
unfurled, working on an unseen surface below, channels lit and flowing** · alert (crown tilts,
channels flare) · arms fully unfurled in radial symmetry (its one dramatic silhouette change) ·
disabled (legs folded, crown resting on ground, channels dark).
**Counter (CI-required):** interrupt its task — it defends its work, not itself; disable the arm
ring during the unfurl.

## Interaction and collider envelope
Colliders: crown box, torso capsule, three leg capsules. **Sockets:** gaze (crown front — it has
no eye) · weak-point ×1 (the arm-ring hub beneath the crown, exposed only when unfurled) · grab
handle: **none** (nothing on it is grabbable — deliberate) · feet ×3 · VFX mount (channel network)
· audio mount (crown) · carry handle: none.
Separable parts: none — Architect machines do not shed. Ragdoll: no. Pushable: no.

## Build acceptance
Surface must stay seamless — no rivets, panels or wear, only dust and age. Amber channels are a
single continuous inlay material region. The unfurled radial pose must be perfectly symmetrical.
Dormant pose must read as architecture at 10 m.

---

# ASSET: WAKE-GUILD SALVAGE HAULER

## Scale and ergonomics
**3.0 m tall**, four hydraulic legs, two grapple arms. Lens cluster at ~2.2 m. Threat-height band:
overhead→body. Closest approach: **1.0 m** (it grabs — this is the one machine that legitimately
comes close). Enters arm's reach: yes, via grapple arms only.

## State vocabulary
Neutral (arms folded, lenses dim) · **ambient: working — braced low, both grapple arms dragging
wreckage** · alert (rises to full height, arms unfold wide, four lenses lit red) · telegraph
(one arm cocks back, 0.6 s) · apex (grapple lunge) · whiff/recover (arm overextended, hydraulics
venting, 1.0 s) · additive flinch · stun-loop · disabled (legs splayed, body on its belly, lenses
dark, hydraulics slack).
**Counter (CI-required):** it must plant its legs to grapple — strike the exposed knee hydraulics
during the plant, or make it grab a heavy object and overbalance.

## Interaction and collider envelope
Colliders: body box, four leg capsules, two arm capsules. **Sockets:** gaze (lens cluster) ·
weak-point ×2 (knee hydraulic packs, front pair) · grab handle ×1 (arm forearm) · tether point
(body frame) · feet ×4 · VFX mounts (hydraulic vents, lens cluster) · audio mount (body) · carry
handle: none (too heavy).
Separable parts: **lens cluster** and **one grapple claw** detach as salvage (cap geometry at both
joins). Grabbable: forearm. Ragdoll: no. Pushable: no.

## Build acceptance
Must read as maintained-but-hard-used: mismatched panel repairs, leaking hoses, mud to the knee.
Anchor+cog emblem legible at 3 m. Red lenses are the only emissive.

---

# ASSET: FRACTAL-SPLITTER

## Scale and ergonomics
**Three nested sizes: 1.6 m / 0.8 m / 0.4 m.** Crystal shard cluster on a leg fringe. Threat-height
band: body→floor by size. Closest approach: 0.8 m.

## State vocabulary
**Ambient: at rest, shards rotated open like a blooming geode, core exposed and slowly pulsing** ·
alert (shards clamp into a dense defensive ball, core hidden) · **mid-split (body fracturing along
seams, two smaller copies emerging, fragments in the air)** · disabled (shards fallen loosely open,
core dark, pieces intact).
**Counter (CI-required):** stun, do not shoot. Shooting splits it — one clean disable beats ten hits.

## Interaction and collider envelope
Collider: sphere per size. **Sockets:** weak-point ×1 (the core, exposed only when bloomed open) ·
grab handle ×1 (a shard, small sizes only) · feet = leg fringe (no individual anchors) · VFX mount
(core) · audio mount (cluster).
**Variants: three sizes are separate spawns, not one scaled mesh** — the smallest must have a
proportionally larger, brighter core, so bake three meshes (or one mesh + a core blendshape).
**Separable parts:** the split fragments — decide now whether debris shards are shared props.
Grabbable: small sizes. Ragdoll: no. Pushable: yes.

## Build acceptance
All three sizes must read as identical geometry at different scale. Shard interlock must be a real
modelled seam so the split looks structural. Core is the only emissive.

---

# ASSET: BRIDGE-FORMER

## Scale and ergonomics
Central knot ~3.0 m across; six tendrils each ~4 m extended. **Bridge form spans ~8 m** and must
be genuinely walkable — deck width ≥1.0 m. Threat-height band: body. Closest approach: 1.2 m.

## State vocabulary
Hostile (tendrils raised, coiled, lashing; knot seam clenched) · **ambient: feeding — tendrils
drooping and grazing the stone, rootlets flexing, seam relaxed and breathing** · mid-splice (seam
parting, tendrils straightening and aligning) · **BRIDGE FORM (tendrils parallel, interlocked,
rigid, load-bearing)** · disabled (tendrils slack, knot closed, glow out).
**Counter (CI-required):** splice it, don't kill it — combat becomes traversal.

## Interaction and collider envelope
Colliders: knot sphere; tendrils = capsule chains. **Bridge form needs a separate authored
walkable collider** (flat box across the span) that only exists in that state — this is a
gameplay-critical, pre-mesh decision. **Sockets:** weak-point ×1 (the knot seam) · splice point ×1
(the same seam — the interaction target) · tether/anchor ×6 (root clusters) · VFX mount (seam) ·
audio mount (knot). No grab handle — it is terrain, not a prop.
Separable parts: none. Ragdoll: no. Pushable: no.

## Build acceptance
Bridge form must be **provably walkable** — the interlock detail must show a continuous top
surface with no gaps a foot could catch. Both forms must read as the same organism. Warm green
glow confined to the seam.

---

# ASSET: ORBIT-GRAZER

## Scale and ergonomics
**5.0 m mantle span**, flat disc-shaped, no legs. Vacuum/space only. Threat-height band: overhead
(it circles above). Closest approach: 2.0 m — it never approaches directly, that is the design.

## State vocabulary
Neutral (banking, mantle curled asymmetrically, filaments streaming) · **ambient: grazing — mantle
spread wide and flat, filaments fanned and drifting, intake ring dilated** · alert (mantle furled
tight and narrow, filaments retracted, presenting a thin edge) · disabled (mantle limp and folded,
filaments slack, drifting inert).
**Counter (CI-required):** break its orbit — gravity gun shove it off its arc into a surface.
Head-on shots are the wrong answer.

## Interaction and collider envelope
Collider: flattened box/disc. **Sockets:** weak-point ×1 (the intake ring, underside centre) ·
grab handle ×1 (mantle edge — the gravity-gun purchase point) · tether point (mantle centre) ·
filament roots ×N (VFX only, not colliders) · VFX mount (intake) · audio mount (mantle).
Separable parts: none. Grabbable: via gravity gun only. Ragdoll: no (zero-g drift instead).
**Zero-g note:** it has no feet and no ground contact — the rig must not assume a floor.

## Build acceptance
The permanent asymmetric curl must read as "always banking" even in a static pose. Filaments must
be cheap (cards or spline strips, not simulated). Violet intake glow only.

---

# ASSET: SALVAGE TITAN *(BOSS)*

## Scale and ergonomics
**12 m tall**, quadruped, four mismatched legs, biomechanical arm on the back. Threat-height band:
skyline. Closest approach: **3.0 m** (its feet are the only thing that ever gets near you).
Readable silhouette required at 1 km.

## State vocabulary
**Ambient: feeding — bent low, arm dragging a wrecked hull, flank plating hinged open to absorb
scrap into itself** · alert (rears to full height, arm raised, reactor flares) · telegraph (arm
cocks, 0.9 s — whole-body answer) · apex (arm sweep) · **stage-disabled (front legs buckled, body
slumped forward, arm still live above the waist)** · fully disabled (collapsed, reactor dark, arm
fallen, plating hanging open, harvestable).
**Counter (CI-required):** three-stage disable — knee hydraulics → arm pivot socket → reactor
cavity, in any order; each stage visibly reduces what it can do.

## Interaction and collider envelope
Colliders: body box, four leg capsules, arm capsule chain. **Sockets:** **weak-point ×3 —
`reactor_cavity` (chest), `knee_hydraulic_L/R` (front legs), `arm_pivot_socket` (back)** — each
must be independently targetable and visibly change state when disabled · tether point ×2 (leg
frames) · feet ×4 · VFX mounts (reactor, each weak point) · audio mount (chest) · carry handle:
none.
**Separable parts:** the arm detaches at the pivot socket (cap geometry required); flank plates
open as harvest panels. Grabbable: no. Ragdoll: no — authored collapse. Pushable: no.

## Build acceptance
Each of the three weak systems must be identifiable at 20 m without a HUD marker. Stage-disabled
pose must be stable and clearly "still fighting from the waist up." The accreted look must survive
LOD — silhouette carries it, not texture detail.

---

# ASSET: MIMIC-ECHO

## Scale and ergonomics
**2.0 m tall**, humanoid, plated. Face-plate at ~1.8 m — near adult eye level, deliberately.
Threat-height band: eye. Closest approach: **1.0 m**. Enters arm's reach: yes.

## State vocabulary
Neutral (plates hanging slack and unaligned, posture undecided, face-plate dull) · **ambient:
rehearsing — slowly cycling half-remembered borrowed poses alone, plates rippling** · copying
(locked into a crisp mimicked human stance, plates snapped tight, face-plate brightened) ·
telegraph (it mirrors YOUR last wind-up — the telegraph is the player's own motion, delayed) ·
disabled (plates collapsed loosely downward like dropped scale mail, armature slumped).
**Counter (CI-required):** feint to bait its echo, then strike off-rhythm.

## Interaction and collider envelope
Humanoid rig — **auto-rig and motion-retarget friendly, which is a production advantage; keep it.**
Colliders: standard humanoid capsules. **Sockets:** gaze (face-plate) · weak-point ×1 (the
armature visible in the plate gaps at the chest, exposed only while plates are unaligned) · grab
handle ×1 (forearm) · feet ×2 · VFX mount (face-plate) · audio mount (chest) · carry handle (torso).
Separable parts: individual plates may shed as salvage. Grabbable: forearm. Ragdoll: yes.

## Build acceptance
Plates must visibly LAG the joints they follow — that lag is the character. Must read as
"unfinished, waiting to be told a shape" in neutral. No true mirror finish (tarnished, scratched).
**⚠ Kid-comfort:** a humanoid at arm's reach — hold the 1.0 m minimum and review with a child tester.

---

# ASSET: SOUND-WALKER

## Scale and ergonomics
~1.0 m across × 0.25 m tall, disc with a bristle skirt. Ground/wall/ceiling — it traverses any
vibrating surface. Threat-height band: floor→overhead depending on surface. Closest approach: 0.8 m.

## State vocabulary
Inert (bristles collapsed flat and limp, membrane slack and wrinkled — reads as debris) ·
**ambient: listening — bristles half-raised and slowly sweeping, membrane rippling, drifting** ·
resonating (every bristle rigid and fanned, membrane taut and humming, body lifted clear on the
bristle tips) · disabled (bristles snapped inward beneath the body, membrane slack).
**Counter (CI-required):** the Sonic Thumper *silences* a patch of surface to strand it — or lure
it onto dead ground.

## Interaction and collider envelope
Collider: low cylinder. **Sockets:** weak-point ×1 (the drum membrane centre, taut only while
resonating) · grab handle ×1 (rim, inert state only) · bristle roots (VFX only) · VFX mount
(membrane) · audio mount (membrane — this creature IS an audio object) · carry handle = grab handle.
Separable parts: none. Grabbable: inert only. Ragdoll: no. Pushable: yes.
**Surface-agnostic rig:** must look correct on floor, wall and ceiling — no gravity-dependent droop.

## Build acceptance
Inert and resonating must be near-unrecognisable as the same object in silhouette. Bristles must be
cheap (cards/strips). Copper shimmer confined to the membrane material region, resonating only.

---

# ASSET: HIVE-WARDEN *(BOSS)*

## Scale and ergonomics
**8.0 m wide dome** on six slender ceramic legs; dome underside at ~3.5 m. Threat-height band:
skyline. Closest approach: **2.5 m**. It never touches the player — it dispatches.

## State vocabulary
Sealed (dome closed and seamless, legs folded high, eye dim — reads as architecture) · **ambient:
tending — dome partly open, two drones docked, fine white filament arms servicing them, unhurried**
· deploying (dome fully irised open, berths lit, drones lifting away) · **stage-disabled (three
legs buckled, dome tilted, half the berths dark, still deploying from the working side)** · fully
disabled (legs folded under, dome resting on the ground, all berths dark and open, eye black).
**Counter (CI-required):** disable berths and legs in stages — it has no attack of its own, so the
fight is against its output; closing berths reduces the drone flow.

## Interaction and collider envelope
Colliders: dome dome-primitive, six leg capsules. **Sockets:** gaze (the single eye) · **weak-point
×N = each berth iris** (individually disablable — this is the boss mechanic) · leg joints ×6
(secondary weak points) · VFX mounts (each berth) · audio mount (dome) · carry handle: none.
**Separable parts:** docked drones are separate spawned entities, not part of this mesh.
Grabbable: no. Ragdoll: no — authored collapse. Pushable: no.

## Build acceptance
Sealed pose must read as a smooth architectural dome at distance. Berth honeycomb must be a real
modelled recess array with a working iris (the sheet's detail panel is the reference). Sterile
white is the only emissive. Surface unbroken — no rust, rivets, or salvage anywhere.

---

# ASSET: TIDE-PHASE

## Scale and ergonomics
**2.0 m tall** wader on four long backward-jointed legs; body at ~1.4 m. Threat-height band: eye.
Closest approach: 1.0 m. Lives in W001's tidal flats and canals.

## State vocabulary
Solid (plates opaque, chalky, hard shadow, feet pressing mud) · **ambient: foraging — head down,
proboscis probing mud between its own feet, body rocking slowly** · phased (whole creature glassy
and semi-transparent, edges soft, almost no shadow) · **mid-phase (lower body still solid while the
upper has gone glassy — a visible waterline of substance travelling up)** · disabled (legs folded,
body settled on the mud, fully solid and inert).
**Counter (CI-required):** the world's tide cycle is the timing puzzle — strike during the solid
phase. It is untouchable while phased.

## Interaction and collider envelope
Collider: body capsule + four leg capsules — **colliders must disable in the phased state**, which
is a runtime contract the mesh must support cleanly. **Sockets:** gaze (sensory pit band) ·
weak-point ×1 (the shell seam behind the head, solid state only) · grab handle ×1 (body, solid
only) · tether point (leg joint) · feet ×4 · VFX mount (whole-body phase shader) · audio mount
(shell) · carry handle (body).
**Tell channel is the ENTIRE BODY** — the phase transition is a full-mesh material swap, so UVs and
material regions must support a single-surface transparency pass. Decide before bake.
Grabbable: solid only. Ragdoll: no. Pushable: solid only.

## Build acceptance
The mid-phase waterline must be a real animatable parameter (a height-based mask), not two meshes.
Solid state must read as chalky and mineral; phased as clouded translucent — same silhouette.

---

# ASSET: REWINDER

## Scale and ergonomics
~0.8 m long quadruped (large hare) with a heavy counterweight tail. Threat-height band: floor.
Closest approach: 0.8 m. Fast.

## State vocabulary
Still (single solid body, no trail, tail curled) · **ambient: browsing — nosing the ground in short
bursts, one faint after-shape lagging a body-length behind each burst** · moving fast (three clear
after-shapes strung behind) · **snap-back (solid body dissolving forward into a faint shape while a
previous after-shape resolves into the new solid body)** · disabled (settled, fully solid, all
after-shapes gone).
**Counter (CI-required):** anticipate the snap-back position; or scanner-tag it to lock its timeline.

## Interaction and collider envelope
Collider: capsule. **After-shapes are VFX/instanced ghost meshes, NOT colliders** — a pre-mesh
decision: the ghost is the same mesh at a lower LOD with a transparent material, so budget for
3 concurrent ghost draws. **Sockets:** gaze (sensory band) · weak-point ×1 (the sensory band
itself) · grab handle ×1 (scruff/shoulder) · feet ×4 · VFX mounts (ghost spawn point at the hips)
· audio mount (chest) · carry handle (scruff).
Separable parts: none. Grabbable: yes. Ragdoll: yes. Pushable: yes.

## Build acceptance
Ghost copies must be distinct full outlines, never motion blur. The chalky bloom sloughing backward
must be a real particle/material effect anchored to a socket. No coloured glow anywhere.

---

# ASSET: INVERTER

## Scale and ergonomics
**1.5 m disc diameter × 0.5 m thick**, eight short gripping limbs. Operates on floor AND ceiling.
Threat-height band: floor→overhead. Closest approach: 1.2 m.

## State vocabulary
Floor-mounted (limbs braced down, upper ring dim) · **ambient: grazing the ceiling — hanging
inverted overhead, limbs gripping up, moving slowly across a slab, one ring lit** · mid-flip
(detached, rotating in mid-air, limbs retracted, both rings lit) · disabled (limbs folded flat
beneath the disc, both rings dark, resting).
**Counter (CI-required):** disable it fast during the flip window, or use its own gravity field to
reach it. **⛔ Report-only:** any player-inversion is comfort-gated — likely a "fight it on the
ceiling while you stay on the floor" encounter. Runtime decision, not an art one.

## Interaction and collider envelope
Collider: cylinder. **The rig must be genuinely reversible** — no gravity-dependent droop, no
"up" assumption anywhere in the skeleton. **Sockets:** weak-point ×2 (both ring recesses — one is
always facing away, which is the fight) · grab handle ×1 (disc rim) · limbs ×8 · VFX mounts (both
rings) · audio mount (disc centre) · carry handle = grab handle.
Separable parts: none. Grabbable: yes. Ragdoll: no. Pushable: yes.

## Build acceptance
Top and bottom faces must be near-identical — flip the model and it should look correct. Indigo
glow confined to the two ring recesses. Reads as geological mass, not machinery.

---

# ASSET: MIMIC VAULT *(BOSS / AMBUSH)*

## Scale and ergonomics
Closed: **2.4 m × 2.4 m × 5.0 m** — the exact footprint of a real cargo container in the world.
Open: ~5 m tall radial maw on six limbs. Threat-height band: body→overhead. Closest approach:
**player will walk right up to it — that is the trap**; the opening must not clip the headset.

## State vocabulary
Closed (reads as an ordinary cargo vault; tells visible only on close inspection) · **ambient:
luring — hatch cracked open with a warm glow spilling out, genuine salvage arranged in front** ·
mid-open (four seams splitting, limbs unfolding from beneath) · fully open (wide radial maw,
limbs braced) · disabled (plates fallen loosely outward, limbs splayed, glow dead, harvestable).
**Counter (CI-required):** read the tells before you reach in — too-rounded corners, one breathing
seam, salvage arranged too neatly. Recognition IS the counter.

## Interaction and collider envelope
Collider: closed = box (must match real container dimensions exactly, so it groups with them);
open = maw + six limb capsules. **Sockets:** weak-point ×1 (the interior throat, open only) ·
grab handle ×1 (a corner fitting — the lure) · limbs ×6 · VFX mount (hatch glow, then throat) ·
audio mount (body) · carry handle: none.
**⚠ Opening envelope:** the unfold must be authored so no plate sweeps through the player's head
position at 1 m. Pre-mesh constraint.
Separable parts: shell plates as salvage. Grabbable: corner fitting. Ragdoll: no.

## Build acceptance
Closed form must be indistinguishable from the world's real containers at 5 m and only subtly
wrong at 1 m. Interior must be structural, not fleshy — no gore, family-appropriate startle.

---

# ASSET: TIDE-KITE

## Scale and ergonomics
**3.0 m wingspan**, slender spar body, two grasping limbs. Aerial only. Threat-height band:
overhead. Closest approach: **1.5 m at the bottom of a dive** — it snatches and climbs, never hovers.

## State vocabulary
Gliding (wing taut and flat, limbs tucked) · **ambient: circling — wing angled in a lazy bank,
limbs trailing, riding a thermal** · dive (wing swept into a narrow dart, limbs extended, hooks
open — committed and unmistakable) · carrying (climbing away with salvage in both hooks, wing
beating) · disabled (wing collapsed and folded over the spar, limbs curled, grounded but unhurt).
**Counter (CI-required):** it steals rather than harms — net it, or recover the item it took. A
chase, not a fight.

## Interaction and collider envelope
Collider: flattened capsule along the spar. **Sockets:** gaze (sensory nub) · weak-point ×1 (the
wing root where membrane meets spar) · grab handle ×1 (spar body, grounded only) · **carry socket
×1 between the two hooks — the stolen item attaches here** (gameplay-critical, pre-mesh) · VFX
mount (wing tips) · audio mount (spar) · carry handle = grab handle.
Separable parts: none. Grabbable: grounded only. Ragdoll: yes (grounded). Pushable: no.

## Build acceptance
Dive silhouette must be unmistakable **from directly below**, since that is the player's view.
Wing membrane needs backlit translucency. The carry socket must hold a salvage prop without
clipping the wings.

---

# ASSET: ARCHITECT SEAL-WARD

## Scale and ergonomics
Dormant: a **2.5 m circular medallion flush in a wall**. Deployed: ~3 m wheel extended ~1.5 m from
the wall on four braced legs. Threat-height band: eye→overhead. Closest approach: **1.0 m** — it
guards a doorway the player must pass.

## State vocabulary
Dormant (flush, seamless, channels dark — reads as decorative architecture) · **ambient: idling —
the outermost ring alone rotating slowly in place, channels dimly lit, the rest still asleep** ·
mid-deploy (rings separating and extending forward, legs unfolding) · fully deployed (all rings
extended and counter-rotating, braced, channels blazing amber) · disabled (rings collapsed
together at an angle, half-retracted, channels dark).
**Counter (CI-required):** it denies passage but never pursues — disable the ring hub during a
rotation gap, or find another route. Leaving is always a valid answer.

## Interaction and collider envelope
Colliders: wall-plane box (dormant); ring discs + four leg capsules (deployed). **Sockets:**
weak-point ×1 (the central hub, reachable only when the rings separate) · grab handle: none ·
wall anchors ×4 · VFX mount (channel network) · audio mount (hub) · carry handle: none.
**⚠ It is wall-mounted** — the mesh must author its own wall interface, and placement is a scene
socket, not a floor spawn. Pre-mesh constraint.
Separable parts: none. Grabbable: no. Ragdoll: no. Pushable: no.

## Build acceptance
Dormant state must be indistinguishable from architecture at 5 m. Ring rotation must be a real
modelled mechanism with concentric separation. Amber channels are one continuous inlay region.

---
---

# PART 3 — FLORA PASSPORTS

Plants use the same four headings. **The pre-mesh decision unique to plants: growth stages are
separate meshes or blendshapes — decide before generating.** Recommended: separate meshes for
seed/sprout/young/mature (silhouettes differ too much for a blendshape), plus the harvested output
as its own independent prop mesh.

---

# ASSET: DEW-BULB

## Scale and ergonomics
Mature ~0.5 m tall (knee-high). Grown in a plot; the harvestable bulb is fist-sized (~0.12 m).
Reach: the bulb must sit **0.35–1.0 m** above the plot surface so a seated or child player can pick
it without stretching.

## State vocabulary
Four growth stages: seed (ridged capsule, thumbnail) → sprout (two folded leaf-cups) → young
(leaves open, small cloudy bulb) → mature (full form, bulb swollen and clear, condensation beading
in the leaf channels). Ambient motion: **slow leaf sway + condensation drip** — vertex-shader only,
zero CPU. Harvested: the plant remains with an empty stem and regrows.

## Interaction and collider envelope
Colliders: base cylinder only (leaves non-colliding). **Sockets:** `harvest_point` ×1 (the bulb
attachment — the grab target) · `plot_anchor` ×1 (base) · VFX mount (leaf channel, for drip) ·
audio mount (base).
**Separable part — THE BULB:** its own mesh, its own collider, a carryable inventory prop. Needs
**cap geometry at the stem attachment**. Post-harvest plant silhouette must still read as a
dew-bulb.
Grabbable: bulb only. Ragdoll: no.

## Build acceptance
The four stages must read as one species. The bulb must be visibly heavy enough to bow the stem in
the mature stage. Translucent bulb must survive the Quest shader budget. Cool blue shimmer confined
to the bulb interior.

---

# ASSET: RUST-FERN

## Scale and ergonomics
Mature ~0.9 m tall (waist-high), fronds arching to ~0.8 m radius. Harvested frond ~0.6 m — a
carryable component. Reach: frond bases at **0.4–0.9 m**.

## State vocabulary
Stages: spore-case (dark hard nodule) → sprout (single curled fiddlehead, already metallic at the
tip) → young (2–3 short fronds partly uncurled) → mature (full arching crown, leaflets fully
oxidised). Ambient motion: **stiff fronds barely move** — a slow metallic shimmer/parallax rather
than sway. Harvested: cut fronds regrow from the crown.

## Interaction and collider envelope
Colliders: crown cylinder only. **Sockets:** `harvest_point` ×N (one per frond base) ·
`plot_anchor` ×1 · VFX mount (leaflet surface, for the conductive spark effect when used) · audio
mount (crown).
**Separable part — A CUT FROND:** own mesh, own collider, carryable. Cap geometry at the cut.
**Trait hook:** this is the conductive crop feeding static equipment — the leaflet surface needs
its own material region so a charged state can light it.
Grabbable: fronds. Ragdoll: no.

## Build acceptance
Leaflets must read as genuine oxidised metal (rust-orange with blue-violet temper edges), not
painted leaves. Fiddlehead stage must already show the metallic tip. No emissive by default.

---

# ASSET: SLUDGE-MELON

## Scale and ergonomics
Mature vine sprawls **~2.0 m across**, low to the ground. Fruit ~0.45 m diameter (beach-ball) and
**heavy — a two-handed carry**. Reach: fruit sits on the ground, so the pick-up is a crouch/bend —
verify against seated-mode accessibility.

## State vocabulary
Stages: seed (large flat pale seed) → sprout (paired leaves on a short runner) → young (spreading
vine, fist-sized fruit, one band) → mature (full fruit, all bands, vine sagging under the weight).
Ambient motion: **leaf sway only**. Harvested: vine remains, fruit detaches.

## Interaction and collider envelope
Colliders: fruit sphere (the only collider); vine non-colliding. **Sockets:** `harvest_point` ×1
(fruit stem join) · `plot_anchor` ×1 · **two-handed grab handles ×2 on opposing sides of the
fruit** (it is a two-hand carry — pre-mesh constraint) · VFX mount (cut face) · audio mount (fruit).
**Separable parts — THE FRUIT, plus a CUT-OPEN variant** showing pale flesh and the dark
concentrated core. Both need their own meshes; the cut variant needs an interior.
Grabbable: fruit, two-handed. Ragdoll: fruit yes (it should roll).

## Build acceptance
The rind banding must legibly record what it filtered — silt-dark at the base grading to pale cream
at the top. Cut-open interior must be structural and appetising-adjacent, never organ-like. Warm
yellow-green confined to the cut core.
