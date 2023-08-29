using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// An implementation of <see cref="MovementBase"/> which works for a side scroller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Movement2DSide : MovementBase
{
    [SerializeField] private Transform _groundedTransformTwo;
    public Transform GroundedTransformTwo
    {
        get { return _groundedTransformTwo; }
    }

    private RaycastHit2D _groundedHit;
    private Rigidbody2D _rb;
    private float _oldGravityScale;

    private bool _mayJump;

    [SerializeField] private LayerMask _layerMasksToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _oldGravityScale = _rb.gravityScale;
        _mayJump = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int everyMaskExcept = ~(_layerMasksToIgnore);

        _groundedHit = Physics2D.Raycast(_groundedTransform.position, transform.up * -1, 0.3f, everyMaskExcept);
        if (_groundedHit.collider == null)
        {
            _groundedHit = Physics2D.Raycast(_groundedTransformTwo.position, transform.up * -1, 0.3f, everyMaskExcept);
        }
        _animationGrounded = _groundedHit.collider != null;

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
            StartCoroutine(DelayUngrounded());
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

        _rb.AddForce(new Vector3(direction.x * (_currentMovementState == MovementState.Climbing ? _climbingForce : _speed), direction.y 
            * (_currentMovementState == MovementState.Climbing ? _climbingForce : _mayJump ? _jumpForce : 0) 
            * (direction.x == 0 ? 1 : 1.5f), 0));
        if(direction.y != 0)
        {
            _mayJump = false;
            StartCoroutine(PauseJumping());
        }
    }

    public override void MoveWithStrength(Vector3 direction, Vector3 strength)
    {
        Vector3 scaled = Vector3.Scale(direction, strength);
        _rb.AddForce(new Vector3(scaled.x * (_currentMovementState == MovementState.Climbing ? _climbingForce : _speed), 
            scaled.y
            * (_currentMovementState == MovementState.Climbing ? _climbingForce : _mayJump ? _jumpForce : 0)
            * (scaled.x == 0 ? 1 : 1.5f), 0));
        if (direction.y != 0)
        {
            _mayJump = false;
            StartCoroutine(PauseJumping());
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

    IEnumerator DelayGrounded()
    {
        yield return new WaitForSeconds(0.1f);
        base._grounded = true;
        _rb.gravityScale = 0;
    }    
    
    IEnumerator DelayUngrounded()
    {
        yield return new WaitForSeconds(0.1f);

        int everyMaskExcept = ~(_layerMasksToIgnore);

        RaycastHit2D localGroundedHit = Physics2D.Raycast(_groundedTransform.position, transform.up * -1, 0.3f, everyMaskExcept);

        localGroundedHit = Physics2D.Raycast(_groundedTransform.position, transform.up * -1, 0.01f, everyMaskExcept);
        if (localGroundedHit.collider == null)
        {
            localGroundedHit = Physics2D.Raycast(_groundedTransformTwo.position, transform.up * -1, 0.01f, everyMaskExcept);
        }

        if (localGroundedHit.collider == null)
        {
            _grounded = false;
            _rb.gravityScale = _oldGravityScale;
        }
    }

    IEnumerator PauseJumping()
    {
        yield return new WaitForSeconds(0.2f);
        _mayJump = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer).Equals(_climbingMask))
        {
            _climbing = true;
            _rb.gravityScale = 0;
            _rb.drag *= 2;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer).Equals(_climbingMask))
        {
            _rb.drag /= 2;
            _rb.gravityScale = _oldGravityScale;
            _climbing = false;
        }
    }

    public override void Jump()
    {
        throw new System.NotImplementedException();
    }
}
