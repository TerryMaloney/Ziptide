# 04 — TASK QUEUE

**Current and upcoming tasks.**

---

## In progress

- *(None yet.)*

---

## Next up

- *(Pull from STATUS.md "Next 3 tasks" or add here.)*

---

## Future Sandbox Milestone — Starter Gear Loop

*After current repo verification and after Milestone A/B behavior is confirmed
on device.* Build as modular tools (not one-off player scripts). Full design:
[`docs/09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md). Comfort rule: this milestone
moves objects/enemies, **not** the player's body.

- [ ] **Left Wrist Scan Pulse** — `Scannable` / `ScanPulseTool` / `WristScannerActivator`.
      Right hand touches left-wrist zone → pulse → highlight nearby `Scannable`s,
      cooldown, placeholder VFX/SFX.
- [ ] **Expanded Stun Dart** — `Stunnable` / `StunDartTool` / `StunDartProjectile` /
      `StunStatusEffect`. Slow readable glowing dart, non-lethal stun, expires,
      cooldown; stunned drones become gravity-grabbable.
- [ ] **Gravity Glove** — `GravityGrabbable` / `GravityGloveTool` / `GravityHoldPoint`
      (optional `GravityTargetFilter`). Objects first; enemies only when stunned;
      pull / hold / release / throw without flinging the player.
- [ ] **Sandbox loop arena** — scan → reveal → stun drone → gravity-grab → throw/place
      battery → door opens / reward / RILL comment.
- [ ] **Confirm on Quest.**

---

## Backlog

- **Long-term gear/tool idea bank** — [`docs/09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md).
  Categorized non-bullet explorer tech. Promote ideas into tasks deliberately;
  convert to `ToolRecipe` entries only once the registry system exists.
- **Tidefront galaxy strategy layer** — [`docs/10_TIDEFRONT.md`](10_TIDEFRONT.md).
  Metadata-first; local sandbox → AI → private async → ranked. Design-only until
  the content/registry architecture is stable.

---

*Keep in sync with docs/STATUS.md "Next 3 tasks".*
