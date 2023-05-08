using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// An implementation of <see cref="MovementBase"/> which works for a side scroller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Movement2DTop : MovementBase
{
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovementState();

        if (!_isMovementLocked)
        {
            Move(_input.actions[_movementActionName].ReadValue<Vector2>());
        }

        ModuleManager.GetModule<PlayerEventManager>().Move(_rb.velocity, _currentMovementState);
    }

    public override void Move(Vector3 direction)
    {
        _rb.AddForce(new Vector3(direction.x * _speed,
            direction.y * _speed)); 
    }

    public override void MoveWithStrength(Vector3 direction, Vector3 strength)
    {
        Vector3 scaled = Vector3.Scale(direction, strength);

        _rb.AddForce(new Vector3(scaled.x * _speed,
            scaled.y * _speed));
    }

    public override Vector3 GetCurrentVelocity()
    {
        if(_rb == null)
        {
            return Vector3.zero;
        }
        return _rb.velocity;
    }

    public override void Jump()
    {
        throw new System.NotImplementedException();
    }
}
