# AUDIO PRODUCTION MASTER PLAN

**Lane:** Reasonbox (Fable 5), per the gpt-first-hour work order, 2026-07-23. **Docs/planning only** — this lane generates no audio, edits no runtime, alters no assets, touches no licensing manifests.
**Companion:** `docs/audio/AUDIO_VERTICAL_SLICE_QUEUE.md` (the implementation-ready asset table).
**Reconciled with:** `EXCELLENCE_MAP.md` (ambient-audio row), `FINISHED_GAME_BENCHMARK.md` (mix-bus/sliders gap), `docs/VRC_TEST_PLAN.md` (Audio VRC category), the comfort/accessibility design (subtitle S/M/L law), `MISS_LEDGER.md` Class Law, and the licensing machinery (`docs/licensing/third_party_manifest.json` + `tools/third_party_license_gate.py` + `docs/CREDITS.md` + `art_intake/` holding path). This plan extends those systems; it invents no parallel ones.

---

## 1. EXISTING vs MISSING — the audited inventory (source-verified 2026-07-23)

### What exists (do not rebuild)
| System | Files | State |
|---|---|---|
| Music playback | `Gameplay/Runtime/Audio/AudioDirector.cs` (_Boot singleton, crossfade), `Content/Runtime/Audio/AudioProfile.cs` (clip/volume 0.35/loop/crossfadeSeconds 2), per-world via `WorldRuntime`/`WorldPackDefinition` | FUNCTIONAL, minimal by design: ONE clip slot per world, no stems, no states. |
| Procedural ambience | `AmbienceDirector.cs` + `AmbienceCore.cs` (pure spec: mix weights, events/min) | FUNCTIONAL + tested (`AmbienceTests`: seamless loops, deterministic synthesis, true silence at zero, biome distinctness). Synthesized wind/hum/rumble + drip/chirp one-shots; **zero assets, 4 AudioSources, ~1 MB generated per world**. This is the architectural gem — asset-free beds mean biome coverage scales by DATA. |
| The one licensed track | `Content/Audio/Clips/Zerogravity_Bloom_Favorite_1.wav`, wired by `ScenePatcherD2.cs:20` as the default world music | The only committed audio asset. Licensing status governed by the existing manifest machinery. |
| Scattered SFX call sites | ~24 scripts reference `AudioSource`/`AudioClip` (shutter click, engine hum seams, UI pops, XRI sample sounds) | Ad-hoc; several synthesize or reuse; no shared trigger vocabulary. |
| Speech (text) | `RillCompanion.cs` subtitle delivery (world-entry + flag-poll + follow-up memory), `RillLineAuthor` authored lines | Subtitle-first is the SHIPPED decision; VO explicitly deferred (roadmap "VO & subtitles" row). |
| Captions/accessibility | Subtitle system with S/M/L sizes (comfort design), readability audit (`UiReadabilityAuditRules`) | Text path exists; no caption path for NON-speech audio. |
| Licensing machinery | manifest + gate tool + CREDITS.md + `art_intake/` holding path | Enforced in CI; audio must flow through it (see §9). |

### What is missing (the production program's scope)
1. **A mix architecture** — no AudioMixer, no buses, no ducking, no player volume sliders (benchmark-flagged, VRC-relevant).
2. **An SFX event layer** — no shared audio-event vocabulary/registry; each script does its own thing.
3. **Stingers/stems/adaptive states** — EXCELLENCE_MAP's named remainder; music is one static loop per world.
4. **The asset catalog itself** — beyond one track and synthesis, every clip in the slice queue is unproduced.
5. **VO pipeline decision artifacts** — deferred, but the DECISION (subtitle-first v1, VO slot-in later) needs to stay recorded here.
6. **Non-speech captions** — accessibility gap for critical audio cues (alarms, creature telegraphs).
7. **Audio acceptance evidence classes** — nothing in the proof ladder listens.

---

## 2. ZIPTIDE SONIC IDENTITY (the taste contract — Terry adjudicates, then it locks)

- **The world is wet, hollow, and resonant.** Tides, brine, metal drums, pipes: reverb spaces are pipes/cisterns/hulls, not cathedrals. Percussive sounds ring metallic; ambiences carry water motion at the noise floor.
- **Non-lethal canon has a sound:** disables are DISCHARGE (crackle, capacitor whine-down, clatter) — never gore, never screams. Creatures power down like machines exhaling.
- **The ship is a body:** its idle is a heartbeat-adjacent low hum (the "hums" already in ship customization data); every ship state change (arming, PUNCH IT, arrival) is first an AUDIO event, then a visual.
- **RILL's presence is musical, not vocal (v1):** a soft synth chime signature precedes/underscores her subtitle lines — her "voice" until VO lands, and her audio caption forever.
- **Quiet is a feature.** The AmbienceCore zero-level = true-silence law is canon: dramatic silence is authored, not accidental. Music DUCKS for story beats and vanishes in caves unless a stinger earns entry.
- **Register:** analog-warm synthesis over orchestral; Terry's one kept track (`Zerogravity_Bloom_Favorite_1`) is the tonal reference — dreamy, floating, mid-tempo; new music must sit beside it without clashing.

---

## 3. BUS / MIX / DUCKING ARCHITECTURE

One Unity `AudioMixer` asset (`ZiptideMix`), authored create-only by a future `AudioMixAuthor` (the established author pattern; **not built by this lane**):

```
Master
├── Music        (AudioDirector routes here)
├── Ambience     (AmbienceDirector's 4 sources)
├── SFX
│   ├── UI          (2D: panels, boot, toasts)
│   ├── Hands       (grabs, holsters, tools)
│   ├── Weapons     (fire, impacts, discharge)
│   ├── World       (machines, doors, ziplines, POIs)
│   └── Creatures   (vocalisations, movement, telegraphs)
├── Voice        (RILL chime signature now; VO clips later — the slot exists from day one)
└── Comfort      (reserved: tinnitus-safe alarm shaping, loudness ceiling)
```

**Ducking rules (data, not code):** Voice ducks Music −8 dB and Ambience −4 dB (80 ms attack / 400 ms release). Weapons duck Ambience −2 dB momentarily. Nothing ducks Comfort. Travel crest: Music crossfades per `AudioProfile.crossfadeSeconds`; Ambience hard-crossfades inside the crest (the frozen-world hitch must not be audible).
**Player controls (closes the benchmark gap):** Master/Music/Ambience/SFX/Voice sliders on the comfort console surface (FH-S07 family), persisted DEVICE-level in PlayerPrefs like comfort presets — never in the profile save.
**Loudness law:** mix to −16 LUFS integrated for the golden route, true peak ≤ −1 dBTP; alarms/stingers never exceed Master by design headroom (Comfort bus ceiling). Measured, not vibed — see §11.

## 4. SPATIALIZATION RULES

- **2D (no spatializer):** Music, ambience beds, UI on the wrist/panels the player faces, RILL chime (she's at the shoulder — constant presence, not a localization puzzle).
- **3D full spatial:** everything diegetic — machines, creatures, weapons (others'), doors, ziplines, drips. Unity spatializer with Meta XR Audio SDK as the upgrade path (decision recorded: start with built-in panning + distance curves; adopt Meta spatializer only as a measured Phase-3 upgrade, it costs CPU).
- **Own-hands sounds:** 3D but near-field clamped (min distance 0.3 m) so grabs/holsters never feel detached.
- **Distance curves:** logarithmic, per-family max distances — UI 2 m · Hands 5 m · Weapons 40 m · Creatures 30 m · World machines 25 m · Alarms 60 m. Doppler OFF globally (comfort: pitch swoops are nausea-adjacent in VR).
- **Occlusion:** v1 = none (cost). Interiors get it FREE from the portal-cull room data if ever needed — flagged as a possible Phase-4 upgrade using `InteriorVisibilityCore` rooms as occlusion volumes; do not build speculatively.

## 5. VR COMFORT + ACCESSIBILITY / CAPTIONS

- **Comfort:** no sustained pure tones > 30 s; alarm loops mandatory 2 s gaps; loudness ceiling on the Comfort-shaped path; nothing pans hard-100% left/right for > 1 s (headphone fatigue); travel/flight audio follows the vignette philosophy — intensity scales DOWN with Cozy preset.
- **Captions for non-speech (new, closes the accessibility gap):** critical-cue captions through the EXISTING subtitle surface — `[COUPLER ARMED]`, `[CREATURE NEARBY]`, `[HULL ALARM]` — driven by the same audio events (§6 vocabulary), toggleable, S/M/L sizes inherited. Rule: **any audio cue whose miss blocks progress or safety MUST have a caption twin.** That's a checkable standard (slice queue marks which rows).
- **Subtitle-first remains v1 law** for speech; the Voice bus and per-line asset IDs mean VO drops in later without rewiring.
- **Handedness/hearing:** mono-compatibility check on all critical cues (single-ear players); sliders per §3.

## 6. FILE / IMPORT STANDARDS

- **Format at rest (repo):** WAV PCM 16-bit / 48 kHz, mono for 3D point sources, stereo only for Music/ambience-stereo beds/UI flourishes. Loudness normalized per family before import (SFX −18 LUFS-S reference).
- **Unity import settings (law, auditable):** SFX ≤ 2 s = Decompress On Load + PCM; 2–10 s = Compressed In Memory + Vorbis q0.7; Music/long loops = Streaming + Vorbis q0.6; `Force To Mono` ON for all 3D point sources; `Load In Background` ON for streaming. Sample rate: preserve.
- **Loops:** must be sample-exact loop points (the AmbienceTests seamlessness law extends to authored loops — the acceptance test in §11 verifies no click at the seam).
- **Location:** `Ziptide/Assets/Ziptide/Content/Audio/Clips/<family>/` — beside the existing track, family-foldered.

## 7. QUEST MEMORY / VOICE BUDGETS (proposed; baseline then ratchet, PerfBudget pattern)

- Runtime audio memory ≤ **24 MB** total (streaming buffers excluded); slice target ≤ 12 MB.
- Concurrent voices: **≤ 24 hard cap**, ≤ 16 typical; per-family caps: Weapons 6 · Creatures 6 · World 6 · UI/Hands 4 · beds 4+1. Voice-steal: oldest-quietest within family.
- One `AudioMixer`; DSP buffer "Best performance" (1024) unless latency testing on device demands 512 for hands/weapons.
- Per-frame audio CPU ≤ 2 ms on device (measured in the perf artifact class).
- Gate: extend the audit family with `AudioBudgetAuditRules` (WARN → BLOCK after baseline) counting clips-in-build, import-setting conformance (§6 law), and scene AudioSource census. *(Future lane builds it; this plan defines it.)*

## 8. NAMING / VERSIONING

- **Asset ID (stable, forever):** `aud.<family>.<event>[.<variant>]` — e.g. `aud.ui.boot_ready`, `aud.ship.punchit_ignite`, `aud.creature.swarm_telegraph.a`. File name = ID with underscores. The ID is the contract; the clip behind it may be re-produced freely (taste-layer law).
- **Versioning:** re-produced clips replace in place (same GUID, same ID) — history lives in git. `aud_manifest.json` (future author output) maps ID → file → import-law conformance → license entry, and is the audit's census source.
- **Trigger vocabulary:** every play site goes through one `AudioEvents.Play(id, position?)` seam (future, one small Core class) so the census, caps, captions, and ducking all key off IDs — never raw `AudioSource.Play` in new gameplay code. Existing 24 call sites migrate opportunistically, tracked as slice-queue debt rows.

## 9. LICENSING-AT-IMPORT WORKFLOW (extends the existing machinery — no new system)

1. Candidate audio lands in **`art_intake/audio/`** (the agreed holding path) with a `source.json` (origin, author, license, URL, date).
2. Import PR adds: the WAV under `Content/Audio/Clips/`, the manifest entry in `docs/licensing/third_party_manifest.json` (or `original-work` entry naming the generating tool + prompt-holder), and the `docs/CREDITS.md` anchor.
3. `tools/third_party_license_gate.py` must pass; `distributionStatus` anything but `approved` ⇒ the clip may not be referenced by any shipped scene/author (audit-checkable via the manifest↔census join).
4. AI-generated audio: recorded as original-work WITH generator + model + date (obligations evolve; provenance is cheap now, impossible later).
5. `Zerogravity_Bloom_Favorite_1.wav` gets a backfilled manifest entry via this exact flow — **first row of the queue's licensing debt**.

## 10. PRODUCTION PHASES

- **P0 — Rails (no assets):** mixer + buses author, sliders on comfort console, `AudioEvents` seam, captions-twin plumbing, budget audit WARN. All LLM-buildable, all device-verifiable in one session.
- **P1 — The vertical slice set:** the ~40 assets in `AUDIO_VERTICAL_SLICE_QUEUE.md` (first 20 minutes / M1–M4), produced to §6 law, landed through §9.
- **P2 — Stingers + adaptive music:** biome stingers into `AmbienceDirector`'s one-shot scheduler (it already schedules), music state layers (calm/alert) per world via extended `AudioProfile` (additive fields).
- **P3 — Breadth:** remaining worlds' beds are DATA (AmbienceCore specs), weapons/creature full coverage, Meta spatializer evaluation.
- **P4 — VO:** the deferred decision executes — same line IDs, Voice bus, captions already true.

## 11. ACCEPTANCE EVIDENCE (per the proof ladder; audio joins the lanes, not beside them)

- **EditMode:** import-law conformance (every clip matches §6 for its family); ID↔file↔manifest census clean; loop-seam click test (windowed RMS across the seam within ε); caption-twin coverage for flagged IDs.
- **PlayMode:** golden-route run asserts the expected `ZIPTIDE: AUD_PLAY id=…` tag sequence at the beats (boot chime before Home Hub interactable; PUNCH IT ignite between arm and travel; duck engages on RILL lines — mixer param sampled).
- **Visual-lane sibling — the LISTENING capture:** the deterministic route recording writes a loudness trace; assert integrated LUFS window and true-peak law (§3) per scene.
- **Device checklist rows:** slice queue's per-row acceptance column feeds `DEVICE_TEST_CHECKLIST.md`; every ❌ = MISS_LEDGER entry per the Class Law.
- **Proof-level claims:** an audio feature is "done" only at its required rung, same as everything else. `CI GREEN` says nothing about sound; the LUFS trace and device rows do.
