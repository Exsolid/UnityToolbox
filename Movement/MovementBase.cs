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
    [SerializeField] protected string _attackActionName;

    [SerializeField] protected Transform _groundedTransform;
    public Transform GroundedTransform 
    { 
        get { return _groundedTransform; } 
    }

    [SerializeField] protected PlayerInput _input;
    protected bool _isMovementLocked;

    protected MovementState _currentMovementState;
    public MovementState CurrentMovementState 
    { 
        get { return _currentMovementState; } 
    }

    [SerializeField] protected LayerMask _climbingMask;

    protected bool _trueGrounded;
    protected bool _onLadder;

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
        if (_onLadder)
        {
            _currentMovementState = MovementState.Climbing;
        }
        else if(!_trueGrounded)
        {
            _currentMovementState = MovementState.Jumping;
        }
        else
        {
            _currentMovementState = MovementState.Moving;
        }
    }

    public abstract void Move(Vector3 direction);
    public abstract void MoveWithStrength(Vector3 direction, Vector3 strength);
    public abstract Vector3 GetCurrentVelocity();
}

public enum MovementState{
    Moving,
    Climbing,
    Jumping
}
