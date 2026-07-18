# 🎬 THE FIRST HOUR — DIRECTOR'S CUT v2.1: "THE KEY THAT KNEW YOU"

> **Status: DESIGN — no code. (Terry, 2026-07-16/17.)** v2 reconciled Terry's expanded arc with the
> 22-beat contract; **v2.1 adopts Terry's artifact story** (space-salvage find → second half via a
> client mission → the joined key beacons to YOUR OWN SHIP → plug it in → BOOM, first Ziptide) and
> answers the open story questions with recommended calls. Everything stays data-driven
> (beats/flags/pack data/RILL lines) so the arc can be re-staged if we change direction — that
> modifiability is a REQUIREMENT (§8), not an accident.
> Sources reconciled: `docs/first_hour/first_hour_beats.json` (+ adapters FH-S01…S07 code-green),
> `ONBOARDING_TUTORIAL.md`, storyboard (STORY_BIBLE / CHAPTER_0-1 / W001 / W002),
> `SHIP_DESIGN_INTERIOR_EXTERIOR.md` §10 "THE SHIP RIDES THE TIDE", the Space Lane
> (`ShipFlightRuntime` fly/stun/salvage — built), fun-bar docs (WEAPON_FEEL / ENEMIES /
> RILL_CAPTION v2), recovery governance (`docs/recovery/RECOVERY_PROGRAM.md`).

---

## 1 · THE SHAPE (one paragraph)

Cal wakes broke on a junker ship, learns her hands (W000), flies a **space salvage sortie** and
pulls something out of a wreck that *should not exist* — half of an artifact. A paying contract in
**Toxic City** (the fun: taser toy, ziplines, feral drones, the technician job) turns out to be held
by the client who has **the other half**. Joined, the key wakes — and its beacon threads across the
sky to… **Cal's own berth.** The key chose her ship. She seats it into the coupler socket she
repaired an hour ago, pushes PUNCH IT — and the ship rides **the first ZIPTIDE** to a world no door
leads to: **W002, The Dry Cistern**, where she rebuilds, defends, and plants — the whole cycle in
miniature — then rides home to a ship that is visibly no longer ordinary.

**Why this beats v2's harbor-gate staging:** the peak moment happens AT and THROUGH the player's
own ship (peak + ownership in one beat); the tutorial coupler repair becomes a loaded gun that
fires at minute 45; "the artifact knew you" plants the Transmission's identity layer invisibly; and
ship-as-gate-vessel is already the hero-ship doc's canon (§10) — we're staging existing intent,
not inventing.

## 2 · THE ARTIFACT THREAD (story beats + dialogue direction)

Voice per the bible: Cal talks to machines like they're being stubborn; RILL is dry, glitchy,
Dormant→Stirring across the hour. Lines below are DIRECTION, not final VO — they live as data
(`RillLineAuthor` pack data) and are swappable.

1. **THE FIND (space, ~min 13–19).** A wreck-clearance contract — posted by one *V. Bootstrapper*
  (§2b), pays badly, coordinates oddly precise. Salvage sortie on the built Space Lane: fly the
  rings, stun one derelict drone, close to salvage range on the wreck. Among the scrap, one object doesn't
  scan. RILL: *"Scrap… scrap… hull plate… wait. That one's not scrap. That's not supposed to be —
  anywhere."* Cal (grabbing it): *"So what are you supposed to be?"* It's HALF of something —
  a clean fracture face, machined by nobody's tooling. It goes in the hold. (Physically: a
  grabbable artifact item, holstered/stowed — it travels, per the holster contract.)
2. **THE JOB (Toxic City, ~min 20–40).** The Dockmaster's contract is the authored W001 loop
  (drones, relay repair). But docked at the berth, the half **hums**. RILL: *"It started doing
  that when we landed. I don't love it."* The contract's payment includes a "paperweight" the
  Dockmaster wants gone — junk that kills dock instruments. It's **the other half.** (Story
  economy: the relay-reads-WRONG seed stays — the relay and the artifact resonate on the same
  wrong frequency; two mysteries become one.) The client of record on the work order:
  `>_ V. BOOTSTRAPPER` — same signer as the salvage job (the noticing beat lands after the
  join, §2b.3).
3. **THE JOIN (~min 41–43).** Two-handed VR beat: bring the halves together — they *pull* at
  ~20cm (magnetic snap, strong haptics), seat with a deep clunk. A thin thread of teal light
  rises off the key and lies down across the sky — a path. RILL: *"That's a heading. It goes…"*
  (beat) *"…back the way we came."*
4. **THE REALIZATION (the hangar walk, ~min 43–45).** Following the thread through the shipyard —
  it descends berth by berth and pours into **berth six.** Cal: *"That's OUR berth."* RILL:
  *"…I know."* The thread ends at the ship's **coupler socket** — the one Cal repaired at minute
  eight. RILL (Stirring, first tier-shift line): *"Cal. It's not a beacon. It's a key. And it
  already knows the lock — it rewrote its own fitting. To OURS."* ("It knew who you were" —
  said about the ship, felt about Cal. The Transmission stays buried.)
5. **THE FIRST ZIPTIDE (~min 45–48, THE peak).** Seating the key mirrors the coupler repair
  (panel → seat → power: the tutorial's muscle memory, now mythic). The console lights a
  destination that has no name Cal's charts know — just a depth reading. PUNCH IT: cast-off rails
  fire, and mid-streak the full `ZiptideGateEffect` erupts *around the ship* — 26 pillars off the
  hull, the whirlpool crest, W002's palette tinting the tide, THE DRY CISTERN riding the wave.
  Save reassurance lands on this transit. Camera never moves; rig never parents.
6. **THE OTHER SIDE (W002).** Arrival burst; underground dark; RILL quieter than usual:
  *"…I've been here. I don't remember when. I don't remember* being *anywhere."* (Stirring #2.)
  The cycle plays (§4). The glyph-plate on the cistern pump matches the key's fracture face —
  third data point, pattern confirmed, hour ends with the mystery OPEN.
7. **HOME (~min 63–68).** The key stays mounted — the hull socket **glows faintly forever** (the
  changed ship is the story, not a trophy). Holo-map now shows two things ticking: W002's
  extractor and a second faint depth-reading further down the thread. RILL: *"So. Do we tell
  anyone?"* Hard save. End of hour.

## 2b · THE QUEST-GIVER: VEX BOOTSTRAPPER (Terry-directed; recommended treatment)

**The name (Terry's pick, grounded):** *bootstrapping* is, literally, the process by which a
computer starts itself from nothing — the first tiny program whose only job is to load everything
that comes after. **"Vex"** carries *vector* (an entry point; how something gets in) and his
permanent mood. And the name hides in plain sight: in salvage-world slang, a *bootstrapper* is
someone paid to restart dead ships and derelicts. Nobody in-universe thinks the name is strange.
That's the trick — the cosmic function wearing an ordinary trade name. (Alternates on file if the
feel shifts: *Colonel Bootstrapper* (military-surplus comedy), *Old Man Init* (deeper cut — `init`
is the first process a computer starts; every other process is its descendant). Data-swappable
like everything else.)

**What he is (surface, hour one):** a contract broker Cal has never met. A voice-and-signature on
the dispatch network — dry, businesslike, oddly *specific*. His signature glyph is a chevron and
underscore: **`>_`** (the command prompt, hiding as a logo). **He is not physically present in
hour one** — cheaper, spookier, and modifiable: a body can be added any time without touching the
beats.

**What he is (sealed canon, THE_TRANSMISSION layer):** the universe-program evolved a role the way
ecosystems evolve niches — *the process that starts processes*. Vex doesn't know what he is. He
thinks he's a broker with uncanny luck for finding work that "needs doing." When the system needed
its Debugger launched, the job routed through him — because that is what he is FOR. He is the
Matrix-Oracle pattern played as a working stiff: not a guide, not a sage — **a command prompt with
a payroll.** (Hour one reveals NONE of this.)

**How he threads the hour (two signatures, one realization):**
1. The **space-salvage contract** (min ~13) is posted by *V. Bootstrapper* — routine wreck-clearance,
  pays badly, oddly precise coordinates. This is how Cal finds half #1.
2. The **Toxic City contract** (min ~27) comes through the Dockmaster — but the *client of record*
  on the work order is the same signature: `>_ V. BOOTSTRAPPER`. His "payment in kind" is the
  junk paperweight — half #2.
3. The noticing beat (min ~42, right after THE JOIN): RILL, reading both work orders: *"Two
  contracts. Two halves. One signer. Cal — who is Vex Bootstrapper?"* Cal: *"Somebody with
  terrible taste in paperweights."* The question is planted, unanswered, and becomes a series
  engine: **every act can open with a Vex contract** — the player learns to feel the pattern
  ("a Bootstrapper job means the story is about to move") long before anyone explains it.

**Diegetic surfaces (all existing tech):** contracts on the DispatchKiosk/board carry a signer
line + the `>_` glyph; RILL reads his messages aloud (caption v2 name-tag: VEX, in a distinct
accent color); no new systems. **Canonization note:** this character touches the story bible —
final canon entry belongs to the story lane with Terry's sign-off; this section is the design of
record until then.

## 3 · TOXIC CITY: THE FUN (unchanged from v2, order is law)

Toy before chore: arrival vista (return door shown) → **found taser + cans + dummy drone** (90s of
pure play; WEAPON_FEEL Phase-1 feedback layers) → **zipline TO the job** → the authored contract
(5 feral drones via the excellent-drone loop, signature creature en route, relay scan/repair
ceremony) → reward + the resonance beat (§2.2). One verb per moment; hesitation-triggered RILL;
captions at v2 spec. Never trim the toy beat.

## 4 · W002: THE CYCLE IN MINIATURE (unchanged from v2)

**REBUILD** (pump repair + build ONE extractor with W001 credits — `BuildSocketRuntime` rises a
real rig that keeps producing) → **DEFEND** (restarting the pump wakes the swarm; one 90-second
wave protecting the machine you fixed; ENEMIES pacing, salvage sweep after) → **GROW** (water
returns; plant ONE seed — RILL: *"It'll need time. Things grow while you're gone, you know."* —
the next-session hook) → **UNDERSTAND** (collect the extractor's first hopper payout on the way
out: the loop, shown whole).

## 5 · MINUTE-BY-MINUTE (0–70 target)

| Min | Where | Beat(s) | Feel |
|---|---|---|---|
| 0–2 | _Boot | title → New Game | trust |
| 2–10 | W000 ship | wake → RILL → **comfort console** → move/grab/holster → **coupler repair** | "this ship is mine" |
| 10–13 | W000 | helm → PUNCH IT → cast-off rails (ship flight, no gate FX) | launch #1 |
| 13–19 | Space Lane | fly rings → stun one derelict → salvage → **THE FIND** (artifact half #1) | wonder + "huh?" |
| 19–21 | W001 | dock at Toxic City; arrival vista; return door | awe |
| 21–25 | W001 | taser toy beat (cans, dummy drone) | pure play |
| 25–27 | W001 | zipline across the canal to Dispatch | traversal joy |
| 27–39 | W001 | the contract: 5 feral drones → signature creature → relay repair → reward **+ half #2** | competence |
| 41–45 | W001 | **THE JOIN** → the beacon thread → the hangar walk → **berth six** | chills |
| 45–48 | ship | seat the key in the coupler → PUNCH IT → **THE FIRST ZIPTIDE** (full spectacle + save) | THE peak |
| 48–61 | W002 | rebuild (pump + extractor) → defend (one wave) → grow (one seed) | the cycle |
| 61–63 | W002 | first hopper payout; glyph-plate matches the key; gate home | ownership |
| 63–68 | W000 ship | glowing key socket · holo-map ticking (extractor + a deeper reading) · RILL's question · save | peak-END |

~68–70 min honest. Compression valve if needed: space sortie 6→4 min (2 rings), drones 5→3.
Never cut: the toy beat, the join/realization, the gate spectacle, the seed.

## 6 · RECONCILIATION (what changes vs the 22-beat contract)

**Survives untouched (code + signals):** all W000 beats 1–8 (FH-S07; the coupler beat gains story
weight, zero code), the W000→W001 travel signal (FH-S03 — beat 9 renames to `FH_FIRST_FLIGHT`),
all W001 job/scan/repair/shoot/creature/zipline/reward beats 10–20 (presentation reorder only),
the payoff shape (FH-S08, unbuilt — absorbs the extended ending). **Changed:** beat 9 label; gate
FX suppressed on the flight, reserved for the key transit. **NET-NEW beats (~11):**
`FH_SPACE_SORTIE` + `FH_SALVAGE_FIND` (Space Lane — built system, new mission data),
`FH_ARTIFACT_RESONANCE` (W001 dock), `FH_ARTIFACT_JOINED`, `FH_BEACON_FOLLOWED`,
`FH_KEY_SEATED`, `FH_FIRST_ZIPTIDE_TRANSIT` (ship→W002), `FH_REBUILD_PUMP`, `FH_BUILD_EXTRACTOR`,
`FH_DEFEND_THE_PUMP`, `FH_PLANT_FIRST_SEED` (+ `FH_COLLECT_FIRST_YIELD` folded into the exit).
Every net-new beat composes a BUILT system: Space Lane flight/salvage, item grab/holster, the
repair loop (key-seating reuses its panel/part/power verbs), cast-off, `ZiptideGateEffect`,
BuildSocket, drone waves, GardenPlot, MiningRig. New content = mission/pack data, ~10 RILL lines,
one artifact ItemDefinition (two halves + joined form), one hull socket visual. **No new
mechanics.** Governance unchanged from v2: Stage 1 device-proves the EXISTING contract first
(Saturday's golden slice); the garden plot unhides at Stage 2 with Terry's sign-off.

## 7 · STAGED ADOPTION (unchanged)

Stage 0 = this doc is the target · Stage 1 = ship + device-prove the existing 22-beat slice ·
Stage 2 = contract v2.1 (rename beat 9, add the ~11 beats, author lines, artifact item, unhide one
plot) — packet-sized per owner (GPT's ledger/validators own the JSON; worlds lane owns pack data;
FH-S08 absorbs orchestration; Picasso stages the join/beacon/hull-socket looks) · Stage 3 = the
fun-bar polish lands on these beats.

## 8 · MODIFIABILITY (Terry's requirement, made mechanical)

The arc must survive a change of heart. Rules: every story beat is **contract JSON + pack data +
RILL line data** — never hardcoded sequencing in a runtime class; the artifact is an ordinary
`ItemDefinition` + profile flags (`ARTIFACT_HALF_A/B`, `ARTIFACT_JOINED`, `KEY_SEATED`); the
beacon target, the first gate destination, and which mission yields each half are **data fields**,
so "the other half is on a different world" or "the beacon points somewhere else" is a data edit,
not a refactor. The FH-X02 progression core already evaluates beat order from data — extending the
ledger is the designed path. If we later drop the artifact entirely, the v2 harbor-gate staging in
git history remains a valid fallback arc on the same beats.

## 9 · STORY CALLS (Terry delegated — my recommendations, all data-swappable)

1. **Where the gate moment happens: AT YOUR SHIP** (v2.1 core). The tide erupts around the hull
  mid-cast-off. Strongest possible fusion of peak + ownership; and the ship-as-tide-vessel is
  hero-ship canon.
2. **Kid-mode defend wave: YES** — linked to the comfort preset (Cozy = 2 drones, no fail state,
  the pump never dies; Standard = 4 drones). Family playtest is an acceptance criterion.
3. **Changed-ship payoff: the KEY ITSELF** — the mounted artifact glows on the hull forever, plus
  the holo-map ticking (W002 extractor + the deeper reading). Emotional + systemic in one object;
  no separate trophy needed in hour one (the trophy shelf starts filling in hour two).
4. **How much "it knew you" to reveal: almost nothing.** RILL says the key rewrote its fitting to
  *the ship*. Nobody says "you." The Transmission layer stays sealed; players who replay hour one
  after finishing the game should get chills. That's the test of the line.

## 10 · ACCEPTANCE (how we know it's GREAT)

Mechanical: all v2.1 beats fire in order on device · every verb taught once · captions ≤2×38 ·
saves at both transits · 72Hz through the gate FX. **Felt (Terry + the kids, one sitting):**
① nobody asks "what do I do?" aloud; ② they name the moment they'd show a friend (target: the
beacon pointing at YOUR ship, or the tide swallowing it); ③ they explain the cycle back
("fixed it, built a miner, defended it, planted a seed — it all runs while I'm away");
④ they ask to come back — and the plant has grown, the hopper is full, and the key is still
glowing. That last sentence is the whole game.
