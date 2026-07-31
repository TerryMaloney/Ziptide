# CONCEPT PROMPT — THE ARMOURY RAIL (ship weapon rack)

**Status:** ready to generate. Authored to `CONCEPT_ART_PROMPT_PLAYBOOK.md` §2 field structure.
**Built counterpart:** `ShipArmouryCore` (slot maths) + `ShipBoardingStation.BuildArmouryRack` (the
placeholder that ships today). ⚖ Terry: *"the ship is the armoury… this gun rack can be consistent
possibly across all different modes."*

**Grounded in the SHIPPED numbers, not invented ones** — a concept that disagrees with the code costs a
rebuild:

| Property | Value | Source |
|---|---|---|
| Slots | 8 | `ShipArmouryCore.RackSlots` — the full authored arsenal |
| Slot spacing | 0.34 m | `SlotSpacing` |
| Rail width | ≈2.4 m | `RackSlots × SlotSpacing` |
| Rail height | 1.15 m | `RailHeight` — no crouch, no stretch |
| Placement | by the hatch | so it is on the way out, not a detour |

---

## The prompt

> Production concept sheet for a **wall-mounted weapon rack — the "armoury rail"** — for a Meta Quest VR
> salvage game. Not a display case: a working piece of ship furniture the player uses every time they
> leave.
>
> **Gameplay function.** The rack is where the player's entire arsenal lives aboard their ship. They
> take a weapon off it, put it on their hip, and only then will the ship's ramp open. It fills up over
> the campaign — most slots start empty. **An empty slot must read as "something belongs here," not as
> damage.**
>
> **Scale and ergonomics.** Horizontal rail **2.4 m wide** carrying **8 evenly spaced slots, 34 cm
> apart**, rail centred **1.15 m above the deck** — mid-chest for a standing adult, everything grabbable
> without crouching or stretching. Show a human figure or gloved hand for scale. Weapons hang
> **muzzle-down, grip outward**, angled slightly toward the viewer so a hand can close on a grip without
> rotating the wrist.
>
> **Family grammar.** Refit salvage industrial: worn structural metal, visible repair seams, mismatched
> replacement brackets, bolt-through mounting plates. Cold grey-blue steel and dark gunmetal (#2E3B47,
> #1F2830) with one **warm amber** functional accent (#FF6B1F) used *only* where the design signals
> state — never decoratively. Retaining clamps are chunky, mechanical, obviously VR-grabbable. Every
> slot is the same repeated module; the rail is a repeated unit, not a bespoke sculpture.
>
> **The one thing that must be legible: slot state.** Design three clearly distinguishable slot
> conditions — **occupied**, **empty-but-owned** (weapon exists, currently on the player's belt), and
> **empty-never-filled** (not yet earned). Distinguish these with *shape and value*, not colour alone —
> a stencilled outline, a physical blanking plate, a recessed shadow. They must read at 2 m in a headset.
>
> **Cross-mode consistency.** Design it as **one modular unit with three mounts**: bolted to a bulkhead,
> free-standing on a floor plinth, and recessed into a wall. Same rail, same slots, same clamps in all
> three — only the mounting changes. Show the three mounts side by side.
>
> **World context.** Interior of a lived-in salvage ship on a toxic industrial world — cramped,
> functional, maintained by one person who repairs rather than replaces.
>
> **Presentation.** Clean production concept sheet on a neutral grey background: (1) front elevation,
> fully loaded; (2) front elevation showing all three slot states; (3) three-quarter view with a human
> figure for scale; (4) detail of one slot and its clamp; (5) the three mount variants. Even, neutral
> studio lighting. **No dramatic rim light, no atmospheric haze, no lens flare** — this is a build
> reference, not a beauty shot. Orthographic where possible. No text labels, no UI, no logos.

---

## Why the prompt is shaped this way

**The empty slot is the teaching device, so it is the hero requirement.** The ramp refuses to open until
the player is armed. A rack whose empty slots read as *damage* tells them the ship is broken instead of
"go take one" — that is the difference between a prop and a piece of design, and it is the one thing a
generic "sci-fi gun rack" prompt will get wrong.

**The modular-unit spec is what makes Terry's "consistent across all modes" achievable.** A single
bespoke sculpture looks wrong the moment it has to appear in a PvP loadout room or a hub. Asking for one
rail with three mounts up front yields a rack that travels without being redrawn.

**Slot state is shape-and-value, not colour**, because it must survive Quest resolution, the city's new
green acid haze (`WorldAtmosphereBinder`), and colour-blind players. Anything that only reads as hue
will fail at least one of those.

**Neutral lighting is requested deliberately.** Per `CONCEPT_TO_BUILT_PIPELINE.md` this image feeds a
measured visual spec; dramatic lighting hides exactly the proportions and seams that step needs.

## Refinement rule

Per playbook §1, refine **only named failures** and explicitly restate what to preserve. Tell it what to
keep, not just what to change — an unqualified "make it better" loses approved structure and costs a
selection round.
