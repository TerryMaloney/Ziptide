# OPERATING MODEL — one operator, three verification classes

**Why:** Ziptide is built by a **single AI operator** (you) plus **Terry** (the human with the Unity PC +
headset). You own everything code/data/docs; Terry is the hands for what you physically can't run. There
are **no lanes and no claim protocol** — you can't collide with yourself. What matters is **who can verify
a change**, because you can't run Unity or a headset. Every task is one of three classes.

> *(History: this used to be a two-chat split — "Architect" (data) vs "T-Dog" (scenes). Retired. The old
> two-column charter is gone; one operator spans both. Lane credits in the logs are historical only.)*

## The three verification classes
| Class | You do | Terry does | Verified by |
|---|---|---|---|
| **⚙ CI** | Write + commit all C# (gameplay, editor, patchers), world/job **data assets**, docs | nothing (optional review) | **CI**: compile + EditMode tests + `WorldAuditRunner` + Android APK build |
| **🔧 UNITY** | Write + commit the C# **patcher/builder**; queue the menu step in `TERRY_RUNBOOK.md` | Run `Ziptide → …` menu → commit the generated `.unity`/`.asset` | CI audits the committed scene after |
| **🎮 HEADSET** | Spec feel/geometry in code + data | Build to Quest, test, send ❌s + feel notes | Terry on-device (`DEVICE_TEST_CHECKLIST.md`) |

Most work is **⚙ CI** — do it freely, self-verify, move on. Batch **🔧/🎮** items so Terry can clear them
in one sitting.

## The patcher-indirection recipe (how a no-Unity operator authors scenes)
You never hand-edit `.unity`/`.prefab` YAML (it has corrupted scenes before). Instead:
1. **You** write/commit a C# editor script — a `ScenePatcher*` or a data-driven builder (e.g. `CityBuilder`
   consuming a `CityLayoutDefinition`).
2. **You** append the menu step to `TERRY_RUNBOOK.md`.
3. **Terry** runs the menu item in Unity; it generates the `.unity` scene + `WorldPackDefinition`/marker
   `.asset`s deterministically, and he commits them.
4. **CI** audits the committed scene (`WorldAuditRunner` blockers fail the build) + builds the APK.
5. **Terry** tests feel on the headset if it's 🎮.

Scenes are **generated from code + data, not authored by hand** — so a no-Unity operator can still build worlds.

## The editor menu items (🔧 — only Terry can run these)
**Generate committable assets** (`.unity` + `WorldPackDefinition`/markers):
- `Ziptide → Worlds → Build Toxic City` · `… → Build Toxic City Contract` · `… → Build PvP Arena`
- `Ziptide → Dev → Build Starter World (graybox)` · `… → Build Sandbox Test Lab`
- `Ziptide → Dev → Rebuild Dev World Manifest` — the in-VR Y+B world list; **the APK build auto-rebuilds it last**, so a manual run is optional (harmless)
**Apply to the currently-open scene:** `Ziptide → Apply Boot Scene Patcher` · `Apply D0/D1/D2 … To Current Scene` · `Apply Theme To Current Scene`
**Debug/CI:** `Ziptide → Audit → Run Audit (All Scenes)` · `Ziptide → Diagnostics → Dump Scene + Rig Config` · `Ziptide → Dev → Warp Window`

Source: `Assets/Ziptide/Editor/Patching/*`, `Editor/DevTools/*`, `Editor/Build/BuildAndroid.cs`
(`PatchScenesThenAPK`, the entry CI + `tools/dev_build_install.ps1` both call).

## Workflow
1. **One branch: `terry-local-wip`.** `git pull --rebase` before work; small commits; push often.
2. **Confirm CI green after every push.** If red → warn Terry loudly, stop shipping C# (`CLAUDE.md`).
3. **Queue Terry's steps in `TERRY_RUNBOOK.md`** the moment you create 🔧/🎮 work — never let your progress
   silently depend on an un-communicated menu run.
4. **Log each session in `HANDOFF.md`** (what you did / what's next) so continuity survives a context reset.

## Definition of Done
CI-green · 🔧/🎮 work **Terry-verified** · HANDOFF entry appended · MASTER_CHECKLIST updated if state
changed · no new TODO without a backlog line. **Don't claim done on unverified C#.**

## Guardrails (keep autonomous work from drifting)
- CI + the **world audit** (blockers fail the build) are the gates — keep them green.
- `MASTER_CHECKLIST.md` is the single source of "what's real" — update it, don't let it drift.
- Honor `00_LOCKED_CONTRACTS.md` (asmdef layering, `_Boot` ownership, travel via `TravelCoordinator`,
  data-driven content, no runtime reflection for item creation) and the locked story canon.
- Work phases **A→B→C→D→E** in `FABLE5_BACKLOG.md` roughly in order; don't run phases ahead blind.
