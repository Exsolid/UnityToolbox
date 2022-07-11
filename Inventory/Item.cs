using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Item : Saveable
{
    private Inventory _inventoryOfItem;
    public Inventory Inventory 
    { 
        get { return _inventoryOfItem; } 
        set { _inventoryOfItem = value; } 
    }

    [SerializeField] protected string _itemName;
    public string ItemName { 
        get { return _itemName; } 
        set { _itemName = value; } 
    }

    [SerializeField] protected Sprite _icon;
    public Sprite Icon 
    { 
        get { return _icon; } 
        set { _icon = value; } 
    }

    private string _indentifier;
    public string Indentifier 
    { 
        get { return _indentifier; }
        set {
            Item item = this.MemberwiseClone() as Item;
            _indentifier = value;
            if(_inventoryOfItem != null)
            {
                _inventoryOfItem.UpdateItem(item, this);
            }
        }
    }

    public override bool Equals(object other)
    {
        Type type = other.GetType();
        if (type.IsSubclassOf(typeof(Item)) || type.Equals(typeof(Item)))
        {
            Item otherAsItem = other as Item;
            string thisIndentifier = _itemName + _indentifier;
            return thisIndentifier.Equals(otherAsItem._itemName + otherAsItem._indentifier);
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected override void LoadData(GameData data)
    {
        if (data.GetType().Equals(typeof(ItemData)))
        {
            ItemData item = (ItemData)data;
            _itemName = item.ItemName;
            if (item.InventoryID != null && !item.InventoryID.Equals(""))
            {
                ModuleManager.GetModule<InventoryManager>().GetByID(item.InventoryID).AddItem(this, 1);
            }
        }
    }

    protected override List<GameData> SaveData()
    {
        ItemData data = new ItemData();
        data.ItemName = _itemName;
        if (_inventoryOfItem != null)
        {
            data.InventoryID = _inventoryOfItem.ID;
        }
        return new List<GameData>() { data };
    }

    protected override void OnObjectDeleted()
    {
        if (_inventoryOfItem != null)
        {
            _inventoryOfItem.RemoveItem(this, 1);
        }
    }
}
