using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityToolbox.UI.Menus;

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
