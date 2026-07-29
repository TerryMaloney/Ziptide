using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FH-S05 — the one additive signal the first-hour orchestrator needs to know the signature
    /// creature was resolved non-lethally. The envelope's law is narrow on purpose: expose the
    /// disable that already exists, publish it exactly once per down cycle, and change nothing about
    /// damage, health, loot, ecology, respawn or visuals.
    /// </summary>
    public class CreatureDisabledSignalTests
    {
        private GameObject _creature;
        private int _events;
        private CreatureRuntime _last;

        [SetUp]
        public void SetUp()
        {
            _events = 0;
            _last = null;
            CreatureRuntime.CreatureDisabled += OnDisabled;

            _creature = new GameObject("Creature_swarm_bug");
            var runtime = _creature.AddComponent<CreatureRuntime>();
            runtime.creatureId = "swarm_bug";
            runtime.respawnDelay = 0f;
        }

        [TearDown]
        public void TearDown()
        {
            CreatureRuntime.CreatureDisabled -= OnDisabled;
            if (_creature != null) Object.DestroyImmediate(_creature);
        }

        private void OnDisabled(CreatureRuntime runtime)
        {
            _events++;
            _last = runtime;
        }

        private CreatureRuntime Runtime => _creature.GetComponent<CreatureRuntime>();

        private void HitUntilDown()
        {
            for (int i = 0; i < 64 && !Runtime.IsDisabled; i++)
                Runtime.ReceiveHit(PvpWeapon.Taser, Vector3.zero, Vector3.forward);
        }

        [Test]
        public void FirstDisable_PublishesExactlyOnce_AndCarriesTheInstance()
        {
            HitUntilDown();

            Assert.IsTrue(Runtime.IsDisabled, "the creature must actually be down");
            Assert.AreEqual(1, _events, "one disable must raise exactly one event");
            Assert.AreSame(Runtime, _last,
                "the event must carry the instance so the orchestrator can filter the signature creature");
            Assert.AreEqual("swarm_bug", _last.creatureId,
                "the species must be readable from the event payload");
        }

        [Test]
        public void DamageWhileAlreadyDown_DoesNotRepeatTheEvent()
        {
            HitUntilDown();
            for (int i = 0; i < 8; i++)
                Runtime.ReceiveHit(PvpWeapon.Taser, Vector3.zero, Vector3.forward);

            Assert.AreEqual(1, _events, "a body already on the floor must not re-announce its disable");
        }

        [Test]
        public void ShockingADownedCreature_IsIgnored()
        {
            HitUntilDown();
            Runtime.Shock(1.5f);

            Assert.AreEqual(1, _events);
            Assert.IsTrue(Runtime.IsDisabled);
        }

        [Test]
        public void IsDisabled_IsFalseBeforeAnyDamage()
        {
            Assert.IsFalse(Runtime.IsDisabled);
            Assert.AreEqual(0, _events);
        }

        [Test]
        public void AThrowingListener_CannotStopTheDisableFromCompleting()
        {
            System.Action<CreatureRuntime> thrower = _ => throw new System.InvalidOperationException("boom");
            CreatureRuntime.CreatureDisabled += thrower;
            try
            {
                HitUntilDown();
                Assert.IsTrue(Runtime.IsDisabled,
                    "a bad subscriber must never leave a creature half-disabled");
                Assert.AreEqual(1, _events, "the other listeners still ran");
            }
            finally
            {
                CreatureRuntime.CreatureDisabled -= thrower;
            }
        }
    }
}
