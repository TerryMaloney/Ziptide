using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class InputActionReadKindCoreTests
    {
        [TestCase("Button", "System.Single", InputActionReadKind.Scalar)]
        [TestCase("Axis", "System.Single", InputActionReadKind.Scalar)]
        [TestCase("", "System.Int32", InputActionReadKind.Scalar)]
        [TestCase("Vector2", "UnityEngine.Vector2", InputActionReadKind.Vector2)]
        [TestCase("Stick", "UnityEngine.Vector2", InputActionReadKind.Vector2)]
        [TestCase("Dpad", "UnityEngine.Vector2", InputActionReadKind.Vector2)]
        [TestCase("", "UnityEngine.Vector2", InputActionReadKind.Vector2)]
        [TestCase("Quaternion", "UnityEngine.Quaternion", InputActionReadKind.Object)]
        [TestCase("", "", InputActionReadKind.Object)]
        public void Resolve_UsesDeclaredAndRuntimeValueTypes(
            string expected, string runtime, InputActionReadKind result)
        {
            Assert.That(InputActionReadKindCore.Resolve(expected, runtime), Is.EqualTo(result));
        }

        [Test]
        public void Resolve_IsCaseInsensitiveAndDeterministic()
        {
            Assert.That(InputActionReadKindCore.Resolve("button", "system.single"),
                Is.EqualTo(InputActionReadKind.Scalar));
            Assert.That(InputActionReadKindCore.Resolve("VECTOR2", "unityengine.vector2"),
                Is.EqualTo(InputActionReadKind.Vector2));
            Assert.That(InputActionReadKindCore.Resolve("VECTOR2", "unityengine.vector2"),
                Is.EqualTo(InputActionReadKindCore.Resolve("VECTOR2", "unityengine.vector2")));
        }
    }
}
