using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenuInteraction : RaycastInteraction
{
    public override void OnInteraction(RaycastHit raycastHit)
    {
        if (raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>() != null)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuType, true);
        }
    }
}
