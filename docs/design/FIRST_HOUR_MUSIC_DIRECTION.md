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

## §1a — THE CANONICAL PROMPT (Terry's, 2026-07-18 — this supersedes the generic direction below)

Terry found the sound on his first free-tier session. **This prompt is the score's genome;
every other prompt in this doc derives from it:**

> *"Atmospheric sci-fi space-western score. Ethereal, wordless youthful choir chants, haunting
> airy vocals drifting over deep orchestral string drones. Ominous industrial synths, low
> pulsing sub-bass, and metallic tubular textures mimicking alien forest ambiance. Sparse,
> heavy cinematic percussion hits. Moody, beautiful yet dangerous, suffocating wilderness
> tension, retro-futuristic, 'used future' aesthetic, 70s sci-fi cinema score. quiet tide
> sounds."*

Terry's verdict on the result: "a perfect mixture of Prospect and Halo and Blade Runner 2049" —
which is the tidal-ambient brief hit dead center (Prospect is the skyscape bible's canonical
reference). Plan: regenerate under Pro with this prompt until a take lands beside the free-tier
reference (license §0); **when the Pro keeper lands, detect ITS tempo and key and use those in
every derived prompt** (replacing the generic 76 bpm / D minor defaults below — the keeper is
the authority, not the guess).

**The DNA words (the kinship thread — reused verbatim in derived prompts):** wordless youthful
choir · deep orchestral string drones · industrial synths · pulsing sub-bass · metallic tubular
textures · used-future / 70s sci-fi cinema · quiet tide sounds.
**The toning-down operators (how a derivation gets quieter):** demote "sparse heavy cinematic
percussion hits" → "almost no percussion"; add "distant / faint / half-remembered"; swap
"score" → "background score, understated"; replace "suffocating tension" with the world's own
adjective; keep the tide sounds ALWAYS (they are the game's name in audio).

## §1a-B — THE SECOND CANONICAL PROMPT: THE SOAR STRAND (Terry's, 2026-07-18)

Terry's second prompt — his description: a fusion of the **Oblivion** (M83) and **Prospect**
soundtracks. Recorded verbatim as **Genome B**:

> *"Epic cinematic sci-fi fusion score. Bright, crystallized synth arpeggios and soaring
> M83-style electronic swells blending with an ominous, low-pulsing sub-bass. Ethereal,
> wordless youthful choir chants and haunting, airy vocals floating over wide, dramatic
> orchestral strings. Sweeping French horn crescendos contrast with metallic, tubular textures
> and buzzing alien forest ambiance. A transition from intimate, suffocating wilderness tension
> to vast, lonely, post-apocalyptic grandeur. Pounding cinematic percussion hits and deep
> electronic drums. Beautiful, melancholic, soaring, and dangerous."*

**How A and B relate (this is the score's whole architecture now):**
- **Genome A** (§1a) = the INTIMATE strand — ground-level, moody, the wilderness pressing in.
- **Genome B** = the SOAR strand — airborne, vast, the world seen from above.
- **Shared DNA (deliberate, in both prompts):** wordless youthful choir · pulsing sub-bass ·
  metallic tubular textures · wilderness tension · beautiful-and-dangerous. The shared thread
  means A and B takes will sound like ONE score in two moods — the kinship dial applies to
  BOTH strands.
- **Note "M83-style" may trip Suno's artist-name filter** — if a generation refuses or drifts,
  substitute "soaring anthemic electronic swells, shimmering analog synth walls."

**Assignment map (which strand scores what):**
| Strand | Scores |
|---|---|
| **A — intimate** | title/menu (the berth is quiet) · world beds · gate-wake build · tension/interior moments |
| **B — soar** | `flight_punchit` (B-derived, not A) · **`ziptide_crossing`** (the A theme stated in B's clothes — the intimate melody goes airborne at the peak: that IS the first-crossing feeling) · space/vista worlds and the ship-flight layer · late-game "vast lonely grandeur" (scarred states, Earth-approach corridor) · the trailer |
| **A×B blend** | chapter capstones; the endgame — the two strands finally play at once (and if the partner's voice ever gets a musical identity, it's strand B to Cal's A) |

## §1a-C — THE SEPARATION CLAUSE (Terry's, 2026-07-18 — a mix technique, not a third theme)

Terry's cleaned-up prompt solving the "music runs into itself" problem via explicit
foreground/midground/background staging. Verbatim:

> *"Cinematic sci-fi score with clean instrument separation. Foreground features a soaring,
> melancholic melody led by bright, crystallized synth arpeggios and airy, wordless choral
> chants. Midground is held by a sweeping, cinematic orchestra of clean strings and dramatic
> French horn swells. Background layers provide deep, steady electronic sub-bass and subtle,
> sparse metallic ambient textures. No clutter, minimal elements, spacious and wide
> soundstage. Emotional, beautiful, lonely, and epic."*

**How to use it:** this is an OVERLAY, appendable to any A- or B-strand prompt. The load-
bearing phrases are the staging skeleton + the closer — reuse them verbatim:
`"clean instrument separation… Foreground [the cue's lead] … Midground [the cue's body] …
Background [sub-bass + sparse metallic textures]. No clutter, minimal elements, spacious and
wide soundstage."`
For quiet beds (K1/K2), the same skeleton with humbler occupants: foreground = almost nothing
or the faint choir thread; midground = drones; background unchanged. The EMPTIER the
foreground, the better a bed behaves under gameplay.

**Two happy alignments worth knowing:**
- The staging maps 1:1 onto the future stem architecture (`ADAPTIVE_AUDIO.md`): foreground ≈
  the melodic/ancient stems, midground ≈ orchestral body, background ≈ the sub-drone stem —
  cues generated with this clause will SPLIT better when stems matter.
- It is the music wearing the game's own five-layer depth law (FORGE IV's near/mid/far world
  equation): the score now composes space the same way the vistas do. One aesthetic, every
  sense.

## §1b — THE KINSHIP DIAL (Terry's design, 2026-07-18: the title theme runs through the game at four volumes)

| Tier | Who gets it | Prompt recipe |
|---|---|---|
| **K3 FULL VOICE** | title/menu · first Ziptide crossing · endgame beats | the canonical prompt, verbatim (± length/act tweaks) |
| **K2 RELATIVE** | signature story worlds (Toxic City, capstones) | all DNA words kept + toning-down operators + the world's palette line — "a toned-down version, quieter, not as massive" |
| **K1 TRACE** | most standard worlds | world's own palette FIRST + exactly one thread: *"a faint, distant wordless choir occasionally surfaces, half-remembered"* + quiet tide sounds |
| **K0 OWN VOICE** | the deliberately-weird worlds (The Hum, Mirror Flats class) | fully own palette; keep ONLY "quiet tide sounds" (or nothing, for one or two truly alien places) |

Rarity logic mirrors the physics-dial law: K3 is rare and earned, K2 for story spines, K1 is
the default, K0 is the seasoning. This is how "some of the title track keeps running in your
head" without eighty worlds of the same song.

## §1 — What Ziptide should SOUND like (the direction — now subordinate to §1a's genome)

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

## §3 — Paste-ready prompts, v2 — DERIVED FROM THE CANONICAL PROMPT (supersedes the v1 generics below)

Once the Pro keeper exists, append ITS bpm/key to each. Kinship tier marked per cue.

1. **title_berth (K3):** the canonical prompt, verbatim. Add: *"instrumental, slow build,
   the main theme fully stated by the midpoint, seamless loop."*
2. **w000_wake (K1):** *"Quiet atmospheric sci-fi ambience, used-future 70s cinema feel. Deep
   string drones and soft industrial hum, weightless and curious, like a sleeping ship waking.
   Almost no percussion, no melody yet. A faint, distant wordless choir surfaces once,
   half-remembered. quiet tide sounds. Instrumental, seamless loop."*
3. **flight_punchit (K2):** *"Soaring retro-futuristic sci-fi flight cue. Deep orchestral
   string drones lifting, industrial synths brightening, pulsing sub-bass momentum, metallic
   tubular textures streaming past like wind. The wordless youthful choir rises open and
   hopeful. Sparse cinematic percussion enters late. Leaving harbor for open sky — wonder, not
   war. Ends unresolved. Instrumental."*
4. **w001_toxiccity_bed (K2 — Terry's named example):** *"Toned-down atmospheric sci-fi
   space-western background score, quieter and less massive. Deep string drones low in the
   mix, soft industrial synths, damp dripping-water textures over metallic tubular ambiance,
   slow pulsing sub-bass. The wordless choir is distant and rare, drifting in like fog.
   Almost no percussion. Patient, moody, beautiful yet dangerous, used-future aesthetic.
   quiet tide sounds. Instrumental, seamless loop, stays out of the way."*
5. **w001_gate_wake (K3 build):** *"Atmospheric sci-fi score build — something ancient waking.
   Deep orchestral string drones swelling, industrial synths pulsing like a huge machine
   breathing, metallic tubular textures ringing. The ethereal wordless youthful choir
   assembles the main theme in fragments, closer and closer. Sparse heavy cinematic hits
   marking each awakening stage. Awe rising, not fear. used-future, 70s sci-fi cinema.
   quiet tide sounds growing to a rush. Instrumental, 90 seconds."*
6. **ziptide_crossing (K3):** *"Cinematic sci-fi stinger — a tidal wave of light. The full
   ethereal wordless youthful choir states the main theme once, triumphant, over massive
   string drones, industrial synths and heavy percussion hits, then sudden calm afterglow
   with quiet tide sounds. Beautiful, enormous, brief. 70s sci-fi cinema, used future.
   Instrumental, 40 seconds."*
7. **w002_cistern_bed (K1):** *"Hopeful quiet sci-fi background score with a stone-cistern
   feel: soft mallet patterns like water drips in a vast chamber, warm low drones, gentle
   workshop rhythm, used-future aesthetic. A faint distant wordless choir surfaces
   occasionally, half-remembered. quiet tide sounds. Instrumental, understated, seamless
   loop."*
8. **w002_defend (K1 energetic):** *"Driving but friendly retro-futuristic defense groove:
   pulsing sub-bass, industrial synth rhythm, metallic tubular percussion, playful urgency —
   arcade energy, zero horror, kid-safe. One faint choir swell at the climax. used-future
   aesthetic. Instrumental, 90 seconds, clean start and end."*
9. **return_changed_ship (K3, small):** *"Gentle sci-fi homecoming cue: the main theme played
   slow and warm — soft wordless choir and low string drones, faint metallic textures, almost
   no percussion. Tender, earned rest, used-future warmth. quiet tide sounds. Instrumental,
   short, fades to quiet."*

**K0 examples for later worlds (the own-voice tier):** The Hum: *"Alien resonance study: the
music IS a planet humming — deep tuned drones beating against each other, no choir, no melody,
slow phasing pulses in the rock. Instrumental, hypnotic, seamless loop. quiet tide sounds."* ·
Mirror Flats: *"Vast empty light: glassy shimmering pads, crystalline chimes, near-silence,
enormous space, no choir, no percussion. Instrumental, seamless loop."*

**The world-prompt formula (for the other ~70 worlds, when their time comes):**
`[kinship-tier recipe from §1b] + [world palette line: its hazard/biome as instruments] +
[its adjective] + "quiet tide sounds" (K1+) + "Instrumental, seamless loop" + keeper bpm/key.`

## §3-v1 — original generic prompts (kept for reference; superseded by v2 above)

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
