# AFFORDANCE SHEEN & LOST ITEMS — "can I use this?" and "where did my stuff go?"
### The interaction-truth layer, and the no-lost-tools guarantee
**Status: DESIGN — planning only, zero code. 2026-07-24, Terry-directed. Round-shaped (§5).**
This is the generalization of the coupler lesson: the misleading red "button-looking" indicator
cost a device session because the game LIED about what was interactable. The sheen makes
interaction-truth a system instead of a per-object accident.

---

## 1 · THE AFFORDANCE SHEEN (interaction truth, whispered)

**The idea (Terry's):** a subtle light sheen — "not super noticeable" — on things the player can
actually use: doors that open vs doors that don't, harvestable plants vs scenery, grabbable vs
bolted. The player's hands learn the language in minutes and stop testing every surface.

### 1.1 The law
**Sheen = a promise.** If it glints, you can use it *right now*; if it doesn't, you can't.
Never decorate a non-interactable with the sheen material; never ship an interactable without
it. (Enforceable — §1.5.) State variants:

| State | Read |
|---|---|
| Usable now | faint white/theme rim, ~10–15% intensity — visible on approach (~8m), not from across the world |
| Usable, hand/gaze near | rim brightens ~2× + existing haptic tick (XRI hover events — already fire) |
| Exists but not YET usable (locked door, unripe plant, unpowered machine) | **no sheen** + a state tell from the object itself (lock plate, closed bud, dead panel) — absence must also be honest |
| Just became usable (ripened, powered, unlocked) | one soft 1s pulse, then settle to faint — the world quietly says "now" |

### 1.2 Where it applies (the cross-game audit Terry asked for)
Doors/travel surfaces (openable vs welded — the derelict archetype NEEDS this on day one) ·
harvestable plants vs scenery flora (**this IS the 10m readability law** from
`WORLD_RECIPE_GAP_AUDIT.md` §2.3, unified: ripe = sheen pulse via `GardenService` timing) ·
grabbable items · machine interaction points (panel, part seat, PRESS POWER — the coupler
retrofit) · holster/belt sockets when an item hovers near · kiosks/boards · zipline handles ·
build sockets (payable = sheen) · repairables' CURRENT stage point only (guides the sequence
hands-free). Explicit non-targets: enemies (combat has its own read), scenery, anything whose
verb is "look".

### 1.3 Quest budget implementation (the part that decides everything)
No post-process outlines, no second camera, no stencil pass — all budget killers. The cheap
correct answer in URP on Quest: **a fresnel-rim emissive term in the shared interactable
material variants**, driven per-object by `MaterialPropertyBlock`… except property blocks break
SRP batching — so the rule is **a small fixed set of shared "sheen state" materials**
(off / faint / bright / pulse — 4 variants per base family), swapped by reference on state
change. Material count stays inside the ≤25 budget because interactables already share the
`Mat()` cache families; we add ~4 variants, not per-object materials. One `AffordanceSheen`
component (Gameplay) owns the swap + hover subscription; factories and builders attach it
(`ItemFactory`, door/POI builders, `GardenPlotRuntime`, machine stages) so content gets it FREE
— authored scenes never hand-place it.

### 1.4 Accessibility (why sheen beats color-coding)
Luminance-based rim reads for colorblind players by construction; it also reads at kid height
and in the dark (derelicts: the sheen is the wayfinding). Pair with the narration seam: focus
speech says the verb ("door — opens", "ripe — harvest") for the youngest players. Never
color-only, never sheen-only for critical paths (the state tell rule above).

### 1.5 The gate (PG-flavored, one rule)
`AFFORDANCE_TRUTH` audit: every component implementing an interaction interface carries
`AffordanceSheen` (or an explicit opt-out reason string); no renderer outside an interactable
hierarchy uses a sheen material. Deliberately-broken-scene test proves both directions fire.

## 2 · LOST ITEMS (the no-lost-tools guarantee)

### 2.1 The verified hole
`InventoryState`: **only holstered items travel; "loose" items are left behind on travel** — and
loose items don't persist in scene saves either. `ReleaseFeel` (P0.4) already rescues accidental
*throws* at release time, but a set-it-down-and-walked-away taser is silently gone forever after
travel. For a kid-friendly game this is a progression-loss landmine; for VR specifically it's
worse — items get put down in 3D space constantly and proprioception doesn't save you.

### 2.2 The design: three layers, all cheap, in order of when they act
1. **At drop (exists + one addition):** ReleaseFeel keeps the throw rescue; ADD a belt tell —
   the emptied holster socket shows its faint sheen (§1) + one soft haptic on the belt hand.
   You *feel* the gap immediately instead of discovering it in combat.
2. **While separated (the new core — "the belt misses its tools"):** an OWNED item (player-
   acquired, i.e. it has lived in a holster) that sits loose for **90s** or is **>25m** from
   the player begins a slow beacon: the item's own sheen pulses bright + a soft chirp every
   ~10s (audio budget: one voice, distance-attenuated). RILL, first time only: *"Your taser's
   sulking where you left it."*
3. **At travel/quit (the guarantee):** any OWNED loose item left behind is **reclaimed to the
   ship's LOST & FOUND crate** — a physical bin in the Quarters (fiction: RILL logs it, dock
   drones return it; one more "the ship takes care of you" beat). Implementation is the
   overlay-save idiom: on travel, loose owned items append `{itemId}` to a profile
   `lostAndFound` list (ids only — ItemFactory rebuilds them on collection); the crate is a
   socket cluster reading that list. **LAW: an OWNED item can never cease to exist.** World-
   native props (cans, junk) are explicitly exempt — scenes keep owning their litter.

### 2.3 What we deliberately do NOT build
No minimap/quest-arrow to lost items (breaks diegesis; the beacon + crate cover it) · no
auto-return-to-belt teleport while in-world (watching your dropped gun fly back is cool exactly
once, then it deletes the *put things down deliberately* verb that machines/plots rely on) ·
no item insurance economy. The crate is free, silent, and total.

### 2.4 Tests/evidence
Pure core: lost-and-found list append/dedupe/cap + old-save neutral default (ProfileSerializer
pattern) · EditMode: owned-vs-native classification · diag tags: `ZIPTIDE: ITEM_BEACON id=…`,
`ITEM_RECLAIMED id=… to=lost_and_found`, `AFFORDANCE state=…` · device pass: drop taser →
walk → hear beacon → travel → find it in the crate.

## 3 · ROUND SHAPING (both are Growth-Round-sized)
- **Sheen v1** (~3 commits): material variants + `AffordanceSheen` + factory/door/plot wiring
  + audit. Observable: walk ToxicCity, every openable door glints, scenery doesn't.
- **Lost items v1** (~3 commits): belt tell + reclaim-on-travel + crate + tests. Observable:
  the drop→travel→crate loop on device.
- Beacon pulse + narration lines ride the following round (needs audio asset + RILL line data).
Both slot cleanly into ROUND 01/02 slates; neither touches the spine (no rig/travel/save-format
changes — the profile list rides the existing overlay-save law).
