# AGENTS.md

## Cursor Cloud specific instructions

### Project Overview

Ziptide is a Unity VR game project targeting Meta Quest (Quest 2/3/Pro). It uses Unity 2022.3.62f3 LTS with URP 14.0.12. The project is in early Phase A (scaffolding) — see `docs/STATUS.md` for current state.

For architecture, setup, and module layout, see `docs/01_ARCHITECTURE.md`, `docs/02_SETUP.md`, and `docs/MODULE_MAP.md`.

### Installed Tooling

- **Unity Editor:** `/home/ubuntu/Unity/Hub/Editor/2022.3.62f3/Editor/Unity` (installed via Unity Hub CLI)
- **Unity Hub:** `/usr/bin/unityhub` (for managing editors, licenses, and modules)
- **.NET SDK 8.0:** For C# compilation checking and `dotnet format` linting outside Unity
- **dotnet-format:** Global tool at `/home/ubuntu/.dotnet/tools/dotnet-format`

### Unity License Requirement

Unity batch-mode operations (project open, compile, test, build) require a valid Unity license. Personal licenses must be activated via Unity Hub GUI — they cannot be activated headlessly via CLI. To activate:

1. Launch Unity Hub: `unityhub &`
2. Sign in with a Unity account in the GUI (Desktop pane)
3. Activate a Personal (free) or Pro license
4. License file will be stored at `~/.local/share/unity3d/Unity/Unity_lic.ulf`

For Pro/Plus licenses, CLI activation is possible:
```
/home/ubuntu/Unity/Hub/Editor/2022.3.62f3/Editor/Unity -quit -batchmode -serial <SERIAL> -username <EMAIL> -password <PASSWORD>
```

### C# Compilation Check (without Unity license)

A `.build-check/` directory contains a .NET project that references Unity managed assemblies for offline C# compilation:

```bash
cd /workspace/.build-check/UnityCompileCheck && dotnet build
```

This validates C# syntax and type correctness against Unity APIs without needing a Unity license. It auto-discovers all `.cs` files under `Ziptide/Assets/`.

### Lint / Format

```bash
cd /workspace/.build-check/UnityCompileCheck && dotnet format --verify-no-changes
```

### Unity Batch Mode Commands (require license)

```bash
UNITY=/home/ubuntu/Unity/Hub/Editor/2022.3.62f3/Editor/Unity

# Open project and compile scripts
$UNITY -batchmode -nographics -quit -projectPath /workspace/Ziptide -logFile -

# Run EditMode tests
$UNITY -batchmode -nographics -projectPath /workspace/Ziptide -runTests -testPlatform EditMode -logFile -

# Run PlayMode tests
$UNITY -batchmode -nographics -projectPath /workspace/Ziptide -runTests -testPlatform PlayMode -logFile -
```

### Key Gotchas

- The `.build-check/` project uses `netstandard2.1` (matching Unity's scripting backend) and references DLLs from the installed Unity Editor. If the Unity version changes, update the paths in `.build-check/UnityCompileCheck/UnityCompileCheck.csproj`.
- Unity Hub shows D-Bus errors in logs on this VM — these are cosmetic and can be ignored.
- The `dotnet-format` tool needs `export PATH="$PATH:/home/ubuntu/.dotnet/tools"` to be on PATH.
- No tests are written yet (Phase A scaffolding). The test framework (`com.unity.test-framework 1.1.33`) is already in `Packages/manifest.json`.
