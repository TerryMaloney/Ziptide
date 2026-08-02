using UnityEngine;


namespace Ziptide.Gameplay
{
    /// <summary>Finds data-spawned repair parts after JobDirector builds them and attaches the safety rail.</summary>
    public sealed class RepairPartSafetyInstallerRuntime : MonoBehaviour
    {
        private static RepairPartSafetyInstallerRuntime _instance;
        private float _nextScan;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstaller()
        {
            if (_instance != null) return;
            GameObject go = new GameObject("__RepairPartSafetyInstaller");
            Object.DontDestroyOnLoad(go);
            _instance = go.AddComponent<RepairPartSafetyInstallerRuntime>();
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextScan) return;
            _nextScan = Time.unscaledTime + 0.5f;

            RepairableMachine[] machines = Object.FindObjectsOfType<RepairableMachine>(true);
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] grabs = Object.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(true);
            for (int i = 0; i < grabs.Length; i++)
            {
                UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = grabs[i];
                if (grab == null || !grab.gameObject.name.StartsWith("Part_")) continue;
                if (grab.GetComponent<RepairPartSafetyRuntime>() != null) continue;

                RepairableMachine nearest = null;
                float best = float.MaxValue;
                for (int j = 0; j < machines.Length; j++)
                {
                    RepairableMachine machine = machines[j];
                    if (machine == null) continue;
                    float d = Vector3.SqrMagnitude(grab.transform.position - machine.transform.position);
                    if (d < best) { best = d; nearest = machine; }
                }
                if (nearest == null || best > 36f) continue;

                RepairPartSafetyRuntime safety = grab.gameObject.AddComponent<RepairPartSafetyRuntime>();
                safety.Configure(nearest);
            }
        }
    }
}
