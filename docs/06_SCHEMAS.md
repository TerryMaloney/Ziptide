# 06 — SCHEMAS

**Data shapes the engine respects. No hardcoded story/art.**

---

## WorldProfile

- *(Schema for world-level config: what pods exist, traversal rules, etc.)*  
- Code reads this; no direct "Basalt Coast"–style references.

---

## PodNarrative

- *(Schema for per-pod story/lore/text.)*  
- Pack can be removed/replaced without breaking traversal/building.

---

## VisualTheme (ScriptableObject)

References for:

- Materials/shaders  
- VFX prefabs (Bloom spores, crease lines, gate fold)  
- Lighting presets  
- Audio presets  

Systems resolve via **theme registry**, not by asset name in code.

---

## VisualThemeProfile (World dressing — A.5)

ScriptableObject in Ziptide.Visuals; used by WorldDirector for sky + planet + ground.

- **groundTint:** Color applied to ground plane.
- **skyGradient:** Gradient for sky sphere (vertical).
- **planet:** PlanetSettings — baseColor, accentColor, angularSizeDegrees, distance, direction (normalized), rotationSpeed, followPlayer.

Assets under `Assets/Ziptide/Content/VisualThemes/` (e.g. DefaultVisualTheme.asset).

---

## Registry / single source of "what exists"

- One registry (or manifest) so an LLM can orient instantly.  
- Document in MODULE_MAP.md and STATUS.md.

---

*Add schema docs and field lists as they stabilize. Prefer generated ScriptableObjects from schema.*
