# STATUS — LLM dashboard

**Single source of truth for "what exists". Check this first every session.**

---

## Current Milestone

- **Phase:** A — **COMPLETE** (Grab Cube on Quest).
- **Phase A.5:** World Dressing (Sky + Planet + Theme) — implemented; verify on device (see [MILESTONE_A5.md](MILESTONE_A5.md)).
- **Phase B:** Play-Area Safety + WorldProfile + Theme Switch Station — **code implemented but NOT yet wired into the committed scene** (`MilestoneA_GrabCube.unity` has no `WorldRuntime`). Run **Ziptide > Apply World Profile To Current Scene** and save the scene to activate it. See [MILESTONE_B.md](MILESTONE_B.md).

---

## ✅ Working on device

- **Milestone A:** Scene runs on Quest; environment visible (ground, horizon, sky); cube visible and grabbable; locomotion works (teleport/move); user can pick up, drop, and throw the cube.
- **Milestone A.5 (in progress):** Data-driven world dressing: VisualThemeProfile, SkyPlanetRig (sky + planet), WorldDirector. Menus: **Ziptide > Create Default Visual Theme (Sky + Planet)**; **Ziptide > Apply Theme To Current Scene**.
- Asmdef skeleton in place (Core, Gameplay, Content, Visuals, Ship, Platform.Quest, Editor).
- **Milestone B (in progress):** WorldProfile, WorldRuntime, PlayAreaBounds, FallRespawner, ThemeSwitchStation. Menus: **Ziptide > Create Default World Profile**; **Ziptide > Apply World Profile To Current Scene**.
- **Ziptide menu:** **Ziptide > Apply Quest player defaults**; **Ziptide > Create Milestone A scene (Grab Cube)**; **Ziptide > Create Default Visual Theme (Sky + Planet)**; **Ziptide > Apply Theme To Current Scene**; **Ziptide > Create Default World Profile**; **Ziptide > Apply World Profile To Current Scene**; **Ziptide > Diagnostics > XR Grab Readiness**.
- **Build pipeline:** `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1` (with optional `-Logcat`).
- **GPT sync:** `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\gpt_sync.ps1` → outputs `GPT_PROJECT_UPDATE.md`.

---

## 🔥 Blocking issues / unverified

- **Latest fixes NOT yet verified on device.** Milestone A.5 + B and the PR #1/#2
  bug fixes have **not** been uploaded to the Quest yet (the build/install didn't
  complete in the last session). Treat A.5/B as "implemented, device-unverified"
  until a successful `dev_build_install.ps1` run + on-device check. Only the
  original Milestone A (grab cube + locomotion) is confirmed working on hardware.

---

## ✅ Tests & CI

- **EditMode tests:** `Assets/Ziptide/Tests/` (assembly `Ziptide.Tests`). Run via Unity Test Runner or CI. See [08_TESTS.md](08_TESTS.md).
- **CI:** GitHub Actions `.github/workflows/ci.yml` (GameCI) compiles the project and runs EditMode tests on push/PR. Requires repo secrets `UNITY_LICENSE`/`UNITY_EMAIL`/`UNITY_PASSWORD`.

---

## Next 3 tasks (exact)

1. **Upload to Quest + verify A.5/B on device** (the upload didn't complete last
   session): run `dev_build_install.ps1`, then walk the MILESTONE_A5/MILESTONE_B
   checklists (sky/planet, bounds, fall-respawn, theme switch, grab cube).
2. **Walking skeleton (device-independent, in progress):** data-driven pod seam
   landed — `IPodLoader` + `PodNarrative` (Core), `PodRegistry` (Content), tests.
   Next seam: `IThemeProvider` so the existing `WorldDirector` sits behind an
   interface (see docs/architect/01_OPEN_QUESTIONS B1).
3. **Open decisions for Terry:** perf/device target + story canon (docs/architect/01_OPEN_QUESTIONS Q2–Q3).

---

## Repo links + build target settings

- **Repo:** https://github.com/TerryMaloney/Ziptide.git (public)
- **Build target:** Meta Quest (Unity), Android, ARM64, IL2CPP, API 29
- **Unity version:** 2022.3.62f3
- **URP:** Yes (Balanced for Android)

---

*This file prevents drift by forcing "centerline" every session.*
