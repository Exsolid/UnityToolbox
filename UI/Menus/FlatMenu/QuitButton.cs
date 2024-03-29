using UnityEngine;
using UnityEngine.EventSystems;
using UnityToolbox.Audio;

namespace UnityToolbox.UI.Menus.FlatMenu
{
    /// <summary>
    /// A button which quits the application to desktop.
    /// </summary>
    public class QuitButton : MonoBehaviour, IPointerDownHandler
    {
        private bool _isEnabled;
        [SerializeField] private AudioMixer _clickSounds;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (_isEnabled)
            {
                if (_clickSounds != null)
                {
                    _clickSounds.PlayRandomSource();
                }

                Application.Quit();
            }
        }
    }

}