# Quest checkpoint findings — 2026-07-19

**Status:** FAIL — recovery remains open.  
**Tested artifact:** previously authorized Golden APK from source `2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295`.  
**Evidence:** Terry's headset observations, screenshots, and `quest_checkpoint_20260719_122154.log`.

## Confirmed blocker

The bounded route could not leave W000. The gate coupler reached `panel_off` and `part_seated`, but never emitted `MACHINE_REPAIRED`; `ShipCastOffRuntime` continued to report the coupler as unrepaired and blocked PUNCH IT.

Root cause in source:

- the prominent red object at the top was a collider-free status lamp, not the final control;
- the actual power switch was a small side control at local height 1.1 m and was not clearly distinguished from the lamp;
- the interaction did not meet the family/child reach contract.

## Additional Quest defects in the same bounded surface

1. Forged pistol/taser bodies were extremely small because the metre-authored Forge mesh inherited the ItemFactory primitive root scale and was scaled a second time.
2. Guns pointed upward in hand because the attach transform used a +45-degree local X rotation. Forge Grip sockets repeated the same rejected rotation.
3. Breaker Blade and Tide Pike used separate guessed positive-X grip angles and also failed the held-forward contract.
4. Coupler access-panel/replacement-part grabs logged XRI warnings about throwing kinematic rigidbodies.

## Authorized correction scope

Only the following recovery work is authorized before the unchanged route is retried:

- normalize Forge item root scale while preserving the primitive dimensions as collider dimensions;
- enforce item local +Z as the canonical held-forward axis for guns and melee;
- tune the demonstrated weapon families into bounded human-scale envelopes;
- replace the ambiguous lamp/switch presentation with one distinct, centered, child-reachable final power target;
- remove the demonstrated kinematic-grab warnings;
- add regression coverage without weakening or increasing the locked 43-test Recovery PlayMode suite;
- produce a new exact-SHA Golden APK and checkpoint record after every automated lane is green.

No unrelated feature, world, multiplayer, progression, or broad art work is authorized by this failure packet.
