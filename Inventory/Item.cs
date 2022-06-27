using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Item : MonoBehaviour
{
    private Inventory _inventoryOfItem;
    public Inventory Inventory { get { return _inventoryOfItem; } set { _inventoryOfItem = value; } }

    [SerializeField] protected string _itemName;
    public string ItemName { get { return _itemName; } set { _itemName = value; } }

    [SerializeField] protected Sprite _icon;
    public Sprite Icon { get { return _icon; } set { _icon = value; } }

    [SerializeField] protected GameObject _model;
    public GameObject Model { get { return _model; } set { _model = value; } }

    private string _indentifier;
    public string Indentifier { get { return _indentifier; }
        set {
            Item item = this.MemberwiseClone() as Item;
            _indentifier = value;
            _inventoryOfItem.UpdateItem(item, this);
        } }

    public override bool Equals(object other)
    {
        Type type = other.GetType();
        if (type.IsSubclassOf(typeof(Item)))
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
}
