# THE HANGAR, AND THE COUPLER — first-impression pass

**Status:** design + concept prompts, ready to build/generate. No canon changes.

⚖ Terry, 2026-08-01: *"if this is the first thing we see outside the ship we really want to build a
sense of awe and perspective… the hangar was pretty much just like a bunch of sort of box areas, not
very good"* and *"the coupler cell and the part it goes into is like a square and a box… we need a
male and a female part and it should fit into the spot."*

---

## 1. Why the hangar reads as boxes — the actual measurements

Not an impression. This is what `ToxicCityLayout.asset` authors for the Shipyard district:

| Element | Size | Count |
|---|---|---|
| District bounds | 22 × 18 m | 1 |
| Crane landmark | 16 m tall, **2 m wide** | 1 |
| Shipyard Office | 7 × 7 × 4 m | 1 |
| `props` | — | **0** |
| Berth walk (added 2026-07-31) | 5 pads, 12 × 14 m each | 1 |

**The problem is not that the shapes are boxes. It is that nothing in the space states its own scale.**
Awe in VR is manufactured by exactly four things, and the yard currently has none of them:

1. **Human-scale detail on something enormous.** A 16 m crane that is 2 m wide is a stick. The same
   crane at 4 m wide with a ladder, a walkway, handrails and a cab reads as *sixteen metres* because
   the player can see the rungs and knows a rung is 30 cm.
2. **Vertical layering.** Nothing is above the player. A ceiling, a gantry, a pipe run overhead — VR
   scale is felt when you have to *look up*.
3. **Depth in three bands.** Foreground (reachable), midground (walkable), background (unreachable,
   large). The yard is one flat band.
4. **Motion at scale.** One slow-moving large thing sells size better than ten static ones.

## 2. The hangar pass — five changes, in value order

### 2.1 Widen the crane and put rungs on it *(the single biggest win)* — ✅ BUILT 2026-08-01
`Crane` goes 2 m → **4.5 m wide**, and gains a ladder with **30 cm rungs**, a mid-height walkway, and a
cab at the top. Cost: a handful of cubes. Effect: the yard gets a ruler, and every other object inherits
scale from it. **Do this one first even if nothing else happens.**

**As built.** `LandmarkScaleCore` (Content, pure) owns the two numbers that decide whether this works —
rung pitch (locked to the 25–35 cm band a real ladder uses, because the trick *is* that the player
already knows the distance) and slenderness (past 4:1 there is no surface left to put detail on). It
also owns a **detail ceiling at 7.5 m**: rungs above that are three pixels and a draw call each, so a
40 m tower costs exactly what a 16 m one does. `CityBuilder.BuildLandmark` builds mast · ladder ·
walkway + rail posts · cab · jib + hook, and only the mast keeps a collider — a collider on a rung is a
snag the player can neither see nor climb.

Two things worth knowing:
- **Detail is opt-in per landmark** (`LandmarkDef.kind`). Twelve authored worlds carry landmarks that
  are thin *on purpose* — W002's LightShaft is a shaft of light, W003's prisms are prisms — and bolting
  an operator cab onto those would be a cross-world regression shipped in the name of a hangar fix.
  `Tower` still builds the exact single cube it always did.
- **The crane moved**, x=8 → x=5, z=−6 → z=−5. At 4.5 m the old position intersected the east facade
  row; the widening would have shipped as clipping geometry.

`CityLandmarkAuthoringTests` pins the authored data, not just the rule — the shipped crane would have
failed `ReadsAsStick` from the day it was authored and nothing was asking.

**Follow-up (needs Terry's editor, not the cloud):** `W000_DriftIn`'s `GantryCrane` is 10 m × 2 m — the
same defect, in the first room of the game. Its BerthBay is small and ringed by facades on both edges
near the crane, so the widened footprint needs a placement eyeballed in the scene view rather than
guessed from bounds arithmetic. One `kind = Crane` + a width and position, once someone can see it.

### 2.2 A roof the player has to look up at
The berth is a *hangar* and has no overhead. Add a partial gantry roof over the ship's berth only —
open at the seaward end so the sky (and the new acid haze) is still the backdrop. Trusses at 6 m,
lamps hanging at 4.5 m. **Looking up is the mechanic that makes a space feel big.**

### 2.3 One thing that moves, slowly
The crane's hook descends and rises on a slow loop, or a gantry trolley tracks the length of the quay.
One object, constant motion, no interaction. This is the cheapest possible "the world is running
without me" signal and it is worth more than five props.

### 2.4 The three depth bands, deliberately — ✅ FOREGROUND BUILT 2026-08-01
- **Foreground:** crates, a spilled toolbox, cable spools *on the walk itself* — things the player
  passes within arm's reach. **Built:** `ApproachClutterCore` places nine pieces, alternating sides,
  measured off the live berth and district rather than doc coordinates.
- **Midground:** the berths (already built) and the office.
- **Background:** the city skyline already exists but is not *framed*. Aim the walk so the player exits
  the ship facing the skyline, not facing the office wall. *(Still open.)*

The part worth keeping in mind: the interesting half of foreground dressing is the **corridor**, not the
scatter. Junk in the walking lane is a soft-lock on the game's first walk, and it is the hiding kind —
a VR player cannot see their own feet, so a knee-high crate on the line is invisible until it stops
them. The lane is 2.2 m wide, the law is a test, and each piece's near edge is measured off the
**footprint** the author actually builds (a toolbox is stretched 1.5× across the walk) rather than its
nominal size. Both of those were bugs first.

### 2.5 What the first walk should teach — without a tutorial line — ✅ TWO OF THREE BUILT
The route from ramp to Dispatch is the game's first lesson, and it can teach three things by layout
alone:
- **Look up** — because the roof edge and the crane force it. *(Crane built §2.1; roof still open §2.2.)*
- **Things are reachable** — ✅ one crate on the walk that can be picked up and is worth nothing. A
  player who grabs a worthless crate has learned the grab verb at zero cost. **Built** as
  `LooseCrate`: Rigidbody + `XRGrabInteractable`, no `ItemDefinition`, never enters the inventory.
  It only works if it really is worthless, so it stays that way.
- **The lanterns mean "this way"** — ✅ **built.** The lantern route started *at* Dispatch, so the
  player crossed the whole quay unlit and then arrived at a lit city with no idea the lamps meant
  anything. `ArrivalWalk` (Quay → Shipyard → Dispatch) is now lit the same way, kept separate from
  `JobRoute` because that one is a loop that must come home. `WayfindingCore.MergeLanterns` drops
  coincident lamps — and de-duplicating within each list also fixed something already wrong: the job
  route names Dispatch twice, so two globes have been z-fighting on that corner since the compass
  shipped.

**None of that needs a line of dialogue.** That is the point.

---

## 3. The coupler — male and female, and it should *fit*

Current state: a cell that is a box and a socket that is a box-shaped hole. In VR that is the single
most disappointing kind of interaction, because the hands are the whole medium and the object gives
them nothing to do.

### 3.1 What it must be, mechanically
- **Keyed, not symmetric.** One orientation works. The player should be able to feel — visually — which
  way is up before they try.
- **A lead-in.** A chamfer/funnel at the socket mouth so a near-miss slides in rather than bouncing off.
  This is the difference between "satisfying" and "fiddly" in VR, and it is geometry, not code.
- **Two-stage seat.** It goes in, then it *turns* or *clamps* to lock. One motion is a slot; two motions
  is a mechanism.
- **A visible state change on seat.** The contact ring lights, the collar drops, something moves.

### 3.2 Nano Banana prompt — the coupler cell and its socket

> Production concept sheet for a **two-part power coupling** in a Meta Quest VR salvage game: a
> hand-held **cell** (male) and the **receiver socket** (female) it seats into. Not a prop — the player
> physically picks the cell up, aligns it, pushes it in and turns it to lock, with VR hands.
>
> **Gameplay function.** The cell is a portable power source the player carries to broken machines.
> Seating it correctly restores power and completes a repair. It must be obvious by LOOKING which way
> up the cell goes, and obvious when it is seated.
>
> **Scale and ergonomics.** The cell is **two-handed-carryable, roughly 30 cm long and 18 cm across** —
> heavy enough to matter, small enough to hold in front of your chest and see past. Show it beside a
> gloved hand. The socket sits at **chest height** on a machine face.
>
> **The mechanical requirements — these are the point of the sheet:**
> - **KEYED, not symmetric.** An asymmetric cross-section — a D-profile, a single flat, or one wide
>   spline among narrow ones — so exactly one rotation fits and the player can SEE which.
> - **A LEAD-IN CHAMFER** on the socket mouth: a funnelled entry that guides a near-miss into place.
>   Show the chamfer clearly in section.
> - **TWO-STAGE SEATING:** push in, then rotate ~30° to lock. Show a locking collar, lugs or a bayonet
>   fitting. Draw the unlocked and locked positions side by side.
> - **A HANDLE the hand obviously goes on** — a recessed grip or a bail handle, not a smooth block.
> - **Contact faces that read as electrical** — concentric rings or pins, recessed and protected so they
>   look like they matter.
>
> **Family grammar.** Refit salvage industrial, matching an existing weapon rack: worn grey-blue steel
> and dark gunmetal (#2E3B47, #1F2830), one warm amber accent (#FF6B1F) used ONLY to signal state —
> unlit when unseated, lit when locked. Visible repair seams, bolt-through plates, mismatched brackets.
>
> **Presentation.** Neutral grey background production sheet: (1) the cell, three-quarter, with a gloved
> hand for scale; (2) the socket, front, showing the keyway and chamfer; (3) a **cross-section** through
> both, seated, showing how the lead-in and the lock engage; (4) the two-stage sequence — approaching,
> inserted, locked — as three small drawings; (5) unlit vs lit contact state. Even neutral studio
> lighting, orthographic where possible. No dramatic lighting, no text labels, no UI.

### 3.3 Why the cross-section panel matters
Panel (3) is the one that makes this buildable. A pretty exterior tells the modeller nothing about the
lead-in angle or how deep the lock travels — and those two numbers are what decide whether it feels
good in the hands.

---

## 4. Recommended order

1. ~~**§2.1 crane width + rungs**~~ — ✅ done 2026-08-01.
2. **§3.2 coupler concept** — Terry can run this immediately; it unblocks the modelling. **Next.**
3. ~~**§2.5 lantern extension**~~ — ✅ done. **§2.2 roof** still open — layout only, teaches by
   forcing the player to look up.
4. **§2.3 one moving thing** — needs a small runtime component; worth it. The crane now has a jib and
   a hook to move, so this is a slow loop on an object that already exists.
5. ~~**§2.4 foreground clutter**~~ — ✅ done.

### What is left in this document
- **§2.2** the partial gantry roof over the berth (layout).
- **§2.3** the crane hook on a slow loop (small runtime component; the geometry now exists).
- **§2.4 background band** — frame the walk so the player exits facing the skyline, not the office wall.
- **§3** the coupler itself — the prompt in §3.2 is ready to run; nothing is built.
- **W000_DriftIn's GantryCrane** — same stick defect in the first room of the game, needs a placement
  eyeballed in the editor (see §2.1).
