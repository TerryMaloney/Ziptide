#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class AudioDirectorLifecycleTests
    {
        private GameObject _object;
        private AudioClip _clip;

        [TearDown]
        public void TearDown()
        {
            if (_clip != null) Object.DestroyImmediate(_clip);
            if (_object != null) Object.DestroyImmediate(_object);
            _clip = null;
            _object = null;
        }

        [Test]
        public void StopAndClear_ReleasesClipReferenceAndResetsVolume()
        {
            _object = new GameObject("AudioDirectorLifecycleTest");
            var source = _object.AddComponent<AudioSource>();
            _clip = AudioClip.Create("AudioDirectorLifecycleClip", 16, 1, 8000, false);
            source.clip = _clip;
            source.volume = 0.73f;

            MethodInfo method = typeof(AudioDirector).GetMethod(
                "StopAndClear",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);
            Assert.DoesNotThrow(() => method.Invoke(null, new object[] { source }));

            Assert.IsNull(source.clip, "stopped persistent sources must not pin old clips in memory");
            Assert.AreEqual(0f, source.volume, 0.0001f);
        }

        [Test]
        public void StopAndClear_IsNullSafe()
        {
            MethodInfo method = typeof(AudioDirector).GetMethod(
                "StopAndClear",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);
            Assert.DoesNotThrow(() => method.Invoke(null, new object[] { null }));
        }

        [Test]
        public void RuntimeSource_PreservesSingletonSubscriptionAndTracksOneTransition()
        {
            string source = ReadRuntimeSource();

            Assert.AreEqual(1, Count(source, "SceneManager.sceneLoaded += OnSceneLoaded;"));
            Assert.AreEqual(1, Count(source, "SceneManager.sceneLoaded -= OnSceneLoaded;"));
            StringAssert.Contains("private Coroutine _transitionRoutine;", source);
            StringAssert.DoesNotContain("_fadeTimer", source);
            StringAssert.DoesNotContain("_fading", source);
        }

        [Test]
        public void RuntimeSource_CancelsBeforeReuseAndClearsEveryRetiredSource()
        {
            string source = ReadRuntimeSource();
            int apply = source.IndexOf("private void ApplyProfile");
            int cancel = source.IndexOf("CancelTransition();", apply);
            int clearNext = source.IndexOf("StopAndClear(next);", cancel);
            int assignNext = source.IndexOf("next.clip = profile.clip;", clearNext);

            Assert.GreaterOrEqual(apply, 0);
            Assert.Greater(cancel, apply);
            Assert.Greater(clearNext, cancel);
            Assert.Greater(assignNext, clearNext,
                "inactive source must release its previous clip before accepting the new one");

            int crossfade = source.IndexOf("private IEnumerator Crossfade");
            int crossfadeClear = source.IndexOf("StopAndClear(fadeOut);", crossfade);
            Assert.Greater(crossfadeClear, crossfade,
                "completed crossfade must release the retired clip reference");

            int fadeOut = source.IndexOf("private void FadeOut()");
            int clearInactive = source.IndexOf("StopAndClear(inactive);", fadeOut);
            Assert.Greater(clearInactive, fadeOut,
                "silent profiles must immediately release the non-active source");

            int fadeCoroutine = source.IndexOf("private IEnumerator FadeOutCoroutine");
            int clearActive = source.IndexOf("StopAndClear(source);", fadeCoroutine);
            Assert.Greater(clearActive, fadeCoroutine,
                "silent-profile fade completion must release the active clip reference");

            int destroy = source.IndexOf("private void OnDestroy()");
            Assert.Greater(source.IndexOf("StopAndClear(_sourceA);", destroy), destroy);
            Assert.Greater(source.IndexOf("StopAndClear(_sourceB);", destroy), destroy);
            StringAssert.Contains("source.clip = null;", source);
        }

        private static string ReadRuntimeSource()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Audio",
                "AudioDirector.cs");
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
