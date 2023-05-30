using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityToolbox.Item.Management;

namespace UnityToolbox.Item
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

        private HashSet<ItemField> _itemFields;
        public HashSet<ItemField> ItemFields
        {
            get { return _itemFields; }
            set { _itemFields = value; }
        }

        public dynamic GetValueForField(string fieldName)
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