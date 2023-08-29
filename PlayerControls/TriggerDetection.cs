using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolbox.General;
using UnityToolbox.General.Management;
using UnityToolbox.UI;
using UnityToolbox.UI.Menus;
using UnityToolbox.UI.Settings;

namespace UnityToolbox.PlayerControls
{
    /// <summary>
    /// This script is used for interactions with objects with optional tooltips.
    /// </summary>
    public abstract class TriggerDetection : MonoBehaviour
    {
        [SerializeField] protected string _tooltip; //Todo loca?
        protected string _appendedTooltip; //Todo loca?
        [SerializeField] protected string _interactActionName;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] protected PlayerInput _input;

        [SerializeField] private ColliderInfo _colliderInfo;

        /// <summary>
        /// Whether the tooltip module should be used to display text.
        /// </summary>
        protected bool _tooltipEnabled;

        protected bool _isBinding;

        private bool _inputLocked;

        public void Start()
        {
            _tooltipEnabled = true;
            ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) =>
            {
                _isBinding = isSetting;
            };

            ModuleManager.GetModule<PlayerEventManager>().OnLockMove += (isLocked) =>
            {
                _inputLocked = isLocked;
            };
        }

        public void Update()
        {
            if (_colliderInfo.GetAllCollisions2D(_layerMask).Count != 0)
            {
                if (_tooltipEnabled && ModuleManager.ModuleRegistered<TooltipManager>())
                {
                    ModuleManager.GetModule<TooltipManager>().UpdateTooltip(ModuleManager.GetModule<SettingsManager>().CurrentValueOfControl("", _interactActionName).Split("/").Last().ToUpper(), _appendedTooltip == null || _appendedTooltip.Trim().Equals("") ? _tooltip : _tooltip + " " + _appendedTooltip, this);
                }
                else if (ModuleManager.ModuleRegistered<TooltipManager>())
                {
                    ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
                }

                if (_input != null && _input.actions[_interactActionName].triggered && !_isBinding && !_inputLocked)
                {
                    OnInteraction(_colliderInfo.GetAllCollisions2D(_layerMask).First().gameObject);
                }
            }
            else if (ModuleManager.ModuleRegistered<TooltipManager>())
            {
                ModuleManager.GetModule<TooltipManager>().UpdateTooltip("", "", this);
            }
        }

        /// <summary>
        /// Execute whatever should happen when the trigger found a valid object to interact with.
        /// </summary>
        /// <param name="foundObject">The trigger data.</param>
        public abstract void OnInteraction(GameObject foundObject);
    }
}
