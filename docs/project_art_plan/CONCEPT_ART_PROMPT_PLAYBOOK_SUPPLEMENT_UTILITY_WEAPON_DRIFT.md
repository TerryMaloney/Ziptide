# CONCEPT-ART PROMPT PLAYBOOK SUPPLEMENT — UTILITY VERSUS WEAPON DRIFT

**Status:** binding supplement to `CONCEPT_ART_PROMPT_PLAYBOOK.md` for non-weapon tools, repair equipment, scanners, bridge modules, biological utilities, and other held or belt-mounted props. Docs-only and freeze-compatible.

**Origin:** Circuit Bridge Mk I first-pass failure. The written prompt repeatedly excluded guns and tasers, but the requested forward contacts, one-hand operation, side lever, and active electrical effect still combined into weapon grammar.

## 1. Core law

> A negative instruction cannot rescue a design whose positive geometry, pose, and effect already describe a weapon.

For a non-weapon utility, the prompt must positively prescribe a non-weapon interaction—not merely say `no gun`.

## 2. Five drift channels

Evaluate these independently before keeper selection.

### 2.1 Silhouette drift

Weapon cues include:

- long front axis;
- barrel or muzzle;
- paired forward prongs;
- pistol grip;
- trigger position;
- stock or shoulder brace;
- sight rail;
- pointed front face;
- dominant forward direction.

Corrections include:

- broad/flat body;
- symmetric or side-oriented interaction;
- rear docking face;
- top lever;
- clamp, cradle, collar, cuff, patch, pad, plate, frame, or box silhouette;
- local attachment rather than projected action.

### 2.2 Pose drift

Weapon cues include:

- one hand pointing at a target;
- two-handed aiming stance;
- arms extended toward threat;
- device aligned with camera like a first-person gun;
- second hand supporting the front.

Corrections include:

- place, dock, seat, wrap, press, latch, brace, scan, unfold, or crank;
- show the tool attached to environment;
- show the player's hands performing a different task after attachment;
- use side-on or close interaction framing instead of combat framing.

### 2.3 Effect drift

Weapon cues include:

- projectile;
- visible arc across open air;
- beam leaving the tool;
- muzzle flash;
- impact burst;
- target hit reaction;
- combat-scale electrical discharge.

Corrections include:

- internal state movement;
- deformation under load;
- analog needle response;
- mechanical latch position;
- environmental flow change;
- restrained effect contained inside guarded parts;
- target mechanism operating rather than being struck.

### 2.4 Control drift

Weapon cues include:

- index-finger trigger;
- pistol grip plus thumb safety;
- fire/select switch;
- recoil brace;
- charge meter beside a muzzle.

Corrections include:

- top lever;
- two-handed crank;
- broad palm latch;
- pull ring;
- guarded commit bar;
- rotary valve;
- fold-over handle;
- separate cancel/release control.

### 2.5 Verb drift

High-risk verbs:

- fire;
- shoot;
- aim;
- target;
- zap;
- stun;
- blast;
- discharge at;
- lock on.

Preferred utility verbs:

- dock;
- bridge;
- route;
- seat;
- brace;
- filter;
- stabilize;
- clamp;
- couple;
- drain;
- scan;
- align;
- connect;
- release.

## 3. Pre-generation utility test

Before writing the natural-language prompt, answer:

1. What physical surface receives the tool?
2. Does the tool launch anything away from itself?
3. What prevents it from being aimed?
4. What happens to the player's hands after activation?
5. What mechanical state communicates success?
6. Can the function be shown without a target being struck?
7. Would a child call the silhouette a gun?
8. Does the active effect remain inside the tool or attachment path?

If the answers still imply aiming, firing, forward prongs, or open-air effect, redesign the interaction before generation.

## 4. Keeper gate for non-weapon utilities

A non-weapon utility automatically fails keeper status when any of these are true:

- dominant silhouette resembles a gun at thumbnail size;
- player-eye scene uses an aiming pose;
- active effect leaves the device like a projectile or arc;
- controls resemble a trigger and pistol grip;
- the prompt claims local utility while the scene depicts remote action;
- the device must remain held like a weapon throughout use;
- a six-year-old describes the object primarily as a gun, blaster, taser, or launcher.

A strong component idea may still be preserved as an exploration reference.

## 5. Positive prompt pattern

Use this structure:

```text
The device is a [broad/flat/local] utility that [docks/seats/clamps] onto [specific surface].
Its [contacts/pad/socket] face [rear/side/downward], never forward like a muzzle.
The player activates it using [top lever/broad latch/crank].
The device remains physically attached while [mechanism/environment] responds.
The player's hands are then free to [actual task].
The active state is communicated by [internal route/deformation/gauge/mechanical state], not by a beam or projectile.
```

## 6. Reference authority split

When a first pass contains one useful feature but fails weapon drift:

- preserve the useful component, material, or internal anatomy;
- reject the overall silhouette and use pose;
- do not label the whole image a near-keeper;
- write a new positive interaction contract;
- allow one controlled redesign;
- park the asset if weapon grammar persists.

## 7. Meta-pipeline extraction

Future structured prompt records should add:

```yaml
non_weapon_utility_check:
  dominant_axis: local | forward
  active_pose: docked | held-neutral | aimed
  effect_path: internal | contact-contained | open-air
  primary_control: lever | latch | crank | trigger-like
  hands_after_commit: free | one-free | occupied
  child_thumbnail_read: utility | ambiguous | weapon
```

This data can become a generator-independent lint step before prompt submission.