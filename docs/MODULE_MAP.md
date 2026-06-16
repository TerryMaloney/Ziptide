# MODULE MAP

**What each folder/module is responsible for. Use this to orient quickly.**

---

## Assembly Definitions (Unity)

| Module | Responsibility |
|--------|----------------|
| **Ziptide.Core** | Contracts, schemas, events, utilities. No story/art specifics. |
| **Ziptide.Gameplay** | Build/traverse logic, pod loading, traversal rules. |
| **Ziptide.Content** | Generated assets, pod packs, narrative data (JSON + ScriptableObjects from schema). |
| **Ziptide.Visuals** | Bloom, origami VFX, theme binding. Depends on Core only. |
| **Ziptide.Ship** | Ship scene logic, including DriftCorridor. |
| **Ziptide.Platform.Quest** | XR / Meta-specific glue. |

---

## Doc locations

| File | Purpose |
|------|---------|
| **docs/STATUS.md** | LLM dashboard — milestone, working, blocking, next 3 tasks. |
| **docs/00_LOCKED_CONTRACTS.md** | The laws; interfaces and rules that must not break. |
| **docs/01_ARCHITECTURE.md** | Data-driven packs, skins, asmdef layout, registry pattern. |
| **docs/02_SETUP.md** | How to open project, Unity version, URP, Meta XR setup. |
| **docs/03_MILESTONES.md** | Phase A/B/C and goals. |
| **docs/04_TASK_QUEUE.md** | Current and upcoming tasks. |
| **docs/05_DECISIONS.md** | Why Unity, URP, Meta XR, etc. |
| **docs/06_SCHEMAS.md** | WorldProfile, PodNarrative, VisualTheme, registry schemas. |
| **docs/07_PERF_BUDGET.md** | Quest performance targets. |
| **docs/08_TESTS.md** | Contract tests, smoke tests, per-module acceptance. |
| **docs/architect/** | Architect's shared "mind": whole-project mental model, discrepancy ledger, and the cross-LLM coordination log. Start at `docs/architect/README.md`. |

---

*Keep this map updated when adding modules or docs.*
