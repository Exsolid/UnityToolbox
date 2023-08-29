using UnityEngine;

namespace UnityToolbox.GameplayFeatures.Items.Inventory.Types
{
    public class HeldInventory : InventoryBase
    {
        private ItemInstance _itemHeld;
        public ItemInstance ItemHeld 
        { 
            get { return _itemHeld; } 
        }

        private int _count;
        public int Count
        { 
            get { return _count; } 
        }

        public override bool AddItem(ItemInstance item, int count)
        {
            if (_itemHeld != null && !item.Equals(_itemHeld))
            {
                return false;
            }

            item.Inventory = this;
            if(_itemHeld == null)
            {
                _itemHeld = item;
                _itemHeld.GetComponent<Renderer>().enabled = true;
            }

            _count++;
            return true;
        }

        public override void RemoveItem(ItemInstance item, int count)
        {
            if (_itemHeld == null || !item.Equals(_itemHeld))
            {
                return;
            }

            _count--;
            if(_count == 0)
            {
                if (item.Inventory.Equals(this))
                {
                    item.Inventory = null;
                }

                _itemHeld = null;
            }
        }

        public override void UpdateItem(ItemInstance old, ItemInstance updated)
        {
            if (_itemHeld != null && _itemHeld.Equals(old))
            {
                RemoveItem(old, 1);
                AddItem(updated, 1);
            }
        }

        public override void RemoveItem(ItemInstance item)
        {
            _count = 0;
            _itemHeld = null;
        }
    }
}