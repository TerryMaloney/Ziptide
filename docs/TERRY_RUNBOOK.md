# TERRY RUNBOOK — the stuff only you can do (Unity menus + headset)

**What this is:** the AI operator can't run Unity or the headset. Everything that needs your hands lands
here, batched, so nothing stalls waiting on you and you can clear it in one sitting. **When the operator
says "queued a step in the runbook," it's here.** Do the pending block, commit what it generates, then run
the device pass.

> How the split works (full detail in `docs/ROLES.md`): the operator writes C# **patchers**; you run a
> `Ziptide → …` menu that **generates** the `.unity`/`.asset` deterministically; you **commit** it; CI
> audits it. You never hand-edit scenes either — the menu does it.

---

## 1. Pending Unity menu steps (run in the editor, then commit the results)
Do these in order after pulling. Each generates committable assets. *(This mirrors
`DEVICE_TEST_CHECKLIST.md` §0 — that doc has the full copy-paste block.)*

- [ ] PowerShell: `cd C:\Ziptide; git pull origin terry-local-wip`
- [ ] `Ziptide → Worlds → Build Toxic City`
- [ ] `Ziptide → Worlds → Build Toxic City Contract`
- [ ] `Ziptide → Worlds → Build PvP Arena`
- [ ] `Ziptide → Dev → Build Starter World (graybox)`  *(regenerates with the safety floor)*
- [ ] `Ziptide → Dev → Build Sandbox Test Lab`  *(only if you want the sandbox refreshed)*
- [ ] **Commit** the generated `.unity` / `.asset` files (PowerShell: `git add -A; git commit -m "..."; git push origin terry-local-wip`).
- [ ] Build + install: `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1`
  *(re-runs all patchers, so scene-side fixes apply automatically.)*
- [ ] If install fails `INSTALL_FAILED_UPDATE_INCOMPATIBLE`: `adb uninstall com.terrymaloney.ziptide`, re-run.
- [ ] Unity Console has **no red errors** (harmless warnings only: `frameH` unused, `AudioDirector._fadeTimer/_fading`).

> **No manual "Rebuild Dev World Manifest" step** — the build now rebuilds the manifest itself, **last**
> (after the D0→"D0 City (legacy)" rename), so the Y+B menu is always correct and there's no duplicate
> "Toxic City". (Clicking it by hand is harmless; the build re-does it right.)

*Watch logcat during testing:* `adb logcat | findstr "ZIPTIDE"` *(dropped `-s Unity` — too strict; it hid everything last time.)*

## 2. Pending headset pass
- [ ] Run the full on-device checklist → **`docs/DEVICE_TEST_CHECKLIST.md`** (9 areas: rig/gun, dev menu,
  Toxic City, economy `ECON_RESOLVE`, travel-gating, PvP, Starter World, spawn/fall-safety, perf).
- [ ] Send the operator the **❌s + feel notes** (ray length, drone speed, scanner size, hop distance, gun
  grip, city scale). Those drive the next round.

## 2b. NEW — the 11 story worlds (modularity sprint; they build themselves)
The build now **auto-authors and ships W002–W012** (no menu steps — they appear in the Y+B dev menu after
your next build/sideload). Quick smoke per world (a minute each, warp via Y+B):
- [ ] Spawn is on solid ground, you can walk the loop, and the **sky/mood is distinct** (W002 pitch-dark
  cistern → W003 bright mesas → W004 dead-screen dread → W005 rust dusk → W006 blinding salt → W007
  space station + HUGE gridded planet → W008 gold-lit archive → W009 amber swarm city → W010 storm coast →
  W011 dark resonance → W012 raw void + the Shell filling the sky).
- [ ] The kiosk offers the contract; the objective board updates; drones fight where expected (W002 easy /
  W005+W009 standard).
- [ ] **Story gating:** doors to W002 stay LOCKED until the ToxicCity bounty is done, then unlock in chain
  order (`TRAVEL_LOCKED` in logcat when locked — that's correct behavior now, not a bug).
- [ ] Completing W004's contract logs `WORLD_FLAGS_GRANTED` + `TRANSMISSION_CLARITY tier=1` (the first
  fragment!).
- [ ] Feel notes: which worlds' scale/colors/heights feel wrong — each is fixable by editing ITS layout
  asset fields (see `HOW_TO_CHANGE_ANYTHING.md`).

## 2c. NEW — M1 "The Story Speaks" (RILL + fragments + playback; no menu steps, all auto)
The next build carries the story layer. Smoke it while doing §2b:
- [ ] **RILL exists:** a small glowing orb hovers near your **left shoulder** in every world; entering a
  world shows a **"RILL: …" subtitle** low-center (`ZIPTIDE: RILL_LINE` in logcat). W001/ToxicCity =
  "Systems nominal. I think."
- [ ] **RILL reacts to story beats:** finishing W004's contract → the cargo question; W009 → the
  misidentify glitch line; W012 → "it's a cage."
- [ ] **W002 collect step:** the contract now has "Collect 3 mineral sample" — three small glowing
  shards on the gallery route; grab each (`ZIPTIDE: COLLECTED`). Grabbing them EARLY (before that step)
  must still count when the step arrives.
- [ ] **W004 fragment is physical:** a "?? recording" shard at the broadcast core — grabbing it logs
  `COLLECTED item=transmission_fragment flag=FRAGMENT_T1_FOUND` + `TRANSMISSION_CLARITY tier=1` and
  RILL comments.
- [ ] **Playback console:** next to where the fragment sat is a small terminal — point + select the
  screen → mostly-static text with a few legible words (`TRANSMISSION_PLAYBACK tier=1`). (It gets
  clearer in later chapters — tier 0 shows "signal too degraded" if you somehow play it pre-fragment.)
- [ ] Feel notes: RILL orb position/size, subtitle readability, line timing, shard visibility.

## 2d. NEW — M2 "The Job Is Real" (hands-on repair, hazards, visible mine; all auto)
- [ ] **W002 — the full gate loop:** accept the contract → descend → down the drones → grab the 3
  mineral samples → at the pump house, **repair the pump with your hands**: pull the rusted access
  panel off (it comes free — drop it), walk back toward the shaft for the glowing **pump valve**, carry
  it to the exposed socket (it snaps in), flip the **power switch** → lamp goes green
  (`MACHINE_STAGE`/`MACHINE_REPAIRED`) → bounty pays.
- [ ] **W002 extractor:** next to the pump, a small rig with a floating readout that ticks up — select
  the **hopper** → yield pays into your profile (`MINE_COLLECT`). Leave the world, come back later —
  it produced while you were gone (`ECON_RESOLVE`).
- [ ] **W003 wind:** the two exposed bridge crossings **shove you sideways** (tinted lanes) — lean into
  it. `(HAZARD kind=Wind enter)`
- [ ] **W005 spores / W010 flood:** standing in the tinted pockets **slows you** (flash ticks); wading
  the W010 tide flats drags hard — the bridges are the safe route. Slows must **fully clear** when you
  step out (no stuck-slow).
- [ ] Feel notes: wind strength, slow severity, repair-part snap distance, mine rate/cap.

## 2e. NEW — M3 "Living Worlds" (creatures; all auto)
- [ ] **W002 (dark gallery):** two greenish blobs that **GROW while you're not looking at them** and
  drift at you; **facing them shrinks them** back down (they recoil to the dark). `(CREATURE_DOWN)`
- [ ] **W005 (canopy):** skittering bug clusters orbit you and **pull tight just before darting in** —
  the gather is the tell. Taser stuns the whole cluster.
- [ ] **W009 (chitin wall):** shooting the **tether-swarm's bugs does nothing** — cut the **glowing
  cord node** strung between the clusters instead. Also: two crawlers that **ripple then drop-lunge**
  off the wall; and a stun makes a molter **shed a grey decoy** and skitter out the back (`HUSK_MOLT`)
  — hit the moving one.
- [ ] **W012 (the failing gate):** the tall sentinel by the gate core. If you did W010 first (Signal 2),
  it **turns its eye to watch you**; crowd it and the eye **ramps orange→red** (back off = it calms);
  stand your ground and it makes ONE arrest-stun, then disengages (`WARDEN mode=…`, `WARDEN_ARREST`).
- [ ] Universal: creatures never pass through walls; disables are non-lethal crumples; slows always clear.
- [ ] Feel notes: creature speeds/sizes, telegraph readability, Warden warning window.

## 2f. NEW — M4 "The Ship" S1+S2 (boardable + fly-out; all auto)
- [ ] **W002/W003/W004 + ToxicCity:** a berthed ship sits on a deck pad — walk to the hull's side,
  select **BOARD SHIP** → you're teleported up to the **cockpit deck** (`SHIP_BOARD`).
- [ ] The **helm** lists destination worlds as rows — locked ones dark-red **LOCKED** (same story
  gating as doors, `TRAVEL_LOCKED`), open ones teal.
- [ ] Select a destination → **the fly-out**: you're seated, star-streaks race past and stretch as the
  engines spool (~4.5s) → the world loads (`SHIP_DEPART dest=…`). **Comfort check: the CAMERA never
  moves — only the streaks. Any nausea = ❌ + note.**
- [ ] **DISEMBARK** (panel behind the seat) puts you back on the berth (`SHIP_DISEMBARK`).
- [ ] Doors still work (fallback until the ship is device-proven).
- [ ] **QUARTERS (new):** on the cockpit deck, the purple **QUARTERS** panel teleports you into a warm
  little cabin aft — three display bays with your ACTUAL taser + gravity gun turning on the plinths,
  plus a locker board. **THE FIRST SUPPLY DROP IS LIVE:** the WEAPON SKINS bay lists **Rustline** (free)
  immediately; finishing story worlds unlocks more (W002 → Tidebreak taser, W004 → Ember Coil gravity,
  the W012 capstone → **Voidglass**; first bounty → the Wake Guild livery + emblem). **Select a skin →
  the showcase weapon AND every gun you pick up afterward wears it** (`COSMETIC_EQUIPPED` /
  `COSMETIC_APPLIED`). Bays with nothing unlocked yet show "NO ITEMS AVAILABLE" — correct.
  **RETURN TO DECK** exits. `(QUARTERS_ENTER / QUARTERS_BROWSE)`
- [ ] Feel notes: fly-out length, streak density/speed, deck size, helm row readability, quarters
  room size/coziness.

## 2g. NEW — M7 Multiplayer: the SMART BOT + five arenas (all auto)
The next build carries the multiplayer program's first wave. Warp via Y+B:
- [ ] **The bot is a real opponent now** (original PvP Arena + all new arenas; `PVP_BOT_BRAIN` in
  logcat): it patrols a route when it hasn't seen you; **breaks line of sight and it HUNTS your last
  position** then searches; **shoot it and it ducks behind cover**, then peek-fires; it flanks if you
  hold still too long; at low HP it **retreats while still shooting**. Telegraph (yellow flash) is
  always there — the fight should feel fair, just smarter.
- [ ] **Five new arenas in the dev menu** — quick lap each: **The Cistern** (dark, glowing center
  hill, two breakable tunnel walls), **Chitinwall** (catwalk alleys — fight happens on two levels),
  **Mirror Flats** (long bright lanes, LOW cover — the VETERAN bot leads your strafe here, dodge
  sideways after it flashes), **Tidal Array** (island hopping; the center ford DRAGS you — bridges are
  the safe route), **The Shell Gate** (bridges under a huge Shell; NIGHTMARE bot — expect to lose).
- [ ] Each arena: spawn solid, distinct sky, taser+gravity+pistol on glowing pads, hammer by spawn,
  exit door 3m to your right at spawn.
- [ ] **Difficulty is yours to tune:** `Resources/Bots/{rookie,regular,veteran,nightmare}.asset` —
  sliders for aim error, reaction time, dodge chance, cover discipline. Feel notes on which tier
  feels "fun-hard" vs "unfair" drive the tuning.

## 2h. NEW — ART track: SKYSCAPES EVERYWHERE (all auto; Picasso's first wave)
The next build gives **every generated world + all five arenas a movie-grade sky** (one baked dome +
up to 3 celestial bodies; ~zero perf cost — ≤4 draw calls). Look UP in each world; this pass is about
whether the skies make you stop:
- [ ] **The canon progression reads** (this is story, not decoration): W005 — the banded giant is
  CLOSER than you remember from the city; W007 Sable Station — raw space, dense stars, the giant is
  HUGE and there's a **faint hexagonal grid** if you look for it; W009 Chitinwall — amber/violet
  aurora sky, the grid is now clearly banding the planet; **W012 — the full Shell wall + starfield.
  This is the "oh." moment of Chapter 2 — tell me if it lands.**
- [ ] **W003 Glass Shelf**: TWO moons + a faint geometric shimmer straight up at the zenith (the
  first Pattern seed — subtle by design; can you find it without knowing where to look?).
- [ ] **W002/W004/W011** (the underground/sealed worlds) stay oppressive-dark — no stars leaking in.
- [ ] **Arenas**: five distinct skies; **The Shell Gate arena now fights under a BLACK HOLE** with an
  accretion ring + the full grid.
- [ ] **72 FPS check with skies on** (the dome bakes once at world entry during the travel
  transition — flag any hitch on entering a world).
- [ ] **VR banding check**: the sky gradients are dithered — flag any visible color-stepping bands,
  especially in the dark worlds.
- [ ] ToxicCity itself still shows the OLD sky (its patcher has no theme seam yet — queued with the
  story lane; its canon vista is authored and waiting).

## 3. The two open judgment calls (yours)
- **"Can you run in Toxic City?"** — was it the input bug (now fixed) or actual walls/narrow streets? If
  still wall-blocked, say so and the streets get widened.
- **Feel/tuning** across the board — it's all dial-able data; send impressions.

---

## Reference — the full editor menu map (for when the operator asks for a specific bake)
**Generate committable assets:** `Worlds → Build Toxic City` · `… Build Toxic City Contract` ·
`… Build PvP Arena` · `Worlds → Generate World From Selected Layout` / `Generate All Layout Worlds`
*(NEW — the world factory: any `CityLayoutDefinition` asset with a `sceneName` becomes a full world;
the build also auto-regenerates these, so running the menu is only needed to preview in-editor)* ·
`Dev → Build Starter World (graybox)` · `Dev → Build Sandbox Test Lab` · `Dev → Rebuild Dev World Manifest`.
**Apply to the open scene:** `Apply Boot Scene Patcher` · `Apply D0/D1/D2 … To Current Scene` ·
`Apply Theme To Current Scene`.
**Debug:** `Audit → Run Audit (All Scenes)` · `Diagnostics → Dump Scene + Rig Config` · `Dev → Warp Window`.

*Nothing here is a code change — it's baking + testing. The operator keeps this list current; if a task in
`FABLE5_BACKLOG.md` is tagged 🔧UNITY or 🎮DEVICE, its step shows up here.*
