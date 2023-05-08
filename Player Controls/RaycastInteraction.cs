using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// This script is used for interactions with objects with optional tooltips.
/// </summary>
public abstract class RaycastInteraction : MonoBehaviour
{
    [SerializeField] private bool _is2D;
    [SerializeField] private string _tooltip; //Todo loca?
    [SerializeField] private string _actionName;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _raycastLocation;
    [SerializeField] private PlayerInput _input;
    /// <summary>
    /// Whether the tooltip module should be used to display text.
    /// </summary>
    protected bool _tooltipEnabled;

    private RaycastHit _raycastHit;
    private RaycastHit2D _raycastHit2D;

    private bool _isBinding;

    private void Start()
    {
        _tooltipEnabled = true;
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) =>
        {
            _isBinding = isSetting;
        };
    }

    private void Update()
    {
        if (_is2D)
        {
            _raycastHit2D = Physics2D.Raycast(_raycastLocation.position, _raycastLocation.transform.right, 4, _layerMask);
        }
        else
        {
            Physics.Raycast(_raycastLocation.position, _raycastLocation.transform.forward, out _raycastHit, 4, _layerMask);
        }

        if (_raycastHit2D.collider != null || _raycastHit.collider != null)
        {
            if (_tooltipEnabled && ModuleManager.ModuleRegistered<TooltipManager>())
            {
                ModuleManager.GetModule<TooltipManager>().UpdateTooltip(ModuleManager.GetModule<SettingsManager>().CurrentValueOfControl("", _actionName).Split("/").Last().ToUpper(), _tooltip, this);
            }
            else if (ModuleManager.ModuleRegistered<TooltipManager>())
            {
                ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
            }

            if (_input != null && _input.actions[_actionName].triggered && !_isBinding)
            {
                OnInteraction(_raycastHit);
                OnInteraction(_raycastHit2D);
            }
        }
        else if(ModuleManager.ModuleRegistered<TooltipManager>())
        {
            ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
        }
    }

    /// <summary>
    /// Execute whatever should happen when the raycast found a valid object to interact with.
    /// </summary>
    /// <param name="raycastHit">The raycast data.</param>
    public abstract void OnInteraction(RaycastHit raycastHit);

    /// <summary>
    /// Execute whatever should happen when the raycast found a valid object to interact with.
    /// </summary>
    /// <param name="raycastHit">The raycast data.</param>
    public abstract void OnInteraction(RaycastHit2D raycastHit);
}
