# FORGE VII — THE DIRECTOR'S CHAIR
### Terry's north star, made literal: art direction from inside the headset

**Status:** 🔵 PLANNED — no code authorized. Gate: `FORGE_V_AND_BEYOND.md` §1 ladder; opens LAST —
live direction is only safe when every edit routes through systems that already validate and
refuse (Forge I–VI), and only useful when there is a staged, compiled game to direct (V–VI).
**North star (CLAUDE.md, verbatim):** *"Terry puts on the headset, says 'move that building /
make this do that,' and it just happens with ~zero errors."*

> Every generation before this one runs through a PC session and a rebuild. Forge VII closes the
> final loop: the director stands inside the world and directs it. The one law that makes it safe:
> **the chair has exactly the powers the editor authors have — zero new mutation paths.**

---

## §0 — Rails

1. **The author-power law (absolute):** every command compiles to recipe/author-data edits and
   author re-runs. If an author can't do it, the chair can't either — that's a new author
   envelope, not a chair feature. No live scene mutation outside author namespaces, ever.
2. All edits land as normal commits through normal gates (CI, audits, conformance, budgets).
   The chair produces *proposals* that become commits — it never bypasses the pipeline.
3. `DevAccessGate` idiom governs every chair surface. Shipping player builds contain the player
   features (photo mode, cosmetics) and NOTHING of the director tooling unless gated.
4. **The player's head is sovereign — even Terry's.** No chair feature moves or rotates the
   viewpoint. Spectator/trailer cameras are separate render targets, never the HMD.
5. Comfort/performance floors unchanged: 72 Hz including the chair's own UI cost; director
   panels are diegetic boards (the DevWarpBoard idiom), not screen-space overlays.
6. Circuit breaker, one envelope per session, no scene YAML — as everywhere.

## §1 — DC-1 · THE COMMAND SEAM

**Why first:** it is the load-bearing safety architecture; every other envelope rides on it.

**What — three layers, strictly separated:**
- **Intent capture (in-headset):** point/grab-select a target (existing XRI ray) + a spoken or
  board-picked verb. Speech-to-text runs on the companion PC session, NOT on device.
- **Command compilation (companion session):** a model session (the same kind that builds today)
  translates intent → a **closed command grammar**:

```yaml
command: move
target: { provenance: building_module, holder: W014_salvage_row_03 }   # resolved via conformance provenance
delta: { position: [+4.0, 0, 0] }        # snapped to the kit grid
compiled: { edit: layout_data, field: modulePlacements[27].cell, rerun: [BuildingBuilder, PracticalAuthor, GroundingAuthor] }
guards: { budgets: pass, conformance: pass, identity: unchanged }
```

  Verbs (closed, extend by decision): `move` · `remove` · `duplicate` · `swap_style` · `recolor`
  (palette-family space only) · `relight` (F3.1 clamp space) · `regrade` (F3.2 clamp space) ·
  `replant` (dressing tags) · `restage` (LS-2 event params) · `retide` (LS-5 act params) ·
  `mark_hero` (WC-4 hero-slot assignment). Targets resolve through the F3.9 conformance
  provenance chain — a thing the audit can't attribute is a thing the chair refuses to touch.
- **Application:** the compiled edit applies to the recipe/author data in a working branch,
  affected authors re-run (create-only), the world hot-reloads via the normal content path, and
  the change appears in-headset. **Nothing is committed yet** — see DC-1b.
**DC-1b — the double-verdict commit flow:** in-headset A/B (hold-to-compare before/after) →
Terry accepts → companion session runs audits + tests → CI green on the branch → auto-merge to
the working branch with a `chair:` commit message recording the command verbatim. Reject at any
step = clean revert (the data edit is one diff; revert is mechanical).
**Acceptance:** grammar closed-enum + `Validate()` tests; the author-power law as a contract test
(chair's write surface ⊆ authors' write surface, asserted by reflection over the seam's API — an
EditMode test, not runtime reflection); refusal tests (unattributable target, out-of-clamp delta,
budget-exceeding duplicate). Log `ZIPTIDE: CHAIR cmd=… target=… result=applied|refused reason=…`.
**Budget:** 6 commits (grammar+compile / provenance targeting / apply+rerun / A-B + revert /
commit flow / audit+docs). **Do not:** free-form deltas (everything snaps to grids/clamps/enums);
let the chair touch `_Boot`, travel, saves, or input; queue >1 uncommitted command (single-flight).

## §2 — DC-2 · LIVE LOOK TUNING

**Why:** the highest-frequency direction need ("warmer", "foggier", "dimmer street") should not
cost a command round-trip each step.

**What:** a diegetic **tuning board** (DevWarpBoard idiom) exposing ONLY already-clamped
parameters: grade (F3.2 ranges), light script overrides (F3.1 ±30%/20–55° bounds), fog/vista
weights, act blend (LS-5), practical intensity family, audio stem mix (CP-8). Sliders move
runtime values live; **the clamps are hard-coded into the board** — it physically cannot produce
an out-of-range world. A `WRITE BACK` tile compiles the current deltas into ONE DC-1 `regrade`/
`relight`/`retide` command (same double-verdict flow); leaving the world without write-back
reverts to data truth.
**Acceptance:** board-range == clamp-range contract tests (a clamp change in F3.1/F3.2 must break
a chair test until the board matches); revert-on-exit test; write-back equivalence test (board
state → command → recompiled world reproduces the look, hash-checked).
**Budget:** 3 commits. **Do not:** expose any un-clamped parameter (the moment one exists, the
board is a new mutation path); persist runtime tweaks outside write-back.

## §3 — DC-3 · THE VERDICT CHAMBER

**Why:** WC-7 ends in "Terry's headset queue" — this is the queue's consumer. Review stops being
"Terry reads PNGs on a monitor" and becomes the product reviewing itself.

**What:** an in-headset review mode (gated, `_Boot`-launched like a world visit, travel contract
respected):
- **The rail:** per queued world, a teleport rail through its contact-sheet moments (arrival,
  route, vista, night, awe slots) — teleport-to-marker, never a moving camera.
- **Verdicts:** at each stop, board tiles — ✅ pass · ❌ fail (+ category tiles matching the
  rubric: silhouette / materials / depth / grounding / identity / comfort) · 🎙 a voice note
  transcribed by the companion session into the WC-7 verdict file's `note` field.
- **Ceremonies:** promotion acts are explicit in-headset acts on a dedicated tile with a
  hold-to-confirm: `LOCK WORLD` (F3.9 `ConformanceLockedWorlds` entry), `PASS AWE NODE` (CP-5),
  `SHIP STATE` (LS-1). Each ceremony compiles to the corresponding data/whitelist edit through
  DC-1's commit flow — the human verdict the whole ratchet system has always required, now
  captured at the moment of judgment.
**Acceptance:** verdict files written by chamber == WC-7 schema (shared tests); ceremony edits
route through DC-1 (no direct writes); queue integrity (chamber can only consume
`pass_to_queue` worlds). **Budget:** 4 commits.
**Do not:** auto-advance the rail on a timer (Terry moves when Terry moves); allow bulk-pass
(one world, one verdict, one act); let a ceremony fire without hold-to-confirm.

## §4 — DC-4 · CAPTURE & SHOWCASE

**Why:** the game will need store assets, trailers, and social captures — and players deserve the
photo mode the completion roadmap already promises. Both come from the same seam.

**What:**
- **Photo mode (player-facing, ships):** pause-the-moment capture — a detached photo camera on a
  1.5 m tether from the player position (comfort: the HMD view stays put; the photo camera is
  manipulated like a held object), FOV/roll/grade-preset controls (clamped, F3.2 families),
  hide-UI, capture to the quarters gallery (existing `QuartersCameraFeature` seam extends).
- **Spectator/trailer path (dev, gated):** author-placed camera splines rendered to a SEPARATE
  target (Quest: capture service / PC: render texture) — never the HMD. Speed/ease presets tuned
  for external viewing; used for store/trailer captures of compiled worlds at their best acts.
- **The gallery:** in-quarters photo wall (F3.7 signage machinery displays captures); doubles as
  the LS-6 memory surface.
**Acceptance:** HMD-sovereignty contract test (no spectator/photo code path writes the rig camera);
photo-mode perf test (capture UI ≤ budgeted draw calls); gallery persistence round-trip.
**Budget:** 4 commits. **Do not:** post-process stacks in photo mode beyond the shipped grade
families (no bloom/DoF unlocks that lie about the game); network upload (out of scope/lane).

## §5 — DC-5 · THE COSMETIC FORGE

**Why:** the progression sink (completion roadmap §5) needs a visual economy, and every asset
system it needs already exists — this envelope is wiring, not invention.

**What, strictly tiered:**
- **Tier 1 — recolors:** ship hull / glove / chest-rig / belt palette variants through
  `CosmeticDefinition` + the `ForgeModuleLook`/palette machinery. Palette-family law binds:
  cosmetics recolor within families, so no cosmetic can break a world's read.
- **Tier 2 — variants:** swapped Forge parts (grip styles, visor trims, quarters décor items) —
  ordinary recipes, ordinary class budgets, booth turnarounds, no gameplay stat coupling (art
  lane owns look ONLY; economy/unlock logic stays with the progression owner).
- **Tier 3 — earned marks:** cosmetic surfaces that render LS-6 achievements (a hull scar from a
  story chapter, expedition tally glyphs) — the memory system worn on the player's gear.
Acquisition/pricing/unlock = progression lane (claim required). The forge side ships: definition
schema + `Validate()` (family/budget clamps), applier wiring, booth sheets per cosmetic, audit
`COSMETIC_OFFFAMILY` blocker.
**Acceptance:** family-clamp tests; applier idempotence; save round-trip of equipped set;
booth sheet per shipped cosmetic. **Budget:** 3 commits + content batches.
**Do not:** stat-affecting cosmetics; off-family colorways ("neon skin" that breaks every world's
grade); unbudgeted geometry tiers.

## §6 — Cross-lane seam table (claims required BEFORE implementation)

| Seam | Owner | Chair touches |
|---|---|---|
| XRI rig / input | locked (`_Boot`) | read-only selection rays; board interactions via existing interactor path |
| Travel | `TravelCoordinator` (locked) | chamber world-visits use `TravelTo` like everything else |
| Saves / progression | gameplay lane | photo gallery + equipped-cosmetic fields are NEW claims; unlock logic stays theirs |
| CI / branch flow | infra (GPT lane) | `chair:` commits enter the same proof ladder; auto-merge rules are a named claim |
| Speech capture | companion session tooling | on-PC only; no on-device audio processing claim |
| `QuartersCameraFeature` | existing owner | extension claim for photo mode |

## §7 — Sequencing, pilot, and the acceptance sentence

Order: **DC-1 → DC-2 → DC-3 → DC-4 → DC-5.** (Safety architecture first; tuning rides it; the
chamber consumes WC-7; showcase and cosmetics close.)
**Pilot:** one compiled world from the FORGE VI pilot batch becomes the chair's test stage.
**The acceptance test is Terry's sentence, run live:** Terry stands in that world and says "move
that building." The building moves — snapped, validated, hot-reloaded — he A/B compares, accepts,
and the change lands as a CI-green commit with zero manual cleanup, while the frame never leaves
budget. Then the same session: one `recolor`, one `relight` from the tuning board, one chamber
verdict with a ceremony, one photo captured to the gallery. Anything less is a demo, not the
envelope.

## §8 — Definition of done

FORGE VII is done when the acceptance session (§7) passes end-to-end on device; every chair write
is provably inside author power (the contract test suite is green and has refused at least one
real out-of-bounds attempt in testing); the chamber has processed a full WC-7 queue batch with
ceremonies; photo mode ships to players gated-clean; and the first cosmetic batch is live in the
progression economy. At that point the Forge program's final loop is closed: the game is directed
from inside itself, and the pipeline beneath it refuses everything the direction shouldn't do.
