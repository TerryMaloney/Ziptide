# RILL CAPTION RESEARCH — how VR games do spoken text well, and the v2 spec for ours

**Status:** 🔵 RESEARCH + PLANNED SPEC — no code authorized (recovery freeze; Terry commissioned
the research 2026-07-17). Implementation is one small envelope when the freeze lifts; decisions
marked ⚖ are Terry's.
**Scope:** RILL's subtitle/caption presentation only — placement, motion, type, color, plate,
timing. Line *content/writing* is the story lane; the diegetic UI language overall is FORGE IV
CP-9. This doc is the bridge: captions are the one piece of UI the player reads hundreds of times.

---

## 1. Where ours is today (audited from `RillCompanion.cs` / `SubtitleText.cs`)

What v1 already gets right:
- **In the cinema zone, not mid-gaze** — low-center placement, deliberately below the action.
- **Wrapped at 38 chars** (`SubtitleText.Wrap`) — inside the broadcast-standard 37–40 window.
- **Read-time-aware duration** (5 s + length/34 cps) and a fade-in, not a pop.
- **A speaker body** — the orb leans in while speaking, so the voice has a source.

What the research says to fix:
1. **Hard head-lock** — `DriveOrb` sets the text transform to the exact camera pose every frame.
   Hard-locked text jitters with every micro head-motion and reads as "stuck to my face."
   Research-preferred: head-anchored **with lag** (smoothed follow).
2. **~24° below gaze** — position is `forward·1.3 − up·0.58` ⇒ atan(0.58/1.3) ≈ 24° down. The BBC
   eye-tracking work landed on **~12.5° below the eye line**; 24° forces a real downward eye strain
   or head tilt to read.
3. **No contrast backing** — pale cyan text (0.75, 0.92, 1) floats raw over the world. Our worlds
   are FULL of pale cyan: skies, water, the Crest gate flash (0.85, 0.98, 1 — nearly the same
   color!). Over the berth water or a bright vista the line can vanish entirely. Every guideline
   set (games + broadcast) calls for a semi-opaque plate ("letterboxing"), with outline/shadow as
   a supplement, never the sole device.
4. **Unlimited line count** — `Wrap` produces as many lines as the text needs; standards cap at
   **2 lines** per displayed subtitle, chunking longer text into successive cards.
5. **Default TextMesh font** — Unity's built-in Arial at fontSize 64 is a small bitmap; in-headset
   it blurs and shimmers. (Project convention is TextMesh, not TMP — honored below; the fix is a
   properly imported font, not a new text system.)
6. **Body text carries the speaker color** — cyan-as-text is doing two jobs (identity + content).
   Practice: near-white body text; speaker identity via a name tag, accent rule, or tinted plate
   edge — not by tinting the words the player must read.
7. **World clipping** — an in-world quad at 1.3 m clips into walls/props in tight interiors; the
   caption needs an always-legible answer (see §3.7).

## 2. What the research actually says (games, VR studies, standards)

**Placement & motion (VR-specific):**
- Head-locked captions beat world-fixed for comprehension and preference in eye-tracked studies
  (87.5% strong preference in one); the refined winner is head-locked **with a slight lag** so the
  text glides rather than jitters. BBC R&D's four-behavior comparison settled on lagged follow at
  **~12.5° below the eye line**.
- Owlchemy Labs (Cosmonious High — the industry's reference VR caption system): captions **always
  visible**, high-contrast, semi-transparent so the world shows through; they **enlarge when the
  speaker is far** and show **speaker icons when the speaker is behind you** — captions double as
  a "guide back to the speaker."
- The immersive alternative — speech bubbles anchored at the speaker — works when speakers stand
  in the world; our speaker hovers at the player's shoulder, so the two approaches converge:
  RILL's caption can live in a fixed comfortable zone AND still visually belong to her orb.

**Type & size:**
- Mixed-case **humanist sans-serif**, medium/semibold weight; thin strokes shimmer in VR; all-caps
  reserved for sound labels. Distinct letterforms (Il1, rn/m) matter.
- Flat-screen floor is 46 px @1080p; in VR think **angular size**: subtitle cap-height around
  **1.0–1.5°** of visual angle, verified IN the headset (and in our PlayMode screenshots), not on
  a monitor. Meta's guidance: legibility is tested on-device, scale with distance.
- Distance note in our favor: Quest optics focus near ~1.3–2 m, so our 1.3 m caption distance is
  already at the comfortable focal plane. Keep it; fix the angle and smoothing instead.

**Presentation:**
- Max **2 lines × ~37–40 chars**; top line longer when uneven; long text = successive cards with a
  ~¼ s beat between them.
- Duration: ~2–2.5 s per line minimum, plus a beat; never strobe single words shorter than 1 s.
- **Letterbox plate** (black or dark, semi-opaque — commonly ~50–75% alpha) + a subtle outline or
  shadow on glyphs. Drop shadow alone is explicitly insufficient.
- Speaker indication: name tags or per-speaker color accents (avoid mid-red; colorblind-safe).
- Player options are the gold standard (size, plate opacity, speaker tags on/off) — cheap to add
  once presentation is data-driven.

## 3. THE V2 SPEC (concrete, ready to be an envelope when the freeze lifts)

1. **Motion — lazy follow:** caption anchor targets `cam.forward` yaw-only, **12–15° below eye
   line**, 1.3–1.6 m out; the transform *chases* the target (smoothed, ~0.25–0.35 s settle,
   critically damped — the DevWarpBoard/board idiom, not per-frame snap). Roll never applied;
   pitch follows only within a soft band so lying back doesn't pin text to the ceiling.
2. **The plate:** rounded-corner quad, **dark slate ~65% alpha** (not pure black — pure black
   reads as a hole in bright worlds), padding ~0.6 em, sized to the current text block. Plate
   fades in/out with the line. ⚖ *Terry option:* plate opacity LOW / MED / HIGH in settings later.
3. **Type:** ship a real font asset (humanist sans — e.g. an open-license Noto/Inter-class face),
   imported at high point size so TextMesh renders crisp; **semibold**, mixed case, slight
   letter-spacing. Body color **near-white (0.95, 0.97, 1.0)**. Cap-height target **~1.2°**
   (≈ 3.2–3.5 cm at 1.5 m) — verified via the PlayMode screenshot audit, then headset.
4. **RILL's identity, without dyeing the words:** a small **"RILL" name tag** (or her glyph) at
   the plate's top-left in her state color (the existing Dormant/Stirring/Remembering/Late
   palette — the tag becomes a mood telltale for free), plus a 2 px accent rule along the plate's
   left edge in the same color. Body text stays near-white always. Other speakers (Cal, boards)
   get their own tag colors — the attribution system scales past RILL.
5. **Chunking:** `SubtitleText` gains `Chunk(text) → cards of ≤2 lines × 38 chars`; successive
   cards auto-advance at reading pace with a ~0.3 s beat. No more 4-line towers on long lines.
6. **Timing:** keep the read-time formula; floor any card at 1.5 s; keep the fade-in.
   ⚖ *Typewriter:* the per-character reveal is charming ("RILL speaks, not pastes") but standards
   favor whole-card presentation for reading flow, and it will fight VO timing at the M6 audio
   pass. Recommendation: reveal **per-word** (not per-char) at speech cadence now; when a line
   has a VO clip, present the card whole and let the VOICE carry the cadence. Terry calls it.
7. **Never unreadable:** captions render on an overlay queue (always on top, no world clipping) —
   they are UI, not scenery; this also survives the gate flash. And one Owlchemy borrow worth
   keeping: if RILL's orb is off-view while speaking, the name tag gains a small directional
   chevron toward her — captions guide you back to the speaker.
8. **Proof:** extend the existing PlayMode UI spatial audit: caption present in the golden route
   screenshots, plate luminance-contrast vs. sampled background ≥ 4.5:1, ≤2 lines, angle-below-
   gaze within band. A caption regression then fails CI like any other visual contract.

## 4. What we deliberately do NOT adopt

- **World-fixed subtitle boards** — research loser for comprehension; our worlds are traversal-heavy.
- **Speaker-anchored bubbles as the primary mode** — RILL hovers at the shoulder edge of view;
  pinning text there guarantees neck strain. (Bubbles stay an option for *world* speakers later.)
- **TMP migration for this** — the TextMesh convention stands (device-proven; the TMP canvas has a
  recorded on-device failure). Everything above is achievable with TextMesh + a good imported font
  + quads. If a future UI generation adopts SDF text globally, captions ride along then.
- **Pure-black 100% plates and boxed-in HUD frames** — kills the "world through the glass" feel;
  semi-transparency is the VR norm for exactly this reason.

## 5. Sequencing

Small, self-contained envelope (~3 commits: chunker+tests · presentation/plate/follow · audit
hook), player-facing win, zero cross-lane risk — a strong candidate for the early post-recovery
quality slice, before/alongside FORGE IV CP-9 (which inherits this spec as its caption section).
Gate ladder unchanged: nothing starts before recovery exits and the vertical slice is underway.

## Sources

- BBC R&D four-behavior VR subtitle study; eye-tracked VR 360° subtitle experiments (head-locked
  + lag preference, ~12.5° below eye line): tandfonline.com/doi/full/10.1080/0907676X.2023.2268122,
  3dvar.com/Brescia-Zapata2023Eye.pdf, redalyc.org/journal/2550/255066938005/html/
- Owlchemy Labs / Cosmonious High caption + vision-accessibility system:
  owlchemylabs.com/blog (vision accessibility update), developers.meta.com/horizon/blog
  (vision-accessibility-cosmonious-high), gamedeveloper.com (accessible-not-utilitarian)
- Game Accessibility Guidelines (subtitle presentation + customization):
  gameaccessibilityguidelines.com; Xbox Accessibility Guidelines 101/104:
  learn.microsoft.com/xbox/accessibility
- Ian Hamilton, "How to do subtitles well": ian-hamilton.com — 37–40 chars, ≤2 lines, 46px@1080p
  floor, letterbox + stroke, timing floors, speaker colors
- Meta Horizon OS developer guidance (legibility, angular sizing, billboarding, on-device
  verification): developers.meta.com/horizon
- Half-Life: Alyx accessibility options (subtitle size/width/reading-speed customization):
  roadtovr.com
