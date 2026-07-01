# HOW TO CHANGE ANYTHING — the modularity playbook

**For any operator (esp. Opus after Fable 5): find the thing you want to change, edit exactly what this
table says, and let the listed verifier prove it.** Everything here is data/asset-driven by design — if a
change seems to require editing runtime C# or scene YAML by hand, STOP and re-read the row; you're
probably about to break something that has a data path. Rows marked ⚠ are report-only zones (locked
contracts — confirm with Terry first).

Legend: **Edit** = the one place to change · **Then** = how it takes effect · **Verify** = the proof.

---

## Worlds

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Change a world's layout/geometry** (districts, streets, canals, buildings, landmarks) | that world's `CityLayoutDefinition` asset (`Content/City/…`) — positions/bounds/heights/connections are plain fields | the next build regenerates the scene (`WorldStubGenerator` build hook); or preview via `Ziptide → Worlds → Generate World From Selected Layout` | APK build's `WorldAuditRunner` (spawn/floor/trap checks); walk it on device |
| **Add a whole new world** | author a new `CityLayoutDefinition` with the `World identity` block filled (`sceneName` = `W###_Name`) — copy a `WorldLayoutLibrary` spec as the template | it auto-ships next build (scene + WorldPack + exit door + spawn + kiosk/board, Build Settings) | dev menu (Y+B) lists it; audit green |
| **Rewrite a world wholesale** | swap/replace its layout asset (keep `sceneName`) | build regenerates from the new data | same |
| **Change where the player spawns** | layout `spawnDistrictId` (or the pack's `spawnMarkers`) | regen | `ZIPTIDE: SPAWN_AT` in logcat |
| **Change a world's jobs/steps/rewards** | the world's `JobDefinition` + step assets (`Content/Jobs/…`), `reward` list, `completionFlag` | live next run (JobDirector reads the pack) | `ZIPTIDE: PACK_VALIDATION_FAIL` warns on slips; `JOB_REWARD_GRANTED` on completion |
| **Gate a world behind story progress** | the world's `WorldPackDefinition.flagsRequired` (use `ZiptideFlags` constants) | travel doors show it LOCKED until met | `ZIPTIDE: TRAVEL_LOCKED` / `WORLD_LOCKED` |
| **Grant story flags on world completion** | `WorldPackDefinition.flagsGranted` | granted when the contract finishes | `ZIPTIDE: WORLD_FLAGS_GRANTED` |
| ⚠ **Change ToxicCity specifically** | its layout asset for data; `ScenePatcherToxicCity.cs` for shell logic (hand-tuned reference world) | menu `Build Toxic City` / next build | audit + device |

## Sky / atmosphere / look

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Change a GENERATED world's sky/planet/ground tint** | its layout asset's **`Sky theme` block** (`skyHorizonColor`/`skyTopColor`/`themeGroundTint`/`planet*`) — the layout is the source of truth; the generator (re)writes `Content/Worlds/Themes/<Scene>_Theme` + `Profiles/<Scene>_WorldProfile` from it every regen | regen (build or menu) | look at it (device) |
| **Change a HAND-BUILT world's sky** (ToxicCity/D0/etc.) | that world's `VisualThemeProfile` asset directly (gradient bottom=horizon top=zenith, PlanetSettings, groundTint) | `WorldRuntime` applies on entry | device |
| **Hide the planet in a world** | layout `planetVisible = false` (renders at 1° = dim star) | regen | device |
| **Change fog** | the layout's `fogEnabled/fogColor/fogDensity` | regen | device |
| **City color identity** (concrete/metal/buildings/accent) | the layout's `palette` (or a district's `paletteOverride`) | regen | device; rules in `docs/design/CITY_DESIGN.md` |

## Weapons / gear

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Tune weapon gameplay feel** (fire rate, range, stun time, launch force, haptics, cooldowns) | the weapon's definition asset under `Resources/Items/` (Pistol/TaserDartGun/GravityGun definitions — all plain fields) | live next run (ItemFactory reads the asset) | fire it on device |
| **Tune weapon SIZE / COLOR / GRIP / MUZZLE** | same definition asset — `visualScale` (zero = keep default), `visualColor` (alpha 0 = keep default), `gripLocalPos`, `muzzleLocalPos` | live next run | grab it on device — grip angle/size feel |
| **Add a new item that can be spawned/holstered/travel** | new definition asset **under `Resources/Items/`** (CI FAILS if placed elsewhere or with a duplicate `itemId` — `ItemRegistryConventionTests`) + a creation branch in `ItemFactory` for new *types* | `ItemFactory.Create(itemId, pos)` anywhere | `ZIPTIDE: ITEM_REGISTRY` lists it at boot |
| **Give a weapon a real 3D model** | set `modelPrefab` on its definition (import the .glb first) | factory hook (visual swap is a small ItemFactory change if not yet wired for that type) | device |

## Creatures / drones

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Tune drone combat difficulty per world** | the `DroneCombatProfile` variants in `Resources/Enemies/` (`drone_easy` / `drone_standard` / `drone_veteran` — chapter bands, authored by `CreatureVariantAuthor`) — or add a new one | set its name as `variantId` on the world's `DroneZoneDef` (layout asset); regen | fight it on device |
| **Story-creature stats/loot** (Phase-E prep) | the `CreatureDefinition` assets under `Content/Creatures/Generated/` (swarm_bug, tendril, …) — data-ready; **nothing consumes them until Phase E's `CreatureRuntime`** | — | EditMode/CI |
| **Change where/how many drones spawn in a world** | the layout's `droneZones` (center/radius/count/respawnDelay/combat) | regen | `ZIPTIDE: DRONE_DOWN` etc. |
| **New creature *types* (non-drone)** | Phase E (`docs/systems/CREATURE_DESIGN.md`) — `CreatureDefinition` data exists; runtime behaviors not built yet | — | — |

## Story / progression

> **THE STORY-CHANGE PIPELINE (one direction, always):**
> `STORY_BIBLE.md` (canon prose) → the world's **`WORLD_DATA.md` record** (flags/jobs/markers/beats as
> data) → the **library spec** (`WorldLayoutLibrary`/`WorldJobLibrary`/`RillLineLibrary` entries) → the
> **next build** regenerates the world. Never edit downstream artifacts (packs/scenes) to change story —
> edit the record + library and let the factory re-author. This is what makes "change the story and the
> game follows" mechanically true.

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Story canon** | `docs/storyboard/STORY_BIBLE.md` + `THE_TRANSMISSION.md` (locked — deepen, don't contradict) | serialize changes into `WORLD_DATA.md` records → world/job assets | cohesion checklist in THE_TRANSMISSION §9 |
| **A world's story data** (flags, fragments, beats → mechanics) | its record in `docs/storyboard/WORLD_DATA.md`, then the matching pack/job assets | see Worlds rows above | validator + flag log tags |
| **Transmission fragments/clarity** | fragment flags live in `ZiptideFlags`; tier derivation in `TransmissionProgress` (pure, tested — thresholds are code by design) | clarity re-syncs after any grant | `ZIPTIDE: TRANSMISSION_CLARITY` |

## Economy
| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Job payouts** | each `JobDefinition.reward` | live | `JOB_REWARD_GRANTED` |
| **Idle/mine/garden rates & caps** | `BalanceConfig` / `MachineDefinition` / `PlantDefinition` assets; per-mine `storageCap` bounds offline accrual | resolved on world entry | `ZIPTIDE: ECON_RESOLVE` |

## PvP
| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Match rules** (HP, hit damage, charges, timers, best-of) | `Multiplayer/Runtime/PvpRules.cs` consts (pure, fully tested) | EditMode tests enforce the contract — update `PvpMatchTests`/`PvpCombatTests` with the rule change | CI |
| ⚠ **PvP scene/arena/HUD** | scene-side files under `Gameplay/Runtime/Pvp/` — pending Terry's device round; coordinate first | — | device |

## Ships / vehicles

| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **Ship size/cockpit/flight feel/comfort vignette** | the `ShipDefinition` asset (data-only today; the S1 boardable-shell runtime consumes it — build plan in `docs/systems/SHIPS.md`) | — | — |
| **Add an upgrade socket / a second hull** | `slots` list / a new `ShipDefinition` asset | — | — |
| **Where the berth+placeholder ship sits in a world** | the layout's `ShipyardBerthDef` | regen | device |
| ⚠ **Build the boardable ship (S1+)** | follow `docs/systems/SHIPS.md` phases — the ship is a **mobile travel station**; NEVER bypass `TravelCoordinator`, never parent the rig to the hull | — | device |

## Audio
| I want to… | Edit | Then | Verify |
|---|---|---|---|
| **World music/ambience** | the world's `AudioProfile` asset → `WorldPackDefinition.audioProfile` | `AudioDirector` applies on travel | device |

## ⚠ The do-not-touch-casually list (locked contracts — `CLAUDE.md`)
`TravelCoordinator` (only scene-change path) · `_Boot` ownership/rig singletons · `PlayerRigPersistence.EnsureXRIWiring`
(read `docs/systems/VR_RIG_GOTCHAS.md` FIRST) · `InputActionManager` · holster-only travel rule · no
reflection for item creation · never hand-edit `.unity`/`.prefab` YAML (patchers/data only).

---
*Verify workflow for ANY change: push → CI (compile + EditMode) → for scenes/feel, APK dispatch → Terry
sideloads. Log-tag reference: grep `ZIPTIDE:` in `CLAUDE.md` + this file.*
