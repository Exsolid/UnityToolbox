using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

public abstract class InventoryBase: MonoBehaviour
{
    [SerializeField] [ReadOnly] private string id = "";
    public string ID
    {
        get { return id; }
    }

    public abstract bool AddItem(ItemInstance item, int count);
    public abstract void RemoveItem(ItemInstance item, int count);
    public abstract void RemoveItem(ItemInstance item);
    public abstract void UpdateItem(ItemInstance old, ItemInstance updated);

    private void OnValidate()
    {
        if (id.Equals(""))
        {
            id = IDManager.GetUniqueID();
        }
    }
}
