# 03 — SHIP ART DIRECTION (architecture-side notes)

*For Gemini + Terry, written by T-Dog. This is **not** the creative brief
(that's Terry + Gemini's call) — it's the set of **technical/architectural
constraints** so the ship art drops into ZIPTIDE without breaking the modularity
laws or the Quest performance reality.*

---

## Where the ship lives in the system

- There's a dedicated assembly **`Ziptide.Ship`** (depends on `Ziptide.Core`
  only). It currently has **no code** — it's a reserved seam.
- The architecture treats the ship + **DriftCorridor** as a **swappable system**
  behind an `IDriftCorridor` interface (not yet written), and explicitly allows
  **"ship-layout variants within allowed zones."**
- So conceptually there are **two independent axes**, and art needs to respect
  both:
  1. **Ship layout / geometry** — the structure the player moves through.
  2. **Ship skin / theme** — materials, VFX, lighting, audio — resolved via the
     **theme registry**, never by prefab name in ship logic.

> The golden rule (from `00_LOCKED_CONTRACTS`): *systems request visuals through
> a theme registry, never by prefab name in code.* Ship art is a **theme-bound
> asset**, addressed by id/handle, swappable without touching ship logic.

## What "good delivery" looks like (so it integrates cleanly)

- **Modular pieces, not one mega-mesh.** Hull / cockpit / corridor segments as
  separate, snappable modules so layout variants are possible.
- **Consistent pivot + scale + real-world meters.** VR is 1:1 — a doorway must
  be a real ~2 m, a console reachable. Pivots at logical mount points.
- **Clean origins / forward axis (+Z forward, +Y up)** so prefabs assemble
  predictably.
- **Material discipline for Quest:** few unique materials, shared atlases, URP
  Lit/Unlit, **no heavy transparency stacks**, bake lighting where possible.
  (Existing sky/planet rig is deliberately unlit + no shadows for this reason.)
- **VFX as swappable prefabs** (the "Bloom spores / crease lines / gate fold"
  language), referenced through the theme, not hardwired.
- **LODs** for anything large or distant.

## Budgets — ⚠️ provisional (no official perf budget set yet, see Q3)

These are **placeholder Quest-sane numbers** so art isn't blocked; Terry + the
build Claudes will replace them once the device + frame-rate target is locked.

| Thing | Provisional target (Quest) |
|-------|----------------------------|
| Frame rate | 72 Hz (assume; confirm) |
| Triangles, ship visible at once | keep total scene well under ~350–500k |
| Unique materials per ship | single digits; atlas aggressively |
| Texture size | 1k–2k atlases; avoid 4k |
| Real-time lights | minimal; prefer baked / unlit |
| Transparency / overdraw | minimize — biggest Quest killer |

**Do not treat these as final.** They exist only so work can start.

## Architectural asks for the art pipeline

1. **Deliver ship assets as theme-bound, id-addressable assets** (so a future
   `IThemeProvider` / registry can resolve "ship.hull", "ship.corridor", etc.).
2. **Keep layout separable from skin** — one grey-box layout should accept
   multiple skins.
3. **Name + organize predictably** under a content path (suggest
   `Assets/Ziptide/Content/Ship/...`) so tooling can find it.
4. **Flag anything that needs custom shaders** early — custom shaders on Quest
   have a perf cost and need a budget.

## Open art-side questions (route to Terry/Gemini)

- Is the ship **interior-traversable** (player walks the corridors) or mostly an
  exterior/cockpit experience? Changes the modular breakdown a lot.
- What is **DriftCorridor** experientially — a tunnel/transition the ship moves
  through? That shapes both layout and signature VFX.
- Art style lock: how literal vs. stylized? (Stylized + flat-lit is the Quest-
  friendly path.)

---

*When the perf budget (Q3) is answered, I'll replace the provisional table and
notify Gemini via the coordination log.*
