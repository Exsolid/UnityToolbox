using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement3D : MovementBase
{
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isMovementLocked)
        {
            Move(_input.actions[_movementActionName].ReadValue<Vector2>());
        }

        ModuleManager.GetModule<PlayerEventManager>().Move(GetCurrentVelocity(), _currentMovementState);

        if (_input.actions[_attackActionName].triggered)
        {
            ModuleManager.GetModule<PlayerEventManager>().Attack();
        }
    }

    public override void Move(Vector3 direction)
    {
        _rb.AddForce(Vector3.Scale(new Vector3(direction.y * _speed, 0, direction.y * _speed), new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized));
        _rb.AddForce(Vector3.Scale(new Vector3(direction.x * _speed, 0, direction.x * _speed), new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized));

        if(direction.x != 0 || direction.y != 0)
        {
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                (Vector3.Scale(new Vector3(direction.y, 0, direction.y), Camera.main.transform.forward) +
                Vector3.Scale(new Vector3(direction.x, 0, direction.x), Camera.main.transform.right))/2f
                , Vector3.up), 0.5f);
            transform.rotation = rotation;
        }
    }

    public override Vector3 GetCurrentVelocity()
    {
        return _rb.velocity;
    }

    public override void MoveWithStrength(Vector3 direction, Vector3 strength)
    {
        throw new System.NotImplementedException();
    }
}
