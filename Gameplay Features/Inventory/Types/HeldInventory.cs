using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeldInventory : InventoryBase
{
    private Item _itemHeld;
    public Item ItemHeld 
    { 
        get { return _itemHeld; } 
    }

    private int _count;
    public int Count
    { 
        get { return _count; } 
    }

    public override bool AddItem(Item item, int count)
    {
        if (_itemHeld != null && !item.Equals(_itemHeld))
        {
            return false;
        }

        item.Inventory = this;
        if(_itemHeld == null)
        {
            _itemHeld = item;
            _itemHeld.GetComponent<Renderer>().enabled = true;
        }

        _count++;
        return true;
    }

    public override void RemoveItem(Item item, int count)
    {
        if (_itemHeld == null || !item.Equals(_itemHeld))
        {
            return;
        }

        _count--;
        if(_count == 0)
        {
            if (item.Inventory.Equals(this))
            {
                item.Inventory = null;
            }

            _itemHeld = null;
        }
    }

    public override void UpdateItem(Item old, Item updated)
    {
        if (_itemHeld != null && _itemHeld.Equals(old))
        {
            RemoveItem(old, 1);
            AddItem(updated, 1);
        }
    }

    public override void RemoveItem(Item item)
    {
        _count = 0;
        _itemHeld = null;
    }
}