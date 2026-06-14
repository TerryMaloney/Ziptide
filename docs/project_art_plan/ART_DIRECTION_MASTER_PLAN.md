# ZIPTIDE — Project Art Direction Master Plan

**Status:** Draft v0.1  
**Scope:** Art direction, surface/asset pipeline, prompt-to-world workflow, Quest-safe visual strategy.  
**Important:** This document is planning-only. It does not change gameplay code, scenes, prefabs, materials, shaders, or build settings.

---

## 1. Art Direction Goal

ZIPTIDE should look like a high-contrast cyberpunk / cosmic-space / alien-infrastructure VR game while staying performant on standalone Meta Quest hardware.

The target feeling is:

- **Interstellar-scale awe:** giant planets, black holes, impossible skies, curved orbital structures, high-contrast silhouettes.
- **Worker-level tactility:** dirty metal, worn glass, safety markings, salvage crates, cables, industrial tools, hand-built repairs.
- **Alien-but-readable surfaces:** symbols, repeating glyph panels, luminous seam lines, nonhuman geometry, but still modular enough for procedural/LLM placement.
- **Small-world illusion:** each loaded area is compact, but skyboxes, portals, silhouettes, blocked sightlines, and doors imply a much larger universe.

The game should not chase photorealism through expensive real-time rendering. It should chase **memorable composition, strong material language, and cheap but striking visual tricks**.

---

## 2. Non-Negotiable Art Production Rules

1. **Gameplay code never depends on specific art names.**  
   Systems request assets through registries, profiles, or tagged content definitions.

2. **Every art element belongs to a kit.**  
   Buildings, rocks, props, symbols, portals, surface decals, and sky objects must be grouped into named kits that can be swapped.

3. **One loaded world is small. The universe looks huge.**  
   Use doors, airlocks, gates, occlusion, fog, sky domes, massive background meshes, and portal transitions to imply scale without rendering giant worlds.

4. **Materials must be reusable and parameterized.**  
   Prefer one material family with different parameters over dozens of unique materials.

5. **No heavy art feature ships without a budget.**  
   Glass, transparency, particles, reflections, emissive surfaces, post-processing, and giant vistas must have explicit limits.

6. **AI may place content, but humans approve the kit grammar.**  
   Claude/Cursor should be allowed to assemble from approved kits; it should not invent random one-off visual structures that break performance or consistency.

---

## 3. Current Repo Structure Observations

Based on the GitHub repo docs currently visible:

- The project already declares a strong modularity law: story, visual themes, characters, pods, sectors, and signature visual layers should be swappable without cascading breakage.
- Existing architecture expects data-driven content packs and visual skins through a `VisualTheme` / registry style system.
- Existing module map separates Core, Gameplay, Content, Visuals, Ship, and Platform.Quest.
- The performance budget document exists but is still mostly TBD.

This art plan should therefore live as a **docs-side production guide first**, then later become:

- `VisualThemeProfile` fields
- `SurfaceSetDefinition` assets
- `WorldArtKitDefinition` assets
- material/shader presets
- prefab kit registries
- art audit tests

---

## 4. Visual Pillars

### 4.1 Cyberpunk Working-Class Space

ZIPTIDE should not look like generic neon cyberpunk. The first Earth city should feel like a poisoned, elevated, working-class infrastructure world:

- toxic lower canals
- stacked catwalks
- dense service pipes
- translucent roof gardens above
- cheap hazard paint below
- glass owned by the upper class
- rust owned by everyone else

### 4.2 Cosmic Scale Without Rendering Cosmic Scale

Use distant visual anchors:

- giant planet or moon behind skyline
- orbital ring silhouette
- black-hole lensing texture on sky dome
- distant megastructure seen through windows
- curved sky bridges that disappear behind fog
- fake parallax background layers

These should be mostly skybox/sky-dome/background mesh tricks, not fully traversable geometry.

### 4.3 Alien Symbolic Surfaces

Alien or future-human architecture should use a consistent symbol language:

- glowing vertical strokes
- repeating ring/crescent motifs
- triangular warning glyphs
- non-English but readable shape families
- panels that look like circuitry and ceremonial writing at the same time

Symbols should be decals or material overlays, not unique geometry whenever possible.

### 4.4 Tool-Built Geometry

The world should look like it was repaired, extended, and modified over time:

- plates bolted over cracks
- mismatched railings
- emergency foam seals
- temporary bridges
- welded seams
- power cables tied to ancient surfaces

This supports the player fantasy: the player is not a chosen hero; they are a field tech trying to keep impossible infrastructure alive.

---

## 5. Surface Language Taxonomy

Every visible surface should belong to one of these families.

### 5.1 Toxic Earth Industrial

Use for W001 Toxic Venice and early Earth worlds.

- oxidized steel
- stained concrete
- safety yellow markings
- cheap polymer panels
- dirty glass
- rusted rails
- chemical residue
- green-blue toxic glow from below

Prompt words: `oxidized`, `layered`, `service-worn`, `hazard-marked`, `chemical-stained`, `worker-built`.

### 5.2 Upper-Class Clean Glass

Use for oligarch zones and rooftop gardens.

- red glass towers
- white stone or ceramic panels
- clean gold trim
- protected greenery
- water without sludge
- soft lighting
- curved balcony silhouettes

Prompt words: `sterile`, `privileged`, `controlled nature`, `red glass`, `white stone`, `quiet luxury`, `sealed air`.

### 5.3 Salvage / Field Repair

Use for player-built repairs and job objects.

- snap plates
- clamps
- cable bundles
- portable generators
- exposed battery packs
- magnetic anchors
- modular blocks

Prompt words: `temporary`, `bolted`, `patched`, `field-rigged`, `portable`, `industrial tool`.

### 5.4 Alien Origami / Pattern Surfaces

Use for gates, Pattern/Bloom structures, alien worlds.

- folded metallic paper
- luminous crease lines
- impossible bevels
- geometric seams
- calm silver/gold glow
- not slimy, not horror

Prompt words: `folded`, `creased`, `geometric`, `living paper`, `silent`, `ceremonial`, `correction lines`.

### 5.5 Cosmic / Deep Space

Use for stations, black-hole worlds, orbital zones.

- dark panels
- rim-lit silhouettes
- black glass
- starfield reflections
- cold blue lights
- giant background discs/rings

Prompt words: `vast`, `rim-lit`, `void-backed`, `silent`, `orbital`, `black glass`, `subtle scale`.

### 5.6 Waterworld / Biofilter Surfaces

Use for garden and water worlds.

- translucent bio-panels
- algae tubes
- wet stone
- floating platform foam
- luminous plant veins
- grown structures

Prompt words: `biofilter`, `translucent`, `grown`, `wet`, `softly luminous`, `filtered air`.

---

## 6. Recommended Art Architecture

### 6.1 Core Definitions To Add Later

These should be ScriptableObject concepts later, not necessarily code now.

#### WorldArtKitDefinition

Purpose: one data asset per world/art kit.

Fields:

- `kitId`
- `displayName`
- `surfaceFamilies[]`
- `buildingPrefabs[]`
- `walkwayPrefabs[]`
- `rockPrefabs[]`
- `propPrefabs[]`
- `symbolDecalSet`
- `skyProfile`
- `lightingProfile`
- `materialPalette`
- `performanceTier`

#### SurfaceSetDefinition

Purpose: reusable surface palettes.

Fields:

- `surfaceSetId`
- `baseMaterial`
- `trimMaterial`
- `decalMaterial`
- `emissiveColor`
- `roughnessRange`
- `damageLevel`
- `symbolDensity`
- `allowedWorldTypes`

#### PromptBuildRequest

Purpose: structure natural-language art placement into validated data.

Example input:

> “I want a red building on the right side made mostly of glass with alien symbols. On the left I want a short tower made of white stone.”

Output should become:

- object type: building/tower
- kit: `UpperClassCleanGlass`
- placement zone: right/left/front/back
- approximate size: small/medium/large
- material family: red glass / white stone
- symbol density: low/medium/high
- collision type: decorative / walkable / blocker
- performance cost estimate

Never let raw prose directly modify a scene. Convert prose into a structured build request first.

---

## 7. Prompt-to-World Authoring Workflow

The desired workflow is:

1. Terry describes the scene in natural language.
2. Claude converts the description into a structured `ArtBuildPlan`.
3. The plan is checked against art rules and performance budgets.
4. A scene patcher or editor tool places approved kit pieces.
5. An art audit reports object count, material count, transparency use, lights, decals, and blocked paths.
6. Only then does the build run.

### Example Authoring Flow

Terry prompt:

> “Make the right side a red glass oligarch building with alien symbols. Add a short white stone tower on the left. Put a toxic canal below and a bridge between them.”

Claude should output something like:

```json
{
  "worldId": "W001",
  "zoneId": "dispatch_courtyard_viewline",
  "placements": [
    {
      "type": "building_shell",
      "kit": "upper_class_clean_glass",
      "side": "right",
      "size": "large",
      "surfaceSet": "red_glass_gold_trim",
      "decalSet": "alien_warning_low",
      "walkable": false,
      "collision": "blocker",
      "performanceTier": "cheap"
    },
    {
      "type": "tower",
      "kit": "upper_class_ceramic_stone",
      "side": "left",
      "size": "short",
      "surfaceSet": "white_stone_soft_gold",
      "walkable": false,
      "collision": "blocker",
      "performanceTier": "cheap"
    },
    {
      "type": "bridge",
      "kit": "toxic_venice_industrial",
      "connects": ["left_platform", "right_platform"],
      "surfaceSet": "rusted_steel_hazard_trim",
      "walkable": true,
      "collision": "walkable"
    }
  ]
}
```

The scene patcher should only place objects from approved registries.

---

## 8. Quest-Safe Rendering Strategy

### 8.1 Lighting

Use:

- one main directional light per world
- baked or static lighting for art-heavy areas later
- emissive materials for “fake lights”
- very limited real-time point lights

Avoid:

- many real-time lights
- real-time shadows everywhere
- expensive reflection probes in every area

### 8.2 Materials

Use:

- URP Lit for most surfaces
- URP Unlit for sky/portal/far background effects
- shared material palettes
- texture atlases where practical
- material property blocks for color variation

Avoid:

- many unique material instances
- expensive transparent materials
- high-overdraw particle fog
- screen-space effects as the core look

### 8.3 Glass

Glass is visually important, but expensive if handled naively.

Use fake glass first:

- opaque tinted material
- bright edges
- subtle texture lines
- emissive rim trims
- no true refraction by default

Reserve real transparency for a few hero objects.

### 8.4 Black Hole / Cosmic Vistas

The Interstellar-style black-hole ambition should be treated as a **skybox/cinematic vista system**, not a physically simulated effect.

Use:

- pre-rendered or procedural sky dome texture
- layered billboard rings
- shader-distorted disc on a far sphere/quad
- slow rotation
- high contrast silhouettes in foreground
- no expensive full-screen gravitational lensing in gameplay

The player should feel like the black hole is enormous because of composition and scale reference, not because Unity is calculating it physically.

---

## 9. World Illusion Strategy

The game should rely on compact loadable spaces that feel large.

Techniques:

- doors/airlocks that load adjacent worlds
- blocked sightlines and turns
- fog/haze to hide edges
- sky domes with huge background structures
- distant unreachable silhouettes
- windows looking into fake spaces
- portal travel to “nearby” areas that are actually separate scenes
- vertical layering to imply a city below/above

Art direction should always ask:

> What does the player see that makes this place feel bigger than the playable area?

---

## 10. Kit-Based World Families

### 10.1 Toxic Venice City Kit

Used for W001 and related Earth worlds.

Required pieces:

- elevated walkway straight
- elevated walkway corner
- railing segment
- bridge segment
- toxic canal surface
- lower-level pipe wall
- upper-class glass facade
- white stone garden tower
- dispatch kiosk shell
- salvage crate
- security drone perch
- warning sign panels

Visual signature:

- green toxic glow below
- red glass towers above
- rusted walkways in middle
- white stone garden terraces as class contrast

### 10.2 Orbital Station Kit

Required pieces:

- curved corridor segment
- window wall
- airlock door
- maintenance hatch
- exposed cable trunk
- ring station background mesh
- docking arm silhouette

Visual signature:

- black glass
- blue rim lights
- huge planet view
- quiet scale

### 10.3 Desert Triple-Sun Kit

Required pieces:

- sand platform
- heat-shield canopy
- reflective solar panel
- wind-cut stone arch
- buried alien panel
- triple-shadow sky dome

Visual signature:

- hard orange/white lighting
- triple cast-shadow motif
- heat haze using cheap screen-independent tricks

### 10.4 Waterworld Platform Kit

Required pieces:

- floating platform
- mooring pole
- algae filter tank
- wet metal walkway
- distant wave dome/plane
- storm wall background

Visual signature:

- blue-green light
- reflective-but-cheap wet surfaces
- moving water far below

### 10.5 Black Hole Fringe Kit

Required pieces:

- black metal platform
- white rim-light rails
- distorted sky dome
- accretion disc billboard
- gravitational warning glyphs
- tether anchor points

Visual signature:

- foreground silhouettes
- enormous cosmic background
- slow movement
- silence / sub-bass implied by audio

---

## 11. AI Art/Scene Prompt Rules

When asking Claude or Cursor to build art/layout, use this structure:

```text
World:
Player route:
Main visual anchor:
Left side:
Right side:
Background vista:
Walkable objects:
Decorative-only objects:
Hazards:
Lighting mood:
Material families:
Performance limit:
Do not modify:
Acceptance test:
```

Example:

```text
World: W001 Toxic Venice Dispatch Courtyard
Player route: spawn -> dispatch kiosk -> bridge -> drone test platform
Main visual anchor: red glass oligarch tower behind toxic canal
Left side: short white-stone garden tower, decorative only
Right side: red glass building with alien glyph decals, decorative blocker
Background vista: layered elevated city silhouettes fading into green haze
Walkable objects: rusted bridge, catwalk, courtyard platform
Decorative-only objects: pipes, toxic tanks, distant towers, signs
Hazards: toxic canal below, railings block fall paths
Lighting mood: green underglow + warm upper light
Material families: toxic industrial, upper-class clean glass, alien glyph decals
Performance limit: under 60 decorative objects, no true glass transparency, one realtime light
Do not modify: XR rig, Boot scene, TravelCoordinator, input settings
Acceptance test: scene audit passes, player can traverse full route, no new runtime scripts
```

---

## 12. Future Folder/Asset Structure

Recommended future Unity project organization:

```text
Assets/Ziptide/Content/ArtKits/
  ToxicVenice/
    Materials/
    Prefabs/
    Decals/
    Sky/
    Definitions/
  OrbitalStation/
  TripleSunDesert/
  Waterworld/
  BlackHoleFringe/

Assets/Ziptide/Visuals/
  Runtime/
    ThemeBinding/
    MaterialSystems/
    SkyVistas/
    PortalVFX/
  Editor/
    ArtKitValidators/
    PromptBuildPlanImporters/
```

Recommended docs structure:

```text
docs/project_art_plan/
  ART_DIRECTION_MASTER_PLAN.md
  SURFACE_LANGUAGE.md
  KIT_AUTHORING_RULES.md
  PROMPT_TO_WORLD_WORKFLOW.md
  QUEST_ART_PERFORMANCE_BUDGET.md
  W001_TOXIC_VENICE_ART_BRIEF.md
```

For now, this branch only creates the first planning file.

---

## 13. Art Audit Requirements

Before any generated art/layout is accepted, an audit should report:

- object count
- renderer count
- unique material count
- transparent material count
- realtime light count
- particle system count
- reflection probe count
- estimated decorative vs walkable objects
- forbidden runtime scripts on decorative art
- missing colliders on walkable objects
- blocked player route checks

Art audit should fail if:

- a generated art pass adds XR/runtime systems
- a world scene contains Boot-owned singletons
- a decorative object is accidentally walkable
- a walkable route becomes blocked
- unique material count exceeds the budget
- transparency count exceeds the budget
- realtime lights exceed the budget

---

## 14. W001 Toxic Venice Art Brief

### Core Image

A poisoned Venice-like vertical city: the canals are not romantic water, but chemical sludge. The playable layer is a network of rusted walkways and courtyards. Above, the wealthy live behind red glass and white stone garden structures. Below, pipes leak into glowing green canals.

### Composition

The player should spawn on a safe platform with three clear visual layers:

1. **Below:** toxic green canal glow, trash, pipes, unreachable lower structures.
2. **Middle:** player route, rusted walkways, job kiosk, drone test platform, salvage equipment.
3. **Above:** red glass oligarch building, white stone tower, protected greenery.

### First Art Targets

- Red glass building on the right side, decorative blocker, alien symbols on glass.
- Short white stone garden tower on the left, decorative, upper-class contrast.
- Main bridge across toxic canal, walkable.
- Distant city silhouettes in green haze.
- Alien/oligarch warning glyphs on panels and glass.

### Visual Rules

- Toxic green underlight should make the lower layer feel dangerous.
- Red glass should be mostly opaque/fake glass, not expensive transparency.
- White stone tower should be clean and almost unfairly beautiful.
- Player route must remain obvious.
- Every decorative object should be placed to improve composition, not clutter.

---

## 15. Next Steps

1. Keep this plan as docs-only until runtime/travel stability remains green.
2. Ask Claude to produce `W001_TOXIC_VENICE_ART_BRIEF.md` from this plan.
3. Add `SurfaceSetDefinition` and `WorldArtKitDefinition` only after the art contracts are approved.
4. Build one Toxic Venice kit with placeholder primitives first.
5. Add art audit before importing high-detail assets.
6. Only then begin prompt-driven scene dressing.

---

## 16. Claude Implementation Prompt Seed

Use this later, after the core project remains stable:

```text
You are working in ZIPTIDE as an art/layout implementation agent.
Do not modify runtime systems, XR rig, Boot scene, input, travel, inventory, or gameplay code.
Your task is art-only and must use approved art kit folders and definitions.
Read docs/project_art_plan/ART_DIRECTION_MASTER_PLAN.md first.
Convert the requested natural language scene change into a structured ArtBuildPlan.
Then implement only through art kit definitions, scene patcher data, or approved decorative prefabs.
Run art audit after changes.
Stop if route traversal, spawn safety, or performance budgets fail.
```
