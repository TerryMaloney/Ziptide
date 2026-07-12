# FIELD CAMERA — COMPLETION LOG

**Owner:** GPT-5.6 Thinking, Terry-authorized next-best sprint  
**Branch:** `terry-local-wip` via atomic staging branch `gpt56-staging/field-camera-complete`  
**Status:** 🟡 CLAIMED — foundation green; runtime capture, item availability, and Quarters wall in progress  

## Existing green foundation

- `PhotoComposition` deterministic scoring and rating bands;
- `PhotoAlbum` bounded 24-record ring with evicted-record return;
- additive `CapturedPhoto` profile schema v3 + migration;
- `CameraDefinition`, `CameraAuthor`, `ItemFactory.CreateCamera`, and shutter/viewfinder shell in `CameraRuntime`;
- BuildAndroid camera author hook;
- durable green head: tested SHA `44208c784418594a5f707de06f35f4f83ee8689e`, run `29205519639`.

## Completion envelope

### Runtime capture

- one camera-owned 512×384 `RenderTexture`, hidden child `Camera`, readback `Texture2D`, and viewfinder material;
- low-rate live viewfinder rather than full-rate offscreen rendering;
- trigger capture renders, reads PNG, evaluates `PhotoComposition`, appends to the bounded profile album, deletes an evicted PNG, and saves through the existing `SaveSystem`;
- resources are destroyed by the camera owner; no persistent second camera singleton;
- camera culls the viewfinder/UI layer to prevent recursive feedback.

### Subject detection

- capture-time only renderer-frustum scan;
- signature sky tier when `SkyVistaRig` is present;
- horizon from lens pitch/FOV;
- celestial body, landmark, and creature classification from the existing authored naming vocabulary;
- deterministic centered-subject score and subject id.

### Player availability

- Quarters gains one grabbable `handheld_camera` dock;
- no display de-fanging on the camera: it is a real travel-capable item using the existing item/holster system;
- no scene or prefab YAML.

### Quarters photo wall

- six newest album photos, newest first;
- rating-colored frames;
- missing/corrupt files show a neutral placeholder instead of breaking the room;
- loaded textures/materials are destroyed with the wall;
- no cloud, networking, gallery, external sharing, or unbounded storage.

### Verification

- pure horizon, centering, classification, filename, and wall-layout tests;
- source wiring pins `CameraRuntime` → capture owner, exact Quarters dock/wall calls, save/eviction/delete behavior, cleanup, and live-viewfinder rate rail;
- no duplicate save owner, no `DontDestroyOnLoad`, no second item registry, no scene YAML.

## Locked rails

- album cap remains 24;
- capture is 512×384 PNG;
- live viewfinder target ≤10 Hz;
- maximum six displayed wall photos;
- no per-frame scene-wide subject scan;
- no photo rewards beyond the already-defined `EarnsDiscovery` result in this sprint;
- no UI/menu overhaul;
- three CI reds stops the envelope.
