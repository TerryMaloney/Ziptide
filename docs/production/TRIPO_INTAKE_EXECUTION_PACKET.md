# TRIPO INTAKE EXECUTION PACKET — BASE GAME MONTH 1

**Status:** GENERATION/INTAKE AUTHORITY · no Unity import until the M0/model-band gate authorizes it
**Written:** 2026-07-27
**Purpose:** make the paid generation month produce reusable, game-ready base assets rather than disconnected pretty models

## 0. Non-negotiable rule

> **No stable game socket, no paid generation.**

Every generated keeper must already have or be assigned:

- a stable content/visual id;
- a canonical concept/reference;
- real dimensions;
- an existing or explicitly planned gameplay root;
- interaction/animation sockets;
- art tier and Quest budget;
- all campaign and side-mode consumers;
- skin/material zones;
- provenance/license evidence;
- a fallback placeholder;
- a booth and device acceptance route.

Generation produces geometry. It does not own gameplay, colliders, item ids, damage, save state, world progression or story flags.

Before purchasing or generating, re-check the service’s current commercial-use, ownership, redistribution, training-data and post-subscription terms and archive the evidence with the plan/date. The repository does not treat old screenshots or assumptions as current legal proof.

---

## 1. Intake folder contract

Keep raw generation outside Unity until accepted:

```text
art_intake/<asset_id>/
  SOURCE/
    concept crops
    generation inputs
    prompt/settings record
  RAW/
    generated GLB/FBX + textures
  KEEPER/
    one selected source asset
  MANIFEST.md
  LICENSE_RECORD/
  REVIEW/
    silhouette/contact-sheet notes
    cleanup checklist
```

Only accepted cleaned outputs move under `Ziptide/Assets/` through the future WC-5/intake path.

`MANIFEST.md` fields:

- asset id and display name;
- source concept paths;
- generation date/tool/plan;
- variants generated and keeper selected;
- intended worlds/modes;
- real scale;
- expected tier/budget;
- material/skin zones;
- rig/animation/sockets;
- cleanup issues;
- license/provenance references;
- reviewer verdict.

---

## 2. The three intake pilots

Do these before any volume batch. A failure changes the intake system, not only the mesh.

### Pilot A — Taser hero visual

**Why:** existing stable item id, runtime, projectile, grip, muzzle, Forge visual, cosmetics, holster, travel and both campaign/Arena consumers.

**Target:** `taser_dart_gun` / visual successor to `taser_gun_mk1`.

**Must preserve:**

- stable root/item id;
- existing grip and muzzle ownership;
- room for moving slide/coil/mechanical part sockets;
- body/grip/barrel/emissive separated material zones;
- sight and optional attachment points;
- highest face-distance texel density;
- one-hand dimensions and child reach.

**Proves:** held-item cleanup, pivot, UV/zones, skin compatibility, LOD, applier swap, campaign/Arena inheritance and Quest face-distance quality.

### Pilot B — W001 signature creature

**Why:** proves the hardest recurring content path before generating monsters in volume.

**Target:** canonical canal stalker id assigned through the W001 creature packet.

**Must preserve/declare:**

- stable species/behavior id;
- skeleton and animation list;
- gameplay tell bones/materials;
- colliders/hit zones owned outside the visual where possible;
- disable/capture/salvage state;
- LODs and shadow limits;
- skin/material zones;
- campaign encounter + Horde/Arena eligibility.

**Minimum animation list:** idle/breathe, locomotion, warning/telegraph, attack/counter response, hit/stagger, non-lethal disable, recover/retreat where applicable.

**Proves:** rig/animation intake, tell preservation, creature applier, Horde inheritance and Quest motion/readability.

### Pilot C — one W001 hero landmark

**Recommended:** leaning Tower or Dockmaster booth, whichever has the clearest final gameplay placement first.

**Must preserve:**

- stable landmark/POI id;
- real footprint/height;
- collision and walkable surfaces generated separately where practical;
- material-family conformance;
- LOD/impostor story;
- no embedded gameplay scripts;
- reference-plate comparison against approved W001 concept.

**Proves:** world hero intake, scale, collision, lighting/material cohesion and deterministic placement.

Do not proceed to the broad batch until all three have passed source checks, booth review and a representative Quest build.

---

## 3. Priority generation queue

### Tier 1 — recurring identity and first-level product

1. **Taser hero visual** — Pilot A.
2. **W001 canal stalker** — Pilot B.
3. **W001 leaning Tower or Dockmaster booth** — Pilot C.
4. **RILL orb** — recurring face-distance companion; separate iris/channel material zones and state-driver sockets.
5. **SLV-01 Scrapper hull** — exterior visual only; stable root retains seats, doors, coupler, colliders and boarding.
6. **Scrapper grabber arm** — separate articulated asset with pivots; never fused decoratively into hull.
7. **Scrapper cockpit console cluster** — physical lever/toggle/throttle sockets; child/seated reach.
8. **Artifact key half A**.
9. **Artifact key half B**.
10. **W001 Dockmaster/contract hero prop set** — booth/ledger/stamp/harbor crane as stable POI/prop ids.
11. **W001 derelict/beached barge hero wreck** — first derelict-catalog proof.
12. **Gate pillar/monolith cluster** — geometry only; tide/entrainment remains VFX.

### Tier 2 — first four worlds and signature enemies

13. **W002 swarmer hero body** — use one imported signature exemplar; bulk/variants remain Forge.
14. **W003 tendril hero body** — anchored creature with wind/telegraph bones.
15. **Warden human-scale drone** — recurring enforcement enemy, shared with Horde/Tidefront.
16. **Warden capital close-range visual** — only after distant Forge silhouette has been judged insufficient for an actual close story beat.
17. **W002 pump-house hero machine shell** — gameplay repair/socket roots remain separate.
18. **W003 wind-baffle hero relay**.
19. **W004 broadcast spine hero machine**.
20. **W004 memory-shard hero prop**.
21. **Tide skiff** — recurring small craft/vehicle/derelict family proof.

### Tier 3 — shipped-quality arsenal

22. **Gravity projector/tool** — heavy two-hand visual with emitter, cable and attachment zones; distinct from Glove interaction.
23. **Breaker Blade** — only after final grip/length contract is frozen.
24. **Tide Pike** — only after final two-hand/reach contract is frozen.
25. **Sonic Thumper** — heavy/two-hand moving/impact sockets.
26. **Prism Beam** — charge chamber and beam emitter zones.
27. **Static Net launcher/throwable family** — use generic throwable/launcher root.
28. **Foam Cannon** — generate after foam effect receiver/runtime is defined.
29. **Arc Rifle** — generate after shared chain-effect and two-hand contract exists.
30. **Shield Disc** — generate after returning-projectile/shield sockets are defined.

### Tier 4 — home density and reusable prop families

31. Pilot seat.
32. Bunk pod module.
33. Oil lantern identity prop.
34. Floodlight cluster.
35. Cargo family: crate/barrel/strapped bundle.
36. Contract desk/ledger/stamp.
37. Salvage hand-tool rack.
38. Door/airlock hero module.
39. Fabricator/workshop hero shell.
40. Garden hero props: watering can and one rare crop body.

### Tier 5 — second hero ship and later campaign

Only after MK2 acquisition and room contracts are locked:

41. MK2 exterior hull.
42. MK2 cockpit shell.
43. MK2 lounge/galley shell.
44. MK2 bedroom/bunk shell — currently missing concept; concept first.
45. MK2 greenhouse shell.
46. MK2 drive-heart.
47. MK2 workshop/fabricator bay.
48. MK2 airlock/grafted salvage module.
49. modular skin/attachment parts.
50. one close-range Architect or Warden hero landmark from the next chapter batch.

---

## 4. What stays Forge-native

Do not waste paid generations on content whose value comes from repetition, modular precision or controlled variation:

- tiling building facades, doors, windows, roofs and trim modules;
- cave tunnel/chamber/junction/shaft kits;
- generic scatter/dressing and bulk cargo;
- common creature variants and distant populations;
- distant fleet/background ship silhouettes;
- procedural plants and most crops;
- terrain, skies, Pattern overlays and gate/water VFX;
- LOD/impostor representations;
- cosmetics/skin colorways.

External geometry is reserved for signature silhouettes, close-range hero surfaces and difficult rigged creatures. The Forge and skin systems generate breadth.

---

## 5. Per-asset intake brief template

```text
ASSET ID:
DISPLAY NAME:
CANON SOURCE:
USED BY WORLDS:
USED BY MODES:
GAMEPLAY ROOT / DEFINITION:
FALLBACK VISUAL:
ART TIER:
REAL DIMENSIONS:
CAMERA BAND:
TRI / MATERIAL / TEXTURE / LOD BUDGET:
PIVOT:
COLLIDER OWNER:
RIG / SKELETON:
ANIMATIONS:
INTERACTION SOCKETS:
GAMEPLAY-TELL SOCKETS:
MATERIAL ZONES:
FIXED IDENTITY ZONES:
SKIN / ATTACHMENT REQUIREMENTS:
CONCEPT INPUTS:
GENERATION SETTINGS / VARIANTS:
CLEANUP CHECKLIST:
PROVENANCE / RIGHTS RECORD:
BOOTH ACCEPTANCE:
QUEST ACCEPTANCE:
SIDE-MODE INHERITANCE CHECK:
```

No field may be ignored merely because the generator produced a visually strong result.

---

## 6. Character and creature rule

### Cal

The approved low-risk path remains first-person hands/forearms/chest/boots plus a limited full-body asset for mirror/title/staged moments if needed. Do not build a full NPC-animation engine for a body the camera rarely sees.

A generated Cal body must:

- use one clean skeleton;
- preserve red laces, tally guard and personal identity zones;
- separate suit/gloves/straps/visor/accent materials;
- support modular helmet/chest/arm/boot attachments;
- share skins without rerigging;
- limit animation scope to actual camera use.

### Creatures

Use a hybrid roster:

- imported hero body for signature/face-close species;
- Forge-native bodies and skins for common variants;
- same behavior/species id underneath both;
- animation retarget only within declared compatible skeleton families;
- no visual swap may erase gameplay tells.

The first imported creature decides the intake standard for the rest.

---

## 7. Ships and derelicts rule

Generate ship geometry as modular packages:

- hull visual;
- articulated appendages separately;
- cockpit/interior shells separately;
- doors/landing gear/moving segments with pivots;
- hardpoint/engine/boarding/coupler sockets declared;
- material/skin zones and fixed identity zones;
- close LOD and distant/impostor package.

Derelicts should reuse a clean source hull or station family with damage-state kits:

- breached panels;
- exposed ribs;
- burn/acid/flood/Pattern/Bloom overlays;
- missing modules;
- interior entry/room variants.

Do not generate a unique complete wreck for every world. Generate reusable hull families and damage modules, then let world data compose the wreck.

---

## 8. End-of-generation close

For each keeper:

1. archive legal/plan/date evidence;
2. fill manifest and intake brief;
3. keep one source winner;
4. record rejected silhouette reasons;
5. list cleanup work honestly;
6. do not copy raw output into Unity;
7. update the campaign/content matrix with the asset’s readiness state;
8. update the shared-inheritance manifest with all consumers;
9. queue intake only after the gameplay root/socket is stable;
10. close only after concept comparison and Quest verdict.

A large folder of attractive GLBs is not progress unless the assets are traceable to stable game content and can be swapped without breaking the game.
