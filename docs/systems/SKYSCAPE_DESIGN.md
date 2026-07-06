# Skyscape Design — the Prospect bar (added 2026-07-06)

**Status:** design plan only — nothing in this doc is built yet. Existing code (`SkyVistaDefinition`/
`SkyVistaRig`/`SkyPlanetRig`, `Editor/Patching/SkyVistaLibrary.cs`) is the *current* system and is
referenced throughout as the foundation this plan extends, not replaces. Terry's brief: the sky is one
of the most important things in the game — the thing that has to make VR feel like *"you are standing
on a different planet,"* the way *Prospect* (2018) does on a screen. A planet or a sun sitting in a
skybox is not enough. This doc exists so that bar is nailed down in writing before more worlds get
authored against a weaker one.

## 0. The thesis, in one line
**The sky should never read as a backdrop. It should read as weather.** A backdrop is something you
glance at once and stop seeing. Weather is something the air itself is doing to you right now —
haze in your eyeline, particulate catching the light, a color temperature your skin would believe if
it could feel it. That's the difference between "there's a planet in the sky" (decoration) and "I am
on a moon, under something enormous, and I can feel the air is not Earth's" (a place).

**Why this matters more in VR than in any film or flat game:** in *Prospect*, the audience looks where
the camera points. In this game, Terry looks where *he* points, with his own neck, and the giant thing
in the sky is not a shot — it's just *there*, indifferent to whether he's looking at it. That's a kind
of awe flat media literally cannot deliver. If the sky is only as good as a 2D skybox, VR is being used
to show a worse version of what a movie already does better. If the sky is built around *presence* —
things that are actually far away, actually huge, actually filtered through real-feeling air — VR does
something no movie can. This is the argument for spending real budget here.

**The specific mechanism, not just the vibe:** stereo VR gives depth cues a flat screen can't —
binocular disparity, and real parallax as the player's head moves, not simulated ones. That only pays
off if the haze card, the particulate layer, and the celestial body actually sit at *different
world-space distances* from the camera, not all painted onto one infinite-distance skybox dome. A
haze card at 40m, drifting particulates at 8–15m, and a body treated as genuinely distant will
separate into real depth the moment the player leans or turns their head — a flat re-render of the
same composition on a screen loses that instantly. This is the one place in this whole plan where "do
it in VR" is not just "do the same thing at higher fidelity" — it's a different, better *kind* of
effect that doesn't exist outside a headset. Any implementation pass should treat depth-placement as a
first-class decision, not an afterthought once the visuals "look right" in a 2D preview.

## 1. What *Prospect* actually does (the reference, made specific — not "space opera")
*Prospect*'s look is not "lots of planets and starfields." Its whole visual thesis is the OPPOSITE of a
starfield flex — it almost never shows clean open space. What it actually does, specifically:
- **Thick, warm, particulate-choked atmosphere.** Amber/gold haze, visible drifting spores/pollen
  catching backlight, everything softened at a middle distance — you are always looking *through*
  something, never through clean air.
- **One enormous body, filtered, not framed.** The gas giant is huge and close, but it's almost always
  glimpsed *through* canopy, fog, or a doorway — partially occluded, never a clean centered "wallpaper"
  shot. Occlusion is what sells scale; an unobstructed view of something huge reads as a screensaver.
- **Human-scale foreground, cosmic-scale background, always both in the same shot.** A dirty tarp, a
  rusted ladder, a breathing mask — ordinary, textured, *used* objects in the foreground with the
  impossible sky behind them. The contrast is what makes the sky feel real instead of ornamental.
- **A degraded, analog, "lived-in" color grade.** Muted, warm-leaning, slightly desaturated except
  where a light source justifies a hot color. Nothing reads as a clean digital render — it reads like
  a photograph of a real, humid, dirty place that happens to have an alien sky.
- **Scale is felt, rarely announced.** The film almost never gives you a hero "look up in awe" beat —
  the giant planet is just *always there*, unremarked, which paradoxically makes it feel more real than
  if characters kept reacting to it.

**The lesson for Ziptide, precisely:** stop treating the sky as a sticker to add variety to (one planet,
some stars, done) and start treating it as **atmosphere the player is standing inside of.** The
celestial bodies matter less than the air between the player and them.

## 2. Design pillars
1. **Occlusion over exposure.** The biggest, most impressive sky object in a world should be glimpsed
   through or past something — canopy, a structure's silhouette, fog banks, a doorway — in its primary
   view from the arrival vista, not floating in a clean empty sky. A totally unobstructed giant reads as
   a screensaver; a partially-hidden one reads as a place.
2. **Atmosphere is a first-class layer, not a fog density slider.** Haze between the player and the
   horizon needs to visibly scatter and soften what's behind it — the edge of a planet through mist
   should blur and warm, not stay a crisp disc on a gradient.
3. **Something is always drifting.** Spores, dust, ice crystals, ash — biome-appropriate particulate in
   the near-to-mid distance sells "you are breathing this air" harder than any amount of skybox detail,
   for the least cost of anything in this doc.
4. **The sky's color is the scene's color.** If the dominant body/nebula is amber, the fog, the
   ambient light, and the horizon haze should all lean amber too — a cosmic backdrop that doesn't touch
   the ground light never feels like it's really there.
5. **Scale is felt, not announced.** No forced "look up" cutscenes. If pillars 1–4 are done right, a
   player noticing the giant is a byproduct of just standing there, not a scripted beat.
6. **Restraint is a signature too.** Not every world needs three bodies and a nebula. `THE_EDGE` (W038)
   working because it's *stark* — near-black, one wall, one crack of light — proves the game already
   understands this. The Prospect bar is about *conviction*, not maximalism; a world can pass it with
   one correct, well-atmosphered choice instead of every knob turned up.

## 3. The technique stack (what's missing from the current system, and the cost of each)
Current `SkyVistaDefinition` gives a gradient dome, up to 3 bodies, a nebula layer, stars, a shell grid,
and light/fog tie-ins — real, working, and underused (only 2 of 12 built worlds push it toward "real
space"; see `WORLD_DATA.md` §4.1). What it's missing is everything that makes a sky feel like *air*
instead of a painted dome. In priority order, cheapest-and-highest-impact first:

| Layer | What it does | Cost (Quest budget: `PerfBudgetAuditRules` — 1 light target/3 cap, 900 renderers target) |
|---|---|---|
| **Horizon haze card** | A single large soft-edged unlit quad (or a few concentric ones) near the horizon, alpha-blended, tinted by the dominant body/nebula color. Makes distant objects look like they're being viewed through real air instead of painted flat on the dome. **Place it at a real, finite world-space distance (tens of meters), not on the infinite-distance skybox** — see §1's stereo-depth note; this is what lets binocular disparity read it as actually nearer than the body behind it. | 1 renderer, 1 unlit material, no lights. Cheapest item on this list — should be default-on everywhere with a body or nebula. |
| **Particulate drift layer** | A small GPU/CPU particle system (biome-keyed: spores, dust, ash, ice) drifting slowly through the near-to-mid field, unlit billboards, no per-particle lighting. **Placed nearest of all three layers (single-digit-to-teens meters)** — the layer stereo depth will sell hardest, since it's the one the player's own parallax crosses fastest. | 1 renderer (1 particle system), ~50–150 particles, zero new lights. Second-cheapest, highest immersion-per-cost item in this whole doc. |
| **Occlusion-aware vista placement** | Not a new asset type — a *rule* for where `WorldExperienceBuilder`'s arrival-vista/POI-facing logic points the camera relative to a world's tallest silhouette (canopy, towers, terrain ridge), so the signature body is naturally partially hidden from the vista's hero angle. | Zero runtime cost — this is an authoring-time discipline, not new code, though it may need a small hook so `SkyVistaLibrary`'s body `direction` can be authored against the same POI/vista-facing data `WorldExperienceBuilder` already computes. |
| **Body edge softening / glow** | Instead of a crisp-edged disc, blur/bloom the silhouette edge of a celestial body so it reads as scattering light through atmosphere rather than a cardboard cutout. Cheapest version: an unlit soft-glow sprite behind/around the existing procedural disc, no new shader complexity. | 1 extra renderer per body with this treatment (only signature-tier worlds need it — see §4). |
| **Light/fog coupling made mandatory, not optional** | `directionalLightColor/Intensity`, `overrideAmbient`/`ambientColor`, and `overrideFog`/`fogColor` already exist on `SkyVistaDefinition` — today they default to off/neutral in almost every authored world. Make "the sky's dominant color reaches the ground" a checklist item (§5), not an optional field nobody sets. | **Zero new cost** — these fields already exist and are already budgeted (reuses the world's existing single directional light; does not add lights). Pure authoring discipline. |
| **Color grade (biome mood curve)** | A light URP color-adjustments Volume per biome family (warm/amber, cold/teal, sickly/toxic, etc.) so the whole scene's temperature — not just the sky dome — matches the world's atmosphere. | **Open perf question, not assumed free** — this project has no existing post-process Volume usage (checked before writing this doc). Needs an actual on-device frame-time test before it's adopted project-wide; do NOT roll this out broadly until Terry confirms it doesn't cost the Quest 2/3 budget. Lowest priority item in this stack for exactly that reason. |

**Nothing in this stack adds a real-time light.** That's deliberate — the budget has room for exactly 1
target / 3 cap total, and the existing system already spends that on the world's single directional
light. Every haze/glow/particulate layer above is unlit or emissive-only by design.

## 4. Authoring tiers (so this is achievable across 68 unbuilt worlds, not just 2)
Not every world can or should get the full stack — that's how "make every sky epic" quietly becomes
"make every sky the same." Three tiers, chosen per-world at authoring time from the chapter seed's own
language (a world whose `Sky:` text is already vivid and specific earns Signature; a world whose text
says "none" or is purely functional stays Interior):

- **Signature** (the "wow" worlds — chapter capstones, first-sight-of-a-new-place beats, arrival
  vistas): full stack — haze card + particulate drift + occlusion-composed body + edge-soft glow +
  mandatory light/fog coupling. Budget: maybe 6–10 worlds across the whole 68, not all of them. W038
  (The Edge), W047 (Architect's Chamber planetarium), W060 (Architect's Tomb), W057 (Transit Void, just
  sharpened this session) are natural candidates already flagged in the storyboard.
- **Standard** (most Exterior/Void/Coastal/Station worlds): haze card + light/fog coupling, mandatory;
  particulate drift where the biome supports it (spore worlds, ash worlds, dust worlds); body/nebula
  as already authored. This is "good sky," not "hero sky" — still never a flat sticker, just not a
  signature set-piece.
- **Interior** (sealed/underground/no-sky worlds): no change — `Sky: none` with a dark gradient and
  maybe a light nebula-glow (as several already do, e.g. W002/W011) is *correct*, not a gap. Forcing
  space into a cave is the opposite mistake.

## 5. The pass/fail rubric (read this before authoring or judging any world's sky)
A vista passes the Prospect bar if the honest answer to all of these is yes:
1. **Is something drifting?** (Spore, dust, ash — anything.) If the air is perfectly still, it isn't air.
2. **Is the horizon hazy, not crisp?** Can you tell there's atmosphere between you and the far object,
   or does it look painted directly onto a flat dome?
3. **Is the signature body at least partially hidden** by something in its primary view (canopy,
   structure, ridge, fog) rather than floating clean in open sky?
4. **Does the sky's dominant color show up anywhere else** — the fog, the ambient light, the ground?
   Or does the cosmic backdrop stop existing the moment you look away from it?
5. **Would a screenshot of just the ground/foreground, with the sky cropped out, still look alien** —
   from the color grade and haze alone? (If the answer is no, the "otherworldliness" is only living in
   the sky layer, which is the exact mistake this whole doc exists to fix.)
A world can fail #3 on purpose (an Interior world, or a deliberate-restraint Signature beat like The
Edge) — the rubric is a prompt for a conscious choice, not a mechanical checklist every field must
tick.

## 6. What to build first, if this moves from plan to code
In order, cheapest-to-prove-out first:
1. **Prototype the haze card + particulate drift on ONE already-built world** (W005 Oxidized Canopy is
   the obvious pick — it's already the game's own "amber jungle moon," closest in mood to *Prospect*'s
   reference, and already has a banded giant + nebula authored). Ship it, get Terry's on-device gut
   check ("does THIS feel different"), before touching anything else.
2. If it lands: extend the haze/particulate pair to the other 11 built worlds per the tiers in §4.
3. Only after that: the occlusion-composition hook into `WorldExperienceBuilder`'s vista-facing logic
   (needs care — it's touching the arrival-vista system another pass built, not a from-scratch add).
4. Color grading Volume stays parked until a dedicated on-device perf test says it's free — do not
   fold it into the same pass as 1–3.

## 7. What this doc is not
Not a mandate that every world becomes an amber jungle moon — *Prospect* is the technique reference
(atmosphere, occlusion, grounded color, drift), not a palette to reskin everything into. W038's stark
near-black wall-and-crack and W060's planetarium ceiling are exactly as "Prospect" as W005's canopy,
because the discipline (haze, restraint, grounded light-coupling, felt scale) is the same even though
the palette is opposite. The rubric in §5 is deliberately technique-based, not color-based, for exactly
this reason.

**Not the same target as the "No Man's Sky" bar in `SPRINT.md`, either** — that's about *terrain/biome
variety* (procedural worlds that don't repeat), a different axis from this doc's *atmosphere/sky feel*.
A world can hit both bars at once (varied terrain, hazy grounded sky) without them being in tension —
they're answering different questions ("does the ground feel distinct?" vs. "does the air feel real?").

**Not a soundscape spec either, and that's a real gap, not a non-issue.** *Prospect*'s alien-place
feeling is at least half wind, insects, and atmospheric hum — this doc only covers what the eye sees.
`AudioDirector.cs` already exists for looping ambience; whether it currently varies by biome the way
this doc asks the SKY to is worth checking before anyone assumes the sensory picture is complete once
this plan ships. Flagging it here so it doesn't get silently forgotten, not scoping it into this pass.
