using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.UI.Dialog
{
    /// <summary>
    /// A script which is placed on a UI element to act as a button.
    /// When triggered, the next dialog node will be displayed based on the set option.
    /// Requires <see cref="DialogManager"/>, <see cref="DisplayDialog"/> and <see cref="UIEventManager"/> to work.
    /// </summary>
    public class NextDialogOptionButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private int _optionID;
        [SerializeField] private AudioMixer _soundToPlay;
        private bool _isEnabled;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
            {
                _isEnabled = isActive;
            };
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (_isEnabled)
            {
                _soundToPlay?.PlayRandomSource();
                ModuleManager.GetModule<DialogManager>().NextNode(_optionID);
            }
        }
    } 
}
