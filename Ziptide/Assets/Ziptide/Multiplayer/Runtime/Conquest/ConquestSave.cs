using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>
    /// Campaign persistence (the spec's ConquestSave row). The save is a DYNAMIC OVERLAY, not the
    /// whole galaxy: on load the map is rebuilt from the canonical seed list and only the fields the
    /// war actually changes (owners, defenses, stockpiles, fleets, turn) are applied on top. That
    /// keeps the save tiny (one profile-flag string, the CosmeticLocker idiom), and campaigns survive
    /// the galaxy growing — a planet id that no longer exists is just skipped.
    ///
    /// Format (single line, culture-invariant, no spaces):
    ///   CONQ1|t=3|P0=7,4,1,0,v:pulse_frigate+scout_skiff|Wdry_cistern=0,2,0,1,1,0.30,0.00,d:shield_spire
    /// Records: '|' · key'=' · fields ',' · list items '+'. Ids are snake_case, so separators are safe.
    /// </summary>
    public static class ConquestSave
    {
        public const string FlagPrefix = "CONQ_SAVE:";
        private const string Header = "CONQ1";

        public static string Serialize(ConquestState s)
        {
            if (s == null) return "";
            var sb = new StringBuilder(Header);
            sb.Append("|t=").Append(s.turn);

            foreach (var p in s.players)
            {
                sb.Append("|P").Append(p.playerId).Append('=')
                  .Append(p.flux).Append(',').Append(p.alloy).Append(',')
                  .Append(p.bloommatter).Append(',').Append(p.attacksThisTurn);
                if (p.fleetVesselIds.Count > 0)
                    sb.Append(",v:").Append(string.Join("+", p.fleetVesselIds));
            }

            foreach (var w in s.planets)
            {
                sb.Append("|W").Append(w.planetId).Append('=')
                  .Append(w.ownerId).Append(',')
                  .Append(w.defenseLevel).Append(',')
                  .Append(w.orbitalShieldLevel).Append(',')
                  .Append(w.stationedDefenseUnits).Append(',')
                  .Append((int)w.conflictState).Append(',')
                  .Append(w.instabilityLevel.ToString("F3", CultureInfo.InvariantCulture)).Append(',')
                  .Append(w.bloomContaminationLevel.ToString("F3", CultureInfo.InvariantCulture));
                if (w.builtDefenseIds.Count > 0)
                    sb.Append(",d:").Append(string.Join("+", w.builtDefenseIds));
            }
            return sb.ToString();
        }

        /// <summary>Rebuild the galaxy from seeds and overlay the save. Null = unreadable (caller
        /// starts a fresh war). Unknown planet/player records are skipped, never fatal.</summary>
        public static ConquestState Deserialize(string data, List<WorldSeed> seeds)
        {
            if (string.IsNullOrEmpty(data)) return null;
            var parts = data.Split('|');
            if (parts.Length < 2 || parts[0] != Header) return null;

            var state = ConquestGalaxy.BuildTwoPlayer(seeds);
            try
            {
                for (int i = 1; i < parts.Length; i++)
                {
                    var rec = parts[i];
                    int eq = rec.IndexOf('=');
                    if (eq <= 0) return null;
                    string key = rec.Substring(0, eq);
                    string val = rec.Substring(eq + 1);

                    if (key == "t")
                    {
                        state.turn = int.Parse(val, CultureInfo.InvariantCulture);
                    }
                    else if (key[0] == 'P')
                    {
                        var player = state.GetPlayer(int.Parse(key.Substring(1), CultureInfo.InvariantCulture));
                        if (player == null) continue;
                        var f = val.Split(',');
                        player.flux = int.Parse(f[0], CultureInfo.InvariantCulture);
                        player.alloy = int.Parse(f[1], CultureInfo.InvariantCulture);
                        player.bloommatter = int.Parse(f[2], CultureInfo.InvariantCulture);
                        player.attacksThisTurn = int.Parse(f[3], CultureInfo.InvariantCulture);
                        player.fleetVesselIds.Clear();
                        for (int k = 4; k < f.Length; k++)
                            if (f[k].StartsWith("v:", StringComparison.Ordinal))
                                player.fleetVesselIds.AddRange(f[k].Substring(2).Split('+'));
                    }
                    else if (key[0] == 'W')
                    {
                        var planet = state.GetPlanet(key.Substring(1));
                        if (planet == null) continue;   // world removed/renamed — skip, don't die
                        var f = val.Split(',');
                        planet.ownerId = int.Parse(f[0], CultureInfo.InvariantCulture);
                        planet.defenseLevel = int.Parse(f[1], CultureInfo.InvariantCulture);
                        planet.orbitalShieldLevel = int.Parse(f[2], CultureInfo.InvariantCulture);
                        planet.stationedDefenseUnits = int.Parse(f[3], CultureInfo.InvariantCulture);
                        planet.conflictState = (ConflictState)int.Parse(f[4], CultureInfo.InvariantCulture);
                        planet.instabilityLevel = float.Parse(f[5], CultureInfo.InvariantCulture);
                        planet.bloomContaminationLevel = float.Parse(f[6], CultureInfo.InvariantCulture);
                        planet.builtDefenseIds.Clear();
                        for (int k = 7; k < f.Length; k++)
                            if (f[k].StartsWith("d:", StringComparison.Ordinal))
                                planet.builtDefenseIds.AddRange(f[k].Substring(2).Split('+'));
                    }
                }
            }
            catch (Exception) { return null; }   // any malformed field → treat the save as unreadable
            return state;
        }
    }
}
