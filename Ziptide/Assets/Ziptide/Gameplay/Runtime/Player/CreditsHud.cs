using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Legacy always-on credits readout. FullDevelopment may retain it while systems migrate, but
    /// recovery GoldenSlice/Diagnostic builds fail closed through RecoveryPlayerSurfacePolicy so
    /// the intrusive yellow CR 0 cannot contaminate the checkpoint view.
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

        private void Awake()
        {
            if (RecoveryPlayerSurfacePolicy.Allows(RecoveryPlayerSurfaceId.CreditsHud)) return;

            enabled = false;
            Debug.Log("ZIPTIDE: PLAYER_SURFACE_BLOCKED id=CreditsHud profile="
                + RecoveryRuntimeGate.ActiveProfileName);
        }

        private void Start()
        {
            // Awake disables this component before Start in recovery profiles. Re-check here so a
            // manually enabled scene-authored component still cannot bypass the central policy.
            if (!RecoveryPlayerSurfacePolicy.Allows(RecoveryPlayerSurfaceId.CreditsHud))
            {
                enabled = false;
                return;
            }

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
            _text.characterSize = 0.01f;
            _text.fontSize = 64;
            _text.anchor = TextAnchor.MiddleLeft;
            _text.alignment = TextAlignment.Left;
            _text.color = new Color(1f, 0.86f, 0.35f);
            _text.text = "CR 0";
        }

        private void Update()
        {
            if (!RecoveryPlayerSurfacePolicy.Allows(RecoveryPlayerSurfaceId.CreditsHud))
            {
                enabled = false;
                return;
            }

            if (_cam == null)
            {
                if (Camera.main != null) _cam = Camera.main.transform; else return;
            }
            if (_text == null) return;

            _text.transform.position = _cam.position + _cam.forward * 0.8f
                - _cam.up * 0.36f - _cam.right * 0.46f;
            _text.transform.rotation = Quaternion.LookRotation(
                _text.transform.position - _cam.position);

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
