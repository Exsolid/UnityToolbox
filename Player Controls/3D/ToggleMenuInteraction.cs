using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleMenuInteraction : MonoBehaviour
{
    [SerializeField] private string _actionName;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _raycastLocation;
    private RaycastHit _raycastHit;

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
        if(Physics.Raycast(_raycastLocation.position, transform.forward, out _raycastHit, 2, _layerMask))
        {
            if (_input != null && _input.actions[_actionName].triggered && !_isBinding && _raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>() != null)
            {
                ModuleManager.GetModule<MenuManager>().ToggleMenu(_raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuType, true);
            }
        }

    }

}
