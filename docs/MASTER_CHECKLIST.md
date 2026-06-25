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

> Last updated: 2026-06-21. **🚀 North star: the Ship** (hub + fly-between-worlds) — see LONG-TERM.
> **🆕 Fable-5 prep complete:** [`FABLE5_START_HERE.md`](FABLE5_START_HERE.md) (primer) · [`CODE_SCORE.md`](CODE_SCORE.md) (3.5/5) · [`ROLES.md`](ROLES.md) · [`FABLE5_BACKLOG.md`](FABLE5_BACKLOG.md) · [`design/WORLD_FLOW_TEMPLATES.md`](design/WORLD_FLOW_TEMPLATES.md) · [`systems/CREATURE_DESIGN.md`](systems/CREATURE_DESIGN.md).

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

## 🔜 SHORT-TERM (now → this device session) — *prove it plays well on the headset*
- 🟡 **Device test pass (Terry, at the Quest):** run the one-time Unity menus (`Build Toxic City`,
  `Build Toxic City Contract`, `Build PvP Arena`) → **`Dev → Rebuild Dev World Manifest`** (else new
  worlds won't show in the Y+B menu) → commit → build & verify: ToxicCity walkable + drones + **bounty pays**,
  PvP vs bot (4 mechanics), spawn/locomotion fixes.
- 🟡 **On-device feel tuning** — wrist scanner, hammer swing, gravity hop, gun grip, combat pacing.
- 🔲 **PvP Phase 3** — import **Photon PUN2** → real 2-player over room code (the `IPvpTransport` seam is ready). *The "play with a friend/the kids" payoff.*
- 🔲 **W001 polish** — ObjectiveBoard/RILL text so the first contract reads as story, not just steps.
- 🟢 **Done:** Drone Combat v1 *(T-Dog)*; bounty payout wired (`JobDirector → JobRewards.Grant`, *T-Dog* on my reward system).

## 🟡 MEDIUM-TERM (next few weeks) — *a complete, repeatable single-world loop + tools to scale*
- 🔲 **Economy live & meaningful** — idle/welcome-back on world entry; make credits matter (spend/upgrade) ([`ProfileEconomy`]).
- 🔲 **Creatures v1** + **Tools & Repair** + finish the **gear set** (gravity glove, expanded stun dart — scan pulse done).
- 🔲 **World scaling pipeline** (`WorldStubGenerator`) — worlds become data, not hand-built (ToxicCity blueprint already proves the pattern).
- 🔲 **Real art swap** — Tripo models replacing graybox (gun, drones, key props) ([`systems/ASSET_SWAP_PIPELINE.md`](systems/ASSET_SWAP_PIPELINE.md)); travel fade + **Alien Origami** kit.
- 🔲 **Cloud save / cross-headset progress** (saves are per-headset today).

## 🔭 LONG-TERM — 🚀 **THE SHIP IS THE NORTH STAR**
**Headline goal:** the spaceship is the keystone that turns a set of separate worlds into one connected
game — the hub, the travel system, the progression sink, and the on-ramp to the whole vision. Almost
everything already points at it (the shipyard berth + static ship exist in ToxicCity; the bounty earns
"passage credits" to *undock*; travel/persistence/economy/save are in place; "fly between worlds" is the
natural upgrade of the travel door).
- 🔭 **The Ship v1** — board your berthed ship, real **cockpit interior**, and **leave a world by flying
  out** (replacing the placeholder travel door). The moment ToxicCity becomes "the first stop," not "a level."
- 🔭 **Ship = the hub / world-select** — choose your next planet from the cockpit; the ship *is* the menu between worlds.
- 🔭 **Ship customization / upgrades** — credits sink that ties the economy together ([`design/SHIP_SYSTEM.md`](design/SHIP_SYSTEM.md)).

Then the ship unlocks the rest of the vision:
- 🔭 **80-world / 12-chapter campaign** — RILL waking, the Bloom, the Earth/containment reveal. **Story
  fully designed:** [`storyboard/STORY_BIBLE.md`](storyboard/STORY_BIBLE.md) (locked meta, ⭐ Terry review)
  + the **identity layer** [`storyboard/THE_TRANSMISSION.md`](storyboard/THE_TRANSMISSION.md) (Cal = the
  Debugger; the self-message; the trapped partner; ⭐ Terry review)
  + per-world template + **all-80 seed catalog** (`storyboard/CHAPTER_*.md`) + deep Ch.1 READMEs (W001–W004).
  Each world ties story → real buildable data (biome/machines/crops/enemies/sky). Canon table:
  [`ZIPTIDE_MASTER_BUILD_PLAN.md`](ZIPTIDE_MASTER_BUILD_PLAN.md) §12.
- 🔭 **Tidefront** — the ship's holo-table commanding planets; Risk-style galaxy strategy + MP ([`10_TIDEFRONT.md`](10_TIDEFRONT.md)).
- 🔭 **Gear/tools idea bank** — non-bullet explorer tech ([`09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md)).
- 🔭 **Adaptive Audio Layer** — stem-mixing dynamic music (global `ThreatLevel 0→1`, "Halo → Beastie
  Boys") + Quest **Audio-LOD** diegetic SFX + per-world `PlanetAudioProfile` + stem auto-importer.
  Evolution of `AudioDirector`; one audio asset per world (fits the World Blueprint). **PLANNED —
  architecture only, not started** ([`design/ADAPTIVE_AUDIO.md`](design/ADAPTIVE_AUDIO.md)).

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

## 🧪 PARKED IDEAS — explore *after* the full game ships (don't build now)
Captured so we don't lose them; both depend on the architecture staying clean (data-driven worlds +
swappable content), which is exactly what we've been building toward.
- 🧪 **Community World-Builder + World-vs-World tournaments.** Get our world-authoring pipeline
  (`CityLayoutDefinition`/`WorldPackDefinition`/definitions) into a **player-facing builder**, then let
  people **battle their world against someone else's, tournament-style**. Big replay loop. *(Leans on the
  same data pipeline + the PvP/netcode work; post-launch.)*
- 🧪 **"Adult" content variant (swappable).** Once the base (all-ages, non-lethal) game is complete, a
  mature variant that swaps the stun/non-lethal layer for brutal/violent content. **Design intent: make
  it a content swap, not a rewrite** — keep combat effects/gore behind a data/profile layer so it can be
  dropped in later without touching core systems. *(Validates our "content is data" architecture; post-launch, gated.)*
- 🧪 **Optical-illusion mechanics** — anamorphic reveals / impossible geometry / illusion-camo enemies for
  the **Pattern** worlds; the meta payoff = a world only coherent from the Observers' viewpoint. Plan:
  [`design/OPTICAL_ILLUSIONS.md`](design/OPTICAL_ILLUSIONS.md). *(Comfort-gated, visual-not-vestibular; explore later.)*

---

*Keep this current: when something ships, move it up to BUILT with its real verification level. This is
the quick map; detailed tasks live in WORKLIST/TASK_QUEUE, deep vision in MASTER_BUILD_PLAN.*
