# 🎨 ART & AUDIO — 50 ideas (bank; **this file is Picasso's** — read README first)

Current (shipped by the art track — build on, don't duplicate): the FORGE (recipe → deterministic
mesh → CI photo turnarounds → APK; FORGE II added UV atlas + textures + normal/emissive + ONE
material; E1.4 device bake next; P2 geometry ops / P3 skinned creatures / P4 motion enveloped),
SkyVista canon skies, ArtModuleRegistry (`buildingModule:`/`scatterKind:`/`surfaceSet:` ids, primitive
fallback), ADAPTIVE_AUDIO design (4-stem Signal-reactive), QUEST budget (72fps / ≤120 draw calls /
≤150k tris / ≤25 materials), avatar v1, ASSET_FORGE_MAP (law against re-invention). Bar: premium-indie
AAA read — every frame screenshottable, every sound intentional. **All rows 🎨 by definition.**

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Building-module Forge family for `salvage_row` + `toxic_tenement`: wall/window/door/roof/trim recipes registered via ArtModuleRegistry so W002's warren wears real kit (the top-leverage Q2 payoff). | M | Forge, ArtModuleRegistry, BuildingGrammar | 🎨 |
| 2 | Muzzle/impact/haptic-audio triads: each of the 7 weapons gets a paired muzzle flash + impact decal + haptic-synced fire sound as one authored "feel packet" swapped per ArenaWeaponDefinition. | M | Forge VFX, AudioDirector, weapons | 🎨 |
| 3 | Stun-arc VFX language: a single reusable crackle shader (color-tinted per source) for taser darts, creature disables, thumper — one material, budget-cheap, instantly readable as "non-lethal". | S | VFX, AudioDirector | 🎨 |
| 4 | Gate-bloom set piece: the Ziptide gate-ring opening as a layered emissive + refraction bloom, the recurring "wonder" beat (W000 cast-off, every travel). | M | SkyVista/VFX, ShipCastOffRuntime | 🎨 |
| 5 | Bloom-contamination surface: a spreading corruption decal/shader for the story's Bloom, driven by a 0-1 float so worlds can dial infection per WorldSpec. | M | surface shader, WorldSpec | 🎨 |
| 6 | Adaptive audio ART-3 kickoff: `AdaptiveAudioManager` (4-stem, Signal-reactive) per ADAPTIVE_AUDIO.md, stems crossfading on combat/calm/story state. | L | AudioDirector, adaptive audio | 🎨 |
| 7 | Per-biome ambience beds: a looping ambience + one-shot layer per biome matrix cell (toxic wind, cistern drip, salt-flat shimmer), distance-mixed, Quest-cheap. | M | AudioDirector, biome matrix | 🎨 |
| 8 | RILL VO pipeline: ElevenLabs voice production for the ~34 existing lines + a naming/format convention so new RILL lines drop VO clips into `RillLineAuthor` by id. | L | RillLineAuthor, audio pipeline | 🎨 |
| 9 | Transmission blend-voice: a distinct degraded, layered voice for the Transmission (vs RILL's clean one), fidelity worsening per clarity tier — the story's dread instrument. | M | audio pipeline, TransmissionConsole | 🎨 |
| 10 | HUD visual language pass: unify PvpHud / wrist UI / objective toasts under one type ramp, corner-frame motif, and 2-color accent system so every readout looks like one game. | M | UI, PvpHud, wrist UI | 🎨 |
| 11 | Weapon wrap cosmetics: Forge material variants (not geometry) per weapon as CosmeticDefinition looks-only skins — the arena/quarters unlock currency's art. | M | Forge materials, CosmeticDefinition | 🎨 |
| 12 | Avatar v2: articulated forearms + a subtle idle breathing/hand-settle so the player's own body reads as a character, not floating gloves. | M | avatar rig | 🎨 |
| 13 | Photo mode: a deployable orbit camera + framing UI writing PNGs to a Quarters photo wall — turns the "screenshottable" bar into a player-facing feature. | M | camera rig, Quarters, Visuals | 🎨 |
| 14 | Creature silhouette language: a shared read-at-a-glance shape grammar (grazers rounded/soft, threats angular/asymmetric) applied as FORGE II P3 skinned meshes land. | L | Forge P3, creature meshes | 🎨 |
| 15 | Lighting mood per chapter: a VisualThemeProfile lighting preset set (warmth, contrast, fog) that shifts as the story darkens Ch.1→endgame — atmosphere on the narrative curve. | M | VisualThemeProfile, chapter data | 🎨 |
| 16 | The Shell reveal escalation: author the hexagonal Shell-grid at each canon stage (faint → banding → full wall) as SkyVista keyframes so the "oh." moment lands harder each chapter. | M | SkyVista | 🎨 |
| 17 | Impact-frame juice: a 2-3 frame scale/emissive pop on every successful hit (weapon + creature) — the universal "it connected" language, one shared shader. | S | VFX shader | 🎨 |
| 18 | Holstering/grab audio: a distinct click + haptic per item category (gun/tool/hammer) so the belt loop feels tactile without looking down. | S | AudioDirector, belt | 🎨 |
| 19 | Ambient world audio pockets: reverb + spatial one-shots for interiors/caves/canyons registered from RoomPartitioner + terrain bounds — spaces that sound like spaces. | M | AudioDirector, RoomPartitioner | 🎨 |
| 20 | Forge material family: Toxic Earth Industrial (rust, grime, hazard-stripe) as the first full `surfaceSet:` fulfillment, replacing CityBuilder flat colors behind the registry. | M | Forge, ArtModuleRegistry, CityBuilder | 🎨 |
| 21 | Forge material family: Salvage (welded plate, cable, patchwork) for the arena/ship aesthetic. | M | Forge, ArtModuleRegistry | 🎨 |
| 22 | Forge material family: Alien Origami/Pattern (the ALIEN_ORIGAMI brief) — the story's "wrong geometry" surface for late worlds. | M | Forge, ArtModuleRegistry | 🎨 |
| 23 | Weapon-charge VFX: a per-weapon emissive charge tell (the amber last-charge glow, prism charge ring) as authored looks the combat rows request. | S | VFX, WeaponCharge | 🎨 |
| 24 | Water/flood shader: a single tiling water surface (depth tint, foam edge, refraction) for canals, floods, tide flats and the arena flood hazard — one material, many worlds. | M | surface shader, hazards | 🎨 |
| 25 | Objective beacon language: a unified "go here" beam/ring (ObjectiveBeacon) styled per context (job = gold, mode = team color, story = cyan) — consistent, comfort-safe. | S | VFX, ObjectiveBeacon | 🎨 |
| 26 | Creature disable finisher VFX: the "bubble away" confetti-crumple the combat/creature rows want, as one shared non-lethal disable effect. | S | VFX | 🎨 |
| 27 | Forge P2 geometry ops: extend the shape grammar (bevel-loops, insets, greeble strips) so weapon/prop silhouettes gain the detail that reads as AAA at close VR range. | L | Forge P2 | 🎨 |
| 28 | Adaptive music intensity: the 4-stem manager reads combat proximity/health so fights swell and calm exploration breathes — the Signal-reactive core made audible. | M | adaptive audio, combat state | 🎨 |
| 29 | Sky day/night keyframes: author dawn/day/dusk/night SkyVista variants per canon world so the WORLDS day/night idea has real art to lerp between. | M | SkyVista | 🎨 |
| 30 | UI diegetic-ization pass: replace floating TextMesh readouts (garden, mine, machine) with in-world holo cards + printed tickets — the "no floating UI" AAA read. | M | UI, world runtimes | 🎨 |
| 31 | Ship hull paint + decal system: VisualThemeProfile-driven material sets + a decal layer (nose art, faction marks) across the 19 hull parts. | M | ShipHullBuilder, VisualThemeProfile | 🎨 |
| 32 | Foliage/scatter art kits: `scatterKind:` mesh fulfillments per biome (rocks, flora, debris, crystals, bones) replacing scatter primitives, GPU-instanced. | M | Forge, ArtModuleRegistry, ScatterField | 🎨 |
| 33 | Muzzle-to-tracer unity: the TracerFx line + muzzle + impact share one color/width language per weapon so every shot reads as its weapon at a glance. | S | TracerFx, VFX | 🎨 |
| 34 | Warden/authority audio motif: a recurring low chord + eye-pulse SFX for the arrest sentinels — the story's "you are watched" leitmotif. | S | AudioDirector, WardenBehavior | 🎨 |
| 35 | Chapter title cards: an authored transition card (type + motif + audio sting) on entering a new chapter's first world — AAA pacing punctuation. | M | UI, audio, chapter data | 🎨 |
| 36 | Emote/taunt animation set: the MP emote-wheel content (wave, dab, GG) as avatar animation clips + audio, sync-ready for two-Quest. | M | avatar rig, MP emote wheel | 🎨 |
| 37 | Podium/celebration VFX: confetti, trophy shine, framed victory-photo styling for the MP end-of-match ceremony. | M | VFX, MP podium | 🎨 |
| 38 | Forge normal/roughness authoring pass: bring FORGE II's map pipeline to the building modules + surfaces so buildings catch light like real materials on device. | M | Forge II maps | 🎨 |
| 39 | Hazard visual identity: each hazard (Wind/Static/Flood/Spore/Radiation) gets a distinct, kid-readable particle + color signature so danger is instantly parseable. | M | VFX, hazards | 🎨 |
| 40 | Locator/scanner VFX v2: the wrist-locator pulse + holo-radar styling (from the ABILITIES locator-v2 design) as the art fulfillment. | M | VFX, LocatorState | 🎨 |
| 41 | Ambient life motes: tiny instanced drifting particles (dust, spores, embers) per biome — the cheap trick that makes a static world feel alive. | S | VFX, biome matrix | 🎨 |
| 42 | Forge P3 skinned creature: the first archetype (Swarmer — smallest) rebuilt as a skinned Forge mesh replacing primitives, proving the creature pipeline. | L | Forge P3, CreatureRuntime | 🎨 |
| 43 | Forge P4 motion: procedural idle/move animation for the P3 creature (breathing, gait) so creatures move like organisms. | L | Forge P4, creature anim | 🎨 |
| 44 | Cosmetic supply drops: batches of looks-only CosmeticDefinition art (weapon wraps, avatar skins, ship paints) as the meta-progression's reward content. | M | Forge materials, CosmeticDefinition | 🎨 |
| 45 | RILL character presence: an authored orb visual (idle shimmer, reactive color on emotion) + subtle proximity SFX so the companion feels present, not a subtitle source. | M | Visuals, RillCompanion, audio | 🎨 |
| 46 | Environmental storytelling kits: authored ruin/wreck/graffiti prop sets that say what a place WAS without text, placed by ScatterField at StoryAnchor POIs. | M | Forge, ScatterField, POI | 🎨 |
| 47 | The Transmission playback visual: a degrading-signal screen shader for TransmissionConsole (scanlines, dropout, clarity-tier decode) — the story's central object made cinematic. | M | shader, TransmissionConsole | 🎨 |
| 48 | Adaptive ambience-to-combat transition SFX: the whoosh/duck that sells the moment calm becomes danger, tied to the 4-stem manager's state changes. | S | adaptive audio | 🎨 |
| 49 | Full W001 Toxic Venice art pass: execute the W001 brief end-to-end (surfaces + building modules + props + sky + audio bed) as the first world that looks SHIPPED — the art proving ground. | L | Forge, ArtModuleRegistry, SkyVista, AudioDirector | 🎨 |
| 50 | Signature lighting moment per world: one authored "postcard" framing per canon world (light shaft, silhouette, color contrast) the arrival vista points the player at — 12 screenshots that sell the game. | L | VisualThemeProfile, SkyVista, landmark kits | 🎨 |
