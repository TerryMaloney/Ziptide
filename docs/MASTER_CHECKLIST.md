# ZIPTIDE — MASTER CHECKLIST

**The one scannable "state of the build" page: what's BUILT, what's NEXT, and where we're HEADED.**
Skim this first; the deep docs are linked per item.

- **How to read status:** ✅ done · 🟢 backend built + CI-green (not yet device-verified / mostly
  stubbed) · 🟡 in progress · 🔲 planned/stubbed · 🔭 vision.
- **Most gameplay is "scaffold" quality** — it exists and compiles, but is rough and not all
  on-device-verified. That's expected; this list tracks reality, not polish.
- **Related docs:** [`STATUS.md`](STATUS.md) (current milestone dashboard) ·
  [`WORKLIST.md`](WORKLIST.md) (near-term punch list) ·
  [`ZIPTIDE_MASTER_BUILD_PLAN.md`](ZIPTIDE_MASTER_BUILD_PLAN.md) (deep long-term vision/architecture) ·
  [`docs/design/SYSTEMS_ARCHITECTURE.md`](design/SYSTEMS_ARCHITECTURE.md) (build order) ·
  [`docs/systems/`](systems/README.md) (per-system specs).

> Last updated: 2026-06-20.

---

## ✅ BUILT SO FAR (the foundation)

### Workflow / infrastructure
- ✅ CI: compiles + runs EditMode tests on every push (`terry-local-wip` green).
- ✅ **Cloud APK build** — CI builds the real APK (patch + audit) and uploads `ziptide-apk`; sideload, no Unity PC needed.
- ✅ **Sandbox auto-adds to Build Settings** during the build (no manual step).
- ✅ Shared cross-chat coordination log ([`HANDOFF.md`](HANDOFF.md)); world-integrity audit (blockers fail the build).

### Core / data / economy (🟢 backend, CI-green, not yet wired into live boot)
- 🟢 `PlayerProfile` + `ProfileSerializer` + `SaveSystem` (built, **not yet wired** into `_Boot`/world-entry).
- 🟢 Generic `DefinitionRegistry<T>` + definitions: Resource / Tool / Machine / Plant / Creature / Biome / Recipe / BalanceConfig.
- 🟢 `IdleEngine` (offline accrual), `EconomyState` (Mine/Plot), `ProfileEconomy` (resolve-on-entry math).
- 🟢 **Harvest v1**, **Mining/conveyor v1 + idle**, **Garden v1** (plant→tend→grow→harvest) — backend loops, tested.

### Gameplay / VR (🟡 on-device, varying verification)
- 🟢 VR locomotion (smooth/snap turn, dash/jump), play-area bounds (now opt-in), fall safety + EmergencyRespawn.
- 🟢 Belt + holsters; **Pistol** (C0); **Taser dart gun**; **Gravity gun** (new, in Sandbox).
- 🟢 **Drones** (`DroneRuntime` + `IShockable` + `HitZones`): taser shock→zone-based death reactions.
- 🟢 Scene **travel** (`TravelCoordinator` + `ProximityTravelTrigger`); `PlayerRigPersistence` + inventory across travel.
- 🟢 **Job system** (JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle); audio system (AudioDirector).

### Worlds & dev tools
- 🟢 Scenes: `_Boot`, `MilestoneA_GrabCube`, `D0_City` (legacy blockout), `SandboxTestLab`, **`StarterWorld`** (10-zone onboarding graybox), **`ToxicCity`** (new blueprint city), **`PvP_Arena01`**.
- 🟢 **ToxicCity WORLD BLUEPRINT** — `CityLayoutDefinition` + `CityBuilder` + `ScenePatcherToxicCity`: a data-driven, reusable "clone-a-world" recipe (districts/canals/hero interiors/shipyard). *(T-Dog)*
- 🟢 **Developer Warp** + **in-VR Dev Menu** (Y+B → warp any world, TMP fixed); Sandbox; scene-dump exporter.
- 🟢 **W001 story + first contract:** STORY.md beats + `ToxicCity_Contract` job (4 steps) with bounty **reward** (`JobDefinition.reward` + `JobRewards.Grant`, tested) → passage credits. *(Architect)* Runtime grant-hook + ObjectiveBoard/RILL text pending.

---

## ⚔️ PARALLEL TRACK — 1v1 PvP Mode (APPROVED, separate module)
A brand-new real-time **1v1 PvP** mode, fully separate from single-player. Plan:
[`design/PVP_1V1_MODE.md`](design/PVP_1V1_MODE.md). Decisions locked: **Photon PUN2** (sideload-friendly,
room-code invites), **solo-playable first**, **comfort-first gravity gun**. Distinct from Tidefront
(that's *async* strategy MP; this is *real-time* PvP).
- 🟢 **Phase 1 backbone BUILT** — `Ziptide.Multiplayer` pure-C# core (`PvpRules`/`PvpMatch`/`PvpCombatant`/
  `WeaponCharge`) + 14 EditMode tests *(Architect)*.
- 🟢 **Phase 2 BUILT (solo + bot)** *(T-Dog)* — `PvP_Arena01` + `ScenePatcherPvP`, `PvpPlayer`/`PvpBot`/
  `PvpMatchDirector`/`PvpHud`, `IPvpDamageable` weapon hits, **all 4 mechanics**: breakable walls +
  hammer (auto-return), wrist locator (hold→ping/cooldown), comfort gravity hop. Bot = the seam a remote
  player replaces. *Needs Terry to run `Build PvP Arena` once + on-device feel tuning.*
- 🔲 **Phase 3 (shared, Terry's PC):** import **Photon PUN2** + `Net/` adapter → 2-headset room-code match (swap bot for remote behind `IPvpDamageable`).
- 🔲 **Phase 4:** spawn-protection/disconnect/anti-cheat polish + 2nd arena.

---

## 🔜 SHORT-TERM (next up — the active loop)
- 🟡 **Device test pass (Terry, at the Quest):** run the one-time Unity menus (`Build Toxic City`,
  `Build Toxic City Contract`, `Build PvP Arena`) + commit the generated scenes/assets, then build & verify
  on-device: ToxicCity walkable + drones + bounty, PvP arena vs bot (4 mechanics), spawn/locomotion fixes.
- 🟢 **Drone Combat v1 BUILT** *(T-Dog)* — non-lethal patrol/engage + telegraphed stun bolts + `PlayerStunReceiver` ([`systems/DRONE_COMBAT_v1.md`](systems/DRONE_COMBAT_v1.md)). On-device tuning pending.
- 🔲 **JobDirector → `JobRewards.Grant`** runtime hook (pay the W001 bounty on completion; needs a live `PlayerProfile`).
- 🔲 **Creatures v1** (build-order #6): `CreatureDefinition` data/spawn/loot (Architect) + runtime AI (T-Dog). *(claimed)*
- 🔲 **Gun model swap + Quest grip offset** — drop the Tripo taser model in ([`systems/ASSET_SWAP_PIPELINE.md`](systems/ASSET_SWAP_PIPELINE.md)).
- 🔲 **Starter Gear Loop** — Left Wrist Scan Pulse + Gravity Glove (stun dart exists) ([`09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md)).

## 🟡 MID-TERM (once the core loop feels good)
- 🔲 **Wire the economy live**: hook `SaveSystem` + `ProfileEconomy.ResolveWorld` into `_Boot`/world-entry (idle/welcome-back).
- 🔲 **Tools & Repair** loop + **Build/Creator** mode ([`systems/TOOLS_AND_REPAIR.md`](systems/TOOLS_AND_REPAIR.md), [`systems/BUILD_CREATOR_MODE.md`](systems/BUILD_CREATOR_MODE.md)).
- 🔲 **World scaling pipeline** (`WorldStubGenerator`) — so 80 worlds aren't hand-built (MASTER_BUILD_PLAN E1/E2).
- 🔲 **Level 1 — Toxic Venice** full build; travel fade transition; **Alien Origami** art kit.
- 🔲 **Cloud save / cross-headset progress** (currently saves are per-headset, local).

## 🔭 LONG-TERM (the vision)
- 🔭 **80 worlds + 12-chapter story**, RILL companion, the Bloom — see [`ZIPTIDE_MASTER_BUILD_PLAN.md`](ZIPTIDE_MASTER_BUILD_PLAN.md).
- 🔭 **Tidefront** — Risk-style galaxy strategy + multiplayer ([`10_TIDEFRONT.md`](10_TIDEFRONT.md)).
- 🔭 **Gear/tools idea bank** — non-bullet explorer tech ([`09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md)).
- 🔭 **Ship system** — cockpit, modular kit ([`design/SHIP_SYSTEM.md`](design/SHIP_SYSTEM.md)).

---

## ⭐ NEXT BIG MILESTONE — Starter World Blockout (onboarding planet)
*Planned, not started — Terry wants a few short-term items done first.* GPT's 6/18 brief: graybox the
first real world as a **compact onboarding planet** (10 named regions: Hub → Spaceport/Vehicle Port →
Toxic City spine → Canals/Slum → Outskirts → Open Badlands → Mission Pocket → Dormant Ziptide gate),
walkable end-to-end, ~25–35 min, gateway to the multi-world premise. **Don't overbuild** — scale,
pathing, landmarks, placeholders over final art. Plan: [`design/STARTER_WORLD_BLOCKOUT.md`](design/STARTER_WORLD_BLOCKOUT.md);
full brief in [`GPT_ADDITIONS/2026-06-18_Starter_World_Blockout/`](GPT_ADDITIONS/2026-06-18_Starter_World_Blockout/01_architect_starter_world_blockout_brief.md).
- ⚠️ **Lane to reconcile:** the brief says "Architect = blockout," but in our repo scene/blockout is
  T-Dog's lane (Architect = backend/data, can't Unity-verify). Confirm owner before building.
- This is the first real user of the **world-scaling pipeline** (mid-term) and refines **Level 1 — Toxic Venice**.

---

*Keep this current: when something ships, move it up to BUILT with its real verification level. This is
the quick map; detailed tasks live in WORKLIST/TASK_QUEUE, deep vision in MASTER_BUILD_PLAN.*
