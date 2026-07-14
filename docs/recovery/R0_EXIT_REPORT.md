# ZIPTIDE R0 EXIT REPORT — REPOSITORY TRUTH AND SYSTEM CONTRACTS

**Status:** CLOSED  
**Recovery owner:** GPT-5.6 Thinking  
**Branch:** `terry-local-wip`  
**Canonical inventory:** [`canonical_system_contracts.json`](canonical_system_contracts.json)  
**Validation:** [`generated/recovery_inventory_validation.md`](generated/recovery_inventory_validation.md) — 22 systems, 0 findings

## 1. R0 conclusion

ZIPTIDE is salvageable, but the previous development model was not safe for continued expansion.

The codebase contains substantial pure logic, generators, save/economy systems, world data, Forge tooling, PvP/conquest cores and prototype gameplay. The dominant failure is composition ownership: too many automatic runtime owners, independent presentation translators and hidden fallback paths were allowed to coexist without PlayMode, integrated visual or bounded Quest acceptance.

The recovery program therefore preserves valuable cores and data while replacing the integration layer through one golden slice.

## 2. Evidence gathered

### Repository cartography

- 598 C# files scanned.
- 2,013 exact ownership signals recorded across bootstraps, persistence, scene loading, runtime creation, UI, XRI, input, events, saves, global rendering, materials and fallback markers.
- 18 confirmed automatic/persistent runtime owners cataloged separately.
- 22 canonical responsibility-level systems now have exact source paths, a proposed surviving owner, proof level, Quest status, exposure class and next action.
- Canonical inventory validation: **PASS, 0 findings**.

### Scene exposure

- 24 Build Settings scenes.
- 24 enabled.
- Only `_Boot`, `W000_DriftIn` and selected destination `ToxicCity` belong in the first recovery candidate.
- Twenty enabled scenes are legacy tests, non-golden worlds or multiplayer prototypes.

### Completion-language audit

- 260 current-board lines contain strong completion language such as `SHIPPED`, `COMPLETE`, `VERIFIED`, `GREEN`, `✅` or `💎`.
- 138 do not name a proof qualifier on the same line.
- These are not automatically false; they prove the old status layer cannot be treated as the runtime acceptance ledger.

### Input contract audit

- 19 runtime-created bindings.
- Five cross-owner control collisions:
  - left thumbstick — ship and vehicle motion;
  - left thumbstick click — sprint, ship boost and vehicle boost;
  - right A — jump, ship boost and vehicle boost;
  - right B — quick-swap and ship roll;
  - right thumbstick — ship and vehicle motion.
- Two actual stale Y+B references remain:
  - `QuickSwap` suppresses B while Y is held because it still assumes Y+B is the dev menu chord;
  - `DashLocomotion` still logs Y+B as the menu control.
- The current `DevWarpBoard` itself uses forehead gesture, F2 or ADB. It is not the source of the stale Y+B assumption.

### Runtime owner audit

Priority recovery conflicts:

1. `RuntimeMaterialFixer` rewrites null/non-URP renderer materials after every scene load and invents green/blue/gray runtime materials outside authored art/audit evidence.
2. `RuntimeInputEnabler` globally reflects across action references and enables whole assets while `PlayerRigPersistence` separately owns manager/action adoption.
3. `EnsureXRCameraActive` disables cameras from parent-name heuristics rather than explicit roles.
4. `DebugHUD` and `CreditsHud` automatically contaminate the player view.
5. Conquest missions, PvP progression and Quarters camera features can still hook/inject while their features are supposedly parked.
6. `RuntimeHealthMonitor` runs `Resources.UnloadUnusedAssets` after loads; useful, but its hitch must be measured.

## 3. Canonical owner decisions

### Preserve as canonical cores/owners

- `BootLoader` — cold-start coordinator.
- `PlayerRigPersistence` — persistent rig and XRI/input-session owner.
- `TravelCoordinator` — sole scene-travel owner.
- `SaveSystem` — sole profile/disk owner.
- `JobRuntime` — semantic job/repair progress owner.
- `JobDirector` — scene adapter, runtime content coordinator and reward translator.
- `ObjectiveBoard` — read-only projection only.
- `CityBuilder` + `ShipHullBuilder` root + `ShipBoardingStation` + `ShipRefit` — one boarding-ship root contract.
- `MeleeWeaponRuntime` item path — surviving physical melee foundation.
- `AudioDirector` — composition owner; `AmbienceDirector` subordinate biome provider.
- WorldSpec/compiler/generator and Forge pure/data pipelines.

### New contracts required

- `RecoveryRuntimeGate` and recovery exposure profiles.
- `DiegeticPanel` world-space UI framework.
- central mode-aware `InputContract` registry.
- explicit camera role registry/player-view owner.
- build-time material validation and explicit fallback policy.
- scene presentation coordinator for sky/grade/fog/light.
- item presentation schema for scale, hand, holster, muzzle and impact axes.
- shared weapon-owner/self-collision policy.
- creature contact/grounding contract.

### Merge or retire

- retire automatic `RuntimeMaterialFixer` after replacement proof.
- retire/gate `RuntimeInputEnabler` after canonical input-session proof.
- replace name-based camera enforcer.
- remove always-on Credits HUD and default-on debug overlay from GoldenSlice.
- retire Y+B menu assumptions.
- retire standalone primitive `HammerTool` after arena migration.
- migrate Sonic Thumper effect onto shared tracked-tip melee contact.
- make ship cast-off consume semantic job completion rather than rediscovering machine visual state.
- remove independent raw TextMesh/layout/facing infrastructure as panels migrate.

## 4. GoldenSlice exposure

### Scenes

1. `_Boot`
2. `W000_DriftIn`
3. `ToxicCity` / W001

W001 is selected because it is the intended first real destination. Choosing W002 would bypass the integration path the recovery must prove.

### Required player path

`Cold boot → choose/resume → W000 → travel to W001 → use one correctly presented tool → resolve one grounded creature non-lethally → complete one job → receive and spend one reward → use/board the canonical ship → return → quit and resume`

### Hidden during recovery

- broad world-door selection;
- Quarters;
- Tidefront table;
- PvP/Photon;
- zipline;
- free flight;
- hammer/melee pair;
- non-golden scenes;
- unverified practical-light quads;
- legacy striped planet path;
- visible runtime material fallback;
- experimental vehicles, gardens, factories and broad catalogs.

Nothing is deleted. Hidden means unavailable in the recovery candidate and unable to auto-bootstrap.

## 5. Proof model after R0

Player-facing completion requires explicit lanes:

- `SOURCE`
- `CORE`
- `PATCHED`
- `PLAYMODE`
- `VISUAL`
- `APK`
- `QUEST`

`CI GREEN` is not a proof level. The recovery verdict must list which lanes ran and which did not.

The current codebase has substantial SOURCE/CORE/PATCHED/APK proof. It has almost no automated PLAYMODE or integrated VISUAL proof and several failed Quest observations.

## 6. R1 entry decision

R1 is authorized.

The locked implementation order is:

1. one-frame PlayMode infrastructure spike;
2. repeat-green proof on an unrelated descendant;
3. tests-only fake tracked rig;
4. runtime owner/object census;
5. recovery exposure profile;
6. Home Hub ray/select smoke;
7. `_Boot → W000 → ToxicCity → W000` travel/save round trip;
8. renderer-capable snapshot spike;
9. UI spatial findings;
10. fallback visibility findings;
11. performance/resource artifact bundle;
12. first recovery APK checkpoint.

Terry performs no headset work until the first checkpoint already passes the available automated lanes.

## 7. R0 artifacts

- [`RECOVERY_PROGRAM.md`](RECOVERY_PROGRAM.md)
- [`canonical_system_contracts.json`](canonical_system_contracts.json)
- [`automatic_runtime_owners.json`](automatic_runtime_owners.json)
- [`canonical_owner_decisions.json`](canonical_owner_decisions.json)
- [`r0_decision_closures.json`](r0_decision_closures.json)
- [`recovery_exposure_manifest.json`](recovery_exposure_manifest.json)
- [`R1_INTEGRATION_HARNESS_SPEC.md`](R1_INTEGRATION_HARNESS_SPEC.md)
- [`generated/recovery_contract_scan.md`](generated/recovery_contract_scan.md)
- [`generated/recovery_contract_map.md`](generated/recovery_contract_map.md)
- [`generated/recovery_scene_exposure.md`](generated/recovery_scene_exposure.md)
- [`generated/recovery_claim_audit.md`](generated/recovery_claim_audit.md)
- [`generated/recovery_input_contract_scan.md`](generated/recovery_input_contract_scan.md)
- [`generated/recovery_source_resolver.md`](generated/recovery_source_resolver.md)
- [`generated/recovery_event_save_graph.md`](generated/recovery_event_save_graph.md)
- [`generated/recovery_focus_reference_graph.md`](generated/recovery_focus_reference_graph.md)
- [`generated/recovery_workflow_status.md`](generated/recovery_workflow_status.md)

## 8. Exit gate result

- Critical responsibilities mapped: PASS.
- Exact source paths: PASS.
- Canonical owner or explicit replacement contract: PASS.
- Proof/exposure status recorded: PASS.
- Automatic owners cataloged: PASS.
- Input, scene, event/save and focused decision graphs: PASS.
- Canonical inventory validator: PASS, zero findings.
- Runtime behavior changed during R0: NO.

**R0 CLOSED. R1.1 may begin.**
