# GROWTH ROUNDS — the whole game gets better at once, and the factory gets faster every round
### The operating cadence after M0: select → build → verify → retro → grow
**Status: PROPOSED OPERATING SYSTEM — Terry-directed concept (2026-07-24), activates AFTER the
M0 headset verdict + freeze lift. Complements, never replaces, the depth spine
(`docs/production/POST_HEADSET_WORLD_FACTORY_ORDER.md`: M0 → W000→W001 band → W002 replication).**

Terry's directive, verbatim intent: *"after we prove the system works we improve two or three
things about everything, and then check and see if they all went through, and if they don't we
figure out why and fix it so that next time we should be able to — and then we pick up speed."*

---

## 1 · THE SHAPE

The project has two growth axes, and they are different machines:

- **The DEPTH axis** (already canonical): the world-production spine — first hour, W001, W002
  replication, then batches. This proves the factory can make *a game*.
- **The BREADTH axis** (this doc): **growth rounds** — recurring waves where *every* aspect of
  the game improves by 2–3 small, visible steps simultaneously, followed by one verification
  sweep and one retrospective that converts every failure into a system fix.

A round is not a sprint board of feature work. It is a **pulse**: the whole organism grows a
ring, we X-ray the ring, and we fix the growth machinery wherever the ring came in wrong.
Speed compounds because the retro output is *always* framework work — the same automation
ratchet the world factory uses, generalized to every aspect.

## 2 · THE ROUND CYCLE (five phases, one headset session as the heartbeat)

**Phase 0 — SELECT (one operator, ~an hour, Terry approves the slate).**
From each EXCELLENCE_MAP aspect row, pick **1–3 improvements**, each sized **≤3 commits**, each
**observable** — visible in headset, audible, or provable by a new passing test on new behavior.
*Documentation does not count as an improvement.* (Docs are how we work, but a round measures
the GAME growing, and the 1,082-doc-touches-to-1-visuals-commit week is the failure mode this
rule exists to prevent.) The slate lands as `docs/rounds/ROUND_NN.md` — a checkbox ledger:
improvement · aspect row · owner lane · size · how it will be verified.

**Phase 1 — BUILD (all lanes parallel, WIP-locked).**
Lanes execute their slate items under the two-lane gate policy: **spine changes** (boot / rig /
input / travel / save / packages) ride the full proof ladder; **everything else** needs compile
+ tests green and ships to the round build. Nothing new enters the slate mid-round — ideas
discovered mid-round go to the next round's candidate list. The circuit breaker (3 CI-reds on
one item → stop, mark blocked, move on) protects round tempo.

**Phase 2 — VERIFY (the sweep).**
One batch: CI green on the round head → audits → contact sheets for anything visual → **one
Terry headset session that walks the round** — a written route visiting every observable
improvement, checkbox by checkbox. The headset session IS the round boundary; rounds are sized
to Terry's real availability (about one session per round). Each item gets a verdict:
**LANDED · LANDED-BUT-WRONG (works, reads badly) · FAILED (didn't survive to device) ·
UNTESTABLE (couldn't be reached/observed — itself a failure of the slate, not of Terry).**

**Phase 3 — RETRO (where the speed comes from).**
For every non-LANDED item, classify the cause and file the fix as **framework work**:
- content fault → next round's slate for that aspect;
- **system fault** (author didn't regenerate, gate checked the wrong universe, scene stale,
  bake needed a human, verdict machinery lied) → a named fix in the factory itself, RED-CAUSE
  tagged, ratchet-logged. System faults are gold: each one removed makes EVERY future round
  cheaper. The retro is ~10 lines appended to the round ledger, not a new document.

**Phase 4 — GROW.**
Next round's slate may expand only as fast as the land-rate justifies: land-rate ≥80% → grow
the slate ~25%; 50–80% → same size; <50% → SHRINK and spend the round on system faults. This
is the compounding law: **we go faster by removing friction, never by pushing harder.**

## 3 · THE THREE NUMBERS (the only metrics, kept in the round ledger)

1. **Land-rate** — LANDED / attempted. The health of the factory.
2. **System-fault count** — how many retro items were factory bugs. Should trend to zero;
   a rise means the factory changed under us.
3. **Round span** — calendar days select→retro. Should trend down at constant slate size.

Nothing else is tracked. Three numbers, one ledger line per round, and after four rounds the
trend answers Terry's real question ("are we picking up speed?") with data instead of vibes.

## 4 · GUARDRAILS (the failure modes, pre-answered)

- **The peanut-butter trap** — spreading effort so evenly nothing gets *good*. Answer: the
  depth spine (first hour / world band) is always the LARGEST slate allocation (~⅓ of the
  round); breadth rounds decorate the spine, never starve it.
- **The invisible-improvement trap** — "refactored X" landing as a round item. Answer: the
  observability rule in Phase 0. If Terry can't see/hear/feel it and no new test proves new
  behavior, it's not a round item.
- **The mid-round pivot** — a great idea arriving on day 2. Answer: candidate list for round
  N+1. The exception is a real blocker discovered mid-round; the circuit breaker handles it.
- **Spine churn** — five lanes touching input/save in one round. Answer: spine items are
  capped (2 per round) and serialized, never parallel.
- **Verification debt** — a round "done" without the headset walk. Answer: a round without
  Phase 2 is not a round; it's just work. The ledger stays open until the walk happens.

## 5 · ROUND 01 — proposed slate seed (Terry adjudicates after M0; sized deliberately small)

The first round is half-size on purpose: its real product is a measured land-rate baseline.

| Aspect | Improvements (each ≤3 commits, observable) |
|---|---|
| **First hour (spine, largest share)** | FH-S07 bake + device pass · beat-1-to-3 walkable start-to-finish |
| City/visuals | City Stage A building grammar, constants from `CITY_VISUAL_SPEC.md` · ToxicCity regen |
| Weapons feel | pistol+taser grip/recoil/haptics Phase 1 |
| Enemies | drone telegraph + hit reaction pass |
| Machines/growing | BioRefiner Mk1 keeper → measured spec → Forge primitive build (the pipeline back-half pilot) |
| Audio | first Suno-lane cue in W000 + travel stinger audible |
| Ship shell | HMD-doff pause+mute (0.2) — spine-adjacent, full ladder |
| Photo/capture | Field Camera author bake + first on-device photo on the Quarters wall |
| Story | Dockmaster contract signed `>_ V. BOOTSTRAPPER` visible on the kiosk |
| Factory itself | recipe-hash law S1 on ONE create-only author (vista library) as the pilot |

Ten aspects, ~14 items, one headset walk to verify all of it. If that lands at 80%, round 02
grows; whatever fails teaches us exactly which part of the factory still lies.

## 6 · WHY THIS ISN'T MORE PROCESS (the honest defense)

Everything in this doc already exists as parts: EXCELLENCE_MAP (the aspect registry), the
automation ratchet (retro→framework), RED-CAUSE labels (system-fault visibility), the two-lane
gate policy, Terry's session cadence (already the de-facto heartbeat), the circuit breaker.
Growth rounds add exactly TWO new things: a **slate with an observability rule** and a
**land-rate number**. Both exist to answer the one question the current setup cannot:
*is the whole game actually growing, and are we actually getting faster?*
