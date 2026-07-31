using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Draws the armor state where a VR player actually notices it — the edge of vision.
    ///
    /// Thin translator over <see cref="ArmorHudCore"/>: it owns a camera-parented quad and asks the
    /// core what colour and opacity to draw. It decides nothing, so "is a healthy player's view
    /// clear?" and "does broken read differently from hurt?" are answered by tests rather than by a
    /// headset session.
    ///
    /// The quad is built with the same idiom as PlayerStunReceiver's flash — parented to the camera,
    /// unlit, collider stripped — deliberately, rather than inventing a second way to put something in
    /// front of the eyes. Two overlay systems with different rules is how one of them ends up stuck on.
    /// </summary>
    [DisallowMultipleComponent]
    public class ArmorVignetteRuntime : MonoBehaviour
    {
        private PlayerArmor _armor;
        private Camera _cam;
        private GameObject _quad;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        /// <summary>Ensure the readout on the rig. Installed beside the armor it reports on.</summary>
        public static ArmorVignetteRuntime EnsureOn(GameObject rig)
        {
            if (rig == null) return null;
            ArmorVignetteRuntime v = rig.GetComponent<ArmorVignetteRuntime>();
            if (v == null)
            {
                v = rig.AddComponent<ArmorVignetteRuntime>();
                Debug.Log("ZIPTIDE: ARMOR_HUD ready style=peripheral_vignette");
            }
            return v;
        }

        private void LateUpdate()
        {
            if (_armor == null) _armor = GetComponent<PlayerArmor>();
            if (_armor == null) return;

            float opacity = ArmorHudCore.CurrentOpacity(
                _armor.Armor01, _armor.IsBroken, Time.time);

            // Full armor draws nothing at all — and the quad is switched OFF rather than drawn
            // transparent, so a healthy player costs zero overdraw on a mobile GPU.
            if (opacity <= 0.001f)
            {
                if (_quad != null && _quad.activeSelf) _quad.SetActive(false);
                return;
            }

            if (!EnsureQuad()) return;
            if (!_quad.activeSelf) _quad.SetActive(true);

            ArmorHudCore.Tint(_armor.Armor01, _armor.IsBroken, out float r, out float g, out float b);
            if (_block == null) _block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_block);
            _block.SetColor("_Color", new Color(r, g, b, opacity));
            _block.SetColor("_BaseColor", new Color(r, g, b, opacity));
            _renderer.SetPropertyBlock(_block);
        }

        private bool EnsureQuad()
        {
            if (_quad != null) return true;
            if (_cam == null) _cam = GetComponentInChildren<Camera>();
            if (_cam == null) return false;

            _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _quad.name = "__ArmorVignette";
            Collider col = _quad.GetComponent<Collider>();
            if (col != null) Destroy(col);

            _quad.transform.SetParent(_cam.transform, false);
            // Slightly further out than the stun flash so a hit's flash reads ON TOP of the armor
            // state rather than fighting it for the same depth.
            _quad.transform.localPosition = new Vector3(0f, 0f, 0.40f);
            _quad.transform.localRotation = Quaternion.identity;
            _quad.transform.localScale = new Vector3(2.8f, 2.8f, 1f);

            _renderer = _quad.GetComponent<Renderer>();
            if (_renderer != null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
                _renderer.material = new Material(shader);
                _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _renderer.receiveShadows = false;
            }
            return _renderer != null;
        }
    }
}
