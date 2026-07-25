# ⚡ SIGNATURE POWERS IN VR — the hero-power layer: what we have, what to emulate, what nobody ships yet

**Status:** DESIGN (research synthesis, Fable 5 C-lane/T-Dog, 2026-07-23). No code changed.
Companion to `POWERS_MOBILITY_AND_EQUIP.md` (the four-tier power economy — still canon; this doc
adds the HERO-SIGNATURE layer on top) and the growing-loop plan (per-world signature abilities).
Terry's brief: Fortnite-mythic / Cap-shield / Zeus-bolt feel, VR-correct, kid-friendly,
non-lethal, and "something novel that nobody is doing." Review routing: **GPT reconciles** (Terry
directed single-reviewer mode; no multi-model round for this doc).

---

## 0 · Core finding

Across every great VR power (Alyx's gravity gloves, Asgard's Wrath 2's recall axe, RUMBLE's
stances, Ghostbusters' capture tether), the formula is identical: **performed with the body →
effect in the world (never on the camera) → state shown on the object/hands (never a HUD) →
a ceremony that makes it feel legendary → weight/resistance that makes it feel real.**
"Pressing a button for a big effect" is precisely what VR players call fake.

Two strategic facts fall out of the research:
1. **We already own the right chassis.** The wrist scanner's cover-and-charge ritual, the
   gravity gun, charge-by-salvage ultimates (designed), the 3-slot belt scarcity, and the
   disable+salvage verb ARE the hero-power substrate — nothing needs replacing.
2. **The industry has six documented gaps** (asymmetric two-hand powers · playspace-position as
   casting input · voice-cast · scale powers · summon-and-command · multi-step ritual ults).
   ZIPTIDE can OWN two or three of them — see §4.

## 1 · What we already have (source-audited 2026-07-23)

| Asset | State | Hero-power relevance |
|---|---|---|
| **Wrist scanner** (`WRIST_SCAN_PULSE`, device-tested) | 🟢 LIVE | Our first true gesture power: cover wrist → haptic/visual charge ramp → pulse. The exact grammar the research canonizes (two-stage, on-body state, big pose) |
| **Gravity gun** (+ comfort hop, wielder-ignore) | 🟢 LIVE | Closest mythic item; force-push verb already non-lethal |
| **6 augments** (surge_dash, bubble_guard, overclock, magnet_palm, sure_step, sixth_sense) | 🟢 LIVE | The Active/Passive tiers of the power economy, as belt gems |
| 8 non-lethal weapons w/ distinct verbs | 🟢 LIVE | Sonic AoE, static net, prism beam, melee blade, pike — the "arsenal of verbs" base |
| 3-slot belt (source-confirmed `BeltRig.cs`) | 🟢 LIVE | Slot scarcity = the loadout-choice engine for signatures |
| Ultimate tier: ChargeMeter, Capture Net, Salvage Storm, Time Dilation, Raise-Shield, Two-Hand Combine, Glyph Cast | 📋 DESIGNED (zero code) | The staging system every §4 signature plugs into |
| Charge-by-disable+salvage economy | 📋 CANON | "The thing you were doing anyway fuels the ult" |

## 2 · The translation rules (why Cap's shield works in VR and Dr. Doom's blast doesn't)

**Works:** a thrown iconic object that RETURNS (throw + yank-recall = the Thor/Cap grammar,
shipped and beloved in Asgard's Wrath 2) · palm-aimed force verbs with two-stage confirm
(highlight → commit; Alyx/Vader) · held STANCES (RUMBLE) · charge shown as glow-on-the-object +
haptic ramp + rising pitch (Until You Fall) · tug-of-war captures (Ghostbusters — our closest
tonal cousin: the capture fantasy outsells the kill fantasy in VR).
**Fails:** screen-filling blasts and camera shake (comfort + photosensitivity — Doom's actual
power is a CAMERA effect, so it dies in VR; the fix is world-space physics + particles with at
most a vignette pulse) · drawn-shape gestures under combat stress (The Wizards' documented
frustration; poses beat strokes) · HUD meters (state lives on hands/objects) · menu-selected
powers (radial wheels go unreadable) · continuous scale change and physics-dragged bodies
(nausea) · fine finger gestures on controllers (pinch = least reliable primitive).
**Reliability kit we adopt as LAW:** held poses over strokes · broad thresholds · trigger-gated
gestures (hold trigger while posing, release to cast — kills false positives) · two-stage
aim-then-commit · instant light+sound+haptic receipt on every recognition · big gestures ONLY
for rare ultimates (fatigue), small gestures for frequent verbs.

## 3 · EMULATE — proven mechanisms mapped onto our stack (build-ready shortlist)

1. **Flick-and-catch salvage pull** (Alyx grammar): hover-highlight → trigger + wrist-flick →
   constant-flight-time arc → generous catch. Already canon as the Gravity Glove plan — this
   research confirms it as VR's highest feel-per-engineering-hour; make it THE salvage verb.
2. **Weapon-glow super** (Until You Fall): every hero tool accumulates charge from clean
   disables; at full glow, one big swing releases a world-space EMP that powers down everything
   in radius. Maps 1:1 onto the designed ChargeMeter — the meter IS the glow.
3. **Salvage lasso tug-of-war** (Ghostbusters): tether a disabled drone → physically haul
   against its drift → overheat-vent timing beat → it lands in the recycler. This is the
   enemies-doc capture window, now with the proven physical-struggle staging.
4. **Palm force-pull/push with resistance** (Vader/B&S): heavy objects resist — lifting a crate
   is an act of will. Feeds "move that building" and prop placement too.
5. **Imbue-pour** (B&S): physically pour a charge into a held tool to enchant its next use —
   the ritual beat for the Fuse-Snap identity.

## 4 · THE NOVEL SET — signatures nobody ships (own these)

1. **THE STASIS GRAB** *(gap: asymmetric two-hand — the lone industry example is two independent
   powers; nobody ships one power operated by two coordinated hands).* Off-hand projects a
   stasis bubble that freezes a drone mid-air (frame); dominant hand reaches INTO the bubble and
   unscrews the part you want (act). Input: off-hand palm-out hold + head-gaze highlight →
   trigger to freeze → real reach-and-twist inside. **It is the disable+salvage loop AS one
   elegant power** — our thesis made physical. Kid check: slow, readable, zero pressure.
   Comfort: nothing moves the player. THE flagship signature.
2. **"ZIPTIDE!" — the spoken storm ult** *(gap: voice-cast; proven mass-market fun flatscreen
   (Mage Arena), never a core verb in shipped Quest VR).* When the ult meter is full, grip-and-
   SHOUT the game's name to unleash the tide storm (Capture Net/Salvage Storm staging). Kids
   demonstrably love casting out loud; shouting the title is identity marketing that plays
   itself. **⚠ Report-only riders:** needs RECORD_AUDIO permission — in a Mixed Ages/COPPA title
   that's a privacy-review item (on-device keyword spotting ONLY, zero audio retained, disclosed
   in the DUC/privacy policy) and a VRC minimal-permissions row. Ships with an equal gesture
   path (both-gems slam) as default; voice is the opt-in delight.
3. **THE ZIP DISC** *(the Cap-shield grammar, non-lethal).* A ricochet disc that tag-stuns
   drones on bounce and boomerangs home to a yank-recall + auto-magnet catch. Skill = banking
   multi-tag throws. Charge-by-salvage upgrades add bounce count / chain-arc. The "legendary
   physical THING" of the arsenal, and the merch-able icon.
4. **DUCK-AND-SURGE** *(gap: playspace-position as casting input — dodging exists, casting
   doesn't).* Really duck under a drone's scan beam (stealth, already comfortable per
   Blaston/Superhot) → RISE FAST from the crouch to fire the counter-pulse upward. Dodge and
   cast become one body motion; zero artificial movement; kids get a superhero move made of
   pure body. (Guard: generous height thresholds, seated-mode alternative per the
   accessibility contract.)
5. **TIDE-CHARGE — the Zeus ritual** *(gap: multi-step ritual ults).* Near a tide gate or in a
   storm world, raise an open hand skyward → visible charge crawls down your arm (glow + haptic
   ramp) → SLAM it into a held tool to imbue its next three shots with chain-arc. Two-step
   ceremony (collect, then imbue), world-sourced — the lightning-god moment, non-lethal
   (chain-stun), and it makes WORLDS into power sources (world-identity synergy).
6. **THE RECLAIMER WISP** *(gap: summon-and-command — Moss proved the bond, nobody ships the
   power).* Summon a palm-sized salvage wisp from your wrist; point at tagged salvage and it
   fetches; recall by holding your palm flat. Utility-first (fetch, scout, indicate), never a
   combat pet. **KEEP LATER** — coordinate with the growing-plan sprite review (my growing
   review parked sprites-as-systems; the wisp is the ONE justified exception if the loop needs
   a fetch verb, and only post-W002).
7. **THE STATIC GLOVE — Terry's pitch (2026-07-23), adopted as a first-class signature.**
   *The world:* one planet's atmosphere is canonically hyper-statically charged — objects on the
   surface arc and crackle at random intervals (the world VISIBLY telegraphs that power lives
   here; the arcing props double as charge hotspots). This gives the already-planned "static"
   biome hazard (GAME_PLAN M2's hazard five: wind/static/flood/spore/radiation) its power
   payoff: **the hazard IS the resource.**
   *The glove:* hold trigger + **turn your hand palm-UP** → the character's palm opens and
   ABSORBS static from the air — crackling arcs stream in from nearby charged objects/sky,
   glove glow builds, haptic ramp + rising pitch (faster near arcing props or under open storm
   sky, slow indoors — positioning becomes play). **Flip your hand over palm-OUT** → the charge
   primes (audible clack, glow snaps to the knuckles) → trigger fires a powerful static BLAST:
   chain-arcing stun that powers down drones in a cone, shoves loose props, non-lethal by
   nature.
   *Why it's mechanically excellent by the §2 rules:* the input is pure **palm orientation +
   trigger-gate** — a held pose measured with one dot product (no gesture recognition to
   misfire, EditMode-testable math); absorb→flip→fire is a **three-beat ritual** (collect,
   prime, release = the ceremony law); all state lives ON the glove (glow/arcs, never HUD);
   the payoff is world-space chain lightning, camera untouched. Flip-primes-then-trigger-fires
   keeps the two-stage confirm so kids never fire accidentally while absorbing.
   *Canon hooks:* shares the VFX/audio family with the shipped Static Net weapon · the growing
   plan's conductive-trait plants are its cross-path upgrade ingredients (bio path improves
   capacity/chain count — the garden-feeds-equipment law in action) · sibling of the
   Tide-Charge ritual — build ONE `WorldChargeSource` service (world declares its ambient
   charge type + hotspots) powering both, so every future world can source a power the same
   way.
8. *(Parked, on record: god-scale "Giant's Moment" — discrete swap only, world set-piece not a
   carried power; revisit at world-factory scale.)*

## 5 · Architecture mapping (extends, never forks)

- All §4 signatures are **`effectId`s in the existing augment/ultimate economy** —
  `AugmentDefinition` rows (stasis_grab, zip_disc, duck_surge, tide_charge) + pure cores for
  each state machine (pose detection thresholds as data), staged via the designed ChargeMeter.
  Zip Disc is an `ItemDefinition` + `ZipDiscRuntime` (the ItemFactory per-kind pattern; recall =
  the Alyx-catch socket magnetism we already use for holsters).
- **Gesture reliability layer** (one small new pure service): trigger-gated pose detection with
  broad thresholds + the receipt rule (light/sound/haptic on every recognition) — reused by
  wrist scanner, stances, duck-surge. EditMode-testable as pure threshold math.
- **⛔ Report-only:** everything touching rig/camera/comfort (duck thresholds, any vignette
  pulse), input actions (new bindings), and the microphone (permission + COPPA + DUC + store
  disclosure — Terry ⚖ before any voice work).
- Charge state NEVER on HUD: gem/tool glow + haptic ramp + pitch rise (matches the audio plan's
  bus/ducking data and the caption-twin law — "ULTIMATE READY" gets its caption).

## 6 · Prototype-first (recommendation, post-M0 like everything)

1. **Weapon-glow super on ONE weapon** (taser) — smallest proof of the whole charge economy.
2. **The Stasis Grab** — the flagship; it upgrades the core loop itself.
3. **The Static Glove on its storm world** — Terry's pitch; the best first WORLD-signature to
   build because its input is pure pose math (no recognition risk), it proves the
   `WorldChargeSource` pattern for every later world power, and the arcing-world telegraph is
   a world-improvement module the compiler can already stage.
4. **The Zip Disc** — the iconic object; high joy, moderate build (ricochet + recall).
5. **Tide-Charge ritual** (now riding the same WorldChargeSource service as the glove).
6. **Duck-and-Surge** (report-only review first) · then voice-cast ⚖ · wisp last (post-W002).
