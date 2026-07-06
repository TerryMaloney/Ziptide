# 🎮🎮 TWO-QUEST SETUP — getting two headsets into the same match (Terry's guide)

**Goal:** you + a friend, two Meta Quests, one Ziptide PvP arena. This page is YOUR half (accounts,
imports, one menu click, sideloads) — it takes ~20 minutes and can be done TODAY, independent of the
code. The operator's half (streaming the other player's avatar + hits over the wire — **A6** on
`SPRINT_MULTIPLAYER.md`) plugs into what you set up here with no re-setup.

**What already exists in the repo (don't redo it):** the network seam (`IPvpTransport` + message
contract), the transport registry (`PvpNetHub` — solo play runs on a loopback), the Photon adapter +
room-code launcher (`Assets/ZiptideNet/PhotonPvpTransport.cs` — dormant until step 4), and the
`Ziptide → Net → Enable Photon` menu. CI compiles all of it with Photon absent, so nothing here can
break the build until you flip it on.

---

## Step 1 — Photon account + App ID (5 min, free, one-time)
1. Go to **photonengine.com** → create a free account (the free tier = 20 concurrent users — years of
   headroom for us).
2. Dashboard → **Create a new app** → type **"Photon PUN"** → name it `Ziptide` → Create.
3. Copy the **App ID** (the long hex string on the app card). You'll paste it in step 3.

## Step 2 — Import PUN2 into the Unity project (5 min, one-time)
1. Open the Ziptide project in Unity 2022.3.62f3.
2. **Window → Asset Store** (or the web Asset Store) → get **"PUN 2 - FREE"** by Photon Engine →
   **Window → Package Manager → My Assets → PUN 2 - FREE → Download → Import**. Import everything
   EXCEPT the `PhotonUnityNetworking/Demos` folder (uncheck it — demo bloat).
3. Wait for the compile. The project must still be error-free (PUN2 lives in its own folders;
   nothing references it yet).

## Step 3 — App ID into the wizard (1 min)
- The **PUN Wizard** window pops up after import (or `Window → Photon Unity Networking → PUN Wizard`).
- Paste the App ID → **Setup Project**. This writes `PhotonServerSettings.asset`.
- ⚠ The App ID is not a secret worth much (it only routes to OUR relay), but if you'd rather keep it
  out of the public repo, tell the operator and we'll gitignore the settings asset + document a
  local-setup step instead. Default: commit it (simplest for a hobby project).

## Step 4 — Flip the switch (10 sec)
- **Ziptide → Net → Enable Photon (ZIPTIDE_PHOTON)**. This adds the `ZIPTIDE_PHOTON` scripting define
  (Android + PC) so the adapter in `Assets/ZiptideNet` compiles. If it complains "Photon not found",
  step 2 didn't finish.
- Project must compile clean. If it doesn't, **stop here and paste the errors to the operator** —
  the adapter was written blind against the PUN2 API and an editor compile pass on first activation
  was always the plan.
- **Commit + push everything** (PUN2 folders, PhotonServerSettings, the define change) on
  `terry-local-wip` so the operator and CI see the same world you do.
  ⚠ CI note: the cloud build does NOT get the define flipped by your local PlayerSettings alone if
  you skip committing `ProjectSettings/ProjectSettings.asset` — commit it too (the menu edits it).

## Step 5 — The first two-headset smoke (A6 v1 IS LIVE — 2026-07-06)
Steps 1–4 done + the build carries A6 v1 (online **presence**: you SEE each other's head + hands
moving in the same arena; combat sync is the next chunk, A6.2). To smoke it:
1. Sideload the same APK to BOTH Quests (`tools/dev_build_install.ps1` per headset — plug one in at
   a time — or drag the CI `ziptide-apk` artifact into SideQuest twice).
2. Both headsets on the SAME Wi-Fi is nice but NOT required — Photon relays over the internet.
3. Both players: travel into the **same arena** → on the match board press **GO ONLINE** (bottom-right,
   next to START). The tile turns amber and the **NET:** line reads `connecting…` → `in ZIP-001 (2/2)`
   once BOTH are in.
4. You should now see the other player as a **helmet + glowing amber gloves** that track their real
   head/hands (you're teal to them, they're amber to you). Move around — it should feel like sharing
   the room.
5. Logcat breadcrumbs (`adb logcat | findstr ZIPTIDE`): `NET_STARTER_INSTALLED` (at boot) →
   `LOBBY_ONLINE_START` → `NET_CONNECTING` → `NET_MASTER_OK` → `NET_ROOM_JOINED` →
   `NET_PLAYER_JOINED players=2` → `NET_PRESENCE remote=1 joined`. `NET_DISCONNECTED cause=...` is the
   failure breadcrumb; solo/bot play auto-falls back to loopback, so a net failure never bricks the arena.
6. **Feel notes for the operator:** avatar readability/scale, tracking lag, does the room feel shared?
   Then the next build adds shooting-each-other (A6.2). If the NET line says
   `offline (no netcode in build)`, the APK was built without the `ZIPTIDE_PHOTON` define — confirm
   `ProjectSettings.asset` was committed with the define on and rebuild.

## What you can do RIGHT NOW vs what waits on A6
| Now (this page, steps 1–4) | After A6 lands |
|---|---|
| Photon account, App ID, PUN2 import, wizard, define flip, commit | the actual cross-headset match |
| Result: project compiles WITH the network stack live | Result: two Quests, one arena, real PvP |

Doing steps 1–4 early is genuinely useful: it de-risks the only part the operator can't do from the
cloud (Asset Store import + wizard) and lets A6 ship as pure code.

---
*Written at the A3-scene sprint close (2026-07-02) per Terry's directive. Questions → HANDOFF.*
