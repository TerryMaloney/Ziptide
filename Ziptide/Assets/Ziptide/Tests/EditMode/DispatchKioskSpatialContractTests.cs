#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class DispatchKioskSpatialContractTests
    {
        [Test]
        public void HowToPlaque_IsCompactInsideRightEdgeAndAboveSubtitleBand()
        {
            Assert.LessOrEqual(DispatchKiosk.HowToCharacterSize, 0.0095f,
                "The kiosk instructions must remain compact enough for the Quest viewport.");
            Assert.LessOrEqual(DispatchKiosk.HowToLocalPosition.x, -0.8f,
                "The plaque must sit far enough inside the right edge to avoid clipping.");
            Assert.That(DispatchKiosk.HowToLocalPosition.y, Is.InRange(1.65f, 1.95f),
                "The plaque must remain above the opening RILL subtitle band without floating out of view.");
        }

        [Test]
        public void HowToPlaque_NegativeZFacesTheViewer()
        {
            GameObject sign = new GameObject("KioskSignFacingTest");
            try
            {
                sign.transform.position = new Vector3(2f, 1.8f, 3f);
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
