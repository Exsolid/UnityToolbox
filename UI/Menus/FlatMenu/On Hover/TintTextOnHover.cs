using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A script which tints a <see cref="Text"/> on hover.
    /// </summary>
    public class TintTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioMixer _hoverSounds;
        [SerializeField] private Color _colorOnHover;
        private Color _original;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
            _original = GetComponent<Text>().color;
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

            GetComponent<Text>().color = _colorOnHover;
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (_colorOnHover == null)
            {
                return;
            }
            GetComponent<Text>().color = _original;
        }
    }
}