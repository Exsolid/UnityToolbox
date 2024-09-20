using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolbox.General.Management;
using UnityToolbox.UI;
using UnityToolbox.UI.Menus;
using UnityToolbox.UI.Settings;

namespace UnityToolbox.PlayerControls
{
    /// <summary>
    /// This script is used for interactions with objects with optional tooltips.
    /// </summary>
    public abstract class RaycastDetection : MonoBehaviour
    {
        [SerializeField] private bool _is2D;
        [SerializeField] protected string _tooltip; //Todo loca?
        protected string _appendedTooltip; //Todo loca?
        [SerializeField] protected string _interactActionName;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] protected Transform _raycastLocation;
        [SerializeField] protected PlayerInput _input;
        [SerializeField] private float _maxDistance = 2;
        /// <summary>
        /// Whether the tooltip module should be used to display text.
        /// </summary>
        [SerializeField] protected bool _tooltipEnabled = true;

        private RaycastHit _raycastHit;
        private RaycastHit2D _raycastHit2D;

        protected bool _isBinding;

        private bool _inputLocked;

        private TooltipManager _tooltipManager;
        private SettingsManager _settingsManager;
        public void Start()
        {
            ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) =>
            {
                _isBinding = isSetting;
            };

            ModuleManager.GetModule<PlayerEventManager>().OnLockMove += (isLocked) =>
            {
                _inputLocked = isLocked;
            };
            _tooltipManager = ModuleManager.GetModule<TooltipManager>();
            _settingsManager = ModuleManager.GetModule<SettingsManager>();
        }

        public void Update()
        {
            if (_is2D)
            {
                _raycastHit2D = Physics2D.Raycast(_raycastLocation.position, _raycastLocation.transform.right, _maxDistance, _layerMask);
            }
            else
            {
                Physics.Raycast(_raycastLocation.position, _raycastLocation.transform.forward, out _raycastHit, _maxDistance, _layerMask);
            }

            if (_raycastHit2D.collider != null || _raycastHit.collider != null)
            {
                if (_tooltipEnabled && _tooltipManager != null)
                {
                    _tooltipManager.UpdateTooltip(_settingsManager.CurrentValueOfControl("", _interactActionName).Split("/").Last().ToUpper(), _appendedTooltip == null || _appendedTooltip.Trim().Equals("") ? _tooltip : _tooltip + " " +_appendedTooltip, this);
                }
                else if (_tooltipManager != null)
                {
                    _tooltipManager.UpdateTooltip("", "", this);
                }

                if (_input != null && _input.actions[_interactActionName].triggered && !_isBinding && !_inputLocked)
                {
                    if(_raycastHit.collider != null)
                    {
                        OnInteraction(_raycastHit);
                    }
                    if (_raycastHit2D.collider != null)
                    {
                        OnInteraction(_raycastHit2D);
                    }
                }
                if (_raycastHit.collider != null)
                {
                    OnHit(_raycastHit);
                }
                if (_raycastHit2D.collider != null)
                {
                    OnHit(_raycastHit2D);
                }
            }
            else if(_tooltipManager != null)
            {
                _tooltipManager.UpdateTooltip("", "", this);
                OnNull();
            }
            else
            {
                OnNull();
            }
        }

        /// <summary>
        /// Execute whatever should happen when the raycast has found nothing.
        /// </summary>
        public abstract void OnNull();

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

        /// <summary>
        /// Execute whatever should happen when the raycast found a valid object.
        /// </summary>
        /// <param name="raycastHit">The raycast data.</param>
        public abstract void OnHit(RaycastHit raycastHit);

        /// <summary>
        /// Execute whatever should happen when the raycast found a valid object.
        /// </summary>
        /// <param name="raycastHit">The raycast data.</param>
        public abstract void OnHit(RaycastHit2D raycastHit);
    }
}
