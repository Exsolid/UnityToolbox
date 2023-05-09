using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script starts a dialog when interacted with the correct layer.
/// Requires the <see cref="DialogManager"/>
/// </summary>
public class StartDialogInteraction : RaycastInteraction
{
    /// <summary>
    /// The ID which is used to indentify the root object within the <see cref="DialogManager"/>.
    /// </summary>
    [SerializeField] private string _referenceID;

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
}
