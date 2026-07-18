# CAL — VOICE & BARKS (the player character finally reacts to her own life)

**Status: 🔵 PROPOSED — Terry picks; nothing is canon until his read.** Commissioned 2026-07-18:
movie-level lines for Cal — entries, outbursts, work talk — *"realistic but also cinematic,"*
never annoying. Written against the locked canon (`STORY_BIBLE.md` §3/§3b, `THE_TRANSMISSION.md`
§4 voice) and the shipped lines (`RillLineAuthor.cs`). Companion doc: `RILL_PROFOUND_LINES.md`.
No code in this pass; §6 sketches the future bark seam as a planning note.

---

## §1 — Where Cal's voice stands today

Cal has ~25 shipped lines — and every single one is an **answer to RILL**. She never reacts to a
fight, a find, her own work, or a room. The canon already gives her the raw material:
- **The habit (bible §3):** she talks to broken machinery like it's being deliberately stubborn —
  *"oh, NOW you work"* — dry, competence-under-pressure, never precious.
- **The voice (`THE_TRANSMISSION.md` §4):** gender-ambiguous blend, warm, tired in the right
  places. Every line below must read correctly in that voice — no gendered swagger.
- **The rhythm law (bible §3b):** a joke, then a real moment, then a joke. Applies to her barks
  exactly as to RILL's lines.

## §2 — How the greats do it (research)

- **Scarcity is the power move.** Master Chief's lines land because there are so few — "I need a
  weapon" is four words carried by an hour of silence around it. The DOOM Slayer says nothing and
  loses nothing. Lesson: **silence is Cal's default state**; a bark is an event, not wallpaper.
- **Reaction beats narration.** Nathan Drake's charm isn't quips into the void — it's that he
  reacts to the *specific* thing that just happened ("that just happened"-ness without the
  catchphrase). Context-specific beats generic every time; a bark that could play anywhere says
  nothing anywhere.
- **One bark, three jobs.** The Far Cry 2 standard (GDC): a great bark can carry what happened,
  what the speaker believes, and how they feel about it, in one breath. Aim every candidate at
  ≥2 of those.
- **The repetition failure mode is a WRITING failure first** — the craft literature is blunt:
  write many variations, ship the best few, and control usage systemically. Barks follow all the
  constraints of dialogue *plus* a variety requirement.
- **The movie register Terry named** (The Mummy, Independence Day): adventure-movie charm =
  **understatement + competence + complaint-as-affection**, with the one BIG line saved for the
  one big moment. Rick O'Connell doesn't announce awe — he complains his way through wonders and
  means the opposite. That's already Cal.

## §3 — Cal's voice charter (test every line against this)

1. **Understatement first.** The bigger the moment, the smaller the line. Awe is played sideways
   or in a beat of silence with three words after it.
2. **She talks TO things, not to camera.** Machines, drones, weather, her own hands — Cal
   addresses the world. She narrates nothing. If a line explains what the player just saw, cut it.
3. **Complaint is her love language.** She gripes at what she's fond of (the job, the ship, RILL,
   the machines). Real alarm is flagged by her going QUIET — the inversion players learn to read.
4. **Competence under pressure.** Even hurt, even losing: procedural, wry, moving. Panic is not
   in her register; urgency is.
5. **No Marvel-quip cadence.** Nothing winks at the audience. Her humor is for herself and RILL,
   not for us.
6. **Exclamations are EARNED** — one per rare, genuinely big event, and even then Cal-sized
   ("HA! — okay. Okay." is her ceiling, not a whoop).
7. All-ages clean; wonder-first tone charter applies (bible §9).

## §4 — The anti-annoyance constitution (numbers, not vibes)

- **Silence default:** most kills, most repairs, most rooms get NO line. Target: the player hears
  a Cal bark **a few times per session, not per minute**.
- **Pools:** every bark event ships ≥6 variants or it doesn't ship; the delivery system draws
  no-repeat over the last 4 (pool machinery exists — the gate pool's random-draw pattern).
- **Cooldowns:** one global Cal-bark cooldown (~60–90 s) on top of per-event rarity; RILL lines
  and Cal barks share a courtesy gap (never stack within ~8 s unless a scripted pair).
- **Notability gates for combat:** Cal does NOT bark per kill. She barks on: first takedown of a
  session · a streak (3+ fast) · a near-miss survived · a save (RILL's warning mattered) · a wave
  cleared · a new enemy type's first defeat. That's it.
- **Awe lines: once per world per save,** on first entry only — they're the movie moments and
  they die on repeat.
- **Priority:** scripted story beats > RILL plot lines > Cal barks. A bark never interrupts; it
  yields and stays silent (not queued — a late bark is worse than none).

## §5 — THE CANDIDATES

### A. First entries — the movie moments (once per world; the biggest lines in the doc)

- **A1** (a vast vista, the skyscape bar): *[two seconds of nothing]* — *"...You could have
  warned me."* — RILL: *"I did not want to spoil it."* *(The awe is in the pause. Our flagship
  entry pair.)*
- **A2** (cathedral-scale ruin): *"Whoever built this wasn't getting paid by the hour."*
- **A3** (hostile, wrong, beautiful): *"Hate it. It's perfect. Let's go."*
- **A4** (toxic canal city, W001-family): *"Smells like money. Old, wet money that somebody
  wants moved."*
- **A5** (a giant dead machine hall): *"Now THAT is a machine. ...Don't tell the ship I said
  that."*
- **A6** (empty world, W006-family): *"No wind. No birds. Nothing. ...If I whistle and something
  whistles back, we leave. Agreed?"* — RILL: *"Agreed."*
- **A7** (a world that scares even RILL): *[quiet]* *"Stay close, okay?"* *(Charter rule 3 — the
  quiet IS the alarm. No joke bracket on this one; rarity makes it land.)*
- **A8** (returning to a world she's changed — LS-6 marks): *"Huh. Place kept the lights on."*

### B. Combat outbursts (notability-gated per §4)

- **B1** (first takedown of a session): *"Still got it. Some of it."*
- **B2** (drone drops): *"Stay down."* · *"Nap time."* · *"And STAY— thank you."*
- **B3** (a genuinely great shot — long/ricochet/mid-air): *"Tell me you saw that."* — RILL:
  *"The log saw it."*
- **B4** (streak): *"Okay. NOW I'm awake."*
- **B5** (near-miss survived): *"HEY— ...okay. Rude."*
- **B6** (RILL's warning saved her): *"...Thanks."* *(Two syllables, sincere, rare — worth more
  than any speech.)*
- **B7** (wave cleared): *"Anyone else? ...No? Wise."*
- **B8** (weapon dry mid-fight): *"Not now, not now, not— "* *[recharge chime]* *"—we're fine."*
- **B9** (big new enemy, first defeat): *"HA! — okay. Okay."* *[breath]* *"What ARE you?"*
  *(Her exclamation ceiling, per charter 6.)*

### C. The work — repair, build, and the sacred habit

- **C1** (machine starts after scolding — the canon family, more variants for the log): *"Oh,
  NOW you work."* · *"See? Was that so hard?"* · *"We had a DEAL, and you know it."* · *"Good
  machine. I take back a third of what I said."*
- **C2** (long repair finally lands): *"There. Forty years dead, two hours of me. Somebody write
  that down."* — RILL: *"Written."*
- **C3** (a build completes): *"Built to last."* *[beat]* *"...At least to Tuesday."*
- **C4** (fails first attempt): *"Fine. We'll do it YOUR way."*
- **C5** (delicate final step, half-whispered): *"Hold... hold... hold— there we go."* *(The
  competence-porn line; players will hold their own breath with it.)*
- **C6** (something she fixed hums to life beautifully): *[listens]* *"...Yeah. That's the
  sound."*

### D. Salvage & discovery

- **D1** (a good find): *"Ohh, hello. You're coming home with me."*
- **D2** (obvious junk RILL dignifies): *"'Evidence.' RILL, it's a bucket."* — RILL: *"It is
  evidence of a bucket."*
- **D3** (a wreck log): *"Another log."* *[beat]* *"Someday somebody finds mine and feels weird
  about it too."* *(Quiet mortality per the wreck-thread; earns its once-latch.)*
- **D4** (mystery object of the world): *"That's not supposed to be here. ...Bag it."*

### E. Damage & recovery (mutters, not screams)

- **E1** (hit): *"Okay. Felt that."* · *"Ow. Noted."*
- **E2** (hit hard, low): *"Still up. ...Mostly up."*
- **E3** (recovered/healed): *"Better. Let's not do that again."*
- **E4** (a fall the net caught): *"...We speak of this to no one."* — RILL: *"The log speaks of
  everything."*

### F. Traversal & idle (rarest class — session-level frequency)

- **F1** (mid-zipline): *"Never gets old."* *[beat]* *"Don't tell RILL."* — RILL, deadpan:
  *"Heard."*
- **F2** (a long look down): *"Long way down. Good thing I'm a professional."*
- **F3** (rain/weather act starts): *"Of course it does."* *(Complaint-as-affection at the whole
  planet.)*

## §6 — The bark seam (future envelope — planning note only)

The current pipeline (`RillTrigger` WorldEnter/FlagSet/GateDeparture/FollowUp) can't hear combat
or work events. The future envelope (post-freeze, cross-lane claim with gameplay owners): a small
**`BarkEvent` trigger family** fed by hooks at the existing log points (`DRONE_DOWN`, repair
complete, wave clear, fall-net catch, salvage pickup) with the §4 constitution enforced IN the
delivery system (pool draw, no-repeat window, global cooldown, notability gates, priority-yield)
— rules as code, so annoyance is structurally impossible, not editorially avoided. Awe entries
(A-group) ride the existing `WorldEnter` trigger today with no new machinery.

## §7 — Do-nots

- No bark on routine actions (per-kill, per-pickup, per-door). The pool sizes are for VARIETY
  within rare events, not license for frequency.
- No catchphrase engineering — if a line is designed to be repeated by players, it will be
  repeated AT the game. Let them find their own favorites.
- No narration of visible things; no tutorializing inside barks (that's RILL's job, sparingly).
- No panic register, no gendered swagger, no fourth-wall winks.
- Nothing in the A-group ships without its headset read: awe timing (the pause length in A1) is
  a device verdict, same as any visual.
