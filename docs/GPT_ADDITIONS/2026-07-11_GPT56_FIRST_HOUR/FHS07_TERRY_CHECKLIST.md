# FH-S07 TERRY CHECKLIST — W000 HOME / COMFORT / HELM

**Code status:** Unity CI green at `e51dce54b984184139812c89ac344c86f2b6f39a`, run `29165306485`  
**Scene status:** not yet authored into W000  
**Device status:** pending

## 1. Pull the green branch

```powershell
git checkout terry-local-wip
git pull --rebase origin terry-local-wip
```

## 2. Author the W000 surfaces once

In Unity:

`Ziptide → First Hour → Author W000 Surfaces`

Expected Console lines:

- `ZIPTIDE: FIRST_HOUR_SURFACES_AUTHORED scene=W000_DriftIn castoff=<bool>`
- `ZIPTIDE: FIRST_HOUR_SURFACES_SAVED scene=W000_DriftIn`

The command writes only:

`Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity`

Stable markers expected exactly once:

- `__FIRST_HOUR_COMFORT_CONSOLE`
- `__FIRST_HOUR_BUNK_OBJECT`
- `__FIRST_HOUR_FIRST_HELM`

Rerunning the command must not duplicate them.

## 3. Commit the generated scene

```powershell
git add Ziptide/Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity
git commit -m "build: author W000 first-hour surfaces"
git push origin terry-local-wip
```

## 4. Build and install

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
```

## 5. Headset acceptance

### Cold boot

- With no valid save: **New Game** and **Settings** appear; Continue does not.
- With a valid save: **Continue** appears.
- Settings opens Cozy / Standard / Bold and does not travel.
- New Game travels once and does not silently delete device comfort settings.

### Comfort

- Standard is selected by default.
- Cozy: 45° snap, strongest vignette, effectively no slide, slower zipline cap.
- Standard: 30° snap, medium vignette, normal slide and zipline cap.
- Bold: smooth 120° turning, light vignette, normal slide and zipline cap.
- No preset causes a rig jump, camera move, duplicated vignette, or stuck locomotion.

### W000

- The comfort console is the first obvious interaction.
- The **BUNK KEEPSAKE** is visible and grabbable.
- It logs `ZIPTIDE: FIRST_HOUR_BUNK_GRAB id=bunk_keepsake` only once.
- The first helm shows only **W001 TOXIC CITY**.
- Selecting it logs `ZIPTIDE: FIRST_HELM_SELECTED dest=W001_ToxicCity` but does not load a scene immediately.
- Existing **PUNCH IT** still owns launch.
- The gate-coupler repair still blocks PUNCH IT until repaired.
- Departure still uses the existing star-streak launch and TravelCoordinator path.

## 6. Report back

Send:

- any missing/duplicated marker;
- whether Continue visibility is correct;
- which comfort preset feels best/worst;
- any nausea or rig movement;
- whether the bunk object and helm are obvious;
- whether PUNCH IT and the repair gate still behave normally;
- relevant `ZIPTIDE:` log lines for failures.