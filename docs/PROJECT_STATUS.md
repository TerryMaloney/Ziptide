# 🌊 ZIPTIDE — PROJECT STATUS (ground-truth, code-surveyed)

> **Built from the CODE, not the docs.** 2026-07-08, head `d6c887f`, branch `terry-local-wip`,
> **CI green** (armor-only combat lock passed). This is a fresh survey of the actual C# in
> `Ziptide/Assets/Ziptide/` so the status reflects what's really shipped — the older
> `MASTER_CHECKLIST.md` (2026-07-01) and `PRIORITIES.md` rev 8 both lag reality (see "Doc drift").
>
> Legend: **🟢 Built** = deep + working (usually tested) · **🟡 Partial** = spine exists, the
> playable/visible half is missing · **🔴 Not built** = design/seam only, no working feature.

---

## The one-paragraph truth
The **machine** is impressively deep — world generation, economy, combat, PvP, conquest, and the
Forge art pipeline are all real, tested spines. What's thin is the **content and the last-mile
wiring** that turns a spine into something you *feel* in the headset: worlds that don't render their
buildings, a garden with 4 plants, a flight model with no cockpit, "machines/conveyor" with zero
physical belts, and online MP that shows other players but can't yet trade shots. That gap — spine
built, payoff not wired — is exactly the "too simple" you felt on device.

---

## 🟢 BUILT — deep, working, mostly tested

| System | What's actually there | Tests |
|--------|----------------------|-------|
| **World generation spine** | `WorldSpec → WorldSpecCompiler → CityLayout → WorldExperienceBuilder / PoiBuilder / DressingBuilder / BuildingBuilder`. `TerrainField` **is** wired into the experience builder. 8 audit gates (`Building/Economy/Experience/Forge/PerfBudget/SkyVista/WorldContent/WorldReachability`). | ✅ |
| **Economy spine (META-LOOP)** | One economy: `EconomyState / ResourceLedger / ProfileEconomy / IdleEngine / RewardRouter`; `ProductionGraph` pure factory sim. Golden meta-loop test pins the full flow. | ✅ |
| **Combat / weapons** | 8 weapon runtimes + 8 item defs (Taser, Pistol, Gravity, StaticNet, SonicThumper, PrismBeam + melee Breaker Blade / Tide Pike / Hammer). Full control scheme: jump / sprint / auto-run / crouch / slide / dash. Stun-based (`PlayerStunReceiver / IShockable`). | ✅ |
| **PvP arena (local + bots)** | 5 modes (Deathmatch, Gun Game, KotH, Fragment Rush, Horde) as deterministic state machines. 5 procedural arenas. `BotBrain` 8-state FSM, 4 difficulty profiles. Weapon pads, N-way kill credit. | ✅ |
| **Tidefront conquest (sim)** | Full deterministic sim: `ConquestState / Rules / Resolver / Catalog / Galaxy / AI` over the 12 story worlds; anti-snowball economics. | ✅ |
| **Forge art pipeline** | Geometry + material + **texture** (`ForgeUV / ForgeTexture / ForgeBaker`, E1.1–**E1.4**). Baker is build-hooked → **textured assets ship ON DEVICE** (flat-color is only the editor fallback). SkyVista system (~19 vistas). | ✅ |
| **Travel & persistence** | `TravelCoordinator` (only way to change scenes), rig + holstered-inventory persistence across loads. | ✅ |
| **Jobs** | `JobDirector / DispatchKiosk / ObjectiveBoard / DeliveryCradle` + `AudioDirector`. | partial |
| **Creatures — behavior** | ~10 behavior components (Swarmer, WallCrawler, Flyer, Bruiser, HuskMolter, LightGrazer, TetherSwarm, WitnessMite, Warden, DroneCombat) + Forge gait/skinning pipeline (`ForgeGaitMotor`). | ✅ |
| **CI** | Compile + 64 EditMode test files + APK build with world-audit-as-blocker. | — |

---

## 🟡 PARTIAL — spine built, the payoff half is missing

| System | Built | Missing (the gap) |
|--------|-------|-------------------|
| **Story worlds** | 12 worlds authored (W000, W002–W012). Generation spine solid + tested. | **11 of 12 render NO grammar buildings** — only W002 sets `buildingStyleId`. Only 2 building-style assets exist. Worlds read as empty lots. |
| **Garden** | Pure `GardenService` + tests; plant→tend→grow→harvest; overripe decay to a 50% floor. | Only **4 plants**; fresh-harvest bonus deferred (`FreshBonus = 0`); **no hands-on tools** (watering can, prune), no pests/weather/cross-breeding. |
| **Story / RILL** | 72 authored lines; per-world enter beats for W000–W012; `RillCompanion` + subtitles. | Authored arc C1–C12 keys off **unbuilt worlds** (W013, W019…). 4 endings are **dialogue-only** — no ending scene/runtime. No companion memory/callbacks. |
| **Online multiplayer** | Photon PUN2 **imported + activated** (`ZIPTIDE_PHOTON` on, AppId set). `PvpOnlinePresence` streams head+2 hands @20 Hz — you see other players move. | **Combat sync not built** (A6.2). No networked fire/hit/score authority → you can't actually *fight* across two headsets yet. |
| **Save system** | `ProfileSerializer` serialize/migrate logic built + tested. | `SaveSystem` **not wired into `_Boot`** or travel-autosave — nothing persists between sessions yet. |
| **Building kits (art)** | `ArtModuleRegistry` id→factory seam exists. | **Zero `Register()` callers** — every `buildingModule:` / `surfaceSet:` id falls to primitive fallback. No authored kits. |
| **GamePool** | Pure pool + `PoolCoreTests` (built + green). | **Adopted at zero call-sites** — a ready-but-unused utility (projectiles still `Instantiate`). |

---

## 🔴 NOT BUILT — design/seam only, no working feature

| Pillar | Reality |
|--------|---------|
| **Free-flight ("PUNCH IT" / the crown jewel)** | `FlightModel` is **pure math only** (tested). **No flight scene, not playable.** Only consumer is a rails takeoff. Terry already put the fuel cell in — waiting on the scene. |
| **Machines / conveyors** | The whole "machines/conveyor" pillar has **zero physical belts/hoppers**. `ProductionGraph` is an abstract graph; `NodeKind.Conveyor` is "representative visuals only, never truth." |
| **Tidefront VR war table (B2) + mission modifiers (B3)** | Sim is built + tested; **no MonoBehaviour, no VR table, no UI.** Nothing you can touch in-headset. |
| **Adaptive audio** | Only `AudioDirector` (scene-crossfade music). No `AdaptiveAudioManager`, no layers/ducking. |
| **RILL voice-over** | Subtitle-only; **no VO recorded** (slots reserved for the M6 audio pass). |
| **Creature ecology** | Behaviors exist per-creature but **no nests, packs, territory, or population sim**. Only 2 Forge creature bodies authored. |
| **Weapon depth** | No ADS / reload / magazine. Charge is PvP-only. |
| **MP progression / augments / dual-wield** | MP100 board: all designed, **none built** (stats, credits, unlock ladders, augments, locator v2). |

---

## Doc drift caught by this survey
`PRIORITIES.md` rev 8 and `MASTER_CHECKLIST.md` are behind the code:
- **FORGE II E1.4 (device textures)** — PRIORITIES lists it `⬜ next art`; **it's shipped** (`ForgeBaker.cs`, build-hooked).
- **PERF_BUDGET gate** — PRIORITIES lists it `⬜ unowned`; **it's built** (`PerfBudgetAuditRules.cs`, E5.2, runs in the audit).
- **GamePool (Q4a)** — marked board-ready; **it's built + tested**, just not adopted.

So three "next up" priority rows are actually already done. Real remaining top-of-list items: Terry's
headset pass, **free-flight**, and content/wiring — not the art-pipeline rows.

---

## Current priority order (PRIORITIES.md rev 8, corrected for drift)
1. **🧑 Terry headset pass** on the latest APK — closes device ❌s (tiny guns, matchboard spawn).
2. ~~FORGE II E1.4~~ — **done in code.**
3. ~~PERF_BUDGET gate~~ — **done in code.**
4. **Free-flight v1** — the Star Wars pillar; FlightModel tested, needs a scene.
5. ~~GamePool~~ built; remaining is **adoption** at projectile call-sites.
6. **META-LOOP economy follow-through** — keep the spine honest before content scales.
7+. World recipe handbook, Forge creatures P2–P4, building modules, two-Quest online, augments, Tidefront table.

## Open device feedback (Terry's ❌s, not yet closed)
- **1.1** laser guns "suuuper tiny" — needs real sizes on PrismBeam/SonicThumper/StaticNet (awaits next logcat).
- **1.4** "stuck under the level in matchboard" — arena spawn-Y ground sample (diagnostic shipped, awaits logcat).
- **2.2** Aim v2 — laser sights + dot reticle + optional aim-assist.
- **Wave 3** — flight v1, destruction v2, HUD/how-to-play, building read.
- **Wave 4** — gardens/build-sockets showing up in worlds, detail-density pass.

---

## 🎯 Recommended "crush" candidates (highest leverage first)
1. **Free-flight v1** — biggest wow-per-commit; the math is done and tested, it just needs a cockpit scene. Directly closes Wave-3 "crown jewel."
2. **Building content** — set `buildingStyleId` on the 11 empty worlds + author 2 real kits through `ArtModuleRegistry`. Instantly turns empty lots into cities. Closes "worlds feel empty."
3. **Physical machines / conveyors** — the entire pillar you named has zero touchable objects today. Highest gap vs your AAA ask.
4. **Garden depth** — watering can (hands-on tend), more plants, harvest pop/haptics. Cheap, VR-native, and you named grow-a-garden.
5. **Story delivery** — RILL memory + callbacks and author beats for the worlds that *exist*; wire the 4 endings. Haiku workshop is ready to draft.
6. **Online combat sync (A6.2)** — presence works; making shots land across headsets is the difference between "we see each other" and "we're playing."
