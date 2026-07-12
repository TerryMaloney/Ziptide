# F3.5 COMMIT 3A — AMBIENT WORLD MOTES LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ CODE + UNITY CI GREEN — GENERATED-SCENE/DEVICE LOOK VERDICT PENDING; FILE CLAIM RELEASED  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.5

## Delivered

### Runtime marker

- `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/AmbientMoteVolume.cs` + `.meta`
- scene-owned marker reserves one looping `Motes` recipe through the existing `VfxFactory`;
- automatic start occurs only in Play Mode;
- explicit/automatic stop returns the system to the bounded shared pool;
- one-shot or missing recipes are rejected;
- no player lookup, rig following, persistence, gameplay state or second particle owner.

### Deterministic author

- `Ziptide/Assets/Ziptide/Editor/Patching/AmbientMoteAuthor.cs` + `.meta`
- at most three fixed route anchors: start, midpoint, far end, duplicate indices removed;
- marker height is `1.45m` above authored terrain;
- `TideFlats` and `CavernFloor` use `motes_spore`;
- `Dunes`, `Mesas`, `Canyon` and fallback use `motes_amber`;
- markers contain only Transform + `AmbientMoteVolume`: no renderer, collider, light, rigidbody or static batching flag;
- author replaces only its own `AmbientMotes` child.

### World wiring

`WorldDressingBuilder` gained exactly one additive call after the existing water pass:

```csharp
AmbientMoteAuthor.Place(dressRoot, kit, route);
```

No route, cairn, scatter, practical-light, water, plant, material or helper behavior changed.

## Budget behavior

- maximum three ambient loops per world;
- shared factory remains capped at six systems total/live;
- at least three VFX slots remain available for impacts, sparks, muzzle effects or reactions;
- ambient volume never follows the player or fills the entire map with transparent overdraw.

## Verification

`AmbientMoteTests` cover:

- stable unique route-index sampling from 0–99 route points;
- both canonical mote recipes and biome mapping;
- three-marker cap and exact positions;
- idempotent rebuild;
- no physical/render/light components;
- one-shot rejection;
- looping factory reservation and release;
- Play-Mode-only automatic lifecycle;
- no persistence/player lookup;
- exactly one world-dressing author call after water.

## CI proof

- claim: `75e6f412998a29141ab5e164cd60b81c7f008bd3`
- runtime: `b7033208e15ecb4d88e5fce28568c947efb56d80`
- verification-trigger head: `0b47fd047b47c36e019ba11282b67910aa19f33e`
  - green run `29181133477`
- editor author: `cdc0ce58a4eb1165af009eb2a4ad13b88496f302`
- world hook: `1e45a98898102f6f45d0411d08244512a407c56a`
  - green run `29181287390`
- final tests: `d3800af2020bb0aeb9e78415d69323299e2dd765`
  - green run `29181420996`
  - Unity EditMode: `success`
  - project-contract reports: `success`
  - Android: skipped as expected for ordinary branch CI
- circuit breaker: `0/3` reds.

## Terry device evidence

After the next world re-bake/build:

1. inspect one dry/amber world and one TideFlats/Cavern/spore world;
2. verify the particles are visible when crossing route anchors but do not resemble square cards or confetti;
3. confirm there are no particles permanently attached to the head or hands;
4. verify repeated world travel resets old ambient systems rather than accumulating them;
5. stress six simultaneous effects and confirm 72 Hz remains stable;
6. treat normal `VFX_DROPPED reason=live_cap` logs as a tuning failure if ambient loops starve gameplay effects.

## Remaining F3.5 work

- **Gameplay/combat half remains blocked:** weapon/creature/wall impact calls must be assigned to the Gameplay owner before touching `CreatureRuntime.ReceiveHit` or weapon hit paths.
- Art-owned runtime factory and ambient world wiring are complete.

## Handoff — Did / Next / Heads-up / Commits

**Did:** gave every generated world a bounded, biome-specific atmospheric-motion layer using the existing route and VFX pool, without creating a player follower or unbounded particle field.

**Next:** F3.7 signage is the cleanest independent Picasso-lane target. F3.6 reactive props and F3.5 combat impacts cross Gameplay/damage ownership and should remain held.

**Heads-up:** the generated scenes must be re-authored before Terry will see the new marker components. Compilation proves structure/budget, not final density or particle softness.

**Commits:** runtime `b703320`; author `cdc0ce5`; world hook `1e45a98`; tests `d3800af`; final run `29181420996`.

## Closure

F3.5 commit 3A is code/CI green and its file claim is released. It remains device-yellow until the generated-world and headset look/performance checks pass.
