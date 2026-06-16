using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>A concrete Definition only for tests (the base is abstract).</summary>
    public class RegTestDefinition : Definition
    {
        public int value;
    }

    public class DefinitionRegistryTests
    {
        private static RegTestDefinition Make(string id, int value = 0)
        {
            var d = ScriptableObject.CreateInstance<RegTestDefinition>();
            d.id = id;
            d.value = value;
            return d;
        }

        [Test]
        public void RegisterAndGet()
        {
            var reg = new DefinitionRegistry<RegTestDefinition>();
            var a = Make("scrap", 5);
            reg.Register(a);
            Assert.AreSame(a, reg.Get("scrap"));
            Assert.AreEqual(1, reg.Count);
            Assert.IsTrue(reg.Contains("scrap"));
        }

        [Test]
        public void Get_Unknown_ReturnsNull_TryGetFalse()
        {
            var reg = new DefinitionRegistry<RegTestDefinition>();
            Assert.IsNull(reg.Get("nope"));
            Assert.IsFalse(reg.TryGet("nope", out _));
        }

        [Test]
        public void EmptyIdOrNull_Ignored()
        {
            var reg = new DefinitionRegistry<RegTestDefinition>();
            reg.Register(Make(""));
            reg.Register(null);
            Assert.AreEqual(0, reg.Count);
        }

        [Test]
        public void DuplicateId_KeepsFirst()
        {
            var reg = new DefinitionRegistry<RegTestDefinition>();
            var first = Make("ore", 1);
            var second = Make("ore", 2);
            reg.Register(first);
            reg.Register(second);              // ignored + warned
            Assert.AreEqual(1, reg.Count);
            Assert.AreSame(first, reg.Get("ore"));
            Assert.AreEqual(1, reg.Get("ore").value);
        }

        [Test]
        public void RegisterAll_AndClear()
        {
            var reg = new DefinitionRegistry<RegTestDefinition>();
            reg.RegisterAll(new[] { Make("a"), Make("b"), Make("c") });
            Assert.AreEqual(3, reg.Count);
            reg.Clear();
            Assert.AreEqual(0, reg.Count);
            Assert.IsNull(reg.Get("a"));
        }
    }
}
