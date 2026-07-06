# CONTROL SCHEME — the Fortnite-class verb set, VR-ized (Terry's directive 2026-07-06)

**The law:** every verb a modern console shooter gives the player exists here, mapped to Quest
controllers, DATA-DRIVEN through `LocomotionProfile`/`ItemDefinition` so feel-tuning never needs
code. This table is the spec; each ⬜ row is a one-commit envelope any operator can implement.
Runtime owner for body verbs: `Gameplay/Runtime/Locomotion/DashLocomotion.cs` (owns the
CharacterController + its own InputActions — add verbs THERE, the pattern is established).
Item verbs: `Gameplay/Runtime/Items/ItemFactory.cs` + per-weapon runtimes.

| Fortnite verb | Quest mapping | Status | Seam / notes |
|---|---|---|---|
| Move / strafe | LEFT stick | ✅ | `ActionBasedContinuousMoveProvider`, profile `moveSpeed` (3 m/s) |
| Sprint | HOLD/CLICK left stick (L3) | ✅ `2fd06ab` | profile `sprintMultiplier` (2× → 6 m/s), logs `LOCO_STATE sprint=` |
| Auto-run | DOUBLE-CLICK L3 inside window | ✅ S2 | profile `autoRunDoubleTapWindow`; any stick input/jump cancels |
| Jump | A (right primary) | ✅ | `DashLocomotion.HandleJump`, grounded-checked |
| Crouch | R3 (right-stick click) toggle | ✅ S2 | CC height 1.8→1.0 + speed × `crouchSpeedFactor`; logs `LOCO_STATE crouch=` |
| Slide | crouch WHILE sprinting | ✅ S2 | `slideBoost` decaying over `slideSeconds`, ends crouched |
| Turn | RIGHT stick (smooth/snap per profile) | ✅ | `LocomotionProfile.turnMode` |
| Aim (reticle) | laser sight on every gun | ✅ S3 | `GunLaserSight`, `ItemDefinition.laserSightColor` |
| ADS / zoom | bring gun to eye OR half-trigger → 1.5× vignette | ⬜ envelope | comfort-gated; do AFTER laser sights prove out |
| Fire | trigger of the holding hand | ✅ | per-weapon runtimes |
| Reload | flick-down gesture w/ held gun | ⬜ envelope | weapons are cooldown-based today; arrives with ammo design |
| Weapon slots | belt holsters (physical) | ✅ | `BeltRig` + `HolsterSocketInteractor` |
| Quick-swap | B (right secondary) hand ⇄ last holster | ✅ S4 | `QuickSwap` on rig; belt IS the state; logs `QUICK_SWAP` |
| Ping / marker | LEFT trigger (empty hand): point + release | ✅ S5 | drops a 20 s `ObjectiveBeacon`; logs `PING_AT`; co-op ready |
| Interact / pickup | grab (grip) / poke-select | ✅ | XRI grab + `XRSimpleInteractable` seams |
| Build (Fortnite build mode) | — deliberately N/A | 🚫 | Ziptide's build verb is BuildSockets (M2 repair/build), not free-building |
| Inventory / map | Y+B dev menu today; player menu later | ⬜ envelope | reuse DevMenu canvas idiom at player scope |
| Emote | — | ⬜ later | after A6 multiplayer avatars |

**Comfort rules (non-negotiable):** all camera motion is player-initiated; slides are short; ADS
zoom (when built) must be vignetted; nothing ever rotates the camera except the turn provider.

**Tuning file:** `Content/Locomotion/DefaultLocomotionProfile.asset` — every number above lives
there. Terry feedback loop: change asset values, no code.
