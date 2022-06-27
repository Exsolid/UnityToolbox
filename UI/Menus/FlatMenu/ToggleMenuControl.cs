using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ToggleMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] private MenuType _menuType;

    private PlayerInput _input;

    private bool _isBinding;

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        ModuleManager.get<UIEventManager>().bindingKey += (isSetting) => {_isBinding = isSetting; };
    }
    private void Update()
    {
        if (_input != null && _input.actions[_actionName].triggered && !_isBinding)
        {
            ModuleManager.get<UIEventManager>().ToggleMenu(_menuType);
        }   
    }
}
