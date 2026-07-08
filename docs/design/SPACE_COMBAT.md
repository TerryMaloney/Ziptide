# SPACE COMBAT — weapons, abilities, battles, salvage (design doc)

> **Hardwiring §6 / Phase 3.** Non-lethal ship-to-ship combat + salvage in the space world. Status:
> skeleton for Fable 5. Depends on §4 (hardpoints) + §5 (flight + space world). Pairs with
> `SPACEFLIGHT_PHYSICS.md`, `ABILITIES_AND_ARSENAL.md`.

## Locked decision
**Non-lethal disable + salvage** — EMP / armor-break, then salvage the wreck into the economy.
Consistent with the all-ages / non-lethal canon and the armor-only combat model.

## What to build
- [ ] **`ShipWeaponDefinition`** — hardpoint-mounted, data-driven (blaster, missile, beam, mining/
  salvage laser), consistent with ground `ItemDefinition` philosophy.
- [ ] **Ship ability system** — boost, shield, EMP, cloak, deployable drone, tractor/salvage beam;
  button-mapped, cooldown-based, comfort-safe.
- [ ] **Damage = armor-only** — recharging armor, no health bar (locked combat model). Disable, don't
  destroy; disabled enemy ships become salvage.
- [ ] **Enemy ship AI** — adapt `BotBrain` (pursue / evade / strafe / formation / retreat) to 3D flight;
  turrets; a capital-ship set-piece with weak-point targeting.
- [ ] **Space encounters as POIs** — patrols, ambushes, derelict salvage, escort — stamped into the
  space world; rewards route through `ProfileEconomy` (salvage → resources → ship upgrades → §4 loadout).

## Consistency-spine hooks
Ship weapons/abilities are Definitions-by-id; enemy AI reuses `BotBrain`; damage reuses the armor model;
salvage feeds the one economy; encounters are POIs in a world scene.

## Technique research TODO (cite at build time)
VR space-combat comfort (targeting without nausea, auto-aim assist), 3D dogfight AI, non-lethal
disable/salvage loops.

## Tests + audit
Weapon/ability resolution deterministic; armor model golden test; enemy-AI state machine tests; salvage
grants correct economy entries.

## 🚀 Room to expand
Wingmen / squad orders; boss capital ships with multi-phase weak points; space hazards (nebulae,
minefields) as arena events; bounty/faction layer tying to Tidefront; co-op space missions over the
Photon seam. Keep it non-lethal and spectacle-forward.
