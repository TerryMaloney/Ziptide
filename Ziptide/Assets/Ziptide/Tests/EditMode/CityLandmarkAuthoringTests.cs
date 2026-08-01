using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The core test next door proves the RULE. This one proves the SHIPPED DATA obeys it — which is
    /// the half that was actually broken: `LandmarkScaleCore.ReadsAsStick` would have flagged the
    /// hangar's crane at 16 m x 2 m from the day it was authored, and nothing was asking.
    ///
    /// It checks BOTH levels on purpose. `docs/worldspecs/ToxicCity.spec.json` is upstream of the
    /// layout asset — `WorldSpecCompiler` finds the asset by scene name and overwrites its districts
    /// wholesale — so fixing only the asset would have shipped a fix that silently reverts the next
    /// time anyone runs "Compile World Specs". That is the exact shape of MISS_LEDGER #24 and #25.
    ///
    /// Scope is deliberately narrow. Plenty of authored landmarks across the twelve worlds are thin on
    /// purpose (W002's LightShaft is a shaft of light; W003's prisms are prisms). A landmark that
    /// declares itself a CRANE is claiming to be a working machine with a person's ladder on it, and
    /// that claim is checkable.
    /// </summary>
    public class CityLandmarkAuthoringTests
    {
        [Test]
        public void EveryCraneInTheLayoutAsset_HasEnoughSurfaceToCarryItsScaleCues()
        {
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(ZiptideConstants.PathToxicCityLayout);
            if (kit == null)
                Assert.Ignore("No ToxicCity layout on disk; the patcher's code default applies instead.");

            int cranes = 0;
            foreach (var d in kit.districts)
            {
                if (d == null || d.landmarks == null) continue;
                foreach (var lm in d.landmarks)
                {
                    if (lm == null || lm.kind != LandmarkKind.Crane) continue;
                    cranes++;
                    AssertNotAStick(d.id + "/" + lm.name, lm.height, lm.width);
                }
            }

            Assert.GreaterOrEqual(cranes, 1,
                "The shipyard's crane is the yard's only human-scale cue — if it stops being a Crane, "
                + "the district goes back to being boxes of unknowable size.");
        }

        [Test]
        public void EveryCraneInTheWorldSpec_HasEnoughSurfaceToCarryItsScaleCues()
        {
            int cranes = 0;
            foreach (var lm in SpecLandmarks())
            {
                if (lm.Kind != 1) continue;
                cranes++;
                AssertNotAStick("spec/" + lm.Name, lm.Height, lm.Width);
            }

            Assert.GreaterOrEqual(cranes, 1,
                "the spec is what the compiler writes into the layout asset — a spec with no crane in "
                + "it un-fixes the yard the next time anyone compiles");
        }

        private static void AssertNotAStick(string what, float height, float width)
        {
            Assert.IsFalse(LandmarkScaleCore.ReadsAsStick(height, width),
                what + " is " + height + " m tall and " + width + " m wide — too slender to hold a "
                + "ladder, a walkway or a cab, so it will read as a stick no matter how tall it is. "
                + "Minimum readable width at that height: "
                + LandmarkScaleCore.MinimumReadableWidth(height).ToString("F1") + " m.");
        }

        private readonly struct SpecLandmark
        {
            public readonly string Name;
            public readonly float Height, Width;
            public readonly int Kind;

            public SpecLandmark(string name, float height, float width, int kind)
            {
                Name = name; Height = height; Width = width; Kind = kind;
            }
        }

        /// <summary>Landmarks straight out of the spec file, read the same way CityWayfindingTests
        /// reads it — by regex, so the test does not need the editor's JSON plumbing.</summary>
        private static List<SpecLandmark> SpecLandmarks()
        {
            string path = Path.Combine(
                Path.GetFullPath(Path.Combine(Application.dataPath, "../..")),
                "docs", "worldspecs", "ToxicCity.spec.json");
            Assert.That(File.Exists(path), Is.True, "missing world spec: " + path);

            var list = new List<SpecLandmark>();
            foreach (Match m in Regex.Matches(File.ReadAllText(path),
                         "\"name\"\\s*:\\s*\"([A-Za-z0-9_]+)\".*?" +
                         "\"height\"\\s*:\\s*(-?[0-9.]+)\\s*,\\s*\"width\"\\s*:\\s*(-?[0-9.]+)" +
                         "\\s*,\\s*\"kind\"\\s*:\\s*([0-9]+)",
                         RegexOptions.Singleline))
            {
                var c = CultureInfo.InvariantCulture;
                list.Add(new SpecLandmark(m.Groups[1].Value,
                    float.Parse(m.Groups[2].Value, c), float.Parse(m.Groups[3].Value, c),
                    int.Parse(m.Groups[4].Value, c)));
            }

            Assert.Greater(list.Count, 0,
                "no landmark in the spec carries a 'kind' — the field the compiler needs to know a "
                + "crane from a tower has gone missing");
            return list;
        }
    }
}
