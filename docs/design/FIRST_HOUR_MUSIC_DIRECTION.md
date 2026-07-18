# FIRST-HOUR MUSIC DIRECTION — the Suno cue sheet
### The sound of Ziptide, the Tide motif, and nine cues mapped to the Director's Cut

**Status:** 🔵 DIRECTION — Terry generates on Suno (free tier) at his pace; zero code (freeze).
Companions: `ADAPTIVE_AUDIO.md` (the stem architecture this feeds later), `SFX_FORGE.md`
(one-shots — NOT this doc), `TITLE_MENU_EXPERIENCE.md` TM-1 (first consumer),
`FIRST_HOUR_DIRECTORS_CUT.md` (the minute map these cues score).

---

## ⚠️ §0 — THE LICENSE LAW (updated 2026-07-18 after the commercial-options check)

- **Suno free tier is NON-COMMERCIAL,** and rights attach at CREATION time (subscribing later
  does NOT retroactively license free-tier tracks). Free-tier output never ships. Terry
  dropped the free-tier plan accordingly.
- **The commercial answer (checked 2026-07-18): Suno Pro, $10/month ($8 annual),** grants
  commercial rights to tracks generated WHILE subscribed — explicitly including games sold on
  stores — with 2,500 credits/month (≈500 generations: the entire first hour many times over,
  realistically the whole game's exploration phase in 1–2 months). Premier ($30) adds volume +
  studio tools, NOT better rights — Pro is enough. **Recommendation: skip the free tier
  entirely; run ONE Pro month and generate everything in it.** Re-verify the live terms on
  subscribe day (terms drift).
- **Alternatives, ranked, if Suno disappoints:** AIVA Pro (cinematic/game scoring specialist,
  full copyright on Pro — strongest fit for orchestral awe cues); Stable Audio (licensed
  training dataset — cleanest provenance story if that ever matters at store review); Udio
  (downloads currently disabled while it rebuilds as a licensed platform — OUT for now);
  curated CC0/paid music packs (OpenGameArt, itch.io, Unity Asset Store perpetual game
  licenses) as the zero-AI fallback.
- **Copyright nuance, so it never surprises us:** pure-AI output currently lacks copyright
  protection in the US — the paid license lets US ship it, but we can't stop others from using
  identical output. Fine for a game score; just don't build the brand's ONE signature motif on
  something unprotectable without knowing that — if the Tide motif becomes precious, a human
  re-records/arranges it later (a derivative human work IS protectable) — future option, not
  a blocker.
- **The remix question (ruled 2026-07-18):** Terry generated a strong free-tier candidate for
  the title theme (Prospect × Halo × BR2049 — on-target for the tidal-ambient brief). Suno's
  help docs: rights attach at the ORIGINAL song's creation time, no retroactive licensing "by
  default," and Pro-era Cover/Extend/Remaster of a free-tier song is NOT clearly granted
  commercial rights — treat the remix path as unsafe. Safe paths, in order: ① ask Suno support
  in writing for retroactive rights on that specific track (the "by default" wording implies
  exceptions exist; a written yes + track URL = bulletproof); ② subscribe to Pro FIRST, then
  regenerate with the same prompt until a take lands beside it — the Pro keeper becomes
  canonical, the free track stays as the A/B reference (legal as reference). Terry: save the
  exact prompt + track link NOW as keeper-log entry #1.
- Every shipped track gets a `CREDITS.md` line: tool, plan at generation, date, prompt on file
  in `music/PROMPTS_LOG.md`.

## §1 — What Ziptide should SOUND like (the direction)

**"Tidal ambient"** — an organic-electronic hybrid where water and salvage metal are the
instruments:

- **Foundation:** deep, slow synth pads (the future `stemA_SubDrone` — 30–60 Hz anchor lives
  here); unhurried, never tense by default.
- **Texture:** processed water — drips, moving tide, hull resonance; bowed/struck metal ("the
  ship is an instrument"); granular shimmer (future `stemB_Spores`).
- **Wonder voice:** sparse celesta/music-box/soft piano for the motif; **distant wordless
  choir** reserved for ancient/awe moments only (future `stemC_Ancient` — the canon says
  "Halo's awe," and that's the reference: sacred, far away, earned).
- **Rhythm:** restrained; work-worlds get a low tinker-groove; full percussion exists ONLY in
  combat (future `stemD_CombatGroove`).
- **Mood law (tone charter):** wonder first, melancholy underneath, nothing harsh or horror-
  coded — a 6-year-old hears "mysterious ocean space," never "scary."
- **Family coherence cheats:** generate everything at **~76 BPM** and in/around **D minor**
  (say both in every prompt). Shared tempo+key makes separately-generated cues feel related —
  and phase-aligned stems need one BPM anyway (`ADAPTIVE_AUDIO.md`).

**THE TIDE MOTIF:** one short hummable phrase (5–7 notes) that becomes the game's signature —
title theme, gate crossings, return-home, and (someday) the name moment. Suno can't be told
exact notes, so: **generate the title theme FIRST, in bulk, and pick the take whose hook you'd
hum in the shower.** That take DEFINES the motif; describe it in later prompts ("reprise the
gentle rising 6-note theme") and accept approximation — where exact reprise matters
(return-home), we can also just re-edit the title take's intro bars in a wave editor.

## §2 — THE CUE SHEET (nine cues, mapped to the Director's Cut)

| # | Cue id | Scores | Feel | Length |
|---|---|---|---|---|
| 1 | `title_berth` | title menu (TM-1) | the identity theme; pre-dawn calm → the motif fully stated by bar 8 | 2–3 min, loopable |
| 2 | `w000_wake` | Drift In wake/tutorial | near-ambient; pads + faint textures; curiosity, no melody yet (the motif is EARNED later) | 3–4 min loop |
| 3 | `flight_punchit` | the W000→W001 ship flight | rising, bright, wind-and-engine wonder — NOT action-movie; ends open | 60–90 s |
| 4 | `w001_toxiccity_bed` | Toxic City work loop | low damp tinker-groove; patient; stays under the world's sound | 3–4 min loop |
| 5 | `w001_gate_wake` | relay repair → the gate wakes (~min 45) | the ancient layer arrives: distant choir + motif fragments assembling; awe rising | 90 s build |
| 6 | `ziptide_crossing` | the FIRST Ziptide (the hour's peak) | the motif at full voice over the surge; brief, huge, then gone | 30–45 s stinger |
| 7 | `w002_cistern_bed` | Dry Cistern rebuild/grow | brighter key-lift of the family; hopeful work rhythm | 3–4 min loop |
| 8 | `w002_defend` | the one 90-second wave | combat groove variant — SAME 76 BPM as cue 7 (future stem alignment); energetic, kid-safe, zero dread | 90 s |
| 9 | `return_changed_ship` | the changed-ship ending beat | the motif, warm and low — home rhymes with the title | 60–90 s |

Free-tier arithmetic: ~10 generations/day × 2 takes each — one cue per day with 4–10 candidate
takes is a comfortable curation pace; the title theme deserves 2–3 days of takes by itself.

## §3 — Paste-ready prompts (tune freely; keep BPM/key/instrumental in every one)

1. **title_berth:** *"Instrumental. Ambient electronic title theme, 76 bpm, D minor. Pre-dawn
   ocean calm: deep slow synth pads, soft water textures, gentle struck-metal chimes. A simple
   memorable 6-note melody enters on celesta and swells with quiet awe. Melancholy but hopeful.
   Cinematic, spacious, seamless loop, no drums."*
2. **w000_wake:** *"Instrumental ambient, 76 bpm, D minor. Weightless and curious: warm sub
   drone, faint granular shimmer, occasional soft hull resonance like a sleeping ship. Almost
   no melody. Peaceful mystery, not tension. Seamless loop."*
3. **flight_punchit:** *"Instrumental, 76 bpm, D minor rising to F major. Soaring first-flight
   cue: airy arpeggios building, wind-like pads, bright metallic percussion entering late,
   sense of leaving harbor for open sky. Wonder, momentum, no aggression. Ends unresolved."*
4. **w001_toxiccity_bed:** *"Instrumental, 76 bpm, D minor. Low-key salvage-work groove: muted
   bass pulse, damp dripping-water percussion textures, distant foghorn pads, patient and
   unhurried. Background music that stays out of the way. Seamless loop, no lead melody."*
5. **w001_gate_wake:** *"Instrumental cinematic build, 76 bpm, D minor. Something ancient
   waking: distant wordless choir far away in the mix, deep pulses like a huge machine
   breathing, fragments of a gentle 6-note theme assembling, rising awe without fear."*
6. **ziptide_crossing:** *"Instrumental cinematic stinger, 76 bpm, D minor blooming to D major.
   A tidal wave of light: full swell, choir and bright synths, a simple 6-note theme stated
   triumphantly once, then sudden calm afterglow. 40 seconds. Awe, not battle."*
7. **w002_cistern_bed:** *"Instrumental, 76 bpm, F major. Hopeful rebuilding: soft mallet
   patterns like dripping water in a huge stone cistern, warm pads, light workshop rhythm,
   optimism after rain. Seamless loop, understated."*
8. **w002_defend:** *"Instrumental, 76 bpm, D minor. Energetic defense groove: driving but
   friendly breakbeat, rubbery bass, metallic hits, playful urgency — arcade energy, zero
   horror, kid-safe. 90 seconds with clean start and end."*
9. **return_changed_ship:** *"Instrumental, 76 bpm, D major. Coming home: the gentle 6-note
   theme played slow and warm on celesta over low pads and soft tide sounds. Tender, earned
   rest. Short piece, fades to quiet."*

## §4 — Workflow + wiring notes

- **Curate hard:** generate ≥4 takes per cue, keep 1. The keeper test: cue 1 you can hum;
  cues 2/4/7 you DON'T notice while doing a task (that's their job); cues 5/6 raise arm hair.
- **Name files for the pipeline:** `firsthour_<cueid>_vX.wav` now; the AdaptiveAudio importer
  convention (`WorldXX_A..D`) applies LATER when stems exist — don't force it yet. Keep every
  keeper's prompt+date in a `music/PROMPTS_LOG.md` beside the files (feeds CREDITS.md).
- **Loops:** Suno loops imperfectly — trim loop points in a wave editor (Audacity, free);
  beds want ≥3 min before looping so repetition hides under gameplay.
- **Wiring today needs NO new code:** cues 2/4/7 drop into the existing per-world
  `AudioProfile` slots (clip/volume/crossfade already built); cue 1 = TM-1's boot profile
  slot; cues 3/5/6/8/9 are event-triggered pieces that wait for the small stinger hooks
  (post-freeze, rides the SFX/TM envelopes). Stems + ThreatLevel mixing = the AdaptiveAudio
  build, later; these single-file cues are forward-compatible as its A-stems.
- **Volume law:** beds sit LOW (the existing 0.35 default is right); the world's sound stays
  the lead vocalist (CP-8's silence-is-authored law) — cues 5/6 are the sanctioned exceptions.
