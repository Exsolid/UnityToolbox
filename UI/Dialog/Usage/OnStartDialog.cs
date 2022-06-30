using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartDialog : MonoBehaviour
{
    [SerializeField] private string referenceID;
    void Start()
    {
        ModuleManager.GetModule<DialogManager>().startDialog(referenceID);
    }
}
