using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityToolbox.Audio;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;

namespace UnityToolbox.UI.Menus.FlatMenu
{
    /// <summary>
    /// A button which opens another menu and thereby closes the current.
    /// </summary>
    public class GotoMenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField][DropDown(nameof(_menuTypes))] private int _menuType;
        [SerializeField][DropDown(nameof(_menusOfType))] private int _menuOfType;
        private List<string> _menuTypes;
        private List<string> _menusOfType;
        [SerializeField] private AudioMixer _clickSounds;
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

                ModuleManager.GetModule<MenuManager>().SetActiveMenu(null);
                ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, _menuOfType);
            }
        }
        private void OnValidate()
        {
            _menuTypes = MenuManager.MenuTypeNamesForEditor;
            _menusOfType = MenuManager.GetAllMenusOfType(_menuType);
        }
    } 
}
