# ZIPTIDE — Active Worklist (the running tally)

**The single "what do we do next" list.** Updated every session.
Design lives in `docs/design/`; long-term vision in `ZIPTIDE_MASTER_BUILD_PLAN.md`.

Last updated: 2026-06-15

---

## ⚠️ WORKFLOW STATUS: DEGRADED — fix before code work
**CI cannot compile-check** (Unity license expired → `Code 20110 serial invalid`). Until green,
we are flying blind: unverified C# can reach the headset. See `docs/RECOVERY_STEPS.md`.

## 🖥️ WHEN BACK AT THE PC — do in this order
1. **Fix CI's Unity license.** The activation workflow's `.alf` is waiting in GitHub Actions →
   upload at license.unity3d.com/manual → choose Unity Personal → download `.ulf` → paste into the
   `UNITY_LICENSE` secret → re-run CI to green. (`RECOVERY_STEPS.md` step 1)
2. **Register Unity Smart Merge** (3 `git config` commands you have) — done if already run.
3. **Give Claude eyes:** open `MilestoneA_GrabCube`, `_Boot`, `D0_City` → `Ziptide → Diagnostics →
   Dump Scene + Rig Config` in each → `git add docs/_generated; git commit; git push`.
   (Tell me when pushed — unblocks grab/holster + lower-level-glitch fixes.)
4. **Pull + build + snapshot**, then test the latest fixes (holster-rides-hip, etc.) and report by
   `ZIPTIDE:` tags.

---

## ✅ Fixed this session (pushed to terry-local-wip)
Dead controllers / frozen view (input-action consolidation); `.gitignore` ate the Build folder;
brown screen on return (door pointed at `_Boot` + guard); gun pile-up (only holstered items
travel); gun lost on travel (`ItemFactory` cache); taser darts sticking at muzzle; drones die &
stay down; A=jump, L3=sprint; first-load move helper; gun snap-to-grip; holster docks to hip;
`.gitattributes` (stops phantom pull conflicts); `CLAUDE.md` workflow-integrity rule; scene-dump
exporter; CI enabled on `terry-local-wip`.

## 🔧 Open polish (needs CI green + scene dumps to do precisely)
- [ ] **Grab from too far / near-grab feel** — needs rig interactor config (from the scene dump).
- [ ] **Lower-level fall glitch in D0_City** — fall to catwalks/stray targets; fix spawn-on-solid +
  bounds using the scene dump.
- [ ] **No locomotion on first test-room entry** (works after a round-trip) — verify the
  force-enable-move fix on a clean build.
- [ ] **Choppy look on entry** — first-frame warm-up; low priority.
- [ ] Verify on device: holster-rides-hip, taser fly, drone death, jump/sprint.

## 🛠️ Workflow upgrades (queued)
- [ ] Switch `dev_build_install.ps1` → `BuildAndroid.PatchScenesThenAPK` (patch + audit every build).
- [ ] One-shot log+diag capture script.

---

## 🗺️ Build roadmap

### Now: nail the vertical slice (Level 1 + controls)
See `docs/design/LEVEL1_TOXIC_VENICE.md` + `docs/design/CONTROLS_AND_FLIGHT.md`.
- Fix lower-level glitch; open one building interior; full job chain (arrive→move→shoot→building→
  flight teaser→ziptide); guidance (ObjectiveBoard + waypoint); Xbox-style control mapping + VR
  iron-sight aiming; `Ziptide.Ship` arcade flight scaffold + short tutorial hop.

### Next: big systems (see `docs/design/SYSTEMS_ARCHITECTURE.md`)
1. **Persistence + data model first:** `SaveSystem` + `PlayerProfile` + offline-idle engine.
2. Expand `WorldPackDefinition`; add Resource/Tool/Machine/Plant/Creature/Biome/Recipe/BalanceConfig.
3. Harvest v1 → 4. Mining/conveyor v1 → 5. Garden v1 → 6. Creatures v1 (swarm/crawl/fly/bruiser) →
   7. World generator v1 (generate-to-editable-data) → 8. Mod/creator mode → 9. Balance tuning.
10. (Later epic) Online + Risk-style planet conquest + leaderboards (no pay-to-win).

---

## Project status
- **Playable:** `MilestoneA_GrabCube` (test room), `D0_City` (Toxic Venice).
- **Solid foundations:** data-driven definitions + factories, Job system, TravelCoordinator,
  per-world audio/theme, scene-patcher + WorldAuditRunner, persistent rig + belt.
- **Greenfield (designed, not yet built):** profile/save, idle economy, mining, garden, tools/
  machines, creature AI, world generation, multiplayer.
