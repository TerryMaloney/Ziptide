# PROMPT TEST 09 — THE CATCH: orbital infrastructure, debris and the Overrun

**Status:** concept-generation pack. Docs-only; authorizes no runtime, scene, prefab or APK change.
**Governed by:** `CONCEPT_ART_PROMPT_PLAYBOOK.md` (prompt anatomy §2, canonical template §3, visual
constants §4) · `PROXY_CONTRACTS.md` (what the built proxy must carry) ·
`CONCEPT_TO_BUILT_PIPELINE.md` (measured spec → Forge → comparison → device verdict).
**Design authority:** `docs/design/THE_CATCH.md` — read §2 before generating. Every object here is
part of ONE machine, and the prompts only work if the generator is told what the machine does.

---

## 0 · WHY THESE SIX

Terry, 2026-07-29: *"the ring lamp chase doesn't make a lot of sense… we need the rings to have some
functional look and use that maybe a spacefaring civilization would have. This game doesn't just
need to be fun, it's got to make sense."*

The answer (THE_CATCH.md): the Moss throws cargo to orbit on a **mass driver**, so orbit needs
something to **catch** it — a line of electromagnetic arrestor rings on the pods' arrival arc.
Everything below is a part of that system or a consequence of it failing. Generated separately they
are six props; generated as one machine they are a place with a history.

| # | Asset id | Role in the machine |
|---|---|---|
| 1 | `catch_ring_mk1` | the arrestor gate — the hero object |
| 2 | `cargo_pod_mk1` | what the Catch exists to catch (and what the wrecks used to be) |
| 3 | `ring_tender_drone_mk1` | maintains the rings; challenges unlogged traffic |
| 4 | `orbital_debris_kit` | the uncleared junk that makes the swept lane worth flying |
| 5 | `overrun_hauler_wreck` | the big dead thing past the last gate — where THE FIND sits |
| 6 | `the_throw_mass_driver` | the ground end, on Toxic City's horizon |

**Generate in this order.** 1 and 2 establish the construction language; 3–6 inherit it.

---

## 1 · CATCH RING — `catch_ring_mk1`

```text
SUBJECT: orbital cargo arrestor ring ("catch ring"), single free-floating structure
GAMEPLAY FUNCTION: electromagnetically brakes an incoming cargo pod; the player flies through the
  bore as the corridor is the only debris-swept volume in orbit
PLAYER MOMENT: first sight of the corridor from the dock — five of these receding into the dark
SCALE / ERGONOMICS: ~12 m clear bore (four cargo pods wide); grab rungs and a drone perch present at
  human scale so the opening reads big
DOMINANT SILHOUETTE: open lattice torus, NOT a solid ring — radiator fins break the outer circle
FAMILY GRAMMAR: Salvage / ToxicIndustrial heavy infrastructure — bolted plate, exposed truss,
  serviceable modules, stencil-and-chevron signage
MATERIAL / COLOR: grey-brown anodised alloy, oxidised steel, dull orange-brown primer through worn
  paint; amber lamps only; NO cyan (cyan is the tide, not human hardware)
WORLD CONTEXT: The Roads, orbit above the Moss; built by a guild that has since shrunk away
PRESENTATION FORMAT: four-view turnaround sheet — hero 3/4, straight-on through the bore, side
  profile showing fins, close detail of one coil housing + numeral collar
PROTECTED FEATURES: (first pass — none)
VARIABLES TO EXPLORE: truss density; fin count and length; coil housing proportion; collar placement
AVOID: neon hoop, energy field, chrome, holograms, symmetry-for-beauty, functionless greebles
```

> **Prompt —** Industrial concept art turnaround sheet of an **orbital cargo arrestor ring** from a
> gritty working-class science-fiction setting. Hard-surface industrial design, believable
> engineering, **not sleek and not neon.**
>
> A free-floating ring in space, roughly **12 metres across the open bore**, built as an **open
> lattice truss torus** with visible girders and cross-bracing — not a solid donut. Mounted around
> the inner face are **twelve chunky rectangular electromagnet coil housings** with ribbed cooling
> casings; these are the working parts and should look heavy, bolted and serviceable. Standing off
> the outer rim, pointing radially outward, are **flat rectangular radiator fins**, heat-discoloured
> toward their roots. A ring of **small amber lamp fixtures** runs around the inner rim — small,
> jewellery on a machine, not the machine. One flat collar plate carries a **huge stencilled numeral
> "3"** and faded black-and-yellow hazard chevrons. A short **service boom** sticks out with grab
> rungs and a small drone perch.
>
> **Condition:** decades old, still functional but under-maintained — micrometeorite pitting,
> sun-faded paint, streaked grime, a welded rectangular patch plate over one damaged coil housing,
> and one coil segment blown out and scorched black.
>
> **Palette:** grey-brown anodised alloy, oxidised steel, dull orange-brown primer showing through
> worn paint, amber lamps. No chrome, no glowing energy fields, no holograms.
>
> **Lighting:** hard single warm key light, deep black shadows, faint cool bounce from a nearby
> ringed gas giant. Background: black space, dense starfield, faint galactic band.
>
> **Sheet layout:** four views on one image — large three-quarter hero, straight-on front view
> through the bore, side profile showing the fins, and one close detail of a coil housing with its
> cooling ribs and the stencilled numeral collar.
>
> **Style:** grounded industrial concept art in the spirit of Chris Foss and Ron Cobb working
> drawings — plausible hardware an engineer could sign off. Weathered, functional, unglamorous.
> Neutral grey sheet background, orthographic-leaning views, clear silhouette. No watermark, no text
> labels beyond the stencilled numeral.

---

## 2 · CARGO POD — `cargo_pod_mk1`

The object that explains the whole system. It should be obvious that this fits through that ring.

```text
SUBJECT: sealed unpowered cargo pod, thrown to orbit by a mass driver and caught by the Catch
GAMEPLAY FUNCTION: none directly — it is the WHY. Intact ones sit at the ground terminus; burst ones
  are the most common debris in the Overrun
PLAYER MOMENT: recognising, in the junk, the same object the rings were built to stop
SCALE / ERGONOMICS: ~3 m long, ~2.5 m diameter — one quarter of the ring's bore
DOMINANT SILHOUETTE: blunt capsule with a heavy ferrous drive-band collar at its waist
FAMILY GRAMMAR: same Salvage/ToxicIndustrial language as the ring — stencils, chevrons, bolted plate
MATERIAL / COLOR: galvanised grey, ore-dust staining, one amber transponder lamp (dead on wrecks)
WORLD CONTEXT: the Moss's only export method
PRESENTATION FORMAT: three views on one sheet — intact pod 3/4; same pod burst open and tumbling
  with ore spilling; end-on view showing the drive-band collar
PROTECTED FEATURES: drive-band collar must read as ferrous and magnetically gripped
VARIABLES TO EXPLORE: collar depth; ribbing; cargo-door pattern; stencil layout
AVOID: engines, thrusters, windows, cockpit, fins — this thing is thrown, it does not fly
```

> **Prompt —** Industrial concept sheet of an **unpowered sealed cargo pod** thrown into orbit by an
> electromagnetic mass driver. Blunt capsule, roughly **3 metres long**, galvanised grey plate with
> ore-dust staining, bolted seams, stencilled ID numbers and faded hazard chevrons. Around its waist
> is a **heavy ferrous drive-band collar** — the ring the launch track and the orbital arrestor rings
> grip magnetically — visibly scuffed and burnished from use. One small amber transponder lamp.
> **Critically: no engines, no thrusters, no windows, no cockpit, no control fins.** It is freight,
> not a ship.
>
> Three views on one neutral sheet: intact pod in three-quarter view; the same pod burst open and
> tumbling with crushed ore spilling out of a split seam; an end-on view showing the drive-band
> collar and cargo-door pattern.
>
> Grounded used-future industrial style, hard single key light, black space background. No chrome, no
> glow beyond the single amber lamp, no watermark.

---

## 3 · RING-TENDER DRONE — `ring_tender_drone_mk1`

```text
SUBJECT: orbital maintenance drone that station-keeps on the catch rings
GAMEPLAY FUNCTION: the space leg's only "enemy" — wakes when unlogged traffic enters the corridor,
  is stunned non-lethally, then becomes salvage the player flies close to collect
PLAYER MOMENT: it notices you — the eye lights and the bob doubles — before anything else happens
SCALE / ERGONOMICS: ~1.6 m across, roughly human-torso sized, readable at 40 m
DOMINANT SILHOUETTE: squat service body with folded manipulator arms and one dominant sensor eye
FAMILY GRAMMAR: same guild hardware as the ring — it should look like it was issued WITH the rings;
  bolted plate, amber lamp language, stencilled unit number
MATERIAL / COLOR: alloy grey-brown, amber running lights, ONE red-orange sensor eye (the only warm
  saturated element); powered-down state drains to dead grey
WORLD CONTEXT: still doing a job for an employer that no longer exists — bureaucratic, not hostile
PRESENTATION FORMAT: four-panel state sheet on one image — dormant (eye dark, arms stowed); woken
  (eye lit, arms half-deployed); evading (listing, thruster puffs); disabled/salvage (dark, listing
  over, one panel hanging)
PROTECTED FEATURES: single dominant eye; arms must read as TOOLS, never weapons
VARIABLES TO EXPLORE: arm count and stow geometry; thruster placement; body proportion
AVOID: guns, missiles, blades, aggressive animal silhouette, insect/spider read, glowing energy
```

> **Prompt —** Concept state sheet for an **orbital maintenance drone** — a "ring-tender" that
> services electromagnetic cargo-arrestor rings in orbit. Squat service body roughly **1.6 metres
> across**, bolted alloy plate in grey-brown, stencilled unit number, small amber running lights, and
> **one dominant red-orange sensor eye**. It carries **folded manipulator arms with clamps, a
> spot-welder and a grabber — tools, unmistakably not weapons.** Cold-gas thruster nozzles, not
> engines. It should look like the same guild's hardware as the rings it tends: same paint, same
> stencils, same era, same neglect.
>
> **Four panels on one sheet, same drone, same camera:** (1) DORMANT — eye dark, arms stowed flat;
> (2) WOKEN — eye lit, arms half-deployed, alert posture; (3) EVADING — listing sideways with small
> cold-gas puffs; (4) DISABLED — completely dark, tipped over on its axis, one access panel hanging
> open, clearly salvageable.
>
> Grounded used-future industrial design, weathered and under-maintained. Hard single key light,
> black space background. **No guns, no missiles, no blades, no insect or spider anatomy, no
> aggressive creature styling** — this machine is bureaucratic, not hostile. No watermark.

---

## 4 · ORBITAL DEBRIS KIT — `orbital_debris_kit`

A modular kit sheet, not a hero. The Forge builds a field from these pieces.

```text
SUBJECT: modular orbital debris kit — 8 reusable junk pieces
GAMEPLAY FUNCTION: fills the volume OUTSIDE the swept corridor, which is what makes flying the
  corridor meaningful; also dresses the Overrun and the shell around THE FIND
PLAYER MOMENT: understanding, without being told, why the lane is worth staying inside
SCALE / ERGONOMICS: mixed — 0.5 m fragments up to 6 m structural sections
DOMINANT SILHOUETTE: broken MANUFACTURED forms — torn plate, snapped truss, spilled freight
FAMILY GRAMMAR: every piece must be recognisable as a broken part of something already in this pack
  (ring truss, coil housing, pod hull, drone limb, hauler plating)
MATERIAL / COLOR: the same alloy/primer/galvanised family, plus torn-metal bright edges
WORLD CONTEXT: decades of missed catches accumulating where nobody sweeps
PRESENTATION FORMAT: kit sheet — 8 numbered pieces on one neutral background, each in three-quarter
  view, no overlap, with one small assembled cluster in the corner showing them combined
PROTECTED FEATURES: torn edges must read as failure, not as design
VARIABLES TO EXPLORE: fracture language; how much ore/freight spill appears; burn vs clean breaks
AVOID: natural asteroids and rock (this is a MANUFACTURED graveyard), uniform cubes, tidy shapes,
  glowing wreckage, fire or smoke (there is no air)
```

> **Prompt —** Modular **orbital debris kit sheet** for a science-fiction salvage game: **eight
> separate numbered junk pieces** laid out on one neutral grey sheet, no overlap, each in
> three-quarter view.
>
> Every piece must read as a **broken part of industrial orbital infrastructure**, not as natural
> rock: a snapped section of lattice truss; a torn electromagnet coil housing with its ribbed casing
> split; a buckled hull plate with stencilling running off the tear; a crushed cargo-pod end cap; a
> severed drone manipulator arm; a tangled bundle of conduit and cable; a burst freight container
> spilling compacted ore; a bent radiator fin. Sizes range from **half a metre to about six metres**
> — show relative scale across the sheet.
>
> Materials: grey-brown anodised alloy, oxidised steel, galvanised plate, dull orange primer, with
> **bright raw metal at every torn edge** so the breaks read as failure rather than styling. Faded
> stencils and hazard chevrons survive in fragments.
>
> In one corner, a small **assembled cluster** showing several pieces drifting together as a field.
>
> **No natural asteroids or rock, no fire, no smoke, no glowing wreckage, no tidy geometric shapes.**
> Hard single key light, black space background, no watermark.

---

## 5 · THE OVERRUN HAULER — `overrun_hauler_wreck`

```text
SUBJECT: large derelict cargo hauler, broken-backed, past the last catch ring
GAMEPLAY FUNCTION: the landmark that ends the corridor and the body THE FIND is recovered from
PLAYER MOMENT: the artifact half is on this thing, among ordinary junk that all scans normally
SCALE / ERGONOMICS: ~40 m long — big enough to fly around, small enough to read whole
DOMINANT SILHOUETTE: spine snapped just aft of the bow; two halves hanging at a slight angle
FAMILY GRAMMAR: same guild industrial language, one class larger — it caught pods for a living
MATERIAL / COLOR: alloy and primer, one long burn-scar where it went through a ring
WORLD CONTEXT: it missed the catch, clipped a gate and has been drifting in the Overrun ever since
PRESENTATION FORMAT: single environment establishing image, wide, with a small human-scale figure or
  a cockpit frame in the foreground for scale
PROTECTED FEATURES: the break must be a STRUCTURAL failure at a plausible stress point
VARIABLES TO EXPLORE: break angle; how far the halves have drifted; cargo cage exposure
AVOID: military warship styling, gun turrets, fire, an intact pristine hull, obvious "quest marker"
```

> **Prompt —** Wide establishing concept art of a **derelict cargo hauler drifting in orbit**, about
> **40 metres long**, its spine **snapped just behind the bow** so the two halves hang apart at a
> slight angle, still loosely joined by torn structure and trailing cable.
>
> It is working industrial hardware, not a warship: blunt utilitarian hull, exposed frame members,
> bolted plate, stencilled ID, faded hazard chevrons, a **long burn-scar** down one flank where it
> struck something on the way through. Its **open cargo cage** is split and half-empty, with a few
> cargo pods still strapped in place and others drifting free nearby.
>
> Surrounded by a slow field of manufactured debris. In the foreground, a small **cockpit window
> frame** for scale. Beyond it, black space with a dense starfield and a ringed gas giant low in
> frame.
>
> Grounded used-future industrial style, hard single warm key light, deep shadows. **No gun turrets,
> no fire, no smoke, no military styling, no glowing markers.** No watermark.

---

## 6 · THE THROW — `the_throw_mass_driver`

```text
SUBJECT: ground-based electromagnetic mass driver climbing a coastal ridge
GAMEPLAY FUNCTION: horizon landmark in Toxic City — the ground end of the same machine, so sky and
  city are one place; the player flew down its corridor to arrive
PLAYER MOMENT: looking up from the quay and realising where the rings came from
SCALE / ERGONOMICS: ~1 km of track, muzzle gantry ~70 m up; seen only at distance
DOMINANT SILHOUETTE: a long shallow rising line ending in a heavy gantry — unmistakable at 250 m+
FAMILY GRAMMAR: same guild hardware at civil-engineering scale; accelerator coil houses repeated
  along the track like pylons
MATERIAL / COLOR: silhouette-dark against sky, amber running lights along the track (matching the
  ring lamps — same system, same signage)
WORLD CONTEXT: a drowned tidal industrial moon; the track climbs the one high ground
PRESENTATION FORMAT: environment establishing image from the city's waterfront, distant
PROTECTED FEATURES: the amber lamp language must match the catch rings
VARIABLES TO EXPLORE: track angle; coil-house spacing; gantry mass; how ruined it is
AVOID: rocket launch pad, gantry cranes for vertical rockets, flame trench, smoke, spaceport terminal
```

> **Prompt —** Environment concept art seen from a **wet industrial waterfront at dusk**, looking
> across green-brown toxic water toward a distant ridge where an **electromagnetic mass driver**
> climbs out of the flats — a **long, shallow, rising track roughly a kilometre long**, carried on
> repeated concrete piers, with **accelerator coil houses spaced along it like pylons** and **amber
> running lights** marking its length. It ends in a **heavy muzzle gantry about 70 metres up**,
> silhouetted against the sky. This is a track that **throws** freight, not a rocket pad: **no flame
> trench, no vertical launch gantry, no smoke, no terminal buildings.**
>
> Foreground: the dark silhouettes of harbour cranes and stilted shanties. Sky: dusk, heavy amber
> haze, a large ringed gas giant low on the horizon.
>
> Grounded used-future industrial style, heavy atmospheric perspective, restrained emissive — only
> the amber track lights. Painterly environment concept, no watermark, no text.

---

## 7 · WHAT HAPPENS WHEN THE KEEPERS COME BACK

The path is the existing one — nothing bespoke:

1. **Keeper selection** — Terry names the winning image per asset (playbook §1 step 5).
2. **Sidecar** — one JSON per keeper in `keeper_sidecars/` (stable id, class, scale band, palette
   slots, authority split, provenance). `internal_reference` unless proposed for shipping.
3. **Measured spec** — `measured_specs/` entry: real numbers pulled off the keeper (bore diameter,
   fin count, coil count, collar size), because a prompt is not a build instruction.
4. **Forge proxy** — a `ForgeRecipeDefinition` per id with `qualityState = Proxy` and the
   `PROXY_CONTRACTS.md` requirements for its `storyTag` (`prop` needs an `Interact` anchor if
   interactive and its collider on the contract root, never the visual child).
5. **The graybox note is the field, not a comment.** `qualityState` IS the "needs upgrading" marker:
   `Proxy → ProxyPlus → ProductionCandidate → ProductionReady`, with `Locked` pinning an approved
   result behind a content hash. Tripo/imported hero art lands **behind the same id** — sockets,
   colliders and gameplay scripts never change, so nothing downstream is touched.
6. **Photo comparison** — `ForgePhotoBooth` turnaround vs the keeper.
7. **Device verdict** — headset judgment where scale and readability are the question.

**Until step 4 lands, the in-game rings are the procedural v1 already committed** (truss torus, coil
housings, radiator fins, numeral collar, service spar, dead CATCH 3). They are deliberately
`Proxy`-grade: correct scale, correct silhouette logic, correct signage language — and ugly, on
purpose, until the keepers arrive.
