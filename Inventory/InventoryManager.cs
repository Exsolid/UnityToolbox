using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InventoryManager : Module
{
    private List<BoundInventory> _allInventorys;

    public override void Awake()
    {
        base.Awake();
        _allInventorys = new List<BoundInventory>();
    }

    public void RegisterInventory(BoundInventory inv)
    {
       _allInventorys.Add(inv);
    }

    public BoundInventory GetByID(string ID)
    {
        var invWithID = _allInventorys.Where(inv => inv.ID.Equals(ID));
        if (!invWithID.Any())
        {
            Debug.LogWarning("Inventory with ID " + ID + " not found");
        }
        return invWithID.FirstOrDefault();
    }
}
