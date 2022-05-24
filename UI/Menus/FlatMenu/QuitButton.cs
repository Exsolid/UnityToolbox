using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Canvas parentCanvas;

    public void OnPointerDown(PointerEventData data)
    {
        if (parentCanvas.enabled) Application.Quit();
    }
}
