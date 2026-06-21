# City Design — making Ziptide's worlds read as real places

**What this is:** a practical playbook pairing city-design principles with **concrete changes to our own
generator** (`CityLayoutDefinition` + `CityBuilder` + per-world `ScenePatcher*`), prioritized quick-wins
first. Born from a code audit + research after Toxic City "looked like it makes no sense" on device.

> **The core reframe:** it's **not mainly the missing textures** — it's **massing, scale, color-zoning,
> and ground floors**. Texturing bad massing is lipstick on a pig. Get the graybox reading as a city
> *first*; ~80% of "believable city" is spatial composition, ~10% ground-floor detail, ~10% textures.

---

## Why ours reads wrong today (audit → root causes)
Every issue below is in `CityBuilder.cs` / the default `ScenePatcherToxicCity` layout:
1. **Uniform boxes + random heights.** Buildings are plain cubes; height = `base * (0.7–1.5 random)`
   (noise, not hierarchy). No setbacks, no rooflines. → reads as chaos, not a skyline.
2. **No ground floor / human scale.** Facades are blank base-to-roof; **no doors, storefronts, awnings,
   stairs.** The street level is what sells a city, and it's absent.
3. **Invisible color zoning.** `building1` (0.30,0.31,0.34) vs `building2` (0.24,0.26,0.30) differ ~3% →
   the whole city reads as one dead gray.
4. **Streets read as ramps.** Connections are flat slabs (no curb/sidewalk/width-feel); the **25% random
   building gaps** make "streets" look accidental, not planned.
5. **Cramped proportions.** 16–24 m districts, 5–8 m streets, 9–15 m buildings → too tight to feel urban.
6. **Landmarks don't pop.** Crane/towers use the **same box + same material** as everything → no anchors
   for navigation. Distant skyline = 26 random cubes (noise).

## The 6 principles that matter most → the change in OUR system

### 1. Legibility (Lynch: paths · edges · districts · nodes · landmarks)
A player should navigate by memory, no minimap. Give every world: distinct **paths** (street widths that
differ), readable **district** identity, **node** plazas at junctions, and **landmarks** you can see from
far. → *In our data:* we already have `districts`/`connections`/`landmarks`/`heroBuildings` — the fix is
using them with intent (below), not adding systems.

### 2. Height hierarchy, not random noise
Step heights **intentionally toward landmarks** (low at edges → tall at the civic node), podium+tower for
big buildings. → *CityBuilder:* replace the `0.7–1.5` random multiplier with a height driven by
`heightTier` + **distance-to-nearest-landmark** (closer to a landmark = taller, framed). Add a simple
**podium** (a wider, shorter, different-color base cube) under tall facades.

### 3. The ground floor (human scale) — biggest "lived-in" win
Split every facade into a **ground band (~0–3 m)** and the **upper floors**. Ground band gets: a real
**door** (recessed 1.2×2.4 m), a **storefront** color/strip, optional **awning** box; upper floors get the
window grid. → *CityBuilder:* split `AddFakeDepthWindows` into `AddGroundFloor()` + `AddUpperWindows()`.
Add a per-building **door** at street-facing edges. Scatter **human-scale reference props** (a 3 m
lamppost cylinder every ~10 m along connections) — nearly free, massive scale calibration.

### 4. Color & material **zoning** (the #1 quick win — reads before textures)
Make districts distinguishable by **value + hue** in graybox. → *Data:* (a) fix the palette so
`building1`/`building2` actually differ; (b) **use `DistrictDef.paletteOverride`** (already exists, unused)
to give each district a hue: Shipyard=cold steel, Plaza=pale civic, Market=warm, CanalRow=toxic-green,
Slum=rust. Keep **value hierarchy**: landmarks ~85% value (near-white + the yellow `accent`), default
~50%, alleys/recesses ~25%. One accent color, used sparingly, only on landmarks/gates/objectives.

### 5. Street hierarchy + sightline termination
Three street widths that *feel* different (boulevard ~16–24 m, street ~10 m, alley ~4–6 m) and **every
main street should end on a landmark** (the player always sees the next goal). → *Data:* set
`ConnectionDef.width`/`kind` per tier deliberately; place a tall distinct landmark at the end of the spine
so it terminates the view. **Remove the 25% random gap** — put streets at planned intervals. Make
intersections **node plazas** (widen + central marker).

### 6. Proportion, silhouette & "lived-in"
Loosen cramped proportions (wider main boulevard, a real central plaza). Add **roofline variation** (small
boxes on roofs — HVAC/tanks/antennae; vary roof height ±0.5–1 m) so the silhouette has peaks/valleys.
**Repetition with variation:** a few base silhouettes (tall/wide/narrow/courtyard) rotated/mirrored/offset
beats identical boxes — and beats random. Mark **signage zones** (accent rectangles over doors) = implied commerce.

## Prioritized actions (quick wins first — mostly data / small `CityBuilder` edits)
**P0 — do first (cheap, huge):**
- Fix `building1` vs `building2` to clearly differ; set **`paletteOverride` per district** (hue zoning) + value hierarchy.
- Make **landmarks distinct** (accent color / near-white; bigger silhouette) so they anchor navigation.
- Replace **random height** with `heightTier` + distance-to-landmark **stepping**; replace **25% random gaps** with planned streets.

**P1 — the "it's a city" pass:**
- **Ground floor**: split facades, add a real **door** + storefront band per building; add **lamppost/prop** scale refs along streets.
- **Sightline termination**: ensure the main spine ends on a landmark; add 1–2 **node plazas**.
- Loosen **proportions** (boulevard width up; one real plaza).

**P2 — polish before/with textures:**
- Roofline clutter + roof-height variation; podium+tower on big buildings.
- A few base silhouettes rotated/mirrored/offset; signage-zone accents; subtle per-building value wear.
- LOD/occlusion pass for Quest (background = silhouettes only) when density grows.

## Graybox → art rule (don't skip)
**Validate the graybox before texturing.** Walk it with no minimap and check: can I navigate by landmarks?
do heights read as hierarchy (not noise)? do streets feel like streets, districts feel distinct, ground
floors feel active, scale feel right in-headset? If any "no" → fix massing/color/scale, **then** texture.

## Lane / next step
City geometry = **T-Dog's lane** (`CityBuilder`/`ScenePatcher*`, on-device-verified). This doc is the
spec; suggested order = the P0 list (a few `CityBuilder` lines + palette/`paletteOverride` data) for an
immediate readability jump, even pre-texture. Architect can help on any **data-schema** additions
(`DistrictDef` fields for ground-floor/door/roof control) since that's the data lane.

## Sources (key)
Kevin Lynch *Image of the City* (paths/edges/districts/nodes/landmarks) · The Level Design Book
(massing/wayfinding/metrics) · Bethesda GDC modular design (repetition-with-variation/kitbash) · Jane
Jacobs "eyes on the street" (ground-floor activation) · Meta VR performance guidelines (LOD/draw budget).
