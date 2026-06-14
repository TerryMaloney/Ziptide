# Art & Audio Content Architecture (data-driven)

**Goal:** art and audio are fully **swappable through data** — no gameplay code references a
specific material, prefab, clip, or story name. Everything resolves through IDs + registries.
This is what makes "describe a scene → it gets built" possible without breaking runtime.

Last updated: 2026-06-14

---

## 1. Principle

Gameplay resolves content by **ID**, never by direct asset reference:

```
Gameplay/World  ──asks for──▶  Registry  ──returns──▶  Definition (ScriptableObject)  ──▶ assets
```

A world never says "use `RedGlass.mat`." It says "surface family = UpperClassGlass, kit =
ToxicVenice" and the registry resolves the approved asset. Swapping the asset never touches code.

Existing data already follows this: `WorldPackDefinition`, `VisualThemeProfile`,
`LocomotionProfile`, `AudioProfile`, item/job definitions. This doc extends that set for art.

---

## 2. Art definitions (ScriptableObjects)

| Definition | Holds | Notes |
|---|---|---|
| `WorldArtKitDefinition` | kitId, surfaceSets[], propSets[], decalSets[], sky ref, audio theme ref | one per world archetype (e.g. `ToxicVenice`) |
| `SurfaceSetDefinition` | material family id → material/shader settings | e.g. ToxicIndustrial, UpperClassGlass |
| `PropSetDefinition` | approved prop prefabs/primitives + placement metadata | walkable/decorative/blocker, collider mode |
| `DecalSetDefinition` | glyph/symbol decal materials | alien/oligarch glyphs |
| `SkyVistaProfile` | sky dome / backdrop mesh + distant silhouette settings | "large universe, small world" illusion |
| `VisualThemeProfile` | sky color, planet, ground tint (already exists) | extend with kit ref |

All live under `Assets/Ziptide/Content/Definitions/` and `Assets/Ziptide/Content/ArtKits/<Kit>/`.

## 3. Audio definitions

| Definition | Holds |
|---|---|
| `AudioThemeProfile` | music cue ref, ambience layer refs, reverb hint |
| `AmbienceLayerDefinition` | loop clip, gain, optional spatial emitter spec |
| `MusicCueDefinition` | clip, loop, crossfade time |
| `TravelSoundProfile` | one-shot travel stinger(s) |
| `RILLLineSet` | per-world VO clips keyed by RILL memory state |
| `HazardAudioProfile` / `DroneAudioProfile` | per-hazard / per-enemy sfx |

`AudioDirector` lives in `_Boot`. Worlds only **declare** an `AudioThemeProfile` reference and
ambience zones. Worlds never own global audio state. Missing refs log `AUDIO_AUDIT_FAIL` /
`MISSING_AUDIO_THEME` — never silent.

## 4. Registries

A registry maps `id (string)` → definition asset, validated at load:
- `ArtKitRegistry`, `SurfaceSetRegistry`, `AudioThemeRegistry`.
- Built from assets in known folders (or an explicit registry asset).
- Unknown id → `MISSING_ART_KIT` / `MISSING_AUDIO_THEME` audit failure.

## 5. Hard separation rules

- Art/audio assets **must not** reference gameplay scripts.
- Decorative objects carry no gameplay components (audit: `decorative object has gameplay script`).
- Walkable objects must have colliders (audit: `walkable object missing collider`).
- Gameplay code references **IDs**, never concrete material/prefab/clip names.

## 6. Folder layout (target)

```
Assets/Ziptide/Content/
  ArtKits/<KitName>/{Definitions,Prefabs,Materials,Decals,Textures,Sky,AudioRefs}
  Audio/{Music,Ambience,SFX,Voice}
  Definitions/{WorldPacks,VisualThemes,AudioThemes,ArtKits,SurfaceSets}
Assets/Ziptide/Visuals/{Runtime/{ThemeBinding,SkyVistas,Decals,PortalVFX}, Editor/{ArtKitValidators,ArtAudit}}
Assets/Ziptide/Audio/{Runtime/{AudioDirector,Ambience,Music,Voice}, Editor/AudioValidators}
```
