# ARCHITECT HANDOFF — Combat / Armor / Damage (start with Phase A)

**You are taking over implementation of the combat-health system.** The design is DECIDED and the melee
grip is already shipped. Your job: build it, keeping CI green. Read this top to bottom, then read
`docs/systems/COMBAT_HEALTH_PLAN.md` (the full design) once.

**Branch:** `terry-local-wip` (what Terry's headset builds from — the ONLY branch; `git pull --rebase`
first, small commits, push here). **Verify:** you can't run Unity/headset; CI (EditMode tests, push-
triggered) is your compile+test proof. Log a `Did/Next/Heads-up/Commit` entry in `docs/HANDOFF.md` in the
same commit you push. Read `CLAUDE.md` for the laws (spec-is-truth, pure-core-first, circuit breaker:
3 CI-reds on one task → stop and escalate to Terry).

---

## THE DECIDED MODEL (do not re-litigate)
- **Armor only. No health bar.** The player has ONE recharging **armor meter** and nothing under it.
- A hit **drains** armor by the attacker's damage. While armor `> 0` a hit **never kills** (overkill just
  empties it to 0 — "armor breaks").
- **Armor at 0 + next hit = immediate death.** (Default: the emptying hit itself does NOT kill — you
  always get the "broken, run for cover" beat. One flag toggles this.)
- **Armor recharges** on its own after a short out-of-combat delay. No health packs, no pickups.
- **Death → checkpoint respawn, SERVERLESS:** teleport to the current scene's `__SPAWN_PLAYER`
  (`SpawnMarkerRuntime`), refill armor, brief spawn-protection. No server, no metadata — the checkpoint is
  the world entry point every scene already has.

## Why the code needs this (the situation you're inheriting)
Two disconnected damage economies, and the player is in neither:
- **Campaign:** `CreatureRuntime._health` (float, from `CreatureDefinition.maxHealth`≈30), weapon damage
  **hardcoded** (`TaserDamage=10f`, `GravityDamage=8f`). Player = `PlayerStunReceiver` which is explicitly
  *"NO health, NO death"* (stun+slow only). `CreatureDefinition.damage` (5f) is authored but **never
  applied** to anyone.
- **PvP:** `PvpCombatant` (int `Health`, `PvpRules.MaxHealth=6`), weapon damage in the `PvpRules` **table**
  (taser 2, gravity 1, prism 3, pike 2, blade 1…). Pure, Unity-free, unit-tested (`PvpCombatTests`).
Same taser = 10 to a creature but 2 to a PvP player, and 0 to the campaign player. Phase A makes ONE scale
the truth; Phase B gives the player armor on that scale.

---

## PHASE A — pure C#, no scenes, no device. CI proves it.
Files you'll touch are all pure logic + tests — nothing that needs a headset.

> **STATUS: PHASE A IS DONE.** A1/A2/constants shipped earlier; **A3 shipped 2026-07-09** (Picasso). The
> whole damage economy is now unified in code + CI-green. `ArmorMeter` (+11 tests),
> `PvpRules.PlayerArmor/ArmorRegenPerSec/ArmorRegenDelaySec`, `ItemDefinition.damage`, and now A3:
> `CreatureRuntime` routes every weapon through `PvpCombatant.DamageFor` (no more hardcoded 10/8 or the
> arsenal-all-does-8 bug), creature health re-baselined onto the integer scale via `CreatureBaselines` +
> a version-guarded `CreatureStatRebaseline` migration (auto-runs from the build-hooked
> `CreatureVariantAuthor`; idempotent; never clobbers hand-tuning). **YOUR JOB NOW = Phase B.**
> Creature time-to-kill is Terry's on-device tune (baselines in `CreatureBaselines.HealthFor`).

### A1. `ArmorMeter` — new pure type (no Unity) ✅ DONE
Create `Ziptide/Assets/Ziptide/Multiplayer/Runtime/ArmorMeter.cs` (namespace `Ziptide.Multiplayer`, next
to `PvpCombatant.cs`). Keep it Unity-free so it's deterministic + testable.
```
public enum DamageResult { Absorbed, Broke, Killed }
public class ArmorMeter {
    public int MaxCharge; public int Charge;            // Charge starts == MaxCharge
    public float RegenPerSec; public float RegenDelayAfterHitSec;
    public bool LethalOnBreak = false;                  // default: the emptying hit does NOT kill
    float _regenBlockedUntil;   // set on each hit; regen only runs after it
    // ApplyDamage: Charge==0 -> Killed. Charge>0 -> drain; if it reaches 0 -> Broke (or Killed if LethalOnBreak). else Absorbed.
    public DamageResult ApplyDamage(int amount, float now);
    public void Tick(float now, float dt);              // refill toward MaxCharge after the delay
    public void Reset();                                // refill for respawn
}
```
Pull the starting numbers into `PvpRules.cs` (it's the tuning home): e.g. `PlayerArmor = 4`,
`ArmorRegenPerSec = 1.0`, `ArmorRegenDelaySec = 3.0`. Tune on device later.

### A2. `ItemDefinition.damage` — the single per-weapon damage source ✅ DONE
Field added (`Ziptide/Assets/Ziptide/Content/Runtime/Items/ItemDefinition.cs`, default `0`). Consume it in
A3 read-through style: `def.damage > 0 ? def.damage : PvpCombatant.DamageFor(kind)`. Keep
`PvpCombatant.DamageFor` as the fallback so PvP is untouched.

### A3. Retire `CreatureRuntime`'s hardcoded damage  ⟵ START HERE
In `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs`, replace `TaserDamage=10f` /
`GravityDamage=8f` with the unified scale (read from the weapon's `ItemDefinition.damage` / `DamageFor`).
Re-baseline `CreatureDefinition.maxHealth` defaults onto the integer scale (e.g. small integers, not 30f)
so "one number, one meaning." Keep `ReceiveHit(PvpWeapon,…)` and the non-lethal disable exactly as they
are otherwise.

### A4. Tests (this is the acceptance gate)
Add `Ziptide/Assets/Ziptide/Tests/EditMode/ArmorMeterTests.cs`:
- absorbs while charged (never Killed while Charge>0), returns Broke at exactly 0, Killed on a hit at 0.
- regen holds for `RegenDelayAfterHitSec`, then refills to `MaxCharge`; `Reset()` refills.
Extend an existing test (or add one) asserting every `ItemDefinition.damage ≥ 0` and creature stats are
on-scale. **DO NOT touch `PvpCombatTests` numbers** — PvP stays green as-is.
**Phase A is done when CI is green.** That's the whole proof; no headset needed.

---

## PHASE B — player takes damage / dies (needs Terry on the headset for feel)
Do NOT start B until A is green. B changes device behavior.
1. `PlayerArmor` component, ensured/hosted by `PlayerStunReceiver`
   (`Gameplay/.../Player/PlayerStunReceiver.cs` — it already owns the head, screen flash, damage tracer,
   and is ensured on the rig by `PlayerRigPersistence`). Holds an `ArmorMeter`, ticks regen, owns
   spawn-protection, drives the HUD. Loud armor-break tell (flash + audio via `AudioDirector`).
2. Enemy→player damage: a creature reaching the player calls `PlayerArmor.ApplyDamage(def.damage)`. Reuse
   the drone-bolt path (already homes on `PlayerStunReceiver.HitPoint`). Taser/net stun stays separate +
   non-lethal.
3. Death → `WorldRuntime.RespawnPlayer` / `FallRespawner` / `EmergencyRespawn` to the scene
   `__SPAWN_PLAYER`; refill armor; `PvpRules.SpawnProtectionSeconds`. No new scene-load path.
4. World-space armor HUD (mirror `CreditsHud` ensure pattern). Queue the headset feel-check in
   `docs/TERRY_RUNBOOK.md`.

## PHASE C — enemy variety + difficulty (after B)
Formalize existing **drones** into a non-alien "machine" line (drone/turret/sentinel) via
`CreatureVariantAuthor`; a per-world difficulty multiplier off `CityLayoutDefinition.tier` scaling
creature `maxHealth`/`damage`/zone `count`; multi-archetype zones (2–3 creature IDs per planet). Full
detail in `COMBAT_HEALTH_PLAN.md` §3 Phase C.

## Guardrails
- Pure-core-first: A is all deterministic C# — land it green before B.
- Don't rewrite `PvpCombatant`; `ArmorMeter` is a **new sibling**. PvP numbers frozen (`PvpCombatTests`).
- No `SceneManager.LoadScene` in gameplay — `TravelCoordinator` is the only travel entry; respawn reuses
  existing plumbing.
- Circuit breaker: 3 CI-reds on one slice → stop, write it up in HANDOFF, escalate to Terry.
- Melee grip angles (blade +70°, pike +30°) are reasoned starting points, NOT device-verified — leave a
  note for Terry's headset pass; don't "fix" them blind.
