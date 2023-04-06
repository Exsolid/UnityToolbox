using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenuInteractionInfo : MonoBehaviour
{
    [SerializeField] [DropDown(nameof(_menuTypes))] public int MenuType;
    private List<string> _menuTypes;

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypesForEditor;
    }
}
