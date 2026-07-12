# F3.6 — THE REACTIVE WORLD LOG

**Owner:** GPT-5.6 Thinking, Terry-authorized next-best sprint  
**Branch:** `terry-local-wip`  
**Status:** ✅ CODE + UNITY CI GREEN — GENERATED-SCENE/DEVICE VERDICT PENDING; FILE CLAIM RELEASED  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.6

## Dependency proof

- pooled VFX factory: green run `29180768058`;
- ambient world VFX: green run `29181420996`;
- practical-light unified `SetLit`: green and reused rather than replaced;
- Shell signage: final green run `29199691568`;
- Gameplay asmdef already referenced Visuals + Multiplayer, so no assembly-cycle change was required.

## Delivered

### Reactive runtime

- `ReactiveProp.cs` + `.meta`
- implements the existing `IPvpDamageable` interface with `PlayerIndex = -1`;
- closed reaction vocabulary:
  - `LightFlickerOut`;
  - `SteamBurst`;
  - `SparkShower`;
  - `Shatter`;
- pure `ReactivePropState` controls one-shot behavior or a 30-second cooldown;
- every reaction resolves to an existing bounded `VfxLibrary` recipe;
- no new damage interface, scoring participant, reward, inventory, save state or navigation state.

### Reaction behavior

- **Lights:** short sparks, then `PracticalLight.SetLit(false)` kills halo, pool and fixture emission together; any existing authored hero `Light` is disabled, never created by this sprint.
- **Signs:** short sparks and a per-instance dim using `MaterialPropertyBlock`; shared sign materials remain untouched.
- **Pipes:** bounded `steam_vent` loop for under one second, explicitly returned to the six-system VFX pool; reaction reopens after 30 seconds.
- **Crates:** visual hides and exactly three non-lethal debris chunks appear; no loot or reward is generated.

### Shared debris rail

- `WorldDebrisBudget.cs` + `.meta`
- one global 24-object rail now covers reactive props and existing breakable-wall chunks;
- oldest live debris retires first;
- chunks remain short-lived, non-lethal and shadow-free;
- `BreakableWall` retained its existing hit mapping, collapse, regeneration, colors and chunk motion while replacing its private duplicate queue with `WorldDebrisBudget.Register(go)`.

### Deterministic world author

- `ReactivePropAuthor.cs` + `.meta`
- scans only authored `ForgeModuleLook.recipeId` values after all other world visual authors finish;
- closed recipe map:
  - `light_sconce_wall`, `light_street_pole`, `light_lantern_hang` → `LightFlickerOut`;
  - the three Shell sign recipes → `SparkShower`;
  - `prop_pipe_cluster` → `SteamBurst`;
  - `prop_patched_crate` → `Shatter`;
- adds exactly one `ReactiveProp` per owning prop;
- adds one tightly bounded non-trigger `__REACTIVE_HIT` BoxCollider and no Rigidbody;
- lantern child looks resolve upward to their parent `PracticalLight` owner;
- root practical looks preserve `PracticalHalo`, `PracticalPool` and `__REACTIVE_HIT` through the Forge runtime swap;
- sign looks preserve both `ShellGlyph` and `__REACTIVE_HIT`;
- author is idempotent.

### World wiring

`WorldDressingBuilder` gained exactly one additive call after signage and all other visual authors:

```csharp
ReactivePropAuthor.Place(dressRoot);
```

Generated worlds therefore always receive reactive practical lights and Shell signs. Pipe clusters and patched crates receive their reactions wherever those Forge recipes are present. This sprint did not invent arbitrary prop placements merely to manufacture demo targets.

## Verification

Added focused EditMode coverage for:

- one-shot, cooldown and reset timing;
- all four reaction kinds and their real VFX recipes;
- practical halo/pool/emission/hero-light shutdown;
- timed steam reservation and pool return;
- three-piece shatter behavior;
- one shared 24-object wall/prop debris rail;
- closed eight-row Forge recipe vocabulary;
- practical/sign/pipe/crate ownership and collider dimensions;
- lantern owner resolution;
- idempotent one-component/one-proxy behavior;
- practical glow preservation through Forge swap;
- exact one-call author order after signage;
- absence of loot, save, navigation, XRI, new Light creation, Rigidbody creation, material instances or damage-interface forks.

## CI proof

### Commit 1 — core

- tested SHA: `22567a824b41a2cba14230b1d0b08c69f9fbbc39`;
- green run: `29200317996`;
- Unity EditMode: `success`;
- project-contract reports: `success`;
- Android: skipped as expected for an ordinary branch push.

### Commit 2 — deterministic wiring

- tested SHA: `c8563bf085e27d245d17c7cc6cb56da967b7656e`;
- green run: `29200715702`;
- Unity EditMode: `success`;
- project-contract reports: `success`;
- Android: skipped as expected for an ordinary branch push;
- durable verdict-only child: `994a6dec556deffbbc7b293e32803fde92290aa3`;
- circuit breaker: `0/3` reds.

## Device evidence still required

After generated-world re-authoring and the next Quest build:

1. shoot one street pole, sconce or lantern and confirm halo, pool, emissive and hero light die together;
2. shoot a Shell sign and confirm a short spark shower plus a dimmed face, with no menu/UI behavior;
3. where a pipe cluster exists, confirm steam lasts under one second and reopens after 30 seconds;
4. where a patched crate exists, confirm exactly three non-lethal chunks and no reward/drop;
5. stress wall and prop debris together and confirm oldest chunks retire at 24 live;
6. verify dedicated hit proxies do not create route blockers or uncomfortable hand collisions;
7. confirm repeated reactions do not exceed six live VFX systems or destabilize 72 Hz.

## Handoff — Did / Next / Heads-up / Commits

**Did:** completed F3.6 end-to-end: existing weapons can now drive bounded reactions on generated-world practical lights, Shell signs and recipe-tagged pipes/crates without adding a parallel damage, reward or physics system.

**Next:** regenerate worlds, build/install to Quest and execute the device checklist above. The next code sprint should be selected only after those visual/collision/performance observations are recorded.

**Heads-up:** Unity CI proves contracts, lifecycle and budgets; it does not prove that the hit proxies feel comfortable in VR or that the VFX density reads correctly through the headset. Pipe/crate behavior is wired by recipe ID but visible examples depend on those recipes actually being placed in a generated world.

**Commits:** core checkpoint `22567a8`; final wiring checkpoint `c8563bf`; final durable verdict `994a6de`; final run `29200715702`.

## Closure

F3.6 is code/CI green, its file claim is released, and the sprint is complete. It remains device-yellow until regenerated-world and headset verification pass.
