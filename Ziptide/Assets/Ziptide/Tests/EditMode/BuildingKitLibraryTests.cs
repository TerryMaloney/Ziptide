using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Art;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 0.2 — the first ArtModuleRegistry fulfillment. Pins: both shipped styles register
    /// both wall modules (worlds stop shipping flat-cube walls), a window module owns its complete
    /// look (its own Pane — BuildingBuilder skips Inset for kits), and walls still collide.
    /// </summary>
    public class BuildingKitLibraryTests
    {
        [SetUp]
        public void SetUp() => BuildingKitLibrary.EnsureRegistered(); // idempotent; survives a prior Clear()

        [Test]
        public void BothShippedStyles_RegisterBothWallModules()
        {
            foreach (var style in new[] { "salvage_row", "toxic_tenement" })
            foreach (var module in new[] { "WallSolid", "WallWindow" })
                Assert.IsTrue(ArtModuleRegistry.Has("buildingModule:" + style + "/" + module),
                    "unfulfilled: " + style + "/" + module);
        }

        [Test]
        public void WindowModule_OwnsItsPane_AndBuilds()
        {
            Assert.IsTrue(ArtModuleRegistry.TryBuild("buildingModule:salvage_row/WallWindow", out var kit));
            try
            {
                Assert.IsNotNull(kit.transform.Find("Pane"),
                    "kit window walls own their lit pane (builder skips Inset for kits)");
                Assert.Greater(kit.transform.childCount, 5, "a kit wall is structured, not one slab");
            }
            finally { Object.DestroyImmediate(kit); }
        }

        [Test]
        public void SolidModule_StillCollides()
        {
            Assert.IsTrue(ArtModuleRegistry.TryBuild("buildingModule:toxic_tenement/WallSolid", out var kit));
            try
            {
                Assert.Greater(kit.GetComponentsInChildren<Collider>(true).Length, 0,
                    "walls must keep collision — players lean on them");
            }
            finally { Object.DestroyImmediate(kit); }
        }
    }
}
