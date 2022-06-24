using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    private Dictionary<Item, int> _items;
    public Dictionary<Item, int> Items { get { return _items; } set { _items = value; } }

    public Action InventoryChanged;

    private void Awake()
    {
        _items = new Dictionary<Item, int>();
    }

    public void AddItem(Item item, int count)
    {
        if (!_items.ContainsKey(item))
        {
            _items.Add(item, count);
            item.Inventory = this;
        }
        else
        {
            _items[item] += count;
        }
        if (InventoryChanged != null) InventoryChanged();
    }    
    
    public void RemoveItem(Item item, int count)
    {
        _items[item] -= count;
        if (_items[item] <= 0)
        {
            item.Inventory = null;
            _items.Remove(item);
        }
        if (InventoryChanged != null) InventoryChanged();
    }

    public void UpdateItem(Item old, Item updated)
    {
        if (_items.ContainsKey(old))
        {
            RemoveItem(old, 1);
            AddItem(updated, 1);
            if (InventoryChanged != null) InventoryChanged();
        }
    }

}
