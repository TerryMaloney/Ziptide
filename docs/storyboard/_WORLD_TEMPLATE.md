# W<NNN> — <World Name> · Story & Build README (TEMPLATE)

> Copy this to `docs/storyboard/W<NNN>_<Name>/README.md` and fill it in. Every section maps to a **real
> authorable field** so the story directly informs what we build (the data classes are real — see
> `06_SCHEMAS.md` and `systems/WORLD_BLUEPRINT.md`). Inherit meta from `STORY_BIBLE.md`; don't restate it.
> Keep canon: honor `ZIPTIDE_MASTER_BUILD_PLAN.md` §12 + the RILL beats. Tone: all-ages, story-deep.

**Status:** seed | designed | building | shipped
**Chapter:** <0–12 / DLC>  ·  **Biome/Type:** <City/Underground/Void/…>  ·  **Faction:** <Wake Guild/Sable/Warden/none>
**One-line identity:** <the single image a player remembers>

---

## 1. Role in the arc
How this world turns the screw (ref `STORY_BIBLE.md` §6 ladder). Cal's local goal. The **Signal/Pattern**
nudge. Which **RILL memory state** is active (Dormant/Stirring/Remembering/Unsealing/Integrated) and any
**locked RILL beat** here (only the 12 canon worlds have one).

## 2. Premise (local stakes)
2–4 sentences. Lived-in, low-stakes-on-the-surface, class/▶world texture. Who hires Cal and why she can't
just leave (passage credits / the leash).

## 3. The mystery object (required — one per world)
The thing that doesn't fit and pays off later. What it looks like, what's "wrong" about it, what it seeds.

## 4. Planet physics & biome  → `BiomeDefinition`
- `hazardType`: <toxic | wind | flood | radiation | pressure | static | cave-in | vacuum | pattern | none>
- Gravity / atmosphere / locomotion feel (e.g., low-g drift, heavy, slick, dark). *(Design now; some
  physics wired later — flag what's not yet mechanical.)*
- `nativeResourceIds` / `nativePlantIds` / `nativeCreatureIds` (tie §6–§9), `ambientTint`, `artKitId`
  (`WorldArtKitDefinition` — *stub OK for now*).

## 5. Sky & atmosphere  → `VisualThemeProfile` (+ `SkyPlanetRig`)
- `skyGradient` (horizon→zenith mood), `groundTint`, fog.
- `planet`: what hangs in the sky (a cracked moon? the next world? a *gap* where a star should be?) —
  `baseColor`/`accentColor`/`angularSizeDegrees`/`direction`/`rotationSpeed`. Make the sky tell story.

## 6. Machines & build/repair  → `MachineDefinition` + `RecipeDefinition` (+ conveyor)
What infra Cal repairs/builds here and **what it really is** (a seal? a relay? a dampener?). Inputs/outputs,
`ratePerSecond`, build/repair recipes. The signature machine that defines this world's loop.

## 7. Crops & resources  → `PlantDefinition` / `ResourceNodeDefinition` / `ResourceDefinition`
Signature crop (grow time, yield, tend tools, `harvestWith`) and/or mineable nodes (`resourceId`, reserve,
required tool function/tier). The `keyResource` from §12 lives here.

## 8. Weapons / gear found
What new tool/weapon (or upgrade) the player can find/earn here, and the **non-lethal** fiction for it
(ref the gear bank `09_GEAR_AND_TOOLS.md`). Tie to a salvage/wreck or job reward.

## 9. Abandoned ship / station (recurring device)
The derelict here: what it was, the **salvage node**, and the **log** (a line from a prior "waker"). One
sentence of its hidden-thread contribution (read in order across worlds).

## 10. Alien enemies  → `CreatureDefinition` + `DroneZoneDef`/`DroneCombatProfile`
Signature enemy (archetype: Swarmer/WallCrawler/Flyer/Bruiser, or Warden/Tendril/Drone), why it's here
(malfunction/immune-response/feral Bloom), `shockable`, loot, combat zone placement + tuning.

## 11. Missions  → `JobDefinition` (steps + `reward` + `completionFlag`)
Beat sheet table mapping to real step types:

| # | Beat | Player learns | Job step type | Marker/target |
|---|------|---------------|---------------|---------------|
| 1 | … | … | GoToMarker / DisableDronesCount / Deliver / Collect / ShootTargets | … |

- **Reward:** `ResourceCost[]` (passage credits etc.)  ·  **Completion flag:** `ZiptideFlags.<…>`
- **RILL lines (data):** 2–4 (or 6–12 for signature worlds), keyed to beats.
- **Flags required to enter / granted on complete** (`ZiptideFlags` convention `C<chap>_W<id>_<EVENT>`).

## 12. Map & placement  → `CityLayoutDefinition` (or world-kit) → informs the actual scene
Region roots / districts, spawn marker(s), hero-building interiors (job-giver / mission / the wreck),
travel-gate placement, anti-camp/verticality notes. This is what `WORLD_BLUEPRINT.md` builds.

## 13. Audio  → `AudioProfile`
Music mood + ambience (1 line).

## 14. Build recipe & cross-links
- Build via `docs/systems/WORLD_BLUEPRINT.md` (author layout → patcher → WorldPack → jobs → audit).
- Canon row: `ZIPTIDE_MASTER_BUILD_PLAN.md` §12 · Meta: `STORY_BIBLE.md` · Faction: `SHIPS_AND_FACTIONS.md`
  · Art/audio: `ART_AUDIO_CONTENT_ARCHITECTURE.md` · Gear: `09_GEAR_AND_TOOLS.md`.
- **Changeable-without-code note:** which parts are data (assets) vs. need a patcher/runtime.
