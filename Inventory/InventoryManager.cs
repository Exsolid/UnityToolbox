using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InventoryManager : Module
{
    private List<Inventory> _allInventorys;

    public override void Awake()
    {
        base.Awake();
        _allInventorys = new List<Inventory>();
    }

    public void RegisterInventory(Inventory inv)
    {
       _allInventorys.Add(inv);
    }

    public Inventory GetByID(string ID)
    {
        var invWithID = _allInventorys.Where(inv => inv.ID.Equals(ID));
        if (!invWithID.Any())
        {
            Debug.LogWarning("Inventory with ID" + ID + " not found");
        }
        return invWithID.First();
    }
}
