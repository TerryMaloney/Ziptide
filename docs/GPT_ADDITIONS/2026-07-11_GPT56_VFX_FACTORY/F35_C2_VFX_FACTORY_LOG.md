# F3.5 COMMIT 2 — POOLED VFX FACTORY LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — runtime factory implementation in progress  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.5

## Workflow baseline

- live head at claim: `20d121dcf287c05552c0452bdeb6c3754c996056`;
- latest tested art head: `21012fb4d72a652579b0ae2147823199438a0a18`;
- durable CI run: `29174226907`, Unity EditMode green;
- intervening commits are documentation-only Cinematic Presence canon.

## Exact scope

New Art/Visuals runtime:

- `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs` + `.meta`

New EditMode verification:

- `Ziptide/Assets/Ziptide/Tests/EditMode/VfxFactoryTests.cs` + `.meta`

Closure-only documentation after green:

- this log;
- `docs/SPRINT_ART.md` F3.5 row;
- current handoff/checklist only if the status change is not already visible through the art board.

## Runtime contract

- public `VfxFactory.Spawn(id, position, normal)` resolves the existing `VfxLibrary`;
- one scene-local hidden factory root, not a new persistent gameplay singleton;
- one bounded global pool with per-kind buckets and at most six total `ParticleSystem` instances;
- no more than six active systems;
- inactive same-kind instance is reused first; an inactive different-kind instance may be reconfigured instead of growing the pool;
- unknown ids and cap drops are safe no-ops with `ZIPTIDE:` diagnostics;
- one-shots automatically return to the pool; looping effects remain until `VfxFactory.Stop(system)`;
- runtime `maxParticles` is clamped to the existing recipe peak and hard cap;
- no particle lights, collision, trails, sub-emitters, mesh particles or shader graph;
- one shared soft radial particle material/texture, destroyed with the scene-local factory;
- no changes to recipes, caps, combat, `CreatureRuntime.ReceiveHit`, world dressing, scenes or prefabs.

## Visual vocabulary mapping

- Impact: short surface-facing cone puff/debris.
- Muzzle: narrow forward flash.
- SteamVent: narrow rising continuous cone.
- Motes: sparse slow sphere drift.
- Sparks: fast narrow surface-facing cone.
- Drips: compact downward box emitter for future recipes.

## Tests

- unknown id is safe;
- known recipe configures a real system;
- max particles never exceeds recipe/hard cap;
- normal determines orientation;
- stopped systems return and are reused;
- looping systems remain active until explicitly stopped;
- seventh concurrent spawn is dropped at the six-live rail;
- total allocated pool never exceeds six;
- cleanup releases the shared runtime material/texture and static owner.

## Collision / stop rules

- Do not touch Gameplay/combat/ecology, world placement, Forge recipes, water, signage, reactive props, scene YAML or prefabs.
- Do not start F3.5 c3, F3.6 or F3.7 before commit 2 receives a durable green verdict.
- Three CI reds on this task triggers the circuit breaker.
