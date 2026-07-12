using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.5 — the starter VFX vocabulary, authored in CODE (the ForgeRecipeLibrary pattern:
    /// no asset round-trip, so CI can prove every recipe is in-budget). Resolve by string id via
    /// <see cref="Get"/>; the runtime factory builds a pooled ParticleSystem from the returned numbers.
    ///
    /// Every id here MUST eventually have a CALLER (a weapon hit / a world scatter row) — the F3.5
    /// wiring commits + tests enforce that both sides exist. Keep this set SMALL and bounded; new
    /// effects are new rows, each still passing <see cref="VfxRecipeDefinition.Validate"/>.
    /// </summary>
    public static class VfxLibrary
    {
        private static Dictionary<string, VfxRecipeDefinition> _byId;

        public static VfxRecipeDefinition Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            EnsureBuilt();
            return _byId.TryGetValue(id, out var r) ? r : null;
        }

        public static IEnumerable<VfxRecipeDefinition> All()
        {
            EnsureBuilt();
            return _byId.Values;
        }

        private static void EnsureBuilt()
        {
            if (_byId != null) return;
            _byId = new Dictionary<string, VfxRecipeDefinition>();
            foreach (var r in Author())
                _byId[r.id] = r.Validate();
        }

        /// <summary>The 7 canon starters. Colours stay natural (dust is dusty, steam is pale) — the
        /// "lamplight is warm, never neon-pure" discipline from F3.1b applies to sparks/muzzle too.</summary>
        private static VfxRecipeDefinition[] Author()
        {
            return new[]
            {
                // ── impacts: a short puff at the hit point, coloured by the surface ──
                new VfxRecipeDefinition
                {
                    id = "impact_metal", kind = VfxKind.Impact, oneShot = true,
                    burstCount = 14, sizeMin = 0.015f, sizeMax = 0.05f,
                    speedMin = 1.2f, speedMax = 3.2f, lifetimeMin = 0.12f, lifetimeMax = 0.35f,
                    colorStart = new Color(1f, 0.92f, 0.72f), colorEnd = new Color(0.7f, 0.6f, 0.45f, 0f),
                    gravity = 0.35f,
                },
                new VfxRecipeDefinition
                {
                    id = "impact_stone", kind = VfxKind.Impact, oneShot = true,
                    burstCount = 12, sizeMin = 0.02f, sizeMax = 0.07f,
                    speedMin = 0.6f, speedMax = 2.0f, lifetimeMin = 0.25f, lifetimeMax = 0.6f,
                    colorStart = new Color(0.62f, 0.58f, 0.52f), colorEnd = new Color(0.5f, 0.47f, 0.42f, 0f),
                    gravity = 0.5f,
                },
                // ── muzzle: a tight electric flash at the tip ──
                new VfxRecipeDefinition
                {
                    id = "muzzle_taser", kind = VfxKind.Muzzle, oneShot = true,
                    burstCount = 8, sizeMin = 0.02f, sizeMax = 0.06f,
                    speedMin = 1.5f, speedMax = 3.5f, lifetimeMin = 0.05f, lifetimeMax = 0.15f,
                    colorStart = new Color(0.7f, 0.9f, 1f), colorEnd = new Color(0.4f, 0.7f, 1f, 0f),
                    gravity = 0f,
                },
                // ── steam vent: continuous pale vapour rising off a pipe ──
                new VfxRecipeDefinition
                {
                    id = "steam_vent", kind = VfxKind.SteamVent, oneShot = false,
                    burstCount = 0, rateOverTime = 12f, sizeMin = 0.08f, sizeMax = 0.22f,
                    speedMin = 0.3f, speedMax = 0.8f, lifetimeMin = 1.0f, lifetimeMax = 2.0f,
                    colorStart = new Color(0.9f, 0.92f, 0.95f, 0.45f), colorEnd = new Color(0.85f, 0.88f, 0.92f, 0f),
                    gravity = -0.35f,
                },
                // ── ambient motes: the air stops being empty (biome-keyed dressing) ──
                new VfxRecipeDefinition
                {
                    id = "motes_amber", kind = VfxKind.Motes, oneShot = false,
                    burstCount = 0, rateOverTime = 6f, sizeMin = 0.01f, sizeMax = 0.03f,
                    speedMin = 0.05f, speedMax = 0.2f, lifetimeMin = 3.0f, lifetimeMax = 5.0f,
                    colorStart = new Color(1f, 0.82f, 0.5f, 0.5f), colorEnd = new Color(1f, 0.82f, 0.5f, 0f),
                    gravity = -0.05f,
                },
                new VfxRecipeDefinition
                {
                    id = "motes_spore", kind = VfxKind.Motes, oneShot = false,
                    burstCount = 0, rateOverTime = 5f, sizeMin = 0.01f, sizeMax = 0.03f,
                    speedMin = 0.05f, speedMax = 0.18f, lifetimeMin = 3.5f, lifetimeMax = 6.0f,
                    colorStart = new Color(0.65f, 0.85f, 0.55f, 0.5f), colorEnd = new Color(0.6f, 0.8f, 0.5f, 0f),
                    gravity = 0.03f,
                },
                // ── sparks: fast bright orange, gravity-pulled, gone in a blink ──
                new VfxRecipeDefinition
                {
                    id = "sparks_short", kind = VfxKind.Sparks, oneShot = true,
                    burstCount = 12, sizeMin = 0.008f, sizeMax = 0.025f,
                    speedMin = 2.0f, speedMax = 4.5f, lifetimeMin = 0.15f, lifetimeMax = 0.4f,
                    colorStart = new Color(1f, 0.75f, 0.35f), colorEnd = new Color(0.9f, 0.35f, 0.1f, 0f),
                    gravity = 0.8f,
                },
            };
        }
    }
}
