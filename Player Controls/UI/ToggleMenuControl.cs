using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// This script toggles a menu with the defined key (action name).
/// </summary>
public class ToggleMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] [DropDown(nameof(_menuTypes))] private int _menuType;
    [SerializeField] [DropDown(nameof(_menusOfType))] private int _menuOfType;
    private List<string> _menuTypes;
    private List<string> _menusOfType;

    [SerializeField] private PlayerInput _input;

    private bool _isBinding;

    private void Start()
    {
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) => 
        {
            _isBinding = isSetting;
        };
    }
    private void Update()
    {
        if (_input != null && _input.actions[_actionName].triggered && !_isBinding)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, _menuOfType);
        }
    }

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypeNamesForEditor;
        _menusOfType = MenuManager.GetAllMenusOfType(_menuType);
    }
}
