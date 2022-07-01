using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toShow;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<MenuEnable>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if(_isEnabled) toShow.GetComponent<Image>().enabled = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        toShow.GetComponent<Image>().enabled = false;
    }
}
