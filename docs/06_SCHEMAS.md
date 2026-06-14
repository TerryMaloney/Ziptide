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

## VisualThemeProfile (World dressing — A.5)

ScriptableObject in Ziptide.Visuals; used by WorldDirector for sky + planet + ground.

- **groundTint:** Color applied to ground plane.
- **skyGradient:** Gradient for sky sphere (vertical).
- **planet:** PlanetSettings — baseColor, accentColor, angularSizeDegrees, distance, direction (normalized), rotationSpeed, followPlayer.

Assets under `Assets/Ziptide/Content/VisualThemes/` (e.g. DefaultVisualTheme.asset).

---

## ItemDefinition (Content — C0)

ScriptableObject in Ziptide.Content; base for data-driven items (belt/holster, weapons).

- **itemId:** string — unique id (e.g. `"pistol"`); used by HolsterSocketInteractor allowed list.
- **modelPrefab:** GameObject (optional) — prefab for visual; if null, use existing renderer.
- **mass:** float — Rigidbody mass; 0 = leave default.
- **colliderSizeOverride:** Vector3 — override collider size; (0,0,0) = use mesh/bounds.

Assets under `Assets/Ziptide/Content/Items/` or Content/Runtime/Items as needed.

---

## PistolDefinition (Content — C0)

Inherits ItemDefinition. ScriptableObject for pistol tuning.

- All **ItemDefinition** fields (itemId, modelPrefab, mass, colliderSizeOverride).
- **fireRate:** float — seconds between shots.
- **range:** float — hitscan ray length.
- **hitForce:** float — passed to TargetRuntime.Hit.
- **recoilKick:** float — reserved for recoil.
- **hapticAmplitude:** float (0–1) — controller haptic on fire.
- **hapticDuration:** float — haptic duration in seconds.
- **muzzleFlashPrefab:** GameObject (optional) — spawned at Muzzle on fire; if null, short-lived primitive used.
- **fireClip:** AudioClip (optional) — played on fire.

Default asset: `Assets/Ziptide/Content/Items/DefaultPistol.asset` (created by ScenePatcher if missing).

---

## Registry / single source of "what exists"

- One registry (or manifest) so an LLM can orient instantly.  
- Document in MODULE_MAP.md and STATUS.md.

---

## WorldPackDefinition (Content — D0)

ScriptableObject in Ziptide.Content; defines a loadable world: scene, themes, jobs, spawn markers.

- **packId:** string — unique id (e.g. `d0_city`, `test_room`).
- **sceneName:** string — scene to load (must match Build Settings scene name).
- **defaultTheme:** VisualThemeProfile — theme applied when entering this world.
- **availableThemes:** List of VisualThemeProfile — themes offered at theme switch.
- **jobs:** List of JobDefinition — jobs offered (e.g. at DispatchKiosk).
- **spawnMarkers:** List of SpawnMarkerDefinition — named spawn points for GoToMarker and travel.

Assets under `Assets/Ziptide/Content/Worlds/Packs/`.

---

## SpawnMarkerDefinition (Content — D0)

Serializable; one entry in WorldPackDefinition.spawnMarkers.

- **markerId:** string — unique id referenced by GoToMarkerStep and travel.
- **localPosition:** Vector3 — position relative to world/pack anchor.
- **localEulerAngles:** Vector3 — rotation in degrees.

---

## JobDefinition (Content — D0)

ScriptableObject in Ziptide.Content; one job/tutorial with ordered steps.

- **jobId:** string — unique id within the pack.
- **title:** string — display title.
- **steps:** List of JobStepDefinition — ordered steps (use derived step assets).

Assets under `Assets/Ziptide/Content/Jobs/`.

---

## JobStepDefinition (Content — D0)

Abstract ScriptableObject base for a single job step.

- **stepLabel:** string — short label for UI.

Derived step types (each is a ScriptableObject asset type):

- **GoToMarkerStepDefinition** — markerId (string), arriveDistance (float).
- **CollectItemIdCountStepDefinition** — itemId (string), count (int).
- **DeliverToSocketStepDefinition** — socketId (string), itemId (string), count (int).
- **ShootTargetsCountStepDefinition** — count (int).

---

*Add schema docs and field lists as they stabilize. Prefer generated ScriptableObjects from schema.*
