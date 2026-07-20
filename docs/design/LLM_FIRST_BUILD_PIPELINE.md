# LLM-FIRST BUILD PIPELINE — change anything, have it work the first time
### The current change-pipeline audited, its three structural faults, and the staged path to a fully LLM-operable factory
**Status: RESEARCHED SPEC — staged, not one-shot. 2026-07-19, Terry-directed. Companion to
`PERCEPTUAL_GATE_PROGRAM.md` (gates) and `FORGE_VI_WORLD_COMPILER.md` (the horizon this feeds).**

Terry's goal, on record: *"we want to be able to build or rebuild or change every aspect of our
game and have it work perfectly the first time — and it's important that that be done by LLM."*

---

## 1 · WHAT WE ACTUALLY HAVE (audited 2026-07-19 — it's more than the runbook implies)

**The headline finding:** CI already runs the ENTIRE author/patcher pipeline headlessly on every
push. `BuildAndroid.PatchScenesAndAudit` (the required per-push gate) executes 20+ authors and
patchers in batch mode — creature variants, building styles, camera, RILL lines, first-hour
contract, gardens, vehicles, economy, world layouts, `WorldSpecCompiler.CompileAll`, arena
layouts/weapons, every scene patcher — then the full audit. The APK jobs run the same pipeline
before building. **The old belief "the cloud can't make .asset files" is false at pipeline
level** — Unity-in-CI makes them on every push; it just throws them away (see fault F2).

The change paths today, by LLM-operability:

| Aspect changed via | Verified by | LLM-operable end-to-end? |
|---|---|---|
| Pure C# (Core/Content/Gameplay) | compile + 1,062 EditMode tests | ✅ fully |
| Data in code-defined recipes (city layouts, arena specs, forge recipes, jobs) | authors run in CI + audits | ⚠️ *writes* yes — but **stale committed assets can mask the change** (F1/F2) |
| Generated scenes | patchers at build time + audits + PlayMode routes | ✅ mostly (patch-before-build is the right pattern) |
| `.asset` regeneration after recipe-code changes | ❌ Terry deletes stale assets + reruns editor menus (runbook §1) | ❌ the bottleneck |
| Visual verdicts | forge-photos artifact (optional review) | ⚠️ PG-5 fixes |
| Device verdicts | Terry's headset | ❌ by design for feel; PG-6 closes the correctness half |

Also already in hand: deterministic seeds in recipes · schema validators (`WorldPackValidator`,
first-hour contract gates) · a machine-readable outcome loop (`CI_VERDICT.md`, audit reports,
observation files — this is how LLM sessions already self-verify) · and **`ForgeStaleness`** — a
real impact classifier (SafeAuto / Review / Breaking / Deprecated buckets, tested) that today
covers only forge items. It is the seed of the whole answer.

## 2 · THE THREE STRUCTURAL FAULTS (why "first time" currently fails)

**F1 — Create-only authors.** The dominant author pattern is
`if (LoadAssetAtPath(path) != null) return;` (verified at `ArenaLayoutLibrary.cs:34`; same
pattern across the `*Library`/`*Author` set). Consequence: **changing recipe code does nothing
to an existing asset.** This is the W007-vista bug, the arena-melee-pads bug, and every runbook
entry that reads "delete X, then run the menu." An LLM's change is silently absorbed — the
worst failure mode, because CI stays green while the content stays old.

**F2 — Two sources of truth.** Generated `.asset`s are committed to the repo, AND CI regenerates
content at build time — but because of F1, the committed (possibly stale) asset always wins.
So the repo state, the CI state, and the code's intent can be three different games. First-time-
right is impossible while "what ships" is not a pure function of "what's in the recipes."

**F3 — Human-gated regeneration.** The runbook bake sittings exist almost entirely because of
F1+F2 (menus are just the manual override for staleness). Terry is not needed to *make* assets —
CI proves that on every push — he's needed to *un-stick* them. Remove the staleness, remove the
sitting.

(F4, honorable mention: some patch steps write to `EditorBuildSettings` etc. as side effects —
harmless today, but the compile pass should eventually declare its outputs. Forge VI territory.)

## 3 · THE TARGET MECHANISM — THE RECIPE-HASH LAW (aligns with Forge VI's hash law)

Every generated asset carries the hash of everything that produced it; every author becomes
**ensure-CURRENT, not ensure-exists**:

1. Each author computes `recipeHash` = hash(recipe data serialized + a hand-bumped
   `RecipeVersion` const in the author code) and stamps it into the generated asset (a string
   field on the definition, or a sidecar `.hash` entry in a generated manifest).
2. `EnsureAllAuthored` → `EnsureAllCurrent`: asset missing → create; hash matches → skip;
   hash differs → **regenerate IN PLACE** (same GUID — load, overwrite fields, save; never
   delete+recreate, which breaks references).
3. Player-facing breaking changes route through the generalized `ForgeStaleness` buckets:
   SafeAuto regenerates silently; Review regenerates but demands a PG-5 sheet stamp; Breaking
   requires an explicit migration note in the commit. The classifier exists — generalize it
   from forge items to all recipe kinds.
4. **Staleness becomes a CI Blocker** (`ASSET_STALE_VS_RECIPE` in a new
   `Editor/Audit/GeneratedContentAuditRules.cs`): any committed generated asset whose hash
   mismatches its recipe fails the push. The silent-absorption failure mode dies here.

With the hash law in place, "change any aspect" becomes: edit recipe (data or recipe code) →
push → CI regenerates exactly the affected assets → audits + gates verify → APK carries the
change. No menus, no deletions, no Terry, no memory of what's stale.

## 4 · THE STAGED PROGRAM (each stage independently shippable; do NOT one-shot this)

- **S1 — Hash law on the create-only set** (~5 commits, GPT-executable now): implement §3 for
  the `*Library`/`*Author` create-only offenders (arena layouts, sky vistas, building styles,
  creature variants, cosmetics, world layouts first — the ones with runbook debt). Add the
  `ASSET_STALE_VS_RECIPE` blocker + a deliberately-stale-asset test. **Deletes most of the
  standing runbook §1 bake list permanently.**
- **S2 — One source of truth** (~3 commits + one workflow): a `bake` workflow (the CI_VERDICT
  writer idiom) that, when a push changes recipes, runs `EnsureAllCurrent` in batch mode and
  bot-commits the regenerated assets `[skip ci]`-safe (or chained through the normal gate once).
  Repo state now always equals recipe state; the editor and every operator see current content.
  (Alternative — stop committing generated assets entirely and go build-time-only — is cleaner
  but breaks editor UX and diffability; recommend the bot-commit path now, revisit at Forge VI.)
- **S3 — The change contract for LLMs** (~3 commits): a pre-flight impact tool an operator runs
  before pushing (batch method + saved report): given a diff, list affected assets by bucket,
  the gates that will judge it, and the evidence to expect (which sheets, which tags). This is
  `ForgeStaleness.Affected` generalized into the front door of every change. Output lands in
  the audit report so the NEXT session can see what the LAST change was expected to touch.
- **S4 — Closed-loop evidence** (already ordered elsewhere; listed for the map): PG-5 referenced
  sheets (see the change) + PG-6 agent bus (walk the change on device) + the ratchet law.
  First-time-right = validated input (schemas) × deterministic compile (S1/S2) × gated output
  (PG-1..5) × visible evidence (S4). Feel remains human, by design.
- **S5 — Horizon (Forge VI, unchanged gate ladder):** recipes become the ONLY handle on the
  game; compile pass owns all outputs + hash manifest; identity guarantee per world. S1–S3 are
  deliberately designed as subsets of that plan so nothing is thrown away.

## 5 · HONESTY — what "perfectly the first time" can and cannot mean

Guaranteeable by this program: *a change expressed in recipes/data/code either ships exactly as
specified or fails loudly before install* — no silent absorption, no stale masking, no
editor-state divergence, no destination/scale/route/reach escapes (PG gates), no unreviewed
visual change (PG-5). Not guaranteeable by any pipeline: that the change is *good* — fun,
comfortable, readable. That's Terry's seat, and the whole point of the machine half is that his
headset minutes are spent ONLY there.

## 6 · ACCEPTANCE
- The runbook's standing "delete X + rerun menu" entries are gone because they're impossible —
  recipe edits propagate on push (S1/S2 proof: change one vista color in code, push, watch the
  asset regenerate + audit pass with zero human steps).
- A deliberately stale asset fails CI with `ASSET_STALE_VS_RECIPE`.
- An operator can answer "what will this change touch?" from the S3 report before pushing.
- A brand-new world added ONLY via recipe data ships through the full ladder (authors → gates →
  sheets → APK) with no editor session — the 80-world path, demonstrated on world one.
