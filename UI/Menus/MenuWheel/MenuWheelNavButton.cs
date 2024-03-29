using UnityEngine;
using UnityEngine.EventSystems;
using UnityToolbox.Audio;
using UnityToolbox.General.Management;

namespace UnityToolbox.UI.Menus.MenuWheel
{
    /// <summary>
    /// A button which swaps to the next <see cref="MenuWheel"/> item.
    /// </summary>
    public class MenuWheelNavButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] MenuWheelNavigationTypes _direction;
        [SerializeField] private AudioMixer _clickSounds;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) => { _isEnabled = isActive; };
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (!_isEnabled) return;
            switch (_direction)
            {
                case MenuWheelNavigationTypes.Next:
                    ModuleManager.GetModule<UIEventManager>().MenuWheelNext();
                    break;
                case MenuWheelNavigationTypes.Previous:
                    ModuleManager.GetModule<UIEventManager>().MenuWheelPrevious();
                    break;
            }

            if (_clickSounds != null)
            {
                _clickSounds.PlayRandomSource();
            }
        }
    }

    enum MenuWheelNavigationTypes
    {
        Next,
        Previous
    } 
}