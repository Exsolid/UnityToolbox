using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIInventory : MonoBehaviour
{
    private List<UIInventorySlot> _slots;
    [SerializeField] private Inventory _inventory;

    private void Start()
    {
        _slots = GetComponentsInChildren<UIInventorySlot>().ToList();
        UpdateSlots();
        _inventory.InventoryChanged += UpdateSlots;
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
            if (_inventory.Items.Count == i)
            {
                break;
            }
            _slots[i].UpdateItem(_inventory.Items.Keys.ElementAt(i), _inventory.Items.Values.ElementAt(i));
        }
    }
}
