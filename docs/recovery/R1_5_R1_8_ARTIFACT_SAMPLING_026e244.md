# R1.5–R1.8 Independent Artifact Sampling — `026e244`

**Review date:** 2026-07-16  
**Source SHA:** `026e2446aeb77713a4779130cbe635ec10a6f059`  
**Recovery PlayMode run:** `29497675520`  
**Workflow artifact:** `8375190439` (`recovery-playmode-r1-results-026e2446aeb77713a4779130cbe635ec10a6f059`)  
**Downloaded archive SHA-256:** `de6ee737bb2d055ffab2d2257ddfbb7d15cc929f9c46b8154f859059c3ae39a9`  
**NUnit result:** **38 total / 38 passed / 0 failed / 0 skipped / 0 inconclusive**  
**Verdict:** **R1.5–R1.8 artifact sampling complete. Clear to begin R1.9. Still not headset-ready.**

## Scope and method

This is an independent review of the raw green artifact, not a restatement of the test result. The following were opened and inspected directly:

- `playmode-results.xml` and `playmode.log`;
- every actual-scene runtime census and runtime-artifact report;
- Home Hub, W000 and ToxicCity PNGs and their measurement JSON;
- W000/ToxicCity standing-clearance reports;
- Home Hub, W000 and ToxicCity UI spatial reports;
- controlled stale-artifact, spawn-obstruction and UI-failure canaries;
- round-trip travel/save output and persistent-owner identity assertions.

No threshold, assertion or finding was changed as part of this review.

## R1.5 — actual boot and Home Hub

The actual `_Boot` scene reached the Golden Home Hub with:

- one active player camera under the persistent rig;
- one active `XRInteractionManager` and one active canonical `InputActionManager` on the same persistent manager object;
- one persistent player rig, SaveSystem, TravelCoordinator and AudioDirector;
- no forbidden automatic owner active;
- no runtime-artifact or census finding;
- bilateral simulated XR controls resolving and reading all eight real Move/Turn actions;
- Settings and New Game exercised through the real XRI interactable route.

The apparent two-manager count in the raw manager array is not duplication: one record is the `XRInteractionManager` component and the other is the colocated `InputActionManager` component, each with its distinct required role.

## R1.6 — travel, persistence and standing clearance

The exact successful sequence was:

`_Boot → W000_DriftIn → ToxicCity → W000_DriftIn`

Raw output proves:

- three `TRAVEL_START` / three `TRAVEL_OK` completions in the exact expected order;
- zero `TRAVEL_FAIL` and zero `XRI_NOT_READY` records;
- exactly three travel autosaves, one per hop;
- a newly created player ID remained identical through both destinations, return and explicit disk reload;
- the probe flag and probe resource survived autosave and reload;
- inventory save/restore ran through the real travel path;
- the same persistent rig, XRI manager, input manager, SaveSystem, TravelCoordinator, AudioDirector and input assets survived each hop;
- both deferred golden destination captures completed (`captured=2/2`).

### Actual standing-volume evidence

| Arrival | Floor collider | Head above floor | Torso overlaps | Findings |
|---|---|---:|---:|---:|
| W000 first arrival | `__W000_DRIFT_IN_ROOT/District_BunkBay/BunkBay_Ground` | 1.650 m | 0 | 0 |
| ToxicCity | `__TOXIC_CITY_ROOT/Connections/Bridge_Dispatch_CanalRow` | 1.650 m | 0 | 0 |
| W000 return | `__W000_DRIFT_IN_ROOT/District_BunkBay/BunkBay_Ground` | 1.650 m | 0 | 0 |

The controlled spawn canary also correctly separates a clear standing volume from an intentionally torso-blocked fixture.

### Classification of the W000 `buriedAtTorso=True` log

This line is **not** a contradiction with the arrival evidence and is not a current spawn blocker.

`SpawnMarkerRuntime.Start` performs an old coarse diagnostic at the static marker: `Physics.CheckSphere(marker + 0.9 m, 0.25 m)` with the default query behavior. It includes trigger volumes, does not exclude the marker hierarchy or rig, and runs before it evaluates the settled tracked-head standing volume. It therefore answers “does any collider touch this sphere at the marker?” rather than “is the player torso embedded after arrival?”

The dedicated recovery audit runs after settlement against the actual tracked-head position, uses a conservative torso capsule, ignores triggers, excludes the persistent rig and floor collider, names every overlap, and returned zero overlaps on both W000 arrivals. The old log remains diagnostic-noise debt and must not be used as Quest clearance proof.

### Classification of headless `grounded=False`

The zero-input virtual rig logs `CharacterController.isGrounded=False` because no locomotion `Move` call refreshes the controller grounding state in this headless route. It does not outweigh the explicit floor ray and non-overlapping standing capsule. The old device observation of prolonged `grounded=False` remains a bounded Quest checkpoint item; it is not silently declared solved by desktop CI.

## R1.7 — directly reviewed visual evidence

| View | PNG bytes | Quantized colors | Avg luminance | Std. dev. | Dynamic range | Review |
|---|---:|---:|---:|---:|---:|---|
| Home Hub | 123,514 | 351 | 0.269 | 0.232 | 0.962 | Real board; title and both choices readable |
| W000 spawn | 158,766 | 389 | 0.244 | 0.154 | 0.816 | Real world geometry and labels; dark/rough presentation |
| ToxicCity spawn | 303,422 | 216 | 0.253 | 0.139 | 0.949 | Real city, railing/buildings/moon and Rill caption; no teleport shell |

The ToxicCity result independently confirms Fable 5's timing diagnosis: the corrected capture is a composed city view, not the previous three-color cyan transition shell.

### Presentation debt observed, not hidden

The images are valid structural proof, not accepted final art references:

- W000 remains dark and primitive-heavy, with a large foreground obstruction and placeholder ship/city presentation.
- ToxicCity is tightly framed against a building and dark.
- the ToxicCity Rill subtitle occupies roughly 16.1% of the viewport and visually dominates the lower frame.
- both world views remain below the intended cinematic quality bar.

These are recorded for the R2 scene-presentation/diegetic-panel work. None is being relabeled as polished or device-proven. No unresolved P0 blank-frame, mirrored-menu or full-screen collision was found in this contact sheet.

## R1.8 — UI spatial evidence

### Actual Home Hub

- 4 runtime text records;
- 0 findings;
- title and both actionable choices face the camera, fit the viewport and do not overlap.

### Actual W000

- 11 runtime text records;
- 0 blockers;
- 1 warning: `TEXT_RENDERER_MISSING` on `ObjectiveBoard/ObjectiveCanvas/ObjectiveText`.

### Actual ToxicCity

- 7 runtime text records;
- 0 blockers;
- the same ObjectiveBoard warning;
- the visible Rill subtitle is fully inside the viewport, but large as noted above.

### Classification of the ObjectiveBoard warning

The warning is a real coverage limitation, not proof that the text is absent. `TextMeshProUGUI` renders through `CanvasRenderer`, while the current camera-space audit obtains bounds only from `Renderer`. It detects and names the UGUI text but cannot project its bounds, so it correctly emits a warning instead of inventing a pass.

R1.8 is explicitly findings-first under the locked harness specification. This warning does not block R1.9, but no operator should claim that every UGUI label has full spatial proof until CanvasRenderer/RectTransform projection is added or the panel is independently accepted on-device.

### Controlled UI canary

The controlled fixture correctly emitted blockers for:

- text facing away;
- viewport clipping;
- major text overlap;
- multiple overlapping TMP/TextMesh combinations.

This demonstrates that the blocker paths actually fire rather than merely existing in source.

## Census and exposure sampling

Across settled Home Hub, W000 first arrival, ToxicCity and W000 return:

- active profile remained `GoldenSlice`;
- no census finding was emitted;
- forbidden DebugHUD, Photon/Net bootstrap, PvP progression, conquest mission, ecology injection, dev warp board, runtime material fixer, runtime input enabler, camera enforcer, Quarters camera and VR boot diagnostics remained absent/inactive as required;
- controlled stale-artifact fixtures correctly detected both an active DebugHUD artifact and a Photon runtime artifact;
- the second camera recorded after visual capture is the disabled test-owned `__RECOVERY_SNAPSHOT_CAMERA`, not a competing active player camera.

## Log and failure sampling

The successful run contains:

- no `NullReferenceException`;
- no assertion exception;
- no `TRAVEL_FAIL`;
- no `AUDIT_FAIL`;
- no recovery read/bilateral failure;
- no unhandled gameplay exception in the golden route.

Unity's licensing refresh warning and graphics capability enumeration are runner/environment noise and did not alter the test result.

## Promotion decision

R1.5–R1.8 evidence is internally consistent after classifying the two coarse diagnostics above. The raw artifacts substantiate the promoted claims:

- real boot and XRI selection;
- canonical persistent composition;
- exact travel/save round trip;
- settled standing clearance;
- real nonblank player-view captures;
- calibrated UI findings and working canaries;
- forbidden-owner/runtime-artifact exclusion.

**The recovery lane may proceed to R1.9.** This does not authorize a headset build. R1.9, R1.10, exact-SHA Golden Android evidence, canary proof for shader/build-hook gates, and final independent sampling remain mandatory.