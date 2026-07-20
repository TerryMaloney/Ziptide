using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Audit;

namespace Ziptide.Tests.EditMode
{
    public sealed class RouteContinuityAuditRulesTests
    {
        [Test]
        public void ContinuousSurface_HasNoUnsupportedSamples()
        {
            GameObject floor = null;
            try
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "ContinuousRoute";
                floor.transform.position = new Vector3(0f, -0.10f, 0f);
                floor.transform.localScale = new Vector3(2f, 0.20f, 6f);
                Physics.SyncTransforms();

                int unsupported = RouteContinuityAuditRules.CountUnsupportedSamples(
                    new Vector3(0f, 0.30f, -2.85f),
                    new Vector3(0f, 0.30f, 2.85f),
                    0.40f, 0.05f, 1.0f);

                Assert.That(unsupported, Is.Zero);
            }
            finally
            {
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }

        [Test]
        public void RotatedBoxRoute_SamplesItsActualLongAxis()
        {
            GameObject route = null;
            try
            {
                route = GameObject.CreatePrimitive(PrimitiveType.Cube);
                route.name = "RotatedRoute";
                route.transform.position = new Vector3(2f, -0.10f, -1f);
                route.transform.rotation = Quaternion.Euler(0f, 37f, 0f);
                route.transform.localScale = new Vector3(2f, 0.20f, 8f);
                Physics.SyncTransforms();

                RouteContinuityAuditRules.GetSurfaceCenterline(route.GetComponent<Collider>(),
                    out Vector3 start, out Vector3 end);
                int unsupported = RouteContinuityAuditRules.CountUnsupportedSamples(
                    start, end, 0.40f, 0.05f, 1.0f);

                Assert.That(unsupported, Is.Zero,
                    "A rotated solid walkway must not be sampled across its world-space AABB diagonal.");
                Vector3 sampledDirection = (end - start).normalized;
                Assert.That(Mathf.Abs(Vector3.Dot(sampledDirection, route.transform.forward)),
                    Is.GreaterThan(0.99f));
            }
            finally
            {
                if (route != null) Object.DestroyImmediate(route);
            }
        }

        [Test]
        public void DeliberatelyBrokenRoute_HasUnsupportedSamples()
        {
            GameObject a = null;
            GameObject b = null;
            try
            {
                a = MakeFloor("RouteA", new Vector3(0f, -0.10f, -2f), new Vector3(2f, 0.20f, 2f));
                b = MakeFloor("RouteB", new Vector3(0f, -0.10f, 2f), new Vector3(2f, 0.20f, 2f));
                Physics.SyncTransforms();

                int unsupported = RouteContinuityAuditRules.CountUnsupportedSamples(
                    new Vector3(0f, 0.30f, -2.90f),
                    new Vector3(0f, 0.30f, 2.90f),
                    0.35f, 0.05f, 1.0f);

                Assert.That(unsupported, Is.GreaterThan(0),
                    "A visible two-metre gap must be sampled as unsupported space.");
            }
            finally
            {
                if (a != null) Object.DestroyImmediate(a);
                if (b != null) Object.DestroyImmediate(b);
            }
        }

        [Test]
        public void TriggerOnlySurface_DoesNotCountAsSupport()
        {
            GameObject floor = null;
            try
            {
                floor = MakeFloor("TriggerFloor", new Vector3(0f, -0.10f, 0f), new Vector3(2f, 0.20f, 2f));
                floor.GetComponent<Collider>().isTrigger = true;
                Physics.SyncTransforms();

                Assert.That(RouteContinuityAuditRules.HasSolidSupport(
                    new Vector3(0f, 0.35f, 0f), 1f), Is.False);
            }
            finally
            {
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }

        private static GameObject MakeFloor(string name, Vector3 position, Vector3 scale)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = name;
            floor.transform.position = position;
            floor.transform.localScale = scale;
            return floor;
        }
    }
}
