using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class DisableMenuControl : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] private MenuType _menuType;
    private bool _mayDisable;

    private PlayerInput input;

    private bool isBinding;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        ModuleManager.GetModule<UIEventManager>().bindingKey += (isSetting) => { isBinding = isSetting; };
        ModuleManager.GetModule<UIEventManager>().togglePaused += (active, toggledType) => { _mayDisable = _menuType.Equals(toggledType) && active; };
    }
    private void Update()
    {
        if (input != null && input.actions[_actionName].triggered && !isBinding && _mayDisable)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
        }
    }
}
