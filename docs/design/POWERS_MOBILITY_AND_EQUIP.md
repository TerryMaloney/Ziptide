# ⚡ POWERS, MOBILITY & THE SPECIAL-EQUIPPABLE META

**Status:** DESIGN (Round 4 research synthesis). No code changed by this doc. Companion to
`docs/design/WEAPON_FEEL_AND_ARSENAL.md` and `docs/design/ABILITIES_AND_ARSENAL.md`. **This EXTENDS
the existing augment system — it does not fork one.** Everything maps onto `AugmentDefinition` /
`AugmentLoadout` / `AugmentEffects` / `ItemFactory`.

Grounded in Overwatch, Apex, Fortnite, Splatoon, Titanfall 2, Hades/Risk of Rain, Monster Hunter,
Zelda BotW/TotK, Stormland, Lone Echo, Gorilla Tag, Superhot VR.

---

## 0. The core finding — SALVAGE is the whole economy
The game's non-lethal **"disable + salvage"** identity isn't just combat flavor — it's the single
currency that unifies every "Fortnite-layer" system. Disabling drones and collecting salvage:
- **charges your ultimate** (Overwatch/Splatoon: the thing you were doing anyway IS the charge fuel),
- **is your crafting currency** (Monster Hunter: build signature gear from accumulated salvage),
- **fills the anticipation meter** you watch across rooms ("almost enough for the Grapple…").

One resource, three anticipation loops. That is the design spine of this whole layer.

**Already built (the foundation to expand):** `AugmentDefinition : ItemDefinition` (kind Active/Passive,
`cooldownSeconds`, `durationSeconds`, `effectId`→pure `AugmentEffects`, `magnitude`, `gemColor`), a pure
`AugmentLoadout` (slot scarcity), an `AugmentController` that switches on `effectId`, `AugmentAuthor.Ensure()`,
and 6 live effects (surge_dash, bubble_guard, overclock, magnet_palm, sure_step, sixth_sense).

---

## 1. The power economy — the four-tier stack
A power feels **special only when it is not always there** (the user's exact instinct; every studied game
enforces a gap between *wanting* and *getting to use*). Adopt four tiers:

| Tier | Availability | Feel | Fuel |
|---|---|---|---|
| **Passive** | always on | invisible build-shaper | none |
| **Active** | cooldown 8–25s | reliable, used often | timer (already built) |
| **Ultimate** | charge meter 0→100% | the "it's ready!" event | **disables + salvage pickups** |
| **Consumable** | one-shot, looted/hoarded | scarce, saved for the moment | inventory count |

**The anticipation loop (the dopamine surface):** disable a drone → salvage sparks fly into your gem →
wrist/gem glow ticks up → gem hits full and *pulses + chimes* → you hold it, hunting the perfect moment →
gesture to unleash → meter empties, loop restarts. **The gap between "ready" and "spent" is where the
specialness lives** — never let ultimates recharge instantly.

Rarity tiers (white→blue→purple→gold, Apex) are **feedback juice only — they change numbers, not identity.
No power-creep spiral.**

---

## 2. Mobility & traversal — special by fuel, comfort by law
Ranked by (delight × VR-comfort × feasibility). **Everything ships as fuel / charges / cooldown /
consumable, so traversal stays a treat, never the default walk.**

1. **Grapple / Zip — THE FLAGSHIP (ties to the "Ziptide" name).** Player aims with the hand, fires, is
   *reeled on a fixed eased arc* (no jerk), auto-brakes on arrival, motion-vignetted. Avoid free
   pendulum-swinging as default (worst nausea) — opt-in "advanced" toggle only. Special via 1–2 charges +
   ~8–12s cooldown, or a `zipline_anchor` consumable.
2. **Climb / Mantle — safest delight in VR.** Hand-over-hand (Gorilla Tag/The Climb) feeds matching
   proprioception → near-zero nausea. Auto-mantle ledges. Scarcity from level design; a `gecko_grip`
   augment temporarily lets you climb *any* surface (duration-limited).
3. **Launch / Bounce pads** — thrown/placed deployables; clean player-chosen parabola, telegraph the
   landing reticle, vignette the ascent. Charge-limited (Fortnite ships stacked charges).
4. **Dash / Blink / Burst** — `surge_dash` already ships; keep it SHORT (long smooth dashes = nausea).
   The model to copy for everything else.
5. **Jetpack / Hover — high delight, highest comfort risk.** Do **burst-hover, not sustained flight**:
   short hand-aimed thruster pops (Stormland: control with arms not stick; ramp velocity fast then hold
   constant — slow accel is the nausea agitator). Fuel meter that won't recharge airborne (Fortnite/Valkyrie).
6. **Glide / Wingsuit** — two-arms-out **descent-only** controlled fall (kills fall damage, extends
   jumps). Genuinely comfortable VR-native input; auto-deploys off big launches.

### The VR comfort rulebook (non-negotiable for mobility)
Player **initiates and aims every motion** (arms/gaze over stick) · **constant velocity, never lingering
acceleration** (ramp fast, cruise flat, auto-brake) · **motion vignette** on all artificial travel ·
keep fast imagery **out of central vision** · **snap-turn, never smooth-turn** during traversal ·
multi-modal + opt-in intensity slider · hold a stable framerate.
**⛔ Everything that moves the XR rig (grapple reel, jetpack/glide velocity, auto-mantle, dash smear,
vignettes) is REPORT-ONLY and device-verified per CLAUDE.md's rig/locomotion contract — never auto-changed.**

---

## 3. Anticipation & acquisition — how gear stays an EVENT
Dopamine fires *before* the reward arrives (Knutson), so the job is to **stretch, gate, and ritualize**
the moment. Four mechanisms:
1. **The reveal ritual** — a special equippable never just *appears*. A drone carrying an augment
   **cracks open with a gold beam + rising tone** (reuse the salvage break-open; rare = longer/louder/gold).
2. **The choice moment (Hades boons)** — a chapter grants **three, pick one**, visible rarity. The tension
   of forgoing two is more memorable than the item.
3. **First-use as an event (Metroidvania gate)** — show a lock the player *can't* solve yet, name the
   gadget that solves it, then let them earn it. First activation = a promise paid off.
4. **Cosmetic / power separation** — drop *cosmetic* flourishes (weapon "voice," skins) freely; ration the
   *power tier*. Be kid-generous without power-creep.

**Acquisition — three lanes at different tempos (never two of the same in a row):**
- **Found/drop (fast):** common augments + charges break out of disabled drones — the everyday hum.
- **Crafted from salvage (medium):** signature gadgets (Gravity Glove, Ravel Gun, Grapple) **built at a
  machine from accumulated salvage** — the salvage bar filling IS the anticipation meter.
- **Story/challenge unlock (slow, ceremonial):** **one hero augment per chapter**, tied to that biome's
  creature, behind a Metroidvania lock — the "exotic" moments players remember.

---

## 4. Concrete powers & items (kid-friendly, non-lethal)
**Existing:** Surge Dash (A), Bubble Guard (A), Magnet Palm (P), Overclock (P/A), Sure Step (P), Sixth Sense (P).
**New actives:** Stasis Snare (throw a slow-field that freezes drones for capture — sets up salvage combos).
**Ultimates (charged by disable+salvage):** **Capture Net** (wide net harmlessly captures every drone in
view → instant salvage; the screen-clearing "I earned this") · **Salvage Storm** (vortex disassembles
nearby enemies into a fountain of parts).
**Consumables (looted/hoarded):** **Chug Cell** (drink-gesture restores shield) · **Mythic Gauntlet**
(one-per-world rare drop → a single super-charged version of any equipped active; finding it is an *event*).
**Mobility augments/items:** grapple_reel, gecko_grip, burst_hover, glide_wings; launch_pad, bounce_pad,
zipline_anchor (deployables).

### Signature VR-native powers (impossible on a gamepad)
- **Time Dilation** (Superhot-inverted) — raise **both hands and hold still** to slow the world for a few
  seconds; triggered and sustained by your body. Kids intuitively "reach out to stop time." Ultimate-tier.
- **Raise-Shield** — physically **lift your forearm** and a hard-light barrier snaps to it; the angle/height
  you hold IS the aim.
- **Two-Hand Combine Ult** — **slam both charged gems together** to fuse two ultimates into a bigger one
  (Hades Duo Boon, gestural). Pairs with the weapons doc's Fuse-Snap identity.
- **Glyph Cast** — hand-draw a simple shape in the air to select which salvage-power fires (anticipation + skill).

---

## 5. The VR loadout model (reconciled with the weapons doc — NO pause menus mid-fight)
- **Primary** = dominant hip/over-shoulder · **Secondary** = opposite hip · **Throwable** = chest/bandolier
  tap (spawns primed) · **Gadget/augment** = **off-hand wrist-radial** (a deliberate half-second flick is
  *fine* for "special" verbs — its slowness reinforces they're not spammable).
- **Augment/ultimate activation** = cover-and-charge the wrist gauntlet — the charge ramp IS anticipation,
  physicalized (gem glow shows fill %, pulses on ready).
- **Rare items get their own body slot with a glow/magnet nudge** so equipping them is a distinct,
  ceremonial reach — the rare thing *feels* different to draw.

---

## 6. Kid-friendly reward guardrails
**DO:** guaranteed generous drops · visible **collection/completion** ("3 of 5 augments this biome," a
Quarters trophy shelf) as the core hook · the end-of-room tally (Pikmin/de Blob) · earned, transparent unlocks.
**DON'T:** no purchasable random boxes · no one-in-a-million odds · no paid rarity · no FOMO timers that cost
money. Uncertainty for *delight* (which drone drops which part) is fine; uncertainty tied to *spending* is
the line never crossed. Vaulting/seasonal rotation = free freshness, never a paywall.

---

## 7. Architecture mapping (data-driven; extends what exists)
**Extends cleanly (enum + fields on systems that already exist):**
- `AugmentKind`: add **`Ultimate`** and **`Consumable`** to the Active/Passive enum. `AugmentLoadout`
  already enforces slot scarcity — add one dedicated **Ultimate slot** + a **Consumable pouch count**.
- `AugmentDefinition`: add `chargeToFill` (float; 0 = cooldown-driven, >0 = charge-driven),
  `chargeSource` enum (`DisablePerCapture` / `SalvagePickup` / `TimePassive`), and `rarity` enum
  (Common→Mythic) for gating/loot + gem-glow intensity. Consumables reuse `durationSeconds`/`magnitude`,
  set `cooldown=0`, count-limited.
- `AugmentEffects` / `AugmentController`: new pure `effectId`s routed by string — `stasis_snare`,
  `capture_net`, `salvage_storm`, `time_dilation`, `raise_shield`, `chug_cell`, `grapple_reel`,
  `gecko_grip`, `burst_hover`, `glide_wings`. No new hard refs.
- **Deployables** (`launch_pad`/`bounce_pad`/`zipline_anchor`) reuse the weapons doc's planned
  `DeployableRuntime`; charge-limited items via `ItemFactory`.

**Genuinely new (small, isolated):**
- **`ChargeMeter` service** (pure, Core/Gameplay) — subscribes to disable/salvage events, accumulates per
  equipped Ultimate. The one real new system, mirroring Overwatch's charge-points model.
- **Gem/wrist UI** — fill % + the "ready" pulse (the dopamine surface).
- **Salvage-as-currency + a craft-from-salvage station** — ties acquisition to the core loop.

**⛔ Report-only (per CLAUDE.md):** anything touching the XR rig / locomotion / comfort (all mobility
motion), inventory persistence, holster travel, or input actions. Device-verified, never auto-fixed.

---

## 8. What to prototype first (recommendation)
1. **The `ChargeMeter` service + gem "it's ready!" UI** — the smallest thing that makes powers feel
   *special*, and it makes the existing 6 augments better immediately.
2. **`AugmentKind` Ultimate/Consumable + `chargeSource=DisablePerCapture`** — wire the ultimate meter to
   the disable+salvage loop (the design spine).
3. **Capture Net ultimate** — the first "I earned this" screen-clearing payoff; pure non-lethal spectacle.
4. **Grapple (`grapple_reel`)** — the signature Ziptide traversal verb (report-only, device-verified).
5. Then salvage-as-crafting + the three-lane acquisition rhythm (the anticipation engine).
