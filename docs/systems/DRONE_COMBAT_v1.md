# Drone Combat v1 — movement, attack, and "you got hit" response

**Plan to turn the current drone (a hovering target that only reacts to *being* shot) into an active
enemy: it moves around you, telegraphs, fires a stunner at you, and hitting you does something
readable and recoverable.** Builds on the existing `DroneRuntime` / `IShockable` / `HitZones`
(see [`CREATURE_DRONE.md`](CREATURE_DRONE.md)).

> **Order of operations (per Terry):** ship the current bug fixes first → drop in the real gun model
> ([`ASSET_SWAP_PIPELINE.md`](ASSET_SWAP_PIPELINE.md)) → THEN add this drone combat in the Sandbox.
> The first real level does **not** need this on day one; the Sandbox is where we practice the loop.
> This is the second half of the reusable **enemy authoring loop** (see end).

---

## 1. The encounter we want (design)

A drone (or a few) **circles the player**, keeps its distance, occasionally **winds up and fires a
slow stun bolt** you can **dodge or block**. Get tagged once → your view is partly obscured + you're
briefly sluggish. Get tagged again while impaired → a bigger penalty (brief full stun / forced
"shake it off"). You clear drones with the taser/gravity gun using the existing shock-then-down kill.

Design rules that keep it fun + comfortable in VR:
- **Readable telegraph before every shot** (wind-up glow + audio) so a miss feels fair.
- **Slow, visible projectiles** you can physically dodge (lean/duck) or block — never hitscan at you.
- **Non-lethal, recoverable** hits (family-friendly): impair + recover, no death screen.
- **Comfort first:** the screen effect is a *partial vignette / splat*, never a full blackout or
  anything that yanks the camera (no forced rotation/translation — that causes nausea).

---

## 2. Movement — "orbit + strafe + reposition"

Standard flying-enemy pattern (orbit its target, strafe, reposition between attacks). State machine on
the drone (`DroneState`):

| State | Behavior |
|-------|----------|
| **Idle/Patrol** | bob at home point until the player is detected (range/LOS). |
| **Orbit** | circle the player at radius **~4–6 m**, height **~2–3 m**, slow lateral drift; keep facing the player. |
| **Reposition** | every few seconds pick a new angle/height and slide there (so it's not a fixed ring). |
| **Telegraph** | stop strafing, face player, **wind-up** (glow + charge SFX, ~0.6–1.0 s). |
| **Fire** | launch one slow stun bolt at the player's head/chest; return to Orbit. |
| **Shock/Down** | existing `DroneRuntime` shock-then-zone-death when the *player* hits *it* (unchanged). |

Implementation notes:
- Move kinematically (lerp/`MoveTowards` toward a target point each frame); no NavMesh needed for a
  flyer. Clamp speed; keep motion smooth (drones snapping around reads bad and is hard to dodge).
- Multiple drones: **stagger their fire timers** and **spread their orbit angles** so you're not hit
  by a wall of bolts at once. A simple shared "attack token" (only N drones may be in Telegraph at
  once) keeps it dodgeable.
- All numbers (radius, height, orbitSpeed, repositionInterval, telegraphTime, fireCooldown,
  projectileSpeed, detectRange) are **tunable fields** now → promote to `CreatureDefinition` /
  `DroneVariantDefinition` later (data-driven variants, matches the `Definition` pattern).

---

## 3. Attack — the stun bolt

- New `StunBoltProjectile` (mirror the existing `TaserDartProjectile`, but enemy→player): slow,
  glowing, clear arc, lifetime cleanup, audio.
- On hit **the player** (not a drone): raise the player's **stun stacks** by 1 and trigger the screen
  response (§4). The bolt should be **blockable** — if it hits a held gun/shield collider, it fizzles
  (rewards using your gear defensively).
- Keep it **non-lethal**: it impairs, it doesn't damage health (no health system needed for v1).

---

## 4. "You got hit" response — screen feedback + recovery

A single reusable **`PlayerStunState`** on the rig (camera-mounted), driven by stun stacks. Standard
VR-comfort technique is a **post-process / camera-space overlay** (vignette + splat), faded in/out at
runtime — *not* a hard cut, and **no camera movement**.

| Stacks | Effect | Recovery |
|--------|--------|----------|
| **1 hit** | **partial screen obscure**: a vignette darkening the edges + a translucent "static/goo splat" overlay over part of the view; movement speed **~50%** for a short window. | auto-fades over **~3–4 s**; or shake it off (wave hand across face / both-grip) to clear faster. |
| **2+ hits (while still impaired)** | **heavy obscure** (most of the view fogged for ~1 s) + **brief full stun** (can't move/aim ~1 s), then drops back to the level-1 state. | recovers automatically; never a death/respawn in v1. |

Implementation:
- A **world-space quad / canvas parented to the camera** with a transparent material whose alpha +
  texture you lerp, OR a URP **full-screen vignette** driven at runtime. Both are cheap on Quest.
  Keep peripheral-only when possible (center stays clearer = less nausea, still "I'm hit").
- `PlayerStunState` owns: `currentStacks`, timers, `OnStunChanged` event (so the HUD / audio / move
  provider all react from one source — avoids competing effects, a known VR overlay pitfall).
- Hook the **move provider** to read a `speedMultiplier` from `PlayerStunState` (slow while impaired).
- Audio: muffled/ringing SFX on hit, clears as it fades.

**Comfort guardrails:** never fully black the screen for more than a flash; never rotate/translate the
camera as "knockback"; keep the heavy state short; make it obvious it's *recovering* (the fade tells
the player "you're getting better"). All durations tunable.

---

## 5. What already exists vs. new (for whoever builds it)

- **Exists (reuse):** `DroneRuntime` (hover + shock-then-down), `IShockable`, `HitZones`,
  `TaserDartProjectile` (mirror for the enemy bolt), `OnDroneDisabled` job hook, the Sandbox arena.
- **New (T-Dog runtime lane):** `DroneState` movement SM, telegraph VFX, `StunBoltProjectile`
  (enemy→player), `PlayerStunState` + the screen overlay, move-provider speed hook.
- **New (Architect data lane):** `CreatureDefinition`/`DroneVariantDefinition` fields for the tunables
  above (orbit/attack/stun numbers, `hitsToDown`, detect range) so variants are data, not code. This
  is the data half of the **Creatures v1** claim.

---

## 6. The reusable Enemy Authoring Loop (the process to nail)

This drone is the **template**. Every future enemy follows the same five beats — that's the workflow
we're standardizing:

1. **Visualization** — model in via [`ASSET_SWAP_PIPELINE.md`](ASSET_SWAP_PIPELINE.md) (Tripo → GLB →
   prefab → swap the placeholder, keep the runtime).
2. **Movement** — a `*State` machine (patrol → engage → reposition), kinematic for flyers, tunable.
3. **Attack** — a telegraph + a slow, dodgeable/blockable projectile or melee tell.
4. **Response to attack** — what it does to the player (impair via `PlayerStunState`) and what the
   player's hit does to *it* (existing `HitZones` shock-then-down).
5. **Data** — promote the tunables to a `Definition` so new variants = a new asset, not new code.

Prove all five **in the Sandbox** with the drone first; then reuse the beats for the next creature and
start dropping enemies into real worlds.

## Links
[`CREATURE_DRONE.md`](CREATURE_DRONE.md), [`CREATURES.md`](CREATURES.md),
[`ASSET_SWAP_PIPELINE.md`](ASSET_SWAP_PIPELINE.md), `docs/design/SYSTEMS_ARCHITECTURE.md`,
`docs/09_GEAR_AND_TOOLS.md` (the gear that fights them).

Sources for the patterns:
- Flying-enemy orbit/strafe/reposition AI: https://forums.unrealengine.com/t/how-to-set-up-ai-navigation-for-a-flying-drone-helicopter-type-enemy/149527
- Dodge/block incoming projectiles as a core VR mechanic: https://www.thevrgrid.com/drone-striker/
- VR damage/comfort overlays (runtime vignette, peripheral obscure, single control source): https://discussions.unity.com/t/implementing-vignette-for-stereoscopic-vr/670909 , https://cesium.com/learn/unreal/vr-vignettes/
