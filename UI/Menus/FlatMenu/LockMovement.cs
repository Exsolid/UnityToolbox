using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
