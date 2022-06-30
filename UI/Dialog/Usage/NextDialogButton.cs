using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class NextDialogButton : MonoBehaviour, IPointerDownHandler
{
    private bool areOptionsPresent;
    [SerializeField] private Canvas parentCanvas;

    private void Start()
    {
        ModuleManager.GetModule<UIEventManager>().dialogNodeChanged += changeOptionsPresent;
    }

    private void OnDestroy()
    {
        ModuleManager.GetModule<UIEventManager>().dialogNodeChanged -= changeOptionsPresent;
    }

    public void changeOptionsPresent(DialogNode currentNode)
    {
        if(currentNode != null)areOptionsPresent = currentNode.GetOutputNodes().ToList().Count > 1;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if(!areOptionsPresent && parentCanvas.enabled) ModuleManager.GetModule<DialogManager>().nextNode();
    }
}
