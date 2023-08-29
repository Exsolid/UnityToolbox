using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.Audio;
using UnityToolbox.General.Attributes;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.PlayerControls.UI
{
    /// <summary>
    /// Information used for the <see cref="ToggleMenuInteraction"/>, to be able to read the required menu.
    /// </summary>
    public class ToggleMenuInteractionInfo : MonoBehaviour
    {
        [SerializeField] [DropDown(nameof(_menuTypes))] public int MenuType;
        [SerializeField] [DropDown(nameof(_menusOfType))] public int MenuOfType;
        [SerializeField] private AudioMixer _soundToPlay;
        public AudioMixer SoundToPlay
        {
            get { return _soundToPlay; }
        }

        private List<string> _menuTypes;
        private List<string> _menusOfType;

        private void OnValidate()
        {
            _menuTypes = MenuManager.MenuTypeNamesForEditor;
            _menusOfType = MenuManager.GetAllMenusOfType(MenuType);
        }
    }
}
