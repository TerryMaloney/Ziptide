# F3.6 — THE REACTIVE WORLD LOG

**Owner:** GPT-5.6 Thinking, Terry-authorized next-best sprint  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — commit 1 core/state/debris work in progress  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.6

## Dependency proof

- pooled VFX factory: green run `29180768058`;
- ambient world VFX: green run `29181420996`;
- practical-light unified `SetLit`: already green;
- Shell signage: final green run `29199691568`;
- Gameplay asmdef already references Visuals + Multiplayer, so no assembly-cycle change is required.

## Two-commit envelope

### Commit 1 — core reaction contract

New Gameplay runtime:

- `ReactiveProp.cs` + `.meta`
  - implements existing `IPvpDamageable` (`PlayerIndex=-1`);
  - closed `ReactionKind { LightFlickerOut, SteamBurst, SparkShower, Shatter }`;
  - pure `ReactivePropState`: one-shot or 30-second cooldown, injected-clock tests;
  - closed rule table mapping every kind to an existing VFX id;
  - lamp shutdown delegates to `PracticalLight.SetLit(false)` and disables only its authored hero Light;
  - sign spark/dim uses `MaterialPropertyBlock`, never material instances;
  - steam uses the existing looping `steam_vent` but returns it to the six-system pool after a short bounded burst;
  - shatter hides the prop look and emits three non-lethal chunks.

Shared debris rail:

- `WorldDebrisBudget.cs` + `.meta`
  - one 24-object budget shared by existing breakable-wall chunks and reactive-prop chunks;
  - EditMode-safe cleanup; no new physics budget.
- additive `BreakableWall` refactor: register existing chunks through the shared budget; wall behavior/math unchanged.

Tests:

- one-shot/cooldown/reset/determinism;
- every ReactionKind has a real VfxLibrary recipe;
- practical shutdown is unified;
- shatter emits at most three chunks through the shared cap;
- wall and prop debris share the same 24-live rail;
- no loot, navigation, persistence, or material-instance ownership.

### Commit 2 — deterministic wiring

New editor pass:

- `ReactivePropAuthor.cs` + `.meta`
  - scans authored `ForgeModuleLook.recipeId` under the rebuilt Dressing root;
  - `light_*` → `LightFlickerOut`;
  - `sign_*` → `SparkShower`;
  - `prop_pipe_cluster` → `SteamBurst`;
  - `prop_patched_crate` → `Shatter`;
  - adds exactly one existing-interface component per owning prop and a tightly bounded projectile hit proxy;
  - lantern child looks resolve to the parent `PracticalLight` owner;
  - future pipe/crate placements inherit the reaction automatically.

One additive hook:

- `WorldDressingBuilder`: `ReactivePropAuthor.Place(dressRoot)` after all visual authors.

Tests:

- all four recipe families map;
- exact component/collider ownership, no Rigidbody/loot/XRI;
- idempotent scan and one author call;
- every ReactionKind has at least one recipe caller row.

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
