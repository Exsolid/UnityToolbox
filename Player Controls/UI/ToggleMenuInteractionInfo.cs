using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Information used for the <see cref="ToggleMenuInteraction"/>, to be able to read the required menu.
/// </summary>
public class ToggleMenuInteractionInfo : MonoBehaviour
{
    [SerializeField] [DropDown(nameof(_menuTypes))] public int MenuType;
    [SerializeField] [DropDown(nameof(_menusOfType))] public int MenuOfType;
    private List<string> _menuTypes;
    private List<string> _menusOfType;

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypeNamesForEditor;
        _menusOfType = MenuManager.GetAllMenusOfType(MenuType);
    }
}
