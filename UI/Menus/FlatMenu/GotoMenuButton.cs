using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A button which opens another menu and thereby closes the current.
    /// </summary>
    public class GotoMenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioMixer _clickSounds;
        [SerializeField] private Menu _menu;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (_isEnabled)
            {
                if (_clickSounds != null)
                {
                    _clickSounds.PlayRandomSource();
                }

                ModuleManager.GetModule<MenuManager>().SetActiveMenu(_menu);
            }
        }
    } 
}
