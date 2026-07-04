# ZIPTIDE META-LOOP — the one-economy spine (locked 2026-07-04, Fable window)

> **THE BUILD PLAN LIVES HERE.** Boards reference this doc; Terry's original brief + GPT's addendum
> are condensed in the Appendix. Status: SPINE SHIPPED + GOLDEN LOOP PROVEN (see §Implemented).

## The loop (north star)
```
CAMPAIGN exploration ──rewards──▶ seeds/salvage/artifacts/scans (ledgered: campaign_reward)
      ▲                                   │ plant
      │ upgrades/ammo/modules             ▼
      │                          GARDEN (PlotState: pure elapsed-time growth, offline-friendly)
      │                                   │ harvest (ledgered: garden_harvest)
      │                                   ▼
      └──────── FACTORY (ProductionGraph: abstract tick sim over RecipeDefinitions;
                          inputs ledgered recipe_cost, outputs factory_output)
                                          │ components/defenses (spent: upgrade_cost)
                                          ▼
                 CONQUEST async macro (Conquest* cores; same worlds, same resources)
                                          │ income (ledgered: multiplayer_reward)
                                          └────────▶ back into the same profile
```

## Source-of-truth table
| Thing | Truth | Visuals/scene |
|---|---|---|
| Resource ids | `ResourceDefinition` assets in `Resources/Economy` (EconomyAuthor seeds; **RESOURCE_ID_UNREGISTERED fails the build**) | — |
| Balance movement | `PlayerProfile` + **ledger** (every grant/spend through `RewardRouter`) | HUD reflects |
| Garden growth | `PlotState` (elapsed unix time) | `GardenPlotRuntime` reflects |
| Factory | `WorldState.factory` (`MachineNodeState` layout + progress); `ProductionGraph` simulates | future conveyor skin animates REPRESENTATIVE items only |
| Recipes | `RecipeDefinition` (`Resources/Recipes`) — costs+produces+machineType+ticks+unlockFlag+story/world tags | — |
| Multiplayer macro | `Conquest*` pure cores (async; command-model extension below) | arena/scene layers |
| Staleness | computed: `EconomyFlowModel.AffectedBy(worldId/storyTag)` + `docs/_generated/ECONOMY_FLOW_REPORT.md` | — |

## THE MODE CONTRACT (hard law)
- No mode owns a private economy. All grants/spends go through **`RewardRouter.Grant/TrySpend`**
  (which writes the ledger). Direct `AddResource` outside RewardRouter/migration/tests = violation.
- No mode invents resource ids: every id needs a `ResourceDefinition` (build-failing gate).
- No private save containers: extend `PlayerProfile`/`WorldState` additively + a `schemaVersion`
  migration case (`ProfileSerializer.Migrate`) + a fixture test (`OldSave_MigratesForwardClean`).
- Modes expose outputs as reward grants / unlock flags / state deltas — shared world state owns truth.
- **No pay-to-win, ever**: combat/territory/resource advantage comes from gameplay systems only.
  Cosmetics stay looks (CosmeticDefinition law: "a LOOK never a stat").

## The golden proof
`GoldenMetaLoopTests.GoldenMetaLoop_CampaignGardenFactoryConquest_RoundTripsThroughLedger()` runs
the WHOLE chain (campaign job → garden → offline growth → harvest → factory recipe →
stun_charge_cell → defense spend → conquest income → ledger explanation → save/load → staleness
lookup) deterministically in EditMode. **It is the acceptance test: green = the spine holds.**

## Wired chokepoints (all ledgered today)
`JobRewards.Grant` (campaign) · `GardenService.Harvest` (garden) · `ProfileEconomy.CollectMine` +
`ProductionGraph.Tick` (factory) · `RecipeService.TrySpend` (recipe_cost) ·
`BuildSocketRuntime.TryBuild` (upgrade_cost) · `CreatureRuntime` loot + `ResourceNode.Harvest`
(campaign). **Conquest income: enveloped to the architecture track** — route through
`RewardRouter(LedgerSource.Multiplayer)` when resolve pays out.

## Async multiplayer command model (contract for the architecture track)
Extend Conquest as commands, not live object state, so a future backend reuses the model verbatim:
`PlaceDefenseCommand` (spends a DefenseComponent resource) · `StartAttackCommand` ·
`ClaimIncomeCommand` (→ RewardRouter multiplayer_reward) · `ResolvePlanetConflictCommand`
(deterministic, cooldowns, offline resolution). No live VR netcode before these contracts pass tests.

## Proxy/visual contracts (for Picasso's kits — reserved, not built)
- Garden bed: plot size · plant-stage socket · harvest anchor · scan anchor · quality state.
- Machine: input/output ports (`inputNodeIds`) · belt direction (`rotationSteps`) · power socket ·
  upgrade socket · `visualProxyId` (Art Registry id) · collider contract.
- Planet/defense: planet id · defense slot · income display · art/world tokens.

## Garden = Ziptide ecology, not generic farming (reserved fields, sim later)
PlantDefinition additions when content wants them: `sourceWorldId`, `biomeTags`, `stewardshipTags`,
`mutationFamily`, `hazardLevel`, `rillAnalysisState`, `harvestRisk`, `ecologicalRole` — e.g.
memory_lichen (W007): stores gate echoes; yields memory_shard/data_spore; corrupts neighbors if
untended; RILL state "unstable". Balance hooks already exist on ResourceDefinition
(`expectedSource/SinkPerHour`, `progressionPhase`).

## Implemented now (this lock) · Deferred (trigger) · Never
- **Now:** ResourceDefinition registry + author + gate · ledger + RewardRouter + 8 chokepoints ·
  RecipeDefinition factory fields · ProductionGraph (validate/tick/capped catch-up) ·
  `WorldState.factory` · schemaVersion v2 + migration + fixture · EconomyFlowModel +
  no-source/no-sink/unused WARNs + generated flow report · the golden test.
- **Deferred:** conveyor visual skin (trigger: factory device-fun proven) · plant
  mutations/conditions (trigger: garden content wave) · decay/storage rules · leagues ·
  defense content · conquest command model (architecture track, next) · live co-op (after async
  contracts pass).
- **Never:** pay-to-win · per-mode economies · per-item conveyor physics · visuals as truth ·
  story-specific hardcoding in core systems.

## Future Model Rules (the kid-gloves rails)
1. New resource id? `EconomyAuthor` first — the gate will red the build otherwise.
2. Move resources ONLY via `RewardRouter`. 3. No new save containers; additive fields + migration
   case + fixture test. 4. Factory/garden visuals reflect state, never own it. 5. No live netcode
   before async command contracts have passing tests. 6. Don't hardcode story into core systems —
   tags + definitions. 7. All of OPERATOR_START_HERE's laws still apply (circuit breaker included).

## Appendix — Terry's brief + GPT addendum (condensed, faithful)
A ResourceRegistry (id/category/source_worlds/story_tags/rarity/stack/storage/multiplayer_use) ·
B InventoryLedger (tick/resource/delta/source_system/reason/related/world) · C Garden data-driven,
sim=metadata, visuals reflect · D Factory graph-based, machine types {input_bin…power_node},
recipes {inputs/outputs/machine/duration/unlock/story refs/mode use}, NO per-item physics ·
E OfflineTickSimulator (deterministic, capped catch-up) · F ModeRewardRouter (campaign/garden/
factory/multiplayer reward classes; no isolated economies) · G async metadata multiplayer first
(planet control, income rules, defense slots, resolve rules, cooldowns, anti-P2W, campaign links) ·
H save schema (ledger/plots/graph/queues/recipes/progress/planets/pending/migration) · I staleness
integration with Forge (affected resources/plants/recipes/machines/defenses/art on story change) ·
J production-aware proxies · K deterministic tests incl. catch-up cap + staleness · L this doc ·
GPT: golden loop test (done) · mode contract (above) · flow report + NO_SOURCE/NO_SINK/UNUSED/
RECIPE_OUTPUT_UNREGISTERED audits (done) · balance hooks (done) · ecology fields (reserved) ·
layout-vs-skin factory law (done) · command-model multiplayer (enveloped) · no-P2W (law) · save
fixtures (done) · future-model rules (above).
