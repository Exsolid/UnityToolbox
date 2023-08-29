using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.Items.Inventory.Types;

namespace UnityToolbox.GameplayFeatures.Items.Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        private List<InventoryUISlot> _slots;
        [SerializeField] private BoundInventory _inventory;

        private void Start()
        {
            _slots = GetComponentsInChildren<InventoryUISlot>().ToList();
            UpdateSlots();
            _inventory.OnInventoryChanged += UpdateSlots;
        }

        private void UpdateSlots()
        {
            if (_inventory.Items.Count > _slots.Count)
            {
                Debug.LogWarning("The inventory is bigger than its given UI. Not all items can be shown.");
                return;
            }
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].InventoryOfItem = _inventory;
                if (_inventory.Items.Count <= i)
                {
                    _slots[i].UpdateItem(null, 0);
                    continue;
                }
                _slots[i].UpdateItem(_inventory.Items.Keys.ElementAt(i), _inventory.Items.Values.ElementAt(i));
            }
        }
    }
}
