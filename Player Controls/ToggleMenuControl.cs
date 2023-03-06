using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ToggleMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] [DropDown(nameof(_menuTypes))] private int _menuType;
    private List<string> _menuTypes;

    private PlayerInput _input;

    private bool _isBinding;

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) => 
        {
            _isBinding = isSetting;
        };
    }
    private void Update()
    {
        if (_input != null && _input.actions[_actionName].triggered && !_isBinding)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
        }
    }

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypesForEditor;
    }
}
