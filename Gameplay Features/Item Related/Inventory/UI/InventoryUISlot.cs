using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Item;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    private ItemInstance _itemToShow;
    public ItemInstance ItemToShow { get { return _itemToShow; } set { _itemToShow = value; } }
    private int _itemCount;
    public int ItemCount { get { return _itemCount; } set { _itemCount = value; } }
    [HideInInspector] public BoundInventory InventoryOfItem;

    [SerializeField] private Image _iconImage;
    [SerializeField] private Text _itemNameText;
    [SerializeField] private Text _itemCountText;

    // Start is called before the first frame update
    void Start()
    {
        if (_iconImage != null)
        {
            _iconImage.enabled = false;
        }
        if (_itemCountText != null)
        {
            _itemCountText.text = "";
        }
        if (_itemNameText != null)
        {
            _itemNameText.text = "";
        }
    }

    public void UpdateItem(ItemInstance item, int count)
    {
        _itemToShow = item;
        _itemCount = count;
        if (_iconImage != null)
        {
            if (_itemToShow != null)
            {
                _iconImage.enabled = true;
                _iconImage.sprite = _itemToShow.Icon;
            }
            else
            {
                _iconImage.enabled = false;
            }
        }
        if (_itemCountText != null)
        {
            if (_itemToShow != null)
            {
                _itemCountText.text = _itemCount.ToString();
            }
            else
            {
                _itemCountText.text = "";
            }
        }
        if (_itemNameText != null)
        {
            if (_itemNameText != null)
            {
                _itemNameText.text = _itemToShow.ItemName.ToString();
            }
            else
            {
                _itemNameText.text = "";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ModuleManager.GetModule<HeldInventoryManager>().ManageInventorys(InventoryOfItem, _itemToShow);
    }
}
