# Controls Framework — On-foot (Xbox-like) + Space Flight

Target feel: an Xbox-style shooter, mapped to Meta Quest controllers. Aiming follows the **Quest
shooter standard**: physical iron-sights (raise the gun to your eye to aim precisely), not an
artificial button-zoom — that's what RE4 VR / Onward / Pop:One do and it avoids nausea. Decisions:
2026-06-15. Current bindings live in `DashLocomotion.cs`, the XRI move/turn providers, and the
weapon scripts; new bindings should go through a single `InputRouter` + `LocomotionProfile` so it
stays data-driven.

---

## On-foot mapping (the Xbox parallel)
| Quest input | Action | Xbox equivalent | Status |
|---|---|---|---|
| Left stick | Move (analog: tilt = walk, full = jog) | Left stick | ✅ exists |
| Left stick click (L3) | Sprint (hold) | L3 sprint | ✅ exists |
| Right stick | Turn — smooth or snap (comfort setting) | Right stick (look) | ✅ exists |
| A (right primary) | Jump | A | ✅ exists |
| Right trigger | Fire (held gun) | RT | ✅ exists |
| Grip (either) | Grab / holster / use | grab | ✅ exists |
| **Raise gun to eye** | **Aim down sights / steady** | LT aim (analog) | ➕ add |
| Off-hand grip on a 2-handed gun | Two-handed steady (less sway) | — | ➕ add |
| B (right secondary) | Reload / eject | X (reload) | ➕ assign |
| X (left secondary) | Crouch toggle | B (crouch) | ➕ assign |
| Y (left primary) | Use / interact / objective ping | Y | ➕ assign |
| R3 (right stick click) | Recenter / quick-melee | R3 | ➕ assign |
| Both grips held 1s | Emergency respawn | — | ✅ exists |

**Walk vs run:** keep analog speed on the stick (gentle = walk) **and** L3 to sprint — matches
console muscle memory. Expose `walkSpeed`, `runMultiplier` on `LocomotionProfile` so it's tunable
without code.

**Aiming (the "zoom"):** the standard Quest shooter approach — bring the weapon up to your view to
look down its iron sights; **scoped weapons magnify** when the scope is near your eye. Add a subtle
**steadying** (reduced sway / slight slow) when the gun is within ~20cm of the head camera, so it
*feels* like ADS without an artificial zoom. Two-handed grip on long guns further steadies. This is
the modern Quest standard and reads as "aim" to console players without motion sickness.

**Comfort settings (ship with these):** snap-turn vs smooth-turn, turn speed/snap angle, vignette on
move, sprint-as-toggle-vs-hold. All on `LocomotionProfile` (already has turnMode/speeds).

## Where it goes in code (data-driven)
- Movement/turn: existing XRI providers + `LocomotionProfile` (extend with walk/run/comfort fields).
- Jump/sprint + keep-actions-live: `DashLocomotion.cs` (already the persistent-rig input owner).
- New buttons (reload/crouch/use/melee) + ADS steadying: a small **`InputRouter`** MonoBehaviour on
  the rig that reads the XRI Default Input Actions and raises events weapons/systems subscribe to —
  one place to see/rebind everything (don't scatter `InputAction` literals across scripts).
- Weapons subscribe to fire/reload/ADS events instead of polling triggers themselves.

## Space flight (scaffold now, short tutorial hop soon)
A short guided flight is part of the opening tutorial (teaser: "you can fly, not just walk/shoot").
Lives in the empty **`Ziptide.Ship`** assembly. Arcade-simple, VR-comfortable (seated cockpit frame
to fight nausea — the cockpit gives the inner ear a stable reference).

| Quest input | Flight action | Xbox-flight equivalent |
|---|---|---|
| Left stick Y / triggers | Throttle (fwd/back) or boost | LT/RT throttle |
| Left stick X | Strafe / yaw-assist | — |
| Right stick | Pitch + yaw (steer) | Right stick |
| Grips or X/B | Roll left/right | bumpers roll |
| Right trigger | Fire ship weapon | RT |
| A | Boost | A |

**Tutorial flight task:** a short on-rails-ish hop — fly through a few guided rings / to a landing
marker — enough to teach throttle + steer + boost and tease the mechanic before the first ziptide.
Keep it forgiving (auto-level assist, generous collision) for a first-time VR flyer.

**Scaffold only for now:** create `Ziptide.Ship/Runtime/ShipController.cs` (arcade flight + cockpit
rig) + a `FlightTutorial` task hook in the level. Full flight combat is a later milestone.

## Build order (AFTER CI is green so we can compile-verify)
1. `InputRouter` + extend `LocomotionProfile` (walk/run/comfort) — no behavior change yet.
2. ADS steadying + iron-sight readiness on weapons (verify "aim feels good" on device).
3. Assign reload/crouch/use/melee to X/Y/B/R3.
4. `Ziptide.Ship` `ShipController` arcade scaffold + cockpit rig.
5. Wire the short flight tutorial task into Level 1.
Each step = one small commit, CI-compiled, then on-device check.
