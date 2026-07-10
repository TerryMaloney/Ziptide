# TERRY RUNBOOK — the stuff only you can do (Unity menus + headset)

**What this is:** the AI operator can't run Unity or the headset. Everything that needs your hands lands
here, batched, so nothing stalls waiting on you and you can clear it in one sitting. **When the operator
says "queued a step in the runbook," it's here.** Do the pending block, commit what it generates, then run
the device pass.

> How the split works (full detail in `docs/ROLES.md`): the operator writes C# **patchers**; you run a
> `Ziptide → …` menu that **generates** the `.unity`/`.asset` deterministically; you **commit** it; CI
> audits it. You never hand-edit scenes either — the menu does it.

---

> 🎮 **TESTING TODAY? Start with `docs/TEST_DAY.md`** — install PowerShell + the consolidated
> project & testing checklists for the current build. This file remains the per-system detail.

> ⭐ **THIS IS THE CONSOLIDATED NEXT-COMPUTER-SESSION CHECKLIST.** Every model: queue your 🔧 Unity-menu
> and 🎮 headset items HERE (§1 = Unity-menu steps, §2 = headset feel passes), in the shared checkbox
> format — do NOT fork a second
> checklist. Terry clears this whole file in one sitting. (Last consolidated: 2026-07-09 — combat A3,
> flight v1.1, worlds.)

## 1. Pending Unity menu steps (run in the editor, then commit the results)
Do these in order after pulling. Each generates committable assets. *(This mirrors
`DEVICE_TEST_CHECKLIST.md` §0 — that doc has the full copy-paste block.)*

- [ ] PowerShell: `cd C:\Ziptide; git pull origin terry-local-wip`
- [ ] **NEW (depth pass 2, 2026-07-06) — W007's sky got a second moon + more stars/nebula in code, but
  `SkyVistaLibrary` is create-only, so the *existing* baked asset won't pick it up on its own:** delete
  `Assets/Ziptide/Content/Worlds/SkyVistas/W007_SableStation_Vista.asset` (+ its `.meta`) in the Project
  window, THEN run `Ziptide → Art → Author Sky Vistas (missing only)` — it'll regenerate that one file
  from the updated code. Commit the regenerated `.asset`. (W012 was left alone on purpose — it's already
  at max intensity by design; adding more would dilute the "the wall is the point" staging.)
- [ ] **NEW (melee pack, 2026-07-06) — two arenas gained melee weapon pads in code (Cistern:
  `breaker_blade` west flank · Chitinwall: `tide_pike` on the west catwalk), but `ArenaLayoutLibrary`
  is create-only like the vistas:** delete `Assets/Ziptide/Content/Arenas/Generated/Arena_Cistern_Arena.asset`
  and `Arena_Chitinwall_Arena.asset` (+ `.meta`s), then run `Ziptide → Worlds → Author Arena Layouts
  (missing only)` (or just build — it's build-hooked) to reseed them with the new pads, then rebuild
  those two arena scenes. The two melee weapon ASSETS themselves (`BreakerBlade`/`TidePike` under
  `Resources/Items`) need no action — `ArenaWeaponAuthor` creates missing ones automatically at build.
  The blade also joins the starter-weapon lineup in every generated story world on the next world
  rebuild — no manual step, just look for the flat cyan blade next to the three guns at spawn.
- [ ] `Ziptide → Worlds → Build Toxic City`
- [ ] `Ziptide → Worlds → Build Toxic City Contract`
- [ ] `Ziptide → Worlds → Build PvP Arena`
- [ ] `Ziptide → Dev → Build Starter World (graybox)`  *(regenerates with the safety floor)*
- [ ] `Ziptide → Dev → Build Sandbox Test Lab`  *(only if you want the sandbox refreshed)*
- [ ] **NEW (flight, 2026-07-09 — Reasonbox):** `Ziptide → Worlds → Build Space Lane (Flight Trial)` —
  generates `SpaceLane_Trial.unity` + its world pack/theme and adds it to Build Settings. Commit the
  generated scene + assets. After the next build, "Flight Trial" appears on every berthed ship's helm
  and in the Y+B dev menu. **Headset check:** board → TAKE THE HELM (you teleport to the seat, walking
  is suspended) → left stick = throttle **(pull back = reverse)**, right stick = pitch + snap-yaw
  flicks, **L3 or A = boost (works in reverse too, like sprint), X = barrel roll left / B = barrel
  roll right** (a quick self-completing 360° — the vignette pulses hard during it, watch
  `ZIPTIDE: FLIGHT_ROLL` in logcat) → fly the 5 orange rings (they turn green; `ZIPTIDE: FLIGHT_RING`)
  → DOCK to stand up, RETURN HOME to travel back. **Xbox-parity pass (v1.2):** left stick X now
  **strafes** (pure sideways slide, no rotation), and **holding** the right stick keeps snap-turning
  on a 0.4s cadence instead of turning once. Feel notes wanted: snap-yaw angle (30°) + repeat cadence
  (0.4s), strafe speed (30% of cruise), pitch speed, boost strength (1.8×–2.5×), barrel-roll speed
  (~0.86s — too fast/slow/nauseating?), reverse cap (40%), vignette strength, ring size.
- [ ] **NEW (traversal 1.4b, 2026-07-09 — traversal Fable):** climbing + world ziplines are in. No menu
  step (all auto at build: the Sandbox regenerates with a **ClimbTower + a zipline off its top**, and
  every generated world strings ONE zipline between its two farthest POIs). **Headset check (Sandbox,
  by F: Locomotion):** grip the stud-covered tower face and hand-over-hand up (`ZIPTIDE: CLIMB_GRIP` in
  logcat; stick-walk is suspended while gripping, resumes on release — `CLIMB_RELEASE`); at the top,
  grab the yellow zip handle and ride back to spawn (`ZIPLINE_RIDE_START/END`). In any story world,
  find the `__WorldZipline` between the two farthest POIs. Feel notes wanted: climb "weight" (1:1 hand
  mapping — too fast?), grip reach (1.2m), stud visibility, zip speed cap (8 m/s), whether letting go
  mid-climb feels okay WITHOUT a fling (v1 logs it, doesn't launch you).
  **1.4c/d addendum (same corner, same build):** the tower is now reachable THREE ways — a **lift**
  (amber deck, north face; stand on it, it dwells 4s then rises; step across at the top —
  `ZIPTIDE: LIFT_READY`) and a **jump pad** (green disc at x≈7; step on → ~1.2s arc onto the tower
  top — `JUMPPAD_LAUNCH/LAND`; stick is suspended mid-flight). Feel notes: lift speed (1.6 m/s),
  dwell time, pad arc height (apex +2m — comfortable or startling?), landing accuracy.
- [ ] **NEW (caverns 1.4e, 2026-07-09) — THE FIRST CAVE:** run `Ziptide → Dev → Build Cavern Test
  Lab` once (creates + saves `Cavern_TestLab.unity`, adds itself to Build Settings), commit the scene
  + its `_WorldPack.asset`. **Headset:** warp in via Y+B ("Cavern Test Lab") — you spawn in a dim
  crystal-lit chamber network: walk the rock bridges between chambers, climb the stud-faced shaft
  walls (deep shafts also have a lift beside them), ride the zipline from the top chamber back to the
  bottom. Feel notes: chamber scale, bridge width (2.4m — vertigo?), shaft climb height, whether the
  dim light + crystal tips read as "cave" or just "dark".
- [ ] **NEW (1.4g, 2026-07-09) — GRAPPLE + THE UNDERCROFT:** also run `Ziptide → Worlds → Build W011
  Undercroft (cave world)` once, commit scene + pack. **Headset:** (a) in the Sandbox corner, point
  the ray at the breathing **rose ring** high on the tower's south shoulder and GRIP — the reel pulls
  you up (`ZIPTIDE: GRAPPLE_FIRE/END`; from too far you get a dim miss-pulse, `GRAPPLE_MISS`);
  release mid-reel to bail. (b) Warp to "The Undercroft" — W011's deep cave layer: RILL speaks on
  entry, high chambers carry grapple rings, the return door travels home to W011. Feel notes: reel
  speed (7 m/s), ease-in, stop distance (1.1m short), whether pointing-to-fly feels great or cheap.
- [ ] **NEW (Tidefront B2, 2026-07-09) — THE WAR TABLE:** no menu step (the Sandbox regenerates with
  the holo table on the south wall, by D/E). **Headset:** walk up — 12 planet orbs in story order
  (cyan = yours, red = the Rival's, grey = neutral). Tap a cyan world → its card + SPIRE/FRIGATE
  build tiles (real costs; watch the FLUX/ALLOY ticker). Tap an adjacent red/grey world → LIVE ODDS →
  tap it AGAIN to strike → **(B3 changed this: the second tap now opens the STRIKE NOW / FLY THE
  MISSION offer — see the next item)** → the outcome STAMPS over the planet (MAJOR VICTORY gold …
  COUNTERSTRIKE red) and floats away. END TURN → watch the Rival move line by line (never a silent
  jump). First strike: RILL says "The Wardens will notice this." Feel notes: orb size/reach from
  standing height, AI turn pacing (0.8s/move), ticker readability.
- [ ] **NEW (Tidefront B3, 2026-07-09) — RISK MISSIONS ("the gulag"):** no menu step. **Headset,
  at the war table:** double-tap an adjacent target → two tiles pop: **STRIKE NOW** (base odds) /
  **FLY THE MISSION +10%** (underdog +15%). Fly it → THE ZIPTIDE carries you into that ACTUAL world:
  three orange **shield pylons** ring the spawn — shoot, tase, or slap each one down before the
  2:30 clock (floating board shows objectives + time; goes red under 0:30). Win or lose you're
  auto-returned to the table and the held battle resolves WITH your tilt (card shows "MISSION WON —
  tilt +2" and the outcome; `ZIPTIDE: CONQ_MISSION_RESOLVE`). Walking out a travel door instead =
  declined, base odds. **Defense side:** END TURN until the Rival strikes one of YOUR worlds — its
  turn PAUSES on LET IT RIDE / DEFEND. DEFEND drops you into your world with 5 red **scout drones**
  fanned overhead — pistol and taser both down them. First flown mission: RILL notes a pattern.
  Feel notes: pylon ring radius (12m), drone height/spread, 2:30 budget, board readability, and
  whether the auto-return (2.5s after the verdict) feels right or abrupt.
- [ ] **NEW (Tidefront save/fog, 2026-07-10):** the war now SURVIVES QUITTING — play a few turns,
  quit the app entirely, come back to the table: ticker says "CAMPAIGN RESUMED — TURN N" with your
  exact owners/fleets/stockpiles. Fog of war: worlds not bordering your territory are dim flat dots
  (no defense intel; card says UNSCOUTED) and light up as your border grows. A dark **NEW WAR** tile
  (right side, near END TURN) resets — it asks "tap again" before abandoning, and any other tap
  disarms it. Win/lose a campaign → the save clears itself and NEW WAR is the rematch. Feel notes:
  fog dimming amount (72%), whether resume-on-entry should announce louder, NEW WAR tile placement.
- [ ] **NEW (abilities sprint A4.5, 2026-07-09) — AUGMENTS:** no menu step (the six gems author at
  build; the Sandbox regenerates with an **augment rack** by A: Grab). **Headset:** select a gem with
  the ray to EQUIP (`ZIPTIDE: AUGMENT_EQUIP`; one active + one passive — re-selecting swaps). With an
  ACTIVE equipped, a small orb rides your right hip — its brightness is the cooldown; TOUCH-SELECT it
  to fire (`AUGMENT_FIRE`): Surge Dash = 4.5m comfort burst along your gaze · Bubble Guard = 2s
  shield sphere (bots' bolts bounce) · Overclock = 4s of halved weapon cooldowns. Passives are
  always-on: Magnet Palm pulls loose items from 3m · Sure Step halves slow-zone effects · Sixth
  Sense halves the wrist-locator cooldown. Feel notes: dash length/speed, orb position (right hip),
  bubble size, whether touch-to-fire beats a button.
- [ ] **NEW (combat A3, 2026-07-09 — Picasso):** the damage economy was unified onto ONE scale. No manual
  menu needed — **the build auto-migrates the 7 creature assets** (`CreatureVariantAuthor` →
  `CreatureStatRebaseline`, version-guarded/idempotent) from their old health (~8–60) to the new
  integer scale (`swarm_bug` 4 … `warden` 20). After the build, `git status` will show the 7
  `Resources/Enemies/*.asset` **modified** — that's the migration; **commit them.** (If you'd rather run
  it explicitly first: `Ziptide → Worlds → Rebaseline Creature Stats (unified scale)`.)
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

## 2g. NEW — W000 "The Drift In" (the game's opening; all auto)
- [ ] Warp to **Your Ship** (W000) via Y+B: you wake in a tight warm-dark **bunk bay** — RILL greets you
  ("...there you are. I have been awake for six minutes and I already have opinions.").
- [ ] The **Cast Off** contract at the kiosk: walk to the **helm** marker in the berth bay → grab your
  **guild manifest** by the bunk → repair the **gate coupler** (panel → coupler cell → power) →
  `TUTORIAL_COMPLETE` grants.
- [ ] Your ship is berthed RIGHT THERE — board it, hit QUARTERS if you like, then depart to Toxic City:
  casting off is the travel lesson.
- [ ] **Judgment call for you:** should W000 become the actual BOOT destination (replacing the Sandbox
  dev-bypass)? One-line BootLoader change in a locked zone — say the word after you play it.

## 2i. NEW — M7 A3: THE MODES ARE PLAYABLE (all auto) + your two-Quest homework
In any of the five arenas, next to your spawn there's now a **MATCH BOARD** (left side; the exit-door
row on the right now has a door to EVERY arena — that's your arena select).
- [ ] **Board basics**: ray-click a mode / difficulty / bot count tile (they gold-highlight), hit
  **START MATCH** — the round restarts with your picks. Logcat: `ZIPTIDE: LOBBY_START …`.
- [ ] **DEATHMATCH with 2–3 bots**: multiple bots hunt you at once; your kills credit YOU on the HUD
  ("You N - M Bots"). Watch `PVP_KILL killer=0` when you down one, `killer=1/2/3` when one tags you.
- [ ] **GUN GAME**: each kill racks your NEXT weapon at chest height in front of you (taser → pistol →
  gravity; the full 6-rung ladder arrives with A4's new guns). Finish the ladder = win.
- [ ] **KOTH**: a gold ring lights up — stand in it to bank hold-seconds (HUD shows "hold Xs/90s");
  bots now PUSH the hill, "CONTESTED" shows when you share it.
- [ ] **FRAGMENT**: grab the gold orb at mid (walk into it), run it back to your spawn ring to bank
  (3 banks wins). Dying drops it back to mid. Bots shadow the orb — and hunt YOU when you carry.
- [ ] **HORDE**: waves of bots + creatures (swarm bugs, tendrils, wardens), one more every wave,
  breather between waves. Tell me which wave you die on per difficulty — that's the tuning data.
- [ ] **THE NEW GUNS (A4)** — each arena grew one new pad, and pads now RESPAWN their weapon
  (~20s after you take one): **Static Net** (green lobber — arc a net, it opens a crackling slow
  field; try netting the bot mid-chase) · **Sonic Thumper** (amber mallet — SWING it hard near the
  bot or a breakable wall; one swing cracks the wall) · **Prism Beam** (magenta rifle — HOLD the
  trigger; a thin guide line grows to full range, then a heavy beam fires; release early to cancel).
  Gun Game now runs all six weapons. Feel notes per weapon = the tuning data.
- [ ] **🏠 HOMEWORK (do anytime, ~20 min, no code needed): `docs/TWO_QUEST_SETUP.md` steps 1–4** —
  Photon account + PUN2 import + App ID + the `Ziptide → Net → Enable Photon` menu click. That's
  YOUR half of two-headset PvP; the operator's half (A6) plugs straight into it.

## 2i. NEW — ART track: THE FIRST FORGED WEAPON (all auto; the LLM studio's proof)
The next build replaces the taser's primitive block with a **generated mesh** (the Asset Forge —
your "LLM studio" is live; manual: `docs/project_art_plan/FORGE_STUDIO_GUIDE.md`):
- [ ] Spawn/grab the **taser** anywhere (Sandbox is easiest): it's now a chunky salvage pistol —
  rust-red receiver, stubby gunmetal barrel with a **teal muzzle ring**, coil greebles on top, teal
  charge windows on the flanks, drill-style grip (`ZIPTIDE: FORGE_APPLIED` in logcat).
- [ ] **Grip/aim feel**: the gun should sit naturally in the hand (the +45° tilt is baked into the
  Forge grip socket). If aim points high/low, say by how much — it's one number in the recipe.
- [ ] Firing/holster/travel behavior unchanged (the swap is look-only; colliders untouched).
- [ ] Want a different look? Just tell any session: *"make the taser more X"* — that's the studio
  loop now. Same for new assets: drone bodies, the ship hull, props are next.

## 2j. NEW — QUALITY BAR P0+P1: "does it feel like a world now?" (all auto; THE gate for scaling)
Your last test drove this whole program. Next build/sideload, please check exactly these:
- [ ] **Menu:** Y+B menu → warp somewhere → Y+B again → **buttons still click** (was the "clickable
  once" bug; logcat shows `MENU_CLICK` per press and `MENU_UI ... raysRebound=` per open). Worlds now
  page **6 at a time** (PREV/NEXT).
- [ ] **Entry text:** RILL's world-entry line is smaller, lower, wrapped to a readable column, fades in.
- [ ] **Release feel:** guns pulse briefly when released and carry your hand's throw; first drop makes
  RILL explain the holster. 3 starter guns now (taser, gravity, pistol).
- [ ] **THE BIG ONE — W002 / W003 / W006:** warp in. You should arrive FACING a huge landmark on real
  terrain — a 250–320m landscape (canyon basin / stepped mesas / blinding flats) with the built
  areas sitting on graded pads, a cliff rim at the edge, distinct ground per world. Walk off the pads
  onto the land. **Tell the operator: does it feel like an alien world or still like a box?** Your
  answer gates whether we roll POIs/paths/dressing across all 12 (P1c-g) or re-tune first.
- [ ] Watch perf while looking at the terrain + vista (it's one static mesh + ~40 blocks — should be
  fine, but flag any judder).
- [ ] **GARDEN (P3):** follow the cairn trail to the HarvestGrove pocket (planter boxes + flora).
  Select a soil bed → it plants (`GARDEN_PLANT`); the plant **visibly grows** (dew bulb = 2 min);
  when it shimmers READY, select again → harvest pays (`GARDEN_HARVEST`, credits HUD moves). Leave
  the world mid-grow and come back — growth continued while you were away.
- [ ] **BUILDING (P3):** at the MachineSite pocket, the plinth shows a hologram + "BUILD: mineral
  extractor 40 CR". Select the base → your credits drop and a REAL rig rises (`SOCKET_BUILD`),
  producing like any mine. Leave and return — **your built machine is still there**.

## 2k. NEW — ARCHITECTURE V2: export the World Specs (one menu click, ~1 min)
The overhaul's keystone landed: every world can now be driven by ONE editable text file (a "spec").
Your click creates the starting spec for every existing world:
- [ ] In Unity: **`Ziptide → Worlds → Export All World Specs (JSON)`** → files appear in
  `docs/worldspecs/`.
- [ ] Commit + push them (`git add docs/worldspecs; git commit -m "world specs exported"; git push`).
- From then on: "make W005's world bigger, redder, with two more combat camps" = an LLM edits
  `docs/worldspecs/W005_OxidizedCanopy.spec.json`, the build applies it, and the validators/quality
  gates reject anything broken BEFORE it can reach your headset. That's the "request it and it
  happens" pipeline from your PDF.

## 2o. NEW — TEST DAY 2 re-check list (after the Feel & Clarity waves land; one row per fix)
Each row names its logcat tag — if the tag shows and the feel is right, check it off.
- [ ] Sprint: hold/click LEFT stick → clearly faster (`LOCO_STATE sprint=true`); speed feels Fortnite-ish
- [ ] Guns: every gun snaps to a consistent grip angle on grab; laser sight shows aim
- [ ] Laser guns are hand-sized (not miniature)
- [ ] Grey weapon: visible tracer every shot
- [ ] Matchboard: spawn on TOP of the floor (`ARENA_SPAWN_OK`), menu text doesn't overlap
- [ ] PvP HUD: small, low in view, doesn't block sight
- [ ] W003 Glass Shelf: spawn ground is clean (no white-noise shimmer)
- [ ] W005: when something hits you, you can SEE the source (`HAZARD_HIT src=`)
- [ ] Kiosk: a beacon/arrow leads you to it (`BEACON_TARGET`)
- [ ] RILL subtitles sit lower and fade out
- [ ] W000 ship: beaconed console by the stern → press PUNCH IT → star streaks → arrive ToxicCity (`FLIGHT_LAUNCH/DEPART`)
- [ ] Gardens: a green beacon marks the first garden plot in harvest-grove worlds (`GARDEN_SPAWNED`)

### Final-sprint controls (CONTROL_SCHEME.md — the full Fortnite set)
- [ ] Crouch: click RIGHT stick → view lowers, slower move (`LOCO_STATE crouch=true`)
- [ ] Slide: crouch WHILE sprinting → short speed burst then crouched (`LOCO_STATE slide=`)
- [ ] Auto-run: DOUBLE-click LEFT stick → runs where you look; stick/jump cancels (`LOCO_STATE autorun=`)
- [ ] Laser sight: every held gun shows a thin aim line to the first hit
- [ ] Quick-swap: with a gun held, tap B → swaps with the belt gun; empty hand + B = draw (`QUICK_SWAP`)
- [ ] Ping: EMPTY left hand, pull trigger at a spot → gold beacon drops for 20s (`PING_AT`)
- [ ] Match board: HOW IT WORKS panel beside it changes with the selected mode; kiosk carries its 3-step sign
- [ ] **CREATURES WALK now:** W005/W009 swarm bugs are six-legged amber chitin skitterers whose legs
  actually scuttle; W002 grazers are pale glowing bells with waving tentacles (`FORGE_CREATURE_APPLIED`
  in logcat). Feel notes: leg speed vs body speed, size, do the feet float above ground?
- [ ] **TWO HEADSETS (A6 v1 — full guide `docs/TWO_QUEST_SETUP.md` step 5):** both Quests → same arena →
  **GO ONLINE** on the match board → `NET:` reads `in ZIP-001 (2/2)` → you SEE each other as helmet+amber
  gloves tracking real head/hands (`NET_PRESENCE remote= joined`). Presence only this build — shooting
  each other is the next chunk (A6.2). Feel notes: avatar scale/readability, tracking lag, shared-room feel.
- [ ] **THE ZIPTIDE**: travel through ANY door/ship/warp → teal tide rises + orbits + contracts
  around you, controllers rumble in crescendo, white crest flash = the cut, receding tide on
  arrival + boom (`ZIPTIDE_GATE depart/arrive`). The destination's NAME rides above the ring,
  and the tide is TINTED toward that world's sky (needs the manifest: it regenerates on every
  build, or run `Ziptide → Dev → Rebuild Dev World Manifest` once). RILL speaks over the rise
  ("Brace. The tide has us." — varies per crossing, `RILL_LINE gate_*`). Door-started travels
  pour streaks OUT of the doorway; the crest now whites out the view so the load happens on
  white, and the arrival tide resolves out of it — the cut should feel like ONE moment.
  Verdict wanted: does it feel Stargate-grade EVERY time? Do the colors read as "where I'm
  going"? What would make it more?
Full program + envelope details: `docs/TEST_DAY_1_RESPONSE.md` + `docs/design/CONTROL_SCHEME.md`.

## 2l. NEW — git safety, one-time (~2 min; from your PDF's version-control chapter)
Four operators + you push to one branch — this makes scene-file collisions unable to corrupt anything:
- [ ] In PowerShell, in the repo (exact commands also in `docs/AI_WORKFLOW.md` §Smart Merge):
  `git config --global merge.unityyamlmerge.name "Unity SmartMerge"`
  `git config --global merge.unityyamlmerge.driver '"C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Data\Tools\UnityYAMLMerge.exe" merge -p "%O" "%B" "%A" "%A"'`
  *(single-quoted on purpose — the old backtick-escaped version fails to parse in Windows
  PowerShell 5.1 with "error: invalid pattern"; tested 2026-07-05.)*
- `.gitattributes` now also protects Terrain/NavMesh/Lighting pseudo-binaries and carries the LFS
  plan as comments (we deliberately DON'T enable LFS until the repo carries real audio/texture
  weight — the operators know the trigger).

## 2m. OPTIONAL — metavr MCP (would give your operators eyes on the headset)
Your PDF's Meta VR CLI is real and free: it exposes Quest device tools (install APK, pull logcat,
performance traces) to AI assistants over MCP. If you connect it to your Claude sessions
(`npx -y metavr mcp install claude-code` per Meta's docs, then link the headset), operators could
read `ZIPTIDE:` device logs and install builds WITHOUT you hand-copying logcat — the single biggest
speedup available for the device-feedback loop. Entirely optional; try it when you have 20 minutes.

## 2n. NEW — Q2d: THE FIRST REAL BUILDINGS (W002 GalleryB; one judgment call)
The next build gives W002's GalleryB district **actual generated buildings** (lots + door-on-street
grammar, `toxic_tenement` style) instead of facade slabs — the proof gate for the whole building
system:
- [ ] **One-time on your machine:** delete `Assets/Ziptide/Content/City/Generated/W002_DryCistern_Layout.asset`
  (it's create-only and yours predates the opt-in; the next build reseeds it with buildings on).
  CI builds already carry it.
- [ ] Warp to W002 → walk to GalleryB (east of the cistern mouth). **THE question: does it read as
  a PLACE — a street of buildings you could enter — or as boxes?** Doorways must be walkable
  (audit-guaranteed but confirm the feel), no doors blocked, no floating geometry.
- [ ] Feel notes → HANDOFF; a ❌ here re-prioritizes the building track before more styles are made.
  (Picasso's building-module kit lands on the same seam next — E5.1 in `FORGE_II_QUALITY_LEAP.md`.)

## 2p. NEW — COMBAT: melee grip + the unified damage scale (2026-07-09, Picasso; all auto after build)
Two things to *feel* on device; both are pure-data tunes if they're off (no rebuild needed to change the numbers).
- [ ] **Melee grip** — pick up the **Breaker Blade** and the **Tide Pike** (arena weapon pads, or the
  blade in the story starter rack). They used to be held at the *gun* aim angle; now the blade rides
  above the fist (+70°) and the pike sits flatter (+30°, tip-leads-the-thrust). Do they feel like a sword
  and a spear in the hand, not a pistol? If not, tune `ItemDefinition.gripLocalEuler` on
  `Resources/Items/BreakerBlade.asset` / `TidePike.asset` (X = pitch). Guns are unchanged — sanity-check
  one still aims right.
- [ ] **Creature time-to-kill** — the whole arsenal now does its own per-weapon damage to creatures on
  the SAME scale as PvP (taser was 10, now 2; net/thumper/prism used to all wrongly do 8). Creature
  health was re-baselined to match. Quick check: taser a **swarm_bug** (should die ~2 hits), a **warden**
  (~10 hits, a mini-boss). Do fights feel right, or too spongy / too fragile? Tune numbers in
  `CreatureBaselines.HealthFor` (one file, one line each). Watch `ZIPTIDE: CREATURE_DOWN`.
- [ ] **(not Unity) Reconnect the GitHub connector** on claude.ai (Settings → Connectors → GitHub →
  Reconnect, authorize `terrymaloney/ziptide`) so the operators can see CI status again. Pushing already
  works without it; this is just so we can watch the runs go green.

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
