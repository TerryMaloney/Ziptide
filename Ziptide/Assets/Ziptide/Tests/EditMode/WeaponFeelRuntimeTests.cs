using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class WeaponFeelRuntimeTests
    {
        [Test]
        public void Configure_DoesNotMoveRootOrInteractionTruth()
        {
            var host = new GameObject("WeaponFeelHost");
            try
            {
                host.transform.position = new Vector3(2f, 3f, 4f);
                host.transform.rotation = Quaternion.Euler(5f, 12f, 7f);
                Vector3 position = host.transform.position;
                Quaternion rotation = host.transform.rotation;
                var feel = host.AddComponent<WeaponFeelRuntime>();
                feel.Configure("test", WeaponFeelKind.Ballistic, 0.5f, 0.05f, 0.02f, 0.2f);
                Assert.That(host.transform.position, Is.EqualTo(position));
                Assert.That(Quaternion.Angle(host.transform.rotation, rotation), Is.LessThan(0.001f));
                Assert.That(host.GetComponent<Rigidbody>(), Is.Null);
                Assert.That(host.GetComponent<Collider>(), Is.Null);
                Assert.That(feel.Envelope.VisualKick, Is.EqualTo(0.02f).Within(0.0001f));
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [TestCase("pistol")]
        [TestCase("taser_dart_gun")]
        public void ItemFactory_InstallsDedicatedFeelOwner(string itemId)
        {
            GameObject item = null;
            try
            {
                item = ItemFactory.Create(itemId, Vector3.zero);
                Assert.That(item, Is.Not.Null);
                var feel = item.GetComponent<WeaponFeelRuntime>();
                Assert.That(feel, Is.Not.Null, itemId + " must construct its dedicated feel owner.");
                if (itemId == "pistol")
                {
                    Assert.That(feel.Envelope.Primary.Amplitude, Is.GreaterThan(0f));
                    Assert.That(feel.Envelope.Primary.Duration, Is.GreaterThan(0f));
                    Assert.That(feel.Envelope.Tail.Amplitude, Is.GreaterThan(0f));
                }
            }
            finally
            {
                if (item != null) Object.DestroyImmediate(item);
            }
        }

        [Test]
        public void ConfirmHit_IsNullControllerSafe()
        {
            var host = new GameObject("WeaponFeelNullHand");
            try
            {
                var feel = host.AddComponent<WeaponFeelRuntime>();
                feel.Configure("test", WeaponFeelKind.Electric, 0.6f, 0.08f, 0.01f, 0.4f);
                Assert.DoesNotThrow(() => feel.ConfirmHit(null));
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }
    }
}
