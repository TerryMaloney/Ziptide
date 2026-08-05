# ZIPTIDE HOUSE RULES — read this before you plan anything

**You are working on ZIPTIDE, a Meta Quest VR game. Unity 6000.2.9f1, URP, Android/IL2CPP/ARM64.**

This file is inside `Assets/` on purpose: it is the only project documentation you can see. The
repository also has a large `docs/` folder one level up from `Assets/`, outside your context, holding
the laws this project runs on. **This file is the short version of those laws.** Follow it and your
work lands. Ignore it and it gets reverted by the next operator.

Keep this file short. It is rules, not history. Harvested lessons are appended here by
`docs/THE_RATCHET.md`; the reasoning behind each one lives there.

---

## 1 · THE ONE THAT MATTERS MOST — stay inside your brief

**Change only what the task named. Nothing else.**

If you notice something else that looks broken — a wrong scene reference, a physics setting, a
missing asset — **write it down in your plan's notes and leave it alone.** Someone will thank you.
Fixing it silently inside an unrelated bake is how a real regression hides inside 400,000 lines of
scene diff, and it has already happened once:

- A city-building task changed `ToxicCityExit_WorldPack.sceneName` from `W000_DriftIn` to
  `MilestoneA_GrabCube`, repointing the game's "Leave" door at a test scene. It survived review only
  because a human read a diff nobody expected to contain travel changes.
- The same pass flipped `m_AutoSyncTransforms` in `ProjectSettings/DynamicsManager.asset`, a global
  physics behaviour that had been switched off deliberately.

**Never touch, as a side effect of any other task:** `ProjectSettings/**` · any `sceneName` field ·
`_Boot.unity` · build settings · input actions · anything under `Multiplayer/**`.

## 2 · DATA FIRST, THEN CALL THE BUILDER

Never hand-place geometry, and **never hand-edit `.unity` or `.prefab` YAML.** It has corrupted
scenes before.

The correct move — and you already do this well — is:

1. change fields on a ScriptableObject (`CityLayoutDefinition`, `WorldProfile`,
   `VisualThemeProfile`, `WorldPackDefinition`, `*Definition`);
2. invoke the existing named menu command that bakes it (e.g. `Ziptide/Worlds/Build Toxic City`).

If a thing you want cannot be expressed in the data model, **say so and stop.** The right fix is a
new serialized field, not hand-authored geometry. `RingCityDef` exists precisely because the approved
city was once unexpressible.

## 3 · MATERIALS — the cap is 60, and it is real

**Never inline material instances into a scene.** Create or reuse a shared `.mat` asset and point
every renderer at it.

A single re-bake once inlined ~1,464 material instances directly into `ToxicCity.unity`, producing a
474,000-line file. The hard cap for a shipped scene is **60 shared materials**. This is a Quest
game — every unique material is a draw call, and draw calls are the frame budget.

Shadows are off on hero props by budget (`shadowCastingMode = Off`). Leave them off.

## 4 · FINISH WHAT YOU AUTHOR

A half-filled asset reads as done and is not. Before you call a step complete:

- a `WorldProfile` still holding spawn `(0,0,0)` and the default `4×4` play area **is not authored** —
  `(0,0,0)` is usually inside geometry;
- every field you added needs a real value or an explicit note saying why the default is correct.

## 5 · GATES — turn art direction into tests

This is the best thing to come out of your work so far, and we want more of it.

When you build something whose quality is visual, write EditMode assertions **whose failure message
is the art direction itself**. Real examples now in `Tests/EditMode/HeroShipHullBuilderTests.cs`:

> `"must stay on the port flank — the claw is never mirrored."`
> `"must plant outboard of the hull, not tucked under it."`
> `"The truck cab sits ON TOP of the hull line."`

Rules of thumb that came from the same work, and that apply to anything you build:

- **Position parts as fractions of the whole**, not absolute numbers — `Z(f) => l * (0.5f - f)`. Then
  rescaling can never break proportion.
- **Author segments from endpoints**, not hand-written Euler angles
  (`Quaternion.FromToRotation`). Sign errors in a hand-written rotation are how a leg bends the wrong
  way.
- **Rotate articulated things as a rigid group about one pivot.** Rotating each segment on its own
  midpoint pulls the joints apart.
- **Blend identity with theme, don't replace it** — `Color.Lerp(identity, themed, 0.25f)` — and keep a
  skip-list of canon marks (hazard striping, the cyan coupler port) that a livery may never repaint.

## 6 · WHEN YOU RENAME OR DELETE PART NAMES, SEARCH THE WHOLE PROJECT

A part-name vocabulary is almost always enforced in **more than one place**.

The Scrapper rebuild deleted `Wing_L`, `Wing_R`, `TailFin`, `Exhaust_L` and `Exhaust_R`, and
correctly updated `Tests/EditMode/HeroShipHullBuilderTests.cs`. But
`Editor/Audit/FullSendPresentationAuditRules.cs` still *required* all five — **25 CI blockers, five
parts across five scenes.** The EditMode tests went green and the world audit went red.

So before you finish a rename or a deletion, search for the old names across **all** of:

- `Assets/Ziptide/Editor/Audit/**` — the audit rules (these run at build time, and they BLOCK)
- `Assets/Ziptide/Tests/**` — EditMode tests
- `Assets/Ziptide/Editor/**` — patchers, authors, builders
- runtime code that looks parts up by name (e.g. `ShipRefit`)

## 7 · RUNTIME RULES YOU CANNOT BREAK

- `TravelCoordinator.TravelTo(scene)` is the **only** way to change scenes. Never call
  `SceneManager.LoadScene` in gameplay code.
- `_Boot` is persistent, owns the XR rig and every runtime singleton, and is **never** a travel
  destination.
- Only **holstered** items travel between scenes.
- One `InputActionManager`, on the rig.
- No `System.Reflection` for runtime item creation — use public `Init()` methods.
- Log runtime diagnostics as `Debug.Log("ZIPTIDE: <TAG> key=value")`. Editor-time logs never reach a
  headset, so they prove nothing about the game.
- Scene names, layers and asset paths live in `ZiptideConstants.cs`. No raw scene-name strings.

## 8 · HOW TO WRITE THE PLAN

Your plan format is good — keep it. It should always contain:

- **Key Asset & Context** — the exact asset paths and the exact menu command you will invoke;
- **Implementation Steps** — each one "change these fields, run this command", with dependencies;
- **Verification** — split into what a human checks with their eyes, and the **named existing
  EditMode test files** that must stay green.

Then add two things:

- **Out of scope** — everything you noticed but deliberately did not touch (see rule 1);
- **Left half-done** — anything you could not finish, stated plainly. Saying "I could not determine
  the right spawn point" is worth far more than a plausible `(0,0,0)`.

---

*Maintained by the operators via `docs/THE_RATCHET.md`. Last harvest: 001 — 2026-08-05.*
