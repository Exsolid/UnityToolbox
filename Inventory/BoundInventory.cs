using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BoundInventory : InventoryBase
{
    [SerializeField] private Transform _placeToHideObjects;

    private Dictionary<Item, int> _items;
    public Dictionary<Item, int> Items 
    { 
        get { return _items; } 
        set { _items = value; } 
    }

    public Action OnInventoryChanged;

    public int MaxSlotCount;

    private void Awake()
    {
        _items = new Dictionary<Item, int>();
        ModuleManager.GetModule<InventoryManager>().RegisterInventory(this);
    }

    public override bool AddItem(Item item, int count)
    {
        if(MaxSlotCount <= _items.Count && (!_items.ContainsKey(item) || !item.Stackable))
        {
            return false;
        }

        if (!_items.ContainsKey(item))
        {
            _items.Add(item, count);
            item.transform.position = _placeToHideObjects.position;
            item.Inventory = this;
            item.gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            Destroy(item.gameObject);
            _items[item] += count;
        }

        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
        return true;
    }    
    
    public override void RemoveItem(Item item, int count)
    {
        _items[item] -= count;
        if (_items[item] <= 0)
        {
            if (item.Inventory.Equals(this))
            {
                item.Inventory = null;
            }

            _items.Remove(item);
        }

        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }

    public override void UpdateItem(Item old, Item updated)
    {
        if (_items.ContainsKey(old))
        {
            int currentCount = _items[old];
            RemoveItem(old);
            AddItem(updated, currentCount);
            if (OnInventoryChanged != null)
            {
                OnInventoryChanged();
            }
        }
    }

    public override void RemoveItem(Item item)
    {
        item.Inventory = null;
        _items.Remove(item);

        item.gameObject.GetComponent<Renderer>().enabled = true;
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }
}
