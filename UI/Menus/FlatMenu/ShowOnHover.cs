using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toShow;
    [SerializeField] private Canvas parentCanvas;

    public void OnPointerEnter(PointerEventData data)
    {
        if(parentCanvas.enabled) toShow.GetComponent<Image>().enabled = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        toShow.GetComponent<Image>().enabled = false;
    }
}
