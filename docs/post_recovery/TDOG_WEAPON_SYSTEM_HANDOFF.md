# T-DOG WEAPON SYSTEM HANDOFF

**Purpose:** stop future operators from mistaking obsolete `main` for the actual ZIPTIDE game and provide a source-grounded map of the existing weapon stack before any new combat architecture is proposed.

**Branch of record:** `terry-local-wip`  
**Authorized Quest source remains immutable:** `2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295`  
**This document is research/handoff only. It does not authorize implementation before the recovery Quest checkpoint.**

---

## 1. Critical branch warning

The report that ZIPTIDE contains only Milestone-A scaffolding and no weapon code came from inspecting obsolete `main` or a checkout based on it. That report is not describing `terry-local-wip`.

- `main` is an early historical branch.
- `terry-local-wip` is the source of truth.
- PR #3 is the giant historical comparison from `terry-local-wip` into obsolete `main`; do not treat its conflict state as the current work queue.
- GitHub code search may default to the repository default branch and therefore return zero hits for files that exist only on `terry-local-wip`.

Before any audit, run:

```bash
git fetch origin
git checkout terry-local-wip
git pull --ff-only origin terry-local-wip
git rev-parse HEAD
```

Then search from the repository root, including `Ziptide/Assets/Ziptide/**` and `docs/**`.

No weapon, combat, item, Forge, scene, or asset file was deleted to make recovery pass. The commits after the certified source change only documentation, generated evidence, CI verdicts, and the resolved package lock.

---

## 2. The existing architecture in one sentence

ZIPTIDE weapons currently follow this path:

**ItemDefinition asset in `Resources/Items` → `ItemFactory.Create(itemId)` → XR-grabbable item shell + `ItemRuntime` → weapon-specific runtime → projectile/raycast/contact effect → shared target interfaces and enemy/PvP state → Forge recipe swaps the visual without changing mechanics → inventory/holster/save systems preserve the item identity.**

Do not create a second weapon factory, second inventory identity, second damage owner, or parallel target interface without first proving why the existing seam cannot be extended.

---

## 3. Core item and factory layer

### Data base class

- `Ziptide/Assets/Ziptide/Content/Runtime/Items/ItemDefinition.cs`
  - canonical `itemId`;
  - optional `modelPrefab`;
  - `forgeRecipeId` for generated visual replacement;
  - mass/collider tuning;
  - unified `damage` field intended to converge with `PvpRules`;
  - body scale/color;
  - grip position/rotation;
  - muzzle position;
  - laser-sight color.

### Runtime identity

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemRuntime.cs`
  - stores the `ItemDefinition` on the spawned object;
  - reapplies mass/movement settings;
  - reapplies a serialized Forge look for scene-authored items.

### Canonical factory

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs`
  - loads every definition from `Resources/Items` by `itemId`;
  - chooses a weapon builder by definition type;
  - creates Rigidbody, collider, `XRGrabInteractable`, `Grip`, `Muzzle`, and weapon runtime;
  - uses the fixed gun-grip convention, with per-item definition overrides;
  - attaches `ReleaseFeel`;
  - attaches `GunLaserSight` to grabbable items with a `Muzzle`;
  - applies the equipped cosmetic;
  - applies `ForgeVisualApplier` when `forgeRecipeId` is present;
  - logs `ZIPTIDE: ITEM_SPAWN` and fails loudly on missing definitions.

### Registry/build protection

- `Ziptide/Assets/Ziptide/Tests/EditMode/ItemRegistryConventionTests.cs`
  - enforces the `Resources/Items` convention and unique/nonempty IDs.

---

## 4. Existing campaign/sandbox guns

### A. Taser Dart Gun — implemented and used

**Definition and asset**

- `Content/Runtime/Items/TaserDartGunDefinition.cs`
- `Resources/Items/DefaultTaserDartGun.asset`
- current ID: `taser_dart_gun`
- current Forge look: `taser_gun_mk1`
- current data includes cooldown, muzzle velocity, dart mass/lifetime, stun duration, hit impulse, haptics, and optional fire/impact audio.

**Runtime**

- `Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs`
  - listens to `XRGrabInteractable.activated`;
  - creates and launches a physical sticky dart;
  - ignores the gun/player colliders at launch;
  - sends controller haptics and optional fire audio.
- `Gameplay/Runtime/Weapons/TaserDartProjectile.cs`
  - sticks to the collision target;
  - routes drones through `DroneRuntime.RegisterHit(..., true)`;
  - routes PvP through `IPvpDamageable.ReceiveHit(PvpWeapon.Taser, ...)`;
  - otherwise uses `IShockable.Shock`, `TargetRuntime.Hit`, pooled spark VFX, and impact audio.

**Connected response systems**

- `Core/Runtime/IShockable.cs`
- `Gameplay/Runtime/Enemies/DroneRuntime.cs`
- `Gameplay/Runtime/Enemies/HitZones.cs`
- `Gameplay/Runtime/Targets/TargetRuntime.cs`
- `Gameplay/Runtime/Pvp/IPvpDamageable.cs`
- `Gameplay/Runtime/Pvp/PvpHitSource.cs`
- `Multiplayer/Runtime/PvpRules.cs`

**Visual/cosmetic content**

- `Resources/Forge/taser_gun_mk1.asset`
- `Resources/Cosmetics/taser_rustline.asset`
- `Resources/Cosmetics/taser_tidebreak.asset`

**Economy hook**

- `Resources/Economy/stun_charge_cell.asset`
- `Resources/Recipes/stun_charge_cell.asset`

The taser is not hypothetical. Recovery R1.9 inspected and repaired its active Forge visual/material ownership in W000.

### B. Gravity Gun — implemented sandbox/PvP sibling

**Definition and asset**

- `Content/Runtime/Items/GravityGunDefinition.cs`
- `Resources/Items/Sandbox_GravityGun.asset`
- current ID: `gravity_gun`
- current asset has no Forge recipe assigned, so it uses the primitive/factory visual unless another authoring path supplies a look.

**Runtime**

- `Gameplay/Runtime/Weapons/GravityGunRuntime.cs`
  - hitscan grav pulse;
  - ignores its own colliders and the player rig;
  - downs and launches drones with forward/upward impulse;
  - routes PvP through `PvpWeapon.Gravity`;
  - displays a short beam, haptics, and optional audio;
  - uses the same grab/holster conventions as the taser.

**Cosmetics**

- `Resources/Cosmetics/grav_ember.asset`
- `Resources/Cosmetics/grav_voidglass.asset`

### Important distinction: Gravity Gun versus planned Gravity Glove

They are not currently the same implementation.

- **Existing Gravity Gun:** point-and-fire hitscan pulse that launches drones/targets.
- **Planned Gravity Glove:** hand-driven pull, hold, suspend, place, and throw loop for objects; stunned enemies become eligible targets; the player's body must not be moved.

The glove is planned in `docs/09_GEAR_AND_TOOLS.md` and the retired historical task queue, but it does not yet have the dedicated `GravityGloveTool`/`GravityGrabbable` implementation described there. Research may decide whether it should share targeting/effect primitives with the gravity gun, but it must not be falsely marked as already built.

### C. Pistol — implemented firing/template weapon

**Definition and asset**

- `Content/Runtime/Items/PistolDefinition.cs`
- `Resources/Items/DefaultPistol.asset`
- current ID: `pistol`
- current Forge look: `pistol_scrap_mk1`

**Runtime**

- `Gameplay/Runtime/Weapons/PistolRuntime.cs`
  - hitscan ray;
  - `TargetRuntime` impact;
  - visible tracer every shot;
  - haptics;
  - muzzle flash and fallback click;
  - both XRI Activate event and physical-trigger polling fallback.

**Visual content**

- `Resources/Forge/pistol_scrap_mk1.asset`

Recovery R1.9 also inspected and repaired the active pistol Forge visual/material ownership in W000.

---

## 5. Existing expanded arena arsenal

### Shared definition

- `Content/Runtime/Items/ArenaWeaponDefinition.cs`
- kinds: `StaticNet`, `SonicThumper`, `PrismBeam`, `BreakerBlade`, `TidePike`.
- `ItemFactory.CreateArenaWeapon` chooses the runtime and starting grip/silhouette by kind.

### Runtime files

- `Gameplay/Runtime/Weapons/StaticNetWeapon.cs`
- `Gameplay/Runtime/Weapons/SonicThumperRuntime.cs`
- `Gameplay/Runtime/Weapons/PrismBeamRuntime.cs`
- `Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs`

### Item assets

- `Resources/Items/StaticNet.asset`
- `Resources/Items/SonicThumper.asset`
- `Resources/Items/PrismBeam.asset`
- `Resources/Items/BreakerBlade.asset`
- `Resources/Items/TidePike.asset`

### Forge visual recipes

- `Resources/Forge/static_net_lobber.asset`
- `Resources/Forge/sonic_thumper_maul.asset`
- `Resources/Forge/prism_beam_rifle.asset`

### Balance and combat contract

- `Multiplayer/Runtime/PvpRules.cs`
  - health/armor framework;
  - taser and gravity damage;
  - Static Net damage/slow/zone;
  - Sonic Thumper damage/radius/shove;
  - Prism charge/cooldown/range;
  - Breaker Blade and Tide Pike contact values.
- `Multiplayer/Runtime/WeaponCharge.cs`
  - deterministic two-shot/recharge model;
  - reusable pure state for PvP weapon cadence.
- `Gameplay/Runtime/Pvp/WeaponPadRuntime.cs`
- `Multiplayer/Runtime/PlayerCombatState.cs`
- `Multiplayer/Runtime/PvpCombatant.cs`
- `Gameplay/Runtime/Pvp/PvpBolt.cs`

### Tests

- `Tests/EditMode/PvpArsenalTests.cs`
- `Tests/EditMode/PvpCombatTests.cs`
- `Tests/EditMode/ChargeStateTests.cs`
- `Tests/EditMode/PlayerCombatStateTests.cs`
- `Tests/EditMode/StunStateTests.cs`

---

## 6. Visual pipeline: mechanics and looks are deliberately separate

The weapon's gameplay identity comes from its definition/runtime. The final body is a replaceable presentation layer.

- `ItemDefinition.forgeRecipeId`
- `Editor/Patching/ForgeRecipeLibrary.cs`
- `Editor/Patching/ForgeAuthor.cs`
- `Editor/Patching/ForgeBaker.cs`
- `Visuals/Runtime/Forge/ForgeRecipeDefinition.cs`
- `Visuals/Runtime/Forge/ForgeVisualApplier.cs`
- `Resources/Forge/*.asset`

This means research on moving bolts, charge chambers, cylinders, vents, sights, reload parts, and two-hand contact points should recommend additions to the visual/interaction contract without putting gameplay stats inside the mesh or creating a second weapon object.

The current weapons are mechanically functional but visually and physically graybox. The next weapon program should preserve the item identity/factory/target contracts while replacing primitive presentation and deepening handling, feedback, animation, audio, aiming, reload/charge, and two-hand interaction.

---

## 7. Inventory, holster, travel, and persistence integration

Weapon objects participate in the existing item/inventory system:

- `Gameplay/Runtime/Inventory/BeltRig.cs`
- `Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs`
- `Gameplay/Runtime/Inventory/InventoryState.cs`
- `Gameplay/Runtime/Inventory/InventoryPersistence.cs`
- `Gameplay/Runtime/Inventory/QuickSwap.cs`
- `Gameplay/Runtime/Items/ReleaseFeel.cs`
- `Gameplay/Runtime/Persistence/SaveSystem.cs`

`ItemFactory` caches definitions loaded from `Resources/Items` specifically so a holstered item can be restored after the scene that originally referenced it unloads. Any future weapon architecture must retain stable `itemId` identity and travel/save restore behavior.

---

## 8. Design documents and planned expansion

Read these before proposing a replacement architecture:

1. `docs/09_GEAR_AND_TOOLS.md`
   - non-lethal explorer-tech direction;
   - existing Taser/Pistol integration notes;
   - Static Net, Sonic Thumper, Arc Rifle, Pulse Cannon, Gravity Glove, Foam Cannon, Prism Beam, shields, drones, biotech and traversal idea bank;
   - starter loop: scan → stun → gravity-grab → place/throw → reward.
2. `docs/design/ABILITIES_AND_ARSENAL.md`
   - arsenal build order;
   - every weapon needs a visible counter, charge profile, bot preference, and Gun Game slot;
   - dual-wield plan using a shared charge pool;
   - two-handed-only rule for heavy weapons.
3. `docs/design/PVP_ARENA_AAA.md`
   - arena-quality weapon and encounter targets.
4. `docs/GAME_PLAN.md`
   - M2 starter trio: Scan Pulse → Taser → Gravity Glove;
   - M7 arena arsenal and later online integration.
5. `docs/PROJECT_COMPLETION_ROADMAP.md`
   - ground combat is currently GRAYBOX, not absent;
   - handling, feedback, enemy reactions, encounter design, presentation, and breadth are below the required quality bar.
6. `docs/SPRINT.md`, `docs/FABLE5_BACKLOG.md`, `docs/MASTER_CHECKLIST.md`
   - current/historical status and device gates.
7. Historical source brainstorm:
   - `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/01_potential_weapons_and_tools.md`
   - `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/02_starter_gear_loop_next_three.md`

`docs/04_TASK_QUEUE.md` is explicitly retired; use it only as history.

---

## 9. Honest current maturity

### Present today

- data-driven item identity;
- runtime item factory;
- XR grabbing, grip and muzzle conventions;
- holster/release/persistence integration;
- taser physical sticky projectile and stun routing;
- gravity hitscan launch pulse;
- pistol hitscan/tracer/haptics;
- expanded arena runtimes;
- target/PvP response paths;
- basic charge/balance models;
- Forge look replacement and some weapon recipes;
- cosmetics and one stun-charge economy/recipe hook;
- relevant EditMode tests.

### Still weak or incomplete

- convincing final weapon meshes and material finish;
- moving mechanical parts;
- physical reload/chamber/magazine systems;
- robust two-hand stabilization and secondary grip contracts;
- recoil/weight/pose tuning on Quest;
- full sound libraries;
- high-quality muzzle/impact VFX;
- consistent ammunition/energy/recharge model across campaign and PvP;
- unified target-effect taxonomy beyond existing branches;
- campaign enemy reactions and encounters at shooter quality;
- weapon upgrade/crafting progression;
- dedicated Gravity Glove implementation;
- a complete weapon-quality audit and dedicated Quest campaign.

Do not call the system greenfield. Do not call it shooter-quality either.

---

## 10. Instructions for the weapon research account

The research account should:

1. switch to `terry-local-wip` and record the exact HEAD;
2. audit the existing files above before external research synthesis;
3. produce a documentation-only packet on an isolated branch;
4. treat existing `ItemDefinition` → `ItemFactory` → runtime → target interface as the baseline to extend;
5. separately evaluate campaign explorer tools and arena weapons rather than forcing them into one fantasy;
6. identify which systems can be shared: grip/pose, activation, charge/ammo, haptics, muzzle/effect emission, hit reporting, target responses, VFX/audio events, upgrades, AI use, save identity;
7. identify which mechanics need distinct modules: hitscan, physical projectile, beam, lobbed zone, contact melee, gravity manipulation, physical reload, two-hand support;
8. make classic-versus-VR-native decisions per mechanic;
9. propose bounded PRs, tests, visual artifacts, and Quest gates;
10. make no runtime changes before the authorized recovery checkpoint result.

The output must explicitly say what is being preserved, what is being generalized, what is being replaced only visually, and what is genuinely new.

---

## 11. T-Dog next action

Use this document as the weapon-system locator. Correct any specialist report that says ZIPTIDE has no weapon code. After the Quest checkpoint:

- PASS: reconcile the weapon research packet with the first-complete-slice and hero-ship packets, then select one weapon as the first shipped-quality exemplar.
- FAIL: leave all weapon implementation parked and diagnose only the certified checkpoint evidence.

Recommended first exemplar for planning is the **Taser Dart Gun**, because it already crosses the most systems: XR grab, physical projectile, haptics, target interfaces, drone reaction, PvP response, Forge look, cosmetics, economy resource, holster, travel restore, and the first-hour non-lethal fantasy. That recommendation is planning only until Terry's device checkpoint passes.
