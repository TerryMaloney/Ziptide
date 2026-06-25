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
| **docs/FABLE5_START_HERE.md** | 🆕 Fresh-agent primer (token-cheap onboarding: state, lane, roadmap, file map). Read first. |
| **docs/CODE_SCORE.md** | 1–5 codebase health scorecard + critical-path blockers. |
| **docs/ROLES.md** | The two lanes (Architect=data, T-Dog=scene/runtime) + claim protocol + guardrails. |
| **docs/FABLE5_BACKLOG.md** | Lane-tagged pullable task queue (5 phases: Fix→Harden→Worlds→Modes→Creatures). |
| **docs/design/WORLD_FLOW_TEMPLATES.md** | Per-environment "nice flow" recipes for fast, varied, story-connected world graybox. |
| **docs/systems/CREATURE_DESIGN.md** | Creature framework + 12 novel evolution-tied behaviors. |
| **docs/MASTER_CHECKLIST.md** | The scannable "state of the build": BUILT / short / mid / long-term. Skim first. |
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
| **docs/09_GEAR_AND_TOOLS.md** | Idea bank for non-bullet explorer gear (combat/puzzle/utility/companion/traversal). Design-only; ties into existing Taser/`ToolDefinition`. |
| **docs/10_TIDEFRONT.md** | Future galaxy strategy layer (planet control, weighted-probability battles, VR missions, multiplayer). Metadata-first, design-only. |
| **docs/GPT_ADDITIONS/** | Raw GPT planning brainstorms (dated folders). Distilled into the modules above — read the modules first, GPT_ADDITIONS for full detail. |
| **docs/storyboard/STORY_BIBLE.md** | The narrative spine — locked meta + per-mechanic fiction + 4 endings + tone. Start here for story. |
| **docs/storyboard/** | 80-world story system: `STORY_BIBLE.md`, `_WORLD_TEMPLATE.md`, `CHAPTER_*.md` seed catalog, per-world `W<NNN>_*/README.md`. Canonical world table: `ZIPTIDE_MASTER_BUILD_PLAN.md` §12. |

---

*Keep this map updated when adding modules or docs.*
