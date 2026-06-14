# 08 — TESTS

**Contract and smoke tests per module.**

---

## Rule

- Hard acceptance tests per module (even if simple smoke tests).  
- Interfaces (IPodLoader, IThemeProvider, IBloomStewardship, IDriftCorridor, etc.) have tests that verify contract.

---

## Modules to cover

- **Ziptide.Core** — Contract types, events, utilities.  
- **Ziptide.Gameplay** — Build/traverse, pod loading.  
- **Ziptide.Content** — Data loading, schema validation.  
- **Ziptide.Visuals** — Theme resolution, no direct prefab names in logic.  
- **Ziptide.Ship** — DriftCorridor, ship layout.  
- **Ziptide.Platform.Quest** — XR glue (device tests where possible).

---

## Location

- **EditMode tests:** `Assets/Ziptide/Tests/` (assembly `Ziptide.Tests`, Editor-only).
  - `EditMode/ZiptideContractTests.cs` covers: Core↛Visuals dependency law
    (`DependencyValidator.Validate`), `WorldProfile`/`VisualThemeProfile` default sanity,
    and `PlayAreaBounds.Build` (creates four solid walls, idempotent on rebuild).

## How to run

- **In Unity:** Window > General > Test Runner > EditMode > Run All.
- **Headless / CI:** GitHub Actions (`.github/workflows/ci.yml`) runs the EditMode suite via
  GameCI `unity-test-runner` on every push to `main` and `claude/**` and on PRs to `main`.
  Requires repo secrets `UNITY_LICENSE` (or `UNITY_SERIAL`), `UNITY_EMAIL`, `UNITY_PASSWORD`.

---

*Add test list and commands as tests are added.*
