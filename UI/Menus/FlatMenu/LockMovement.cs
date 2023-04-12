using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script which reads the status of the menu it is placed on. Disables movment when found active and vise versa.
/// </summary>
public class LockMovement : MonoBehaviour
{
    [SerializeField] private MovementBase _movement;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += UpdateMovement;
        UpdateMovement(false);
    }

    private void UpdateMovement(bool active)
    {
        _movement.IsMovementLocked = active;
    }
}
