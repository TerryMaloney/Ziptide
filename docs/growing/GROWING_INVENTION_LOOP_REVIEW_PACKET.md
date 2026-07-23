# ZIPTIDE GROWING + INVENTION LOOP — RED TEAM / BLUE TEAM REVIEW PACKET

**Master proposal:** `docs/growing/GROWING_INVENTION_LOOP_MASTER_PLAN.md`

**Status:** Three independent second opinions requested by Terry. Reviews are advisory and must not modify runtime or silently rewrite the master proposal.

**Shared instruction for all reviewers:**

1. Pull current `terry-local-wip`.
2. Read the newest `docs/HANDOFF.md` entries and the complete master proposal.
3. Audit current source/docs relevant to your assigned angle; do not rely only on the proposal's summary.
4. Separate proven current project facts from recommendations.
5. Perform both a blue-team case and a red-team case.
6. Recommend concrete decisions, cuts, ordering, and acceptance evidence.
7. Record disagreements rather than smoothing them over.
8. Do not build runtime, scenes, assets, UI, audio, recipes, or machines during this review.
9. Do not edit the master proposal. Write only your assigned review file and your own handoff entry.
10. Preserve the M0 recovery freeze and the canonical W000→W001, then W002, production order.

---

## 1. Shared questions every reviewer must answer

### Blue team

- What is strongest and most distinctive about this combined growing/invention/loadout loop?
- Which existing ZIPTIDE systems make it unusually feasible?
- How can it increase exploration, replayability, player expression, and ship usefulness?
- What is the smallest implementation that proves the core fantasy?
- Which parts could later generalize into the reusable game-production framework?

### Red team

- Where can this become confusing, grindy, over-scoped, exploitable, technically fragile, or irrelevant?
- Which parts threaten the first-hour/world-factory schedule?
- What existing systems or owners could it duplicate or conflict with?
- What assumptions are unproven?
- What should be cut, delayed, simplified, or forbidden?

### Required recommendation

Each review must end with:

- **KEEP NOW** — items required in the initial W000/W001 or W002 proof.
- **KEEP LATER** — valuable items delayed until the base loop is proven.
- **CUT OR PARK** — items whose cost/risk exceeds their value.
- **OPEN DECISIONS FOR TERRY** — no more than seven high-impact decisions.
- **TOP FIVE FAILURE TESTS** — exact tests/evidence most likely to disprove the design.
- **CONFIDENCE** — confidence in the recommended version and what would change it.

---

## 2. Reasonbox assignment — systemic fun, progression, economy, and pacing

**Output:** `docs/growing/reviews/REASONBOX_GROWING_LOOP_REVIEW.md`

### Primary mission

Determine whether the proposal creates a genuinely fun long-term loop rather than a collection spreadsheet or several disconnected subsystems. Reconcile it with ZIPTIDE's first hour, exploration, story, non-lethal combat, jobs, salvage, ship fantasy, children/family audience, and 80-world pacing.

### Required source audit

Inspect the current authorities and implementations for:

- garden genetics, watering, species and persistence;
- economy/reward routing;
- jobs/contracts and machine repair;
- weapon/ability progression;
- holster/belt/loadout behavior;
- first-hour envelopes and W000/W001 progression;
- world/chapter progression and signature discoveries;
- machine/salvage battle economy;
- existing accessibility and child-use decisions.

### Blue-team focus

- Identify the most compelling emotional and behavioral loop.
- Define what players should anticipate, discover, choose, and celebrate.
- Recommend a comprehensible recipe grammar that children can learn.
- Explain how to make gardening useful without replacing salvage/combat progression.
- Recommend how often major recipes, seed families, machines, abilities, and loadout changes should appear across the first worlds and chapters.
- Identify opportunities for wonder, surprise, collection appeal, and family co-discovery.
- Evaluate sprite-like garden helpers as emotional reinforcement rather than feature bloat.

### Red-team focus

- Grind, recipe lookup, inventory overload, dominant strategies, pacing interruption, forced ship returns, random breeding frustration, and power creep.
- Whether limited loadouts create meaningful expression or repetitive backtracking.
- Whether garden and machine paths feel mutually necessary or artificially coupled.
- Whether the system distracts from story, world discovery, jobs, and combat.
- Whether the first-hour proposal is still too large.
- Whether multiplayer rotations introduce fear of missing out or pressure inappropriate for the family audience.

### Required deliverables

- Recommended core fantasy in one paragraph.
- Recommended campaign progression curve from W000 through mid-game.
- Recommended biological/industrial value split.
- Recommended deterministic-versus-random breeding law.
- Recommended initial seed/recipe/output count.
- Recommended loadout-choice principle, not final slot code.
- Recommended kid-understanding test.
- One example early-game loop and one mid-game loop.
- KEEP NOW / KEEP LATER / CUT OR PARK matrix.

---

## 3. Architect assignment — canonical ownership, schemas, data flow, and automation

**Output:** `docs/growing/reviews/ARCHITECT_GROWING_LOOP_REVIEW.md`

### Primary mission

Determine how the concept can fit the existing project without duplicate services, hard-coded world logic, incompatible persistence, or a new parallel crafting stack. Produce the architecture boundaries and implementation order a lower-cost coding model could later follow.

### Required source audit

Inspect the current implementations and authorities for:

- garden service/genetics/watering/species definitions;
- save/profile overlays and migrations;
- inventory, item IDs, equipment, holster/belt and loadouts;
- economy/reward router;
- machine, belt, automation and persistence systems;
- mining/salvage/material definitions;
- crafting or recipe-like systems already present;
- WorldSpec/world factory/production packets;
- Forge/registry/provenance;
- audio event and accessibility seams;
- multiplayer boundaries;
- current audits/tests/catalog breadth gates.

### Blue-team focus

- Identify canonical surviving owners and reusable interfaces.
- Recommend the smallest data model that supports seeds, traits, recipes, adjacency, outputs, machine processing, equipment assembly, catalog presentation, spoken labels, and loadout capacity.
- Separate generic framework data from ZIPTIDE-specific content.
- Define deterministic save/migration behavior.
- Define how W001 and W002 prove the architecture without premature broad implementation.
- Identify the correct automation ratchets: schema fields, compilers/authors, audits, and device-only evidence.
- Recommend how the catalog/narration metadata can become project-wide rather than garden-specific.

### Red-team focus

- Duplicate inventory/economy/crafting owners.
- Combinatorial recipe explosion.
- Trait inheritance and save-version instability.
- Persistent adjacency/growth/automation state across travel and quit.
- WorldSpec schema pollution.
- Hidden W001 special cases.
- Runtime object counts and update loops.
- Live multiplayer rotation contaminating the offline campaign save.
- Accessibility narration being implemented as hard-coded audio clips per item rather than a scalable text/audio seam.

### Required deliverables

- Current ownership map: proven owner, missing owner, displaced/forbidden duplicate.
- Proposed data-flow diagram in text.
- Minimum schema recommendation with fields grouped by authority.
- Save/migration strategy and failure cases.
- W001 implementation slices and dependencies.
- W002 generalization test.
- Proposed validator/audit list, with which are blocking versus device-only.
- Explicit list of files/systems that must not be touched by the eventual first slice.
- KEEP NOW / KEEP LATER / CUT OR PARK matrix.

---

## 4. T-Dog assignment — Quest device, child usability, accessibility, release, and abuse risks

**Output:** `docs/growing/reviews/TDOG_GROWING_LOOP_REVIEW.md`

### Primary mission

Evaluate whether the proposal can be made understandable, comfortable, performant, certifiable, and safe on Quest for a family audience, especially younger players who recognize objects and spoken labels better than dense text.

### Required source audit

Inspect current authorities and implementations for:

- VR menus, UI reach/readability and child-height requirements;
- comfort presets and controller/input conventions;
- audio mixer/lifecycle/ducking and Reasonbox's audio program;
- subtitle and narration-adjacent systems;
- device performance budgets and runtime health;
- plant, machine, belt and automation object counts;
- Meta VRC/accessibility/store requirements relevant to menu behavior and offline operation;
- save interruption, system overlay, doff/resume and tracking lifecycle;
- release build hygiene, permissions and online/Photon boundaries;
- mixed-age/family audience decisions.

### Blue-team focus

- Design a practical spoken-menu interaction that helps a six-year-old without exhausting adults.
- Recommend icon, silhouette, category, text, speech, haptic and focus behavior.
- Recommend how recipe quantities and unknown ingredients should be represented in VR.
- Recommend confirmation, cancellation, undo and rare-resource protections.
- Recommend kid/device tests for reaching, understanding, selecting, crafting and equipping.
- Recommend object/voice/performance budgets for plants, plots, machines, belts, moving items and helpers.
- Identify how the system could improve accessibility beyond children, including low vision and reading difficulties.

### Red-team focus

- Narration chatter, focus thrashing, overlapping RILL/menu speech/alerts, localization cost, and speech latency.
- Tiny icons, color-only categories, dense crafting grids, accidental spending, unreachable controls, seated/standing mismatch, and controller sleep/resume.
- Too many active plants/helpers/moving conveyor items for Quest.
- Background growth, system overlay, doff/resume, offline start, low storage, save corruption and update-over-install.
- Multiplayer rotations, live-content operations, parental pressure, fear of missing out, and age/audience complications.
- Whether spoken labels should be generated TTS, prerecorded, hybrid, or platform-provided, with commercial/release implications clearly separated from recommendation.

### Required deliverables

- Recommended accessible catalog interaction sequence.
- Spoken label timing/priority/ducking recommendation.
- Visual-symbol and non-color identity rules.
- Child-height, target-size and confirmation rules.
- Device performance budget proposal.
- Interruption/save/device acceptance matrix.
- Campaign-versus-multiplayer rotation release recommendation.
- Five highest-risk Quest failure modes.
- KEEP NOW / KEEP LATER / CUT OR PARK matrix.

---

## 5. Review independence and reconciliation

The reviewers should not read one another's draft before filing their own first opinion. Independence is valuable here.

After all three reviews land, GPT will produce:

`docs/growing/GROWING_INVENTION_LOOP_DECISION_RECONCILIATION.md`

That reconciliation will contain:

- agreements across all three;
- two-of-three agreements;
- direct conflicts;
- current-source corrections to the master proposal;
- recommended minimum W001 slice;
- recommended W002 replication slice;
- proposed cuts and deferrals;
- questions requiring Terry's decision;
- no implementation unless separately authorized after the headset verdict.

---

## 6. Handoff format for each reviewer

Each reviewer appends or queues one entry containing:

- **Did:** source audit completed, blue-team findings, red-team findings, review path.
- **Next:** what GPT/Terry must decide or reconcile.
- **Heads-up:** collisions, unproven facts, dangerous scope, and runtime files not touched.
- **Commit:** exact review commit.

The review file should be complete enough that the final reconciliation does not need the expensive model to repeat the audit.
