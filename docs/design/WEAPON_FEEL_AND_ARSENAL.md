# 🔫 WEAPON FEEL & ARSENAL — the path to the AAA-VR bar

**Status:** DESIGN (2026, T-Dog research rounds 1–2). No code changed by this doc. This is the durable
reference the fleet builds weapons against. Companion to `docs/design/ABILITIES_AND_ARSENAL.md` (the
idea bank) and `docs/post_recovery/TDOG_WEAPON_SYSTEM_HANDOFF.md` (the existing-stack map). **Do not
fork a parallel combat system** — everything here attaches to `ItemDefinition → ItemFactory → runtime
→ target interfaces`.

Grounded in research on Half-Life: Alyx, Pavlov, Contractors, Onward, RE4 VR, H3VR, Boneworks,
Blade & Sorcery, plus CoD/Halo/Fortnite for the flat-game bar the user wants a VR equivalent of.

---

## 0. The core finding
The gap between what we have and the AAA-VR bar is **not the firing logic** — that's sound and cleanly
layered. It's **feedback layering + physicality**. ~70% of the jump is *additive layers* on the
runtimes we already have; only a handful of things are genuinely new systems — and they're the right
ones (two-hand hold, physical reload/moving-parts, gravity glove).

**Current honest state:** guns are one-handed, hitscan, no ammo/reload, no recoil, no moving parts,
single haptic pulse + single audio one-shot. It *works and feels okay* — it is not Alyx/RE4-grade yet.
**Architecture is genuinely good** and was built for exactly this enrichment (look is fully decoupled
from behavior via the Forge recipe; one canonical `ItemFactory`).

---

## 1. The five axes (decided)

### Axis 1 — Clean aiming: THERE IS NO ADS IN VR
Every top VR shooter abandoned aim-down-sights; physically raising the gun to the eye *is* the ADS.
"Crisp aiming" comes from:
- **Red-dot / reflex sight** = the premium "reads clean" option (billboarded dot on target regardless
  of eye position — the CoD/Halo clarity, VR-legal). **Iron sights** default on everything.
- **Weapon smoothing** — a low-pass filter on controller jitter. The single biggest crispness win;
  reads as steadiness, not cheating (Contractors shipped this).
- **Two-hand-vs-one-hand accuracy cone** — never perfect hitscan (perfect accuracy feels *floaty* —
  no skill signal). Two hands tighten the cone; one hand adds sway.
- Optional, tiny, comfort-toggled **sticky radius** (snap to nearest hitbox within a few degrees) is
  the only VR-safe aim-assist. **Never** rotational aim-assist, flat-screen zoom, or screen-shake.

### Axis 2 — The mechanical look that MOVES (the #1 gap; closest to the face)
The gun is ~30cm from the eyes, so a *stateful, moving* gun is the biggest immersion multiplier:
cycling slide/bolt, ejecting brass, a magazine that physically seats, a readable chamber state.
Alyx's signature: reload before empty → skip the cock; reload on empty → must rack. The gun becomes
a stateful object you master. This is both a visual system (moving parts) and the mechanical audio layer.

### Axis 3 — Feedback layering (biggest single ROI)
Replace one-pulse/one-shot with:
- **4-layer audio (the Alyx model):** Head (transient) + Body (punch) + LFE (sub, non-spatialized) +
  Tail (reverb, non-spatialized), 3–5 randomized variations each + a mechanical layer. Head/Body run
  HRTF; LFE/Tail bypass to "feel big."
- **Shaped haptic envelope** (sharp attack + decay), a support-hand pulse on two-handed guns, and a
  **hit-confirm** buzz when your round connects.
- **Surface-typed impacts** (sparks/dust/splinters by material tag) + pooled decals + **hitstop**
  (2–4 frame freeze on hit) + **damage-tiered hit reactions** (flinch → stagger → down).

### Axis 4 — Weapon weight & recoil
Sold through *behavior*, not a prop: heavier guns sway more one-handed, demand two hands, kick with a
recovery you fight back to point-of-aim. All additive, scaled off existing `mass`/`damage` fields.

### Axis 5 — Exotic arsenal & the innovation frontier: the GRAVITY GLOVE
The Alyx gravity-glove loop is a three-stage ritual most VR grabs skip:
1. **Select** — point; the object *glows* to confirm intent before commitment.
2. **Flick-pull** — a wrist flick springs it toward you; tuned for ~constant flight time regardless of
   distance (a rhythm you learn), homing generously but never auto-snapping (agency preserved).
3. **Catch** — you manually grip on arrival; a missed catch drops it. The *earned* catch is the dopamine.
Then **hold → aim → throw/place** with real arm velocity. Gated by **grab-weight tiers**: light props
and *stunned* enemies pull fully; heavy things resist (you feel a tug, they don't move) — a force with
limits, not a cheat. **Comfort law (non-negotiable):** it moves *objects toward a fixed player, never
the player*. This is exactly the planned starter loop: **scan → stun → gravity-grab → place/throw → reward.**

---

## 2. Combat identity: non-lethal "disable + salvage" (a feature, not a limit)
The code already leans non-lethal (taser, gravity-gun *downs* drones, `IShockable`). Lean IN: weapons
that **stun / capture / splat / down** rather than kill are simultaneously **kid-friendly (age-rating
friendly)** AND a **novelty lane** most VR shooters ignore. The reward is salvage/capture, not a body
count. (Round 3 research expands this — see `ABILITIES_AND_ARSENAL.md` updates.)

---

## 3. Classic vs innovate — the calls
- **COPY from CoD/Halo/Fortnite:** red-dot clarity, subtle slowdown/sticky targeting, hitstop, tracers,
  hit-confirm feedback, damage-tiered hit reactions, the primary/secondary/equipment loadout shape.
- **INVENT for VR (copy from Alyx):** no ADS mode (physical raise), two-hand-vs-one-hand sway model,
  4-layer HRTF-aware audio, dual-hand haptic envelope, weapon smoothing as the "aim assist," the
  gravity-glove pull→catch→throw loop.
- **DON'T port:** rotational aim-assist, flat-screen zoom ADS, screen-shake (nausea — express recoil
  through the gun model + haptics instead).

---

## 4. The roadmap — additive-first, device-testable at every phase

### Phase 1 — "Every shot feels AAA" (all additive; fastest, lowest-risk; lands on the existing pistol)
- `WeaponAudioKit` — 4-layer audio (head/body/LFE/tail + variations + mech clack). *New shared service;
  data-driven via new `ItemDefinition` clip-array fields.*
- `HapticEnvelope` — shaped attack/decay, support-hand pulse, hit-confirm. *Replaces the single `SendHapticImpulse`.*
- `SurfaceImpactService` — material-tagged impacts + pooled decals + hitstop + hit-reaction tiers.
  *New service keyed by a `SurfaceType` tag on geometry.*
- Recoil + weight module — angular kick + recovery scaled by `mass`. *Additive on the fire path.*
> This trio + recoil is the ~80% feel jump with zero new interaction systems.

### Phase 2 — "The gun is a physical machine" (new systems; high impact; closest to the face)
- `MovingPartRuntime` — visible slide/bolt cycle on fire + ejecting brass.
- Two-hand grip system — secondary attach point; aim from the two-hand line; the sway/cone accuracy
  model; weapon smoothing. *(This IS the clean-aiming system.)*
- Physical reload — `Magazine` as its own item; `ReloadRuntime` with chamber state + racking gesture;
  diegetic ammo counter.
- Sights — iron default + red-dot component, attached by `ItemFactory` like `GunLaserSight`.

### Phase 3 — "The exotic arsenal / VR innovation"
- `GravityGloveRuntime` — the pull→catch→hold→throw FSM + grab-weight tiers, reusing the gravity gun's
  target fan-out + self-ignore. **The innovation centerpiece.**
- Generalize charge/overheat on `PrismBeamRuntime`; velocity+edge melee on `MeleeWeaponRuntime`; the
  gravity gun's secondary "hold" mode; a Forge recipe for the gravity gun (fix crude look); new runtimes
  for bow / thrown / deployables; secondary weapons + throwables (Round 3).

---

## 5. Architecture mapping (additive vs new system)
| Capability | Type | Attaches at |
|---|---|---|
| Recoil / weight | Additive | fire path in runtime; reuse `ItemDefinition.mass` |
| 4-layer audio | New service | `WeaponAudioKit` + `ItemDefinition` clip arrays |
| Haptic envelope + hit-confirm | Additive→component | `HapticEnvelope` helper; `ItemDefinition` curves |
| Surface impacts / decals / hitstop / hit tiers | New service | `SurfaceImpactService` + `SurfaceType` tags |
| Sights (iron/red-dot) | New components | attached by `ItemFactory`; `ItemDefinition.sightType` |
| Sway / cone / smoothing | New system | `AccuracyModel` component read by runtimes |
| Moving parts (slide/brass) | New component | `MovingPartRuntime` on `slideTransform` |
| Two-hand hold | New system | secondary attach + `TwoHandGripRuntime` |
| Physical reload | New system | `Magazine` item + `ReloadRuntime` (chamber/mag state) |
| Gravity glove | New runtime | `GravityGloveRuntime` (reuses gravity-gun fan-out) |

---

## 6. Open threads
Resolved by the Round-3 creative round below (§7). Still pending Terry's headset read to sharpen the
Phase-1 feel tuning (§4). Weapon progression/crafting/upgrades = a future round.

---

## 7. CREATIVE ROUND (Round 3) — the arsenal, the non-lethal identity, the novelty
Research on Splatoon, Luigi's Mansion, Ratchet & Clank, Pikmin, Fortnite, Portal, Zelda TotK, Apex,
plus VR (Alyx, Boneworks, Iron Man VR). The convergent thesis: **non-lethal isn't a compromise — it's
a JUICIER design space.** Removing death forces the *moment of impact* and the *aftermath* to carry the
payload, which is exactly what makes these games memorable. This is ZIPTIDE's novelty lane.

### 7.1 The non-lethal feedback palette (ranked)
1. **Coverage / splat** (Splatoon) — every shot *guarantees* a satisfying result (you paint whether or
   not you "hit"); splatter that *persists* turns the world into proof of your presence. Strongest single idea.
2. **Vacuum-capture with resistance** (Luigi's Mansion) — stun, then physically *pull* against a
   flailing body with escalating haptic rumble that cuts to silence on the *POP*. The gravity glove's cousin.
3. **Comedic transform / poof-dissolve** (Ratchet & Clank) — enemies *change state* (8-bit rubble,
   cymbal monkey), bloodless and instantly readable as the silhouette morphs in front of you.
4. **Stun-crackle / freeze / shrink** — state overlays that telegraph "disabled, safe to grab."
5. **Comedic ragdoll + bounce** — downed enemies flop and are "boppable."

### 7.2 The reward loop — disable is the SETUP, capture/salvage is the PUNCHLINE
Never let the stun be the whole beat. Stack three payoffs on the existing scan→stun→grab→throw chain:
- **The tally** (Pikmin/de Blob) — end each room with a visible count: drones salvaged, % cleared, parts.
- **Salvage-as-loot** — a downed drone visibly *breaks open* into grabbable parts/currency on capture,
  so disabling literally *pays better* than a vanish-kill would. This IS the "disable + salvage" identity.
- **Style/combo escalation** — chain captures to a meter that upgrades feedback (bigger POP, more confetti).

### 7.3 Loadout & the VR throw
- **Loadout (no menus mid-fight):** primary = dominant hip/over-shoulder · secondary = opposite hip
  (panic sidearm) · **throwable = chest/bandolier tap** (spawns primed in hand) · **gadget = off-hand
  wrist-radial** (deliberate, so a half-second radial is fine).
- **The VR throw (critical tuning):** release velocity = **smoothed controller velocity over the last
  ~3–5 frames** (NOT instantaneous — that's the "dribbles at your feet" bug), a **~1.3–1.6× multiplier**
  so kids don't windmill, a **predicted-arc dotted line** on prime (Alyx's most-praised throwable upgrade),
  generous collision + soft snap-to-target for young players, and **cook-time OFF by default** (no
  "blew up in my hand" punishment) — impact/timer-on-land detonation instead.

### 7.4 Throwable & gadget menu (all non-lethal, kid-friendly)
**Throwables (ship 4–5 first: Foam, Bubble-Snare, Gravity-Well, Stun, Net):** Foam/Goo Bomb (root for
salvage) · Bubble-Snare (encase + lift, pop to salvage) · Stun Orb (soft daze, no blinding flash) ·
**Gravity-Well** (pull scattered drones into a cluster — extends the gravity identity) · Net Bomb (AoE
capture) · EMP/Shock Pod (`IShockable`) · Smoke/Confetti puff · Decoy Beacon (lure).
**Gadgets (distinct verbs):** Salvage Turret (auto-taser that tags drones) · Grapple/Zip tool (traversal
— ties to the "Ziptide" name) · Bubble Shield Dome (cover) · Scanner Pulse (see drones/salvage through
walls) · Repair/Recharge Beacon · Companion Drone (kid-appealing "pet" that collects salvage).

### 7.5 Novel VR-native mechanics (ranked by delight × feasibility)
1. **Fuse-Snap** — bring two held weapons together and they physically *click* into a stronger third
   (pistol+prism = charge-cannon). The gesture of collision is the input — kid-friendly hand-collision
   crafting nothing else on Quest does. Forge builds the combined visual **for free** (a combo table:
   idA+idB→resultId). **The ZIPTIDE signature.**
2. **One-gun-braces-the-other** — cup the off-hand under a pistol to steady aim/tighten spread/unlock a
   charged shot. Posture = state. (This is also the Axis-1 two-hand accuracy model.)
3. **Palm-blast / repulsor hand** — open-palm push shoves drones back / pops shields; hand orientation
   is the input. Rides the gravity glove. Non-lethal shove fits the identity.
4. **Catch-and-return boomerang blade** — throw the blade, it arcs + disables + returns; you physically
   *catch* it (haptic clack). The catch is a real spatial timing act.
5. **Deflect / bat-back** — swing melee into an incoming drone shot to knock it away or back at the sender.
6. **Ricochet / bank-shot** — bouncing orb-shot to curve around cover, faint predicted-bounce line for kids.
7. **Salvage beam (weapon↔tool crossover)** — the gravity gun's *same* beam that disables also *pulls
   the salvage out* and solves grab-puzzles. One device, two hand-intents.

### 7.6 The "juice" microdetail checklist (small touches = personality; mostly per-recipe DATA)
Holster clink on draw/stow · distinct *seat* haptic when a mag/attachment snaps home · idle fidgets
(barrel-spin / muzzle "breathes" after ~8s) · tap-to-react (tap the gun → it chirps/spins) ·
inspect-on-flip (rotate toward face → wiggle + ammo pips) · charge tells (rising haptic + brightening
muzzle → release pop) · good-shot celebration (confetti-spark + rising ding, bigger on multi-disable) ·
belt "magnet" nudge (items tug toward the holster near your hand — forgiving for kids) · a per-recipe
**weapon "voice"** (a tiny signature chirp so each gun feels like a character).

### 7.7 Signature weapons & moves (the identity players clip and share)
**Weapons:** *Ravel Gun* (goo-coat a drone → gravity-glove reels it in like a fishing line; coverage
doubles as reward) · *Static Lasso* (crackling tether — swing the drone as a physics toy, bank it into
a salvage bin) · *Shrink-Vac* (freeze → miniaturize a downed drone into a pocket collectible) · *Bubble
Brig* (encase → grab → pop over the salvage chute) · **The Reclaimer** (gravity-glove ultimate: yank ALL
nearby stunned drones into a spinning scrap-cyclone that compresses to one big loot drop — the style-meter
payoff, pure Ratchet spectacle, entirely bloodless).
**Moves:** the **Fuse-Smash** · the **boomerang bank-disable** (throw behind cover, catch blind behind
your back) · the **palm-slam → salvage** (shove a drone into a wall, yank its salvage with the beam).

### 7.8 Age-rating guardrails (ESRB E/E10 · PEGI 7)
**DO:** target robots/drones/creatures/ghosts (non-human pulls the rating down) · sci-fi/fantasy weapons
(ink/gravity/vacuum/freeze) · defeat by *disappear/dissolve/poof/capture/power-down* · slapstick exaggeration.
**DON'T:** no blood/gore/wounds/death cries (even "minimal blood" → Teen) · no realistic/contemporary
firearms or human targets reacting realistically · no injury detail — a hit reads as *comedy or capture*,
never suffering. **VR-comfort corollary:** never full screenshake — express impact via flashes/particles/
controller kick + hit-stop instead.

### 7.9 Architecture additions (all data-driven; no parallel system)
- **`ThrowableRuntime`** (ONE runtime) — grip-hold + smoothed-velocity release + arc predictor. Every
  grenade is an `ItemDefinition` config (`effectId` + radius + payload). Foam/Bubble/Stun/Net/EMP/Well =
  configs, **not new code per grenade**. Reuses `IShockable` / the static-net snare path / an inverted
  gravity-gun pulse. **Build this first — the gravity glove shares its release math.**
- **`DeployableRuntime`** — spawns a self-owning prefab (Turret/Shield/Beacon/Decoy/Companion).
- **`WeaponCombiner`** (Fuse-Snap) — listens for two grabbables overlapping + grip-squeeze → resolves a
  combo table → `ItemFactory` builds the result (Forge visual free). Additive.
- **`ReturningProjectile`** (boomerang), a **deflect tag** + swing-velocity collision, **ricochet mode**
  on the pistol — small isolated Gameplay runtimes, reference no Visuals.
- Grapple/Scanner = small new traversal/info runtimes. Most **juice** items = per-`forgeRecipeId` data.
- **Report-only gates (per CLAUDE.md):** anything touching inventory persistence, holster travel, or
  input actions (snap-on attachments, holster loadout).

### 7.10 What to prototype first (recommendation)
1. **Phase-1 feel trio** (§4) — needs Terry's headset read to tune, but it's the ~80% feel jump.
2. **`ThrowableRuntime` + arc predictor** — unlocks the whole grenade menu AND the gravity glove in one core.
3. **Fuse-Snap** — the cheapest *signature* novelty (Forge does the visual), the clip-and-share identity.
4. Then the gravity glove + salvage-as-loot break-open (the reward loop that defines the game).
