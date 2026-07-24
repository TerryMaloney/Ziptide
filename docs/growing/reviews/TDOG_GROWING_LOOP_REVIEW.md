# T-DOG REVIEW — Growing/Invention Loop: device, child-usability, narration, performance, interruption, release
### Independent first opinion (blue-team + red-team), source-audited. Per `GROWING_INVENTION_LOOP_REVIEW_PACKET.md`. No runtime implementation. 2026-07-23, Fable 5 C-lane/T-Dog.

---

## 0 · Overall opinion (the answer Terry asked for)

**This is the right long game, and it is unusually well-disciplined for a "big plan."** The thesis
— the garden as one half of an invention economy, not a farming minigame — elegantly solves a
real, audited gap (24 authored species, most unsurfaced; belts/machines built but purposeless).
The plan extends existing owners instead of forking, bakes in determinism/anti-grind/anti-FOMO
laws, and gates itself behind M0 and the W000→W001 production order. Blue-team verdict: **adopt
the thesis and pillars.** Red-team verdict: the plan's real dangers in my lane are exactly four —
**narration cost, menu physicality on Quest, object-count performance, and rotation scope** — and
all four have cheap, already-canon answers below. The single most important schedule opinion:
**nothing here jumps the queue.** The §21 minimum rides INSIDE the locked post-M0 W000→W001 band
or not at all (and note: the W001 scene does not exist yet — the band creates it).

## 1 · Source-audit findings (facts the plan asked for)

1. **The belt has exactly THREE holster sockets** — `BeltRig.cs:58-60` ensures `HolsterRight`,
   `HolsterLeft`, `HolsterCenter`. Terry's "approximately three" is confirmed. The travel law
   (only holstered items travel) already runs through these sockets and is save-proven.
   **Recommendation: lock the field kit at 3 slots, period.** Kid-countable, matches existing
   muscle memory and persistence, and slot scarcity is the proven meaningful-choice engine
   (progression doc §3). Resist "heavy items cost 2" in v1 — one rule, three slots.
2. **Garden state is real and save-shaped**: `PlotState` lives in `EconomyState.cs` with
   genetics (`PlantGenetics`, tested), watering, and the IdleEngine absolute-time pattern —
   growth already survives quit/doff/offline by construction. Automation persistence is
   belt-tested. **Interruption-safety is a genuine blue-team advantage of building on these
   owners** — most games bolt this on late; ours is already the foundation.
3. **Reach/readability gates already exist for the catalog UI**: `InteractionReachAuditRules`
   (0.35–1.9 m, the child-reach law) and `UiReadabilityAuditRules` will gate garden/catalog
   surfaces the day they exist. The accessibility contract in §17 is enforceable, not aspirational.
4. **Narration has zero existing runtime**: no VO, no TTS, one committed audio clip. The §9
   narration behavior is the plan's largest NEW technical+content surface (see §2).

## 2 · Narration — the biggest new cost center, and the answer (§20 Q8)

- **CUT: runtime TTS on device.** Quest TTS means an Android plugin seam, robotic voices judged
  harshly in a kids' title, per-device variance, and a localization trap — and it violates our
  determinism-and-budgets culture (unbudgetable audio at runtime). Do not build it.
- **KEEP NOW (as the contract): prerecorded, batch-generated speech keyed to stable IDs.** Every
  catalog item already must have a stable ID (§7); the audio master plan already defines
  `aud.<family>.<event>` naming, the `ZiptideMix` bus tree, ducking data, and the caption-twin
  law. Narration slots in as `aud.narrate.<itemId>.name` + `.function` clips on ONE narration
  bus that ducks under RILL and alerts. Generation is a batch pipeline job (same lane as RILL VO
  when it lands); until clips exist, **captions carry the labels** — the caption-twin law
  already requires that path, so narration ships progressively without blocking the loop.
- Scope control: speak **name on focus** (after ~0.5 s anti-chatter dwell) and **one-sentence
  function on request**. Full recipe requirements/counts are CAPTION-ONLY (reading a parts list
  aloud is fatigue, not accessibility — §19.9). Speech rate/volume sliders ride the settings
  program. This cuts the recorded-line count from "every string in the system" to
  **2 short clips per catalog item** — budgetable inside the ≤24 MB audio ceiling.
- Release upside worth naming: this narration contract directly answers VRC Accessibility.1/2/3
  (recommended tier) — a store-quality-rating win few kid titles actually ship.

## 3 · Child usability on Quest — physical beats menus (§20 Q7, Q10)

The inventory-burden risk (§19.3) and kid-usability contract (§17) have one shared answer:
**make it physical, not list-shaped.** Seeds as stackable pucks in labeled tray slots; the plot
as sockets you drop seeds into (base/trait/catalyst roles = differently-shaped sockets — the
recipe grammar becomes VISIBLE geometry); catalog cards as physical cards on the bench (the
holo-bench/chip-in-socket canon from the progression doc). A six-year-old cannot operate a
scrolling VR list; they can absolutely post a round seed into a round hole. This also makes
PG-4/UiReadability gates directly applicable, keeps hands-first identity, and kills the
menu-clutter risk structurally. Confirm-before-spend (§17) becomes a physical lever pull —
consistent with PUNCH-IT canon. **Recommendation: state "no scrolling lists anywhere in the
garden/catalog v1" as a design law.**

## 4 · Performance red-team (§19.11)

Plants + growth + machines + helpers multiply persistent objects on the ship AND in worlds.
Requirements to lock before build: per-plot object budgets in the same manifest-budget pattern
the WorldImprovement framework already enforces · growth animated by material/blendshape, never
per-plant `Update()` scripts · GamePool for produce/seeds · the ship interior (where most of
this lives) gets its own PerfBudget row the day it exists · helpers/sprites are the #1
object-count inflator with AI+save tails — **PARK sprites as systems; ambient visual-only life
is fine later.** Soak evidence: 20-plot + 3-machine steady state joins the §10d thermal row on
the device checklist when the slice exists.

## 5 · Interruption & save (§19.12) — mostly already won, two additions

Absolute-time growth (IdleEngine pattern) + belt-tested automation persistence + atomic profile
= the hard part exists. Add two laws to the master plan: **(a) recipe COMMIT is atomic** — the
confirm gesture either fully spends+starts or nothing (no doff-mid-commit half-states; joins
DEVICE_TEST_CHECKLIST §10b as "doff mid-recipe-commit" when built); **(b) no real-time-pressure
steps anywhere in cultivation** — nothing in the garden may fail because the player doffed,
slept, or was called to dinner. Both are one-sentence pillar additions; both are
kid-and-cert-critical (VRC Functional.2/4 alignment).

## 6 · Release & rotation red-team (§15, §20 Q13)

- **Campaign layer: fully compatible with the Mixed Ages / COPPA / offline-first release posture**
  — deterministic progression, no FOMO, offline play. No concerns.
- **Online rotation: PARK entirely until online exists at all.** A rotation service implies
  backend + schedule ownership + moderation surface + a COPPA-relevant data channel — all
  inconsistent with the offline-first release plan and the deferred-online decision on record.
- **KEEP LATER (the safe freshness mechanism): local deterministic date-hash rotation** — the
  "Today's Salvage Contract" pattern: seeded from the date, computed on device, no server, no
  FOMO (rewards never expire, rotation only re-flavors optional variety). This delivers §15's
  intent with zero backend and zero cert exposure. If/when real online ships (M7b), revisit.
- Abuse-risk scan for my lane: offline, no UGC, no chat, no purchases → the only real vectors
  are dark-pattern drift (rotation — parked) and grind (§16 laws cover it). Clean.

## 7 · Verdict table (KEEP NOW / KEEP LATER / CUT OR PARK)

| Item | Verdict | Note |
|---|---|---|
| Thesis + pillars 3.1–3.7 | **KEEP NOW** | Lock after reconciliation; add §5's two interruption laws |
| 3-slot field loadout | **KEEP NOW** | Source-confirmed sockets; one rule, no weight tiers in v1 |
| Three-role recipe grammar (base/trait/catalyst) | **KEEP NOW** | As PHYSICAL socket geometry (§3) |
| §21 W000/W001 minimum slice | **KEEP NOW** | Only inside the locked post-M0 production band |
| Prerecorded stable-ID narration (name+function) + caption fallback | **KEEP NOW** (contract) | Clips land via the audio batch lane; captions carry until then |
| Invention catalog as physical cards | **KEEP NOW** | "No scrolling lists" as law |
| Same-family adjacency demo | **KEEP NOW** | One lesson in the slice; breeding depth later |
| Machine tiers beyond extractor+assembler | **KEEP LATER** | After W002 replication proof |
| Hybrid/mutation depth, cross-world combos | **KEEP LATER** | Optional-surprise layer, post-proof |
| Local date-hash rotation | **KEEP LATER** | The only rotation form before real online |
| Conveyor automation | **KEEP LATER** | Plan already defers it — agree emphatically |
| Garden helpers/sprites as SYSTEMS | **PARK** | Ambient visual-only life acceptable later; no AI/save/roles in v1 |
| Runtime TTS | **CUT** | Prerecorded + captions is the whole answer |
| Online rotation service | **PARK** | Until online exists; date-hash covers freshness |
| Narrating full recipe text/counts | **CUT** | Caption-only; speech = name + one-line function |
| Any new inventory/menu system | **CUT** | Must reuse belt/holster + physical trays + existing profile owners |

## 8 · Answers to my assigned §20 questions

**Q7 (loadout):** 3 slots — the sockets that already exist. **Q8 (narration):** §2's contract.
**Q13 (rotation):** campaign yes; online rotation parked; date-hash local rotation later.
**Q14 (audits/tests/device):** reach+readability audits already apply; add per-plot object
budgets to the manifest-budget pattern; recipe-atomicity EditMode tests; doff-mid-commit +
garden-soak rows to DEVICE_TEST_CHECKLIST §10 when the slice lands; narration anti-chatter as a
testable timing constant; the six-year-old pass uses the kid-playtest methodology already on
record (picture scales, "younger or older than you", observe-don't-interview). **Q15 (cut to
protect schedule):** everything in the PARK/CUT rows, plus: hold the entire lane behind the
M0 verdict and the W000→W001 band — the factory proof outranks the garden.
