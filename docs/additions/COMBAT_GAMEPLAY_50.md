# 🔫 COMBAT & GAMEPLAY FEEL — 50 ideas (bank; read README first)

Current: taser/pistol/gravity/static-net/sonic-thumper/prism-beam/hammer (data-driven +
WeaponCharge + PvpRules), full control scheme (sprint/crouch/slide/auto-run/jump/laser/quick-swap/
ping; ADS+reload open), 10 creatures + smart BotBrain. Non-lethal stun canon, kid-readable telegraphs
LAW. (Augments/dual-wield/locator-v2 already designed elsewhere — not duplicated here.)

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Slide-shot accuracy window: firing during a slide tightens spread for `PvpRules.SlideAccuracySeconds = 0.6` — rewards Doom-flow aggression. | S | WeaponCharge, LocomotionProfile, PvpRules | |
| 2 | Two-hand brace for the prism beam: off-hand grip on the barrel cuts `PrismChargeSeconds` by a brace multiplier — Alyx-style payoff. | S | ArenaWeaponDefinition, XRI grab, PvpRules | |
| 3 | Taser darts ricochet once off metal-tagged surfaces (`DartRicochetCount = 1`), sparking a ping so kids read the bounce. | S | dart projectile, surface tags, PvpRules | |
| 4 | Crouch steady-aim: crouching shrinks the laser-sight cone by a PvpRules constant — crouch becomes a marksman stance. | S | laser sights, LocomotionProfile, PvpRules | |
| 5 | Last-charge tell: on WeaponCharge's final shot the emitter glows amber and hums low — readable to opponent AND holder, no HUD. | S | WeaponCharge, AudioDirector | 🎨 |
| 6 | Active-recharge tap: pressing trigger exactly on the 1.5s recharge tick (audio cue) instantly completes it — Gears-style skill reload. | S | WeaponCharge, AudioDirector, PvpRules | |
| 7 | Holstered weapons recharge at 1.5× — makes the belt + quick-swap loop optimal instead of camping one gun. | S | WeaponCharge, belt, PvpRules | |
| 8 | Spawn-protection shimmer bubble on the existing 2.0s protection so nobody wastes charges on an invulnerable target. | S | spawn flow, PvpRules | 🎨 |
| 9 | Trick-shot holes: shots through a hammer wall-hole deal `+WallHolePeekBonus = 1` — demolition becomes firing-lane craft. | S | hammer holes, damage pipeline, PvpRules | |
| 10 | Point-blank taser jab: trigger while muzzle-touching does a contact stun using ThumperShoveMeters knockback — panic melee for every loadout. | S | taser, PvpRules, haptics | |
| 11 | Core-hit bonus: bots/creatures get a glowing chest core; hits there add `CoreHitBonus = 1` — a non-lethal "headshot" that never targets heads. | S | BotBrain rig, creature prefabs, PvpRules | 🎨 |
| 12 | Enemy ping paint: pinging while aimed at an enemy paints a fading outline for `PingMarkSeconds = 4` — spotting becomes a combat verb. | S | ping system, PvpRules | 🎨 |
| 13 | Per-weapon haptic recharge ticks: each of the 7 weapons gets a distinct pulse pattern on charge-ready — readiness felt, not checked. | S | WeaponCharge, haptics | |
| 14 | Attack bark: BotBrain plays a distinct chirp 0.5s before entering attack — an audio telegraph that works when the bot's behind you. | S | BotBrain, AudioDirector | |
| 15 | Slide-under-the-lane: prism beams fire at standing chest height, so slide/crouch canonically clears them — counterplay taught by geometry. | S | prism beam, LocomotionProfile | |
| 16 | Firing no longer breaks auto-run: shooting during auto-run keeps sprint speed — a pure flow fix. | S | control scheme, LocomotionProfile | |
| 17 | Thumper pogo: swinging the sonic thumper at the ground triggers the gravity gun's comfort-hop boost upward — same vetted arc, new input. | S | sonic thumper, comfort hop | |
| 18 | Ricochet preview: laser sight renders the predicted dart bounce off metal as a dotted line — teaches #3 without a tutorial. | S | laser sights, dart ricochet | 🎨 |
| 19 | Net direct-hit root: sticking a target dead-on (vs zone splash) roots them `StaticNetDirectRootSeconds = 1.0` — rewards accuracy over spam. | S | static net, PvpRules | |
| 20 | Thumper parry: the shockwave deflects incoming darts/projectiles inside ThumperRadius — melee-timing counterplay to every ranged weapon. | S | sonic thumper, projectile layer, PvpRules | |
| 21 | Shield posture: bots raise a front-arc energy shield (BotBrain sub-behavior) blocking taser/pistol, shattering to thumper, wrapping to net — forces quick-swap. | M | BotBrain, ArenaWeaponDefinition, PvpRules | 🎨 |
| 22 | Armor-plated creatures: 2-3 archetypes gain plates only the hammer cracks, exposing the core (#11) — the wall-breaker becomes a monster-opener. | M | creature archetypes, hammer, damage pipeline | |
| 23 | Net + flood puddle chains: a static net zone touching water conducts across the whole connected puddle, applying the slow to everyone in it. | M | static net, water surfaces, PvpRules | 🎨 |
| 24 | Net trampoline: after its slow expires, a floor net becomes a bounce pad for `NetBounceSeconds = 3` using the comfort-hop arc — one projectile, two phases. | M | static net, comfort hop, PvpRules | |
| 25 | Prism refraction: firing through glass/prism props splits the beam into 3 fanned beams at `PrismSplitDamage = 1` each — environmental aim puzzles per arena. | M | prism beam, world props, PvpRules | 🎨 |
| 26 | Dart tether: two darts stuck within `DartTetherMeters = 3` form a visible arc zapping anyone crossing for TaserDamage/2 — misses become area denial. | M | dart projectile, PvpRules | 🎨 |
| 27 | Stun-toss: gravity gun grabs a stunned small creature and throws it, stunning what it hits — chains non-lethal canon into physics comedy. | M | gravity gun, creature stun, ItemFactory mass | |
| 28 | Projectile catch: gravity gun grabs slow enemy orbs mid-flight for return-to-sender throws — the signature Alyx-depth interaction. | M | gravity gun, enemy projectile layer | |
| 29 | Overcharge shot: hold trigger to burn both WeaponCharges on one double-damage shot with `OverchargeRechargeSeconds = 3.0`. | M | WeaponCharge, ArenaWeaponDefinition, PvpRules | |
| 30 | Fill ADS: stick-click aim raises the weapon to eye line, tightens the cone, applies `AdsMoveFactor = 0.7` via LocomotionProfile. | M | CONTROL_SCHEME ADS, laser sights, LocomotionProfile | |
| 31 | Fill reload as a vent flick: snap the weapon down to dump heat and restore one charge, at a `VentLockoutSeconds = 2.5` penalty — gesture-based, no magazines. | M | CONTROL_SCHEME reload, WeaponCharge, gesture detect | |
| 32 | Laser-aware juke: BotBrain strafes when a laser sight rests on it >0.75s — laser sights become a tell you manage, bots feel alive. | M | BotBrain, laser sights | |
| 33 | Flow meter: consecutive hits without damage step RechargeSeconds down (1.5→1.2→0.9) with an escalating glow — Doom-flow made mechanical. | M | WeaponCharge, damage events, PvpRules | 🎨 |
| 34 | Swap-cycling: quick-swap during a recharge lets the incoming weapon fire immediately while the stowed one recharges on belt (#7) — the high-skill loop. | M | quick-swap, WeaponCharge, belt | |
| 35 | Wall-hole knitting telegraph: hammer holes visibly crackle closed over the last 10s of WallHoleRegenSeconds — players read when their lane dies. | M | hammer holes, shader/VFX | 🎨 |
| 36 | Wind vents: fan props curve net/dart projectiles along a visible stream — environmental bank-shot lanes designers place per arena. | M | projectile physics, world props, WorldRuntime | 🎨 |
| 37 | Charged thumper slam: hold-to-charge spends both charges for double radius + shove, with a rising whine + expanding ground ring telegraph. | M | sonic thumper, WeaponCharge, PvpRules | |
| 38 | Roaming weak point: one archetype's core migrates between 3 sockets every 5s with a pulse telegraph — tracking practice disguised as a monster. | M | creature archetypes, core-hit (#11) | 🎨 |
| 39 | Adaptive BotBrain: bot nudges up/down its 4 tiers mid-match on kill differential (±2 triggers a step) — every player gets a close game. | M | BotBrain difficulties, PvpRules match state | |
| 40 | Stun finisher pop: a fully stunned creature enters a 2s grab window where a thumper tap "bubbles" it harmlessly away in confetti — non-lethal, triumphant. | M | creature stun, sonic thumper, VFX | 🎨 |
| 41 | Shield-drone mini-boss: needs the full toolkit in sequence — net to ground, thumper to shatter shield, prism to hit core — each phase color-telegraphed. | L | BotBrain-derived drone, all weapons, PvpRules | 🎨 |
| 42 | Generalized destructible cover: hammer chips pooled chunks off designated props (reusing wall-hole regen) — arenas reshape mid-fight, pooled for Quest. | L | hammer, WorldRuntime props, pooling | 🎨 |
| 43 | Tide phases: flood water rises/falls on a telegraphed klaxon cycle, drowning low lanes and supercharging net-puddle chains (#23) — the arena on the PvpRules clock. | L | WorldRuntime, static net, PvpRules timers | 🎨 |
| 44 | Tide-surge horde mode: waves from the 10 archetypes with BotBrain squad pacing — surge/rest beats tuned Doom-style, spawns via ItemFactory registries. | L | creature archetypes, BotBrain, ItemFactory, wave director | |
| 45 | 2v2 bot squads: a role layer (pusher/anchor) over BotBrain's 8 states so pairs flank, cover, revive-protect — teammate bots for solo players. | L | BotBrain, PvpRules team rules | |
| 46 | Physics melee pass: two-hand grips on hammer/thumper with mass-based swing weight + capped angular velocity — Alyx heft within Quest budget. | L | XRI grab, hammer/thumper, LocomotionProfile | |
| 47 | Perfect-dodge tempo: slide/crouch within 0.2s of a telegraphed attack triggers `DodgeSlowmoSeconds = 1.0` of slow-mo with comfort vignette — earned, gated spectacle. | L | LocomotionProfile, BotBrain telegraphs, time-scale | |
| 48 | Zipline anchor: gravity gun alt-fire tethers two points into a rideable line you can shoot from, fixed-speed comfort + vignette — the "Ziptide" namesake verb. | L | gravity gun, comfort locomotion, WorldRuntime anchors | |
| 49 | Training dojo world: a scored drill scene per weapon (accuracy, swap-speed, ricochet) with medal thresholds in PvpRules constants, via TravelCoordinator. | L | new world scene, TravelCoordinator, PvpRules, save | |
| 50 | Ghost replay: after each round a translucent ghost re-runs the winning duel's path (positions + shot events, ring-buffer recorded) — kill review, no camera motion. | L | match recorder, PvpRules flow, VFX | 🎨 |
