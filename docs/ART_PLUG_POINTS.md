# 🎨 ART PLUG POINTS — where art physically enters the game (verified in code 2026-07-02)

**Audience: the art track (Picasso).** Lane ownership + the live board are in **`docs/SPRINT_ART.md`**
(that file is the truth; this one is a reference map so no session re-derives the seams). Written by the
MP-track operator from a code pass; each row was verified against the actual source, not the docs.

| System | The seam | State |
|---|---|---|
| **Materials/colors everywhere** | `CityBuilder.Mat(color)` material cache + `GlobalPalette` on every layout; `ItemFactory.ApplyURPColor` | LIVE — a kit-material lookup slots in here (coordinate: `CityBuilder` is story-lane, `ScenePatcherArena` is MP-lane — claim the edit in HANDOFF and we'll land it, or hand us the resolve-through call) |
| **World geometry** | `CityBuilder.Build(root, kit)` builds from `CreatePrimitive` — kit meshes replace primitives behind a `SurfaceSetDefinition`-style lookup | LIVE, primitive |
| **Arenas** | `ScenePatcherArena.Cube(...)` — same swap, same lookup | LIVE, primitive |
| **Skies/mood** | `VisualThemeProfile` (+ your `skyVista` field) per world/arena via `ThemeAuthor` | ✅ ART-1 shipped |
| **Weapon models** | `ItemDefinition.modelPrefab` field EXISTS but **`ItemFactory` does not consume it yet** — wiring it is the first weapon-swap step (grip/muzzle transforms must survive: read `docs/systems/VR_RIG_GOTCHAS.md` FIRST, recipe in `docs/systems/ASSET_SWAP_PIPELINE.md`). Terry has a Tripo3D taser model waiting. Visual scale/color/grip/muzzle are already per-asset fields | Field unused |
| **Creatures** | each `*Behavior.BuildVisuals()` builds primitive bodies at runtime — creature meshes slot in per-archetype there (story-lane files: spec the kit, hand off the wiring) | LIVE, primitive |
| **Ship/cockpit/Quarters** | `CityBuilder.BuildShipyard`, the M4 ship deck/Quarters builders; `CosmeticDefinition` (Resources/Cosmetics) is a LOOKS-ONLY pipeline — first six-skin drop shipped by T-Dog | LIVE, primitive |
| **VFX/audio hooks** | `muzzleFlashPrefab`/`fireClip` fields on weapon defs; `AudioProfile` per world; `docs/design/ADAPTIVE_AUDIO.md` | Fields mostly unused |

**Import hygiene:** every new asset needs a `.meta`; meshes/textures need Unity-generated importers, so
model/texture imports are 🔧UNITY steps (queue in `TERRY_RUNBOOK.md`). C#/materials-by-code are ⚙CI.

**Queued art requests from the other lanes** (also logged in HANDOFF):
- **W000 viewport awe shot** (T-Dog): the gate-ring blooming open outside the hangar — a `W000_DriftIn`
  SkyVista is the natural vehicle.
- **Quarters cabin warmth** (T-Dog): interior treatment when surface kits reach interiors.
- **Locator v2 gauntlet mesh** (MP): a forearm-gauntlet silhouette for the wrist locator rework —
  spec in `docs/design/ABILITIES_AND_ARSENAL.md` §4; fits ART-4 (gear kits).
