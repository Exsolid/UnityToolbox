using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolbox.Audio;

namespace UnityToolbox.UI.Menus.FlatMenu.OnHover
{
    /// <summary>
    /// A script which displays an <see cref="Image"/> on hover.
    /// </summary>
    public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioMixer _hoverSounds;
        [SerializeField] private GameObject toShow;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) => { _isEnabled = isActive; };
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (_isEnabled)
            {
                if (_hoverSounds != null)
                {
                    _hoverSounds.PlayRandomSource();
                }

                toShow.GetComponent<Image>().enabled = true;
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            toShow.GetComponent<Image>().enabled = false;
        }
    } 
}
