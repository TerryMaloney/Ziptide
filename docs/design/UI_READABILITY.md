# UI READABILITY & INTERACTION TARGET LAW

**Status:** v1 audit contract, 2026-07-11  
**Scope:** world-space `TextMesh` labels and text-bearing diegetic XRI interactables.

## Why

A diegetic panel can technically exist while failing in-headset because its glyphs are too small, its
selectable face is tiny or missing, or its label is visually detached from the thing the player must
touch. These are structural quality failures that can be caught before a headset session.

## Laws

1. **Effective TextMesh scale is literal:** `abs(characterSize) × max(1, fontSize)`.
2. **Readable floor:** a non-empty world-space label warns below `1.2` effective scale.
3. **Text-bearing interactables need a collider:** collider-before-interactable remains the grab/select law.
4. **Touch target floor:** the two largest world-space collider dimensions must each be at least `0.10m`.
   A long needle is not a usable tile just because one axis is large.
5. **Label attachment:** every label belonging to a text-bearing interactable must sit within `0.30m`
   of that interactable's combined collider bounds.
6. **Report, do not mutate:** the audit never resizes text, colliders, or transforms.
7. **v1 is warning-only:** promote individual findings to blockers only after generated scenes are clean
   and Terry confirms the thresholds on-device.

## Finding codes

- `UI_TEXT_TOO_SMALL`
- `UI_INTERACTABLE_NO_COLLIDER`
- `UI_TARGET_TOO_SMALL`
- `UI_LABEL_DETACHED`

## Exclusions

- Empty TextMesh placeholders.
- Interactables with no TextMesh descendants, such as weapons and physical props; their target laws live
  in the interaction/weapon systems rather than this UI gate.
- Runtime-only objects that do not exist in a baked scene. Their author/source tests must pin the same
  constants until the generated artifact can be audited.

## Promotion path

1. Land WARN-only rule and tests.
2. Run the full world audit after Terry's pending authors/rebakes.
3. Fix or explicitly justify real findings; never wholesale-whitelist.
4. Terry confirms text and target comfort in the headset.
5. Promote `NO_COLLIDER` first, then target/text floors only when report noise is zero.
