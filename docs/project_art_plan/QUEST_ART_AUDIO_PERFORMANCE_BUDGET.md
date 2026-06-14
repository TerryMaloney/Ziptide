# Quest Art & Audio Performance Budget

Standalone Quest (mobile GPU) constraints are **design rules**, not afterthoughts. These are
conservative starting defaults; tighten/loosen only after measuring on-device with the
OVR/XR performance overlay. The art audit enforces them (`PERF_BUDGET_WARN` / `PERF_BUDGET_FAIL`).

Last updated: 2026-06-14

---

## Per-world budget (starting defaults)

| Metric | Target | Hard cap |
|---|---|---|
| Frame rate | 72 FPS | never below 72 sustained |
| Draw calls (SetPass) | ≤ 120 | 200 |
| Triangles visible | ≤ 150k | 200k |
| Unique materials | ≤ 25 | 40 |
| Real-time lights | 1 directional | + 0–2 small, approved only |
| Transparent materials | ≤ 4 | 8 (prefer fake glass) |
| Reflection probes | 0–1 baked | 2 |
| Particle systems | ≤ 4 active | 8 |
| Audio voices (simultaneous) | ≤ 8 | 12 |
| Texture memory (loaded) | ≤ 256 MB | 384 MB |
| Scene object count | ≤ 600 | 1000 |
| Prefabs per kit | ≤ 40 | — |

## Techniques (use by default)

- **One main directional light** per world. No extra real-time lights unless approved.
- **Fake glass** (cheap reflective/emissive material) before true transparency.
- **Emissive trims** instead of many real lights for "glow."
- **Sky dome / backdrop mesh / fog tricks** for huge vistas — never render real distant geometry.
- **Repeated modular meshes + shared materials** (GPU instancing / atlasing).
- **LODs** on all imported art; **decals/material overlays** instead of unique geometry.
- **Bake lighting** where possible; avoid real-time shadows on mobile.

## "Large universe, small world" rule

Worlds feel vast through **vistas, occlusion, fog, and framing** — not raw geometry size.
A playable area is small and dense; scale is implied by backdrops and sky.

## Audit hooks

- material/light/transparency/object counts over cap → `PERF_BUDGET_FAIL` (blocks build)
- approaching cap (80%) → `PERF_BUDGET_WARN` (logged, build continues)
