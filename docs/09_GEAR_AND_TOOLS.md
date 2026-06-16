# 09 — GEAR AND TOOLS

**Idea bank for Ziptide's explorer-tech gear. Design-only. Nothing here is
implemented unless STATUS.md says so.**

> Source brainstorm: `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/01_potential_weapons_and_tools.md`
> and `02_starter_gear_loop_next_three.md`. This module is the curated, categorized
> distillation — not every idea becomes a task. Convert an idea into a real task
> only by promoting it into `docs/04_TASK_QUEUE.md`.

---

## Design direction (the rule)

- **No bullets as the default fantasy.** Gear should feel like explorer tech:
  non-lethal, modular, physical, readable in VR, useful for combat *and* puzzles.
- Favor electricity, gravity, bubbles, light, sound, foam, drones, shields,
  scanning, and strange alien tools.
- **Creatures drop materials, not weapons** — samples, crystals, charge organs,
  shell fragments, Bloom residue → used to craft/upgrade tech.
- **Move objects and enemies before you move the player.** Player-locomotion
  gadgets are high VR-comfort risk and come later.

---

## Modular structure (so this stays data-driven)

Build tools as modular tools, not one-off player scripts. Keep names/boundaries
ready for a `ToolRecipe` + registry pattern even if the first pass is direct
MonoBehaviours. Forward-referenced schema lives in
[`docs/06_SCHEMAS.md`](06_SCHEMAS.md) → *ToolRecipe (design-only)*.

```text
ToolRecipe
- toolId
- displayName
- handSlot
- cooldown
- energyCost
- activationGesture
- targetRules
- effectRules
- visualEffectId
- soundEffectId
```

VR interaction direction (body-language mechanics, not controller mappings):
right hand → left wrist = scan; raise palm = gravity aim; grip = pull/hold;
flick/release = throw; aim right hand = fire dart; hand-to-belt/chest = future
gear switch.

---

## Idea bank (categorized)

### Combat / crowd control
- **Taser / Stun Dart** — starter. Variants: single, chain, anchor, pulse, tag, overcharge.
- **Static Net** — throwable electric grid; catch fliers, reveal invisibles, electrify doorways.
- **Sonic Thumper** — short-range push; break crystals, disorient fliers, clear swarms.
- **Stun Blaster** — slow glowing bolts, stun status, charge-shot knockback.
- **Arc Rifle** — two-handed chaining electric beam; can power machines.
- **Pulse Cannon** — large charged shot; push, open heavy doors, break shields.

### Object manipulation / puzzle
- **Gravity Glove** — pull/push/suspend/throw objects; place batteries; hold stunned drones.
- **Magnet Lasso** — tether for metal objects, trip mechs, yank shields, swing cargo.
- **Bubble Snare Launcher** — trap/slow enemies; hold puzzle objects; pop for knockback.
- **Foam Cannon** — hard-light foam: stick enemies, block tunnels, patch leaks, make platforms.
- **Plasma Cutter** — short beam to cut vines/cables/doors/crystals (more tool than weapon).
- **Tide Glove** *(signature)* — wave-force in air: push enemies, move floats, clear fog, redirect projectiles.
- **Echo Seed** *(signature)* — repeats player's last action (hold plates, repeat a pulse, distract).

### Utility / scanning / light / thermal
- **Left Wrist Scan Pulse** — reveal resources, sockets, weak points, hidden paths.
- **Prism Beam** — light tool/weapon; reflect, split through crystals, burn Bloom, light-locks.
- **Cryo Sprayer** — freeze/slow enemies, stop machines, temporary ice surfaces.
- **Heat Lance** — melt ice, burn vines, thermal switches, overheat armor.
- **Energy Shield Disc** — thrown boomerang shield; block, ricochet, hit distant switches.

### Companion / drones
- **Orbital Pebble / RILL Orb** — shield orbit, light, scan, zap, decoy, fetch.
- **Drone Launcher** — deploy zap / shield / repair / scan / lure drones.
- **RILL Projector** *(signature)* — companion as tactical tool: decoy, flashlight, scan, shield, lure.

### Biotech / alien
- **Bloom Splicer** — rewrite hostile vines to bridges, convert nests to pods, open organic doors.
- **Bloom Tamer** *(later)* — temporarily control a small creature.

### Traversal / belt (high VR-comfort risk — gate carefully)
- **Drift Belt** — short low-g glide (strict comfort limits).
- **Bubble Lift Capsule**, **Hover Shell**, **Zipline Seed**, **Blink Buoy**,
  **Pocket Platform**, **Phase Umbrella**, **Gravity Flip Charm** *(snap zones only)*.
- **Mini Gate Puck** *(signature)* — small object portal first; player portals are much harder.

---

## Difficulty guide

| Easy | Medium | Hard |
|------|--------|------|
| Simple stun projectile | Chain lightning | Wall walking |
| Scan pulse | Bubble trap | Portals |
| Simple shield bubble | Gravity push/pull | Swing/tether physics |
| Basic cooldown items | Deployable drone | Creature taming |
| Simple beam | Hover belt | Multi-phase cinematic powers |
| Throwable pulse grenade | Reflecting prism / pocket platform | Anything that rapidly moves the VR player |

---

## Near-term focus: Starter Gear Loop

The agreed first three tools (the complete VR loop): **scan → stun → gravity-grab
→ place/throw → reward**. Full task breakdown lives in
[`docs/04_TASK_QUEUE.md`](04_TASK_QUEUE.md) → *Future Sandbox Milestone — Starter Gear Loop*.

1. **Left Wrist Scan Pulse** — `Scannable`, `ScanPulseTool`, `WristScannerActivator`.
2. **Expanded Stun Dart** — `Stunnable`, `StunDartTool`, `StunDartProjectile`, `StunStatusEffect`.
3. **Gravity Glove** — `GravityGrabbable`, `GravityGloveTool`, `GravityHoldPoint`, optional `GravityTargetFilter`.

Combo rule: normal enemy = not grabbable; **stunned** enemy = grabbable; heavy
enemy = immune / partial tug. Comfort rule: first version moves objects/enemies,
**not** the player's body.

---

*Keep this an idea bank. Promote ideas to tasks deliberately; don't let the list
cause scope explosion.*
