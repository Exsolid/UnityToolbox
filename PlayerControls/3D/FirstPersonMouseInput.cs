using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace UnityToolbox.PlayerControls._3D
{
    /// <summary>
    /// A module which should be placed instead of the default <see cref="InputSystemUIInputModule"/>.
    /// It is required, if the player should interact with its mouse, but it being in <see cref="CursorLockMode.Locked"/>.
    /// </summary>
    public class FirstPersonMouseInput : InputSystemUIInputModule
    {
        public override void Process()
        {
            var lockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = lockState.Equals(CursorLockMode.Locked) ? false : true;
            base.Process();
            if(Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = lockState;
            }
        }
    }
}
