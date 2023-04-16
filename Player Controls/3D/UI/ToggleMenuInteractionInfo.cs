using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information used for the <see cref="ToggleMenuInteraction"/>, to be able to read the required menu.
/// </summary>
public class ToggleMenuInteractionInfo : MonoBehaviour
{
    [SerializeField] [DropDown(nameof(_menuTypes))] public int MenuType;
    private List<string> _menuTypes;

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypesForEditor;
    }
}
