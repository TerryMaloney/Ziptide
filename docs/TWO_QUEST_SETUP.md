# 🎮🎮 TWO-QUEST SETUP — current Photon test path

**Status date:** 2026-07-12  
**Goal:** install one identical ZIPTIDE APK on two Meta Quests, join the same Photon room, and verify cross-headset VR presence.

## Current truth — do not redo old setup

The one-time Photon setup is already committed on `terry-local-wip`:

- PUN2 is present under `Assets/Photon/**`;
- `PhotonServerSettings.asset` contains the Realtime App ID;
- `ZIPTIDE_PHOTON` is enabled for Android and Standalone;
- `Assets/ZiptideNet/PhotonPvpTransport.cs` and `NetBootstrap.cs` are active;
- A6 v1 online presence is implemented: remote head and hands render in the arena;
- development builds are pinned to Photon region `usw`, so both Oregon test headsets cannot accidentally create separate `ZIP-001` rooms in different regions.

**Do not import PUN2 again, do not rerun the Photon wizard, and do not replace the App ID.** Reimporting was the old guide and risks duplicate package/demo files.

PUN2 is in Photon maintenance/LTS mode, but Photon explicitly says existing PUN2 projects continue to run. ZIPTIDE should not migrate networking frameworks during a headset-test sprint.

## What tonight's multiplayer build actually supports

### Supported now — A6 v1

- both players connect to room `ZIP-001`;
- the Match Board shows `NET: in ZIP-001 (2/2)`;
- each player sees the other's helmet and two tracked gloves;
- head and hand poses update at 20 Hz;
- disconnects time out and remove the remote avatar;
- solo and bot modes remain available if Photon fails.

### Not implemented yet — A6.2

- network-authoritative shooting and hits;
- remote held-weapon display;
- synchronized health, scoring, walls, pickups, and match outcomes.

Tonight is a real two-headset **connection/presence test**, not yet a complete human-vs-human combat match.

## Fast path — both Quests connected by USB

After Architect finishes and pushes:

```powershell
cd C:\Ziptide
git checkout terry-local-wip
git pull --rebase origin terry-local-wip
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1
```

The script:

1. rejects the wrong branch;
2. verifies PUN2, the App ID, Android define, adapter, and deterministic region;
3. builds one APK through the canonical patch/audit/build pipeline;
4. installs that exact APK on every authorized Quest returned by `adb devices`;
5. launches both copies;
6. waits while you perform the in-VR test;
7. writes one Photon log per connected headset under `Ziptide\Builds\`;
8. reports PASS, INCOMPLETE, or FAILED from the network breadcrumbs.

## One USB cable — install sequentially

Build and install on the first connected Quest:

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -InstallOnly
```

Swap the cable to the second Quest, approve USB debugging, then reuse the APK without rebuilding:

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -SkipBuild -InstallOnly
```

You can then unplug both and perform the visual two-player test over Wi-Fi. Photon relays over the internet; both headsets do not need a direct peer-to-peer connection.

## In-headset test

On both headsets:

1. Launch **Library → Unknown Sources → ZIPTIDE**.
2. Enter the same arena. Use the headset-native development menu if needed.
3. Find the Match Board.
4. Press **GO ONLINE** on both boards.
5. Watch the line move through `connecting...` to `in ZIP-001 (2/2)`.
6. Stand several feet apart and move head, left hand, and right hand independently.
7. Confirm each player sees one remote helmet and two amber gloves with acceptable scale and latency.
8. Stop and record the exact board text if either player remains at `1/2` or disconnects.

Expected log sequence:

```text
ZIPTIDE: NET_STARTER_INSTALLED (Photon)
ZIPTIDE: LOBBY_ONLINE_START room=ZIP-001 ok=True
ZIPTIDE: NET_CONNECTING room=ZIP-001
ZIPTIDE: NET_MASTER_OK
ZIPTIDE: NET_ROOM_JOINED room=ZIP-001 players=1|2
ZIPTIDE: NET_PLAYER_JOINED players=2
ZIPTIDE: NET_PRESENCE remote=<id> joined
```

## Failure map

| Symptom | Meaning / next check |
|---|---|
| `offline (no netcode in build)` | Wrong or stale APK; `ZIPTIDE_PHOTON` was absent at build time. Run the new script's preflight and rebuild. |
| Both remain `1/2` | Confirm both installed the same new APK. The development region is now pinned to `usw`; a stale APK may still use automatic region selection. |
| `connecting...` never advances | Confirm each Quest has internet, then collect `NET_DISCONNECTED cause=...` from the generated logs. |
| One headset does not appear in `adb devices` | Put it on and approve the USB-debugging prompt; reconnect the cable and restart ADB. |
| `more than one device/emulator` | Use `tools/two_quest_test.ps1`; the old single-device installer invokes plain `adb` and is not the two-headset path. |
| Both reach `2/2`, no avatar | Capture logs and look for `NET_PRESENCE`; this is a gameplay presence issue, not Photon account setup. |
| Avatar works, weapons do not affect the other player | Expected until A6.2 is implemented. |

## Evidence to preserve

Keep these files after the test:

- `Ziptide\Builds\android_build.log`
- `Ziptide\Builds\quest_<serial>_photon.log`
- exact headset/model used for each serial;
- whether both boards reached `2/2`;
- avatar scale, latency and tracking notes;
- any `NET_DISCONNECTED cause=...` line.

Do not change Photon account settings, reimport PUN, or switch networking frameworks in response to a device failure. Diagnose from the saved logs first.
