using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.Items.Inventory.Managers;
using UnityToolbox.GameplayFeatures.Items.Inventory.Types;
using UnityToolbox.GameplayFeatures.Items.Management;
using UnityToolbox.GameplayFeatures.SaveGame;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.Items
{
    /// <summary>
    /// An instanciated item, created from an <see cref="ItemDefinition"/>.
    /// </summary>
    public class ItemInstance : Saveable
    {
        private InventoryBase _inventoryOfItem;
        public InventoryBase Inventory
        {
            get { return _inventoryOfItem; }
            set { _inventoryOfItem = value; }
        }

        protected string _itemQualifiedName;
        public string ItemQualifiedName
        {
            get { return _itemQualifiedName; }
            set { _itemQualifiedName = value; }
        }

        protected string _itemName;
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        [ReadOnly] public string IconPath;

        protected Sprite _icon;
        public Sprite Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private string _indentifier;
        public string Indentifier
        {
            get { return _indentifier; }
            set
            {
                ItemInstance item = this.MemberwiseClone() as ItemInstance;
                _indentifier = value;
                if (_inventoryOfItem != null)
                {
                    _inventoryOfItem.UpdateItem(item, this);
                }
            }
        }

        public int MaxStackCount;

        private Type _itemType;
        public Type ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        private HashSet<ItemField> _itemFields;
        public HashSet<ItemField> ItemFields
        {
            get { return _itemFields; }
            set { _itemFields = value; }
        }

        public object GetValueForField(string fieldName)
        {
            if (_itemFields == null)
            {
                throw new ArgumentException("This item does not have additional defined field.");
            }

            IEnumerable<ItemField> foundField = _itemFields.Where(field => field.FieldName.Equals(fieldName));
            if (!foundField.Any())
            {
                throw new ArgumentException("The given field name \"" + fieldName + "\" cannot be found.");
            }

            return foundField.First().GetValue();
        }

        public override bool Equals(object other)
        {
            Type type = other.GetType();
            if (type.IsSubclassOf(typeof(ItemInstance)) || type.Equals(typeof(ItemInstance)))
            {
                ItemInstance otherAsItem = other as ItemInstance;
                string thisIndentifier = _itemName.Equals("") ? this.name : _itemName + _indentifier;
                return thisIndentifier.Equals(_itemName.Equals("") ? otherAsItem.name : otherAsItem.ItemName + otherAsItem._indentifier);
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
                _itemType = item.ItemType;
                if (item.InventoryID != null && !item.InventoryID.Equals(""))
                {
                    ModuleManager.GetModule<InventoryManager>().GetByID(item.InventoryID).AddItem(this, 1);
                }

                if (item.IconPath != null)
                {
                    _icon = Resources.Load<Sprite>(item.IconPath);
                }
                _itemFields = item.Fields;
            }
        }

        protected override List<GameData> SaveData()
        {
            ItemData data = new ItemData();
            data.ItemName = _itemName;
            data.Fields = _itemFields;
            data.ItemType = _itemType;
            if (_inventoryOfItem != null)
            {
                HeldInventoryManager invMan = null;

                if (ModuleManager.ModuleRegistered<HeldInventoryManager>())
                {
                    invMan = ModuleManager.GetModule<HeldInventoryManager>();
                }

                if (invMan == null || !_inventoryOfItem.ID.Equals(invMan.HInventory.ID))
                {
                    data.InventoryID = _inventoryOfItem.ID;
                }
                else
                {
                    data.InventoryID = invMan.BackUpInventory.ID;
                }

                data.IconPath = IconPath;
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

        public void Click()
        {
            HeldInventoryManager invMan = ModuleManager.GetModule<HeldInventoryManager>();
            if (invMan.EnablePlacement)
            {
                invMan.ManageInventorys(null, this);
            }
        }
    }
}
