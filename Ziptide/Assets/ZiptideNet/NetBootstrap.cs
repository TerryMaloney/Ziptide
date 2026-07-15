// ─────────────────────────────────────────────────────────────────────────────────────────────
// NET BOOTSTRAP (A6) — installs the "go online" hooks into PvpNetHub at startup.
//
// Lives in Assembly-CSharp (no .asmdef above ZiptideNet), so it can both see Photon AND reach
// into Ziptide.Multiplayer's PvpNetHub. Gameplay stays Photon-free: the arena lobby calls
// PvpNetHub.StartOnline(code); that invokes the starter installed here, which spawns the
// PhotonPvpLauncher. Inert text until Terry runs Ziptide → Net → Enable Photon (ZIPTIDE_PHOTON).
// ─────────────────────────────────────────────────────────────────────────────────────────────
#if ZIPTIDE_PHOTON
using UnityEngine;
using Ziptide.Core;
using Ziptide.Multiplayer;

namespace ZiptideNet
{
    public static class NetBootstrap
    {
        private static GameObject _launcherGo;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.NetBootstrap))
            {
                PvpNetHub.OnlineStarter = null;
                PvpNetHub.OnlineStopper = null;
                return;
            }

            PvpNetHub.OnlineStarter = StartOnline;
            PvpNetHub.OnlineStopper = StopOnline;
            Debug.Log("ZIPTIDE: NET_STARTER_INSTALLED (Photon)");
        }

        private static bool StartOnline(string roomCode)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.NetBootstrap)) return false;

            if (_launcherGo == null)
            {
                _launcherGo = new GameObject("__PhotonPvpLauncher");
                Object.DontDestroyOnLoad(_launcherGo);
                var launcher = _launcherGo.AddComponent<PhotonPvpLauncher>();
                launcher.roomCode = string.IsNullOrEmpty(roomCode) ? "ZIP-001" : roomCode;
            }
            return true;
        }

        private static void StopOnline()
        {
            if (_launcherGo != null)
            {
                Object.Destroy(_launcherGo);
                _launcherGo = null;
            }
            if (Photon.Pun.PhotonNetwork.IsConnected) Photon.Pun.PhotonNetwork.Disconnect();
        }
    }
}
#endif
