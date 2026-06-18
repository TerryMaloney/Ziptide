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

> Last updated: 2026-06-18.

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
- 🟢 Scenes: `_Boot` (persistent rig owner), `MilestoneA_GrabCube` (test room), `D0_City` (Toxic Venice blockout), `SandboxTestLab`.
- 🟢 **Developer Warp** + **in-VR Dev Menu** (Y+B → warp any world); Sandbox Test Lab; scene-dump exporter.

---

## 🔜 SHORT-TERM (next up — the active loop)
- 🟡 **Device-verify the latest CI APK**: gravity gun + drones in Sandbox, invisible-wall fix, holster-travel, dev-menu re-click.
- 🔲 **Creatures v1** (build-order #6): `CreatureDefinition` data/spawn/loot (Architect) + runtime AI (T-Dog). *(claimed)*
- 🔲 **Gun model swap + Quest grip offset** — drop the Tripo taser model in ([`systems/ASSET_SWAP_PIPELINE.md`](systems/ASSET_SWAP_PIPELINE.md)).
- 🔲 **Drone Combat v1** — orbit/strafe movement + telegraphed stun bolt + screen-obscure hit response ([`systems/DRONE_COMBAT_v1.md`](systems/DRONE_COMBAT_v1.md)).
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

## 📥 GPT's new direction — PENDING (Terry to provide)
*Reserved slot.* Terry mentioned a new direction from GPT (a few items ahead of current work). The
file/message hasn't landed in the repo yet — once Terry pastes it or commits it under
`docs/GPT_ADDITIONS/`, fold it into the right tier above and link it here. **Do not invent this.**

---

*Keep this current: when something ships, move it up to BUILT with its real verification level. This is
the quick map; detailed tasks live in WORKLIST/TASK_QUEUE, deep vision in MASTER_BUILD_PLAN.*
