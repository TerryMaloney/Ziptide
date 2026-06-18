# Asset Swap Pipeline — placeholder → real model (Tripo3D → Unity)

**The repeatable process for replacing a placeholder primitive (the turquoise gun block, the
drone capsule, etc.) with a real model — and making a VR weapon line up with the Quest controller.**
First instance: the Tripo3D **taser/blaster** gun. Same steps reuse for drones, props, tools.

> This is *art-swap only* — it does not change gameplay logic. Models are visual children; the
> grab/fire/holster behavior stays on the existing runtime (`ItemFactory`, `*GunRuntime`).
> Do this on a build AFTER the current bug-fix run — not blocking the first fixes.

---

## 0. Generate the model (Tripo3D) — prompt for a grip that aligns

The single biggest VR-weapon gotcha: Quest controllers are held at a **forward tilt**, so a gun
modeled "flat" points up or down relative to where your hand actually aims. Two ways to handle it
and you want **both pulling the same direction**:

1. **Prompt the model with a real grip.** Ask Tripo for a **"cordless power-drill style handle"** /
   pistol grip that meets the body at roughly a **100–110° angle** (not a straight flat bar). A
   drill-grip model + the Unity offset below cancel out to a natural aim pose.
2. **Model facing:** aim for the **barrel pointing along +Z** and **up = +Y** in the export, handle
   downward. If Tripo gives you a different orientation, you fix it with the parent rotation in step 3.

**Tripo export settings (use GLB):**
- Format **GLB** (binary; mesh + PBR textures in one file — Tripo's default, best for Unity/Quest).
- **PBR metallic-roughness** materials (Unity URP reads these; avoid specular-glossiness/custom nodes).
- Keep poly count modest for Quest (decimate/optimize in Tripo; target low-thousands tris for a gun).
- Bake textures at 1–2K max for a handheld prop.

---

## 1. Import the GLB into Unity

1. Drop `taser.glb` into `Assets/Ziptide/Content/Models/Weapons/` (create the folder).
2. **Scale:** Tripo/GLB often imports tiny or huge. Select the asset → Inspector → set **Scale Factor**
   until the gun reads ~**20–25 cm** long in the scene (compare to the existing primitive). GLB is
   metric, but verify — a gun should be ~0.2 m, not 2 m or 0.02 m.
3. **Materials:** if it imports grey, click **Extract Materials** (and Extract Textures) so URP can
   bind them; confirm the shader is URP/Lit. Re-assign the albedo/metallic/normal if needed.
4. **Read/Write + colliders off on the mesh:** the visual mesh needs no collider (we add a simple
   box, step 2). Turn off "Generate Colliders" on the import.

---

## 2. Make it a prefab with a clean pivot + collider

1. Drag the model into a scene, then wrap it: create an empty `GameObject "TaserModel"`, parent the
   imported mesh under it at local (0,0,0). The **wrapper is your alignment handle** — you rotate the
   wrapper, never the raw mesh.
2. Add a single **BoxCollider** sized to the gun body (for grab + holster proximity).
3. Save as a prefab: `Assets/Ziptide/Content/Models/Weapons/TaserModel.prefab`.

---

## 3. Swap it in — replace the primitive, keep the runtime

The guns are built in code by **`ItemFactory`** (`Gameplay/Runtime/Items/ItemFactory.cs`). Each gun
creates a primitive body + a child **`"Grip"`** that is the XR **`attachTransform`**, plus a `"Muzzle"`.
Two clean options:

- **Option A (data, preferred long-term):** give the gun's `*GunDefinition` a `modelPrefab` field
  (ItemDefinition already has `modelPrefab`), and have `ItemFactory` instantiate it instead of the
  primitive when set. Then swapping art = assigning the prefab on the asset, no code edits. *(Small
  ItemFactory change — Architect/Content lane.)*
- **Option B (quick):** in `ItemFactory`, replace the `GameObject.CreatePrimitive(...)` body for that
  gun with `Object.Instantiate(taserModelPrefab)`. Keep the existing `Grip` and `Muzzle` children.

Either way: **keep the existing `Grip` (attachTransform) and `Muzzle`** — fire raycasts from Muzzle,
grab snaps to Grip.

---

## 4. The Quest grip offset (the up/down aim fix) — THE important bit

Today `ItemFactory` sets the Grip's **position** but leaves its **rotation at identity** (see
`ItemFactory.cs` ~lines 82–85, 124–127, 165–168 — `grip.transform.localPosition = ...` with no
`localRotation`). That's exactly why a real model will point off-axis.

**Fix:** rotate the Grip (the attach transform) ~**+45° on X** so the held gun aligns with the
controller's natural forward tilt:

```csharp
var grip = new GameObject("Grip");
grip.transform.SetParent(go.transform, false);
grip.transform.localPosition = new Vector3(0f, -0.01f, -0.05f);
grip.transform.localRotation = Quaternion.Euler(45f, 0f, 0f); // Quest controller forward-tilt offset
grab.attachTransform = grip.transform;
```

Tuning notes:
- Start at **45°**. Typical range is **40–45°**. If the muzzle points the *wrong* way (down instead of
  up), use **-45°** instead — depends on the model's forward axis from step 0.
- Also nudge `localPosition` so the **handle** sits in the palm (not the gun's center): roughly
  `y ≈ -0.04..-0.06`, `z ≈ -0.04..-0.08`, tune on-device.
- Because XR Grab uses `useDynamicAttach = false` + `trackRotation = true`, the gun **snaps to this
  Grip pose** on grab — so this single transform fully controls aim. Get it right once per gun.

---

## 5. Verify on device

- Grab → gun snaps to hand, **barrel points where you'd point a drill**, not up/down.
- Fire → ray comes from the **muzzle tip** (move the Muzzle child if the bolt starts off-nose).
- Holster → drops onto the belt socket and **travels** between scenes (holster proximity detect).
- Frame check: a handheld prop should not blow the Quest poly/texture budget (`docs/07_PERF_BUDGET.md`).

---

## Reusing this for any model (drones, props, tools)

Same loop every time — this is the **"drop in a visualization" half** of the enemy/content workflow:
1. Generate (Tripo, GLB, PBR, low-poly, correct facing).
2. Import (scale to meters, extract materials).
3. Wrapper prefab + one simple collider (rotate the wrapper, never the raw mesh).
4. Swap the placeholder for the prefab via a `modelPrefab` field (data) — keep the runtime script.
5. Verify on device (scale, pivot, grip offset for held items).

For enemies, the *movement / attack / hit-response* half is in
[`DRONE_COMBAT_v1.md`](DRONE_COMBAT_v1.md).

## Links
`docs/07_PERF_BUDGET.md`, `MILESTONE_C0_WEAPONS.md`, `ART_AUDIO_CONTENT_ARCHITECTURE.md`,
`docs/project_art_plan/`, `ItemFactory.cs`.

Sources for the technique:
- Tripo3D export formats / GLB → Unity (scale + PBR): https://www.tripo3d.ai/tutorials/tripo-ai-export-formats , https://www.tripo3d.ai/discover/3dgame
- XR Grab Interactable attach transform = grip pose/orientation: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/general-setup.html , https://wirewhiz.com/making-dynamic-vr-interaction-using-grip-points/
