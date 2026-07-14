using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// DS-02 (DEVICE_STABILIZATION_FORENSIC_PLAN) — the developer-UI singleton contract. The device
    /// failure: DevMenu AND DevWarpBoard both self-bootstrapped via RuntimeInitializeOnLoadMethod,
    /// so two competing warp menus appeared. Per-type singleton guards can't catch that — this
    /// source scan does: exactly ONE self-bootstrapping dev interface may exist in DevTools, and it
    /// must be the device-proven DevWarpBoard. A second bootstrap turns CI red and points here.
    /// </summary>
    public class DevToolsSingletonTests
    {
        private const string BootstrapMarker = "[RuntimeInitializeOnLoadMethod";

        [Test]
        public void ExactlyOneDevInterface_SelfBootstraps_AndItIsTheBoard()
        {
            string root = Path.Combine(Application.dataPath, "Ziptide/Gameplay/Runtime/DevTools");
            Assert.IsTrue(Directory.Exists(root), "DevTools folder moved? " + root);

            var bootstrappers = new List<string>();
            foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                if (File.ReadAllText(file).Contains(BootstrapMarker))
                    bootstrappers.Add(Path.GetFileName(file));
            }

            Assert.AreEqual(1, bootstrappers.Count,
                "Exactly ONE dev interface may self-bootstrap (DS-02). Found: ["
                + string.Join(", ", bootstrappers) + "]. If you are adding a new dev surface, mount "
                + "it on DevWarpBoard instead of a second RuntimeInitializeOnLoadMethod.");
            Assert.AreEqual("DevWarpBoard.cs", bootstrappers[0],
                "the surviving bootstrap must be the device-proven physical board");
        }

        [Test]
        public void RetiredDevMenu_HasNoBootstrapAndNoGestureSummon()
        {
            string path = Path.Combine(Application.dataPath, "Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs");
            if (!File.Exists(path)) return; // fully deleted later = also fine

            string text = File.ReadAllText(path);
            Assert.IsFalse(text.Contains(BootstrapMarker), "DevMenu must stay retired from runtime");
            Assert.IsFalse(text.Contains("DevMenuGesture"),
                "the summon gesture belongs to DevWarpBoard alone — a second listener re-creates DS-02");
        }
    }
}
