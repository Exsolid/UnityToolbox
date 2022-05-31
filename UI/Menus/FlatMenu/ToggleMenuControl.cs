using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ToggleMenuControl : MonoBehaviour
{
    [SerializeField] private string actionName;

    private PlayerInput input;

    private bool isBinding;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        ModuleManager.get<UIEventManager>().bindingKey += (isSetting) => {isBinding = isSetting; };
    }
    private void Update()
    {
        if (input != null && input.actions[actionName].triggered && !isBinding)
        {
            ModuleManager.get<UIEventManager>().TogglePauseMenu();
        }   
    }
}
