# 🎓 ONBOARDING & TUTORIAL — taught by moments, never by text (designed 2026-07-10, Fable endgame #2)

**The bar (map §8 + store requirement):** a cold player — a store reviewer — learns every core verb
inside W000→W001 without reading a manual, and a veteran never feels nagged. RILL is the tutorial;
the world is the classroom. No floating tutorial UI, ever.

## Laws
1. **One verb per moment.** A moment = a situation that makes the verb the obvious thing to try,
   plus ONE RILL line if the player hesitates. Never two prompts live at once.
2. **Diegetic only.** RILL lines + world labels (the how-to boards idiom). No modal popups.
3. **Hesitation-triggered, not time-triggered.** The line fires only if the verb hasn't happened
   N seconds after its moment opened. Players who just DO it never hear teaching.
4. **Every beat sets a flag** (`TUT_<VERB>`); a set flag never re-teaches. `TUTORIAL_DONE` when the
   W001 arc closes; veterans (existing profiles with playtime) get it pre-set.
5. **Comfort before motion.** The FIRST interactive moment after waking is the comfort console
   (seated/standing, vignette, snap/smooth — the 0.4 presets, diegetically a ship systems panel).
   Store comfort-rating honesty depends on this ordering.

## The teaching river (verbs in dependency order)
**W000_DriftIn (the ship — safe, enclosed, yours):**
LOOK (RILL greets when you meet its light) → COMFORT CONSOLE (law 5) → MOVE (stick; the quarters
are 3 steps deep — nothing to break) → GRAB (one named object on the bunk; RILL comments) →
HOLSTER (first-release hint already built) → INTERACT/tap (the helm map tile) → **PUNCH IT**
(cast-off, already the W000 climax) → THE ZIPTIDE travel moment (RILL rides with you).

**W001 ToxicCity (the world — guided freedom):**
TRAVEL DOOR (return path shown first — safety) → FIRST JOB (existing beats; the job board teaches
objectives) → WEAPON pickup → SHOOT (practice target by the spawn kiosk) → FIRST CREATURE
(non-lethal law: RILL: disable, salvage, nothing dies here) → ZIPLINE (the city one, on the job
path) → SAVE REASSURANCE (first travel back: "The ship remembers everything. So do I.").
Deferred to their own first-encounter moments wherever they happen: garden, belt pad, war table,
augments, climb/grapple (each already logs a first-use diag — those hooks become beats).

## Implementation shape (OPUS-READY — no new architecture)
`TutorialBeatDefinition` data rows: { id, verb, openCondition (scene+flag/state), hesitationSeconds,
rillLineId, completionSignal (existing diag-tag hooks / flags), setsFlag }. A single
`TutorialDirector` (auto-ensured, the DevMenu idiom) evaluates the open beat, fires the RILL line
via the existing line system on hesitation, listens for the completion signal. All lines authored
in `RillLineAuthor` (create-only, per-id). **Gate (add with the build):** a coverage test — every
verb in the core list above has exactly one beat, every beat's rillLineId exists, and the beat
chain from LOOK to TUTORIAL_DONE is connected (no orphan beats).

## What NOT to do
No forced walk-throughs, no input lockouts (a locked stick in VR = instant discomfort), no arrows
floating in space, no "press X to continue." If a beat needs a lockout to work, the moment is
wrong — redesign the moment.
