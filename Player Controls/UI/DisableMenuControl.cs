using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// This script disables (closes) an active menu, with the defined key (action name).
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class DisableMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] [DropDown(nameof(MenuTypes))] private int _menuType;
    private List<string> MenuTypes;
    private bool _mayDisable;

    private PlayerInput _input;

    private bool _isBinding;

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) => 
        { 
            _isBinding = isSetting; 
        };
        ModuleManager.GetModule<UIEventManager>().OnTogglePaused += (active, toggledType) => 
        {
            _mayDisable = _menuType.Equals(toggledType) && active;
        };
    }
    private void Update()
    {
        if (_input != null && _input.actions[_actionName].triggered && !_isBinding && _mayDisable)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
        }
    }

    private void OnValidate()
    {
        MenuTypes = MenuManager.MenuTypesForEditor;
    }
}
