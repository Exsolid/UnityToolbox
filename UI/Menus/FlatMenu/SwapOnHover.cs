using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite spriteSwapOnHover;
    private Sprite current;

    public void OnPointerEnter(PointerEventData data)
    {
        if (spriteSwapOnHover == null) return;
        current = GetComponent<Image>().sprite;
        GetComponent<Image>().sprite = spriteSwapOnHover;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (spriteSwapOnHover == null) return;
        GetComponent<Image>().sprite = current;
    }
}
