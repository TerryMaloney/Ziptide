# FORGE VI — THE WORLD COMPILER
### Eighty worlds that each feel authored, from a pipeline that doesn't need eighty months

**Status:** 🔵 PLANNED — no code authorized. Gate: `FORGE_V_AND_BEYOND.md` §1 ladder; envelopes
wait for FORGE IV CP-11 (archetype rollout) because the compiler mass-produces exactly those
proven archetype recipes. WC-1's *paper schema* may be drafted during CP-11.
**North star:** a new world goes from "story bible row" to "walkable, conformant, identity-bearing,
budget-clean world" in one operator session — and no player can tell which worlds were compiled.

> The ratchet is what makes mass production safe. A compiled world passes every gate a hand-built
> world passes, automatically, before a human ever looks at it. Quality tiers do not exist;
> identity tiers do.

---

## §0 — Rails

1. All FORGE III/IV budgets, audits, and the conformance ratchet apply to compiled worlds with
   zero exceptions. `ART_UNCONFORMED` on a compiled world blocks exactly like on W001.
2. **Determinism is a gate, not a virtue:** same recipe + same code ⇒ byte-identical world hash.
   A nondeterministic compile step is a CI red by definition.
3. Create-only / idempotent: recompiling an untouched recipe is a no-op. Human-touched (locked)
   elements are NEVER overwritten — the `lockedContentHash` law at world scale.
4. **A model verdict can fail a world; only a headset verdict can promote one.**
5. No parallel toolchain: the compiler is Unity editor code + SO data + EditMode tests + audit
   rules, entry point beside `BuildAndroid`. No Python world generator, no external DSL.
6. Circuit breaker and one-envelope-per-session apply as everywhere.

## §1 — WC-1 · THE WORLD RECIPE

**Why first:** everything downstream derives from it, and drafting it against the story bible will
expose what data FORGE IV should already be recording.

**Paper schema (field list is the deliverable of the early draft; SO comes at implementation):**

```yaml
worldId: W014
displayName: Rust Chandelle
archetype: industrial_city        # CP-11's six, closed enum
storyRefs: { bibleRow: W014, chapter: 3, flags: [ch3_open] }
hazard: corrosive_mist            # feeds SKYSCAPE_DESIGN §4.1 atmosphere defaults
layout: { seed: 91407, size: M, waterBodies: [berth, canal_ring] }
vista: { tier: 2, base: industrial_family, bodies: 2 }   # tiers per SKYSCAPE_DESIGN §4
palette: { family: rust_teal, accent: signal_orange }     # ForgePalettes family law
lightScript: derived               # F3.1 deriver; overrides only inside its clamps
grade: derived                     # F3.2
kits: [salvage_row_set, toxic_tenement_set, dock_set]     # WC-2 module sets
roster: { creatures: [swarm_bug, warden], densityTier: low, lanes: 2 }  # lanes = LS-4
practicals: { density: street_med, heroLights: 2 }        # F3.1b caps unchanged
aweSlots: [arrival, crest]         # CP-5 node slots the compiler places markers for
states: [default, scarred]         # LS-1 list; deltas authored later
acts: [calm, mist_heavy]           # LS-5
audio: { identity: industrial_minor, refs: ADAPTIVE_AUDIO }  # CP-8
heroElement: UNASSIGNED            # WC-4 — a world cannot ship with this unassigned
identity: { skylineSeed: 3, motif: hanging_gantries }     # WC-4 inputs
budgetTier: standard               # standard | signature (signature = W00x hand-built class)
lifecycle: { state: compiled_draft, lockedElements: [] }
```

`Validate()` clamps every field (closed enums, tier ranges, cap cross-checks — e.g. heroLights ≤2,
lanes ≤4) so a wild recipe fails CI before any compile runs.
**Acceptance:** schema tests; a `Validate()` sweep over all authored recipes; story-bible
cross-check (every recipe's bibleRow exists; every plotted world has a recipe or an explicit
`deferred` row). **Budget:** 3 commits (schema+validate / library seed from the bible's first ~12
rows / audit+docs). **Do not:** put derived values IN the recipe (derivation stays in code);
free-text fields where an enum can close the space.

## §2 — WC-2 · KIT COMPLETION

**Why:** the compiler can only compose what the kits contain. Today's building vocabulary
(walls ×4) cannot compose full worlds.

**What, per archetype (bounded by its pilot world's needs — "done" = the pilot composes with
zero one-off geometry):**
- **Architecture:** doorway, corner trim, roof/parapet, ground-floor front, bridge/gantry segment,
  dock edge — the E5.1 debt list, per style family, both-sides law respected.
- **Streetscape:** surface kit (street/canal-edge/interior floor), barrier/rail, debris clusters,
  archetype signage frames (F3.7 consumers).
- **Nature:** 2 flora + 1 ground-cover per biome family beyond the current pair.
- **Set pieces:** 2–3 mid-size landmarks per archetype (crane, silo, gate arch) — P2-band,
  recipe-built, reusable with palette/scale variation.
All through existing machinery: `ForgeRecipeLibrary` + `ForgeBuildingKit` + booth turnarounds,
class budgets unchanged. **Acceptance:** per-kit booth sheets; a *composition test* — a synthetic
layout using every kit piece compiles with zero fallback primitives visible.
**Budget:** ~3 commits per archetype; roll out one archetype at a time in CP-11 order.
**Do not:** grow a kit past its pilot's needs ("kit sprawl" is the failure mode); accept a piece
whose turnaround verdict was never recorded.

## §3 — WC-3 · THE COMPILE PASS

**Why:** the author chain exists but is invoked piecemeal. Scale needs one entry point, one order,
one manifest, one hash.

**What:** `WorldCompiler.Compile(recipe)` — editor, deterministic, create-only — running the
EXISTING authors in locked order:

1. layout (seeded) → 2. buildings (kits) → 3. dressing (props/flora/tufts) → 4. practicals →
5. water bodies → 6. grounding → 7. vista + light script + grade assignment → 8. signage →
9. ambient-lane markers (LS-4) + awe-slot markers (CP-5) + state/act data stubs (LS-1/5) →
10. audio identity assignment → 11. audits (full `WorldAuditRunner` + PerfBudget + conformance) →
12. contact-sheet render queue → 13. **manifest**: `Builds/Reports/WORLD_MANIFEST_<id>.json` —
recipe hash, per-step content hashes, audit counts, sheet list, compiler+code version.

**The hash law:** step hashes make "what changed" mechanical — a code change that shifts hashes on
untouched recipes is *detected*, listed by the dependency auditor's buckets (safe-auto / review /
breaking), and batched intentionally. Locked elements (hero element, hand-tuned overrides) are
skipped by hash identity, exactly like `lockedContentHash` assets today.
**Acceptance:** double-compile no-op test; determinism test (two compiles, identical manifests);
lock-respect test; step-order regression test. Log `ZIPTIDE: COMPILE world=… step=… hash=…`.
**Budget:** 4 commits (entry+order+manifest / hash law+locks / determinism harness / docs+audit).
**Do not:** let any step write outside its author's namespace; add a step that can't hash its
output; compile at runtime (this is editor/CI machinery — device gets baked results).

## §4 — WC-4 · THE IDENTITY GUARANTEE

**Why:** the failure mode of every compiled-content game is *sameyness*. This envelope is the
soul of the generation: distinctness is measured, and a face is mandatory.

**What:**
- **Identity vector per world**, computed from recipe + compile outputs: palette center (Lab),
  skyline silhouette signature (normalized height-profile FFT bins from the layout), vista
  composition class, roster set, practical rhythm (pole spacing / density class), audio tonal
  center, motif tag. Pure functions over data — testable in EditMode, no rendering needed.
- **Distinctness gate:** within an archetype, every world pair must differ in ≥3 of the 7
  dimensions by more than that dimension's threshold (thresholds start generous, calibrated on
  the hand-built pilots — the pilots DEFINE "different enough"). `IDENTITY_TOO_CLOSE` = blocker
  listing the offending pair + dimensions, so the fix is mechanical (change palette family, reroll
  skyline seed, swap motif).
- **The hero-element law:** `heroElement: UNASSIGNED` blocks shipping. Each world gets ONE
  hand-authored (or Terry-verdicted session-authored) landmark — a named recipe asset with a
  booth sheet and a verdict on record — placed by the compiler at the layout's hero slot. The
  compiler builds the body; the hero element is the face.
- **Sky tiers:** signature worlds keep Tier-1 hand-authored vistas; compiled worlds get tier-2/3
  derivations (SKYSCAPE_DESIGN §4) that must still pass the Prospect rubric — the rubric, not the
  tier, is the bar.
**Acceptance:** vector determinism; pairwise gate over the whole library; pilot calibration
recorded in-doc; hero-element block test. **Budget:** 4 commits.
**Do not:** tune thresholds down to make a red pair pass (change the WORLD, not the ruler —
the never-weaken-assertions law); let two worlds share a hero element; treat metrics as a
substitute for the WC-7 human-eye check.

## §5 — WC-5 · EXTERNAL BACKEND ON-RAMP

**Why:** `design/ART_REGISTRY.md` §5 already names the triggers (hero assets from outside,
marketplace/Tripo at scale, glTFast import). When they fire, imports join HERE — behind the same
gates — or they poison the provenance system.

**What:** an `ImportedAssetIntake` path: external mesh → budget validation (class caps) → material
conformance (URP/Lit, atlas rules, mask channels) → provenance record (source, license, hash) →
booth turnaround → verdict file → THEN eligible for recipes/kits. IDs never change when backends
do (registry law). A `provenance: external` flag rides the asset forever; conformance audit knows.
**Acceptance:** intake refuses over-budget/wrong-material imports (tests with fixture meshes);
provenance completeness audit. **Budget:** 3 commits, **only after a §5 trigger actually fires** —
building this speculatively is rejected scope. **Do not:** allow any import to skip the booth;
accept licenses that can't ship; let external style drift the families (palette law still binds).

## §6 — WC-6 · MEMORY & STREAMING

**Why:** ART_REGISTRY §5's second trigger — RAM pressure or >20 worlds — is a certainty at this
generation's scale. It joins the compiler, not the runtime lanes, because budgets are per-world
compile outputs.

**What:** Addressables (or the then-current equivalent) for per-world content groups, baked by the
compile pass; texture-streaming settings per fidelity band (CP-7 director is the runtime
consumer); **new audited numbers per world**: peak RAM, load time to interactive, group size —
recorded in the manifest, warn at target / block at cap, exactly like tris today. Caps set from
device measurements on the pilots, not guessed.
**Acceptance:** budget presence in every manifest; regression alarms on group-size jumps >15%
without a recipe change. **Budget:** 4 commits + a device measurement session.
**Do not:** stream `_Boot`-owned runtime; let streaming reintroduce load hitches into travel
(TravelCoordinator's contract owns timing — coordinate, don't compete).

## §7 — WC-7 · THE REVIEW FARM

**Why:** eighty worlds × CP-1 sheet sets is beyond any human's sustained attention. Models filter;
Terry judges.

**What:** the Forge II photo loop at fleet scale:
- CI renders every compiled world's CP-1 sheet set (arrival/route/interaction/vista/night/fx/perf)
  plus the LS-1 state pairs when present.
- A reviewer session (any capable model) walks a **batch** against the written rubrics
  (`FORGE_STUDIO_GUIDE.md` + Prospect rubric + CP-1 criteria) and writes a machine-readable
  verdict per world:

```yaml
worldId: W014
run: <ci-run-id>
sheets: 9
verdict: fail                  # pass_to_queue | fail
failures: [ { view: vista, rubric: prospect_far_anchor, note: "skyline reads as flat band" } ]
rubricVersion: cp1_v2
reviewer: model_session_ref
```

- `pass_to_queue` worlds enter **Terry's headset queue** (the DC-3 chamber consumes this queue in
  Forge VII; until then it's a runbook list). `fail` worlds route back with mechanical notes.
- **Calibration law:** the reviewer's rubric is periodically audited — Terry re-judges a small
  random sample of model-passed worlds; if his verdict diverges, the rubric (not the sample)
  gets fixed and the batch re-reviews. Divergence is tracked in-doc.
**Acceptance:** verdict schema tests; queue integrity (no unreviewed world reaches the queue);
sample-audit protocol documented and exercised on the pilot batch.
**Budget:** 3 commits + the pilot review cycle.
**Do not:** let a model verdict promote/lock anything; review without the rubric version pinned;
lower a rubric to clear a backlog.

## §8 — Pilot and sequencing

Order: **WC-1 → WC-2 (first archetype) → WC-3 → WC-4 → WC-7 → pilot cycle → WC-2 (next
archetypes) → WC-6 → WC-5 (only on trigger)**.
**The pilot:** take the FIRST CP-11-proven archetype and compile **2–3 sibling worlds** from new
recipes. Run the full chain: compile → audits → farm review → Terry headset pass. Then the
generation's acceptance question: *can Terry tell which sibling was hand-built?* (One of the
"siblings" in the lineup is the original pilot world.)

## §9 — Definition of done

FORGE VI is done when: compiled siblings pass blind comparison against their hand-built pilot on
quality (while failing it on identity — they must feel like different places); the full
bible-plotted world list has recipes or explicit deferrals; the compile pass is deterministic and
no-op-stable in CI; identity gates hold over the whole library; the review farm has completed at
least two calibrated batches; and streaming budgets are measured, audited numbers on every
manifest. Only then does the 80-world map open for production.
