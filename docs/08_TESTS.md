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

- *(Unity Test Runner / asmdef test assemblies — TBD.)*

---

*Add test list and commands as tests are added.*
