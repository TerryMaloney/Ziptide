using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryPersistentDiagnosticRingTests
    {
        [UnityTest]
        public IEnumerator InstalledRing_PersistsAnActualZiptideTag()
        {
            PersistentDiagnosticRing.ResetSubscription();
            PersistentDiagnosticRing.Install();

            string token = "ZIPTIDE: DIAG_RING_TEST frame=" + Time.frameCount;
            Debug.Log(token);
            yield return null;

            Assert.IsFalse(string.IsNullOrEmpty(PersistentDiagnosticRing.CurrentPath),
                "The installed diagnostic ring did not resolve its persistent path.");

            bool found = Contains(PersistentDiagnosticRing.CurrentPath, token)
                || Contains(PersistentDiagnosticRing.PreviousPath, token);
            Assert.IsTrue(found,
                "A live ZIPTIDE tag was not recoverable from either bounded ring segment.");
        }

        private static bool Contains(string path, string token)
        {
            return !string.IsNullOrEmpty(path)
                && File.Exists(path)
                && File.ReadAllText(path).Contains(token);
        }
    }
}
