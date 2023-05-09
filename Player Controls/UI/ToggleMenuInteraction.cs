using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script toggles a menu. An object with the right layer needs the <see cref="ToggleMenuInteractionInfo"/> for this to work.
/// Requires the <see cref="MenuManager"/>.
/// </summary>
public class ToggleMenuInteraction : RaycastInteraction
{
    public override void OnInteraction(RaycastHit raycastHit)
    {
        if (raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>() != null)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuType, raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuOfType);
        }
    }

    public override void OnInteraction(RaycastHit2D raycastHit)
    {
        if (raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>() != null)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuType, raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>().MenuOfType);
        }
    }
}
