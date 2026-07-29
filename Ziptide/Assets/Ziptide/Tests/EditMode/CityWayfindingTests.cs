using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests
{
    /// <summary>
    /// THE CITY'S COMPASS holds, or CI says so. Two failure modes are invisible in a diff and
    /// expensive to find in a headset, so both are laws here:
    ///
    /// 1. a lantern leg with no authored street under it hangs lamps through a building;
    /// 2. three landmarks bunched inside a narrow arc are not a compass, they are a smear.
    ///
    /// Both are checked against `docs/worldspecs/ToxicCity.spec.json` — the file the city is
    /// actually compiled from — so re-laying the city out cannot silently break navigation.
    /// </summary>
    public class CityWayfindingTests
    {
        // The route CityWayfindingAuthor hangs lanterns along. Duplicated here deliberately: the
        // Editor assembly is not visible from tests, and a route that drifts from the author is
        // exactly what the connection law below is for.
        private static readonly string[] JobRoute =
            { "Dispatch", "Market", "Plaza", "Colonnade", "CanalRow", "Dispatch" };

        /// <summary>CityWayfindingAuthor.RelayMastOffset, mirrored (the Editor assembly is not
        /// visible from tests). The mast stands off the CanalRow anchor because the RelayVault is
        /// on it — and the offset is part of the compass, so the law measures the same point the
        /// bake builds.</summary>
        private static readonly Vector3 RelayMastOffset = new Vector3(-6.5f, 0f, -5.5f);

        private static string SpecText()
        {
            string path = Path.Combine(
                Path.GetFullPath(Path.Combine(Application.dataPath, "../..")),
                "docs", "worldspecs", "ToxicCity.spec.json");
            Assert.That(File.Exists(path), Is.True, "missing world spec: " + path);
            return File.ReadAllText(path);
        }

        /// <summary>district id → anchor, read straight out of the spec.</summary>
        private static Dictionary<string, Vector3> Anchors()
        {
            var map = new Dictionary<string, Vector3>();
            foreach (Match m in Regex.Matches(SpecText(),
                         "\"id\"\\s*:\\s*\"([A-Za-z0-9_]+)\"\\s*,\\s*\"anchor\"\\s*:\\s*\\{" +
                         "\\s*\"x\"\\s*:\\s*(-?[0-9.]+)\\s*,\\s*\"y\"\\s*:\\s*(-?[0-9.]+)" +
                         "\\s*,\\s*\"z\"\\s*:\\s*(-?[0-9.]+)",
                         RegexOptions.Singleline))
            {
                map[m.Groups[1].Value] = new Vector3(
                    float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(m.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(m.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture));
            }
            return map;
        }

        private static List<(string From, string To)> Connections()
        {
            var list = new List<(string, string)>();
            foreach (Match m in Regex.Matches(SpecText(),
                         "\"fromDistrictId\"\\s*:\\s*\"([A-Za-z0-9_]+)\"\\s*,\\s*" +
                         "\"toDistrictId\"\\s*:\\s*\"([A-Za-z0-9_]+)\""))
                list.Add((m.Groups[1].Value, m.Groups[2].Value));
            return list;
        }

        // ── The lantern route ──────────────────────────────────────────────────────────────────

        [Test]
        public void EveryLanternLegFollowsAnAuthoredStreet()
        {
            var connections = Connections();
            Assert.Greater(connections.Count, 0, "the spec has no connections to walk along");
            Assert.IsTrue(WayfindingCore.EveryLegIsConnected(JobRoute, connections),
                "a leg of the lantern route has no street under it — the lamps would hang "
                + "through geometry. Route: " + string.Join(" → ", JobRoute));
        }

        [Test]
        public void EveryStopOnTheRouteExistsInTheSpec()
        {
            var anchors = Anchors();
            foreach (string id in JobRoute)
                Assert.IsTrue(anchors.ContainsKey(id), "route stop '" + id + "' is not a district");
        }

        [Test]
        public void TheRouteIsALoop_SoTheWalkComesHome()
        {
            Assert.AreEqual(JobRoute[0], JobRoute[JobRoute.Length - 1],
                "the contract's walk must return to dispatch — a corridor makes the player "
                + "retrace, a loop makes the city feel like a place");
        }

        [Test]
        public void LanternsMarkEveryCorner()
        {
            var anchors = Anchors();
            var route = new List<Vector3>();
            foreach (string id in JobRoute) route.Add(anchors[id]);

            var lanterns = WayfindingCore.LanternPositions(route, WayfindingCore.LanternSpacing);
            foreach (Vector3 corner in route)
            {
                bool marked = false;
                foreach (Vector3 l in lanterns)
                    if (Vector3.Distance(l, corner) < 0.01f) { marked = true; break; }
                Assert.IsTrue(marked, "no lantern at the turn " + corner + " — a turn you cannot "
                    + "see is where a player gets lost");
            }
        }

        [Test]
        public void LanternsAreNeverFurtherApartThanTheSpacing()
        {
            var route = new List<Vector3>
            {
                Vector3.zero, new Vector3(0f, 0f, 37f), new Vector3(41f, 0f, 37f),
            };
            var lanterns = WayfindingCore.LanternPositions(route, 12f);
            for (int i = 0; i < lanterns.Count - 1; i++)
                Assert.LessOrEqual(Vector3.Distance(lanterns[i], lanterns[i + 1]), 12.01f);
        }

        [Test]
        public void ADegenerateRouteDoesNotThrow()
        {
            Assert.AreEqual(0, WayfindingCore.LanternPositions(null, 12f).Count);
            Assert.AreEqual(1, WayfindingCore.LanternPositions(new[] { Vector3.zero }, 12f).Count);
            Assert.IsFalse(WayfindingCore.EveryLegIsConnected(new[] { "A" }, Connections()));
        }

        // ── The sightline triple ───────────────────────────────────────────────────────────────

        [Test]
        public void TheSightlineTripleIsActuallyACompass()
        {
            var a = Anchors();
            Vector3 eye = a["Dispatch"];
            var bearings = new List<float>
            {
                WayfindingCore.BearingDegrees(eye, a["CanalRow"] + RelayMastOffset), // relay mast
                WayfindingCore.BearingDegrees(eye, a["Plaza"]),     // north tower, lit crown
                WayfindingCore.BearingDegrees(eye, BerthCenter()),  // your ship, warm floods
            };

            float min = WayfindingCore.MinPairwiseSeparation(bearings);
            Assert.GreaterOrEqual(min, WayfindingCore.MinLandmarkSeparationDegrees,
                "the three landmarks are within " + min.ToString("F0") + "° of each other as seen "
                + "from dispatch — that is not a compass, it is three lights in a smear");
        }

        [Test]
        public void HomeIsSouthOfDispatch_AndTheNorthTowerIsNorth()
        {
            var a = Anchors();
            Vector3 eye = a["Dispatch"];
            float home = WayfindingCore.BearingDegrees(eye, BerthCenter());
            float north = WayfindingCore.BearingDegrees(eye, a["Plaza"]);

            Assert.Less(Mathf.Abs(Mathf.DeltaAngle(home, 180f)), 45f,
                "the ship must read as SOUTH — 'home is behind you' is the instruction the whole "
                + "city leans on");
            Assert.Less(Mathf.Abs(Mathf.DeltaAngle(north, 0f)), 45f,
                "the north tower must actually be north, or the fixed bearing is a lie");
        }

        [Test]
        public void BearingsUseCompassConvention()
        {
            Assert.AreEqual(0f, WayfindingCore.BearingDegrees(Vector3.zero, Vector3.forward), 0.01f);
            Assert.AreEqual(90f, WayfindingCore.BearingDegrees(Vector3.zero, Vector3.right), 0.01f);
            Assert.AreEqual(180f, WayfindingCore.BearingDegrees(Vector3.zero, Vector3.back), 0.01f);
            Assert.AreEqual(270f, WayfindingCore.BearingDegrees(Vector3.zero, Vector3.left), 0.01f);
            Assert.AreEqual(0f, WayfindingCore.BearingDegrees(Vector3.zero, Vector3.up), 0.01f);
        }

        [Test]
        public void SeparationHandlesTheWrapAtNorth()
        {
            Assert.AreEqual(20f, WayfindingCore.MinPairwiseSeparation(new[] { 350f, 10f }), 0.01f);
            Assert.AreEqual(360f, WayfindingCore.MinPairwiseSeparation(new[] { 10f }), 0.01f);
        }

        private static Vector3 BerthCenter()
        {
            var m = Regex.Match(SpecText(),
                "\"berthCenter\"\\s*:\\s*\\{\\s*\"x\"\\s*:\\s*(-?[0-9.]+)\\s*,\\s*\"y\"\\s*:\\s*" +
                "(-?[0-9.]+)\\s*,\\s*\"z\"\\s*:\\s*(-?[0-9.]+)");
            Assert.IsTrue(m.Success, "the spec has no shipyard berth to point home at");
            var c = System.Globalization.CultureInfo.InvariantCulture;
            return new Vector3(float.Parse(m.Groups[1].Value, c), float.Parse(m.Groups[2].Value, c),
                float.Parse(m.Groups[3].Value, c));
        }
    }
}
