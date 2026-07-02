using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A4: weapon pads are TIMED RESPAWNERS (the arena-shooter map-control classic). The pad spawns its
    /// item at round start; once the item is TAKEN (grabbed or carried off the pad) a respawn timer
    /// arms, and a fresh copy appears after <see cref="respawnSeconds"/>. One live item per pad; taken
    /// copies stay in the world (arena litter is honest). The patcher bakes the pad slab + this
    /// component from ArenaLayoutDefinition.weaponPads — item creation is all runtime now, so restarts
    /// and respawns share one path.
    /// </summary>
    public class WeaponPadRuntime : MonoBehaviour
    {
        public string itemId = "taser_dart_gun";
        public float respawnSeconds = 20f;

        private GameObject _live;
        private float _respawnAt = -1f;
        private const float TakenDistance = 1.2f;

        private void Start()
        {
            SpawnItem();
        }

        private void Update()
        {
            if (_live != null)
            {
                bool taken = IsHeld(_live)
                    || Vector3.Distance(_live.transform.position, ItemPos()) > TakenDistance;
                if (taken)
                {
                    _live = null; // it's theirs now — the pad forgets it
                    _respawnAt = Time.time + Mathf.Max(2f, respawnSeconds);
                    Debug.Log("ZIPTIDE: PAD_TAKEN item=" + itemId);
                }
            }
            else if (_respawnAt > 0f && Time.time >= _respawnAt)
            {
                SpawnItem();
            }
        }

        private void SpawnItem()
        {
            _respawnAt = -1f;
            _live = ItemFactory.Create(itemId, ItemPos());
            if (_live == null) return;
            if (itemId == "gravity_gun" && _live.GetComponent<PvpComfortHop>() == null)
                _live.AddComponent<PvpComfortHop>(); // the arena movement verb rides the gravity gun
            Debug.Log("ZIPTIDE: PAD_SPAWN item=" + itemId);
        }

        private Vector3 ItemPos() => transform.position + Vector3.up * 1.0f;

        private static bool IsHeld(GameObject go)
        {
            var grab = go.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            return grab != null && grab.isSelected;
        }
    }
}
