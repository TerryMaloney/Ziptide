// ─────────────────────────────────────────────────────────────────────────────────────────────
// PHOTON ADAPTER (A6-prep — docs/TWO_QUEST_SETUP.md is the activation guide)
//
// This file lives OUTSIDE the Ziptide asmdefs on purpose: with no .asmdef above it, it compiles
// into Assembly-CSharp — the SAME assembly PUN2 lands in when imported — and it can still see
// Ziptide.Multiplayer (autoReferenced). So: no Photon asmdef surgery, no dependency from our
// assemblies to Photon, and gameplay only ever talks to IPvpTransport via PvpNetHub.
//
// Until Terry (1) imports PUN2 and (2) runs  Ziptide → Net → Enable Photon (ZIPTIDE_PHOTON) ,
// everything below is inert text — CI compiles this project without Photon present.
// First activation is an operator+Terry pairing task (A6): expect an editor compile pass on this
// file with PUN2 actually installed before it reaches a headset.
// ─────────────────────────────────────────────────────────────────────────────────────────────
#if ZIPTIDE_PHOTON
using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Ziptide.Multiplayer;

namespace ZiptideNet
{
    /// <summary>
    /// IPvpTransport over PUN2 RaiseEvent (one byte code per message kind, payload = object[]).
    /// Host = Photon MasterClient. Registered with <see cref="PvpNetHub"/> by <see cref="PhotonPvpLauncher"/>.
    /// </summary>
    public sealed class PhotonPvpTransport : IPvpTransport, IOnEventCallback
    {
        private const byte EvPose = 11, EvFire = 12, EvHit = 13, EvScore = 14, EvWall = 15;

        public PvpNetRole Role => PhotonNetwork.IsMasterClient ? PvpNetRole.Host : PvpNetRole.Client;
        public bool IsHost => PhotonNetwork.IsMasterClient;
        public int LocalPlayerId => PhotonNetwork.IsMasterClient ? 0 : 1;

        public event Action<PlayerPoseMsg> OnPose;
        public event Action<FireMsg> OnFire;
        public event Action<HitMsg> OnHit;
        public event Action<ScoreMsg> OnScore;
        public event Action<WallMsg> OnWall;

        private static readonly RaiseEventOptions ToOthers = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        private static readonly SendOptions Unreliable = new SendOptions { Reliability = false };
        private static readonly SendOptions Reliable = new SendOptions { Reliability = true };

        public void Enable() => PhotonNetwork.AddCallbackTarget(this);
        public void Disable() => PhotonNetwork.RemoveCallbackTarget(this);

        public void SendPose(PlayerPoseMsg m) => PhotonNetwork.RaiseEvent(EvPose, new object[]
        {
            m.playerId, m.headPos, m.headRot, m.lHandPos, m.lHandRot, m.rHandPos, m.rHandRot, m.heldWeapon
        }, ToOthers, Unreliable);

        public void SendFire(FireMsg m) => PhotonNetwork.RaiseEvent(EvFire, new object[]
        {
            m.shooterId, (int)m.weapon, m.origin, m.direction
        }, ToOthers, Reliable);

        public void SendHit(HitMsg m) => PhotonNetwork.RaiseEvent(EvHit, new object[]
        {
            m.attackerId, m.targetId, (int)m.weapon, m.amount, m.killed
        }, ToOthers, Reliable);

        public void SendScore(ScoreMsg m) => PhotonNetwork.RaiseEvent(EvScore, new object[]
        {
            m.score0, m.score1, (int)m.phase, m.winnerIndex
        }, ToOthers, Reliable);

        public void SendWall(WallMsg m) => PhotonNetwork.RaiseEvent(EvWall, new object[]
        {
            m.panelId, m.state
        }, ToOthers, Reliable);

        public void OnEvent(EventData e)
        {
            var d = e.CustomData as object[];
            if (d == null) return;
            switch (e.Code)
            {
                case EvPose:
                    OnPose?.Invoke(new PlayerPoseMsg
                    {
                        playerId = (int)d[0],
                        headPos = (Vector3)d[1], headRot = (Quaternion)d[2],
                        lHandPos = (Vector3)d[3], lHandRot = (Quaternion)d[4],
                        rHandPos = (Vector3)d[5], rHandRot = (Quaternion)d[6],
                        heldWeapon = (int)d[7],
                    });
                    break;
                case EvFire:
                    OnFire?.Invoke(new FireMsg
                    {
                        shooterId = (int)d[0], weapon = (PvpWeapon)(int)d[1],
                        origin = (Vector3)d[2], direction = (Vector3)d[3],
                    });
                    break;
                case EvHit:
                    OnHit?.Invoke(new HitMsg
                    {
                        attackerId = (int)d[0], targetId = (int)d[1],
                        weapon = (PvpWeapon)(int)d[2], amount = (int)d[3], killed = (bool)d[4],
                    });
                    break;
                case EvScore:
                    OnScore?.Invoke(new ScoreMsg
                    {
                        score0 = (int)d[0], score1 = (int)d[1],
                        phase = (PvpPhase)(int)d[2], winnerIndex = (int)d[3],
                    });
                    break;
                case EvWall:
                    OnWall?.Invoke(new WallMsg { panelId = (int)d[0], state = (int)d[1] });
                    break;
            }
        }
    }

    /// <summary>
    /// Room-code session bootstrap: connect → JoinOrCreateRoom("ZIP" + code, max 2) → register the
    /// transport with PvpNetHub. Drop on a GameObject in the arena (A6 wires it to the lobby board;
    /// for the first two-headset smoke both devices use the same hardcoded room code).
    /// </summary>
    public class PhotonPvpLauncher : MonoBehaviourPunCallbacks
    {
        [Tooltip("Both headsets must use the same code. A6 lets the lobby board set it.")]
        public string roomCode = "ZIP-001";

        private PhotonPvpTransport _transport;

        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings(); // reads PhotonServerSettings (App ID from the wizard)
                Debug.Log("ZIPTIDE: NET_CONNECTING");
            }
        }

        public override void OnConnectedToMaster()
        {
            PvpNetHub.Status = "joining " + roomCode + "…";
            Debug.Log("ZIPTIDE: NET_MASTER_OK region=" + PhotonNetwork.CloudRegion);
            PhotonNetwork.JoinOrCreateRoom(roomCode,
                new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            _transport = new PhotonPvpTransport();
            _transport.Enable();
            PvpNetHub.SetTransport(_transport);
            PvpNetHub.Status = "in " + roomCode + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2)";
            Debug.Log("ZIPTIDE: NET_ROOM_JOINED room=" + roomCode
                + " players=" + PhotonNetwork.CurrentRoom.PlayerCount
                + " host=" + PhotonNetwork.IsMasterClient);
        }

        // Both callbacks refresh the "x/2" count as the other headset comes and goes.
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.InRoom)
                PvpNetHub.Status = "in " + roomCode + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2)";
            Debug.Log("ZIPTIDE: NET_PLAYER_JOINED players=" + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.InRoom)
                PvpNetHub.Status = "in " + roomCode + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2)";
            Debug.Log("ZIPTIDE: NET_PLAYER_LEFT players=" + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (_transport != null) { _transport.Disable(); _transport = null; }
            PvpNetHub.SetTransport(null); // back to loopback — solo keeps working
            PvpNetHub.Status = "net error: " + cause;
            Debug.Log("ZIPTIDE: NET_DISCONNECTED cause=" + cause);
        }
    }
}
#endif
