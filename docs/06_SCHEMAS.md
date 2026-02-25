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

## Registry / single source of "what exists"

- One registry (or manifest) so an LLM can orient instantly.  
- Document in MODULE_MAP.md and STATUS.md.

---

*Add schema docs and field lists as they stabilize. Prefer generated ScriptableObjects from schema.*
