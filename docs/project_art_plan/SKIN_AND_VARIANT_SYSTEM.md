# SKIN & VARIANT SYSTEM — how to make 500 Cals (and reskin any gun on a whim)
### Terry 2026-07-21: "any characters we make… make 500 different versions of Cal, all sorts of colors and designs on the guns… build the gun once then alter the design on a whim. Not sure how that works."

**Status:** 🔵 PLAN — zero code (freeze). This is the answer to "how variants work," and the rule
it forces on the Tripo month. Companions: `TRIPO_MONTH_PLAN.md` (build-once discipline),
`HERO_ASSET_STRATEGY.md` (tiers), `CONCEPT_TO_BUILT_PIPELINE.md` (intake).

---

## §0 — THE ONE INSIGHT (this is the whole thing)

**Separate GEOMETRY (expensive, make ONCE) from SKIN (cheap, make infinitely).** You never
regenerate the mesh to make a variant. 500 Cals = ONE Cal mesh wearing 500 different skins.
Every skin shop in games (Fortnite, CoD, Halo) works exactly this way. Tripo builds the body/gun
one time; a lightweight **skin layer** does all the variety after that, most of it for free.

## §1 — THE THREE LAYERS OF VARIANCE (cheapest first)

**Layer 1 — PARAMETRIC TINT (free, runtime, effectively infinite).**
The base mesh's material exposes a few knobs: **primary color · secondary color · accent/emissive
color · metal-vs-matte · wear amount · pattern pick.** Turn the knobs → new look, ZERO new
assets, changeable at runtime, live. One material with 4 color knobs already yields thousands of
combinations. *This is literally "alter the gun's design on a whim" — expose the params, spin the
dials.* A "random 500 Cals" button = 500 random parameter sets on one mesh.

**Layer 2 — AUTHORED SKIN SETS (cheap, one texture pass per skin).**
For looks a tint can't do — a painted pattern, faction livery, camo, gold/chrome, battle damage,
a themed drop — you author ONE texture set (albedo + roughness + metal + emissive) baked onto the
mesh's fixed UVs. Same mesh, new "outfit." This is where the **500 DESIGNS** come from: generate
skin concepts in 2D (nano banana, on a flat template of the gun/character), pick winners, bake to
textures. AI image-to-texture tools can produce the PBR set from a prompt straight onto the UV.
Cost per skin ≈ minutes, not a remodel.

**Layer 3 — MODULAR PARTS (cheap structural variety, combinatorial).**
For changes bigger than paint — a different helmet, barrel, scope, stock, backpack — build a small
KIT of interchangeable parts that snap onto **attach points** on the base. Cal = base body +
swappable {helmet, chest pack, arm-guard, boots}; gun = base frame + swappable {barrel, sight,
grip, magazine}. 5 parts × 4 slots ≈ hundreds of distinct silhouettes from a handful of models.
This is the "different design," not just different color — without ever rebuilding the base.

## §2 — HOW IT LIVES IN OUR ARCHITECTURE (it already fits)

We're already data-driven + applier-based, so this is a natural extension, not a new paradigm:
- **`SkinDefinition` (ScriptableObject, resolved by string ID)** — like every other Definition:
  `targetAssetId` (which base mesh) · tint params (Layer 1) · optional texture-set ref (Layer 2) ·
  optional attachment-part list (Layer 3). 500 Cals = 500 `SkinDefinition`s pointing at one Cal.
- **`ForgeVisualApplier`** already swaps the visual child on a stable root (colliders/grab points
  never move) → applying a skin = swap material params / textures / child parts, gameplay unaware.
- **Material families** already define our look; skins vary WITHIN a family (or declare a new
  family for a themed drop) → cohesion preserved by construction.
- **Quarters/ship/room skins use the SAME system** (Terry's "new quarters as a skin"): a room is
  just another skinnable asset id. Character skins, weapon skins, ship skins, room skins — one
  mechanism.

## §3 — THE RULE THIS FORCES ON THE TRIPO MONTH (author for variance up front)

When a mesh is meant to be skinnable (every character + hero weapon + the MK2 ship + rooms), the
intake cleanup MUST set it up so skins are cheap later. Non-negotiable at build time:
1. **ONE clean UV layout** (skins bake onto it; a messy UV makes every future skin painful).
2. **SEPARATED MATERIAL ZONES** — independent sub-materials for regions you'll want to recolor
   apart: e.g. Cal = {suit · gloves · straps · visor · accents}; gun = {body · grip · barrel ·
   emissive}. This is what lets a tint hit "just the accents" or "just the grip." **The single
   most important variance-enabler.**
2b. **A FIXED-ZONE list** (never reskinned — Cal's identity anchors: bird trinket, red laces, the
   tally guard; the ship's grafted salvage gear) so skins can't erase who it is.
3. **ATTACH POINTS** wherever modular parts are wanted (Layer 3) — declared sockets on the base.
4. **Neutral base texture** (mid-grey PBR) so tints read true.
Tripo builds the geometry once, to THIS spec; the skin system generates the thousands of variants.

## §4 — THE VARIANT WORKFLOW (making the 500)

1. Build base mesh once (Tripo/hero), cleaned to §3 (UV + zones + fixed-list + attach points).
2. **Layer-1 catalog:** define the tintable params; a generator can spit out N random valid
   combos instantly (respecting fixed zones + family rules) → your "500 colorways" in one pass.
3. **Layer-2 drops:** design special skins in 2D (nano banana on the flat template), bake the
   winners to texture sets → the premium/themed skins.
4. **Layer-3 kit:** model the handful of swap parts once → combinatorial silhouettes.
5. Each becomes a `SkinDefinition` (ID); the shop lists IDs; the applier applies. Done.

## §5 — HONEST NOTES / GUARDRAILS

- **Tripo is for the BASE, not the 500.** Don't burn generations making variants — that's the
  skin layer's job (mostly free). Generate one clean base per character/weapon; maybe a few
  Layer-3 parts.
- **Rigged characters:** Cal's base must be cleanly rigged ONCE; all skins/parts ride the same
  skeleton (parts weighted to it). Per the Cal route-1 decision, the visible body is limited
  (first-person + mirror/menu), which keeps the rig cost sane.
- **Perf on Quest:** skins are texture/material swaps → cheap; watch total unique-texture memory
  (share atlases across skins where possible; stream the shop's previews). Modular parts add
  draw calls — keep the base + a few parts, not dozens live at once.
- **Cohesion gate:** skins still pass the conformance audit (a "gold chrome" skin is fine; a skin
  that breaks the family or erases a fixed zone is rejected). Freedom within the guardrails.
- **This is the skin-SHOP substrate too** (monetization/customization): the same `SkinDefinition`
  IDs the shop sells. Character skins · weapon skins · ship skins · room/quarters skins — one system.
