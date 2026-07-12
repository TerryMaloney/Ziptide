# F3.6 — THE REACTIVE WORLD LOG

**Owner:** GPT-5.6 Thinking, Terry-authorized next-best sprint  
**Branch:** `terry-local-wip`  
**Status:** 🟡 FULL ENVELOPE STAGED — final Unity verification requested  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.6

## Dependency proof

- pooled VFX factory: green run `29180768058`;
- ambient world VFX: green run `29181420996`;
- practical-light unified `SetLit`: already green;
- Shell signage: final green run `29199691568`;
- Gameplay asmdef already references Visuals + Multiplayer, so no assembly-cycle change is required.

## Commit 1 — core reaction contract ✅ GREEN

### Runtime

- `ReactiveProp.cs` + `.meta`
  - implements existing `IPvpDamageable` (`PlayerIndex=-1`);
  - closed `ReactionKind { LightFlickerOut, SteamBurst, SparkShower, Shatter }`;
  - pure `ReactivePropState`: one-shot or 30-second cooldown, injected-clock tests;
  - closed rule table mapping every kind to an existing VFX id;
  - lamp shutdown delegates to `PracticalLight.SetLit(false)` and disables only its authored hero Light;
  - sign spark/dim uses `MaterialPropertyBlock`, never material instances;
  - steam uses the existing looping `steam_vent` but returns it to the six-system pool after a short bounded burst;
  - shatter hides the prop look and emits three non-lethal chunks.

### Shared debris rail

- `WorldDebrisBudget.cs` + `.meta`
  - one 24-object budget shared by existing breakable-wall chunks and reactive-prop chunks;
  - EditMode-safe cleanup; no new physics budget.
- `BreakableWall` additive refactor:
  - existing wall chunks now call `WorldDebrisBudget.Register(go)`;
  - private duplicate queue/cap removed;
  - hit mapping, collapse, regeneration, visuals and chunk motion unchanged.

### Verification

`ReactivePropTests` pin:

- one-shot/cooldown/reset timing;
- every ReactionKind resolves to a real `VfxLibrary` recipe;
- practical halo/pool/emission/hero-light shutdown;
- bounded steam reservation and pool return;
- three-chunk shatter behavior;
- wall and prop debris sharing the same 24-live rail;
- no loot, navigation, persistence or material-instance ownership.

Commit-1 proof:

- tested SHA `22567a824b41a2cba14230b1d0b08c69f9fbbc39`;
- green run `29200317996`;
- circuit breaker `0/3` reds.

## Commit 2 — deterministic wiring staged

### Editor author

- `ReactivePropAuthor.cs` + `.meta`
  - scans authored `ForgeModuleLook.recipeId` under the rebuilt Dressing root;
  - `light_sconce_wall`, `light_street_pole`, `light_lantern_hang` → `LightFlickerOut`;
  - three Shell sign recipes → `SparkShower`;
  - `prop_pipe_cluster` → `SteamBurst`;
  - `prop_patched_crate` → `Shatter`;
  - adds exactly one existing-interface component per owning prop and a tightly bounded projectile hit proxy;
  - lantern child looks resolve to the parent `PracticalLight` owner;
  - root practical looks preserve `PracticalHalo`, `PracticalPool`, and `__REACTIVE_HIT` through the Forge swap;
  - future/current pipe and crate placements inherit the reaction automatically by recipe id.

### World hook

`WorldDressingBuilder` gains exactly one additive call after every visual author:

```csharp
ReactivePropAuthor.Place(dressRoot);
```

### Current visibility truth

- generated worlds currently guarantee reactive practical lights and Shell signs because those authors always run;
- pipe clusters and patched crates react wherever those Forge recipes are present;
- this sprint does not invent arbitrary new prop placement merely to manufacture a demo target.

### Verification staged

- closed eight-row recipe vocabulary covers all four ReactionKinds;
- practical/sign/pipe/crate ownership and collider dimensions;
- lantern reaction resolves to practical root;
- idempotent one-component/one-proxy behavior;
- no Rigidbody, loot, XRI, real-Light creation or damage-interface fork;
- practical glow children survive Forge swap regardless of Awake order;
- exact one-call author order after signage.

### Atomic final checkpoint

- staging branch: `gpt56-staging/f36-wiring`;
- staged code/test head before this trigger: `1dd9cb20382013d8f2e44f01b73e992dc61165e6` plus subsequent glow-preservation and wiring-test commits;
- diff remains limited to one new editor author, one two-line builder hook, and focused EditMode tests;
- no scene/prefab YAML, UI, travel, rewards, jobs, saves or unrelated Gameplay changes.

## Locked rails

- no new damage interface;
- no drops/rewards/save state;
- reaction never changes traversal or authored structural colliders;
- only the dedicated projectile hit proxy may disable when a one-shot prop disappears;
- no particle collision/lights/sub-emitters;
- VFX remains ≤6 live, ≤64 particles/system;
- debris remains ≤24 live globally and non-lethal;
- no scene/prefab YAML edits;
- three CI reds stops the envelope.

## Device evidence after green

1. shoot one street/sconce/lantern fixture and confirm halo, pool, emissive, and hero light die together;
2. shoot a Shell sign and confirm a short spark shower plus dimmed face, with no menu/UI behavior;
3. where a pipe cluster exists, confirm steam lasts under one second and reopens after 30 seconds;
4. where a patched crate exists, confirm exactly three non-lethal chunks and no reward/drop;
5. stress wall + prop debris together and confirm the oldest chunks retire at 24 live;
6. verify dedicated hit proxies do not create route blockers or unexpected hand collisions;
7. confirm repeated reactions do not exceed six live VFX systems or destabilize 72 Hz.
