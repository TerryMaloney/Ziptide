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

## 6. Open threads (Round 3+ research)
Secondary weapons & loadout shape · grenades/throwables (incl. non-lethal: stun/foam/bubble/net) ·
kid-friendly-but-satisfying weapon feel · novel VR-native mechanics (dual-wield, morphing guns,
snap-on attachments, catch-and-return, gesture weapons) · weapon progression/crafting/upgrades ·
the "juice" microdetails (inspect, reload flourishes, holster clinks). Pending Terry's headset read to
sharpen Phase-1 tuning.
