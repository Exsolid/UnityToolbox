using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MovementBase : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _climbingForce;
    [SerializeField] protected string _movementActionName;
    [SerializeField] protected string _jumpActionName;
    [SerializeField] protected string _attackActionName;

    [SerializeField] protected Transform _groundedTransform;
    public Transform GroundedTransform 
    { 
        get { return _groundedTransform; } 
    }

    [SerializeField] protected PlayerInput _input;
    protected bool _isMovementLocked;
    public bool IsMovementLocked 
    { 
        get { return _isMovementLocked; } 
        set { _isMovementLocked = value; }
    }

    protected bool _isClimbingLocked;
    public bool IsClimbingLocked
    {
        get { return _isClimbingLocked; }
        set { _isClimbingLocked = value; }
    }

    protected bool _isJumpingLocked;
    public bool IsJumpingLocked
    {
        get { return _isJumpingLocked; }
        set { _isJumpingLocked = value; }
    }
    protected MovementState _currentMovementState;
    public MovementState CurrentMovementState 
    { 
        get { return _currentMovementState; } 
    }

    [SerializeField] protected LayerMask _climbingMask;
    [SerializeField] protected LayerMask _jumpingMask;

    protected bool _grounded;
    public bool Grounded 
    {
        get { return _grounded; } 
    }

    protected bool _animationGrounded;
    public bool AnimationGrounded
    {
        get { return _animationGrounded; }
    }
    protected bool _climbing;
    public bool Climbing 
    {
        get { return _climbing; }
    }

    private void Awake()
    {
        if(ModuleManager.GetModule<PlayerEventManager>() != null)
        {
            ModuleManager.GetModule<PlayerEventManager>().OnLockMove += (locked) =>
            {
                _isMovementLocked = locked;
            };
        }
    }

    public void UpdateMovementState()
    {
        if (_climbing)
        {
            _currentMovementState = MovementState.Climbing;
        }
        else if(!_grounded)
        {
            _currentMovementState = MovementState.Jumping;
        }
        else
        {
            _currentMovementState = MovementState.Moving;
        }
    }

    public abstract void Move(Vector3 direction);
    public abstract void Jump();
    public abstract void MoveWithStrength(Vector3 direction, Vector3 strength);
    public abstract Vector3 GetCurrentVelocity();
}

public enum MovementState{
    Moving,
    Climbing,
    Jumping
}
