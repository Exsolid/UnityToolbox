using System;
using System.Collections.Generic;
using UnityToolbox.GameplayFeatures.Items.Management;

namespace UnityToolbox.GameplayFeatures.Serialization_Data
{
    [Serializable]
    public class ItemData: GameData
    {
        public string ItemName;
        public string InventoryID;
        public string IconPath;
        public Type ItemType;
        public HashSet<ItemField> Fields;
    }
}
