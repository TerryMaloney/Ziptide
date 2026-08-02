using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE JOIN — the first hour's chills beat, and the only genuinely new mechanic the arc needs.
    ///
    /// Cal holds half an artifact in each hand. Bring them within reach of each other and they PULL:
    /// a magnetic snap, strong haptics, a deep clunk, and the two halves become one key. Canon:
    /// `docs/design/FIRST_HOUR_DIRECTORS_CUT.md` §2.3.
    ///
    /// Design notes that matter more than the code:
    ///  • The pull starts BEFORE the snap. A socket that only reacts at the moment of contact feels
    ///    like a checkbox; one that reaches for you feels alive. That is the whole beat.
    ///  • Both halves must be HELD, one per hand. Dropping them next to each other on the floor must
    ///    not do it — the story is that Cal joins them, not that they find each other.
    ///  • It fires once. `ARTIFACT_JOINED` latches in the profile, so a reload cannot un-join a key.
    ///
    /// It owns no inventory, no travel and no progression: it swaps two item instances for one and
    /// sets a flag. Everything downstream reads the flag.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtifactJoinRuntime : MonoBehaviour
    {
        public const string HalfAItemId = "artifact_half_a";
        public const string HalfBItemId = "artifact_half_b";
        public const string KeyItemId = "artifact_key";

        /// <summary>Where the halves begin reaching for each other.</summary>
        private const float PullRadius = 0.45f;
        /// <summary>Where they commit. The canon's number.</summary>
        private const float SnapRadius = 0.20f;
        private const float PullStrength = 2.6f;
        private const float ScanIntervalSeconds = 0.1f;

        public static ArtifactJoinRuntime Instance { get; private set; }

        /// <summary>Raised once, after the key exists. The first-hour director listens.</summary>
        public static event System.Action Joined;

        private float _nextScanAt;
        private bool _joined;
        private RillCompanion _spineRill;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null || FindObjectOfType<ArtifactJoinRuntime>() != null) return;
            var go = new GameObject("__ArtifactJoin");
            DontDestroyOnLoad(go);
            go.AddComponent<ArtifactJoinRuntime>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // A key already joined on a previous session must never re-join. The flag is the truth.
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            _joined = profile != null && profile.HasFlag(ZiptideFlags.ARTIFACT_JOINED);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (_joined || Time.unscaledTime < _nextScanAt) return;
            _nextScanAt = Time.unscaledTime + ScanIntervalSeconds;

            // Cheap gate first. Until the player actually HAS both halves this can never fire, and
            // there is no reason to sweep every item in the world ten times a second to prove it.
            if (!HasBothHalves()) return;

            ItemRuntime halfA = FindHeldHalf(HalfAItemId);
            ItemRuntime halfB = FindHeldHalf(HalfBItemId);
            if (halfA == null || halfB == null)
            {
                // THE HESITATION HINT. Owning both halves and never discovering that they join is
                // the single worst way this hour can end — the peak beat sitting unreachable in a
                // holster because nobody said "one in each hand". Told the project's way: only
                // after the player has carried both for a while without working it out, so a player
                // who simply does it hears nothing at all.
                NudgeTowardTheJoin();
                return;
            }
            _hintDeadline = 0f;

            Vector3 a = halfA.transform.position;
            Vector3 b = halfB.transform.position;
            float distance = Vector3.Distance(a, b);
            if (distance > PullRadius) return;

            if (distance > SnapRadius)
            {
                // THE REACH. Each half leans toward the other, harder as they close. This is a
                // presentation nudge on a held object, never a move of the player.
                float force = PullStrength * (1f - Mathf.InverseLerp(SnapRadius, PullRadius, distance));
                Vector3 toward = (b - a).normalized * force * Time.unscaledDeltaTime;
                Nudge(halfA, toward);
                Nudge(halfB, -toward);
                return;
            }

            Join(halfA, halfB, Vector3.Lerp(a, b, 0.5f));
        }

        /// <summary>Seconds of carrying both halves un-joined before RILL says the quiet part.</summary>
        private const float HintAfterSeconds = 25f;

        private float _hintDeadline;
        private bool _hinted;

        /// <summary>
        /// Waits, then hints once. The teaching law here is the same one the tutorial cues follow:
        /// the game waits rather than narrates, so competence is never interrupted — but it does
        /// eventually speak, because a beat nobody can find is not a beat.
        /// </summary>
        private void NudgeTowardTheJoin()
        {
            if (_hinted) return;
            if (_hintDeadline <= 0f)
            {
                _hintDeadline = Time.unscaledTime + HintAfterSeconds;
                return;
            }
            if (Time.unscaledTime < _hintDeadline) return;

            _hinted = true;
            var rill = FindObjectOfType<RillCompanion>();
            if (rill != null) rill.SayById("ARTIFACT_JOIN_HINT");
            Debug.Log("ZIPTIDE: ARTIFACT_JOIN_HINT both halves carried, not joined");
        }

        /// <summary>
        /// Both halves collected, per the profile. This is a flag read, not a scene search.
        /// </summary>
        private static bool HasBothHalves()
        {
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return false;
            return profile.HasFlag(ZiptideFlags.ARTIFACT_HALF_A)
                && profile.HasFlag(ZiptideFlags.ARTIFACT_HALF_B);
        }

        /// <summary>
        /// Only a half that is actually IN A HAND counts. Socket selection (a holster) does not, for
        /// the same reason the laser sight ignores it: an item on your belt is carried, not held.
        /// </summary>
        private static ItemRuntime FindHeldHalf(string itemId)
        {
            ItemRuntime[] items = FindObjectsOfType<ItemRuntime>();
            for (int i = 0; i < items.Length; i++)
            {
                ItemRuntime item = items[i];
                if (item == null || item.Definition == null) continue;
                if (item.Definition.itemId != itemId) continue;

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

        private static void Nudge(ItemRuntime item, Vector3 delta)
        {
            if (item == null) return;
            item.transform.position += delta;
        }

        private void Join(ItemRuntime halfA, ItemRuntime halfB, Vector3 where)
        {
            _joined = true;

            Haptics(halfA);
            Haptics(halfB);

            Destroy(halfA.gameObject);
            Destroy(halfB.gameObject);

            GameObject key = ItemFactory.Create(KeyItemId, where);
            if (key == null)
                Debug.LogWarning("ZIPTIDE: ARTIFACT_JOIN_NO_KEY id=" + KeyItemId
                    + " — the halves were consumed but no key item exists. Check the item registry.");

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.SetFlag(ZiptideFlags.ARTIFACT_JOINED);

            Debug.Log("ZIPTIDE: ARTIFACT_JOINED at=" + where.ToString("F2"));

            // THEMATIC_SPINE §3.1 — reads as puzzle flavour now, and as the Debugger splitting her own
            // key so one person alone could not undo it, later.
            // Cached, not looked up: Join() is reachable from Update, and frame_cost_gate flagged the
            // naive version the moment it was written. Caught by the gate added an hour earlier.
            if (_spineRill == null) _spineRill = FindObjectOfType<RillCompanion>();
            if (_spineRill != null) _spineRill.SayById("SPINE_HALVES_JOINED");

            var handler = Joined;
            if (handler == null) return;
            foreach (System.Delegate subscriber in handler.GetInvocationList())
            {
                try { ((System.Action)subscriber)(); }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: ARTIFACT_JOIN_LISTENER_FAIL error=" + ex.Message);
                }
            }
        }

        /// <summary>A join you cannot feel is a join that did not happen.</summary>
        private static void Haptics(ItemRuntime item)
        {
            var grab = item != null ? item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() : null;
            if (grab == null) return;
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor in grab.interactorsSelecting)
            {
                var hand = interactor as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
                if (hand != null) hand.SendHapticImpulse(0.9f, 0.22f);
            }
        }
    }
}
