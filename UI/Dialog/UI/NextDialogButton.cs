using UnityEngine;
using UnityEngine.EventSystems;
using UnityToolbox.Audio;
using UnityToolbox.General.Management;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.UI.Dialog.UI
{
    /// <summary>
    /// A script which is placed on a UI element to act as a button. When triggered, the next dialog node will be displayed.
    /// Requires <see cref="DialogManager"/>, <see cref="DisplayDialog"/> and <see cref="UIEventManager"/> to work.
    /// </summary>
    public class NextDialogButton : MonoBehaviour, IPointerDownHandler
    {
        private bool _areOptionsPresent;

        private bool _isEnabled;

        [SerializeField] private AudioMixer _soundToPlay;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += (isActive) => { _isEnabled = isActive; };
        }

        private void Start()
        {
            ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged += ChangeOptionsPresent;
        }

        private void OnDestroy()
        {
            if (ModuleManager.ModuleRegistered<UIEventManager>())
            {
                ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged -= ChangeOptionsPresent;
            }
        }

        private void ChangeOptionsPresent(DialogNodeData currentNode)
        {
            if (currentNode != null)
            {
                _areOptionsPresent = currentNode.OutputIDs.Count > 1;
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (!_areOptionsPresent && _isEnabled)
            {
                _soundToPlay?.PlayRandomSource();
                ModuleManager.GetModule<DialogManager>().NextNode();
            }
        }
    } 
}
