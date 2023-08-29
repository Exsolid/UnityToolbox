using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbox.General.Management;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.UI.Settings.Controls
{
    /// <summary>
    /// This script displays the current setting for a control.
    /// It requires a <see cref="SettingsManager"/>, as well information about the ActionName and the Control (e.g. "Character Move" & "left").
    /// The control can be left empty if no information is necessary. (e.g. "Character Jump" & "")
    /// </summary>
    public class ShowControl : MonoBehaviour
    {
        [SerializeField] private string _control;
        [SerializeField] private string _actionName;
        [SerializeField] private Text _displayText;
        private SettingsManager _manager;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
        }

        void Start()
        {
            _manager = ModuleManager.GetModule<SettingsManager>();
        }

        private void Update()
        {
            if (_isEnabled)
            {
                _displayText.text = _manager.CurrentValueOfControl(_control, _actionName).Split("/").Last();
            }
        }
    } 
}
