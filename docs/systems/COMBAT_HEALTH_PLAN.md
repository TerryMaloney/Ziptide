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

## 2. THE MODEL — DECIDED (Terry, 2026-07-07)

**Armor-only. No health bar at all.** The player has a single **recharging armor meter** and nothing
underneath it:
- **Armor is your only defense.** Taking a hit drains armor by the attacker's damage amount. A hit never
  kills *while you still have armor* — even an overkill hit just empties the meter to 0 (armor "breaks").
- **Broken armor = one hit from death.** Once armor is at 0, the **next** hit is **immediate death**.
  This gives a crystal-clear, comfort-friendly warning state: armor up = trading; armor broken = get to
  cover NOW. (Tunable: whether the emptying hit itself can kill is a one-line flag; default is NO — you
  always get the "broken, run" beat.)
- **Armor recharges** on its own after a short out-of-combat delay (the "break contact and recover" loop).
  No health packs, no pickups to place, no inventory item to manage — the recharge *is* the economy.
- Weapon/enemy damage still matters: bigger weapons and harder aliens **drain more armor per hit**, so
  they break your armor in fewer hits. The per-weapon / per-enemy damage design (Phase A) is what tunes
  "how many hits until I'm broken."

**Death consequence — checkpoint respawn, SERVERLESS (no metadata server).** Reuse what every scene
already has: the **`__SPAWN_PLAYER` marker** (`SpawnMarkerRuntime`) that travel-arrival already teleports
you to. Death = teleport back to the **current world's spawn marker**, armor refilled, brief
spawn-protection (`PvpRules.SpawnProtectionSeconds` already exists). Zero server, zero new metadata — the
checkpoint is the world entry point, which is authored per scene today.
- **If we want mid-world checkpoints later** (still serverless): drop extra `SpawnMarkerRuntime`-style
  nodes in a scene and store just the **last-touched checkpoint id (a string)** in the existing local
  `SaveSystem` profile (local JSON/PlayerPrefs on the headset — no server). Start with "respawn at scene
  spawn"; add the string-in-local-save only if playtests want finer checkpoints.

---

## 3. The phased build (each phase is one reviewable slice; A is CI-provable, B/C are device-feel)

### Phase 0 — Melee held like weapons of the arm ✅ SHIPPED (`8f93847`)
Guns get a +45° aim tilt so the barrel points where your finger points; the Breaker Blade and Tide Pike
were silently inheriting it. Decoupled: `ItemFactory.PoseGrip` now takes a per-weapon default — blade
rides above the fist (+70°, a raised blade), pike sits flatter (+30°, tip-leads-the-thrust), both
overridable via `ItemDefinition.gripLocalEuler`. **Device-verify the exact angles** (art track — the
forge photo booth shows the mesh, not the hold).

### Phase A — Unify the damage core + the armor meter (pure C#, fully CI-testable, no scene/device work)
The safe foundation. No behavior on device changes until it's wired in B.
1. **New pure type `ArmorMeter`** (Unity-free; put it beside `PvpCombatant` in `Multiplayer`, or in `Core`
   if `Gameplay` needs it without a `Multiplayer` ref). Fields: `Charge`, `MaxCharge`, `RegenPerSec`,
   `RegenDelayAfterHitSec`. Methods: `ApplyDamage(int) → DamageResult` and `Tick(float dt)`.
   - **The rule (Terry's model):** if `Charge > 0`, drain it (clamp at 0) and return `Absorbed` — never a
     kill, even on overkill. If `Charge == 0` when the hit lands, return `Killed`. A `bool
     lethalOnBreak = false` flag covers the "does the emptying hit itself kill" variant (default no).
   - `Tick` refills toward `MaxCharge` at `RegenPerSec`, but only after `RegenDelayAfterHitSec` has passed
     since the last hit. `Reset()` refills for respawn.
2. **One canonical damage scale.** Adopt the PvP integer scale (`PvpRules`, ~6-ish) as the single truth.
   Make `CreatureRuntime` stop using its hardcoded `TaserDamage=10f/GravityDamage=8f` and read damage from
   data (step 3). Re-baseline `CreatureDefinition.maxHealth` onto the same integer scale so a number means
   one thing against a creature, a PvP player, and the campaign player's armor.
3. **Add `ItemDefinition.damage` (int)** — the single per-weapon source of truth. It replaces the hardcoded
   constants in `CreatureRuntime` and mirrors/feeds the `PvpRules` table. Damage becomes data on each
   weapon's `ItemDefinition` asset, authored exactly like `gripLocalEuler`/`muzzleLocalPos` already are.
4. **Tests (green CI = Phase A proven):** unit-test `ArmorMeter` — absorb while charged, kill only when
   hit at 0, regen after the delay, `Reset` refills. Add a guard test that every `ItemDefinition.damage`
   is > 0 and every `CreatureDefinition.damage/maxHealth` is on-scale (WiringValidator-style both-sides
   guard for the new damage seam). Leave `PvpCombatTests` green — do NOT change PvP numbers.

### Phase B — The player has armor, breaks, and dies to checkpoint (device-verified feel)
1. **`PlayerArmor`** — a component `PlayerStunReceiver` ensures/hosts on the rig (it already owns the head,
   the screen flash, and the damage-direction tracer). Holds the Phase-A `ArmorMeter`, ticks its regen,
   drives the HUD, and owns spawn-protection. Flips the old *"NO health, NO death"* note — the player is
   now killable everywhere. Armor-break should read loudly (flash + audio cue: "armor down").
2. **Wire enemy → player damage.** `CreatureDefinition.damage` finally does something: a creature that
   reaches the player (contact for Swarmers/Bruisers, projectile for Flyers/drones) calls
   `PlayerArmor.ApplyDamage(def.damage)`. Reuse the existing drone-bolt path — it already homes on
   `PlayerStunReceiver.HitPoint`. Stun (taser/net) stays separate and non-lethal as it is today.
3. **Death → checkpoint respawn (serverless).** On `Killed`, teleport the rig to the current scene's
   `__SPAWN_PLAYER` (`SpawnMarkerRuntime`) via the **existing** respawn plumbing (`WorldRuntime.RespawnPlayer`
   / `FallRespawner` / `EmergencyRespawn`), refill armor, apply `SpawnProtectionSeconds`. No server, no new
   metadata, no new scene-load path — `TravelCoordinator` stays the only travel entry. (Mid-world
   checkpoints later = extra spawn nodes + a last-checkpoint-id string in the local `SaveSystem` profile.)
4. **Armor HUD.** A world-space armor meter (mirror the `CreditsHud` ensure pattern); the break state is
   the important read (armor up vs broken-one-hit-from-death). Damage flash reuses the stun flash.
   Device-verify readability in the headset.

### Phase C — Enemy variety + per-planet difficulty (leans on what's built)
1. **Non-alien "machine" family** — formalize the existing **drones** into a machine line (drone, turret,
   sentinel) so organic aliens aren't the only threat. Authored via the existing `CreatureVariantAuthor` /
   drone path; minimal new art (they exist).
2. **Per-world difficulty multiplier** from `CityLayoutDefinition.tier` — scales creature
   `maxHealth`/`damage`/zone `count` so tier-2 planets are genuinely harder without new enemy code.
   🟡 **PARTIAL (2026-07-09):** the pure math (`Content/DifficultyScale` — HealthMult/DamageMult/
   ScaledHealth/ScaledDamage, tests) is shipped, and `CreatureRuntime` consumes it via a
   `difficultyTier` field (0 = neutral, so no behavior change yet). **Boarded follow-up (worlds track):**
   the spawner (`CityBuilder.MakeCreature`) sets `difficultyTier` from the world/zone tier — one wiring
   line; then a tier-2 planet's creatures scale up automatically.
3. **Multi-archetype zones** — a planet spawns 2–3 creature IDs (mix Swarmer + Flyer + a machine), not one
   repeated body, so encounters stop feeling same-y. Author a couple of new creature IDs + machine
   variants; the string-ID registry (`CreatureRuntime` ← `Resources/Enemies/*`) already supports it.

---

## 4. What already-built systems this touches (the honest blast radius)
- `PlayerStunReceiver` — biggest behavioral change: the player becomes killable in **every** world.
- `CreatureRuntime` — drops its hardcoded `10f/8f`; damage + health become the unified scale.
- `PvpCombatant` / `PvpRules` — the damage scale they define becomes the single truth; **PvP numbers stay
  put**, guarded by the existing `PvpCombatTests`. `ArmorMeter` is a NEW sibling type, not a rewrite of
  `PvpCombatant` (PvP keeps its int-HP pool; the campaign player uses armor).
- **Every weapon runtime** (Pistol/Taser/Gravity/Arena/Melee) — damage moves from code to
  `ItemDefinition.damage` data.
- HUD — new armor-meter readout (break state is the key read).
- Respawn plumbing — reused, not rebuilt (checkpoint = the scene's `__SPAWN_PLAYER`, serverless).
- `CityLayoutDefinition` + city spawn — gains the difficulty multiplier + multi-archetype zones.

## 5. Guards / risks
- **Unifying two damage scales can silently rebalance PvP.** Pin every PvP number with `PvpCombatTests`
  before touching the scale; treat a red there as a blocker.
- **A killable player in VR must feel fair, not punishing** — comfort-first checkpoint respawn, generous
  armor regen, a LOUD armor-break tell, clear damage-direction feedback (the tracer already exists). Tune
  on device.
- **Circuit breaker applies**: Phase B/C are device-feel; if a change can't be CI-verified, it's Terry's
  on-headset call, and 3 CI-reds on one slice → stop and escalate.
- Phase A is the de-risker: it's pure C#, so the whole economy unification lands **green in CI** before a
  single scene or the headset is involved.
