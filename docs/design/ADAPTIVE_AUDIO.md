# Adaptive Audio Layer — architectural blueprint (PLANNED, not started)

Source: Gemini音 plan via Terry (2026-06-21). **Status: design only — do NOT build yet.** Filed so it's in
the overall plan + cross-chat. This is the "Halo → Beastie Boys" dynamic-music + Quest-optimized
diegetic SFX system. It plugs into our existing event-driven, data-driven architecture.

## How it maps onto what Ziptide already has
- We already have **`AudioDirector`** (DontDestroyOnLoad singleton; crossfades BGM per
  `WorldPackDefinition.audioProfile` on scene load) and an **`AudioProfile`** SO. The Adaptive Audio
  Layer is the **evolution of `AudioDirector`** into a stem-mixing `AdaptiveAudioManager`, and
  `PlanetAudioProfile` is the **richer successor to `AudioProfile`** (referenced from
  `WorldPackDefinition`, one per world — fits the World Blueprint "data per world" recipe).
- **Decoupled + data-driven** — same contract style as `WorldProfile`/`CityLayoutDefinition`: the manager
  reads whatever profile the current world pack loads; no hardcoded audio.
- **ThreatLevel source** — we don't have a formal Event Bus yet. Add a lightweight `ThreatDirector`
  (static `event Action<float> ThreatChanged` or a polled float) that combat publishes to:
  `DroneCombatBehavior`/enemy spawns (campaign) and `PvpMatchDirector`/`PvpBot` (PvP) raise it; the
  garden/mining loops keep it near 0. Keep it pure/testable like `PvpMatch`/`WallState`.

## 1. Data: `PlanetAudioProfile` (ScriptableObject)
```csharp
[CreateAssetMenu(fileName="NewPlanetAudio", menuName="Ziptide/Audio/PlanetProfile")]
public class PlanetAudioProfile : ScriptableObject
{
    public string planetName;
    public float bpm;                 // beat-match the breakbeats to the ambient drones
    // Stem layers — SAME bpm + length so they stay phase-aligned:
    public AudioClip stemA_SubDrone;  // 30–60Hz anchor
    public AudioClip stemB_Spores;    // granular textures
    public AudioClip stemC_Ancient;   // Halo-style choir / solo strings
    public AudioClip stemD_CombatGroove; // breakbeats + heavy bass
}
```
One per world; lives in `Content` (alongside `AudioProfile`); referenced by `WorldPackDefinition`.

## 2. Core: `AdaptiveAudioManager`
- 4 looping `AudioSource`s on one GameObject, started with a **single `Play()`** so they're phase-locked.
- Listens to a global **`ThreatLevel` (0..1)**; crossfades stem volumes via Unity `AudioMixer` groups.

**Mixing matrix:**
| ThreatLevel | State | A SubDrone | B Spores | C Ancient | D Combat | Transition |
|---|---|---|---|---|---|---|
| 0.0–0.2 | Gardening / building | 1.0 | 0.8 | 0.4 | 0.0 | C slow sine fade in/out (feels alive) |
| 0.3–0.6 | Radar pings / incoming | 1.0 | 1.0 | 0.8 | 0.0 | B brightens, C locked full — tension peaks |
| 0.7–1.0 | Full swarm combat | 0.3 | 0.0 | 0.0 | 1.0 | fast coroutine crossfade over 1 bar; D takes over |

## 3. Quest-optimized diegetic SFX — Audio LOD
Don't play 50 belts + 20 bugs as full 3D audio (Quest throttles). LOD by distance:
- **LOD0 (in-hand / <2m):** held tool plays hi-fi uncompressed + haptics on collision.
- **LOD1 (2–10m):** machines/belts play standard 3D spatialized audio.
- **LOD2 (10+m):** no per-machine sources — one **"Factory Hum"** source at base center; raise its
  volume/pitch as more machines are built instead of instantiating sources.

## 4. Asset pipeline — `PlanetAudioImporter.cs` (editor)
Editor script watches a folder; AI-music stems named `World12_A.wav`, `World12_B.wav`, … auto-build the
`PlanetAudioProfile` and assign stems (no manual drag-and-drop across 80 worlds). Mirrors our existing
self-bootstrapping patcher/asset conventions.

## 5. Lane split (when we build it)
- **Architect (backend/data, pure + CI):** `PlanetAudioProfile` SO; `AdaptiveAudioManager` (evolve
  `AudioDirector`) + AudioMixer wiring; the pure `ThreatDirector`/mixing-curve logic + EditMode tests
  (matrix → volumes is deterministic and testable). Event listeners.
- **T-Dog (gameplay/VR/editor):** publish `ThreatLevel` from `DroneCombatBehavior`/spawns + PvP; the
  Audio LOD system on machines/belts; haptics on tool-collision; `PlanetAudioImporter` editor script;
  the **Grey-Box Audio Room** test scene (load one profile w/ 4 dummy stems, slide ThreatLevel 0→1 over
  60s to verify crossfades) — built via a `ScenePatcherAudioRoom` like our other self-gen scenes.

## 6. Phasing (later)
1. `PlanetAudioProfile` + `AdaptiveAudioManager` + mixer + pure mixing-curve tests (no real clips).
2. `ThreatDirector` + combat/PvP publishing + the Grey-Box Audio Room.
3. Audio LOD + haptics on tools.
4. `PlanetAudioImporter` pipeline + per-world stems as worlds get their final audio.

## Notes / open questions
- Decide whether `WorldPackDefinition` gets a `PlanetAudioProfile` field (replacing/augmenting
  `audioProfile`) or the manager resolves by world id.
- BPM-synced bar-length crossfades need a clock; reuse the deterministic-from-seconds style of
  `WeaponCharge`/`WallState`.
- Ties into the 80-world `WORLD_BLUEPRINT.md` recipe: a world's audio = one more authored asset.
