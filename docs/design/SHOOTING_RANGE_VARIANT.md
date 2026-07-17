# SHOOTING RANGE VARIANT — RILL PROVING GROUND

**Status:** DESIGN / PLANNING ONLY. No runtime, scene, asset, package, test, workflow, or certified-checkpoint change is authorized by this document.

**Purpose:** create one player-facing shooting-range variant that doubles as ZIPTIDE's canonical weapon sandbox. Terry and the kids can freely try the game's weapons, compare what feels coolest, and identify improvements. Developers can use the same place to validate every weapon without creating a separate test-only combat system.

**Companion documents:**
- `docs/post_recovery/TDOG_WEAPON_SYSTEM_HANDOFF.md`
- `docs/design/WEAPON_FEEL_AND_ARSENAL.md`
- `docs/design/POWERS_MOBILITY_AND_EQUIP.md`
- `docs/design/ABILITIES_AND_ARSENAL.md`
- `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md`

---

## 1. Core decision

Build one canonical **RILL Proving Ground** that uses the real item and combat architecture:

`ItemDefinition in Resources/Items → ItemFactory.Create(itemId) → ItemRuntime → real weapon runtime → real target interfaces`

The range must not create:
- a second weapon factory;
- range-only copies of weapon behavior;
- a second damage system;
- a second inventory identity;
- a second travel path;
- range-specific versions of campaign weapons.

A weapon tested in the range is the same weapon definition and runtime used wherever that weapon belongs in the campaign, arenas, co-op, or other modes. The range only changes **availability and reset rules**, never the weapon's identity.

---

## 2. Product shape

### Player-facing identity

The range is presented as a **RILL training simulation / salvage proving ground**, not a developer test room.

Recommended naming:
1. **RILL Proving Ground** — preferred.
2. Salvage Range.
3. Arsenal Lab.
4. Rustbucket Training Deck.

The first implementation may be a separate small destination reached through the existing ship/dev destination flow. Later it can be visually relocated into the finished hero ship as a simulation room without changing its weapon or target logic.

### Two exposure modes, one implementation

**Public Range**
- shows every weapon approved for normal players;
- safe for Terry's kids to enter from the ordinary game flow;
- no developer labels or unfinished weapons;
- no campaign unlock is granted by using a range weapon;
- free-play first, optional score challenges later.

**Development Range**
- hidden behind the existing developer-access mechanism;
- can expose every registered weapon definition, including unfinished prototypes;
- displays item IDs and diagnostic state;
- may provide reset/refill controls and test-target selectors;
- never changes campaign progression or ownership.

Both modes use the same range shell, catalog, target systems, and `ItemFactory` path. The only difference is the catalog filter and visible diagnostics.

---

## 3. Non-negotiable architecture laws

### Canonical weapon source

The range catalog must be derived from the canonical item registry under `Resources/Items`. Do not maintain a second handwritten list of every gun.

A small additive exposure field may be proposed on the existing item definition contract, for example:

```text
RangeAvailability
- Hidden
- DevelopmentOnly
- Public
```

Neutral/default behavior must be safe: newly added items do not silently become public before they are ready.

Alternative: if the repository already has a suitable content/exposure metadata seam when implementation begins, reuse it instead of adding this enum.

### Canonical factory

Every spawned range weapon must come from:

```text
ItemFactory.Create(itemId, spawnPosition)
```

This preserves:
- real definitions;
- real grip and muzzle transforms;
- real Forge visuals;
- cosmetics where appropriate;
- real XR grab behavior;
- real fire/reload/charge behavior;
- real target and PvP-compatible effect routes.

### Travel ownership

Entering and leaving the range must use `TravelCoordinator`. No direct scene loading and no range-owned teleport/travel singleton.

### Save and inventory isolation

The range must not grant free campaign weapons.

Range-spawned items are session-owned:
- clearly marked as range-created;
- usable and holsterable while inside the range;
- destroyed or reclaimed when leaving;
- excluded from campaign inventory persistence;
- excluded from normal unlock/economy records;
- never restored by SaveSystem after relaunch outside the range.

The player's legitimate campaign inventory remains untouched.

If a range item is holstered when the player exits, the range cleanup still removes it. This rule must have an automated test.

### Balance fidelity

The default public range uses the real weapon data and balance. It does not quietly boost damage, change recoil, or alter projectile speed.

Development conveniences such as infinite charge or automatic refill must be explicit range-session rules and must not mutate ScriptableObject assets or shared weapon definitions.

---

## 4. MVP layout

The first range should remain deliberately compact and legible.

### A. Weapon wall / spawn console

Organize weapons by interaction family rather than one endless list:

- Sidearms / one-handed guns.
- Two-handed or braceable weapons.
- Physical projectiles.
- Beam / charge weapons.
- Gravity and manipulation tools.
- Throwables and deployables.
- Melee weapons.

Current repository examples include:
- Pistol.
- Taser Dart Gun.
- Gravity Gun.
- Static Net.
- Sonic Thumper.
- Prism Beam.
- Breaker Blade.
- Tide Pike.

Future items appear automatically when their canonical definition is registered and their range exposure permits it.

Each tile should show:
- player-facing name;
- simple icon or silhouette;
- interaction family;
- one-sentence purpose;
- spawn/replace button.

### B. Accuracy lane

- fixed targets at several clearly labeled distances;
- moving horizontal and vertical targets;
- optional weak-point target;
- visible hit confirmation;
- reset button.

### C. Drone reaction bay

Use real compatible targets:
- shockable drone;
- moving drone;
- heavier/resistant target later;
- clustered group for crowd-control tools.

This bay proves the full ZIPTIDE identity:

`aim → disable/down → physical response → capture/salvage/reset`

### D. Physics and gravity bay

- light movable props;
- medium resisting props;
- heavy immovable props;
- batteries/sockets or placement targets;
- safe throw wall;
- later: stunned-target manipulation for the Gravity Glove.

### E. Material and impact wall

A small set of tagged surfaces for validating:
- metal;
- stone/concrete;
- wood-like salvage material;
- shield/energy surface;
- Bloom/organic surface later.

This supports T-Dog's planned surface-typed impact VFX/audio without requiring campaign travel for every iteration.

### F. Melee lane

- safe contact dummy;
- reach markers;
- swing-speed feedback;
- thrust target for the Tide Pike;
- resettable breakable object for the Breaker Blade/Thumper family.

### G. Range control panel

MVP controls:
- reset targets;
- clear all range-spawned items;
- respawn selected weapon;
- stationary/moving target toggle;
- return to ship/home.

Development-only controls may later add:
- infinite ammo/charge;
- target durability;
- target family selector;
- distance/speed controls;
- diagnostic labels.

---

## 5. Kid-facing fun layer

The first range must be enjoyable even when nobody is formally testing.

### Free Play

- no failure state;
- instant target resets;
- satisfying non-lethal reactions;
- visible salvage/confetti payoff;
- easy weapon swapping;
- no punishment for poor accuracy.

### Simple challenge variants

Add only after the free-play MVP works:

1. **30-second target pop.**
2. **Drone disable streak.**
3. **Gravity toss accuracy.**
4. **Melee reaction course.**
5. **Crowd-control clear.**

Scores are local and optional. No online leaderboard is required.

### Family feedback kiosk

A lightweight end panel can ask:
- Which weapon was coolest?
- Which weapon felt easiest to aim?
- Which weapon had the best sound/impact?
- Which one should be changed?

For MVP, this may simply display the questions so Terry can ask the kids and record notes manually. Do not build a telemetry backend just to collect family feedback.

A later local-only favorite button may store a harmless preference separately from progression, but it is not required for the first range.

---

## 6. Why this improves development

The range becomes a player-facing feature and a reusable verification surface.

Every new weapon can be checked for:
- registry resolution;
- factory construction;
- correct Forge visual or visible fallback;
- grip position and rotation;
- muzzle alignment;
- one-hand/two-hand interaction;
- trigger or gesture activation;
- projectile/raycast/beam/contact behavior;
- target response;
- haptics and audio;
- moving mechanical parts;
- reload/charge behavior;
- drop, re-grab, holster, and reset;
- material safety and performance.

A weapon should not be considered player-ready until it can be spawned and completed through its relevant range lane without special developer intervention.

The range does not replace focused tests or Quest campaigns. It makes the physical and visual review faster and repeatable.

---

## 7. Recommended implementation sequence after recovery exit

This is not tomorrow's task. It enters the post-recovery master queue after the certified Quest checkpoint passes and T-Dog's weapon packet is integrated.

### RANGE-1 — Pure catalog and isolation contract

**Outcome:** deterministic list of range-visible weapons and a rule preventing range items from entering campaign persistence.

Likely scope:
- pure range-catalog filter over canonical item definitions;
- additive exposure metadata only if no existing seam fits;
- session ownership marker/record;
- tests for Public vs Development filtering;
- tests for cleanup and zero campaign unlock mutation.

Proof:
- EditMode/focused tests;
- ordinary CI.

No scene and no Quest gate yet.

### RANGE-2 — Graybox RILL Proving Ground

**Outcome:** Terry and the kids can enter one compact range, spawn the existing weapons, use targets, reset, and return.

Likely scope:
- one generated/authorable range destination;
- spawn console using `ItemFactory`;
- accuracy targets;
- drone reaction bay;
- gravity props;
- reset/clear/return controls;
- `TravelCoordinator` entry/exit only.

Proof:
- ordinary CI;
- normal PlayMode route for enter/spawn/use/reset/exit;
- visual capture;
- Golden Android before device test;
- focused Quest range session.

### RANGE-3 — Family challenges and feedback polish

**Outcome:** the range becomes a replayable kid-facing activity rather than only a weapon rack.

Likely scope:
- short local challenges;
- visible score/tally;
- simple favorite/feedback surface if still wanted;
- clearer family categorization;
- audio/visual ceremony.

Proof:
- focused tests;
- visual capture;
- targeted Quest review.

### RANGE-4 — Advanced annexes, later only

Separate additions:
- physical reload bench;
- throwable trajectory lane;
- deployable test yard;
- two-hand rifle lane;
- Ultimate/Capture Net bay;
- mobility/grapple course.

Anything that moves the player rig, including grapple, belongs in a separate comfort campaign and must not be bundled into the ordinary weapon range PR.

---

## 8. Placement in the master roadmap

The range is valuable early because it accelerates every later weapon PR, but it must not displace the first complete playable slice.

Recommended relationship:

1. Quest recovery checkpoint passes.
2. Planning packets are reconciled.
3. First-hour bindings/coupler and hero-ship work begins.
4. Shared weapon-feel foundation and the hero Taser direction are locked.
5. RANGE-1 and RANGE-2 land as an early weapon-lane accelerator.
6. Each later weapon is validated in the range before campaign placement.
7. Public challenge polish follows only after the core range feels good.

The range should not modify the locked first-hour 22-beat contract. It is an optional destination/activity outside that critical onboarding spine.

---

## 9. Definition of done for the MVP

The MVP range is complete when:

- every Public-range weapon definition appears without a handwritten duplicate list;
- each selected weapon is created through `ItemFactory`;
- all current weapon families have an appropriate safe target lane;
- no range item survives exit or grants campaign ownership;
- legitimate campaign inventory remains intact;
- reset produces a clean range with no duplicate targets or abandoned projectiles;
- return uses `TravelCoordinator` and leaves rig/input ownership unchanged;
- no pink, null-material, or invisible critical weapon surface appears;
- the range remains readable and comfortable on Quest;
- Terry and the kids can freely compare weapons without developer commands;
- at least one short challenge can be added later without replacing the range architecture.

---

## 10. Explicit non-goals

The first range does not require:
- multiplayer synchronization;
- online leaderboards;
- campaign unlock changes;
- a new inventory/save owner;
- every planned weapon to be implemented first;
- final art for every wall and target;
- space-combat weapons;
- grapple or other rig-moving mobility;
- a telemetry backend;
- a second combat balance table.

---

## 11. Integration note for Reason Box / master packet

PR #61 was written before T-Dog's weapon research and before this range decision. Its Q8 weapon task must be reconciled rather than executed verbatim.

The final post-recovery integration pass should decide:
- which shared weapon-feel components precede the range;
- whether RANGE-1 can run in parallel with hero-ship work;
- when RANGE-2 becomes the physical acceptance surface for the hero Taser;
- how the range avoids file overlap with first-hour W000 and ship-interior work;
- which weapon families are Public versus DevelopmentOnly at first release.

Until that reconciliation occurs, this document is planning input, not an implementation authorization.
