using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement3D : MovementBase
{
    private Rigidbody _rb;
    private float _jumpTimer;

    [SerializeField] private float _maxSlopeAngle;
    private RaycastHit _onSlope;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isMovementLocked)
        {
            Vector2 vector2 = _input.actions[_movementActionName].ReadValue<Vector2>();
            Move(new Vector3(vector2.x,0, vector2.y));
        }

        ModuleManager.GetModule<PlayerEventManager>().Move(GetCurrentVelocity(), _currentMovementState);
    }

    private void Update()
    {
        if (_groundedTransform != null)
        {
            RaycastHit hit;
            Physics.Raycast(_groundedTransform.position, transform.up * -1, out hit, 0.4f, _jumpingMask);
            _grounded = hit.collider != null;
        }

        if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;
        }

        if (_jumpActionName != "" && _input.actions[_jumpActionName].triggered && _grounded && _jumpTimer <= 0)
        {
            Jump();
            _jumpTimer = 0.3f;
        }

        if (_attackActionName != "" && _input.actions[_attackActionName].triggered)
        {
            ModuleManager.GetModule<PlayerEventManager>().Attack();
        }
    }

    private bool OnSlope()
    {
        if (_groundedTransform != null && Physics.Raycast(_groundedTransform.position, transform.up * -1, out _onSlope, 0.6f))
        {
            float angle = Vector3.Angle(Vector3.up, _onSlope.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _onSlope.normal).normalized;
    }

    public override void Jump()
    {
        if (!IsJumpingLocked)
        {
            _rb.AddForce(new Vector3(0, _jumpForce, 0));
        }
    }

    public override void Move(Vector3 direction)
    {
        if (OnSlope())
        {
            direction = GetSlopeDirection(direction);
        }

        if (_climbing && !IsClimbingLocked)
        {
            _rb.AddForce(Vector3.Scale(new Vector3(0, direction.z * -_speed, 0), new Vector3(0, Camera.main.transform.forward.y, 0)));
            _rb.AddForce(Vector3.Scale(new Vector3(direction.x * _speed, 0, direction.x * _speed), new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized));
        }
        else if(!_isMovementLocked)
        {
            _rb.AddForce(Vector3.Scale(new Vector3(direction.z * _speed, direction.y * _speed, direction.z * _speed), new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z).normalized));
            _rb.AddForce(Vector3.Scale(new Vector3(direction.x * _speed, direction.y * _speed, direction.x * _speed), new Vector3(Camera.main.transform.right.x, Camera.main.transform.right.y, Camera.main.transform.right.z).normalized));
        }

        if(direction.x != 0 || direction.z != 0)
        {
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                (Vector3.Scale(new Vector3(direction.z, 0, direction.z), Camera.main.transform.forward) +
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

    private void OnCollisionEnter(Collision collision)
    {
        if ((1 << collision.gameObject.layer).Equals(_climbingMask))
        {
            _climbing = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((1 << collision.gameObject.layer).Equals(_climbingMask))
        {
            _climbing = false;
        }
    }
}
