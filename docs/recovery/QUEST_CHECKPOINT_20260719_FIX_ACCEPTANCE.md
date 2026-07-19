# Quest checkpoint correction acceptance — 2026-07-19

This file defines the exact acceptance boundary for the retry candidate produced from the failed 2026-07-19 Quest checkpoint.

## Automated requirements

The candidate source must pass, on one exact SHA:

1. ordinary EditMode CI;
2. patch-scenes + world audit;
3. the unchanged 43-test Recovery PlayMode suite;
4. locked Golden Android build for `_Boot`, `W000_DriftIn`, and `ToxicCity`;
5. artifact/hash review before installation.

The regression surface must prove:

- every existing ranged/melee ItemFactory family exposes an identity Grip rotation so local +Z remains the held-forward axis;
- the real forged pistol has a visible largest dimension between 0.15 m and 0.45 m;
- Forge normalization transfers the primitive shell dimensions into the BoxCollider, leaves a unit-scale root, and is idempotent;
- the coupler's state indicator has no collider and cannot be mistaken for the control;
- `PowerSwitch_PRESS` is centred, labelled, hidden until the part seats, and remains entirely below 0.95 m local height;
- panel and replacement-part rigidbodies enter XRI as constrained dynamic bodies rather than kinematic throw bodies.

## Quest retry requirements

Run the same bounded route:

`cold boot -> NEW GAME -> W000 -> repair gate coupler -> PUNCH IT -> ToxicCity -> W000 -> save/relaunch/continue`

At W000, before launch:

- grab the pistol and taser; both must be clearly hand-sized and point along the controller/index-finger direction rather than upward;
- grab the Breaker Blade and Tide Pike if exposed by the candidate; each must extend forward from the hand without the prior raised/aimed angle;
- pull the panel, seat the replacement part, and verify the only illuminated final control is the large labelled front switch;
- a child-height reach must be able to select the full target without reaching above shoulder/head height;
- pressing the switch must turn the indicator green, emit `MACHINE_REPAIRED`, arm PUNCH IT, and allow exactly one canonical departure;
- the log must contain no XRI warning about throwing the coupler panel or replacement part while kinematic.

Any failure keeps recovery open. No unrelated exploration replaces these checks.
