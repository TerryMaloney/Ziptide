#if UNITY_EDITOR
namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Source-equivalent proof marker. A concurrent design-only commit advanced terry-local-wip while
    /// the 2e8a9e4 Unity lanes were running, so the freshness recorder correctly refused to write a
    /// verdict for a non-head branch. This test-assembly-only marker creates one new exact source SHA
    /// after that docs advance without changing runtime behavior or the headset APK.
    /// </summary>
    internal static class RecoveryProofRefreshMarker
    {
        public const int Revision = 1;
    }
}
#endif
