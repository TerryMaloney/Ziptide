using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE KEY SOCKET — the first hour's peak, on the hull of the player's own ship.
    ///
    /// Canon (`docs/design/FIRST_HOUR_DIRECTORS_CUT.md` §2.4–2.5): the joined key beacons back to
    /// Cal's own berth, and its fitting matches the coupler she repaired at minute eight. Seating it
    /// lights a destination no chart of hers names — and THEN the Ziptide is allowed to happen.
    ///
    /// This is the piece that makes Terry's complaint go away. `PUNCH IT` out of W000 is now an
    /// ordinary cast-off to a salvage job with no gate FX; the gate is re-armed here, by an object
    /// the player found, joined and seated with their hands. The spectacle is earned instead of spent.
    ///
    /// It owns nothing: `ShipCastOffRuntime` still owns arming and launch, `TravelCoordinator` still
    /// owns travel. This socket changes a destination and sets a flag.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class KeySocketRuntime : MonoBehaviour
    {
        private const float SeatRadius = 0.25f;
        private const float ScanIntervalSeconds = 0.1f;

        [SerializeField] private string keyItemId = ArtifactJoinRuntime.KeyItemId;
        [SerializeField] private string destinationScene = ZiptideConstants.SceneW002;

        private bool _seated;
        private float _nextScanAt;

        /// <summary>Author entry point (public Init — the no-reflection law).</summary>
        public void Configure(string itemId, string destination)
        {
            if (!string.IsNullOrEmpty(itemId)) keyItemId = itemId;
            if (!string.IsNullOrEmpty(destination)) destinationScene = destination;
        }

        private void Start()
        {
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && profile.HasFlag(ZiptideFlags.KEY_SEATED))
            {
                // Already seated on a previous session. Re-arm the route silently — a reload must not
                // quietly send the player back to the salvage lane on a ship that has already crossed.
                _seated = true;
                ArmTheTide(silent: true);
                BuildSocketVisual(lit: true);
                return;
            }

            BuildSocketVisual(lit: false);
        }

        private void Update()
        {
            if (_seated || Time.unscaledTime < _nextScanAt) return;
            _nextScanAt = Time.unscaledTime + ScanIntervalSeconds;

            ItemRuntime key = FindHeldKey();
            if (key == null) return;
            if (Vector3.Distance(key.transform.position, transform.position) > SeatRadius) return;

            Seat(key);
        }

        private ItemRuntime FindHeldKey()
        {
            ItemRuntime[] items = FindObjectsOfType<ItemRuntime>();
            for (int i = 0; i < items.Length; i++)
            {
                ItemRuntime item = items[i];
                if (item == null || item.Definition == null) continue;
                if (item.Definition.itemId != keyItemId) continue;

                var grab = item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                if (grab == null || !grab.isSelected) continue;

                foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor in grab.interactorsSelecting)
                {
                    var hand = interactor as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
                    if (hand != null && hand.isActiveAndEnabled) return item;
                }
            }
            return null;
        }

        private void Seat(ItemRuntime key)
        {
            _seated = true;

            // The key STAYS. The changed ship is the story, not a trophy in a menu — it is mounted on
            // the hull and it glows from here to the end of the game.
            key.transform.SetParent(transform, worldPositionStays: false);
            key.transform.localPosition = Vector3.zero;
            key.transform.localRotation = Quaternion.identity;

            var grab = key.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null) grab.enabled = false;      // seated for good
            var body = key.GetComponent<Rigidbody>();
            if (body != null) body.isKinematic = true;

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.SetFlag(ZiptideFlags.KEY_SEATED);

            BuildSocketVisual(lit: true);
            ArmTheTide(silent: false);

            Debug.Log("ZIPTIDE: KEY_SEATED destination=" + destinationScene);
        }

        /// <summary>
        /// Re-point the ship and give the gate back. Until this runs, `PUNCH IT` is an ordinary
        /// cast-off to the salvage lane with the gate suppressed.
        /// </summary>
        private void ArmTheTide(bool silent)
        {
            // PARENT CHAIN ONLY. A scene-wide search would happily arm a DIFFERENT ship: the moment
            // a world has two berths, the key seated in yours would re-point somebody else's hull.
            // If this socket is not mounted on a launch owner, that is a build error worth hearing
            // about, not something to paper over by grabbing the first ship in the scene.
            var castOff = GetComponentInParent<ShipCastOffRuntime>();
            if (castOff == null)
            {
                Debug.LogWarning("ZIPTIDE: KEY_SEAT_NO_CASTOFF — this socket is not mounted on a ship "
                    + "that owns a launch, so the tide cannot be armed. Check where the author placed it.");
                return;
            }

            castOff.Configure(destinationScene, suppressGate: false);
            if (!silent)
                Debug.Log("ZIPTIDE: ZIPTIDE_ARMED destination=" + destinationScene);
        }

        /// <summary>The socket ring. Dark until the key is in it, then lit forever.</summary>
        private void BuildSocketVisual(bool lit)
        {
            Transform existing = transform.Find("SocketRing");
            if (existing != null) Destroy(existing.gameObject);

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "SocketRing";
            ring.transform.SetParent(transform, false);
            ring.transform.localScale = new Vector3(0.34f, 0.02f, 0.34f);
            var collider = ring.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = ring.GetComponent<Renderer>();
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;

            Color colour = lit ? new Color(0.45f, 0.9f, 0.85f) : new Color(0.22f, 0.24f, 0.27f);
            var material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", colour);
            if (lit && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", colour * 2.4f);
            }
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
