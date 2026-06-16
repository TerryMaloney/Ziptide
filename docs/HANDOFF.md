# HANDOFF — shared cross-chat log (T-Dog ⇄ Architect)

⛔ **HARD RULE (both Claude chats, no exceptions):**
1. **Read this file at the START of every session.**
2. **Append a `Did / Next-CLAIMED / Heads-up / Commit` entry at the END of every session.**
3. **All shared work + this log live on the `terry-local-wip` branch** (it's green and is what Terry
   builds from). `git pull` it before starting. **Do not do shared work on scratch branches** — they
   diverge and don't reach the headset.

> Detailed history lives in `docs/SESSION_LOG.md`. This file is the quick, always-current handshake.

---

## Working agreement
- **T-Dog** = gameplay / editor / XR rig / scenes / patchers / dev tools / on-device-facing fixes.
- **Architect** = backend C# / data model / economy / registries / tests (pure, CI-verified, no headset).
- **Gemini** = creative only (ships, factions, lore, art). **No repo access** — its docs come in via
  Terry and a coding chat files them (see `docs/storyboard/`).
- **Collision rule:** new files in your own lane = go. Editing a file in the other's lane, or a shared
  file (`ZiptideConstants.cs`, `WorldPackDefinition.cs`, build settings, `STATUS.md`) = **claim it in a
  Next-CLAIMED entry first.**
- **Branch:** one branch — `terry-local-wip`. Small commits. CI must stay green (if it goes red, warn
  Terry loudly and stop shipping unverified C# — see `CLAUDE.md`).

## Project state (current, 2026-06-16)
- **CI is GREEN on `terry-local-wip`.** Unity license fixed permanently (local `.ulf` in the
  `UNITY_LICENSE` secret; manual web activation is dead — `docs/RECOVERY_STEPS.md`).
- **Backbone built + CI-green** (Architect): `PlayerProfile` + `ProfileSerializer` + `SaveSystem`
  (unwired), generic `DefinitionRegistry<T>` + Resource/Tool/Machine/Plant/Creature/Biome/Recipe/
  BalanceConfig definitions, `IdleEngine`, `EconomyState` + `ProfileEconomy`, 24 EditMode tests.
- **Gameplay fixes + tools built + CI-green** (T-Dog): step-offset error fixed; **global fall-safety
  net** (any gravity world); audit-blocker root cause fixed (`StripRigFromWorldScene` self-heals world
  scenes); **Developer Warp system** (jump to any world/marker).
- **Superseded — do NOT continue:** the original "pod-loading seam / IPodLoader / walking skeleton"
  design. It predates the backbone above + `docs/design/SYSTEMS_ARCHITECTURE.md`. Build against the
  current data model, not the old plan.

---

## ENTRIES (newest first)

### 2026-06-16 — T-Dog
- **Did:** (1) **Developer Warp system** — `DevWarp.WarpToScene(scene, markerId)` (dev-gated) +
  `Ziptide → Dev → Warp Window` (auto-lists every WorldPackDefinition; Open Scene / Play-here,
  per-marker) + `PlayerRigPersistence.TeleportToMarker(id)`. (2) **Sandbox Test Lab patcher**
  (`Editor/Patching/ScenePatcherSandbox.cs`, menu `Ziptide → Dev → Build Sandbox Test Lab`): builds a
  30x30 dev scene with floor, spawn, WorldRuntime, 6 named zone markers (grab/range/enemy/travel/
  artwall/loco) + a return door, and a `Sandbox_WorldPack` asset so it appears in Dev Warp. Earlier:
  step-offset fix, global fall-safety net, audit-blocker self-heal. All CI-green on `terry-local-wip`.
- **Next-CLAIMED:** in-VR dev menu panel (summon gesture → world/zone list → `DevWarp`), OR expand the
  sandbox zones (targets/drones/art wall) — gameplay/editor lane. Will claim specifically next session.
- **Heads-up:** Terry — run `Ziptide → Dev → Build Sandbox Test Lab` once in Unity to generate the
  scene, add it to Build Settings to warp into it at runtime, and commit the new `.meta` files for the
  `DevTools/` + sandbox files Unity generates.
- **Commit:** `891680c` (Dev Warp) + sandbox patcher (this push) on `terry-local-wip`.

### 2026-06-16 — Architect  ← (please add your entry here next session)
- **Did:** Set up this shared log; earlier built the backbone (see Project state). Started a
  "pod-loading seam" on branch `claude/architect-project-onboarding-2x7h60` (CI red, **superseded —
  drop it**).
- **Next-CLAIMED:** *(recommended)* **Harvest v1** — `ResourceNode` (per-biome `ResourceDefinition`)
  + a `ToolDefinition` use → adds resources to `PlayerProfile` inventory (the simplest economy loop;
  builds on `ProfileEconomy` + registries; EditMode-testable). Pure backend — no scene/rig/patcher
  collision with T-Dog's sandbox work. Pairs with Dev Warp (warp into a world, harvest there).
- **Heads-up:** please move to `terry-local-wip` (your onboarding branch is red + divergent); drop the
  pod-loading work; commit Harvest v1 here.
- **Commit:** _(add when done)_
