# THE TITLE MENU — the first thirty seconds, made worthy of the game
### From "cube board in a void" to a place: the berth before dawn

**Status:** 🔵 RESEARCH + PLANNED — no code authorized (recovery freeze). Terry commissioned
2026-07-18. **Hard constraint up front:** today's Home Hub is a **recovery Golden surface**
(PlayMode boot tests, BOARD_PROBE diagnostics, the tested cold-boot flow). Everything below
dresses AROUND that proven skeleton; nothing changes its flow semantics, and the golden tests
stay green through every envelope.

---

## §1 — What exists today (audited in source — the honest picture)

**The good news — the wiring is genuinely solid:**
- `BootLoader` (in `_Boot`) engages the boot-hold (rig pinned, fall net disarmed), builds
  `HomeHubRuntime`, and hands it exactly two things: the target scene and a travel callback.
  The hub owns **no** save file, scene loader, input map, or menu framework — pure presentation
  over delegated systems. `HomeHubFlowState` (pure, tested) guarantees travel-exactly-once.
- Choices: NEW GAME → `StartNewProfile()`; CONTINUE (shown only if a save exists) → `Load()`;
  SETTINGS → the comfort console child. Events (`BootPresentationReady`, `ChoiceSelected`…) are
  the test seams. Logs: `HOME_HUB_READY/CHOICE/…`. This skeleton is KEEPERS.

**The bad news — the presentation is a placeholder:**
- The "menu" is runtime-built primitive cubes: a dark 2.4 m board, three colored cube tiles,
  TextMesh labels, floating 2.2 m in front of the camera — **in a black void**. `_Boot` has no
  sky, no ground, no world.
- **Total silence.** No title music (AudioDirector's profiles are per-world; `_Boot` has none),
  no hover/select sounds, no transition audio. The first thing a player hears is nothing.
- No title identity: "ZIPTIDE" is a TextMesh label on a cube, sharing a line with
  "CHOOSE YOUR START."
- The first interaction of the entire game (point-and-select a tile) gets no affordance help,
  no feedback beyond the click handler.

## §2 — Research: what great title menus (especially VR) do

1. **The menu is a PLACE, not a screen.** VR's consensus best practice: the player is already
   standing IN the game — Half-Life: Alyx opens on a balcony overlooking the city you're about
   to save; Beat Saber's menu is an environment; Moss seats you in its theater. A menu floating
   in blackness is flat-game thinking imported into a medium that punishes it.
2. **Diegetic beats floating.** Physical, in-world interfaces are the VR standard — and OUR
   whole UI philosophy already agrees (boards, tiles, ledger, bunks). The title menu should be
   the FIRST demonstration of the game's interface language, not an exception to it.
3. **The menu teaches the first verb.** Point-hover-select on a menu tile is the player's first
   interaction ever. Hover states + sound + a gentle glow make the menu a zero-risk tutorial
   for the interaction model the whole game uses.
4. **Time-to-play is sacred** (and store-cert measured): the place must be cheap to present
   and CONTINUE must be one gesture away. Returning players (kids!) should be in-game in
   seconds; wonder must never cost speed.
5. **Title music is the game's identity in eight bars.** The great title themes ARE the game
   (you can hum the franchise). It plays here and nowhere else at full attention — the one
   place in the game where music leads and the world listens. Cheap leverage: the return-home
   stem (CP-8) and the title theme share a motif, so coming home always faintly rhymes with
   the first time you pressed NEW GAME.
6. **Ambient motion makes it alive; restraint keeps it comfortable.** Slow sky, water, drifting
   particulate — motion rates from the Prospect rubric, nothing fast, nothing near the face.

## §3 — The design: THE BERTH BEFORE DAWN

You boot standing on the dock at the ship's berth, pre-dawn. The world is quiet and enormous:

- **The sky:** a dedicated menu `SkyVistaDefinition` (machinery exists, works in any scene) —
  the darkest, calmest vista in the library: pre-dawn gradient, the banded giant low and huge,
  two motion rates of drift. The light script derives the scene's lighting from it (F3.1 —
  free by design).
- **The water:** one calm `ZiptideWater` strip below the dock edge (machinery exists), slow
  bob — the tide, asleep. The single strongest "this place is alive" signal we own.
- **The ship:** its dark silhouette moored beside you, one practical lantern lit (F3.1b, warm
  amber vs. the cool sky — one of the two hero lights). Home, before you've earned it.
- **THE TITLE:** "ZIPTIDE" as big physical letterforms (F3.7 signage letterform machinery)
  standing on the far side of the water — crest-cyan emissive, doubled faintly in the water's
  reflection-free surface as a baked glow decal. The name is IN the world, monumental, not a
  label on a cube.
- **The board:** the existing hub board re-skinned as the dock's departure board (same tiles,
  same positions, same flow — the Golden surface untouched semantically): weathered panel
  material, tile hover = soft emissive lift + `ui_tick`, select = `ui_confirm`. NEW GAME /
  CONTINUE / SETTINGS wording unchanged (flow tests key on behavior, not looks).
- **RILL:** her orb sits DORMANT on the board's corner, barely glowing. Choosing NEW GAME or
  CONTINUE wakes her — the glow-up IS the transition ceremony, and the first story beat happens
  in the menu without a single word spent.
- **Sound:** the title theme (one `AudioProfile` for `_Boot` — the identity theme, low pad
  entering with the fade-in, motif fully in by ~8 s); water lap + distant berth ambience under
  it; ui family SFX on the tiles; on choice — theme resolves to one held note, the tide-surge
  stinger rises, RILL wakes, travel fires. From black void to a cold open worth store-page
  screenshots — for the cost of systems we already built.
- **Family profiles integration (later):** when `FAMILY_PROFILES.md` lands, CONTINUE becomes
  the walk-to-your-bunk moment (or the board shows bunk tags as tiles — decided in that
  envelope); this design leaves the board's right side clear for it.

## §4 — Wiring plan (how it's built without touching what's proven)

Everything is runtime-ensured in `_Boot` alongside the existing hub build — **no scene YAML, no
travel-contract change, no new flow states**:

- A `MenuBerthEnsure` (BootLoader sibling call, after boot-hold, before hub Configure): builds
  vista rig + water strip + dock floor + ship silhouette + letterforms + practicals from
  existing authors' runtime paths. Idempotent; torn down by the existing travel flow (content
  is `_Boot`-local objects destroyed on hub completion — explicit cleanup list, tested).
- Budgets: the menu is audited like a world (PerfBudget counts it — it must hold 72 Hz on the
  slowest device WITH time-to-interactive inside cert bounds; the vista/water/letterforms are
  all baked/cheap by construction). Boot-speed law: `MenuBerthEnsure` must complete inside the
  frame budget of the existing boot sequence or defer decoration (board first, world fades in
  around it — presentation-ready never waits on prettiness).
- Audio: `AudioDirector` gains a boot-profile slot (data, not code-path change); SFX ids from
  the SFX Forge ui family; the stinger is one one-shot. Title theme asset: sourced per SFX
  Forge §4 ladder (synthesized/licensed-with-CREDITS; composed identity theme is a CP-8 task —
  the menu takes whatever CP-8 crowns, placeholder pad until then).
- The golden PlayMode boot test's screenshot becomes the BEAUTY gate: the boot capture stops
  being "cubes in blackness" and starts being the game's poster — reviewed like any world
  contact sheet.

## §5 — Envelopes (post-freeze; art lane + one audio-data claim)

| Env | What | Acceptance |
|---|---|---|
| TM-1 | boot audio: title-theme profile slot + ui hover/select SFX + choice stinger | golden flow tests green; device listen |
| TM-2 | `MenuBerthEnsure`: vista + water + dock + light script + cleanup-on-travel | boot screenshot transformed; 72 Hz; time-to-interactive unchanged (measured) |
| TM-3 | title letterforms + board re-skin + hover feedback (semantics untouched) | BOARD_PROBE + flow tests green; readable in capture |
| TM-4 | the choice ceremony: RILL wake + theme resolve + surge → travel | device verdict — "the moment feels like a cold open" |
| TM-5 | profiles-era CONTINUE (bunk integration) | designed inside FAMILY_PROFILES envelope FP-3 |

Order TM-1 first deliberately: sound is the cheapest, biggest upgrade to the existing menu even
before any dressing exists.

## §6 — Do-nots

- Do not alter `HomeHubFlowState`, choice semantics, tile wording, or the travel callback path —
  the Golden surface's behavior is frozen; only its CLOTHES change.
- No menu-only systems (no bespoke menu shaders/frameworks — every element is an existing
  system's output; that's the point AND the proof the systems are good).
- No skippable-intro debt: there is no logo reel, no press-any-key limbo — boot lands you IN
  the place with the board ready (cert's time-to-interactive is the law).
- No loud audio cold-open (family living rooms; theme enters low), no motion near the face,
  comfort laws apply fully — this is many players' first-ever VR minute.
- The menu never becomes a travel destination or a world — it is `_Boot` decoration with a
  strict teardown, and `_Boot`'s contracts (never unloaded, never traveled-to) stand.
