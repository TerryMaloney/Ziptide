namespace Ziptide.Core
{
    /// <summary>
    /// Compile-time constants for all story/progression flag names.
    /// Use these constants in WorldPackDefinition.flagsRequired and flagsGranted
    /// instead of raw strings to prevent silent typo bugs.
    /// </summary>
    public static class ZiptideFlags
    {
        // ── Tutorial / Onboarding ──────────────────────────────────────────
        public const string TUTORIAL_COMPLETE       = "TUTORIAL_COMPLETE";
        public const string FIRST_HOLSTER           = "FIRST_HOLSTER";
        public const string FIRST_TRAVEL            = "FIRST_TRAVEL";
        public const string FIRST_JOB_COMPLETE      = "FIRST_JOB_COMPLETE";
        public const string FIRST_DRONE_DOWN        = "FIRST_DRONE_DOWN";

        // ── Chapter 1 (W001-W004) ─────────────────────────────────────────
        public const string C1_W001_ARRIVED         = "C1_W001_ARRIVED";
        public const string C1_W001_RILL_BOOT       = "C1_W001_RILL_BOOT";
        public const string C1_W001_SAW_MARA        = "C1_W001_SAW_MARA";
        public const string C1_W001_JOB_COMPLETE    = "C1_W001_JOB_COMPLETE";
        public const string C1_W002_JOB_COMPLETE    = "C1_W002_JOB_COMPLETE";
        public const string C1_W003_JOB_COMPLETE    = "C1_W003_JOB_COMPLETE";
        public const string C1_W004_JOB_COMPLETE    = "C1_W004_JOB_COMPLETE";
        public const string C1_W004_RILL_ASKED_CARGO = "C1_W004_RILL_ASKED_CARGO";
        public const string C1_BLOOM_FIRST_CONTACT  = "C1_BLOOM_FIRST_CONTACT";
        public const string C1_WAKE_GUILD_INTRO     = "C1_WAKE_GUILD_INTRO";

        // ── Chapter 2 (W005-W012) ─────────────────────────────────────────
        public const string C2_W005_JOB_COMPLETE    = "C2_W005_JOB_COMPLETE";
        public const string C2_W009_RILL_MISIDENTIFIED = "C2_W009_RILL_MISIDENTIFIED";
        public const string C2_CONTAINMENT_REVEALED = "C2_CONTAINMENT_REVEALED";
        public const string C2_ARCHITECTS_NAMED     = "C2_ARCHITECTS_NAMED";

        // ── Chapter 3 (W013-W019) ─────────────────────────────────────────
        public const string C3_W013_MEMORY_SHARD    = "C3_W013_MEMORY_SHARD";
        public const string C3_W019_RILL_REFUSED    = "C3_W019_RILL_REFUSED";
        public const string C3_MARA_REVEAL          = "C3_MARA_REVEAL";
        public const string C3_RILL_STIRRING        = "C3_RILL_STIRRING";

        // ── Chapter 4 (W020-W028) ─────────────────────────────────────────
        public const string C4_W024_COLOR_NAMED     = "C4_W024_COLOR_NAMED";
        public const string C4_W028_NO_JOB          = "C4_W028_NO_JOB";
        public const string C4_SABLE_INTRO          = "C4_SABLE_INTRO";
        public const string C4_SABLE_ALLIED         = "C4_SABLE_ALLIED";
        public const string C4_SABLE_OPPOSED        = "C4_SABLE_OPPOSED";

        // ── Chapter 5 (W029-W038) ─────────────────────────────────────────
        public const string C5_PATTERN_VISOR_BLEED  = "C5_PATTERN_VISOR_BLEED";
        public const string C5_W037_WARDEN_STANDOFF = "C5_W037_WARDEN_STANDOFF";

        // ── Chapter 6 (W039-W051) ─────────────────────────────────────────
        public const string C6_W039_PATTERN_WARNING = "C6_W039_PATTERN_WARNING";
        public const string C6_WARDEN_ALLY          = "C6_WARDEN_ALLY";
        public const string C6_WARDEN_ENEMY         = "C6_WARDEN_ENEMY";
        public const string C6_W051_RILL_NAMED      = "C6_W051_RILL_NAMED";

        // ── Chapter 7 (W052-W061) ─────────────────────────────────────────
        public const string C7_RILL_MEMORY_UNSEALED = "C7_RILL_MEMORY_UNSEALED";
        public const string C7_RILL_CHOSE_NAME_A    = "C7_RILL_CHOSE_NAME_A";
        public const string C7_RILL_CHOSE_NAME_B    = "C7_RILL_CHOSE_NAME_B";
        public const string C7_RILL_CHOSE_NAME_C    = "C7_RILL_CHOSE_NAME_C";

        // ── Chapter 8-12 / Endgame (W062-W068) ───────────────────────────
        public const string C8_W062_REVELATION      = "C8_W062_REVELATION";
        public const string C12_W063_BRANCH         = "C12_W063_BRANCH";
        public const string C12_W063_ENDING_A       = "C12_W063_ENDING_A"; // RILL stays in network
        public const string C12_W063_ENDING_B       = "C12_W063_ENDING_B"; // RILL crosses with Cal
        public const string C12_W063_ENDING_C       = "C12_W063_ENDING_C"; // RILL chooses to forget
        public const string C12_W063_ENDING_D       = "C12_W063_ENDING_D"; // RILL becomes the Pattern
        public const string C12_W068_COMPLETE       = "C12_W068_COMPLETE";

        // ── Player Choices ────────────────────────────────────────────────
        public const string PLAYER_HELPED_MARA      = "PLAYER_HELPED_MARA";
        public const string PLAYER_HELPED_SABLE     = "PLAYER_HELPED_SABLE";
        public const string PLAYER_HELPED_WARDEN    = "PLAYER_HELPED_WARDEN";
        public const string PLAYER_DESTROYED_SEAL   = "PLAYER_DESTROYED_SEAL";
        public const string PLAYER_REPAIRED_SEAL    = "PLAYER_REPAIRED_SEAL";
        public const string PLAYER_IGNORED_RILL     = "PLAYER_IGNORED_RILL";
        public const string PLAYER_TRUSTED_RILL     = "PLAYER_TRUSTED_RILL";

        // ── Signal System ─────────────────────────────────────────────────
        public const string SIGNAL_THRESHOLD_1      = "SIGNAL_THRESHOLD_1";
        public const string SIGNAL_THRESHOLD_2      = "SIGNAL_THRESHOLD_2";
        public const string SIGNAL_THRESHOLD_3      = "SIGNAL_THRESHOLD_3";
        public const string SIGNAL_MAX              = "SIGNAL_MAX";

        // ── The Transmission (identity layer — docs/storyboard/THE_TRANSMISSION.md, WORLD_DATA.md §3) ──
        // FRAGMENT_T#_FOUND = set when that fragment collectible is recovered (authored per-world).
        // TRANSMISSION_CLARITY_* = DERIVED tiers — never author these; TransmissionProgress.SyncClarityFlags
        // computes them from the found fragments (the de-garble UI reads the tier).
        public const string FRAGMENT_T1_FOUND       = "FRAGMENT_T1_FOUND";   // W004 — garbled junk; "addressed to the watchers"
        public const string FRAGMENT_T2_FOUND       = "FRAGMENT_T2_FOUND";   // W042 — "the voice is mine"
        public const string FRAGMENT_T3_FOUND       = "FRAGMENT_T3_FOUND";   // W047/W051 era — the "we" becomes the partner
        public const string FRAGMENT_T4_FOUND       = "FRAGMENT_T4_FOUND";   // W060 — THE NAME MOMENT + a coordinate
        public const string FRAGMENT_T5_FOUND       = "FRAGMENT_T5_FOUND";   // W062 — "you built it and chose to enter"
        public const string FRAGMENT_RILL_CONFESS   = "FRAGMENT_RILL_CONFESS"; // late — RILL's cut-off line, recovered elsewhere

        public const string TRANSMISSION_CLARITY_1  = "TRANSMISSION_CLARITY_1"; // after T1 — static; register: professional
        public const string TRANSMISSION_CLARITY_2  = "TRANSMISSION_CLARITY_2"; // after T2 — half-legible; register: confessional
        public const string TRANSMISSION_CLARITY_3  = "TRANSMISSION_CLARITY_3"; // after T3/T4 — clear; register: intimate
        public const string TRANSMISSION_CLARITY_MAX = "TRANSMISSION_CLARITY_MAX"; // full assembly (T1–T5 + RILL confession) — endgame only

        // ── World Completions (batch — W001 through W068) ─────────────────
        // For worlds not individually listed above, use this pattern:
        //   NarrativeSaveSystem.SetFlag("W" + worldId.PadLeft(3,'0') + "_COMPLETE");
        // Key individual completions:
        public const string W001_COMPLETE           = "W001_COMPLETE";
        public const string W002_COMPLETE           = "W002_COMPLETE";
        public const string W003_COMPLETE           = "W003_COMPLETE";
        public const string W004_COMPLETE           = "W004_COMPLETE";
        public const string W005_COMPLETE           = "W005_COMPLETE";
        public const string W006_COMPLETE           = "W006_COMPLETE";
        public const string W007_COMPLETE           = "W007_COMPLETE";
        public const string W008_COMPLETE           = "W008_COMPLETE";
        public const string W010_COMPLETE           = "W010_COMPLETE";
        public const string W011_COMPLETE           = "W011_COMPLETE";
        public const string W012_COMPLETE           = "W012_COMPLETE";
        public const string W009_COMPLETE           = "W009_COMPLETE";
        public const string W019_COMPLETE           = "W019_COMPLETE";
        public const string W020_COMPLETE           = "W020_COMPLETE";
        public const string W024_COMPLETE           = "W024_COMPLETE";
        public const string W028_COMPLETE           = "W028_COMPLETE";
        public const string W037_COMPLETE           = "W037_COMPLETE";
        public const string W039_COMPLETE           = "W039_COMPLETE";
        public const string W051_COMPLETE           = "W051_COMPLETE";
        public const string W062_COMPLETE           = "W062_COMPLETE";
        public const string W068_COMPLETE           = "W068_COMPLETE";
    }
}
