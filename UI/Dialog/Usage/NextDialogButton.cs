using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class NextDialogButton : MonoBehaviour, IPointerDownHandler
{
    private bool _areOptionsPresent;

    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => { _isEnabled = isActive; };
    }

    private void Start()
    {
        ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged += ChangeOptionsPresent;
    }

    private void OnDestroy()
    {
        ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged -= ChangeOptionsPresent;
    }

    public void ChangeOptionsPresent(DialogNode currentNode)
    {
        if(currentNode != null)
        {
            _areOptionsPresent = currentNode.GetOutputNodes().ToList().Count > 1;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!_areOptionsPresent && _isEnabled)
        {
            ModuleManager.GetModule<DialogManager>().NextNode();
        }
    }
}
