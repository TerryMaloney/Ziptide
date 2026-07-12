#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class ReactivePracticalSwapTests
    {
        [Test]
        public void RootPracticalLook_KeepsHaloPoolAndHitProxy()
        {
            var root = new GameObject("ReactiveStreetPole");
            try
            {
                root.AddComponent<PracticalLight>();
                ForgeModuleLook look = root.AddComponent<ForgeModuleLook>();
                look.recipeId = "light_street_pole";
                look.keepChildren = new string[0];

                Assert.AreEqual(1, ReactivePropAuthor.Place(root.transform));
                CollectionAssert.Contains(look.keepChildren, ReactivePropAuthor.HitProxyName);
                CollectionAssert.Contains(look.keepChildren, ReactivePropAuthor.PracticalHaloName);
                CollectionAssert.Contains(look.keepChildren, ReactivePropAuthor.PracticalPoolName);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
#endif
