# GPT Additions — Claude Integration Prompt

Date: 2026-06-16
Label: Prompt for Claude / Opus / Architect / Tdog

## Copy/paste prompt

Claude, there are new GPT planning additions in the repo under:

```text
docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/
```

Please read these files:

```text
01_potential_weapons_and_tools.md
02_starter_gear_loop_next_three.md
03_tidefront_multiplayer_plan.md
```

Do not implement everything yet. First, integrate the ideas into the appropriate planning/checklist modules without breaking the existing structure.

## Integration tasks

1. Add a short pointer in `docs/STATUS.md` noting that GPT planning additions exist under `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/`.

2. Add the starter gear loop to the appropriate near-future task area:

```text
Starter Gear Loop
- Left Wrist Scan Pulse
- Expanded Stun Dart
- Gravity Glove
```

This should be listed as a future sandbox milestone after current repo verification and after Milestone A/B behavior is confirmed.

3. Add the broader weapon/tool brainstorm to the correct long-term planning location. Good candidate locations:

```text
docs/04_TASK_QUEUE.md
docs/06_SCHEMAS.md
or a new docs module such as docs/09_GEAR_AND_TOOLS.md
```

Do not turn every idea into a task. Preserve it as a categorized idea bank.

4. Add the Tidefront strategy/multiplayer plan to the correct long-term planning location. Good candidate locations:

```text
docs/04_TASK_QUEUE.md
docs/06_SCHEMAS.md
docs/01_ARCHITECTURE.md
or a new docs module such as docs/10_TIDEFRONT.md
```

Treat Tidefront as a future metadata-first strategy layer, not an immediate multiplayer implementation.

5. If creating new docs modules, update `docs/MODULE_MAP.md` so future LLM sessions know where those modules live.

6. Keep `docs/STATUS.md` factual. Do not claim any gear/Tidefront feature is implemented unless code exists and has been tested.

7. Preserve current Milestone A/B functionality. No scene-file edits unless explicitly required and owned by one model/session at a time.

## Suggested checklist placement

Near-term after current verification:

```text
Future Sandbox Milestone — Starter Gear Loop
- Implement Scannable / ScanPulseTool / WristScannerActivator.
- Implement Stunnable / StunDartTool / StunDartProjectile.
- Implement GravityGrabbable / GravityGloveTool.
- Build a tiny sandbox loop: scan -> stun -> gravity grab -> place/throw -> reward.
- Confirm on Quest.
```

Long-term planning:

```text
Future System — Gear and Tools
- Maintain idea bank for non-bullet explorer tech.
- Categorize tools by combat, traversal, puzzle, utility, and companion support.
- Convert only selected ideas into ToolRecipe entries when the registry system exists.

Future System — Tidefront
- Metadata-first planet control layer.
- Local/offline prototype first.
- AI opponents second.
- Private async galaxies third.
- Ranked leagues only after anti-ganging rules and server persistence are ready.
```

## Important constraint

Do not let these idea files cause scope explosion. The intended order is:

1. Verify current repo state.
2. Update stale status/checklists.
3. Lock down modular content structure.
4. Only then prototype the Starter Gear Loop.
5. Tidefront stays design-only until the content/registry architecture is stable.
