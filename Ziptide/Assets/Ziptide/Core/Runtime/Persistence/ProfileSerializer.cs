using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Pure (no file IO) serialize / deserialize / migrate for <see cref="PlayerProfile"/>.
    /// Kept separate from SaveSystem so it is fully unit-testable in EditMode — no headset, no disk.
    /// Never throws to callers: bad input yields a fresh profile.
    /// </summary>
    public static class ProfileSerializer
    {
        public static string Serialize(PlayerProfile profile, bool prettyPrint = false)
        {
            if (profile == null) profile = NewProfile();
            return JsonUtility.ToJson(profile, prettyPrint);
        }

        /// <summary>
        /// Parse JSON into a profile, migrating older schema versions forward. Null / blank / corrupt
        /// input returns a fresh profile (so a damaged save can never hard-crash the boot).
        /// </summary>
        public static PlayerProfile Deserialize(string json)
        {
            return TryDeserialize(json, out var p) ? p : NewProfile();
        }

        /// <summary>Like Deserialize, but tells the caller whether the input actually parsed —
        /// the crash-proof load path needs to know "corrupt" from "fine" so it can reach for the
        /// .bak instead of silently handing the player a fresh profile (a total progress wipe).</summary>
        public static bool TryDeserialize(string json, out PlayerProfile profile)
        {
            profile = null;
            if (string.IsNullOrWhiteSpace(json)) return false;
            try { profile = JsonUtility.FromJson<PlayerProfile>(json); }
            catch { profile = null; }
            if (profile == null) return false;
            // A truncated JSON can "parse" into a hollow object — a real profile always has an id.
            if (string.IsNullOrEmpty(profile.playerId)) { profile = null; return false; }
            Migrate(profile);
            return true;
        }

        public static PlayerProfile NewProfile()
        {
            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new PlayerProfile
            {
                schemaVersion = PlayerProfile.CurrentSchemaVersion,
                playerId = System.Guid.NewGuid().ToString("N"),
                createdAtUnix = now,
                lastSavedAtUnix = now,
            };
        }

        /// <summary>Bring an older-schema profile up to current. Add version cases as the schema grows.</summary>
        private static void Migrate(PlayerProfile p)
        {
            // Defensive defaults: a very old / partial save may have left collections null.
            if (p.flags == null) p.flags = new System.Collections.Generic.List<string>();
            if (p.resources == null) p.resources = new System.Collections.Generic.List<ResourceAmount>();
            if (p.worlds == null) p.worlds = new System.Collections.Generic.List<WorldState>();

            // v1 → v2 (META-LOOP): the transaction ledger arrived; older saves simply start empty.
            if (p.schemaVersion < 2)
            {
                if (p.ledger == null) p.ledger = new System.Collections.Generic.List<LedgerEntry>();
                p.schemaVersion = 2;
            }
            if (p.ledger == null) p.ledger = new System.Collections.Generic.List<LedgerEntry>();

            // v2 → v3 (FIELD CAMERA): the captured-photo album arrived; older saves start empty.
            if (p.schemaVersion < 3)
            {
                if (p.photos == null) p.photos = new System.Collections.Generic.List<CapturedPhoto>();
                p.schemaVersion = 3;
            }
            if (p.photos == null) p.photos = new System.Collections.Generic.List<CapturedPhoto>();

            if (p.schemaVersion < PlayerProfile.CurrentSchemaVersion)
                p.schemaVersion = PlayerProfile.CurrentSchemaVersion;
        }
    }
}
