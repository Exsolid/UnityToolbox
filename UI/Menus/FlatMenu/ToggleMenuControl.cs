using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ToggleMenuControl : MonoBehaviour
{
    [SerializeField] private string actionName;

    private PlayerInput input;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        if (input != null && input.actions[actionName].triggered)
        {
            ModuleManager.get<UIEventManager>().TogglePauseMenu();
        }   
    }
}
