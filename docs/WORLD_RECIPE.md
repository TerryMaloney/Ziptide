# WORLD_RECIPE.md — how to author a GOOD world (the mid-level-LLM handbook)

> **Who this is for:** any future session — including a much smaller model — asked to add or improve a
> ZIPTIDE world. You do NOT need taste to succeed here. You fill in tables; the build machinery turns
> them into a world; the **quality gates fail the build** if the result would be bland. If your world
> builds green, it meets the bar by construction.
>
> **Never edit the builders** (`WorldExperienceBuilder`, `WorldPoiBuilder`, `WorldDressingBuilder`,
> `CityBuilder`, `ShipHullBuilder`) to make one world better. Worlds are DATA. If a builder seems to
> need a change, stop and flag it in `docs/HANDOFF.md` instead.

## THE FRONT DOOR: edit the spec, not the code (ARCHITECTURE V2 Q1)

Once `docs/worldspecs/*.spec.json` exist (Terry's one-time export — runbook §2k), **a world's spec
file is its single editable truth document**. To change a world: edit its `.spec.json` (identity,
sky, experience terrain/vista, POI table, districts, creatures, hazards, economy, flags), commit,
push — `WorldSpecCompiler.CompileAll()` runs in every build, validates it against the REAL content
registries (`WorldSpecValidator`, stable error codes), and fans it out to the layout + pack assets.
To add a world: copy an existing spec, change `sceneName`/`cityId`/seed, fill the tables below.
**Until §2k runs**, the fallback authoring path is `WorldLayoutLibrary` (create-only C# seeds) —
same data, same rules; everything below applies to both paths.

## The pipeline you are feeding (all automatic on every build)

```
docs/worldspecs/<World>.spec.json   ← THE editable truth (after §2k; validated, compiled)
   or WorldLayoutLibrary seed       ← the pre-§2k fallback (create-only C# author)
        │  CityLayoutDefinition asset (the world's DNA: identity, sky, districts,
        │  experience block, POIs, hazards, creatures, shipyard, buildingStyleId)
        ▼
WorldStubGenerator                  ← scene + pack + spawn + markers + gardens/sockets sync
CityBuilder                         ← districts/connections/shipyard geometry
  ├─ WorldExperienceBuilder         ← P1a/b: biome terrain + cliff bowl + arrival vista
  ├─ WorldPoiBuilder                ← P1c: one staged pocket per POI + poi_<id> markers
  ├─ BuildingBuilder                ← V2 H1: real buildings on partitioned lots (opt-in per
  │                                    district via buildingStyleId; door-on-street law)
  └─ WorldDressingBuilder           ← P1d/e: route cairns + biome scatter (masked)
WorldJobLibrary.SpecFor()           ← you write ONE contract spec here (routes through POIs)
WorldAuditRunner                    ← THE QUALITY GATES — blockers FAIL the build
        ▼
APK — the world ships
```

## Step 1 — the experience block (pick from tables, don't invent)

In your world's `.spec.json` experience block (or, pre-§2k, the `WorldLayoutLibrary` seed /
`EnsureExperienceAuthored` entry), set:

| Field | Pick | Rule of thumb |
|---|---|---|
| `biome` | `Dunes` · `Mesas` · `Canyon` · `CavernFloor` · `TideFlats` | Match the WORLD_DATA fiction's LANDFORM, not its mood (mood = sky + colors) |
| `worldRadius` | **250–400** | Gate fails < 200. Bigger = emptier unless you raise POI count too |
| `heightAmplitude` | 8–22 | Flats 8–10 · rolling 14–18 · dramatic 20–22 (builder clamps slopes walkable) |
| `groundColor` | any | Make neighboring-chapter worlds visibly different from each other |
| `vista` | `GateSpire` · `Wreck` · `Monolith` · `CrystalForest` · `ArchRing` | The world's "postcard". Pick what the story beat would leave behind |
| `vistaDirection` | XZ vector | Where the player LOOKS on arrival — vary it between worlds |
| `vistaDistance` | 140–200 | Must be < `worldRadius * 0.8` so the hero stands inside the bowl |
| `vistaHeight` | 40–80 | Under 40 stops reading as monumental |
| `vistaColor` / `vistaAccentColor` | any | Accent = the glow (ring/crystals/beacon); echo it in the sky |

Biome quick-map: dead/dry → Dunes · stepped/windy → Mesas · carved/overgrown → Canyon ·
enclosed/hushed → CavernFloor · wet/still → TideFlats.

## Step 2 — the POI table (where the gameplay lives)

A world = **5–9 POIs, ≥3 distinct verbs, exactly ≥1 StoryAnchor**. The default: leave `kit.pois`
empty and the **standard 8-POI ring** seeds itself (story on the vista sightline, camps, grove,
works, ruin, cave behind you, berth). Hand-author only when the fiction demands a custom shape.

| Verb (`PoiType`) | What the player does there | Builder gives you |
|---|---|---|
| `CombatCamp` | fight (creatures scale with `tier`) | cover ring + watch mast + live fauna |
| `HarvestGrove` | plant → grow → harvest (P3 garden) | 6 planters, auto-wired to GardenPlotRuntime |
| `MachineSite` | repair contracts + PAY-TO-BUILD extractor | housing + plinth, auto-wired BuildSocket |
| `RuinCache` | loot / collectibles | broken walls + glowing cache pedestal |
| `CaveSecret` | optional exploration reward | rock shell with one entrance |
| `StoryAnchor` | THE world's WORLD_DATA beat, staged | dais + pylons + sky-high light beacon |
| `TravelBerth` | leave | marker only (berth/station already built) |

Placement rules (the standard ring obeys all of these — copy its shape):
- Everything inside `worldRadius * 0.75` (the cliff rim starts at 0.82).
- StoryAnchor ON the vista sightline (~60% of `vistaDistance`) so walking to the story shows the hero.
- CaveSecret far from the route, ideally behind the spawn — secrets reward turning around.
- Spread POIs so the nearest-neighbor route is LONG (the play-minutes gate counts walking).

## Step 3 — the contract (`WorldJobLibrary.SpecFor`)

Copy an existing spec (W002 is the full-verb reference). Rules:
- Route with **`GoPoi("<poiId>", "friendly label")`** — never raw coordinates in experience worlds.
- End every contract at `"story"` — the last step stages the beat.
- Place physical objects with `PickupAtPoi` / `MachineAtPoi` / `MineAtPoi` (offsets are FROM the
  POI's pad). A machine at one POI with its part at another = the walk is the job.
- Gate with `flagsRequired` (previous world's completion flag) and grant `flagsGranted`
  (`ZiptideFlags` constants only — raw strings are how typo bugs shipped).
- RILL: add one `Enter` line per world + `Flag` reactions in `RillLineAuthor` (≤ ~90 chars, her
  register per `RillState`: Dormant terse → Stirring questions).

## Step 4 — build and let the gates judge you

Run `Ziptide > Worlds > Generate All Layout Worlds`, then `Ziptide > Audit > Run Audit (All Scenes)`
(CI/APK runs both automatically). The gates:

| Gate (blocker = build FAILS) | Bar | Fix |
|---|---|---|
| `WORLD_TOO_SMALL` | radius ≥ 200 | raise `worldRadius` |
| `TERRAIN_MISSING` | terrain built | regenerate worlds |
| `NO_VISTA_LANDMARK` | hero exists | set `vista` ≠ None, regenerate |
| `POI_COUNT_LOW` | ≥ 5 POIs | add POIs (or clear `pois` to re-seed the ring) |
| `VERB_VARIETY_LOW` | ≥ 3 verbs | mix POI types |
| `STORY_ANCHOR_MISSING` | ≥ 1 StoryAnchor | add one |
| `EST_PLAY_MINUTES_LOW` | ≥ 8 min (PoiQuality heuristic) | more POIs, higher tiers, wider spread |

The heuristics live in `PoiQuality.cs` (pure, pinned by `PoiQualityTests`). Tune WORLDS to pass —
never the math.

## Worked example — W005 "Oxidized Canopy" (read this as the template)

Fiction (WORLD_DATA): a forest canopy growing through dead machines; Mara's first contract; spores.
1. **Experience:** biome `Canyon` (an overgrown gorge), radius 300, amplitude 22 (dramatic),
   ground oxidized copper-green `(0.36, 0.44, 0.30)`, vista `CrystalForest` (reads as canopy giants)
   at 160m, height 58, accent bloom-green. Spore hazard zones + swarm creature zone already on the
   layout carry the M2/M3 mechanics.
2. **POIs:** standard ring — for W005 the grove is the fiction's heart, so its contract leads there.
3. **Contract:** `GoPoi("grove")` → `Drones(4)` → `GoPoi("works")` (the scrubber) → `GoPoi("story")`
   (the overgrown gate). Rewards: credits 110 + spore 6.
4. **RILL entry line:** "The canopy is growing through the machines. Do you think it minds us fixing
   them?" — Stirring register (questions), already authored.
5. Build → audit green: 8 POIs, 7 verbs, StoryAnchor present, ~16 estimated minutes. Ships.

## Style checklist (the taste, mechanized)

- [ ] Could a player screenshot the arrival view and know WHICH world it is? (vista + ground + sky)
- [ ] Does the contract's final step pay off the world's story beat at the StoryAnchor?
- [ ] Is at least one thing OPTIONAL and hidden (the cave, an off-route ruin)?
- [ ] Do two adjacent-chapter worlds share a biome? If yes, change one.
- [ ] Does anything here contradict `docs/storyboard/WORLD_DATA.md`? Fiction wins — retune data.
- [ ] Zero raw scene-name/flag strings — `ZiptideConstants` / `ZiptideFlags` only.

## What you may NOT do (hard lines)

- Never hand-edit `.unity`/`.prefab` YAML. Never touch the rig, travel, input, or `_Boot`.
- Never weaken a quality gate or a test to get green. If a gate seems wrong, HANDOFF + stop.
- Never `SceneManager.LoadScene` — `TravelCoordinator.TravelTo` is the only door.
- CI red = stop shipping C#. Warn Terry loudly (see CLAUDE.md workflow-integrity rule).
