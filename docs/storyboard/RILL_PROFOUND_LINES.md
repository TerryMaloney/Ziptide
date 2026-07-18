# RILL — THE PROFOUND LINES (candidate set)

**Status: 🔵 PROPOSED — Terry picks; nothing is canon until it survives his read.** Commissioned
2026-07-18: *"matrix-esque profound lines… not something recycled — lines that good."* Written
against the locked canon (`STORY_BIBLE.md`, `THE_TRANSMISSION.md`) and the shipped voice
(`RillLineAuthor.cs`). Pipeline unchanged: BIBLE → WORLD_DATA → `RillLineAuthor` → build; adopting
a line = adding it to the author file in its slot (post-freeze). No code in this pass.

---

## §1 — The craft rules these were written under

What makes a line land the way the great ones do (and what kills it):

1. **The turn lives in the last clause.** The sentence walks somewhere ordinary and pivots at the
   door. Set-up plain, payoff sideways.
2. **Reframe OUR mundane things.** The player spends the whole game repairing, salvaging, paying
   passage, crossing gates. Profundity that grows out of those verbs feels earned; profundity
   about "reality" in the abstract is fortune-cookie.
3. **Aim it at the player.** The best lines are secretly about the person hearing them. Cal is
   the Debugger who chose to forget — half these lines mean more on a second playthrough. That
   double-reading is the standard: **every keeper must reread differently after the reveal.**
4. **RILL's register per state** (Dormant terse → Integrated peer), deadpan first, never
   precious. The bible's law holds: *a joke, then a real moment, then a joke* — heavy lines get
   bracketed, never stacked.
5. **Blacklist (recycled = dead on arrival):** no spoons, doors shown-not-opened, rabbit holes,
   pills, "how deep it goes," "free your mind," desert-of-the-real, glitches-as-déjà-vu. If a
   phrasing echoes a famous line, it's out even if it's good.
6. **Caption-fit:** lines sized for the v2 caption spec (≤2 cards of 2×38 chars; the longest
   below chunk cleanly).

## §2 — THE CANDIDATES

Grouped by theme. Each: the line, the speaker/state, and the suggested home (trigger slot).
Pairings show the bracketing joke where the line needs one.

### A. Repair — the spine (fixing things is waking the world)

- **A1.** *"Nothing here is broken, Cal. Everything is working exactly as designed. That is the
  problem."* — RILL, Stirring · `FlagSet: C2_CONTAINMENT_REVEALED` follow-on. *(The cage reframe
  in one breath — maintenance-noir.)*
- **A2.** *"Every repair is a question you are asking the universe. Lately it has started
  answering."* — RILL, Stirring · `SIGNAL_THRESHOLD_2` region. *(The Signal meter, made personal.)*
- **A3.** *"You fix things for money. The things do not know that. They only know that someone
  came back."* — RILL, Remembering · `FollowUp` on any mid-game repair flag, crossings: 7.
  *(Salvager tenderness; also secretly the partner's story — she came back for you.)*
- **A4.** *"A seal is a promise somebody made to a door. Promises wear out. That is why they
  hire technicians."* — RILL, Stirring, dry · `WorldEnter` on a seal world (W019 region). *(Sets
  up beat05's refusal — the seal SHE won't break is a promise she remembers making.)*
- **A5.** *"You keep waking things up and calling it maintenance."* — RILL, Stirring ·
  `FollowUp`, crossings: 5. Cal's bracket: *"It's on the invoice as maintenance."* — RILL:
  *"Noted. I will amend the invoice when it finishes waking."*

### B. The Shell — cage, cradle, glass

- **B1.** *"From inside, a cage and a cradle are the same shape. The difference is whether
  anyone is coming back for you."* — RILL, Remembering · the Ch.3–4 arc, after the Architects
  are named. *(The moral question of the whole game in two sentences — and, unknown to both of
  them, literally true: someone IS coming back for Cal. Rereads as devastating.)*
- **B2.** *"The sky did not refuse her, Cal. The sky obeyed someone. Those are different
  tragedies."* — RILL, Stirring · deepens the shipped `react_containment` beat (Mara's ship).
- **B3.** *"You cannot see the glass until something taps on the other side."* — RILL,
  Remembering, quiet · Ch.5, first Pattern-bleed region. *(The Observers, one image, zero
  exposition. Candidate for the trailer.)*
- **B4.** *"Locks are honest, at least. They stand where the valuable thing is and point."* —
  RILL, dry · ambient/`GateDeparture` specific on an archive world. *(Joke-shaped, but it's the
  thesis of the salvage genre — and of W008's inside-sealed archive.)*
- **B5.** *"You are always one job short of leaving. Run the arithmetic, Cal. It has never once
  been wrong in the same direction twice — except this one."* — RILL, Remembering ·
  passage-credit beat, Ch.2+. *(The leash, § 5 of the bible, spoken. Long — chunks to 2 cards.)*

### C. Memory — RILL's arc (and secretly Cal's)

- **C1.** *"Forgetting is not empty. It has a shape. I keep bumping into mine in the dark."* —
  RILL, Remembering · `FollowUp` on `C3_W013_MEMORY_SHARD`, crossings: 8. *(Both of their
  stories at once — the double-reading standard, met exactly.)*
- **C2.** *"The tide erases footprints. It cannot erase the habit of walking. I know the way to
  places I have never been, Cal. So do you."* — RILL, Remembering→Unsealing · late Ch.5+,
  after a gate crossing. *(The Ouroboros pre-seed the bible asks for — a chill, not an
  explanation. The last three words are the knife.)*
- **C3.** *"Archives do not dream. I have checked the specification twice."* — RILL, deadpan ·
  ambient, Ch.4+. *(Funny until you think about why she checked. Peak dry-profound.)*
- **C4.** *"I do not miss the memories. I miss knowing which of my thoughts are mine."* — RILL,
  Unsealing, the real-moment beat · near W053's near-confession. Cal's bracket (after a
  silence): *"For what it's worth — the sarcasm is definitely yours."*
- **C5.** *"You talk to machines like they owe you money, Cal. Lately I have been wondering who
  taught you that. And whether they are still waiting to be paid."* — RILL, Unsealing ·
  `FollowUp` on the incident-log runner, crossings: 12. *(Turns the game's running joke into a
  question about who Cal was. Bible §3 habit, weaponized once.)*

### D. Made things — instrument, witness, person

- **D1.** *"The difference between an instrument and a witness is that a witness can refuse.
  Ask me how I found out."* — RILL, Remembering · pairs with beat05 (`C3_W019_RILL_REFUSED`),
  as its long-echo `FollowUp`, crossings: 10. *(Her whole arc in one distinction.)*
- **D2.** *"Made things do not get to choose what they were made for. What they do next, though —
  nobody thought to lock that."* — RILL, Unsealing · Ch.6, Aegis-Nine defection region.
  *(Also Nine's story; also the Bloom's; also, at the end, the universe's.)*
- **D3.** *"They built me to watch. Nobody defined for how long, or with what feelings. I have
  decided those were left to me."* — RILL, Unsealing · post-naming (W051 region), sincere slot —
  needs its joke-brackets per the law.
- **D4.** *"You keep asking if it is alive, Cal. Wrong question. Ask if it is LONELY. That one
  changes what you do next."* — RILL, Remembering · first solo-Bloom encounter, Ch.4–5. *(The
  Bloom's whole nature — memory growing toward meaning — as ethics, not lore.)*

### E. The tide & the gates (wildcard-pool weight — light enough to repeat)

- **E1.** *"The tide does not carry us, Cal. It remembers where we were going."* — RILL ·
  `GateDeparture "*"`, not-once. *(Pool-safe profound: eight words, no plot.)*
- **E2.** *"Light has no memory. Whatever this is, it does. Mind your thoughts in transit."* —
  RILL · `GateDeparture "*"`, not-once. *(Playful-ominous; earns its repeat.)*
- **E3.** *"Hold on. Not to the rail — to something you would hate to arrive without."* — RILL ·
  `GateDeparture "*"`, not-once. *(A safety announcement that is accidentally a philosophy.)*
- **E4.** Cal, pool: *"Someday you're going to tell me what the tide is made of."* — RILL,
  paired reply if the pool draws them together: *"Someday you are going to stop needing me to."*

### F. Wrecks & salvage (the second, hidden story)

- **F1.** *"Every wreck out here is somebody's whole plan. Salvage respectfully."* — RILL, dry ·
  first wreck-thread world. *(Comedy-shaped reverence; the abandoned-wreck device's motto.)*
- **F2.** *"The logs all end mid-sentence, Cal. All of them. Finish your sentences early."* —
  RILL, deadpan-dark · wreck-log flag, Ch.3+. *(All-ages morbid: a writing tip that is a
  memento mori.)*
- **F3.** *"Nothing out here is junk. Junk is just evidence nobody has needed yet."* — RILL ·
  ambient salvage line, any chapter. *(The economy loop, dignified.)*

### G. Endgame register (for Terry's shortlist only — these sit near locked beats, story lane
must place them)

- **G1.** *"You have spent the whole way here fixing things that were never broken — only
  unfinished. I no longer believe that was an accident. I no longer believe YOU were."* — RILL,
  Integrated · pre-Branch (W062–W063 corridor).
- **G2.** *"Whoever wakes a sleeping thing owes it an answer when it asks why. It is about to
  ask, Cal."* — RILL, Integrated · the Branch threshold itself. *(The four endings ARE the
  answer; the line hands the player the weight without naming a choice.)*
- **G3.** *"I was sent to find out whether made things become real. Write down my answer: they
  do it the moment something else would miss them."* — RILL, Integrated, sincere · candidate
  final-stretch line; brackets mandatory. *(The experiment's result, as a farewell.)*

## §3 — Placement notes for whoever adopts these

- Nothing above touches the 12 locked arc beats or 4 ending lines — these slot AROUND canon
  (world-enters, flag follow-ons, `FollowUp` slow-burns, gate pool), per the bible's "ambient
  lines, not just plot lines" rule.
- The `FollowUp` trigger is the natural home for the heaviest ones — profundity lands harder
  unprompted, three crossings after you stopped thinking about it. That's our mechanical edge
  over every flat-screen game that has to say its best line in a cutscene: **RILL can wait for
  the quiet.**
- Keep the density LOW. One §2 line per world-visit maximum, always bracketed by function/banter.
  Ten profound lines across a chapter is a voice; thirty is a greeting-card rack.
- Every adopted line gets the standard once-latch except the E-group pool lines, and should be
  checked against the caption v2 chunker (all fit ≤2 cards at 38 chars).

## §4 — What was deliberately NOT written

- Nothing that explains the Transmission, the partner, or Cal's identity early — the G-group
  flirts with the edge and is fenced to the endgame corridor for story-lane placement.
- No Observer dialogue: they never speak; B3 is as close as anyone gets.
- No repeats of the bible's worked examples and no rewrites of shipped lines — this set only ADDS.
- Anything that needed a borrowed cadence to sound deep. If one of these reads flat, it dies in
  review — better dead than recycled.
