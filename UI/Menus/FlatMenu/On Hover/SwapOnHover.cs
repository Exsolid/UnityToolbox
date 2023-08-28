using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A script which swapes the <see cref="Sprite"/> of an <see cref="Image"/> on hover.
    /// </summary>
    public class SwapOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioMixer _hoverSounds;
        [SerializeField] private Sprite _spriteSwapOnHover;
        private Sprite _original;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
            _original = GetComponent<Image>().sprite;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (_spriteSwapOnHover == null || !_isEnabled)
            {
                return;
            }

            if (_hoverSounds != null)
            {
                _hoverSounds.PlayRandomSource();
            }

            GetComponent<Image>().sprite = _spriteSwapOnHover;
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (_spriteSwapOnHover == null)
            {
                return;
            }
            GetComponent<Image>().sprite = _original;
        }
    } 
}
