using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    private Dictionary<Item, int> _items;
    public Dictionary<Item, int> Items 
    { 
        get { return _items; } 
        set { _items = value; } 
    }

    [SerializeField] [ReadOnly] private string id = "";
    public string ID 
    { 
        get { return id; }
    }
    public Action InventoryChanged;

    private void Awake()
    {
        _items = new Dictionary<Item, int>();
        ModuleManager.GetModule<InventoryManager>().RegisterInventory(this);
    }

    public void AddItem(Item item, int count)
    {
        if (!_items.ContainsKey(item))
        {
            _items.Add(item, count);
            item.Inventory = this;
            item.gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            _items[item] += count;
            item.gameObject.GetComponent<Renderer>().enabled = false;
        }

        if (InventoryChanged != null)
        {
            InventoryChanged();
        }
    }    
    
    public void RemoveItem(Item item, int count)
    {
        _items[item] -= count;
        if (_items[item] <= 0)
        {
            item.Inventory = null;
            _items.Remove(item);
        }

        item.gameObject.GetComponent<Renderer>().enabled = true;
        if (InventoryChanged != null) InventoryChanged();
    }

    public void UpdateItem(Item old, Item updated)
    {
        if (_items.ContainsKey(old))
        {
            RemoveItem(old, 1);
            AddItem(updated, 1);
            if (InventoryChanged != null)
            {
                InventoryChanged();
            }
        }
    }

    private void OnValidate()
    {
        if (id.Equals(""))
        {
            id = IDManager.GetUniqueID();
        }
    }
}
