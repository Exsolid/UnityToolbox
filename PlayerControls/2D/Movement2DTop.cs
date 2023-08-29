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
    [SerializeField] private Camera _camera;
    private Rigidbody2D _rb;
    [SerializeField] private bool _rotateToMouse;
    [SerializeField] private float _turnSpeed;
    private Quaternion _baseRotation;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _grounded = true;
        _baseRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isToggledMoving = _input.actions[_toggleSpeedActionName].IsPressed();
        UpdateMovementState();

        if (!_isMovementLocked)
        {
            Move(_input.actions[_movementActionName].ReadValue<Vector2>());
        }

        if (!_isMovementLocked && _rotateToMouse)
        {
            RotateToMouse();
        }

        ModuleManager.GetModule<PlayerEventManager>().Move(_rb.velocity, _currentMovementState);
    }

    private void RotateToMouse()
    {
        Vector3 goal = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        goal.z = transform.position.z;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, _baseRotation.eulerAngles.z) * (goal - transform.position);

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
    }

    public override void Move(Vector3 direction)
    {
        if (_isToggledMoving)
        {
            _rb.AddForce(new Vector3(direction.x * _toggledSpeed,
                direction.y * _toggledSpeed));
        }
        else
        {
            _rb.AddForce(new Vector3(direction.x * _speed,
                direction.y * _speed));
        }
    }

    public override void MoveWithStrength(Vector3 direction, Vector3 strength)
    {
        Vector3 scaled = Vector3.Scale(direction, strength);
        if (_input.actions[_toggleSpeedActionName].IsPressed())
        {
            _rb.AddForce(new Vector3(scaled.x * _toggledSpeed,
                scaled.y * _toggledSpeed));
        }
        else
        {
            _rb.AddForce(new Vector3(scaled.x * _speed,
                scaled.y * _speed));
        }
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
