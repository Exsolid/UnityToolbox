using UnityEngine;
using UnityToolbox.General.Management;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.PlayerControls.UI
{
    /// <summary>
    /// This script toggles a menu. An object with the right layer needs the <see cref="ToggleMenuInteractionInfo"/> for this to work.
    /// Requires the <see cref="MenuManager"/>.
    /// </summary>
    public class ToggleMenuInteraction : RaycastDetection
    {
        public override void OnHit(RaycastHit raycastHit)
        {
        }

        public override void OnHit(RaycastHit2D raycastHit)
        {
        }

        public override void OnInteraction(RaycastHit raycastHit)
        {
            ToggleMenuInteractionInfo info = raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>();
            if (info != null)
            {
                info.SoundToPlay?.PlayRandomSource();
                ModuleManager.GetModule<MenuManager>().ToggleMenu(info.MenuType, info.MenuOfType);
            }
        }

        public override void OnInteraction(RaycastHit2D raycastHit)
        {
            ToggleMenuInteractionInfo info = raycastHit.collider.GetComponent<ToggleMenuInteractionInfo>();
            if (info != null)
            {
                info.SoundToPlay?.PlayRandomSource();
                ModuleManager.GetModule<MenuManager>().ToggleMenu(info.MenuType, info.MenuOfType);
            }
        }
    }
}
