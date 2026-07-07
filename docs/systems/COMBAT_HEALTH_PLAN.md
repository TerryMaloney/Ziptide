# COMBAT · HEALTH · ENEMY VARIETY — the solid plan

**Status:** PROPOSAL awaiting Terry's two design decisions (§2). Phase 0 (melee grip) is already shipped
(commit `8f93847`). Everything else below is planned, not built.
**Author:** Picasso / Opus 4.8, 2026-07-07, on `terry-local-wip`.
**Read with:** `docs/WIRING_MAP.md` (§3 the seam table), `PvpRules.cs`, `PvpCombatant.cs`,
`CreatureRuntime.cs`, `CreatureDefinition.cs`, `PlayerStunReceiver.cs`.

---

## 1. Why this, and what's actually in the code today

Terry's brief: *"other than getting tased and stunned we can't just have more characters be invincible
forever… the fortnite model has shields which could be interesting… weapons need to do a certain amount
of damage depending on the weapon and different aliens a different amount… enemies that aren't aliens so
that doesn't get overused… more than one type of alien on different planets to adjust the hardness… that
would take adjusting a lot of stuff that's already built so I need a solid plan."*

He's right that it touches a lot. The root reason: **the game has two disconnected combat economies**,
and **the player is in neither** — the player literally cannot be hurt.

| | Campaign (creatures) | PvP (arena) |
|---|---|---|
| Health | `CreatureRuntime._health` — **float**, from `CreatureDefinition.maxHealth` (default **30**) | `PvpCombatant.Health` — **int**, `PvpRules.MaxHealth` = **6** |
| Weapon damage | **hardcoded in `CreatureRuntime`**: `TaserDamage=10f`, `GravityDamage=8f` | **data table in `PvpRules`**: taser 2, gravity 1, prism 3, pike 2, blade 1… |
| Player health | **none** — `PlayerStunReceiver` says *"NO health, NO death"*; only stun+slow | `PvpCombatant` (the player IS a combatant) |
| Enemy→player damage | **`CreatureDefinition.damage` (5f) is authored but never applied** — creatures only move | n/a |

So a taser does **10** to a creature but **2** to a PvP player — the same trigger pull means different
things in different modes, and does **nothing** to the campaign player because there's no player health to
subtract from. That inconsistency is the "adjusting a lot of stuff" Terry sensed. The fix is to make
**one damage scale** the truth everywhere and give the player a real (recoverable) health pool.

**The good news — most of the machinery already exists and is reusable:**
- `PvpCombatant` is already a **pure, Unity-free, unit-tested** health pool (`ApplyDamage → bool killed`,
  `Respawn`, `DamageFor(weapon)`), covered by `PvpCombatTests`. It is the natural template for a shared
  core both the player and creatures use.
- `PlayerStunReceiver` already lives on the persistent rig (ensured by `PlayerRigPersistence`), already
  tracks the head/hit-point and draws the incoming-fire tracer. It is the right host for player health.
- `CreatureDefinition` already carries `maxHealth` **and** `damage` — the per-enemy knobs already exist;
  `damage` just isn't wired to hurt anyone yet.
- **Drones already exist as non-organic enemies** — the "enemies that aren't aliens" line is a
  formalization of something built, not new art from scratch.
- `CityLayoutDefinition.tier` (0 early / 1 mid / 2 capstone) + per-zone `count`/`respawnDelay` is already
  the difficulty knob; nothing new is needed to scale hardness per planet.

---

## 2. TWO DECISIONS I need from Terry before Phase B (everything else is settled)

**Decision 1 — the health model** (Terry leaned Fortnite):
- **(A· recommended) HP + regenerating shield.** Shield sits on top of HP and **regenerates after a few
  seconds out of combat** (the "break contact / get to cover" loop). HP does **not** passively heal — it
  comes back slowly or via pickups. This gives the exact feel he described: you can't be invincible, but a
  smart player recovers. Most depth, and it's the model he already likes.
- (B) Simple regenerating HP, no shield — HP heals after N seconds without being hit. Simplest; less to
  manage; no shield resource.
- (C) HP + pickups, no passive regen — most punishing; forces looting; heaviest on level design.

**Decision 2 — the death consequence** (how punishing):
- **(A· recommended) Forgiving respawn** at the world entry / last checkpoint, **drop nothing** in
  campaign, brief spawn-protection (already have `PvpRules.SpawnProtectionSeconds`). Best for solo VR /
  comfort; escalate punishment later per world tier if it's too soft.
- (B) Drop-loot-on-death (recover it where you fell) — a stakes bump without a hard reset.
- (C) Restart-the-encounter / world — most punishing; risky for VR fatigue.

My defaults if he doesn't answer: **1A + 2A** (Fortnite-style shield, forgiving respawn). They're the most
in line with his words and the least likely to feel bad in a headset.

---

## 3. The phased build (each phase is one reviewable slice; A is CI-provable, B/C are device-feel)

### Phase 0 — Melee held like weapons of the arm ✅ SHIPPED (`8f93847`)
Guns get a +45° aim tilt so the barrel points where your finger points; the Breaker Blade and Tide Pike
were silently inheriting it. Decoupled: `ItemFactory.PoseGrip` now takes a per-weapon default — blade
rides above the fist (+70°, a raised blade), pike sits flatter (+30°, tip-leads-the-thrust), both
overridable via `ItemDefinition.gripLocalEuler`. **Device-verify the exact angles** (art track — the
forge photo booth shows the mesh, not the hold).

### Phase A — Unify the damage/health core (pure C#, fully CI-testable, no scene/device work)
The safe foundation. No behavior on device changes until it's wired in B.
1. **Generalize `PvpCombatant` → a shared `HealthPool`** (keep it Unity-free, in `Multiplayer` or a new
   `Core` type so `Gameplay` + `Multiplayer` both reference it). Adds an **optional shield layer**
   (`Shield`, `MaxShield`, damage spills shield→HP, `RegenShield(dt)`), keeps `ApplyDamage → bool killed`
   and `Respawn`. `PvpCombatant` becomes a thin wrapper (or subclass) so **PvP balance and its existing
   tests are untouched**.
2. **One canonical damage scale.** Pick the PvP integer scale (6-HP-ish) as the truth OR a new shared
   scale, and make `CreatureRuntime` use it instead of its hardcoded `10f/8f`. Re-baseline creature
   `maxHealth` to the same scale so numbers mean one thing everywhere.
3. **Add `ItemDefinition.damage` (int)** — the single per-weapon source of truth, replacing both the
   hardcoded constants in `CreatureRuntime` and (feeding) the `PvpRules` table. Every weapon's damage
   becomes data on its `ItemDefinition` asset (authored the same way grip/muzzle already are).
4. **Tests:** extend `PvpCombatTests` for the shield spill + regen math; add a test that every
   `ItemDefinition.damage` is > 0 and every `CreatureDefinition.damage`/`maxHealth` is on-scale. This is
   the WiringValidator-style both-sides guard for the new damage seam. **Green CI = Phase A proven.**

### Phase B — The player can be hurt, and can die (device-verified feel; needs Decision 1 + 2)
1. **`PlayerHealth`** — a component `PlayerStunReceiver` ensures/hosts on the rig (it already owns the
   head, the flash, and the damage-direction tracer). Holds the `HealthPool` from Phase A (HP + shield per
   Decision 1), regen timers, and spawn-protection. Flips the old *"NO health, NO death"* note.
2. **Wire enemy → player damage.** `CreatureDefinition.damage` finally does something: a creature that
   reaches the player (contact for Swarmers/Bruisers, projectile for Flyers/drones) calls
   `PlayerHealth.ApplyDamage(def.damage)`. Reuse the existing drone-bolt path — it already homes on
   `PlayerStunReceiver.HitPoint`.
3. **Death → respawn.** On HP 0, route through the **existing** respawn plumbing (`FallRespawner` /
   `WorldRuntime.RespawnPlayer` / `EmergencyRespawn`) per Decision 2. No new scene-load path —
   `TravelCoordinator` stays the only travel entry.
4. **Health/shield HUD.** A world-space readout (mirror the `CreditsHud` ensure pattern) — shield bar over
   HP bar, damage flash reuses the stun flash. Device-verify readability in the headset.

### Phase C — Enemy variety + per-planet difficulty (leans on what's built)
1. **Non-alien "machine" family** — formalize the existing **drones** into a machine line (drone, turret,
   sentinel) so organic aliens aren't the only threat. Authored via the existing `CreatureVariantAuthor` /
   drone path; minimal new art (they exist).
2. **Per-world difficulty multiplier** from `CityLayoutDefinition.tier` — scales creature
   `maxHealth`/`damage`/zone `count` so tier-2 planets are genuinely harder without new enemy code.
3. **Multi-archetype zones** — a planet spawns 2–3 creature IDs (mix Swarmer + Flyer + a machine), not one
   repeated body, so encounters stop feeling same-y. Author a couple of new creature IDs + machine
   variants; the string-ID registry (`CreatureRuntime` ← `Resources/Enemies/*`) already supports it.

---

## 4. What already-built systems this touches (the honest blast radius)
- `PlayerStunReceiver` — biggest behavioral change: the player becomes killable in **every** world.
- `CreatureRuntime` — drops its hardcoded `10f/8f`; damage + health become the unified scale.
- `PvpCombatant` / `PvpRules` — become (or feed) the shared `HealthPool`; guard PvP balance with the
  existing `PvpCombatTests` so nothing silently re-tunes.
- **Every weapon runtime** (Pistol/Taser/Gravity/Arena/Melee) — damage moves from code to
  `ItemDefinition.damage` data.
- HUD — new health/shield readout.
- Respawn plumbing — reused, not rebuilt.
- `CityLayoutDefinition` + city spawn — gains the difficulty multiplier + multi-archetype zones.

## 5. Guards / risks
- **Unifying two damage scales can silently rebalance PvP.** Pin every PvP number with `PvpCombatTests`
  before touching the scale; treat a red there as a blocker.
- **A killable player in VR must feel fair, not punishing** — comfort-first respawn, generous shield
  regen, clear damage-direction feedback (the tracer already exists). Tune on device.
- **Circuit breaker applies**: Phase B/C are device-feel; if a change can't be CI-verified, it's Terry's
  on-headset call, and 3 CI-reds on one slice → stop and escalate.
- Phase A is the de-risker: it's pure C#, so the whole economy unification lands **green in CI** before a
  single scene or the headset is involved.
