# Automation Audit — killing the manual steps

**Goal: anything Terry has to *remember* to do by hand is a future broken build.** This is the
inventory of every manual step in the current pipeline + how to remove or shrink it. Ordered by
payoff. Grounded in the real scripts (`tools/`), editor menus, and `BuildAndroid.cs` / `ci.yml`.

> Context: yesterday "nothing got fixed" because the Sandbox scene wasn't in Build Settings — a
> classic *manual-step-that-was-forgotten* failure. That class of bug is what this doc exists to kill.

---

## The manual steps that exist today

| # | Manual step | Where it bites | Cost when forgotten |
|---|-------------|----------------|---------------------|
| 1 | **Add a scene to Build Settings** | new scenes (Sandbox) | content never ships / can't warp at runtime ← *this was yesterday* |
| 2 | **Run editor menu patchers** ("Apply D0/D1/D2", "Build Sandbox Test Lab", "Rebuild Dev World Manifest") | after content changes | stale/missing content in build |
| 3 | **Open Unity once to import hand-written `.meta`** (cloud agents write code blind) | every cloud-authored file | GUID churn, broken refs |
| 4 | **Close the Unity Editor before `dev_build_install`** | every local build | build fails on file lock |
| 5 | **Build + sideload from the PC** (`dev_build_install.ps1`) | every device test | requires a working Unity PC every time |
| 6 | **Verify on device by hand** | every build | unavoidable for VR feel — but a lot can be caught earlier |
| 7 | **Regenerate the Unity `.ulf` license** when it expires | CI | red CI, "flying blind" (CLAUDE.md) |
| 8 | **Run `gpt_sync.ps1`** to produce the GPT update | sharing with GPT | minor |

---

## Roadmap (by payoff)

### ✅ P0 — DONE this session
- **Sandbox auto-enters Build Settings during the build.** `ScenePatcherSandbox.EnsureInBuildSettings()`
  now runs inside `BuildAndroid.PatchScenesThenAPK` (before the scene loop), so the sandbox is always
  enabled, populated (gear + drones), and shipped — **no manual Build Settings step.** Kills step #1
  for the sandbox. *(Verify on Terry's next build: the build log should show "Sandbox added to Build
  Settings" and the gun/drones should be in the sandbox on-device.)*
- **Cloud CI now builds the APK** (was main-only + skipped our patchers). `ci.yml`'s `build-android`
  job now runs `buildMethod: BuildAndroid.PatchScenesThenAPK` (patch + audit + APK, same as the PC)
  with `allowDirtyBuild: true`, and uploads the APK as the **`ziptide-apk`** artifact. Trigger it from
  Actions > CI > **Run workflow** (any branch) or via the API. **Terry can download the APK and
  `adb install -r` it — no Unity PC required** — and build breaks (audit blockers, missing scenes,
  patcher crashes) now fail in the cloud *before* the headset. Removes manual steps #4 and #5.
  - **Agents can self-serve a device build:** after pushing a device-test-worthy change to
    `terry-local-wip`, trigger the workflow (`mcp__github__actions_run_trigger`, workflow `ci.yml`,
    ref `terry-local-wip`) and hand Terry the artifact link. So even kicking the build is hands-off.

### 🔴 P1 — biggest wins, do next

**A. ✅ DONE — Build the APK in the cloud (CI), not (only) on the PC.** *(removes #4, #5; shrinks #6)*
See P0 above — `ci.yml` now runs the real patched build + audit and uploads the `ziptide-apk` artifact.

**B. Make the world audit non-fatal for non-shipping scenes / self-healing.** *(removes a silent
build-abort class)*
`PatchScenesThenAPK` throws on ANY blocker, and `MilestoneA_GrabCube` currently has 2 (XRI manager +
missing spawn) — so a scene Terry isn't even testing can abort the whole build → nothing installs.
Fix options (pick one): (a) have the D2/Boot patchers actually clear MilestoneA's XRI-manager + spawn
so the audit passes; (b) scope the build-abort to scenes that are actually shipping/changed; (c)
downgrade stale-scene issues to warnings. Investigate *why* the patchers don't already strip MilestoneA.

**C. Rebuild the Dev World Manifest during the build.** *(removes part of #2)*
`DevWorldManifestBuilder` is menu-only today, so the in-VR Dev Menu can list stale worlds. Call it from
`BuildAndroid.PatchScenesThenAPK` (like the sandbox populate) so the manifest is always current.

### 🟡 P2 — hygiene + safety nets

**D. `.meta` guard for cloud-authored files.** *(shrinks #3)*
Add a CI check (or pre-commit hook) that fails/flags any new `.cs`/asset without a sibling `.meta`, and
document deterministic GUID generation so cloud agents emit stable `.meta` themselves. Stops GUID churn
and "open Unity once" surprises.

**E. Headless scene-load smoke in CI.** *(shrinks #6)*
A batchmode pass that loads `_Boot` → each world scene and asserts no exceptions / required singletons
present (extends `WorldAuditRunner`). Catches scene-load NREs (the ObjectiveBoard-class bugs) in the
cloud before device.

**F. Auto-run the audit in CI** *(catch blockers without a full build)*
Run `WorldAuditRunner.RunAll()` in an EditMode test or a tiny batchmode step on every push so audit
blockers surface immediately, decoupled from the slow Android build.

**G. License-expiry early warning.** *(softens #7)*
A scheduled CI job (or a check in the test job) that fails loudly with the `RECOVERY_STEPS.md` pointer
the moment activation breaks, instead of discovering it mid-session.

---

## Principle to bake in

**Generation belongs in the build, not in a human's memory.** Every "remember to run X / add Y / open
Z" should become an idempotent step inside `BuildAndroid.PatchScenesThenAPK` (or CI). The patchers
already prove the pattern (C0/D0/D1/D2 run automatically, guarded by scene name). New content =
a patcher/ensure call wired into the build, never a checklist item.

## Links
`BuildAndroid.cs`, `tools/dev_build_install.ps1`, `.github/workflows/ci.yml`, `docs/CI_VERIFY.md`,
`docs/RECOVERY_STEPS.md`, `docs/AI_WORKFLOW.md`.
