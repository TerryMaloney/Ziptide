using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Walking-skeleton contract tests for the data-driven pod loader (IPodLoader / PodRegistry).
    /// Guards 00_LOCKED_CONTRACTS ("code reads data, no hardcoded story") and the Phase B goal
    /// ("one pod loadable from data"). Headless — no device required.
    /// </summary>
    public class PodLoaderTests
    {
        private static PodNarrative MakePod(string id)
        {
            var pod = ScriptableObject.CreateInstance<PodNarrative>();
            pod.podId = id;
            pod.displayName = id + " (test)";
            return pod;
        }

        // PodRegistry authors its list in the inspector; set it from tests via the serialized field.
        private static void SetPods(PodRegistry registry, params PodNarrative[] pods)
        {
            FieldInfo field = typeof(PodRegistry).GetField("pods", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, "PodRegistry is expected to have a private 'pods' list field.");
            field.SetValue(registry, new List<PodNarrative>(pods));
        }

        [Test]
        public void PodRegistry_Loads_KnownPod_ById()
        {
            var registry = ScriptableObject.CreateInstance<PodRegistry>();
            var a = MakePod("pod_a");
            var b = MakePod("pod_b");
            try
            {
                SetPods(registry, a, b);

                Assert.AreSame(a, registry.Load("pod_a"));
                Assert.AreSame(b, registry.Load("pod_b"));
                Assert.IsTrue(registry.Has("pod_a"));
            }
            finally
            {
                Object.DestroyImmediate(a);
                Object.DestroyImmediate(b);
                Object.DestroyImmediate(registry);
            }
        }

        [Test]
        public void PodRegistry_ReturnsNull_ForUnknownOrEmptyId()
        {
            var registry = ScriptableObject.CreateInstance<PodRegistry>();
            var a = MakePod("pod_a");
            try
            {
                SetPods(registry, a);

                Assert.IsNull(registry.Load("nope"));
                Assert.IsNull(registry.Load(null));
                Assert.IsNull(registry.Load(string.Empty));
                Assert.IsFalse(registry.Has("nope"));
            }
            finally
            {
                Object.DestroyImmediate(a);
                Object.DestroyImmediate(registry);
            }
        }

        [Test]
        public void PodRegistry_AvailableIds_SkipsNullAndEmpty()
        {
            var registry = ScriptableObject.CreateInstance<PodRegistry>();
            var a = MakePod("pod_a");
            var blank = MakePod(string.Empty);
            try
            {
                SetPods(registry, a, null, blank);

                IReadOnlyList<string> ids = registry.AvailablePodIds;
                Assert.AreEqual(1, ids.Count);
                Assert.AreEqual("pod_a", ids[0]);
            }
            finally
            {
                Object.DestroyImmediate(a);
                Object.DestroyImmediate(blank);
                Object.DestroyImmediate(registry);
            }
        }
    }
}
