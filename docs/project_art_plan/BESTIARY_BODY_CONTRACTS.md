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

1. Save the three sheets into `concepts/bestiary_ch1/` as:
   `husk_molter_sheet_v1.png` · `dredge_bull_sheet_v1.png` · `warden_sentinel_sheet_v1.png`
2. Commit them.
3. Add three `assets` entries to `docs/project_art_plan/concept_intake_manifest.json` with
   `status: "intake-ready"`, `specPath: "docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md"`,
   `conceptPaths` pointing at the committed PNGs, and `requiredMarkers` copied from the marker
   list in §1 below.
4. Run `python3 tools/concept_intake_gate.py` — it must pass before pushing.

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
