#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class DispatchKioskSpatialContractTests
    {
        [Test]
        public void HowToPlaque_IsCompactAndOffsetTowardThePlayerViewport()
        {
            Assert.LessOrEqual(DispatchKiosk.HowToCharacterSize, 0.0095f,
                "The kiosk instructions must remain compact enough for the Quest viewport.");
            Assert.LessOrEqual(DispatchKiosk.HowToLocalPosition.x, -0.5f,
                "The plaque must sit left of the right-edge kiosk instead of clipping offscreen.");
            Assert.That(DispatchKiosk.HowToLocalPosition.y, Is.InRange(1.0f, 1.5f),
                "The plaque must remain readable without floating above the player view.");
        }

        [Test]
        public void HowToPlaque_NegativeZFacesTheViewer()
        {
            GameObject sign = new GameObject("KioskSignFacingTest");
            try
            {
                sign.transform.position = new Vector3(2f, 1.25f, 3f);
                Vector3 viewer = new Vector3(-1f, 1.6f, -2f);

                KioskHowToSignRuntime.FaceViewer(sign.transform, viewer);

                Vector3 towardViewer = (viewer - sign.transform.position).normalized;
                Assert.Greater(Vector3.Dot(-sign.transform.forward, towardViewer), 0.999f,
                    "Legacy TextMesh reads from -Z, which must face the active player camera.");
            }
            finally
            {
                Object.DestroyImmediate(sign);
            }
        }
    }
}
#endif
