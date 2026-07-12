#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    public class ReactiveWorldWiringTests
    {
        [Test]
        public void WorldDressingBuilder_InvokesReactivePassExactlyOnceAndLast()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "WorldDressingBuilder.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            const string sign = "SignAuthor.Place(dressRoot, kit, route);";
            const string reactive = "ReactivePropAuthor.Place(dressRoot);";
            Assert.AreEqual(1, Count(source, reactive));
            Assert.Greater(source.IndexOf(reactive), source.IndexOf(sign),
                "the scan must run after every visual/Forge author has created its recipe holders");
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
