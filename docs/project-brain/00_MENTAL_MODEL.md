# 00 — MENTAL MODEL (shared understanding of ZIPTIDE)

*Snapshot date: 2026-06-16, by T-Dog. This is the build Claudes' working model of
the project. If you (another mind) disagree with anything here, that's a signal —
log it in `02_COORDINATION_LOG.md`.*

---

## 1. What ZIPTIDE is

A **VR game for Meta Quest, built in Unity** (URP, Android/ARM64, IL2CPP). It is
early — the team has a working VR sandbox (grab + locomotion + world dressing)
and is building outward from a strict modular foundation rather than a vertical
slice. The design intent (from the contracts/architecture docs) points at a game
with:

- **Pods / sectors** the player traverses and/or builds.
- A **ship** the player operates, including something called a **DriftCorridor**.
- A signature aesthetic layer hinted at as **Bloom / stewardship / origami
  folding** (spores, crease lines, gate folds).
- **Swappable story, art, and characters** — lore/names (e.g. placeholder
  "Basalt Coast", "RILL" voice lines) are content, never hardcoded in engine.

> ⚠️ The story/lore specifics above are **inferred from the docs' examples**, not
> a confirmed narrative bible. Treated as placeholders until Terry confirms.

## 2. The prime directive: modularity (the thing I guard)

From `docs/00_LOCKED_CONTRACTS.md` — these are **laws**, not preferences:

- **Everything swappable without cascading breakage:** story, names, visual
  theme, characters/voice, pods/sectors, ship-layout variants, signature VFX.
- **The engine must not "know" story/art specifics.** Systems know only
  **schemas + contracts**.
- **Data rule:** code reads `WorldProfile` + `PodNarrative` only — no literal
  place/story names in engine code.
- **Visuals rule:** systems request visuals through a **theme registry**, never
  by prefab name in code.
- **Assembly dependency rule:** *Visuals may depend on Core; Core may never
  depend on Visuals.* (Enforced in code — see §5.)
- **Contract-first:** each swappable system gets an interface (`IPodLoader`,
  `IThemeProvider`, `IBloomStewardship`, `IDriftCorridor`, …) + a contract doc +
  at least a smoke test.

My job is to make sure new work **does not** violate these, and to keep the docs
matching reality.

## 3. Module map (Unity assembly definitions)

Dependencies verified by reading each `.asmdef` on 2026-06-16:

| Assembly | Depends on | Responsibility |
|----------|-----------|----------------|
| **Ziptide.Core** | Unity XRI, InputSystem, ugui | Contracts, schemas, events, utilities, low-level runtime helpers. No story/art. |
| **Ziptide.Content** | (Visuals) | Data: `WorldProfile`, theme assets, pod packs (JSON + ScriptableObjects). |
| **Ziptide.Visuals** | Core | Sky/planet rig, theme profiles, (future) Bloom/origami VFX + theme binding. |
| **Ziptide.Gameplay** | Core, Visuals, Content, XRI | Build/traverse logic, world runtime, respawn, theme switching. |
| **Ziptide.Ship** | Core | Ship scene logic incl. DriftCorridor. *(Currently asmdef only — no code yet.)* |
| **Ziptide.Platform.Quest** | — | XR / Meta-specific glue. *(Currently asmdef only — no code yet.)* |
| **Ziptide.Editor** | (editor) | Menu tools, setup, validation. Editor-only. |
| **Ziptide.Tests** | (editor) | EditMode contract/smoke tests. |

**Dependency law status: ✅ honored.** `Ziptide.Core` references only Unity
packages — it does not reference `Ziptide.Visuals`. Good.

> 🔎 Note for review: `Ziptide.Content` (a data assembly) references
> `Ziptide.Visuals` because `WorldProfile` holds `VisualThemeProfile`
> references. That's legal under the current contracts (the ban is specifically
> Core→Visuals) but it's worth watching — if Content becomes "engine-ish," a
> Content→Visuals dependency could become a smell. Logged in 01_OPEN_QUESTIONS.

## 4. What actually exists and runs (as of latest commit)

**Milestone A — COMPLETE (verified on Quest):** scene runs; ground/horizon/sky
visible; cube is grabbable; teleport + move locomotion; pick up / drop / throw.

**Milestone A.5 — implemented, device-verify pending:** data-driven world
dressing. `VisualThemeProfile` (groundTint, skyGradient, planet settings),
`SkyPlanetRig` (procedural sky sphere + planet, no colliders/shadows,
Quest-safe), `WorldDirector` applies a theme on Start.

**Milestone B — implemented + now wired into the scene (PR #1):**
- `WorldProfile` (spawn, play-area size, groundY, respawn-on-fall, default +
  available themes).
- `WorldRuntime` orchestrates on Start: ensures WorldDirector, scales ground,
  builds `PlayAreaBounds` (four invisible walls on Ignore Raycast layer),
  attaches `FallRespawner` to XR Origin, builds `ThemeSwitchStation`, applies
  default theme. Exposes `ApplyTheme()` and `RespawnPlayer()`.
- A `WorldRuntime` object referencing `DefaultWorldProfile` is now in
  `MilestoneA_GrabCube.unity`.
- Theme assets exist: Default, Alien, Desert, Night.

**Tests & CI:** `Ziptide.Tests` EditMode suite covers the Core↛Visuals law
(`DependencyValidator`), default-asset sanity, and `PlayAreaBounds.Build`
idempotency. GitHub Actions (`ci.yml`) runs EditMode tests via GameCI on push to
`main`/`claude/**` and PRs to `main`; Android build only on `main`/manual.
Requires repo secrets `UNITY_LICENSE` / `UNITY_EMAIL` / `UNITY_PASSWORD`.

**Build/dev tooling:** `tools/dev_build_install.ps1` (batchmode build + adb
install to Quest); `tools/gpt_sync.ps1` (emits `GPT_PROJECT_UPDATE.md`).

## 5. How the laws are enforced today

- **`DependencyValidator`** (Editor) parses every `.asmdef` and fails if
  `Ziptide.Core` references `Ziptide.Visuals`. Covered by an EditMode test.
- **CI** compiles the project and runs EditMode tests on every push/PR.
- **Docs as source of truth:** `STATUS.md` is the "centerline" each session.

**Gap:** enforcement is currently only the *one* Core→Visuals rule. The broader
contracts (no hardcoded story names; visuals only via registry; interface per
swappable system) are **documented but not yet test-enforced**, mostly because
the interfaces (`IPodLoader`, `IThemeProvider`, etc.) **don't exist as code
yet**. See 01_OPEN_QUESTIONS.

## 6. Where ships fit (for the current art push)

- `Ziptide.Ship` exists as an assembly (→ Core only) but is **empty of code**.
- The architecture reserves the ship + **DriftCorridor** as a swappable system
  (`IDriftCorridor`) with **ship-layout variants allowed within zones**.
- Implication for art (Gemini): ship visuals must come in as **theme-bound
  assets resolved through the registry**, not prefab names baked into Ship logic.
  Ship *geometry/layout* and ship *skin* should be independently swappable.
- Detail + budgets in `03_SHIP_ART_DIRECTION.md`.

## 7. Key facts / settings (quick reference)

- **Unity:** 2022.3.62f3 · **URP** (Balanced profile for Android).
- **Target:** Meta Quest, Android, ARM64, IL2CPP, min API 29.
- **Repo:** github.com/TerryMaloney/Ziptide (Unity project lives in `Ziptide/`
  subfolder; repo root also holds `docs/`, `tools/`, workflows).
- **Active branch for this work:** `claude/architect-project-onboarding-2x7h60`.
- **Perf budget:** TBD (frame rate / draw calls / tris / memory all unset —
  flagged as a risk; for Quest assume 72/90 Hz targets and aggressive budgets).
