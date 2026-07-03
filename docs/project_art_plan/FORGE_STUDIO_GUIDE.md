# THE FORGE STUDIO — prompt → in-game asset, no artists, no Tripo, no fees

**What this is:** Ziptide's LLM asset studio. Terry (or anyone) types a prompt to a Claude session;
the session writes a **ForgeRecipe** (data); deterministic C# builds the actual mesh; CI renders
**turnaround photos** the session can *look at* and iterate against; the next APK carries the asset.
The studio is Claude Code itself — no separate app, no per-token API spend, no asset marketplace.

Proven end-to-end 2026-07-03 (sprint ART-2): the CI photo loop renders PNGs a Claude session can
view and act on. First asset: `taser_gun_mk1`.

---

## The loop (what actually happens per prompt)

```
1. Terry:   "Picasso, make the taser a beat-up salvage pistol — drill grip,
             exposed coil on top, teal charge windows."
2. Claude:  edits/adds a Build*() method in Editor/Patching/ForgeRecipeLibrary.cs
            (parts + palette + sockets + budget + surfaceFamily), pushes.
3. CI:      EditMode tests enforce the guardrails (budget, sockets, palette law);
            the Forge Photos workflow AUTO-TRIGGERS on any Forge-file push and
            renders 7-angle turnarounds (~5–10 min, no APK).
4. Claude:  downloads the `forge-photos` artifact, LOOKS at the PNGs, adjusts the
            recipe, pushes again. Repeat until it reads right.
5. Terry:   sees the final turnarounds in chat; next APK carries the real asset;
            headset check for grip/feel only.
```

### The commands Claude uses (step 4)
```bash
# newest forge-photos run for the branch
curl -s "https://api.github.com/repos/TerryMaloney/Ziptide/actions/workflows/forge-photos.yml/runs?per_page=1"
# artifact id from the run, then:
curl -sL -o photos.zip "https://api.github.com/repos/TerryMaloney/Ziptide/actions/artifacts/<ID>/zip"
unzip photos.zip && <view the PNGs>
```
Terry can also run it locally anytime: **`Ziptide → Art → Forge Photo Booth (render all)`** →
`Ziptide/Builds/Photos/<recipeId>/*.png`.

---

## Prompt grammar (what makes a good asset prompt)

**`[subject] + [silhouette adjectives] + [distinct features] + [surface family / faction] + [palette intent]`**

Examples:
- *"A rust-red patrol drone: squat disc body, two whip antennae, single teal eye — Salvage family."*
- *"An Origami warden totem: folded matte-black column, gold joint seams, teal glyph band — AlienOrigami."*
- *"A hammerhead cargo ship hull, twin engine pods underslung, tape-patched — Salvage, 8k tris."*

What the LLM turns that into (the recipe vocabulary — `ForgeRecipeDefinition.cs`):
| Piece | Means |
|---|---|
| `ForgeOp` (7, closed) | BeveledBox · Cylinder · Tube · Lathe (profile revolve) · SphereSection (domes) · Wedge · GreebleStrip (mechanical detail row) |
| `ForgePart` | one placed shape: size/position/rotation/scale, `mirrorX` (symmetry for free), `paletteSlot`, `smooth` |
| `palette` | ≤6 colors → ≤6 draw calls; colors shared across recipes collapse into shared materials |
| `sockets` | named attach points; **Grip must bake +45° X** (Quest controller tilt) and implies a Muzzle |
| `budgetTris` | hard cap — over it, tests AND the build audit fail |
| `surfaceFamily` | ties the asset to the art bible; strict families (AlienOrigami) reject off-canon colors in tests |

## Guardrails (enforced, not requested)
- `Validate()` + `ForgeRecipeLibraryTests`: budget, socket contract, palette law, size sanity, schema version.
- `ForgeAuditRules` (build blocker): an item pointing at a missing recipe, or a recipe over budget, fails the APK.
- Recipes are **create-only** seeds: the `.asset` under `Resources/Forge/` is the live truth once created
  (delete it to reseed from code). Same contract as every other authoring library in the repo.
- Budgets by type: handheld ≤3k tris · drone/creature ≤2k · large prop ≤5k · ship hull ≤8k.

## Wiring an asset into the game (who consumes recipes)
- **Items/weapons:** set `forgeRecipeId` on the item's definition — done in data by
  `ForgeAuthor.Assignments` (one line per pairing, build-hooked). `ItemFactory` applies it via
  `ForgeVisualApplier` automatically; missing recipe = primitive fallback, never a break.
- **Coming next:** drone/creature bodies (story-lane MakePart seam — coordinate first), ship hull,
  cosmetic mesh variants, W001 building kit pieces (ART-3).

## Story tie-in
`surfaceFamily` + `storyTags` bind every asset to `ART_DIRECTION_MASTER_PLAN.md` and the canon:
Salvage tech carries RILL's teal as its charge color; Origami tech is teal/gold/matte-black by law;
Warden gear escalates with Signal tier. An off-brand asset fails EditMode — canon drift is a red X,
not a review comment.
