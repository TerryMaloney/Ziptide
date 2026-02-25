# 01 — ARCHITECTURE

**How we enforce modularity (practical architecture).**

---

## 1) Data-driven content packs (no hardcoded story)

- All pod content lives as **data** (JSON + ScriptableObjects generated from schema).
- Story text is a **pack** that can be removed/replaced without affecting traversal/building.
- **Rule:** Code reads `WorldProfile` + `PodNarrative` only. No direct references to "Basalt Coast", etc.

---

## 2) "Skins" for visuals (swap art without breaking logic)

- **VisualTheme** asset (ScriptableObject) points to:
  - Materials/shaders  
  - VFX prefabs (Bloom spores, crease lines, gate fold)  
  - Lighting presets  
  - Audio presets  
- **Rule:** Systems request visuals through a **theme registry**, never by prefab name in code.

---

## 3) Strict module boundaries in Unity

Use **Assembly Definitions (asmdef)** so modules stay clean:

| Assembly | Role |
|----------|------|
| Ziptide.Core | Contracts, schemas, events, utilities |
| Ziptide.Gameplay | Build/traverse logic |
| Ziptide.Content | Generated assets, pod packs |
| Ziptide.Visuals | Bloom, origami VFX, theme binding |
| Ziptide.Ship | Ship scene logic including DriftCorridor |
| Ziptide.Platform.Quest | XR/Meta-specific glue |

**Rule:** Visuals can depend on Core; Core never depends on Visuals.

---

## 4) Contract-first, interface-first

- Every swappable system: **interface** + **contract doc**.
- Examples: `IPodLoader`, `IThemeProvider`, `IBloomStewardship`, `IDriftCorridor`.
- Hard acceptance tests per module (even if simple smoke tests).

---

## 5) One source of truth for "what exists"

- **docs/STATUS.md** — current milestone, what works, what's next.  
- **docs/DECISIONS.md** — why Unity version, URP, Meta XR, etc.  
- **docs/00_LOCKED_CONTRACTS.md** — the laws.  
- **docs/MODULE_MAP.md** — what each folder/module is responsible for.

---

*See 00_LOCKED_CONTRACTS.md for immutable rules.*
