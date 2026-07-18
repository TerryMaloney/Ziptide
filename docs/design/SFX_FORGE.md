# THE SFX FORGE — sound effects as a system, not a pile of clips
### The missing half of VR presence: every verb, every material, every machine answers in sound

**Status:** 🔵 RESEARCH + PLANNED — no code authorized (recovery freeze). From
`GAP_AUDIT_JULY2026.md` GAP 2, Terry-approved 2026-07-18 ("hit all of them").
**Home:** executes beside FORGE IV CP-6 (surface response) and CP-8 (audio identity); the
response half is joined at the hip to `docs/project_art_plan/DAMAGE_RESPONSE_AND_RUIN.md` —
**the SFX library and the damage matrix share ONE material taxonomy.**
**Owner lanes:** audio + art; weapon/creature hook points are cross-lane claims.

---

## §1 — What exists today (audited)

- `AudioDirector` (locked `_Boot` singleton) owns MUSIC via per-world `AudioProfile` (clip,
  volume, crossfade). `AmbienceDirector` owns beds. **Neither owns one-shot SFX.**
- SFX today is ad-hoc: weapons and effects call `PlayClipAtPoint`/`PlayOneShot` with single
  clips wired on definitions (taser, pistol ×2, gravity gun, gate effect, scanner, RILL VO).
  No variation, no material response, no mixing rules, no budget. A taser sounds identical on
  chitin, steel, stone, and air.
- The damage program (rb44) requires audio per response-matrix cell; weapon-feel and
  enemy docs assume punchy audio; the caption/VO pipeline (`VOICE_PIPELINE.md`) is separate and
  untouched by this plan.

## §2 — Research findings

- **Variation is the anti-fatigue law:** 5–10 recorded variants per repeated sound, ±10% pitch,
  ±3 dB volume per play — the standard that makes 6 clips feel like 40. Layering (mechanical +
  report + tail) is how single events get weight.
- **Material-based response is standard practice** in every AAA engine pipeline (physical
  material → sound set); our damage matrix gives us the taxonomy for free.
- **Mobile/VR constraints:** aggressive compression (Vorbis for long, ADPCM/PCM for short
  frequent one-shots), spatializer for positional sounds, **voice limit ~32 real voices** with
  priority so dialogue is never culled and distant impacts die first.
- **VR-specific:** near-field sounds (hands, held tools) carry presence disproportionately —
  the click of YOUR tool matters more than the world's thunder.

## §3 — The design: one vocabulary, three consumers

**`SfxDefinition`** (data, `VfxRecipeDefinition` pattern): id, 2–6 clip variants, pitch-jitter
range (clamped ±12%), volume-jitter (±3 dB), priority class (`dialogue` > `player_action` >
`impact_near` > `impact_far` > `ambience_detail`), spatial mode (2D UI / positional /
positional-near), cooldown-per-id (anti-machine-gun), max simultaneous of this id.
**`SfxLibrary`** (closed, code-authored like `VfxLibrary`) — the starter vocabulary:

- **Impacts, by the damage matrix's material classes:** `impact_<mat>_soft/hard` for metal ·
  stone · glass · polymer · organic/chitin · corroded · rock · growth. One family, both
  intensity tiers — the matrix's T0–T4 map to soft/hard + layered extras (T4 adds `debris_<mat>`
  scatter tails).
- **Weapon verbs:** fire/charge/dry/holster per verb family (kinetic, electric, sonic, beam,
  snare, grav) — the verb's environmental signature (rb44) is HALF audio.
- **The work suite:** `servo_strain`, `bolt_seat`, `machine_wake` (the C6 "that's the sound"
  moment!), `repair_tick`, `build_place`, `conveyor_run` — the factory/repair loops' feel.
- **World responses:** `steam_release`, `spark_arc`, `water_disturb`, `flora_brush`,
  `debris_settle` — wired to ReactiveProp kinds and the physics dials (wind gusts get a voice).
- **UI/diegetic:** `ui_confirm/deny/tick`, `ledger_stamp`, `holster_snap` — one family so the
  whole interface sounds related (CP-9 inherits).
- **RILL/Cal proximity:** handled by VO pipeline — explicitly OUT of this library except the
  caption-arrival tick.

**`SfxPlayer`** (runtime, pooled): a small AudioSource pool (≤12 one-shot sources), draws
variant + jitter, enforces priority/voice budget and per-id cooldowns, positional via the
project spatializer. ONE entry point — `SfxPlayer.Play(id, position)` — replaces every ad-hoc
`PlayClipAtPoint` over time (create-only migration, one call site per commit).

## §4 — Sourcing the sounds (the honest question for a no-budget pipeline)

Order of preference, mirroring the Forge's own philosophy: ① **synthesized in-project**
(procedural clicks/hums/noise-shaped impacts baked to clips by an editor tool — the ForgeTexture
of audio; enough for UI, servos, electric, steam); ② **CC0/openly-licensed packs** (freesound-
class, curated into our families, recorded in `CREDITS.md` — the licensing ledger the gap audit
demanded); ③ **recorded foley later** (a phone + a junk drawer covers half a salvage game's
palette — genuinely). Every imported clip: mono for positional, correct compression class,
loudness-normalized to family targets (one `SfxImportAuditor` rule).

## §5 — Envelopes

| Env | What | Acceptance |
|---|---|---|
| **SFX-1** | `SfxDefinition`/`SfxLibrary` + `Validate()` (jitter clamps, variant counts, priority enum) + completeness test vs. damage-matrix cells | pure tests; `SFX_DEAD_CELL` audit warn |
| **SFX-2** | `SfxPlayer` pool + priority/voice budget + per-id cooldown + logs `ZIPTIDE: SFX id=… culled=…` | headless tests on pool/priority logic |
| **SFX-3** | first content batch: impacts × materials + weapon verbs, migrated call sites (taser first — the proven-loop weapon) | device A/B: same fight, old vs new audio |
| **SFX-4** | work suite + world responses + UI family; ReactiveProp/practical wiring | in-headset verdicts; the C6 machine-wake moment reads |
| **SFX-5** | mix pass: ducking rules under `AudioDirector` (VO ducks music ducks SFX beds), loudness normalization audit | device verdict at clamp volumes |

**Proof:** the damage program's test alley gets an **audio pass protocol** — Terry (or a session
via captured video WITH audio from the PlayMode capture rig, feasibility to confirm) walks the
alley firing every verb at every material column; a silent cell is a red. CI keeps the
completeness/validation tests; final sound QUALITY is a headset verdict, always.

## §6 — Do-nots

- No audio middleware (FMOD/Wwise) — same single-source-of-truth logic as every rejected
  parallel toolchain; Unity's mixer + our rails suffice at this scope. (✅ Terry confirmed the
  rejection 2026-07-18.)
- No per-object bespoke AudioSources for one-shots (the pool is the path); no stereo positional
  clips; no unlicensed sounds, ever — `CREDITS.md` or it doesn't import.
- No new singletons — `SfxPlayer` lives under `AudioDirector`'s ownership umbrella.
- Dialogue priority is never culled; the voice budget cuts distant impacts first.
- Silence stays authored (CP-8's law): the SFX Forge adds response, not wallpaper.
