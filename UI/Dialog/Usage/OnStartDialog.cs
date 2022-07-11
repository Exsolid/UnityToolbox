using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartDialog : MonoBehaviour
{
    [SerializeField] private string _referenceID;
    void Start()
    {
        ModuleManager.GetModule<DialogManager>().StartDialog(_referenceID);
    }
}
