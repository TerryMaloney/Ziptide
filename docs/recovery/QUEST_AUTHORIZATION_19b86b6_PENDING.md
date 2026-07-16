# ZIPTIDE QUEST AUTHORIZATION — `19b86b6` PENDING CLEAN PROOF

**Authorization:** HOLD until the clean package row and artifact hashes below are completed and independently inspected.

| Field | Value |
|---|---|
| Source SHA | `19b86b624a132c6efc57f8daf703cf62cfd266a3` |
| Normal PlayMode | Run `29532312905` — 43/43, 0 failed, 0 skipped, 0 inconclusive |
| Ordinary CI | Run `29532312888` — EditMode success, patch/world audit success |
| Contract/static scan | Last scan of unchanged scanned runtime inputs: all 9 tool tests and 9 report generators pass |
| Normal Golden Android | Run `29532312981` — success |
| Build profile | `GoldenSlice` |
| Compile define | `ZIPTIDE_RECOVERY_GOLDEN` |
| Locked scenes | `_Boot`, `W000_DriftIn`, `ToxicCity` |
| Normal Golden artifact | `recovery-golden-apk-19b86b624a132c6efc57f8daf703cf62cfd266a3` |
| Normal Golden APK SHA-256 | `afa0eb3e7c0975b3e68d2c9d2db78ee55b8f6a76a2e29c676fc31aea04f1e968` |
| Normal build-profile SHA-256 | `888633395086c90bd694bd0f8306b41ad0f6f6edbcecc5c4ad9abdeffcb9212c` |
| Clean package proof | `PENDING` |
| Authorized APK artifact | `PENDING — prefer inspected clean Golden artifact` |
| Authorized APK SHA-256 | `PENDING` |
| Authorized build-profile SHA-256 | `PENDING` |
| Final authorization | `PENDING` |

## Artifact review already complete

- Normal PlayMode NUnit evidence is exactly 43/43.
- No travel/input/XRI failure marker appears in the canonical route log.
- W000 and ToxicCity player-camera snapshots are rendered, nonblank frames.
- Home Hub fallback audit: 55 renderers / 55 material slots / 0 findings.
- W000 fallback audit: 196 renderers / 202 material slots / 0 findings.
- ToxicCity fallback audit: 546 renderers / 549 material slots / 0 findings.
- R1.10 returned-W000 renderer, texture, mesh, and source counts remain stable enough for the first Linux reference baseline.
- Normal Golden build profile proves the correct profile, define, three-scene lock, successful build, and nonempty APK.
- Golden audit reports zero blockers.

## Required manual Quest observations

- Physical post-travel movement, snap turn, and smooth turn.
- Tracking, hands, rays, and duplicate-rig symptoms.
- Travel-shell clearance and standing height.
- W000 Objective Board readability. The current automated audit cannot fully measure its world-space `TextMeshProUGUI` CanvasRenderer geometry.
- ToxicCity RILL/dialogue readability from the actual headset viewpoint.
- Device frame pacing and comfort.
- Save/relaunch/CONTINUE on hardware.

## Immutable artifact rule

Do not invoke `tools/quest_smoke.ps1` during the authorized exact-artifact checkpoint. It performs a new local build and install. Follow `QUEST_GOLDEN_CHECKPOINT_V2.md` and install the downloaded authorized APK directly.
