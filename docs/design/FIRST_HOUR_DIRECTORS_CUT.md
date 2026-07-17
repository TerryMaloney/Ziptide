# 🎬 THE FIRST HOUR — DIRECTOR'S CUT (plan of record, v2 target)

> **Status: DESIGN — no code. (Terry, 2026-07-16: "the first hour should get us through Toxic City
> having some fun along the way, then through our first Ziptide, then on to our first world —
> rebuild and defend and grow and understand the whole cycle — and the beginning of the story.")**
> This doc reconciles that directive with everything already planned and built: the 22-beat
> first-hour contract (`docs/first_hour/first_hour_beats.json`, adapters FH-S01…S07 code-green),
> `ONBOARDING_TUTORIAL.md`'s laws, the storyboard (STORY_BIBLE, CHAPTER_0-1, W001/W002),
> `SHIP_DESIGN_INTERIOR_EXTERIOR.md`, the fun-bar docs (WEAPON_FEEL, ENEMIES, RILL_CAPTION v2),
> and the recovery program's golden-slice governance. **Nothing here unfreezes frozen systems or
> reorders the recovery gates — §7 stages adoption explicitly.**

---

## 1 · THE ONE STRUCTURAL DECISION (everything else hangs on it)

**The current contract spends "the first Ziptide" on the commute.** Beat 9 labels the
W000→W001 transit `FH_FIRST_ZIPTIDE`. But Terry's directive — and the story bible, and the original
starter-world brief (city → *dormant gate site* at the end) — stage the Ziptide as **the alien thing
you discover and earn**, not the bus you arrive on.

**DECISION (recommended): re-stage the arc so the first REAL Ziptide transit is the hour's peak,
placed AFTER Toxic City — and it's your own repair work that wakes the gate.**

- **W000 → W001 becomes a SHIP FLIGHT** — same built presentation (coupler repair → PUNCH IT →
  6-second star-streak rails), minus the gate ring. Story: Cal is a broke technician flying to a
  contract. It stays a spectacle (launch #1) without spending the game's namesake on minute 17.
- **The ZIPTIDE gate is discovered IN Toxic City.** The authored W001 contract already ends with
  Cal re-seating a signal-relay node that "comes up WRONG." The director's cut makes that wrongness
  *visible*: seating the relay sends a pulse down the canal — and the derelict ring at the
  RelayVault/harbor **wakes.** Your job — the ordinary, wrench-turning job — turned on the thing
  the whole game is named after. RILL: first Stirring-tier line.
- **The FIRST ZIPTIDE transit (the built `ZiptideGateEffect`: 26 accelerating pillars, contracting
  whirlpool, destination-tinted tide, name reveal riding the crest) fires ONCE, here, at the peak**
  — carrying you to your first true gate-world, **W002 The Dry Cistern**.
- **W002 delivers the cycle in miniature** — rebuild, defend, grow — then the return home lands the
  changed-ship payoff.

Why this is right: (1) it is literally Terry's sentence; (2) peak-end rule — the hour's remembered
peak becomes the game's namesake, earned by the player's own hands; (3) the "repairing the network
IS opening the cage" theme starts working in hour one; (4) it converts W001's existing "relay reads
wrong" mystery seed from a log line into the inciting incident; (5) the ship-flight vs gate-travel
distinction gives travel two flavors forever (your ship for known space, the Tide for the network).

**Cost (honest):** contract v2 — beat 9's *label/story* changes (same tech: `TravelCoordinator`
signal `TRAVEL_W000_TO_W001_COMPLETE` is unchanged, so **FH-S03's code survives as-is**), the gate
FX is *suppressed* for the flight and *reserved* for W001→W002, and ~8 new beats extend the ledger.
Nearly everything built survives: all W000 beats, all W001 job/creature/zipline beats, the payoff
beat. See the reconciliation table (§6).

---

## 2 · THE FUN FIX IN TOXIC CITY (toy before chore)

The contracted W001 order is job → scan → repair → *then* shoot → creature → zipline. That's
homework before recess. The fun-bar docs give us the toy; the director's cut re-orders the same
beats so play precedes procedure:

1. **Arrival = awe + freedom (2 min).** Hero vista on arrival (canal city, the skyscape, the
  derelict ring silhouetted in the harbor — *plant the gate visually at minute 20, pay it off at
  minute 45*). Return door shown (tutorial law).
2. **The taser is found, not issued.** It sits on a dockside crate next to three practice cans and
  a *passive* tutorial drone (the existing trio). SHOOT beat happens as play: knock cans, zap the
  dummy drone, watch it seize and clatter (WEAPON_FEEL Phase-1 layers: 4-layer audio, shaped
  haptics, hit-confirm). 90 seconds of pure toy.
- **Zipline moves UP the arc:** the route to Dispatch crosses the canal — take the zipline *to
  reach the job*, not as errand #4. Traversal-as-fun, and it seeds the return route.
3. **THEN the job** (Dispatch/Dockmaster, the authored 4-beat contract): disable 5 feral drones
  (now using a weapon you already love, vs the ONE excellent readable drone loop from
  ENEMIES_ENCOUNTERS — telegraphs, stun, the salvage-sweep breather), scan → repair the relay
  (the technician fantasy: panel off, part in, power on), reward ceremony (100cr flying to the HUD).
4. **The signature-creature beat stays** (observe → counter, non-lethal) placed on the route to the
  RelayVault — the city is alive, not a shooting gallery.
5. **RILL captions ship at v2 spec** (lazy-follow, dark plate, ≤2×38 chars, name-tag identity) —
  readability is a first-hour feature, not polish.

Everything above uses beats and systems that already exist; only the *order* and the taser's
placement change. One-verb-per-moment and hesitation-triggered teaching stay law.

---

## 3 · THE PEAK: FIRST ZIPTIDE (min ~43–47)

Staging (all built pieces, composed):
1. Relay seats → power-cycle → the machine hums… then **overshoots**. Lights race down the canal
  conduits (the door-anchor tide idiom, reversed: energy pours *toward* the harbor).
2. RILL (Stirring): *"That relay wasn't broken, Cal. It was **sealed**."* — story line #1 of the
  game, earned not narrated.
3. The derelict ring wakes: `ZiptideGateEffect` departure staging visible **across the water**
  first (awe at distance — Alyx balcony rule), then the player walks the final approach.
4. A single gate door stands lit with one destination: **THE DRY CISTERN** (W002's name riding the
  tide). Walking through fires the full departure spectacle — pillars, whirlpool, white crest,
  arrival burst tinted by W002's underground palette. Camera never moves; rig never parented.
5. **Save reassurance** fires on this transit (the tutorial law's placement, moved here).

This is the moment the player tells someone else about. Budget real polish here and nowhere new.

## 4 · THE FIRST WORLD: W002, THE CYCLE IN MINIATURE (min ~47–62)

W002 The Dry Cistern already teaches mining + dark traversal + swarm + pumps. The director's cut
frames its existing 4-step job as the **rebuild → defend → grow** cycle, each beat SMALL:

- **REBUILD (~5 min):** the cistern is dead — restart the ancient pump (the built repair loop:
  scan → panel → part → power) and **build one extractor** on a build socket with your W001 credits
  (`BuildSocketRuntime`: pay → a real rig rises → it's *yours*, it keeps producing while you're
  gone). One repair + one build = "I brought this place back."
- **DEFEND (~4 min):** restarting the pump wakes the nesting swarm — **one 90-second wave** defends
  the machine you just fixed (existing drone-wave/`DisableDronesCount` machinery; ENEMIES pacing:
  ≤4 threats, full telegraphs, salvage sweep after). Defense of a thing you built ≠ arena combat.
- **GROW (~3 min):** water returns to the cistern → one garden plot unlocks by the pump. **Plant
  one seed** (`GardenPlotRuntime`: select soil, plant visibly sprouts). RILL: *"It'll need time.
  Things grow while you're gone, you know."* — the retention hook: the payoff is NEXT session.
- **UNDERSTAND (1 min):** collect the extractor's first hopper payout on the way out — the loop
  shown whole: repair → build → defend → grow → *it produces while you live your life*.
- Mystery thread: the pump's glyph-plate matches W001's relay (already authored) — RILL's second
  Stirring line. Two data points = a pattern = the story has begun.

## 5 · MINUTE-BY-MINUTE (the full 0–70 target)

| Min | Where | Beat(s) | Feel |
|---|---|---|---|
| 0–2 | _Boot | title → New Game | trust |
| 2–10 | W000 ship | wake → RILL greets → **comfort console** → move/grab/holster → coupler repair | "this ship is mine" |
| 10–13 | W000 | helm → destination → **PUNCH IT** ship-flight (rails, no gate FX) | launch #1 |
| 13–15 | W001 | arrival vista; **the derelict ring planted on the horizon**; return door shown | awe |
| 15–19 | W001 | the taser toy beat (cans, dummy drone) | pure play |
| 19–21 | W001 | zipline across the canal to Dispatch | traversal joy |
| 21–38 | W001 | the contract: 5 feral drones (the excellent-drone loop) → signature creature en route → relay scan/repair | competence |
| 38–42 | W001 | reward ceremony; relay reads WRONG; energy races to the harbor | uh-oh (good) |
| 43–47 | W001→W002 | **THE FIRST ZIPTIDE** (full spectacle + save reassurance) | THE peak |
| 47–60 | W002 | rebuild (pump + one extractor) → defend (one wave) → grow (one seed) | the cycle |
| 60–63 | W002 | first hopper payout; glyph-plate mystery; gate home | ownership |
| 63–68 | W000 ship | **changed ship** (trophy/decal + the W002 extractor ticking on the holo-map) → save → "next mystery" tease | peak-END |

Pacing truth: this is a **~65–70 minute** hour. If it must compress to 60: trim W001's drone count
5→3 and the creature beat becomes optional-en-route. Never trim the taser toy beat, the gate
spectacle, or the grow beat — those are the hook, the peak, and the retention hook.

## 6 · RECONCILIATION WITH WHAT EXISTS (contract v1 → v2)

**Survives untouched (code + signals):** beats 1–8 (all W000; FH-S07 surfaces, comfort law), the
W000→W001 transit *signal* (FH-S03 — only its narrative label changes: "first flight," not "first
Ziptide"), beats 10–20 (W001 arrival/job/scan/repair/shoot/creature/zipline/reward — reordered in
presentation, identical in signals), beat 22 payoff shape (FH-S08, unbuilt anyway — now lands
after W002). **Changed:** beat 9 renamed `FH_FIRST_FLIGHT`; gate-FX suppressed on ship flight,
reserved for the W001→W002 transit. **NET-NEW beats (the v2 extension, ~8):** `FH_GATE_AWAKENED`
(relay consequence), `FH_FIRST_ZIPTIDE_TRANSIT` (W001→W002, the real one), `FH_W002_ARRIVAL`,
`FH_REBUILD_PUMP`, `FH_BUILD_EXTRACTOR`, `FH_DEFEND_THE_PUMP`, `FH_PLANT_FIRST_SEED`,
`FH_COLLECT_FIRST_YIELD` — every one composes an ALREADY-BUILT system (repair loop, BuildSocket,
drone waves, GardenPlot, MiningRig hopper); no new mechanics, only new pack data + beats + RILL
lines. **Unbuilt-but-planned pieces this leans on:** FH-S05/FH-A01 (signature creature),
FH-S08 (orchestration + the RILL line set — now ~22 lines instead of 15), RILL caption v2.
**Governance notes:** gardens are currently `PROTOTYPE_HIDDEN` under the recovery freeze — the
GROW beat is *one plot, one seed*, and unhides only at Stage 2 with Terry's sign-off. W002 today
gates on `toxiccity_complete` — already the right gate for this arc.

## 7 · STAGED ADOPTION (respects the recovery program)

- **Stage 0 (now):** this doc is the target. No contract edits until the Saturday certified
  headset checkpoint passes and recovery formally exits.
- **Stage 1 (post-recovery):** ship + device-prove the EXISTING 22-beat golden slice exactly as
  contracted (one destination world). This validates every adapter the v2 arc reuses.
- **Stage 2 (contract v2):** apply §6 — rename beat 9, move the gate FX, add the 8 W002 beats,
  author the ~7 new RILL lines, unhide the single garden plot. Owner-wise this is: contract/JSON
  update (GPT's ledger + validators), W001/W002 pack data edits (worlds lane), FH-S08 orchestration
  absorbing the extended arc, Picasso on the gate-awakening staging. Each is packet-sized.
- **Stage 3 (polish):** the fun-bar passes (weapon feel Phase 1, the excellent drone, caption v2)
  land on the v2 arc — they were scoped for exactly these beats.

## 8 · ACCEPTANCE (how we know the first hour is GREAT, not just complete)

Mechanical (extends the existing gate): all v2 beats fire in order on device · every verb taught
once · no RILL line over 2×38 chars · save works at both transits · 72Hz held through the gate FX.
**Felt (Terry + the kids, one sitting each):** ① a first-time player never asks "what do I do?"
out loud; ② they can name the moment they'd show a friend (target: the gate waking); ③ they
correctly explain the cycle afterward ("I fixed it, built a miner, defended it, planted something,
and it all keeps going while I'm away"); ④ they ask to come back — and when they do, **the plant
has grown and the extractor has a full hopper.** That last one is the whole game in one sentence.

## 9 · OPEN QUESTIONS FOR TERRY (small, none block Stage 1)

1. Peak placement sign-off: gate wakes AT the harbor ring (walk-up approach) vs inside RelayVault
  (interior reveal)? Recommended: harbor — visible from arrival, pays off 25 minutes of skyline.
2. Should the W002 defend wave be skippable for very young players (kid mode: wave size 2, no
  fail state)? Recommended: yes — comfort-preset-linked (Cozy = gentler wave).
3. The changed-ship payoff object: trophy (salvaged drone core on the shelf) vs functional (the
  holo-map now shows W002's extractor ticking)? Recommended: BOTH — one emotional, one systemic.
