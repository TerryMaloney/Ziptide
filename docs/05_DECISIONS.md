# 05 — DECISIONS

**Why we chose X. Change only with explicit rationale.**

---

## Engine & platform

- **Unity** — *(Reason TBD: e.g. Meta XR support, team familiarity, URP.)*  
- **URP** — *(Reason TBD: mobile/Quest performance, single pipeline.)*  
- **Meta Quest / XR** — *(Reason TBD: target platform.)*

---

## Architecture

- **Data-driven content packs** — So story/art can be swapped without code changes (see 00_LOCKED_CONTRACTS).  
- **Assembly Definitions** — To enforce module boundaries and prevent Core from depending on Visuals.  
- **Theme registry** — So systems never reference prefabs by name; visuals are swappable.

---

*Record significant decisions here with date and short rationale.*
