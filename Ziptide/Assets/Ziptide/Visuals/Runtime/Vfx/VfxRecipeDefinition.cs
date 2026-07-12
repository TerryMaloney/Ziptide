using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.5 — THE VFX FORGE, the RAILS half. Particles are the cheapest presence multiplier
    /// in VR, but hand-rolled effects have no budget — one careless system tanks the frame on Quest.
    /// This is the closed vocabulary + the hard caps, so every effect is born inside a budget.
    ///
    /// A recipe is PURE DATA (counts/sizes/speeds/colours) resolved by string id from
    /// <see cref="VfxLibrary"/> — the ItemFactory/ForgeRecipe pattern. The runtime factory (a pooled
    /// ParticleSystem per kind) configures itself from these numbers; <see cref="Validate"/> and
    /// <see cref="PeakParticles"/> are what CI checks so no recipe can ever ask for too much.
    /// </summary>
    public enum VfxKind
    {
        Impact,     // burst on a surface hit (dust/debris puff)
        Muzzle,     // short flash at a weapon tip
        SteamVent,  // continuous rising vapour from a pipe/vent
        Motes,      // slow ambient drift filling the air (biome dressing)
        Sparks,     // fast bright short-lived sparks
        Drips       // slow falling droplets (leaks/condensation)
    }

    [System.Serializable]
    public class VfxRecipeDefinition
    {
        /// <summary>Hard rail: no single ParticleSystem may exceed this (Validate + runtime maxParticles).</summary>
        public const int MaxParticles = 64;
        /// <summary>Runtime rail: no more than this many live VFX systems at once (VfxFactory clamps).</summary>
        public const int MaxLiveSystems = 6;

        public string id = "vfx";
        public VfxKind kind = VfxKind.Impact;

        [Tooltip("One-shot burst size (Impact/Muzzle/Sparks/Drips). 0 = purely continuous.")]
        public int burstCount = 12;
        [Tooltip("Continuous emission rate (Motes/SteamVent). 0 = burst-only.")]
        public float rateOverTime = 0f;

        public float sizeMin = 0.02f, sizeMax = 0.06f;
        public float speedMin = 0.5f, speedMax = 2f;
        public float lifetimeMin = 0.2f, lifetimeMax = 0.6f;

        public Color colorStart = Color.white;
        public Color colorEnd = new Color(1f, 1f, 1f, 0f);

        [Tooltip("-1 (rises, like steam) .. +1 (falls, like sparks/drips). 0 = neutral drift.")]
        public float gravity = 0f;

        [Tooltip("Burst then auto-return to the pool (Impact/Muzzle/Sparks). false = looping (Motes/SteamVent).")]
        public bool oneShot = true;

        /// <summary>Clamp every field into its rail and repair inverted ranges. Returns self for chaining.
        /// The whole reason F3.5 exists: an effect literally cannot be authored outside budget.</summary>
        public VfxRecipeDefinition Validate()
        {
            burstCount = Mathf.Clamp(burstCount, 0, MaxParticles);
            rateOverTime = Mathf.Clamp(rateOverTime, 0f, MaxParticles);
            sizeMin = Mathf.Max(0.001f, sizeMin);
            sizeMax = Mathf.Max(sizeMin, sizeMax);
            speedMin = Mathf.Max(0f, speedMin);
            speedMax = Mathf.Max(speedMin, speedMax);
            lifetimeMin = Mathf.Max(0.01f, lifetimeMin);
            lifetimeMax = Mathf.Max(lifetimeMin, lifetimeMax);
            gravity = Mathf.Clamp(gravity, -1f, 1f);
            return this;
        }

        /// <summary>Worst-case simultaneous particles this recipe can put on screen — the budget CI asserts.
        /// Burst is instantaneous; continuous peaks at rate × longest lifetime. Never above the cap.</summary>
        public int PeakParticles()
        {
            int continuous = Mathf.CeilToInt(rateOverTime * lifetimeMax);
            return Mathf.Min(MaxParticles, Mathf.Max(burstCount, continuous));
        }
    }
}
