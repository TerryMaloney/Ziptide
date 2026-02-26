# 02 — SETUP

**How to open and run the project.**

---

## Requirements

- **Unity version:** 2022.3.62f3  
- **Render pipeline:** URP  
- **Platform:** Meta Quest (XR)

---

## Steps (open in Unity)

1. Clone repo: `https://github.com/TerryMaloney/ziptide` (or Ziptide with capital Z per GitHub redirect).  
2. Open Unity Hub → Add project → select the folder that contains `Assets`, `Packages`, `ProjectSettings` (e.g. `C:\Ziptide\Ziptide`).  
3. Ensure Unity 2022.3.62f3 and Meta XR SDK / OpenXR configured for Quest.  
4. Build target: Android → Meta Quest.

---

## Run from Cursor

Build and install to Quest from Cursor terminal (PowerShell) without opening Unity.

**Get Unity project root:**

```powershell
$ps = Get-ChildItem C:\Ziptide -Directory -Recurse -Filter ProjectSettings | Select-Object -First 1
$projectRoot = Split-Path $ps.FullName -Parent
$projectRoot
```

Result is typically `C:\Ziptide\Ziptide`.

**Unity.exe path (2022.3.62f3):**

```powershell
$unityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe"
```

**Build only (batchmode):**

```powershell
& $unityExe -batchmode -nographics -quit `
  -projectPath "$projectRoot" `
  -executeMethod Ziptide.Build.BuildAndroid.APK `
  -logFile "$projectRoot\Builds\android_build.log"
```

**Install APK to connected Quest:**

```powershell
adb install -r "$projectRoot\Builds\Android\Ziptide.apk"
```

**One command — build + install:**

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
```

Optional: pass `-ProjectRoot` or `-UnityExe` if your paths differ.

---

## First-time manual (one-time)

- **Packages (from Cursor):** `Packages/manifest.json` already adds **Input System** and **XR Interaction Toolkit**. Open Unity once so it installs them; then in Package Manager, import **Starter Assets** from XR Interaction Toolkit (for grab/teleport input actions).
- **Create Milestone A scene from Cursor:** In Unity, run **Ziptide > Create Milestone A scene (Grab Cube)**. This creates `Assets/Ziptide/Scenes/MilestoneA_GrabCube.unity` with XR Origin, ground plane, and a grabbable cube, and adds it to Build Settings. If XR Origin prefab is not found (e.g. before importing Starter Assets), add it manually: **GameObject > XR > XR Origin (VR)**.
- **XR / Meta:** Enabling XR Plug-in Management (OpenXR or Meta XR), accepting package prompts, and Meta-specific toggles are done once in the Unity Editor.
- **Android build:** Ensure Build Settings include your scene(s) and platform is set to Android (switch once if needed).

---

## First-time / LLM orientation

- Read **docs/STATUS.md** for current milestone and next tasks.  
- Read **docs/MODULE_MAP.md** for folder/module roles.  
- Read **docs/00_LOCKED_CONTRACTS.md** for rules that must not be broken.

---

*Update this file when Unity version or SDK steps are finalized.*
