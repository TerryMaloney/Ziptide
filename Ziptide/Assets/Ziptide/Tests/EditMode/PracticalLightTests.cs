using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// F3.1b contract, headless (ForgeBodyTell pattern): a practical's halo and pool exist where
    /// Init put them, carry no colliders (a look, never a stat), and live/die TOGETHER through
    /// the one SetLit switch — the unified death is what sells a shot-out lamp.
    /// </summary>
    public class PracticalLightTests
    {
        private static PracticalLight Make(out GameObject host)
        {
            host = new GameObject("PracticalHost");
            var p = host.AddComponent<PracticalLight>();
            p.InitHalo(new Vector3(0f, 0.4f, 0f));
            p.InitPool(new Vector3(0f, 0f, 0f), Vector3.up, 1.2f);
            return p;
        }

        [Test]
        public void Init_BuildsHaloAndPool_NoColliders_NoShadows()
        {
            var p = Make(out var host);
            try
            {
                var halo = host.transform.Find("PracticalHalo");
                var pool = host.transform.Find("PracticalPool");
                Assert.IsNotNull(halo, "halo quad exists");
                Assert.IsNotNull(pool, "pool quad exists");
                Assert.AreEqual(0, host.GetComponentsInChildren<Collider>(true).Length,
                    "a look, never a stat — no colliders");
                foreach (var r in host.GetComponentsInChildren<Renderer>(true))
                    Assert.AreEqual(UnityEngine.Rendering.ShadowCastingMode.Off, r.shadowCastingMode,
                        r.name + " must not cast (Quest budget)");
                // The pool hugs its surface: +up normal → quad faces up from y≈0.01.
                Assert.AreEqual(0.01f, pool.position.y, 1e-3f, "pool sits just above the surface");
            }
            finally { Object.DestroyImmediate(host); }
        }

        [Test]
        public void SetLit_KillsAndRevives_HaloAndPoolTogether()
        {
            var p = Make(out var host);
            try
            {
                var rs = new System.Collections.Generic.List<Renderer>();
                foreach (var r in host.GetComponentsInChildren<Renderer>(true))
                    if (r.name == "PracticalHalo" || r.name == "PracticalPool") rs.Add(r);
                Assert.AreEqual(2, rs.Count);

                p.SetLit(false);
                Assert.IsFalse(p.IsLit);
                foreach (var r in rs) Assert.IsFalse(r.enabled, r.name + " dies with the lamp");

                p.SetLit(true);
                Assert.IsTrue(p.IsLit);
                foreach (var r in rs) Assert.IsTrue(r.enabled, r.name + " revives with the lamp");
            }
            finally { Object.DestroyImmediate(host); }
        }
    }
}
