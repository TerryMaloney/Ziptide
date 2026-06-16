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
            if (string.IsNullOrWhiteSpace(json)) return NewProfile();

            PlayerProfile p;
            try { p = JsonUtility.FromJson<PlayerProfile>(json); }
            catch { p = null; }

            if (p == null) return NewProfile();
            Migrate(p);
            return p;
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

            // Migration scaffold (no-op while schema is at 1):
            //   if (p.schemaVersion < 2) { /* transform v1 -> v2 */ p.schemaVersion = 2; }

            if (p.schemaVersion < PlayerProfile.CurrentSchemaVersion)
                p.schemaVersion = PlayerProfile.CurrentSchemaVersion;
        }
    }
}
