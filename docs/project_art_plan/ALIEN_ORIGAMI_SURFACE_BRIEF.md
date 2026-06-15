# Alien Origami / Pattern — Surface Family Brief

The **Alien Origami / Pattern** surface family is the visual language for ZIPTIDE's most
advanced alien-built worlds. Where Toxic Venice is lived-in and repaired, Pattern worlds are
*pristine and mathematical* — structures folded from flat geometry like giant origami, covered
in glyphs that look like they mean something. Decisions: 2026-06-15.

Belongs to: `docs/project_art_plan/ART_DIRECTION_MASTER_PLAN.md` surface family table.
Build target: the `SandboxTestLab` scene is where we prototype these shapes before shipping
to a real world. See `docs/design/SANDBOX_TEST_LAB.md`.

---

## Vibe

A highly advanced alien civilisation made everything out of one principle: **fold a flat surface
into a structure**. Walls are folded panels. Floors have crease lines. Ceilings are origami
vaults. Glyphs run along the fold lines like circuit traces but organic. Feels alien because
it is alien — no rivets, no pipes, no repair tape; just mathematics made physical.

Color palette: **deep teal and metallic gold** on matte black, with **cool white emissive
glyph lines**. High contrast. Zero grime (these were built to last). Accent: flashes of
amber at fold joints (the equivalent of load-bearing structure).

Think: sci-fi Kusudama meets Mesoamerican geometry meets circuit board at architectural scale.

---

## Core shapes (the "origami kit")

| Shape | Real-origami base | Game use | Scale |
|---|---|---|---|
| **Fold panel** | flat sheet with one diagonal crease | walls, floors, ceilings | 1 m – 10 m |
| **Chevron arch** | bird-beak fold | doorways, passage frames | 3–5 m tall |
| **Star cluster** | waterbomb / pinwheel | ceiling feature, light source socket | 1–4 m |
| **Lotus tower** | stacked petal layers | mid-size landmark, column | 5–15 m |
| **Tesseract shard** | irregular geometric fold | large set-dressing rock / debris | 2–8 m |
| **Glyph band** | flat strip with encoded crease marks | runs along fold lines as trim / decal | any length |

All shapes are **kitbashed from 4–6 base meshes** (fold panel, chevron, star section,
petal section, shard, glyph strip). New buildings = new configurations of the same parts.
No bespoke one-off meshes for this family.

---

## Surface materials (one `AlienOrigami` art kit)

| Material slot | Look | Notes |
|---|---|---|
| `OrigamiMatte` | flat black with micro-texture (crease surface) | bulk panels, floors |
| `OrigamiGold` | metallic gold, slightly reflective | fold edges, structural joints |
| `OrigamiGlyph` | teal emissive line on matte | glyph strips, "instruction" markings |
| `OrigamiAmber` | warm amber emissive | load-bearing joints, energy conduits |
| `OrigamiVoid` | deep black with subtle iridescence | shadowed interior panels |

Quest budget: **≤ 5 materials for this family**, shared across all meshes in a scene.
Use `OrigamiGlyph` sparingly (emissive = draw cost). Matte > reflective for performance.

---

## Glyph language (art direction, not gameplay text)

Glyphs are **geometric, not alphabetic** — triangles, concentric rings, sine-wave lines,
dot clusters arranged by a rule (left-to-right density = importance). They deliberately
look like they encode meaning (they do, eventually — see SYSTEMS_ARCHITECTURE.md story hooks)
but for now are decorative. Rules:
- Glyph lines always follow crease lines — they never cross free-floating across a panel.
- They scale with the panel: small panel → dense fine glyphs; big panel → sparse bold glyphs.
- Color: always `OrigamiGlyph` teal emissive — no other color for glyphs in this family.
- Decal meshes (thin quads offset from the panel surface) — not UV-painted textures,
  so we can reuse on any mesh shape.

---

## Prompt recipe (for AI image generation + Meshy/kit source)

```
Reference prompt (exterior):
"Alien origami architecture. A dark stone-black wall made of giant folded panels
with sharp geometric creases. Gold metallic fold edges. Glowing teal circuit-like glyph lines
running along every crease. Pristine, mathematical, zero visible repair. Sci-fi Mesoamerican
origami aesthetic. Unreal Engine 5 render, Quest-friendly geometry."

Interior version: replace "black wall" with "interior vault ceiling, star-shaped folded panels,
warm amber glow at the joints, cool ambient teal glyph light."
```

Use these as the concept brief for the AI image; then kit-bash the 6 base meshes to match.

---

## Build order (CI-gated; after Sandbox Test Lab exists)

1. **Prototype in Sandbox:** author the 6 kit meshes + 5 materials in the sandbox scene.
   Verify Quest budget (≤ 6 draw calls for a room corner, 72 FPS). Test glyph emissive cost.
2. **`AlienOrigami` `WorldArtKitDefinition`** ScriptableObject with the kit mesh/material IDs.
3. **Glyph decal meshes** (thin quads per glyph pattern, placed on panels at build time by patcher).
4. **Promote to a Pattern world** once kit passes audit: use the patcher to place fold panels,
   chevron arches, lotus column to build a playable room.
5. Ship audit must pass: Quest perf budget, no bespoke materials, all assets resolve by kit ID.

---

## Connection to story

The Alien Origami worlds are where the Earth-connection story hook lands (see design notes):
the glyphs are the same symbols found in ancient human cultures — players put it together
without being told. The glyph decal asset IDs are shared between this surface family and the
`Stone / Ceremonial Alien` family on purpose. Build the language now; write the story later.
