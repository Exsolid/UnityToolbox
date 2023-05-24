using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Item.Management;

[Serializable]
public class ItemData: GameData
{
    public string ItemName;
    public string InventoryID;
    public string IconPath;
    public HashSet<ItemField> Fields;
}
