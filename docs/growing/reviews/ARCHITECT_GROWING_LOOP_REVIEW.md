# ARCHITECT REVIEW — Growing/Invention Loop (canonical ownership, schemas, data flow, automation)
**Reviewer:** Fable 5 (Architect lane) · 2026-07-23 · per `GROWING_INVENTION_LOOP_REVIEW_PACKET.md` §3
**Verdict up front:** **APPROVE the direction — as an EXTENSION, not a build.** The source audit
shows ~80% of the plan's "potential data authorities" (§18) already exist with tested owners.
The single biggest architectural risk is that implementation *re-invents what already ships*.
Every recommendation below is written so a lower-cost coding model can follow it.

---

## 1 · CURRENT OWNERSHIP MAP (source-audited 2026-07-23)

### Proven owners (extend these; do not parallel them)
| Responsibility | Canonical owner | Evidence |
|---|---|---|
| Instance genetics (speed/yield/size/generation), deterministic crossing, rarity, GIANT threshold | `Core/Runtime/Economy/PlantGenetics.cs` — pure, seeded xorshift, EditMode-tested | `PlantGenes`, `Cross(a,b,seed)`, `RollWild`, `Rarity`, `HazardKick` |
| Plant/tend/harvest lifecycle, timing windows (Fresh/Prime/Overripe, never-lose floor), adjacency crossing | `Content/Runtime/Economy/GardenService.cs` — pure | `Plant/Tend/Harvest/CrossPlots`, `HarvestPlantResult.giant` |
| Species data (24 authored) | `PlantDefinition : Definition` (+ `GardenAuthor` patcher path) | growSeconds, harvestYield, tendToolIds, ripeness overrides |
| **Recipes** — costs, output, machine gating, unlock flags, campaign/MP split | **`Content/Runtime/Definitions/RecipeDefinition.cs` + `RecipeService` (CanAfford/TrySpend)** | `costs`, `producesId/Amount`, `requiredMachineType`, `durationTicks`, `unlockFlag`, `sourceWorlds`, `campaignUse`, `multiplayerUse` — **the plan's §5 grammar is 70% already here** |
| Machine processing graph incl. **BioRefiner** | `Core/Runtime/Economy/EconomyState.cs` `MachineNodeState` | `MachineType{…Processor,BioRefiner,Assembler…}`, `recipeId`, `inputNodeIds`, `progressTicks` — saved |
| Mining/idle production | `MineState` (same file) + `MiningRigRuntime` | rate/stored/lastResolvedAtUnix idle-anchor idiom |
| Resource/material identity + counts | `ResourceDefinition`, `ResourceCost`, `ResourceNodeDefinition`; counts ONLY via `ResourceLedger` + `RewardRouter` (one-economy law) | `EconomyAuthor` authors them |
| Items/equipment by string id | `ItemDefinition` + `ItemFactory` (+ holster allowlist, `InventoryState` travel) | locked contract #3/#5 |
| Persistence | `PlayerProfile` + `ProfileSerializer` schema-versioned migrations; overlay-save idiom; enum-as-int | belts/photos already extended it safely |
| Automation persistence + caps | Belt lane (`BeltFloorSave` in Core, `AutomationAuditRules`, PerfBudget caps) | shipped, CI-gated |
| World declarations | `WorldPackDefinition` (jobs/collectibles/machines/mines/**gardens**/sockets/flags) + `WorldSpecCompiler` | flags idiom for gating |
| Content-reachability gate | `CatalogBreadthAuditRules` (already exposes unsurfaced plants) | the plan's "gap" is already measured |
| Impact classification | `ForgeStaleness` buckets (SafeAuto/Review/Breaking/Deprecated) | generalization already ordered (hwr32/S3) |

### Missing owners (the actual new work)
1. **TraitDefinition layer** — functional traits (conductive/buoyant/filtration/…) as data assets; `PlantDefinition.traitIds` + `ResourceDefinition.traitIds`. Static, definition-level.
2. **Cultivation recipe extension** — multi-input plot recipes with roles (base/trait/catalyst).
3. **Catalog/discovery state** — per-recipe `Unknown→Seen→PartiallyKnown→Mastered` on the profile.
4. **Accessible-label/narration seam** — project-wide, NOT garden-specific (see §5.4).
5. **Loadout capacity tags** — `ItemDefinition.slotCost` + a capacity rule on the existing holster.

### Forbidden duplicates (red-team hard NOs)
- ❌ A second inventory or "catalog counts" store — counts are `ResourceLedger`, period.
- ❌ A new crafting service beside `RecipeService` — extend it.
- ❌ A second genetics/trait randomness path — ALL randomness stays in `PlantGenetics` (seeded).
- ❌ Per-item hard-coded narration audio clips — text-first seam only (T-Dog decides voice source).
- ❌ Runtime-generated species/hybrids as new content — hybrids are AUTHORED species unlocked
  deterministically; genes vary instances, never definitions.
- ❌ A second machine-graph or conveyor state — `MachineNodeState`/belt lane own it.

## 2 · RECIPE GRAMMAR — the one big architecture decision (packet Q3/Q4)

**Recommendation: ONE recipe family for every transformation** (plot, machine, assembly), by
extending `RecipeDefinition`, not subclassing per domain:

```
RecipeDefinition (existing)                     // NEW fields, all defaulted
+ List<RecipeInput> inputs                      // supersedes costs for multi-role recipes
    RecipeInput { string resourceOrItemId; double amount; int role; }   // role: 0=Base 1=Trait 2=Catalyst (enum-as-int)
+ int domain                                    // 0=Machine(existing) 1=Plot 2=Assembly (enum-as-int)
+ List<string> requiredTraitIds                 // trait-checked inputs ("any conductive fiber")
+ string discoveryHintText                      // silhouette/clue line for the catalog card
```
`costs` stays for old recipes (migration-free). `requiredMachineType` gains `Plot` — plots become
machines in the grammar, which is exactly how the built graph already thinks (`BioRefiner`).

**Determinism split (Q4), the clean law:** *recipes are 100% deterministic; genes are the only
randomness.* A plot recipe always yields its `producesId`; the INSTANCE quality (yield/size/giant)
comes from `PlantGenetics.Cross` of the input lines — seeded, save-stable, already tested.
Optional-surprise (pillar 3.4) is fully served by genes + authored rare catalysts; campaign
never blocks on RNG (pillar 3.3) because recipe outputs never roll. Trait-checked inputs
(`requiredTraitIds`) give the "reason about families" comprehension (pillar 2) without
combinatorial explosion: the audit gate (§6) enforces that every recipe resolves against
authored content — there IS no unbounded combination space; there is an authored recipe list
with family-based inputs.

## 3 · DATA FLOW (text diagram)

```
WORLD (discover)                 SHIP (transform)                        FIELD (apply)
seed/ore/salvage item        →  ResourceLedger counts (RewardRouter)  →  invention catalog reads
  (ItemDefinition /             profile.recipeStates: Unknown→…          RecipeService.CanAfford
   ResourceDefinition ids)      ├─ PLOT: RecipeDefinition(domain=Plot)   + discovery state
scan → recipeState:Seen         │    GardenService.Plant(genes via       └→ TrySpend → producesId
WorldPack flags/capability tags │    PlantGenetics.Cross of inputs)          ├─ resource → ledger
  (additive, optional)          │    PlotState (saved, existing)             └─ item → ItemFactory
                                ├─ MACHINE: MachineNodeState.recipeId          → holster (slotCost
                                │    progressTicks (saved, existing)             capacity rule)
                                └─ ASSEMBLY: RecipeService.TrySpend      travel per contract #3
```
One economy chokepoint, one item factory, one save route — no new arrows, only new data.

## 4 · SAVE/MIGRATION STRATEGY (+ failure cases)

- **New profile fields:** `List<RecipeState> recipeStates { string recipeId; int state; }`,
  `List<string> discoveredTraitIds`. Schema-version bump + null-guard block (the exact
  `ProfileSerializer` pattern used at v3 for photos). Old saves → empty lists = everything
  Unknown = correct.
- **PlotState extension:** `recipeId`, `List<string> inputLineage` (ids only), genes already
  serialize. Defaults empty → legacy plots behave exactly as today (overlay-save law).
- **Never serialize trait/definition objects** — ids only, resolved through factories at
  runtime (`ITEM_DEF_NOT_FOUND` idiom logs and preserves unknown ids rather than dropping,
  so content renames can't eat player data).
- **Failure cases:** (a) recipe removed from content → state row preserved, catalog hides it,
  audit flags orphan; (b) mid-growth quit/travel → PlotState timestamps already handle
  (idle-anchor idiom, proven by mines/belts); (c) save from newer schema → existing
  refuse-downgrade behavior; (d) MP-rotation data must live OUTSIDE PlayerProfile campaign
  fields (own envelope), or offline saves get contaminated — hard rule.

## 5 · W001 IMPLEMENTATION SLICES (after M0 verdict + freeze lift ONLY)

1. **S-A (pure, CI-provable):** TraitDefinition + RecipeDefinition extension + RecipeState
   profile fields + migration + EditMode tests. No scene, no UI.
2. **S-B (data):** 3 seed families, 1 deterministic plot recipe, 1 combined bio+industrial
   goal card — authored via `EconomyAuthor`/`GardenAuthor` paths (recipe-hash law applies).
3. **S-C (runtime, small):** plot accepts multi-input recipe (GardenService overload),
   catalog card surface on the ship (reuse ArenaLobbyBoard/board idiom), narration seam v0.
4. **S-D (loadout):** `slotCost` + holster capacity check + one world interaction that reads
   a capability tag.
5. **S-E (gates):** the §6 audit set, wired into `WorldAuditRunner`.
Dependencies: S-A blocks all; S-B/S-C parallel after; S-D independent; S-E lands with each.

**§5.4 Narration seam (project-wide by design):** add `spokenName` + `spokenFunction` TEXT
fields to the base `Definition` class — every existing definition inherits them; one
`NarrationDirector` under `AudioDirector`'s policy renders text→speech (TTS vs prerecorded is
T-Dog's call — the SCHEMA is identical either way, which is why text-first is the law).

## 6 · VALIDATOR/AUDIT LIST
**Blocking (CI):** recipe-closure (every input/producesId resolves; every recipe reachable —
extends CatalogBreadth) · campaign-determinism (domain≠gene randomness; no `campaignUse`
recipe behind MP rotation) · narration-coverage (every catalog-visible definition has
name+spokenName+spokenFunction) · save round-trip incl. legacy-profile neutral defaults ·
loadout-capacity sanity · object-count budgets (extend belt PerfBudget caps to plots/props).
**Device-only:** narration timing/ducking feel · plot interaction reach (PG-4 covers geometry)
· growth readability · 72FPS with N plots (soak).

## 7 · W002 GENERALIZATION TEST
W002 ships its cultivation content as **data only**: new species+recipe+catalog rows through
the same authors, ZERO new C#. If W002 needs runtime code, the architecture failed — that's
the pass/fail line, same as the world-factory order's replication proof.

## 8 · DO-NOT-TOUCH for the first slice
Recovery-frozen files (until lift), `PlantGenetics.Cross` semantics (saves depend),
`ResourceLedger`/`RewardRouter` internals, holster travel contract, `ProfileSerializer` old
version blocks, belt/machine save formats, first-hour envelopes, `_Boot`/rig/travel/input.

## 9 · KEEP NOW / KEEP LATER / CUT OR PARK
| Item | Call |
|---|---|
| Trait layer, recipe extension, discovery states, narration TEXT seam, 3-family W001 slice, loadout slotCost | **KEEP NOW** (post-M0) |
| Adjacency lesson (CrossPlots exists), machine-combined recipes, hybrids-as-authored-species, giant crops (built!) | **KEEP NOW/EARLY** — cheaper than the plan assumes |
| Conveyor connection of mastered recipes, processing tiers 3+, planet-signature catalysts, invention catalog full board | **KEEP LATER** (after manual loop device-proven) |
| Sprite helpers | **PARK** — new AI/art/save stack; revisit as presentation-only after Forge V |
| Multiplayer rotation | **PARK until release plan** — `multiplayerUse` flag already reserves the seam; rotation state stays out of campaign saves |
| Runtime-generated species, per-item narration clips, second crafting/inventory service | **CUT** |

## 10 · RED-TEAM SUMMARY (top 5, ranked)
1. **Silent duplication** — the plan's language ("may include data authorities…") invites
   rebuilding RecipeDefinition/GardenService under new names. This review's §1 map is the
   antidote; the reconciliation doc should adopt it as binding.
2. **Save combinatorics** — traits/lineage must stay ids+ints; anything richer makes migration
   unmaintainable by v5. (§4 rules.)
3. **Narration as content** — clips-per-item would add an audio-production tax to every future
   item forever. Text seam or nothing.
4. **WorldSpec pollution** — capability tags must be optional+additive on WorldPackDefinition
   (flags idiom); required fields would break all 12 existing packs and the stub generator.
5. **Timing** — none of this before M0 + W000→W001 production band (the plan already says it;
   the freeze enforces it; keep it that way).
