using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MovementBase
{
    [SerializeField] private Transform _groundedTransformTwo;
    public Transform GroundedTransformTwo
    {
        get { return _groundedTransformTwo; }
    }

    private RaycastHit2D _groundedHit;
    private Rigidbody2D _rb;
    private float _oldGravityScale; 

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _oldGravityScale = _rb.gravityScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int everyMaskExcept = ~(1 << gameObject.layer);
        _groundedHit = Physics2D.Raycast(_groundedTransform.position, transform.up * -1, 0.01f, everyMaskExcept);
        if(_groundedHit.collider == null)
        {
            _groundedHit = Physics2D.Raycast(_groundedTransformTwo.position, transform.up * -1, 0.01f, everyMaskExcept);
        }


        UpdateMovementState();

        if (_groundedHit.collider != null && !base._grounded)
        {
            StartCoroutine(DelayGrounded());
        }
        else if (_groundedHit.collider == null)
        {
            _grounded = false;
            _rb.gravityScale = _oldGravityScale;
        }

        if (!_isMovementLocked)
        {
            Move(_input.actions[_movementActionName].ReadValue<Vector2>());
        }

        ModuleManager.GetModule<PlayerEventManager>().Move(_rb.velocity, _currentMovementState);
    }

    public override void Move(Vector3 direction)
    {
        if(!base._grounded && _currentMovementState != MovementState.Climbing)
        {
            direction.y = 0;
        }

        _rb.AddForce(new Vector3(direction.x * _speed, direction.y 
            * (_currentMovementState == MovementState.Climbing ? _climbingForce : _jumpForce) 
            * (direction.x == 0 ? 1 : 1.5f), 0));
    }

    public override void MoveWithStrength(Vector3 direction, Vector3 strength)
    {
        Vector3 scaled = Vector3.Scale(direction, strength);
        _rb.AddForce(new Vector3(scaled.x * _speed, 
            scaled.y
            * (_currentMovementState == MovementState.Climbing ? _climbingForce : _jumpForce)
            * (scaled.x == 0 ? 1 : 1.5f), 0));
    }

    public override Vector3 GetCurrentVelocity()
    {
        if(_rb == null)
        {
            return Vector3.zero;
        }
        return _rb.velocity;
    }

    IEnumerator DelayGrounded()
    {
        yield return new WaitForSeconds(0.1f);
        base._grounded = true;
        _rb.gravityScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer).Equals(_climbingMask))
        {
            _climbing = true;
            _rb.gravityScale = 0;
            _rb.drag *= 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer).Equals(_climbingMask))
        {
            _rb.drag /= 10;
            _rb.gravityScale = _oldGravityScale;
            _climbing = false;
        }
    }
}
