# W001 SIGNATURE CREATURE PASSPORT — the Husk-Molter

**Envelope:** FH-A01 · **Blocks:** FH-S05 (resolution adapter) → FH-S08 (final orchestration)
**Status:** 🟢 SPECIES DECIDED · body forged and photo-verified · encounter staged in ToxicCity
**Decided:** 2026-07-29

> This passport exists because FH-A01 was the root of the only strict dependency chain left in the
> first hour, and it was blocked on a decision nobody had written down. The species is chosen here,
> with reasons, so FH-S05 and FH-S08 can close against something real.

---

## 1. The choice, and why

**`husk_molter`.** Seven species ship. Six of them would work as *an* encounter; only one teaches the
thing the first hour exists to teach.

| Candidate | Why not the signature |
|---|---|
| `swarm_bug` | Already the canal ambient life. A swarm reads as pressure, not as a character. |
| `witness_mite` | Its whole mechanic is being *watched* — a lovely second-hour idea, but its counter is stillness, and a first-time player reads stillness as "nothing is happening". |
| `tendril` | Static. Nothing to observe; it is terrain with a temper. |
| `light_grazer` | Harmless and lantern-lit. It belongs to W002's dark cistern, where its light is the point. |
| `tether_swarm` | Its cluster-and-cord presentation is deliberately procedural; it is a systems showcase, not a first meeting. |
| `warden` | The armored sentinel is a chapter-scale threat. Meeting it in hour one spends the game's biggest silhouette on a tutorial. |
| **`husk_molter`** | **It is the only one whose counter is a lesson.** |

**The lesson:** *look before you commit.* Stun it and it sheds a grey decoy husk and skitters out the
back. A player who swings at the statue learns — instantly, physically, without a line of dialogue —
that this world rewards reading a creature before acting on it. That is the exact instinct the whole
game's non-lethal disable loop is built on, and the encounter teaches it in one mistake that costs
nothing.

It also satisfies the contract's hardest constraint: **the tell must not be HUD, colour, or audio
only.** The molt is a whole-body silhouette event. It reads across the canal, at any angle, with the
sound off, and to a player who cannot distinguish the greens.

---

## 2. Ecology — why it is here

The Moss is a drowned scrap city, and the husk-molter is what evolves in a place where everything is
salvage. It is a crawler that grew a *disposable* exoskeleton: shelter is abundant, predators are
sudden, and the cheapest defence in a scrapyard is to leave a convincing copy of yourself behind.

It lives where the wedges meet the relay vaults — inside the wall shadow, near warm machinery. It is
not a guard and not a hunter; the player is walking through its house, and the encounter reads as an
animal defending its corner rather than a monster spawned for combat.

**Habitat traces (dressing, no new systems):** shed husks weathering in corners at three ages —
fresh grey, sagging, crumbled to flakes. Anywhere the player finds three husks, they should feel
watched before anything moves.

---

## 3. Silhouette

Forged body: `husk_molter`, mossy noised carapace, dorsal ridge, four two-segment legs, nine bones —
photo-verified ✅✅ ("wet mossy carapace", "ridge fin + amber eye").

- **Read from any angle:** a low, wide, four-legged wedge with a single tall dorsal ridge fin. The
  ridge is the identity line; nothing else in the roster has a vertical fin over a horizontal body.
- **The decoy is the same silhouette in grey and completely still.** That contrast IS the mechanic:
  identical shape, drained colour, zero motion.
- **Scale:** knee-to-thigh height on an adult. Big enough to matter, small enough that a child-height
  player is not looking up at a threat in their first encounter.

---

## 4. States

| State | Body | Duration | Reads as |
|---|---|---|---|
| **Idle** | Slow four-beat crawl, ridge low | — | An animal minding its own business |
| **Alert** | Stops, ridge rises, amber eye tracks the player | ~0.6 s | "It has noticed you" |
| **Telegraph** | Body compresses back over the hind legs, ridge at full height | **0.6–0.9 s** | "Something is about to happen" — the whole-body window the VR canon requires |
| **Action** | Short committed lunge along the ground | ~0.5 s | A shove, never a strike; non-lethal canon |
| **Recovery** | Legs splayed, ridge drops, eye dims | ~1.2 s | The opening |
| **Stunned** | Frozen mid-posture, discharge arcs | 1.5 s | The moment the trick fires |
| **Molt** | Sheds a grey husk in place and skitters out the back | one per stun, 6 s cooldown | **The lesson** |
| **Disabled** | Crumples wide and low, colour drained, arcs burst | — | Powered down like a machine exhaling |

**Timing law:** the telegraph sits at 0.6–0.9 s. Faster and a first-time player cannot read it;
slower and it stops feeling like an animal.

**Personal space:** ≥3 m. The observation beat only counts from outside its reach — rewarding a
player for walking into a hostile's face would teach exactly the wrong instinct.

---

## 5. The counter

1. **Observe** from outside personal space until the ridge and the eye-track register
   (`SIGNATURE_CREATURE_OBSERVED`, 2.5 s of real attention).
2. **Stun** it — taser, thumper, anything that discharges.
3. **It molts.** The husk stands there looking exactly like the animal.
4. **Hit the one that MOVES.** The husk never moves and crumbles on its own.
   *Shortcut for the observant:* a gravity grab BEFORE it molts skips the trick entirely — the
   reward for having watched first.
5. `SIGNATURE_CREATURE_REDIRECTED_OR_DISABLED` fires once, filtered to this instance.

**Non-lethal, always.** It powers down. Nothing bleeds, nothing screams, and the player can walk past
its crumpled shape on the way back without feeling like they killed something.

---

## 6. Audio grammar (with visual redundancy)

Every cue below has a visual twin, because the whole encounter must survive with the sound off.

| Cue | Sound | Visual twin |
|---|---|---|
| Alert | A single dry chitin click | Ridge rises, eye brightens |
| Telegraph | Rising shell-creak | Body compresses |
| Molt | Wet tearing crack | Grey husk appears, real body bolts |
| Disable | Capacitor whine-down and clatter | Crumple, discharge arcs |

RILL's chime signature precedes her teaching lines, as everywhere else.

---

## 7. Quest budget

Declared **before** any art iteration, per the envelope's law:

| Budget | Value |
|---|---|
| Triangles | ≤ 3,000 at LOD0 |
| Bones | 9 (shipped) |
| Materials | 1 body + the live-emissive eye submesh |
| LOD0 → LOD1 | 12 m · LOD1 → cull 45 m |
| Concurrent instances in W001 | 2 (the authored `Molters_Relay` zone) |

The eye submesh stays live-emissive so the existing `ForgeBodyTell` bridge keeps driving it — the
decoy is a frozen grey clone of the same forged body, which is why the trick costs nothing extra.

---

## 8. Review artifacts still owed (Picasso)

- [ ] Contact sheet: front / side / back at LOD0.
- [ ] State sheet: alert · telegraph · action · recovery · molt · disabled.
- [ ] The pair shot: real body beside its husk, same frame, same light — the readability test that
      matters, because if these two are hard to tell apart *in motion* the encounter is unfair.
- [ ] Arrival-view plate: the creature at 8 m in ToxicCity's authored light, not booth light.

---

## 9. What this unblocks

- **FH-S05** — shipped. `CreatureRuntime.IsDisabled` + `CreatureDisabled`, filtered to this instance
  by `FirstHourW001Orchestrator`.
- **FH-S08** — shipped. The observation window is measured from the real tracked head pose at ≥3 m.
- **Remaining:** device readability. Does the molt read as a trick or as a bug? That answer only
  exists in a headset, and it is the one question this passport cannot settle.
