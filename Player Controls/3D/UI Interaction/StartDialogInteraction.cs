using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogInteraction : RaycastInteraction
{
    [SerializeField] private string _referenceID;

    public override void OnInteraction(RaycastHit raycastHit)
    {
        ModuleManager.GetModule<DialogManager>().StartDialog(_referenceID);
    }
}
