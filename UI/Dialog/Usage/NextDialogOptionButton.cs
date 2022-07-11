using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextDialogOptionButton: MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int _optionID;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => 
        { 
            _isEnabled = isActive;
        };
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (_isEnabled)
        {
            ModuleManager.GetModule<DialogManager>().NextNode(_optionID);
        }
    }
}
