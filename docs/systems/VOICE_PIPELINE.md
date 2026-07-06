# Voice Pipeline — subtitle-now, real VO later (all characters)

## 1. Purpose
Every spoken line in Ziptide ships as text FIRST and audio SECOND — nobody is cast yet, and that's
fine, because the code was built to swap subtitle-only for subtitle+clip without touching a single
trigger or piece of delivery logic. This doc is the one place a future operator (any model) reads
before recording or wiring real VO, for RILL, for Cal, and for the secondary voices (Mara, Sable,
Aegis-Nine/Nine).

## 2. How it works today (text-only stub)
- Every line lives in one data class, `RillLine` (`Content/Runtime/Story/RillLineLibrary.cs`):
  `id`, `trigger` (`WorldEnter` / `FlagSet` / `GateDeparture`), `key`, `text`, `once`, **`speaker`**
  (a plain string — default `"RILL"`), and **`voClip`** (an `AudioClip`, always `null` today).
- `RillLineAuthor.cs` (editor-only, `Editor/Patching/`) is the **source of truth**: every line in the
  game — RILL's and Cal's — is authored there in code, not hand-edited on the asset. Running
  `Ziptide → Story → Author RILL Lines` (or a build) regenerates `Resources/Story/RillLines.asset`
  from that code.
- `RillCompanion.cs` (`Gameplay/Runtime/Story/`) is the only thing that ever plays a line: it watches
  scene loads + profile flags, queues matches, and for each one calls `line.FormatSubtitle()` (→
  `"SPEAKER: text"`) for the subtitle, **and, only if `line.voClip != null`, plays it**:
  ```csharp
  if (line.voClip != null)
      AudioSource.PlayClipAtPoint(line.voClip, _orb.transform.position, 0.9f);
  ```
  That line has been sitting there since RILL first shipped (M1) — it is not new, and it is not a
  TODO. **The audio-playback path already exists and already works.** The only reason nothing plays
  is that every `voClip` field is empty. Wiring a clip in is therefore never a code change — it's an
  asset assignment.

## 3. Cal joined the pipeline (2026-07-06, soul pass 2)
Before this pass, `RillLine`'s subtitle was hardcoded to `"RILL: " + text`. Cal (the player
character) had never spoken a line anywhere in the codebase. Two changes made Cal a first-class
speaker with zero new systems:
1. `speaker` is a plain string, not a two-value enum — any name works, so adding Cal cost one field.
2. `RillLineAuthor.cs` now also authors Cal's lines via `CalEnter`/`CalFlag`/`CalGate` (same
   trigger/key mechanics as RILL's `Enter`/`Flag`/`Gate`, just `speaker = "CAL"`), appended in a
   clearly-marked section at the bottom of the file, banter-paired with the RILL beat each answers.
   List order = playback order for a shared trigger+key, so RILL's line always plays first and
   Cal's answers it — read `RillLineAuthor.cs`'s Cal section for the full current set (~24 lines,
   spanning Ch.0 through the four endings).

**This means the secondary voices (Mara, Sable, Aegis-Nine) could join the SAME pipeline the same
way** — `CalFlag`-style helpers with `speaker = "MARA"` etc., keyed to the flags that already exist
(`PLAYER_HELPED_MARA`, `C4_SABLE_ALLIED`/`OPPOSED`, `C6_WARDEN_ALLY`/`ENEMY`, ...) — reusing the exact
quoted lines already written in `docs/storyboard/CHAPTER_*.md`'s Mara/Sable/Nine beats. **Not done
in this pass** (scope was Cal); it's the natural next content batch for whoever picks this up, and
needs no new engineering — copy the `CalFlag` pattern, one call per already-written quote.

## 4. How to wire in a real VO clip (the exact steps)
1. Get the actor's read as a `.wav` (or whatever Unity import format the project already uses for
   other SFX — match `AudioDirector`'s clips for import settings: mono, VR-appropriate compression).
2. Drop it under `Assets/Ziptide/Resources/Story/VO/<character>/<line-id>.wav` — e.g.
   `Resources/Story/VO/Rill/beat01_boot.wav`, matching the `id` string in `RillLineAuthor.cs` so a
   human (or a script) can find the right clip for the right line by name alone.
3. In `RillLineAuthor.cs`, the authoring helpers (`Enter`/`Flag`/`Gate`/`CalEnter`/`CalFlag`/
   `CalGate`) don't currently take a clip argument — add one **overload**, don't change the existing
   signatures (every other call site should keep compiling untouched):
   ```csharp
   void Flag(string id, string flag, string text, AudioClip clip) =>
       L.Add(new RillLine { id = id, trigger = RillTrigger.FlagSet, key = flag, text = text, voClip = clip });
   ```
   Load the clip with `Resources.Load<AudioClip>("Story/VO/Rill/" + id)` right where each line is
   built, or batch-resolve them in a small helper — either is fine, this file regenerates the asset
   every build so there's no migration step.
4. Re-run `Ziptide → Story → Author RILL Lines` (or just build) — the `.asset` regenerates with the
   clip attached. **Nothing else changes.** `RillCompanion` already checks `voClip != null` and plays
   it at the orb's position; the subtitle keeps showing (VO + captions together, not VO instead of
   captions — Quest accessibility default, keep it that way).
5. Sanity-check timing: `LineSeconds` (5s) + the typewriter reveal time is what currently holds a
   subtitle on screen. Once a clip is attached, that duration should reflect `clip.length` instead
   for that line (short "Brace." lines need less than a 20-word confession) — this is the one small
   behavior change worth making that isn't a pure asset-drop, and it's isolated to `DriveSubtitle()`.
   Not needed until the first real clip lands; noted here so nobody has to rediscover it.

## 5. Casting / tone notes (read `STORY_BIBLE.md` before booking anyone)
Each character has an explicit voice already written down — don't reinvent it, cast to it:
- **RILL** — `STORY_BIBLE.md` §3 (character) + §3b (the Cal relationship, "the log" running gag) +
  the register arc in `THE_TRANSMISSION.md` §3 (surface topic vs. the real thing, tightening over
  the game). Dormant = terse/functional. Stirring = curious, asks questions. Remembering = shaken at
  the edges. Unsealing = full vocabulary, dry humor intact. Integrated = an equal, warmer.
- **Cal** — `STORY_BIBLE.md` §3's "talks to broken machinery like it's being deliberately stubborn"
  line is the whole brief: dry, competence-under-pressure, never precious, breaks into a real
  question a few times a chapter and always deflects back into a joke right after (never the
  reverse — see the Cal section header comment in `RillLineAuthor.cs` for the rule stated plainly).
- **Mara, Sable, Aegis-Nine/Nine** — `STORY_BIBLE.md` §9 "no faction mouthpieces": each one must
  read as contradicting themselves at least once (Mara believes the mission AND fears its cost;
  Sable is right about the door AND wrong about the price; the Warden enforces the cage AND
  recognizes RILL) — cast for that tension, not a single flat attitude. Their full quoted arcs are
  in `docs/storyboard/CHAPTER_2/3/5/6/7.md` at the world beats listed in `STORY_BIBLE.md` §4/§7.

## 6. Status
Designed and load-bearing for RILL; Cal joined 2026-07-06 (this pass); secondary voices are written
(as story-doc quotes) but not yet wired into `RillLineAuthor.cs`. **100% subtitle-only today, by
choice, not by blocker** — the moment clips exist, §4 above is the entire integration job.

## 7. Open questions (Terry's call, not blocking anything)
- Casting: internal TTS pass first (fast, cheap, placeholder-quality) vs. holding out for real
  actors from the start? Either fits the pipeline unchanged — it only cares about `AudioClip`, not
  where the clip came from.
- Should Mara/Sable/Nine's lines get wired into `RillLineAuthor.cs` now (mechanism proven, ~15
  lines of code) as a fast-follow, or wait until M5 world authoring reaches those chapters anyway?
- Mixing: `RillCompanion` currently plays VO via `AudioSource.PlayClipAtPoint` at a fixed 0.9 volume,
  no ducking against `AudioDirector`'s music bed — worth a real mix pass once any clip exists to A/B
  against.
