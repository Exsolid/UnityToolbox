using UnityEngine;
using UnityToolbox.General.Management;
using UnityToolbox.UI.Dialog;

namespace UnityToolbox.PlayerControls.UI
{
    /// <summary>
    /// This script starts a dialog when interacted with the correct layer.
    /// Requires the <see cref="DialogManager"/>
    /// </summary>
    public class StartDialogInteraction : RaycastDetection
    {
        /// <summary>
        /// The ID which is used to indentify the root object within the <see cref="DialogManager"/>.
        /// </summary>
        [SerializeField] private string _referenceID;

        public override void OnHit(RaycastHit raycastHit)
        {
        }

        public override void OnHit(RaycastHit2D raycastHit)
        {
        }

        public override void OnInteraction(RaycastHit raycastHit)
        {
            if(raycastHit.collider != null)
            {
                ModuleManager.GetModule<DialogManager>().StartDialog(_referenceID);
            }
        }

        public override void OnInteraction(RaycastHit2D raycastHit)
        {
            if (raycastHit.collider != null)
            {
                ModuleManager.GetModule<DialogManager>().StartDialog(_referenceID);
            }
        }
        public override void OnNull()
        {
        }
    }
}
