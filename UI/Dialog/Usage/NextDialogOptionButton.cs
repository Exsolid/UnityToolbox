using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextDialogOptionButton: MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int optionID;
    [SerializeField] private Canvas parentCanvas;

    public void OnPointerDown(PointerEventData data)
    {
        if (parentCanvas.enabled) ModuleManager.GetModule<DialogManager>().nextNode(optionID);
    }
}
