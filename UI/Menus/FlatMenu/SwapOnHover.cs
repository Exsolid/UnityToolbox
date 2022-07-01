using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite spriteSwapOnHover;
    private Sprite current;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<MenuEnable>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (spriteSwapOnHover == null || !_isEnabled) return;
        current = GetComponent<Image>().sprite;
        GetComponent<Image>().sprite = spriteSwapOnHover;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (spriteSwapOnHover == null || !_isEnabled) return;
        GetComponent<Image>().sprite = current;
    }
}
