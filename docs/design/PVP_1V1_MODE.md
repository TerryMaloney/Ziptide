# PvP 1v1 Mode — plan (APPROVED)

**Status:** Plan approved by Terry. **Phase 1 backbone BUILT** (Architect) — `Ziptide.Multiplayer`
pure-C# core + 14 EditMode tests (`PvpRules`/`PvpMatch`/`PvpCombatant`/`WeaponCharge`). Everything else
below is planned. This is the repo-committed copy of the approved plan so all chats (T-Dog/Architect/GPT)
can see it.

## Context

Terry wants a brand-new **real-time 1v1 PvP mode**, completely separate from the single-player game. He
has **two headsets in developer mode** and wants to **invite a friend**. Core asks: a small-but-not-tiny
multi-level arena (anti-camp); a starting **taser** (3 hits = kill, with a recharge); a **gravity gun**
(knockback + ~half taser damage); **one gun in hand at a time**; a **left-wrist locator** (hold 3s, ~60s
cooldown, finds the other player in range); a **hammer** that breaks temporary holes in interior walls
(not floors/exterior; small shoot-through + bigger holes; holes regenerate after ~3 min; picked from the
tool belt and auto-returns after 2 min off-belt); **best-of-10 kills**; and a **game-variant menu** (or
fold into the Y+B dev menu). Must be **expandable** (more maps/modes later).

## Decisions (locked with Terry)

- **Networking: Photon PUN2** — runs on sideloaded dev-mode headsets (no Meta store publish), room-code
  remote invites over the internet, free 100-CCU tier, Photon Voice bundled. Skip Meta Platform SDK.
- **First deliverable: a solo playable arena** (mechanics first, networking after).
- **Gravity gun: comfort-first** self-knockback (short hop/mini-teleport + vignette), not a velocity launch.
- **Target:** remote room-code play; the friend **sideloads the same APK** to join (no native Meta invite
  without publishing).
- **Hit registration:** start host-authoritative (light lag-comp); taser is a projectile (lag-forgiving).
- **Avatar:** sync head + 2 hands (+ held gun), IK fills the body. **Walls:** pre-segmented panels with a
  synced state (`Intact→SmallHole→LargeHole`) + 180s regen — NOT runtime mesh-fracture.

## Module structure (clean separation — 100% additive)

```
Assets/Ziptide/
  Multiplayer/  [Ziptide.Multiplayer asmdef]
    Runtime/ PvpRules, PvpMatch, PvpCombatant, WeaponCharge   <-- BUILT (pure C#, tested)
             PvPMatchDirector, PlayerCombat(MB), WeaponDamage, RemotePlayerAvatar,
             Locator/WristLocator, Hammer/HammerTool(+HammerAutoReturn),
             Destruction/BreakableWall, Net/(PUN2 adapter), Modes/Deathmatch1v1   <-- TODO
  Scenes/PvP_Arena01.unity                         [TODO]
  Content/Worlds/Packs/PvP_Arena01_WorldPack.asset [TODO]
  Editor/Patching/ScenePatcherPvP.cs               [TODO]
```
Reuses: weapons (Taser/Gravity/`ItemFactory`/holster ~70%), `HitZones`, `TravelCoordinator`,
`WorldPackDefinition`/`WorldRuntime`, `SpawnMarkerRuntime`, the Y+B `DevMenu`, director pattern.
Net-new: networking, player health/damage (built in core), networked avatar, match flow, ammo/recharge
(built), tool auto-return, destructible walls, locator, variant menu, voice.

## Game design spec

- **Match:** first to **10 kills**. Lobby → ready → countdown → match → result → rematch. HUD: health,
  weapon + recharge, score, locator cooldown ring.
- **Health/damage (starting values, tune on-device):** Health **6**, taser hit **2** (3 = kill), gravity
  hit **1** (6 = kill) + knockback. Weapon **charge = 2 shots then recharge**. Respawn far from opponent
  + ~2s spawn protection.
- **Weapons/belt:** taser + gravity on the belt; **one gun in hand**; comfort-first gravity self-hop.
- **Locator (left wrist):** hold 3s to ping opponent if in range; **60s cooldown** w/ on-wrist ring.
  Maps to the planned Left-Wrist Scan Pulse (`docs/09_GEAR_AND_TOOLS.md`).
- **Hammer + walls:** pick hammer from belt; off-belt >2 min → auto-returns. Breaks **interior walls
  only** (never floors/exterior). Small shoot-through + bigger pass-through holes; **regen ~3 min**;
  pre-segmented panels (cheap to sync, identical on both headsets).
- **Map:** compact but **multi-level** (ramps, open areas, multiple routes); no single power sightline;
  walls + locator are anti-camp tools. **Variant menu:** start as a "Game Modes" entry in the Y+B menu.

## Things to keep in mind (surfaced for Terry)

"Invite" reality (friend sideloads the same APK + room code; no native Meta invite without publishing) ·
build distribution to the friend · authority/anti-cheat (host-auth fine among friends) · latency/hit-reg ·
networked avatar (head+2 hands+IK) · **voice chat** (Quest Party Chat to start) · **comfort** (gravity
self-motion = nausea risk → comfort-first) · spawn protection / out-of-bounds / disconnect / rematch ·
**destructible-wall fairness** (holes line up + regen synced on both headsets) · hammer-swing feel ·
72 FPS budget (2 avatars + segmented walls) · Quest 2/3 parity + same build version · expandability hooks
(mode interface + WorldPack so more maps/modes are data, not a rewrite).

## Phased roadmap

- **Phase 0 — Decisions/research:** ✅ done (PUN2, remote room-code, segmented walls, comfort-first, solo-first).
- **Phase 1 — Backbone (no netcode):** ✅ `Ziptide.Multiplayer` pure-C# rules core + 14 tests (Architect).
  TODO: scene + WorldPack + `ScenePatcherPvP` + MonoBehaviour wiring → appears in Y+B menu.
- **Phase 2 — Mechanics solo:** wire taser/gravity/locator/hammer + `BreakableWall` to the
  core, HUD, respawn, comfort gravity hop. **Tune feel on one headset — the first real build.**
- **Phase 3 — Networking (Photon PUN2):** import PUN2; `Net/` adapter; sync
  head+2-hands avatar, fire/damage/score/wall-hole; host-authoritative hits; room-code host/join;
  2-headset test; voice (Quest Party Chat first).
- **Phase 4 — Polish:** comfort toggles, spawn protection, disconnect/rejoin, anti-cheat pass, 2nd arena.

**Lanes:** Architect = match/health/damage data + netcode message model + tests; T-Dog = scene/VR/UI/
hammer-feel/locator-VR; networking integration = shared, separately claimed.

## Verification

- Phase 1–2: load the PvP arena from the Y+B menu on one headset; health/score/locator/hammer-holes work
  solo; CI green.
- Phase 3: two headsets join by room code; hits, score, knockback, and wall-holes match on both;
  best-of-10 ends cleanly.
