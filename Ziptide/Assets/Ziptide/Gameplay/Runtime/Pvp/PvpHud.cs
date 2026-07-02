using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Minimal PvP HUD: health + score floating in the lower view. Built from a TextMesh (avoids the TMP
    /// import issues that have bitten the dev menu) and billboarded to the camera each frame. Scene-scoped.
    /// </summary>
    public class PvpHud : MonoBehaviour
    {
        private Transform _cam;
        private TextMesh _text;
        private PvpPlayer _player;

        private void Start()
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _cam = rig.GetComponentInChildren<Camera>()?.transform;
            if (_cam == null && Camera.main != null) _cam = Camera.main.transform;
            _player = FindObjectOfType<PvpPlayer>();
            BuildText();
        }

        private void BuildText()
        {
            var go = new GameObject("PvpHudText");
            go.transform.SetParent(transform, false);
            _text = go.AddComponent<TextMesh>();
            _text.characterSize = 0.02f;
            _text.fontSize = 64;
            _text.anchor = TextAnchor.MiddleCenter;
            _text.alignment = TextAlignment.Center;
            _text.color = new Color(0.85f, 0.95f, 1f);
        }

        private void Update()
        {
            if (_cam == null || _text == null) return;

            // Panel sits centered and only slightly below the gaze so it stays comfortably readable inside
            // the FOV (it was forward*1.0 - up*0.5 ≈ 26° down, which drifted out of view at the bottom).
            _text.transform.position = _cam.position + _cam.forward * 0.9f - _cam.up * 0.34f;
            _text.transform.rotation = Quaternion.LookRotation(_text.transform.position - _cam.position);

            var dir = PvpMatchDirector.Instance;
            int hp = _player != null && _player.Combatant != null ? _player.Combatant.Health : PvpRules.MaxHealth;

            // "You 3 - 1 Bots" sums every opponent so N-way FFA stays one readable number.
            int you = dir != null ? dir.Score(0) : 0;
            int opp = 0;
            if (dir != null)
                for (int i = 1; i < dir.PlayerCount; i++) opp += dir.Score(i);

            string modeLine = PvpModeDirector.Instance != null && PvpModeDirector.Instance.StatusLine.Length > 0
                ? "\n" + PvpModeDirector.Instance.StatusLine : "";

            _text.text = "HP " + hp + "/" + PvpRules.MaxHealth + "    You " + you + " - " + opp
                + (dir != null && dir.PlayerCount > 2 ? " Bots" : " Bot")
                + modeLine
                + (dir != null && dir.Phase == PvpPhase.Ended
                    ? "\nWINNER: " + (dir.Match.WinnerIndex == 0 ? "YOU" : "BOT") : "");
        }
    }
}
