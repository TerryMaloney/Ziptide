# GPT Additions — Starter Gear Loop: Next Three Tools

Date: 2026-06-16
Label: Next sandbox gear targets

## Goal

After the current repo state is verified and tested, the next fun gameplay sandbox should focus on three tools:

1. Gravity Glove
2. Expanded Stun Dart
3. Left Wrist Scan Pulse

These should be built as modular tools, not one-off player scripts.

## Why these three

This combination creates a complete VR loop:

1. Scan the area from the wrist.
2. Find an enemy, resource, puzzle object, weak point, or hidden path.
3. Stun a drone/creature with the dart.
4. Use the gravity glove to grab, pull, place, or throw the stunned target or nearby objects.
5. Use object manipulation to solve the room, open a path, or defeat a drone in a playful way.

This is kid-friendly, physically satisfying, and more unique than basic shooting.

## VR interaction direction

Ziptide should use body-language mechanics where possible:

- Touch right hand to left wrist to trigger scan.
- Raise palm to target gravity glove.
- Hold grip to pull/hold.
- Release or flick hand to throw.
- Aim right hand to fire stun dart.
- Use hand-to-belt/chest gestures later for gear switching.

These actions make the game feel like VR instead of a normal controller game.

## Tool 1 — Left Wrist Scan Pulse

### First implementation

- Add a left wrist scanner/watch visual placeholder.
- Detect right-hand touch/overlap near the left wrist scanner zone.
- Trigger a scan pulse on valid gesture.
- Find nearby objects with a `Scannable` component.
- Highlight or outline scannables for a short duration.
- Add cooldown.
- Add simple pulse VFX/SFX placeholder.

### Reveals

Initial scannable targets:

- Hidden resource nodes.
- Puzzle sockets.
- Interactable objects.
- Enemy weak points.
- Creature/drone labels.
- Hidden paths or doors.

### Suggested components

- `Scannable`
- `ScanPulseTool`
- `WristScannerActivator`

### Acceptance tests / checks

- Scan activates only when right hand touches left wrist zone.
- Scan respects cooldown.
- Scannable objects highlight and then return to normal.
- Non-scannable objects are ignored.
- Tool works without breaking current Milestone A/B behavior.

## Tool 2 — Expanded Stun Dart

### First implementation

- Right-hand tool fires a slow, readable glowing dart.
- Dart applies stun to objects/enemies with `Stunnable`.
- Stunned target has clear visual feedback: sparks, wobble, color shift, or frozen pose.
- Stun expires after a short duration.
- Add cooldown or charge limit.

### Starter behavior

- Drones stop moving while stunned.
- Stunned drones become eligible for gravity glove grab.
- Stun should be non-lethal and family-friendly.

### Future variants

- Chain Dart: arcs to a nearby target.
- Anchor Dart: pins target to surface briefly.
- Pulse Dart: small stun burst on impact.
- Tag Dart: marks target for scan/RILL/drone help.
- Overcharge Dart: hold trigger for longer stun.

### Suggested components

- `StunDartTool`
- `StunDartProjectile`
- `Stunnable`
- `StunStatusEffect`

### Acceptance tests / checks

- Dart spawns from correct hand/tool transform.
- Dart hits target and applies stun.
- Stun duration expires correctly.
- Stunned state can be queried by other systems.
- Projectile cleans itself up.

## Tool 3 — Gravity Glove

### First implementation

Start with objects before enemies.

- Add `GravityGrabbable` component.
- Player points palm/hand ray at grabbable object.
- Hold grip to pull object toward hand.
- Object hovers at hold point.
- Release grip to drop.
- Release while hand is moving to throw.

### Enemy rule

Do not let the gravity glove grab active enemies at first.

Recommended rule:

- Normal enemy: not grabbable.
- Stunned enemy: grabbable.
- Heavy enemy: immune or partial tug only.

This creates a clean combo: Stun Dart -> Gravity Glove.

### Suggested components

- `GravityGloveTool`
- `GravityGrabbable`
- `GravityHoldPoint`
- Optional `GravityTargetFilter`

### Acceptance tests / checks

- Grabbable objects can be pulled, held, released, and thrown.
- Non-grabbable objects are ignored.
- Stunned drone can be gravity-grabbed.
- Unstunned drone cannot be grabbed.
- Physics does not explode or fling the player.

## Modular tool structure

Recommended pattern:

```text
ToolRecipe
- toolId
- displayName
- handSlot
- cooldown
- energyCost
- activationGesture
- targetRules
- effectRules
- visualEffectId
- soundEffectId
```

Early implementation can use direct MonoBehaviours, but keep names and boundaries ready for recipes/registries later.

## First gameplay sandbox loop

Build a tiny room/arena that proves:

1. Player enters starter area.
2. Right hand touches left wrist to scan.
3. Scan reveals a drone, battery, socket, or hidden target.
4. Player fires stun dart at drone.
5. Drone enters stunned state.
6. Player uses gravity glove to pull stunned drone or battery.
7. Player throws drone into wall/target or places battery into socket.
8. Door opens, reward appears, or RILL comments.

This loop proves scanning, combat, physics, puzzle logic, and VR body interaction in one small sandbox.

## Important comfort rule

The first version should move objects and enemies, not the player's body. Avoid rapid forced player movement until the base VR interactions feel good and are tested on Quest.