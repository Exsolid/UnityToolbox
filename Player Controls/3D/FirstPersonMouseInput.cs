using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


public class FirstPersonMouseInput : InputSystemUIInputModule
{
    public override void Process()
    {
        var lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        base.Process();
        Cursor.lockState = lockState;
    }
}
