# F3.5 COMMIT 2 — POOLED VFX FACTORY LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ CODE + UNITY CI GREEN — DEVICE LOOK/PERFORMANCE VERDICT PENDING; FILE CLAIM RELEASED  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.5

## Delivered runtime

- `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs` + `.meta`
- public `VfxFactory.Spawn(id, position, normal)` resolving the existing `VfxLibrary`;
- public `VfxFactory.Stop(system)` for looping effects;
- one scene-local hidden factory root, not a persistent gameplay singleton;
- one bounded global pool with per-kind buckets and at most six total `ParticleSystem` instances;
- no more than six active systems;
- same-kind reuse first; full-pool cross-kind recycling instead of unbounded allocation;
- unknown ids and cap drops are safe no-ops with `ZIPTIDE:` diagnostics;
- one-shots auto-return through `ParticleSystemStopAction.Callback`;
- runtime `maxParticles` clamps to the recipe peak and the existing 64-particle hard rail;
- Impact/Muzzle/Steam/Motes/Sparks/Drips receive distinct low-cost shapes and size behavior;
- one shared 32×32 soft radial particle texture/material;
- shared material/texture and pooled systems are released with the scene-local factory;
- no particle lights, collision, trails, sub-emitters, mesh particles, shader graph or real-time shadows.

## Ownership preserved

Untouched:

- `VfxRecipeDefinition`, `VfxLibrary` and their existing caps;
- Gameplay/combat/ecology;
- `CreatureRuntime.ReceiveHit`;
- world dressing and scene placement;
- Forge assets, water, signage and reactive props;
- scenes/prefabs/YAML.

## Verification

Added `VfxFactoryTests` covering:

- unknown-id safety;
- recipe-driven modules and particle cap;
- surface-normal orientation;
- no collision/trails/lights/sub-emitters/mesh rendering;
- explicit stop and same-kind reuse;
- full-pool cross-kind recycling;
- one-shot callback return;
- seventh concurrent spawn dropped at the six-live rail;
- total pool never above six;
- scene-local static reset and explicit shared-resource cleanup discipline.

## CI proof

- claim: `ebc620e35ddfbe589849cac73135e2a572273d29`
- runtime: `8d09ea4ff8a0420addd496822fa82cdf7b5d1248`
  - CI green run `29180422708`
- runtime/test metadata: `1756cbd`, `9f99439`
- initial tests: `96e016a99cd965f2611cfbf03ff0eb240ba4fe47`
  - CI red run `29180592140`, 966/969 passed
  - three failures were test-harness assumptions only: cross-kind reuse timing, EditMode `SendMessage`, and destroyed-resource identity
- test-only correction: `538dba59574c57ca13786b5b928de116d1040d08`
  - final CI green run `29180768058`
  - Unity EditMode: `success`
  - project-contract reports: `success`
  - Android: skipped as expected for ordinary branch CI
  - final total: 969/969 tests passed
- circuit breaker: `1/3` red; runtime remained green throughout.

## Required device verdict

When VFX callers exist in a built scene:

1. impacts/sparks/muzzle read without square-card artifacts;
2. steam and motes remain sparse rather than fogging the view;
3. no visible particle lights or shadow cost;
4. six simultaneous systems do not destabilize 72 Hz;
5. pooled effects restart cleanly after repeated use;
6. `ZIPTIDE: VFX_DROPPED reason=live_cap` appears only during deliberate stress, not normal play.

## Handoff — Did / Next / Heads-up / Commits

**Did:** completed the previously boarded pooled `VfxFactory` runtime with hard live/allocation rails and full EditMode coverage.

**Next:** F3.5 c3 may now proceed in two separately owned halves. The art-owned safe half is ambient world-mote placement in `WorldDressingBuilder`. The weapon-impact call in `CreatureRuntime.ReceiveHit` remains blocked until Terry assigns the Gameplay/combat seam.

**Heads-up:** compiling and tests prove budget/ownership behavior, not the final particle look. The first real caller should be ambient motes because it is low-risk and gives a headset-visible quality verdict before combat wiring.

**Commits:** runtime `8d09ea4`; final tests `538dba5`; final run `29180768058`.

## Closure

F3.5 commit 2 is code/CI green and its file claim is released. It remains device-yellow until at least one ambient and one one-shot caller are judged in-headset.
