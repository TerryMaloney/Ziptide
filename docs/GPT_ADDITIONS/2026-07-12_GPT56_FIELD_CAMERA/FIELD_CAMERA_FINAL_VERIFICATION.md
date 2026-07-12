# FIELD CAMERA — FINAL VERIFICATION

**Status:** full completion envelope pushed; Unity verification requested.  
**Foundation:** `a52d5fcaefd134236539a30574f898aa245dd588`, green run `29205797826`.  
**Completion staging head:** `3ecc517810843a28cf5f56e001b3d82e61bac233`.

## Delivered

- camera-owned 512×384 offscreen capture;
- live viewfinder capped at 10 Hz;
- capture-time-only composition subject detection;
- `PhotoComposition` scoring and rating;
- PNG storage under `persistentDataPath/Photos`;
- bounded 24-photo profile album with evicted PNG deletion;
- existing `SaveSystem` persistence only;
- one real grabbable camera dock in every runtime-created Quarters;
- six newest photos displayed in the Quarters, newest first;
- rating-colored frames and missing-file fallback;
- explicit cleanup for RenderTexture, readback texture, viewfinder material, wall textures, and wall materials;
- no scene YAML, second save owner, cloud gallery, network sync, reward mutation, or unbounded storage.

## Rails

- capture: 512×384 PNG;
- preview: at most 10 renders/second;
- album: 24 records;
- wall: 6 photos;
- scene-wide renderer classification only when the shutter fires;
- existing ItemFactory, holster, CameraAuthor, PlayerProfile, PhotoAlbum, and SaveSystem remain the sole owners.

## Device checks after green

1. take the camera from the Quarters or Sandbox;
2. verify the back screen shows a live view without recursive feedback;
3. capture a sky, landmark, and creature shot;
4. confirm haptic shutter and `PHOTO_CAPTURED` logs;
5. return to the Quarters and verify the newest six images appear;
6. fill beyond 24 captures and verify old files are retired;
7. holster and travel with the camera;
8. watch 72 Hz while the viewfinder is active.
