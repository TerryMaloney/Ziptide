# W001 IDENTITY DECISION BRIEF — THE MOSS / TOXIC CITY

**Status:** RECOMMENDED CANON — awaiting Terry’s explicit approval after M0  
**Written:** 2026-07-24  
**Scope:** documentation/content reconciliation only; no runtime, scene, WorldSpec, asset or APK authorization.

---

## 1. Recommended decision

Lock the hierarchy as:

- **World ID:** `W001`
- **World display name:** **The Moss**
- **Planetary/body identity:** habitable toxic moon of a dominant ringed gas giant
- **First playable region / scene identity:** **Toxic City**
- **Current working scene label:** `W001_ToxicCity` where compatible with existing first-hour data
- **Later orbital view:** `space_moss_orbit`

This means “The Moss” and “Toxic City” are not competing names.

**The Moss is the world. Toxic City is the first settlement/region the player visits on it.**

---

## 2. Evidence

### Celestial/world authority

`docs/design/celestial_system_catalog.json` already marks:

- `worldId: W001`;
- `worldName: The Moss`;
- `decisionStatus: proposed-canon`;
- body role `W001-world-and-launch-origin`;
- a habitable toxic moon orbiting a ringed giant;
- one warm star;
- one visible sibling grey moon;
- dusk baseline;
- amber-toxic atmosphere;
- `space_moss_orbit` as the corresponding orbital scene.

`docs/design/CELESTIAL_SYSTEM_CANON.md` develops the same identity and states that the ground and orbital views must share one celestial truth.

### Region/visual authority

`docs/project_art_plan/TIER_C_CONCEPT_QUEUE.md` establishes the W001 Toxic City region as:

- toxic Venice/canal settlement;
- leaning verticality;
- inhabited decay;
- colossal leaning tenement tower as the hero landmark;
- harbor/berth district;
- toxic canals;
- tidal flats, wreck fields and stilt piers;
- existing approved six-scale concept kit in `concepts/toxic_city_kit/`;
- measured implementation reference `CITY_VISUAL_SPEC.md`.

### Creature authority

`docs/project_art_plan/CONCEPT_ART_NEXT_30.md` assigns the **Canal Stalker** to W001: an amphibious toxic-canal predator with armored plating, sickly flesh, multiple limbs and bioluminescent hazard markings.

### Current implementation truth

`docs/STATE_OF_THE_GAME.md` says:

- the canonical W001 scene slot is empty;
- ToxicCity currently stands in;
- no authoritative W001 WorldSpec has yet been committed.

Therefore the identity is well developed in design/art data but has not yet been converted into the authoritative world-production input.

---

## 3. Why this hierarchy is stronger

### It preserves both developed bodies of work

- “The Moss” carries planetary, story, sky and space identity.
- “Toxic City” carries settlement, architecture, route, job and creature identity.

Nothing needs to be discarded or renamed merely to resolve the apparent conflict.

### It scales beyond the first scene

The Moss can later contain:

- Toxic City;
- tidal outskirts;
- wreck flats;
- canal infrastructure;
- other settlements or wild biomes;
- surface launch sites;
- orbital missions;
- the sibling-moon route.

Calling the whole world Toxic City would make later regions awkward. Calling the first region only The Moss would discard a strong memorable city identity.

### It supports the game’s larger fantasy

The player’s first world establishes:

- grounded salvage culture;
- ordinary people surviving beneath impossible celestial scale;
- toxic ecology that is dangerous but useful;
- repair and infrastructure as real heroism;
- the relationship between ground exploration and later ship/space play.

---

## 4. W001 identity pillars

### 4.1 Emotional purpose

W001 should feel:

- inhabited rather than abandoned;
- harsh but not hopeless;
- wet, industrial and alive;
- melancholy, beautiful and practical;
- small human-scale survival beneath an enormous cosmic landmark.

The player should leave thinking:

> “This place is struggling, but people live here—and I made one part of it work again.”

### 4.2 Visual identity

Mandatory visual anchors:

- dominant ringed giant occupying a major part of the dusk sky;
- warm star direction consistent with later orbit;
- sibling grey moon visible where authored;
- amber-green toxic haze;
- dark canal water;
- leaning stacked architecture;
- patched salvage construction;
- warm lanterns and work lights;
- crest-cyan only for tide/Architect technology;
- leaning tenement tower as far navigation landmark;
- berth district as arrival and return anchor.

### 4.3 World grammar

Foreground:

- wet docks;
- pipes, cables and patched machinery;
- canal edges;
- readable hand-scale job props.

Playable midground:

- berth district;
- job source;
- repair route;
- traversal crossing;
- canal-stalker territory;
- seed/biological source;
- optional story trace.

Background:

- leaning tower;
- layered tenements;
- tidal flats and wreck fields;
- ringed giant, moon and toxic sky.

---

## 5. Recommended first-hour use

### Arrival

The player arrives in the berth district.

The first composition should show:

- a dependable return berth close behind/adjacent;
- Toxic City stretching into the canal network;
- the leaning tower as the dominant land landmark;
- the ringed giant as the dominant sky landmark;
- one independent skiff, crane, light or distant silhouette proving the city exists beyond the objective.

No forced camera motion.

### Primary job

Recommended job family:

**Repair a canal pump, filtration relay or service machine that supports the inhabited district.**

The existing physical sequence remains:

1. accept job;
2. scan fault;
3. reach work site;
4. remove access panel;
5. fetch/seat replacement part;
6. press power;
7. observe water/light/machinery consequence;
8. collect output or salvage;
9. receive payout;
10. leave the system visibly operating.

The exact machine identity should come from existing W001 pack/job authorities and the future WorldSpec rather than a new runtime class.

### Signature creature

Recommended species:

**Canal Stalker**

Encounter requirements:

- foreshadow through water movement, sound, markings or a distant silhouette;
- safe observation before aggression;
- readable bioluminescent tell;
- non-lethal counter using the environment and an already taught tool;
- conclusion redirects/disables the creature or changes access rather than killing it;
- encounter connects to canal ecology and repaired infrastructure.

The exact behavior remains the creature lane’s responsibility.

---

## 6. Growing/invention fit

### Recommended first seed family

**Filter family** is the strongest default for The Moss.

Reasoning:

- the world is explicitly toxic;
- the verb is understandable to children;
- it provides useful function without replacing mining/salvage progression;
- plant biology naturally belongs near canals, filtration machinery or contaminated growth;
- a filter capability can unlock optional contaminated routes without making ordinary survival depend on a garden timer.

### Recommended W001 surfacing

- find one Filter-family seed, pod or membrane near the repair route;
- scan/focus reads its name and one-line function;
- collection advances one visible invention card;
- the main job provides the industrial frame/component;
- return to W000 reveals the starter plant has matured;
- one deterministic recipe produces the first invention.

### Recommended first invention

**Moss Filter / Salvage Filter Rig** — working concept name only.

Functional sentence:

> “Filters toxic air and mist so you can enter contaminated spaces.”

Design rules:

- occupies one existing belt socket;
- does not provide a flat damage increase;
- opens optional or later routes rather than making the first job impossible without it;
- recipe uses one industrial base, one Filter trait input and at most one catalyst;
- campaign result is deterministic;
- icon, silhouette, visible name, spoken name and one-line function are required;
- useful in at least two later worlds/contexts.

Alternative if the final job is power-grid-centered rather than contamination-centered: Conductive Tether. The world identity still remains The Moss/Toxic City.

---

## 7. Story fit

W001 should introduce ordinary life under pressure before escalating the larger mystery.

Strong story-object locations:

- Dockmaster’s post;
- repair ledger;
- prior crew/salvager trace;
- berth manifest;
- object recovered from a canal or wreck;
- RILL reaction to an apparently mundane record that touches its missing memory.

The first story object should:

- be physical;
- remain short;
- not block the job;
- establish one person/place detail;
- give RILL a distinct reaction;
- set or advance an existing story flag rather than creating a parallel lore system.

Exact story text remains the story lane’s decision.

---

## 8. Audio identity

The existing provisional ID `aud.ambience.w001_toxic` fits Toxic City as the first region.

The world’s audible identity should combine:

- wet/hollow canal ambience;
- distant industrial pumps;
- sparse skiff/crane activity;
- warm inhabited practical sounds;
- localized toxic drips/wind;
- creature presence readable before line of sight;
- clear contrast with the W000 ship hull.

The event ID may remain region-specific while the wider Moss world later gains additional ambience profiles.

---

## 9. Scene/data naming recommendation

Use separate identity fields:

| Layer | Recommended value |
|---|---|
| Stable world ID | `W001` |
| World display name | `The Moss` |
| Region ID | `toxic_city` |
| Region display name | `Toxic City` |
| Working scene | `W001_ToxicCity` if migration cost is low; otherwise existing scene alias mapped through data |
| System ID | `moss_system_working` until final system name |
| Body ID | `moss_world_body` until final body name |
| Orbit scene | `space_moss_orbit` |

Generic systems consume stable IDs/data and must not infer world identity from a scene string.

---

## 10. Remaining Terry decisions

Approving this brief would lock:

1. W001 world = The Moss.
2. First region = Toxic City.
3. The Moss is a moon of the ringed giant.
4. One warm star for this system.
5. Sibling grey moon remains a future visit candidate.
6. Leaning tenement tower is the primary ground landmark.
7. Canal Stalker is the default signature-creature candidate.
8. Filter is the default first biological family/invention direction.

Still intentionally open:

- final names for the giant, star, sibling moon and system;
- exact sun color, disc size and keeper-derived bearings;
- final city district route;
- exact repair machine and job fiction;
- exact creature counter;
- exact story object/text;
- final invention name and model;
- whether existing ToxicCity scene is migrated, replaced or regenerated as W001.

---

## 11. Implementation boundary

No implementation follows automatically.

After M0 passes and the freeze is explicitly lifted:

1. Terry approves/edits this identity hierarchy.
2. Story/world owners close the remaining fiction decisions.
3. The model-band packet changes from review draft to execution authority.
4. An authoritative W001 WorldSpec is created using W001 world identity plus Toxic City region data.
5. Existing authors generate the scene.
6. ToxicCity stand-in hard-coding is removed only where the new data route proves it unnecessary.
7. Every visual/interaction decision receives automated and device evidence.

W002 remains the proof that these identities and systems are data-driven rather than W001-specific code.
