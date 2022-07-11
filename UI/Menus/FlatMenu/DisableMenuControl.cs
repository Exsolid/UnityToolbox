using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class DisableMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] private MenuType _menuType;
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
}
