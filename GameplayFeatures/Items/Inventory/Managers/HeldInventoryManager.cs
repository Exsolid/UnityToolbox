using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolbox.GameplayFeatures.Items.Inventory.Types;
using UnityToolbox.General;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.Items.Inventory.Managers
{
    public class HeldInventoryManager : Module
    {
        [SerializeField] public HeldInventory HInventory;

        [SerializeField] private Camera _camera;

        [SerializeField] private string _mouseClickAction;
        [SerializeField] private PlayerInput _input;

        [SerializeField] public bool EnablePlacement;
        [SerializeField] private LayerMask _placementMask;
        [SerializeField] private GameObject _placementParent;

        [SerializeField] private InventoryBase _backUpInventory;
        public InventoryBase BackUpInventory 
        { 
            get { return _backUpInventory; } 
        }

        private bool _mayAct = true;
        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { _isEnabled = value; } }

        public void Update()
        {
            if (!_mayAct || !_isEnabled)
            {
                return;
            }

            if (HInventory.ItemHeld != null)
            {
                if (EnablePlacement)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit, 50, _placementMask))
                    {
                        LayerMask _invertedMask =~ _placementMask;
                        HInventory.ItemHeld.transform.position = Vector3.Lerp(hit.point, HInventory.ItemHeld.gameObject.transform.position, 0.2f);
                        if (_input.actions[_mouseClickAction].triggered && !HInventory.ItemHeld.GetComponent<ColliderInfo>().GetAllCollisions(_invertedMask).Any())
                        {
                            if(HInventory.Count > 1)
                            {
                                Instantiate(HInventory.ItemHeld).transform.SetParent(_placementParent.transform);
                            }
                            else
                            {
                                HInventory.ItemHeld.transform.SetParent(_placementParent.transform);
                            }

                            HInventory.RemoveItem(HInventory.ItemHeld, 1);
                            StartCoroutine(WaitTillNextAction());
                        }
                    }
                }
                else
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    HInventory.ItemHeld.gameObject.transform.position = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.nearClipPlane + 1));
                }
            }
        }

        public void ManageInventorys(BoundInventory other, ItemInstance otherItem)
        {
            if (!_mayAct || !_isEnabled)
            {
                return;
            }

            if (other != null && (otherItem == null && HInventory.ItemHeld != null || HInventory.ItemHeld != null && HInventory.ItemHeld.Equals(otherItem)))
            {
                if (other.AddItem(HInventory.ItemHeld, 1))
                {
                    HInventory.RemoveItem(HInventory.ItemHeld, 1);
                }

                return;
            }
            else if (HInventory.ItemHeld == null && otherItem != null)
            {
                if (HInventory.AddItem(otherItem, 1))
                {
                    if (other != null)
                    {
                        other.RemoveItem(otherItem, 1);
                    }
                }
                return;
            }
            else if(other != null && otherItem != null)
            {
                ItemInstance held = HInventory.ItemHeld;
                int heldCount = HInventory.Count;
                int currentCount = other.Items[otherItem];

                other.RemoveItem(otherItem);
                HInventory.RemoveItem(held);

                if (other.AddItem(held, heldCount))
                {
                    if (!HInventory.AddItem(otherItem, currentCount))
                    {
                        other.RemoveItem(held, heldCount);
                        other.AddItem(otherItem, currentCount);
                        HInventory.AddItem(held, heldCount);
                    }
                }
                else
                {
                    other.AddItem(otherItem, currentCount);
                    HInventory.AddItem(held, heldCount);
                }

                return;
            }
        }

        IEnumerator WaitTillNextAction()
        {
            _mayAct = false;
            yield return new WaitForSeconds(0.5f);
            _mayAct = true;
        }
    }
}

