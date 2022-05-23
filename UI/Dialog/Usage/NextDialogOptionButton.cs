using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextDialogOptionButton: MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int optionID;
    public void OnPointerDown(PointerEventData data)
    {
        ModuleManager.get<DialogManager>().nextNode(optionID);
    }
}
