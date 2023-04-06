using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public abstract class RaycastInteraction : MonoBehaviour
{
    [SerializeField] private string _tooltip; //Todo loca?
    [SerializeField] private string _actionName;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _raycastLocation;
    protected bool _tooltipEnabled;

    private RaycastHit _raycastHit;

    private PlayerInput _input;

    private bool _isBinding;

    private void Start()
    {
        _tooltipEnabled = true;
        _input = GetComponent<PlayerInput>();
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) =>
        {
            _isBinding = isSetting;
        };
    }

    private void Update()
    {
        if (Physics.Raycast(_raycastLocation.position, _raycastLocation.transform.forward, out _raycastHit, 4, _layerMask))
        {
            if (_tooltipEnabled)
            {
                ModuleManager.GetModule<TooltipManager>().UpdateTooltip(ModuleManager.GetModule<SettingsManager>().CurrentValueOfControl("", _actionName).Split("/").Last().ToUpper(), _tooltip, this);
            }
            else
            {
                ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
            }

            if (_input != null && _input.actions[_actionName].triggered && !_isBinding)
            {
                OnInteraction(_raycastHit);
            }
        }
        else
        {
            ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
        }
    }

    public abstract void OnInteraction(RaycastHit raycastHit);
}
