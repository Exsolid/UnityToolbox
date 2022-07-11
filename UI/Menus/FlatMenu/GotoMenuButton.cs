using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GotoMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Menu _menu;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
        { 
            _isEnabled = isActive; 
        };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (_isEnabled)
        {
            ModuleManager.GetModule<MenuManager>().SetActiveMenu(_menu);
        }
    }
}
