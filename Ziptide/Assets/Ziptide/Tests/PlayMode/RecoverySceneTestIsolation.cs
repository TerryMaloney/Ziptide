using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Test-only fresh-process approximation for actual-scene recovery tests. Unity's PlayMode test
    /// player starts once, so RuntimeInitializeOnLoadMethod hooks have already executed before a
    /// test switches to GoldenSlice. This helper removes all cataloged production owners, selects
    /// GoldenSlice, then invokes only the allowed bootstraps at their real Before/AfterSceneLoad
    /// phases. The scene and production code remain unchanged.
    /// </summary>
    public static class RecoverySceneTestIsolation
    {
        public const string EmptySceneName = "__RECOVERY_TEST_EMPTY";

        public static IEnumerator PrepareFreshGoldenBoot()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            DestroyProductionRuntimeImmediate();
            yield return null;
            InvokeAllowedBootstraps(RuntimeInitializeLoadType.BeforeSceneLoad);
        }

        public static void InvokeAllowedAfterSceneLoadBootstraps()
        {
            InvokeAllowedBootstraps(RuntimeInitializeLoadType.AfterSceneLoad);
        }

        public static IEnumerator ResetToEmptyFullDevelopment()
        {
            Scene empty = SceneManager.GetSceneByName(EmptySceneName);
            if (!empty.IsValid()) empty = SceneManager.CreateScene(EmptySceneName);
            SceneManager.SetActiveScene(empty);

            var unload = new List<AsyncOperation>();
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || scene == empty || !scene.isLoaded) continue;
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if (operation != null) unload.Add(operation);
            }
            for (int i = 0; i < unload.Count; i++)
                while (!unload[i].isDone) yield return null;

            DestroyProductionRuntimeImmediate();
            PersistentDiagnosticRing.ResetSubscription();
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        public static void DestroyProductionRuntimeImmediate()
        {
            var ownerSymbols = new HashSet<string>(StringComparer.Ordinal);
            IReadOnlyList<RecoveryAutomaticOwnerRegistration> catalog = RecoveryAutomaticOwnerCatalog.All;
            for (int i = 0; i < catalog.Count; i++) ownerSymbols.Add(catalog[i].Symbol);

            var destroy = new HashSet<GameObject>();
            MonoBehaviour[] behaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || !behaviour.gameObject.scene.IsValid()) continue;
                string symbol = behaviour.GetType().FullName;
                if (ownerSymbols.Contains(symbol) ||
                    behaviour is BootLoader ||
                    behaviour is HomeHubRuntime ||
                    behaviour is CreditsHud)
                {
                    destroy.Add(behaviour.gameObject);
                }
            }

            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go == null || !go.scene.IsValid()) continue;
                if (go.name == "CreditsHudText" ||
                    go.name == "DevWarpBoard" ||
                    go.name == "DevMenuCanvas" ||
                    go.name == "__HOME_HUB_COMFORT_SETTINGS")
                {
                    destroy.Add(go);
                }
            }

            foreach (GameObject go in destroy)
                if (go != null) UnityEngine.Object.DestroyImmediate(go);
        }

        private static void InvokeAllowedBootstraps(RuntimeInitializeLoadType phase)
        {
            IReadOnlyList<RecoveryAutomaticOwnerRegistration> catalog = RecoveryAutomaticOwnerCatalog.All;
            for (int i = 0; i < catalog.Count; i++)
            {
                RecoveryAutomaticOwnerRegistration registration = catalog[i];
                if (!RecoveryRuntimeGate.Allows(registration.FeatureId)) continue;

                Type type = ResolveType(registration.Symbol);
                if (type == null)
                    throw new InvalidOperationException(
                        "RECOVERY_BOOTSTRAP_TYPE_MISSING owner=" + registration.OwnerId +
                        " symbol=" + registration.Symbol);

                MethodInfo[] methods = type.GetMethods(
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int m = 0; m < methods.Length; m++)
                {
                    MethodInfo method = methods[m];
                    object[] attributes = method.GetCustomAttributes(
                        typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    for (int a = 0; a < attributes.Length; a++)
                    {
                        var attribute = (RuntimeInitializeOnLoadMethodAttribute)attributes[a];
                        if (attribute.loadType != phase) continue;
                        if (method.GetParameters().Length != 0)
                            throw new InvalidOperationException(
                                "RECOVERY_BOOTSTRAP_HAS_PARAMETERS owner=" + registration.OwnerId +
                                " method=" + method.Name);
                        try
                        {
                            method.Invoke(null, null);
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw new InvalidOperationException(
                                "RECOVERY_BOOTSTRAP_FAILED owner=" + registration.OwnerId +
                                " method=" + method.Name + " phase=" + phase,
                                ex.InnerException ?? ex);
                        }
                    }
                }
            }
        }

        private static Type ResolveType(string fullName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type type = assemblies[i].GetType(fullName, false);
                if (type != null) return type;
            }
            return null;
        }
    }
}
