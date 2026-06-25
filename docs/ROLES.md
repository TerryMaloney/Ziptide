# ROLES — the two lanes (collision-free parallel work)

**Why:** two AI chats (Architect + T-Dog) build Ziptide in parallel. They must **not touch the same files**,
so when one is rate-limited the other keeps shipping. This is the authoritative lane charter; the
working-agreement in `HANDOFF.md` summarizes it.

## The split: DATA (Architect) vs SCENE/RUNTIME (T-Dog)
| | **Architect** | **T-Dog** |
|---|---|---|
| **Owns** | backend C# · data model · economy · registries · **tests** · netcode **message model** · world **DATA** (story→`WorldPackDefinition`, `CityLayoutDefinition` authoring, flow templates, `CreatureDefinition` stats/loot/archetype) · pure logic | **scenes** · XR rig · editor · **patchers** (`ScenePatcher*`, `CityBuilder`) · on-device fixes · **city geometry** · creature **runtime behavior** (MonoBehaviours/FSMs) · weapon/tool feel · the build pipeline |
| **Verifies via** | **CI** (compile + EditMode tests) — no headset | **the headset** (Terry on-device) + CI |
| **Typical files** | `Core/**`, `Content/Runtime/**` (Definitions/Jobs/City data), `Multiplayer/Runtime/{rules,Net}`, `Tests/**`, `docs/**` | `Gameplay/Runtime/**` (rig/enemies/weapons/pvp scene), `Editor/**` (patchers/build/audit), `Assets/**/Scenes/**`, `Resources/**` |

**Grey zones (call it out, don't guess):** a `*Definition` data field = Architect; the MonoBehaviour that
consumes it = T-Dog. A new `DistrictDef`/`CreatureZoneDef` field = Architect; `CityBuilder`/spawn runtime
reading it = T-Dog. When a task needs both, split it into an `[A]` data sub-task and a `[T]` runtime
sub-task (see `FABLE5_BACKLOG.md`).

## Collision-avoidance protocol (proven this project)
1. **One branch: `terry-local-wip`.** `git pull --rebase origin terry-local-wip` before any work.
2. **Claim before you build:** add a `Next-CLAIMED` line in `HANDOFF.md` naming the task + files. Read the
   *other* lane's latest claim first; if it overlaps, pick a different task.
3. **Small commits, push often, rebase on reject.** Never force-push shared history.
4. **Shared files** (`ZiptideConstants.cs`, `WorldPackDefinition.cs`, build settings, `STATUS.md`,
   `MASTER_CHECKLIST.md`) = additive edits + flag in HANDOFF. Prefer adding a new file over editing a shared one.
5. **Independence rule:** never block on the other lane. If your next task needs their output, pick another
   task from your lane's queue. The backlog is built so each lane always has pullable work.

## Definition of Done (both lanes)
CI-green · runtime/scene work **device-verified by Terry** · HANDOFF entry appended · MASTER_CHECKLIST
updated if state changed · no new TODO without a backlog line. **Don't claim done on unverified C#.**

## Guardrails (keep an autonomous lane from drifting)
- CI + the **world audit** (blockers fail the build) are the gates — keep them green; if red, warn Terry
  loudly and stop shipping C# (per `CLAUDE.md`).
- `MASTER_CHECKLIST.md` is the single source of "what's real" — update it, don't let it drift.
- Honor `00_LOCKED_CONTRACTS.md` (asmdef layering, `_Boot` ownership, travel via `TravelCoordinator`,
  data-driven, no runtime reflection for item creation) and the locked story canon.
- Review at **phase boundaries** (A→B→C→D→E in `FABLE5_BACKLOG.md`); don't run multiple phases ahead blind.
