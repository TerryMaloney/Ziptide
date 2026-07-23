# AUDIO VERTICAL SLICE QUEUE — the first 20 minutes, implementation-ready

**Lane:** Reasonbox (Fable 5), 2026-07-23, docs only. Companion to `AUDIO_PRODUCTION_MASTER_PLAN.md` (IDs, buses, import law, licensing flow, budgets all defined there).
**Scope:** every asset needed for boot/home → W000 wake (RILL, comfort console, grab/holster) → coupler repair → PUNCH IT/travel → W001 ambience/scanner/job → weapons/disable/salvage → creature presence → ship alerts (flight pre-work). Ordered by priority inside each beat; **P1** = slice-blocking, **P2** = slice-complete, **P3** = polish.
**Columns:** ID (stable, §8 law) · Trigger → runtime owner (the EXISTING class that fires it through the `AudioEvents` seam) · Ch (mono/stereo) · Sp (2D/3D) · Loop/One-shot · Target dur · Pri · Depends on · Device acceptance test (feeds `DEVICE_TEST_CHECKLIST.md`; ❌ ⇒ MISS_LEDGER).
**CC** = caption-twin required (master plan §5 law).

## 0 · Rails debt (before any asset lands)
| Row | What | Owner (future build lane) |
|---|---|---|
| R1 | `ZiptideMix` mixer + buses + ducking author (P0) | new `AudioMixAuthor` (create-only) |
| R2 | `AudioEvents.Play(id, pos?)` seam + `AUD_PLAY` tag | new small Core class |
| R3 | Volume sliders on comfort console (device-level persist) | `ComfortConsoleRuntime` surface family |
| R4 | Caption-twin plumbing on the subtitle surface | RILL subtitle path (observe, don't own) |
| R5 | `Zerogravity_Bloom_Favorite_1.wav` licensing backfill | §9 flow, manifest + CREDITS |

## 1 · Boot / Home Hub
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.ui.boot_ready | first Home Hub frame → `BootLoader`/`HomeHubRuntime` | S | 2D | O | 1.5s | P1 | R1,R2 | Chime lands BEFORE panels are interactable; once per cold boot |
| aud.ui.panel_hover | panel gaze/ray hover → Home Hub panels | M | 2D | O | 0.1s | P2 | R2 | Every hover answers; no spam at cap 4 |
| aud.ui.panel_confirm | NEW GAME/CONTINUE select | M | 2D | O | 0.3s | P1 | R2 | Confirm ≠ hover, felt as heavier |
| aud.music.home_drift | Home Hub bed → `AudioDirector` (AudioProfile) | S | 2D | L | 60–90s | P2 | R5 | Sits beside Zerogravity tonal reference; loops seamless |

## 2 · W000 wake + RILL + comfort console
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.rill.signature | any RILL line start → `RillCompanion` | S | 2D | O | 0.8s | P1 | R1,R2 | Precedes every subtitle; ducking dips Music −8dB audibly |
| aud.rill.signature_end | line dismiss/complete | S | 2D | O | 0.4s | P3 | above | Release swell matches 400ms release |
| aud.ambience.w000_hull | W000 load → `AmbienceDirector` spec (DATA row, no asset — synthesized hull hum variant) | — | 2D | L | — | P1 | none | Ship interior sounds different from Home Hub within 2s of arrival |
| aud.ui.comfort_select | preset chosen → `ComfortConsoleRuntime` | M | 2D | O | 0.5s | P1 | R2 | Distinct per-preset pitch step (Cozy low → Bold high) |
| aud.hands.grab | XRI select on grabbable → grab plumbing | M | 3D | O | 0.15s | P1 | R2 | Every successful grab clicks at the HAND's position |
| aud.hands.holster_in | holster socket accept → `HolsterSocketInteractor` | M | 3D | O | 0.3s | P1 | R2 | Holster THUNK ≠ grab; felt without looking (CC: no — haptic twin instead) |
| aud.hands.holster_out | draw from holster | M | 3D | O | 0.25s | P2 | R2 | Draw reads as reverse of stow |

## 3 · Coupler repair (the three stages)
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.world.machine_broken_loop | broken `RepairableMachine` proximity | M | 3D | L | 8s | P1 | R2 | Audible fault sputter localizes the coupler from 10m; 2s gap law |
| aud.world.part_seat | cell socketed (RepairStage advance) | M | 3D | O | 0.6s | P1 | R2 | Mechanical seat-CLUNK on the socket, not the player |
| aud.world.power_cycle | `RepairStage.Running` (`MACHINE_REPAIRED`) | M | 3D | O | 2.5s | P1 | R2 | Spin-up sweep ends in healthy idle; **CC** `[COUPLER ARMED]` |
| aud.world.machine_healthy_loop | repaired machine idle | M | 3D | L | 10s | P2 | above | Healthy ≠ broken blind-listenable |

## 4 · PUNCH IT / gate travel
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.ship.punchit_armed | arming satisfied → `ShipCastOffRuntime` | S | 2D | O | 0.8s | P1 | R2 | Fires once when gate arms; **CC** `[SHIP READY]` |
| aud.ship.punchit_ignite | launch commit | S | 2D | O | 2s | P1 | R2 | Ignite swell covers the crest entry; no audio pop at scene swap |
| aud.travel.crest_wash | crest cover → travel presentation | S | 2D | O | 3–5s | P1 | R2 | Wash bridges departure→arrival; ambience crossfade inaudible inside it |
| aud.travel.arrival_settle | arrival first frame | S | 2D | O | 1.5s | P2 | above | Settle resolves INTO the new world's bed key |

## 5 · W001 ToxicCity ambience + scanner + job
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.ambience.w001_toxic | W001 load → `AmbienceDirector` spec (DATA row — toxic drip/wind densities) | — | 2D | L | — | P1 | none | City reads wet + hollow within 2s; distinct from W000 hull |
| aud.ui.job_accept | contract accepted → job board runtime | M | 3D | O | 0.5s | P1 | R2 | Stamp-CHUNK on the board surface |
| aud.tool.scan_ping | `WristScanner` activate | M | 3D | O | 0.4s | P1 | R2 | Ping from the WRIST; repeat cadence never overlaps itself |
| aud.tool.scan_lock | fault identified | M | 3D | O | 0.7s | P1 | R2 | Lock ≠ ping (resolved two-tone); **CC** `[FAULT LOCATED]` |
| aud.world.zipline_ride | zipline attach→detach → `ZiplineRuntime` | M | 3D | L | ride | P2 | R2 | Whirr pitch tracks speed; stops EXACTLY at detach |
| aud.ui.reward_land | `RewardRouter` payout toast | S | 2D | O | 1s | P1 | R2 | The number lands WITH the sound; felt as earned, not UI noise |

## 6 · Weapons / disable / salvage
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.weapon.taser_fire | taser trigger → weapon plumbing | M | 3D | O | 0.3s | P1 | R2 | Crack from the muzzle; 6-voice cap holds under spam |
| aud.weapon.taser_hit | dart connect → `CreatureRuntime.ReceiveHit` | M | 3D | O | 0.4s | P1 | R2 | Hit confirm at the TARGET; distinct from fire |
| aud.creature.discharge_down | non-lethal disable → `CreatureRuntime.Disable` | M | 3D | O | 1.5s | P1 | R2 | Capacitor whine-down + clatter = the canon disable sound; no gore register |
| aud.world.salvage_grant | `SalvageCacheRuntime.GrantTo` | M | 3D | O | 0.8s | P2 | R2 | Scrap-tumble at the cache; pairs with reward toast when both fire |

## 7 · Creature presence (swarm_bug band + signature)
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.creature.swarm_idle | active swarm_bug proximity → behavior state | M | 3D | L | 6s | P1 | R2 | Chitter localizes the band before line-of-sight; 6-voice family cap |
| aud.creature.swarm_telegraph | aggression telegraph state | M | 3D | O | 1s | P1 | R2 | Telegraph audibly precedes the lunge ≥1s (readability law); **CC** `[CREATURE NEARBY]` |
| aud.creature.emerge | ecology wake/emerge tween | M | 3D | O | 0.8s | P3 | R2 | Burrow-pop matches the scale tween |

## 8 · Ship alerts / flight pre-work (M4 edge)
| ID | Trigger → owner | Ch | Sp | L/O | Dur | Pri | Dep | Device acceptance |
|---|---|---|---|---|---|---|---|---|
| aud.ship.idle_hum | boarded ship interior → boarding station presence | S | 2D | L | 12s | P2 | R2 | The body-heartbeat idle; below speech, above silence |
| aud.ship.alert_generic | ship warning state (coupler offline hint) | S | 2D | O | 1.2s | P2 | R2 | Alert obeys 2s-gap law + Comfort ceiling; **CC** `[SHIP ALERT]` |
| aud.flight.engine_state | `ShipFlightRuntime` throttle/boost (flight campaign only) | S | 2D | L | 10s | P3 | R1 | Pitch/intensity follows FlightModel speed01; Cozy preset scales intensity down |

## Debt rows (tracked, not blocking)
| Row | What |
|---|---|
| D1 | Migrate the ~24 legacy `AudioSource` call sites onto `AudioEvents` ids (opportunistic, per-file) |
| D2 | XRI sample "Button Pop" usage → replace with `aud.ui.*` family, then strip the sample dependency |
| D3 | Music state layers (calm/alert) — additive `AudioProfile` fields (P2 phase) |
| D4 | Meta XR Audio SDK spatializer evaluation (P3 phase; measured decision) |

**Totals:** 34 asset rows (2 are data-only ambience specs), 5 rails rows, 4 debt rows. P1 count: 22 — that is the minimum audible slice. Every P1 row's acceptance line is written to drop directly into `DEVICE_TEST_CHECKLIST.md`; every ❌ becomes a MISS_LEDGER entry with the five Class Law fields.
