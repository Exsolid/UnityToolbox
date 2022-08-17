using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryBase: MonoBehaviour
{
    [SerializeField] [ReadOnly] private string id = "";
    public string ID
    {
        get { return id; }
    }

    public abstract bool AddItem(Item item, int count);
    public abstract void RemoveItem(Item item, int count);
    public abstract void RemoveItem(Item item);
    public abstract void UpdateItem(Item old, Item updated);

    private void OnValidate()
    {
        if (id.Equals(""))
        {
            id = IDManager.GetUniqueID();
        }
    }
}
