# 00 — LOCKED CONTRACTS

**The laws. Systems must not violate these.**

---

## Modularity rule (canon)

Everything must be **swappable without cascading breakage**:

- Story / lore / names  
- Visual theme / materials / VFX  
- Characters / voice / RILL lines  
- Pods / sectors / traversal rules  
- Ship layout variants (within allowed zones)  
- Any "signature layer" (Bloom / stewardship / origami folding language)

**The game engine (systems) must not "know" story/art specifics. It only knows schemas + contracts.**

---

## Contract-first, interface-first

Every system that can change gets an **interface + contract doc**:

- `IPodLoader`
- `IThemeProvider`
- `IBloomStewardship`
- `IDriftCorridor`
- *(etc.)*

Hard acceptance tests per module (even if simple "smoke" tests).

---

## Data rule

- Code reads **WorldProfile** + **PodNarrative** only.
- No direct references to story/place names (e.g. "Basalt Coast") in engine code.

---

## Visuals rule

- Systems request visuals through a **theme registry**, never by prefab name in code.

---

## Assembly dependency rule

- **Visuals** can depend on **Core**.
- **Core** never depends on **Visuals**.

---

*Add new contracts here as they lock. Change only by explicit decision (see DECISIONS.md).*
