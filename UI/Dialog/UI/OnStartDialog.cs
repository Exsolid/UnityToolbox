using UnityEngine;
using UnityToolbox.General.Management;

namespace UnityToolbox.UI.Dialog.UI
{
    /// <summary>
    /// A simple script which starts a dialog on start of the scene.
    /// Requires <see cref="DialogManager"/>, <see cref="DisplayDialog"/> and <see cref="UIEventManager"/> to work.
    /// </summary>
    public class OnStartDialog : MonoBehaviour
    {
        [SerializeField] private string _referenceID;
        void Start()
        {
            ModuleManager.GetModule<DialogManager>().StartDialog(_referenceID);
        }
    }

}