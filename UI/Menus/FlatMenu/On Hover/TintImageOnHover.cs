using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A script which tints an <see cref="Image"/> on hover.
    /// </summary>
    public class TintImageOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioMixer _hoverSounds;
        [SerializeField] private Color _colorOnHover;
        private Color _original;
        private bool _isEnabled;

        [Header("Optional")]
        [SerializeField] private Image _toChange;
        public void Awake()
        {
            if (_toChange == null)
            {
                _toChange = GetComponent<Image>();
            }

            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
            _original = _toChange.color;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (_colorOnHover == null || !_isEnabled)
            {
                return;
            }

            if (_hoverSounds != null)
            {
                _hoverSounds.PlayRandomSource();
            }

            _toChange.color = _colorOnHover;
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (_colorOnHover == null)
            {
                return;
            }
            _toChange.color = _original;
        }
    } 
}