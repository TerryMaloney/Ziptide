using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Tiny always-on credits readout so the economy is visible ("I don't see the economy anywhere").
    /// Lives on the persistent rig (ensured by <see cref="PlayerRigPersistence"/> like the stun receiver),
    /// so it shows in every world. Reads the live profile's "credits" resource — the same id the ToxicCity
    /// bounty pays into — and billboards a small TextMesh in the lower-left of the view. Built from TextMesh
    /// (not TMP) to match the rest of the HUDs and avoid the TMP import issues.
    /// </summary>
    public class CreditsHud : MonoBehaviour
    {
        // Same id the bounty/jobs grant (ToxicCityContractBuilder.RewardResourceId, JobRewards.Grant).
        private const string CreditsResourceId = "credits";
        private const float RefreshInterval = 0.5f;

        private Transform _cam;
        private TextMesh _text;
        private float _nextRefresh;
        private long _shown = long.MinValue;

        private void Start()
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _cam = rig.GetComponentInChildren<Camera>()?.transform;
            if (_cam == null && Camera.main != null) _cam = Camera.main.transform;
            BuildText();
        }

        private void BuildText()
        {
            var go = new GameObject("CreditsHudText");
            go.transform.SetParent(transform, false);
            _text = go.AddComponent<TextMesh>();
            _text.characterSize = 0.018f;
            _text.fontSize = 64;
            _text.anchor = TextAnchor.MiddleLeft;
            _text.alignment = TextAlignment.Left;
            _text.color = new Color(1f, 0.86f, 0.35f); // credit-gold
            _text.text = "CR 0";
        }

        private void Update()
        {
            if (_cam == null)
            {
                if (Camera.main != null) _cam = Camera.main.transform; else return;
            }
            if (_text == null) return;

            // Lower-left of the comfortable FOV, billboarded so it tracks the gaze without drifting offscreen.
            _text.transform.position = _cam.position + _cam.forward * 0.9f - _cam.up * 0.30f - _cam.right * 0.34f;
            _text.transform.rotation = Quaternion.LookRotation(_text.transform.position - _cam.position);

            if (Time.unscaledTime < _nextRefresh) return;
            _nextRefresh = Time.unscaledTime + RefreshInterval;

            long credits = 0;
            var save = SaveSystem.Instance;
            if (save != null && save.Profile != null)
                credits = (long)System.Math.Floor(save.Profile.GetResource(CreditsResourceId));

            if (credits != _shown)
            {
                _shown = credits;
                _text.text = "CR " + credits.ToString("N0");
            }
        }
    }
}
