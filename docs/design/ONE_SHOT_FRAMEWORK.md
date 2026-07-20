# THE ONE-SHOT FRAMEWORK — building the rest with zero defects, and cloning the bones for any game
### Fable 5's take — written thesis-first so multiple models' versions can be merged

**Status:** 🔵 RESEARCH — zero code (freeze). Terry commissioned 2026-07-18: structure + workflow
analysis → how the rest builds "in one shot without bugs" (acknowledged asymptotic) → and the
deeper goal: **a cloneable framework where any future game gets these bones — modular, anything
changeable artistically, nothing structurally breakable.** Multiple models are doing this
exercise independently; §8 is the merge protocol.

---

## T1 — The core equation (the whole take in five clauses)

A project approaches one-shot buildability exactly to the degree that:
1. **Everything is generated from validated data** (genomes → authors → world; no hand-made
   artifacts on the critical path);
2. **Every failure class ever observed is a permanent machine-checked tripwire** (the ratchet:
   bug → law → gate, never bug → fix → hope);
3. **Human judgment is concentrated, not scattered** — few explicit verdict points (headset
   checkpoints, locks, shortlists) instead of continuous babysitting;
4. **The machinery proves itself by regeneration** — deterministic, idempotent, hash-stable:
   rebuilding what already works byte-identically is the recurring self-test;
5. **Operators are replaceable** — the SYSTEM carries the standards (laws, gates, rubrics,
   boards), so any model of any size can drive without lowering the floor.

Ziptide already discovered all five clauses the hard way. The framework is these clauses made
load-bearing on purpose, from day one, for the next game.

## T2 — The seven layers (the anatomy of the bones)

| Layer | Name | Ziptide's version | Cloneable? |
|---|---|---|---|
| **L0** | Substrate | Unity 2022.3.62f3 + PINNED package matrix (the InputSystem 1.7.0→1.6.3 lesson) + clean-room proof lane | ✅ directly (versions change; the pinning + clean-lane PATTERN doesn't) |
| **L1** | Constitution | ownership contracts (`_Boot` owns runtime; travel/input/audio single owners), the locked laws, assembly dependency order (Core←Content←Gameplay←Visuals) | ✅ as a TEMPLATE — every game writes its own nouns into the same contract shapes |
| **L2** | Rails | create-only idempotent authors/patchers · `Validate()` clamps · closed vocabularies · derivation laws (fog FROM sky) · budgets · audit rules · registries/factories by string id | ✅ mostly direct code (audit framework, author base pattern, booth, pure-core-first) |
| **L3** | Genomes | ALL content as validated data: world recipe + content genome, creature genomes, forge recipes, music prompts + kinship dial, line kits, lore registry, physics dials, states/acts | ⚠ the SCHEMAS clone; the content is each game's own |
| **L4** | Proof | the gate ladder: compile → EditMode → audits → PlayMode route + visual capture → clean-room → golden artifact + hash chain → device checkpoint; review farm + machine-readable verdicts | ✅ directly (CI yml + booth + verdict schema are game-agnostic) |
| **L5** | Operators | model-agnostic manual, doc blackboard, lanes/claims, HANDOFF, circuit breaker, honest calibration per model tier | ✅ directly — this is pure pattern |
| **L6** | The Judge | Terry: few, explicit, recorded verdicts (authorization tables, locks, shortlists, kid-tests) | ✅ the ROLE clones; the person is the game's owner |

**The modularity answer lives in the L2/L3 split:** art can change ANYTHING because art is L3
data flowing through L2 rails — and nothing can break because the rails validate, the audits
gate, and the contracts (L1) never move. "Bones so good nothing breaks" = L0–L2 frozen-by-gates;
"change anything artistically" = L3 is the only thing anyone normally touches.

## T3 — The failure taxonomy (every bug class this project has actually seen, with its cure)

The raw material for "nothing breaks" is the honest list of how it DID break. Nine classes from
the recovery/stabilization history — the next game inherits all nine tripwires on day one:

| # | Class | Ziptide evidence | Structural cure (already built ✅ / planned 🔵) |
|---|---|---|---|
| 1 | **Blind push** | broken commits reached the headset; "flying blind" era | ✅ CI-on-every-push + proof ladder + "CI red = blocker" law |
| 2 | **Hand-edit drift** | corrupted scene YAML; committed scene objects diverging from patchers | ✅ authors-only law; scenes as OUTPUT; `.gitattributes` YAML guards |
| 3 | **Ownership violation** | dup singletons, per-scene rig copies, door → `_Boot` reboot | ✅ contracts + contract scan + `DUP_SINGLETON` guards |
| 4 | **Substrate drift** | InputSystem 1.7.0 NRE race — a PACKAGE defect eating weeks | ✅ pinned matrix + clean-package proof lane (Library deleted per job) |
| 5 | **Dangling reference** | `ITEM_DEF_NOT_FOUND`; `PLAYER_TRUSTED_RILL` orphaned in ending math | ✅ registries/caches · 🔵 flag-graph validator (assembly GAP D) |
| 6 | **Provenance gap** | silent `git pull` failures = testing stale builds for hours | ✅ snapshot discipline + tested-SHA verification + hash-chained golden artifacts |
| 7 | **Gate blindness** | CI path filters missed `PlayerRigPersistence`; capture-during-flash; frame-count waits at 3000 fps | ✅ fixed case-by-case · 🔵 the generalized cure: a GATE-COVERAGE AUDIT — lanes must provably watch every input they claim to gate (a gate you can't prove watches X does not gate X) |
| 8 | **Operator collision** | two lanes minting "rb42" the same day; stash-conflict markers pushed | ✅ lanes/claims/boards · 🔵 harden: per-lane id prefixes; conflict-marker pre-push check |
| 9 | **Irreducible judgment** | comfort, feel, fun, awe — only the headset knows | ✅ concentrated verdict points + 🔵 playtest protocol/telemetry; NEVER pretend a gate can replace this class — the framework's honesty depends on naming it |

**The ratchet law, generalized (the framework's immune system):** a failure class, once seen
ANYWHERE, becomes a permanent gate EVERYWHERE. One-shot is the asymptote of this ratchet — and
the reason the SECOND game can be clean start-to-finish is that game one paid the tuition and
the framework keeps the education.

## T4 — The build protocol (how "in sections, then dial in" becomes "one shot" honestly)

Terry's instinct (implement in sections, check everything, dial in) is correct and is the
framework, not a compromise to it:

- **A section = one genome batch walked down the assembly line** (`WORLD_ASSEMBLY_READINESS.md`
  GAP F): content genome → compile → line kit → music → states → flag sweep → review farm →
  device verdict → LOCK. Each section is built one-shot-STYLE: every gate green before the
  headset ever sees it.
- **The convergence metric (measure it, don't feel it): NEW failure classes discovered per
  section.** Section 1 will find some (they become gates). Section 3 should find fewer. When a
  section ships finding ZERO new classes, the machine is proven — THAT is the moment "the rest
  in one shot" stops being hypothetical: batch the remaining sections through the same line.
- **Regeneration is the self-test:** periodically recompile already-locked worlds — byte-drift
  = a machine bug caught free (the hash law doing maintenance work forever).
- **Dial-in stays cheap by construction:** because look/feel is L3 data behind L2 clamps,
  "dial it in" means editing a genome and re-running gates — never surgery on bones.

## T5 — THE SLIPWAY (the cloneable kit — what actually transfers to the next game)

Name for the extracted framework: **the Slipway** (the structure you build every ship on).
A new project = clone the Slipway + write two documents + fill genomes:

**Transfers as-is (repo template):** CI workflow set (compile/tests/audits/PlayMode/clean-room/
golden+hash/review-farm skeleton) · audit-runner framework + ratchet machinery (warn→lock) ·
author/patcher base pattern + idempotence/hash test harness · photo booth + contact-sheet +
machine-verdict schema · pure-core-first structure + LF/style rails · the doc blackboard
skeleton (START_HERE, LAWS, HANDOFF format, boards, EXCELLENCE_MAP shell, claims/lanes) · the
proof-artifact/authorization-table pattern · budget-audit framework · log-tag discipline ·
playtest protocol + local telemetry pattern · store-readiness checklist shell.
**Transfers as template-to-fill:** the CONSTITUTION (each game names its own single owners:
what is its `_Boot`, its travel, its input) · the genome SCHEMAS (world/content/creature/music/
line-kit shapes — new game, new fields, same `Validate()` discipline) · the gate ladder tuning
(a flat-screen game drops comfort gates, keeps everything else).
**Does NOT transfer (each game's own):** the content itself, the art direction, the judge's
taste, and the RESULTS of class-9 judgment — feel is re-earned per game.
**Extraction trigger and cost honesty:** do NOT build the Slipway as a side project now.
Extract it AFTER Ziptide's first fully-compiled section proves the line (T4) — extraction is
then mostly `git mv` + de-Ziptide-ing names, done in weeks not months, and it extracts a
PROVEN machine instead of a hopeful one.

## T6 — What "nothing breaks when art changes" requires, precisely

The sentence has an exact technical meaning — four properties, all already policy, restated as
the framework's warranty:
1. **Interface stability:** art swaps behind stable ids/contract roots (`ForgeVisualApplier`'s
   contract-root-stable/visual-child-replaceable pattern — generalize it to EVERY swappable).
2. **Clamped expressiveness:** every artistic knob has a validated range (grades, light
   scripts, dials, budgets) — freedom INSIDE the clamp is total, outside is impossible.
3. **Provenance completeness:** every visible thing traces to a known system (the conformance
   gate) — so "what broke the look" is always answerable mechanically.
4. **Regression visibility:** every aesthetic dimension has a captured artifact compared over
   time (contact sheets, boot beauty shot, audio alley) — art changes are DIFFS, reviewable,
   revertible, never mysteries.

## T7 — Honest limits (what one-shot can never absorb)

Class-9 truths (fun, comfort, awe, kid-attention) are discovered, not derived — the framework
MINIMIZES headset surprises; it cannot eliminate discovery. Package/platform ecosystems drift
under us (class 4 recurs each engine upgrade — the clean-room lane is permanent, not
transitional). And genuinely NEW failure classes will appear (that's what "new" means); the
framework's promise is not omniscience — it is that **nothing ever breaks the same way twice.**

## §8 — Merge protocol for the multi-model takes (recommendation to Terry)

Ask every model for the same four artifacts so the takes are comparable: ① its failure
taxonomy (mergeable as a UNION — every class any model names gets a cure or an explicit
rejection); ② its layer model (compare boundaries — disagreements here are the interesting
ones); ③ its top-10 laws (INTERSECT first — laws all models independently derive are
load-bearing; debate the rest); ④ its extraction boundary (what clones vs. what's per-game).
**Require every take to map its vocabulary onto the repo's existing nouns** (the
`ASSET_FORGE_MAP.md` rosetta-stone pattern — proven when reconciling GPT's external specs);
a take that renames our machinery instead of mapping to it creates a second glossary, and the
standing rejection of second glossaries applies to meta-plans too. Terry arbitrates conflicts;
the merged result supersedes this doc and lives beside the assembly-line runbook.
