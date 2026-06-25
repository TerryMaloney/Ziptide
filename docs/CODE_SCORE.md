# ZIPTIDE — Code Score (1–5)

**A SIG-style codebase health scorecard.** Re-run this when the architecture changes materially. Today's
overall: **3.5 / 5 — solid foundation + outstanding docs; the gaps are PlayMode tests + a few open
IL2CPP/wiring risks, all known and on the roadmap.** Audited 2026-06-21.

> **Rubric (per dimension):** 5 excellent (clean, low-complexity, well-tested) · 4 good (healthy, safe for
> autonomous agent work) · 3 average (works, accruing debt) · 2 poor (significant debt, risky) · 1 critical.

## Scorecard
| # | Dimension | Score | Why |
|---|-----------|:----:|-----|
| 1 | **Architecture & separation** | **4/5** | Asmdef graph is clean + acyclic (`Core` has no Ziptide deps; `Visuals` never depends on `Gameplay`; `Multiplayer` refs only `Core`). Data-driven `Definition` + `DefinitionRegistry<T>`, compile-time `ZiptideFlags`/`ZiptideConstants`, `_Boot`-scoped singletons with audit guards. Gap: `WorldPackDefinition` has no load-time validation. |
| 2 | **Test coverage** | **3/5** | ~105 EditMode tests across economy/harvest/mining/garden/PvP-rules/job-rewards/registry — excellent *pure-logic* coverage by design. But **zero PlayMode tests** (travel/rig/holster/drone untested in CI), and `WorldAuditRunner` itself is untested. |
| 3 | **Build / CI** | **4/5** | CI compiles + EditMode-tests every push; the **real APK** path (`BuildAndroid.PatchScenesThenAPK`) patches scenes + runs the **world audit** (blockers fail the build) + uploads `ziptide-apk`. Gap: PlayMode not wired into CI; patcher idempotence untested. |
| 4 | **Tech debt / risk** | **3/5** | Most E0 red-team risks resolved (`_Boot`, ProximityTravel→TravelCoordinator, flag typos). Still open: ItemFactory not IL2CPP-hardened, `ProfileEconomy.ResolveWorld` **not wired into world-entry**, NarrativeSaveSystem/RILL persistence unverified, AudioDirector dispose-on-unload, magic numbers. |
| 5 | **Docs & coordination** | **5/5** | The 3-doc spine (HANDOFF / MASTER_CHECKLIST / STORY_BIBLE) + MODULE_MAP + the red-team build plan + per-system docs are genuinely best-in-class for LLM-run dev. "Claim before build" prevents collisions; everything points at one branch. |
| 6 | **Code quality / consistency** | **4/5** | Centralized constants, `ZIPTIDE:` diagnostic tags, dup-singleton guards, registry validation. Gap: scattered magic numbers (fall limits, gun scale), some un-hardened reflection (`ItemFactory`, `PlayerRigPersistence` anchor field), per-frame debug logging not `#if DEBUG`-gated. |

## Highest-value fix per dimension
1. **Architecture:** `WorldRuntime.ValidatePackDefinition()` on load → log + fail loud if `packId`/`sceneName`/jobs are null (catches content-authoring mistakes before 80-world scale).
2. **Tests:** stand up `Tests/PlayMode/` + one `TravelCoordinator` round-trip test (unblocks the whole PlayMode suite).
3. **Build/CI:** unit-test `WorldAuditRunner` (blocker logic + patcher idempotence) so the build gate can't silently regress.
4. **Tech debt:** **wire `ProfileEconomy.ResolveWorld` into world-entry** (the one missing economy link) + verify NarrativeSaveSystem actually persists flags.
5. **Docs:** a story→WorldPack serialization checklist (close the design↔buildable gap) — covered by the world-flow + mass-build work.
6. **Code quality:** extract magic numbers to `ZiptideConstants.Physics`/`.Visuals`; harden `ItemFactory` to a Resources-path convention (IL2CPP-safe).

## 🔴 Critical-path blockers (the "fix first" set — Phase A/B of the backlog)
1. **`ProfileEconomy.ResolveWorld` not called on world-entry** — idle/welcome-back never runs; the loop is half-wired. *(highest leverage)*
2. **ItemFactory IL2CPP safety** — reflection-ish lookup may strip on Quest builds.
3. **PlayMode test scaffold missing** — no automated regression net for rig/travel/runtime.
4. **NarrativeSaveSystem / RILL cross-scene persistence** — story gates may be dead code until verified.
5. **WorldStubGenerator missing** — every new world still needs a hand-written patcher; blocks mass-build.

*Detail + lane assignment for all of these lives in `FABLE5_BACKLOG.md`. Source audits: codebase health +
readiness-gap (2026-06-21). Rubric basis: SIG maintainability model + ISO 25010.*
