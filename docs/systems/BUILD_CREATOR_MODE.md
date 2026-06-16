# System — Build / Creator Mode

**Status:** designed; later milestone. See `docs/design/SYSTEMS_ARCHITECTURE.md` §Layer 4.

## 1. Purpose
A "same game, free access" mode: travel to ANY world with no prerequisites + edit tools, for
building/testing/creating. Normal play is unaffected.

## 2. How it works (intended)
A single **`ModeService`** switch that (a) unlocks travel to any world (no gating), (b) enables a
**free-fly + place/edit** toolset, (c) bypasses progression gates. Generated worlds are
**editable data** (the procedural generator writes a layout asset you can open and tweak), so creator
mode hand-polishes generated worlds.

## 3. Data + code
- `ModeService` (normal vs creator); gating checks consult it.
- Reuses the **Dev Warp** tooling (`DevWarp` / Warp Window) for jumping; creator mode is the
  player-facing cousin of the developer warp.
- World generator writes `WorldPackDefinition` + layout data (generate-to-editable-data).

## 4. VR feel
Free-fly + grab-to-place + snap; an in-VR palette of parts. Comfortable flight (reuse ship cockpit
comfort ideas where relevant).

## 5. Open questions
Is creator mode shipped to all players, or a separate/unlocked mode? How much can players save/share?

## Links
Dev Warp (`Editor/DevTools/DevWarpWindow.cs`, `Gameplay/Runtime/DevTools/DevWarp.cs`),
`docs/design/SYSTEMS_ARCHITECTURE.md` §Layer 4–5.
