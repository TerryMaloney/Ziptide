# ASSEMBLY READINESS — what's still missing before the game can "build itself"

**Status:** 🔵 AUDIT + PROPOSALS — planning only, zero code. Terry commissioned 2026-07-18:
*"hypothetically we should have a framework to build each world and creature and architecture
and music etc — the game should build itself. What are we still missing?"* Scope: the MAIN
game (Terry excluded game-mode variants — they need their own pass later).

## §0 — The honest headline

The ART of a world builds itself on paper (Forge I–VII + World Compiler + music formula + SFX
matrix). What does NOT yet build itself is the world's **content and connective tissue**: the
jobs the player does there, the words they find there, the moments that are staged there, the
flags that wire it to the story, and the assembly manual that runs all the frameworks in
order. Six real gaps, plus the ruling on Terry's cutscene question.

## GAP A — Ride-scenes: the VR-legal cutscene (Terry's instinct, adapted)

**Terry's suggestion:** CoD-style steered-path mini-scenes. **The VR ruling:** steering the
player's BODY while they stand still is the #1 comfort crime — our freedom contract (LS-2)
forbids taking head or legs, and that law holds. **But the instinct is right and VR has a
proven legal form: the CONVEYANCE.** Alyx opens on a balcony then a TRAM; the player consents
by BOARDING. We already own the vehicles: the ship, lifts, ziplines, canal barges, the gate
crossing itself.
**The design:** a `rideScene` extension to the LS-2 staging grammar — an authored moment that
plays out while the player rides something they chose to board: barge glides the canal while
the tenements stage a collapse-reveal; the lift crawls past three floors of the Bloom; the
ship's first flight IS already one. Player's head stays sovereign (look anywhere), hands stay
live (Cal can lean/grab/shoot), the CONVEYANCE is on rails — never the person. Skip = the
conveyance speeds up (diegetic: Cal pushes the throttle). One per world MAX (rarity law).
**Home:** Forge V LS-2 gains the `ride` event kind; conveyance ownership = existing traversal/
ship owners (claims). **This is the missing "little cinematic moments" layer, comfort-legal.**

## GAP B — The world gameplay genome (the content half WC-1 doesn't cover)

The World Compiler recipe (WC-1) builds a world's BODY (layout/kits/vista/roster). Its
PURPOSE — the contract, job steps, encounter placements, the reward — is hand-authored per
world in `WORLD_DATA.md` (fine for 12 worlds, the bottleneck for 80).
**The design:** a `WorldContentGenome` beside WC-1: contract template id (from a closed set of
job SHAPES: repair-chain · salvage-sweep · escort-the-machine · defend-the-work · trace-the-
signal · grow-and-harvest…), step count/difficulty tier, encounter beats keyed to the flow
template, reward table ref, mystery-object slot, wreck-log slot, ride-scene slot (GAP A),
completion flags. `Validate()` + a completeness audit (a world with no contract = blocker).
The step-type vocabulary already exists (`WORLD_DATA.md` §0.1) — this systematizes what the
first 12 worlds proved by hand.

## GAP C — The Lore Forge (words at 80-world scale)

The hidden-story devices are canon: one mystery object per world, wreck logs that chain into
a second story, environmental readables, per-world glyphs. Nobody owns PRODUCING ~60 wreck
logs + 80 mystery objects + readable sets, or validating the chain.
**The design:** a lore registry (data + author, like every library): each world's
mystery-object id + one-paragraph payoff note, wreck-log id + text + its position in the
chain, readable set id. Chain validation in CI (logs form an unbroken readable order; every
world's slots filled or explicitly waived). Writing quality stays human/Terry-reviewed (the
RILL-lines PROPOSED pattern) — the forge makes the SLOTS and the bookkeeping impossible to
forget, not the prose.

## GAP D — The flag-graph validator (story wiring that can't dangle)

Evidence this is real: the bible's own continuity audit found `PLAYER_TRUSTED_RILL` consumed
by the ending math but GRANTED nowhere. At ~100 flags and 12 worlds that's one escape; at 80
worlds it's a plague.
**The design:** pure EditMode analysis over `ZiptideFlags` + world data + line triggers +
ending math: every flag consumed is granted somewhere; every granted flag is consumed
somewhere (or whitelisted as telemetry); gate-chains reachable; no cycle traps. `FLAG_ORPHAN`
warn → blocker per the ratchet law. This is cheap, pure-data, and the single best insurance
that "the game builds itself" doesn't quietly build broken story wiring.

## GAP E — The per-world voice formula (lines at scale)

Every SHIPPED world has hand-written RILL enter/gate lines (register-matched to her memory
state) — hand-authoring 80 worlds of these is the same bottleneck as music was before the
prompt formula. Music now has: genome + kinship dial + per-world formula. Voice needs its
equal: **a per-world line kit** — 1 RILL enter line + 1 Cal answer + 1 gate line + 1 optional
FollowUp, written from the world's data row (hazard, biome, mystery object, RILL's state at
that chapter) against the craft rules (RILL_PROFOUND_LINES §1 / CAL_VOICE_AND_BARKS §3).
Sessions draft per-chapter batches; Terry shortlists (the PROPOSED pattern, now with a
throughput plan). The kit slots are validated like lore slots (a world with no enter line =
audit warn).

## GAP F — THE ASSEMBLY LINE RUNBOOK (the capstone — the actual "builds itself" document)

Everything above + everything already planned needs ONE ordered manual: **"World N: from
bible row to device pass."** Draft order (each step names its framework + its gate):
1. Bible row + `WorldContentGenome` (GAP B) + lore slots (GAP C) →
2. WC-1 world recipe (art genome) → WC-3 compile → conformance/perf audits →
3. Line kit (GAP E) + music prompt via the world formula + SFX coverage check →
4. States/acts/marks declared (LS-1/5/6) + awe slots + ride-scene (GAP A, optional) →
5. Flag-graph validation (GAP D) sweep →
6. WC-7 review farm → Terry headset queue → verdict → lock.
One session should walk one standard world through steps 1–5 in one sitting; that sentence is
the whole program's definition of success, and writing THIS runbook (once the frameworks
exist) is the moment the game genuinely starts building itself.

## §7 — Known and deliberately NOT re-opened here

- **VO casting/recording** — known deferred (`VOICE_PIPELINE.md`); captions carry lines until
  then. The line kits (GAP E) are VO-ready scripts by construction.
- **Game-mode variants** (PvP/conquest/etc.) — Terry named them as needing work; separate
  pass, out of scope here.
- **The Earth endgame art kit** — flagged in the bible (§8b, "design now, art later"); it's a
  scheduled special, not a framework gap.
- **Multiplayer** — paused lane, unchanged.

## §8 — Priority read

D (flag validator) is the cheapest and protects everything → B (content genome) unblocks
scale → A (ride-scenes) + E (voice formula) make scaled worlds FEEL authored → C (lore forge)
fills the second story → F (the runbook) is written LAST, from experience, when the first
compiled world walks the full line. All behind the same gate ladder; B/C/E schemas and D's
rule list are paper-draftable during the freeze.
