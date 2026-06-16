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

*Later planning — after the D-series city work and once we want the next fun
sandbox loop.* Build as modular tools (extend `ToolDefinition`, not one-off
player scripts). Full design: [`docs/09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md).
Comfort rule: this milestone moves objects/enemies, **not** the player's body.
Note: stun-on-drone already exists (Taser + `IShockable`), so the genuinely new
pieces are Scan Pulse and Gravity Glove.

- [ ] **Left Wrist Scan Pulse** — `Scannable` / `ScanPulseTool` / `WristScannerActivator`.
      Right hand touches left-wrist zone → pulse → highlight nearby `Scannable`s, cooldown, VFX/SFX.
- [ ] **Expanded Stun Dart** — generalize the existing Taser path beyond drones
      (`Stunnable` / `StunStatusEffect`); add variants later (chain/anchor/pulse/tag/overcharge).
- [ ] **Gravity Glove** — `GravityGrabbable` / `GravityGloveTool` / `GravityHoldPoint`
      (optional `GravityTargetFilter`). Objects first; enemies only when stunned; no flinging the player.
- [ ] **Sandbox loop arena** — scan → reveal → stun drone → gravity-grab → throw/place battery → reward.
- [ ] **Confirm on Quest.**

---

## Backlog

- **Long-term gear/tool idea bank** — [`docs/09_GEAR_AND_TOOLS.md`](09_GEAR_AND_TOOLS.md).
  Categorized non-bullet explorer tech. Promote ideas to tasks deliberately;
  convert to `ToolDefinition`/`ToolRecipe` entries only once the registry shape is ready.
- **Tidefront galaxy strategy layer** — [`docs/10_TIDEFRONT.md`](10_TIDEFRONT.md).
  Metadata-first; local sandbox → AI → private async → ranked. Design-only until
  the content/registry architecture is stable.

---

*Keep in sync with docs/STATUS.md "Next 3 tasks".*
