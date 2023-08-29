using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolbox.General.Management;

namespace UnityToolbox.PlayerControls
{
    /// <summary>
    /// The base implementation of all player movement.
    /// </summary>
    public abstract class MovementBase : MonoBehaviour
    {
        /// <summary>
        /// Defines the speed of the player.
        /// </summary>
        [SerializeField] protected float _speed;
        /// <summary>
        /// Defines the toggledSpeed of the player.
        /// </summary>
        [SerializeField] protected float _toggledSpeed;
        /// <summary>
        /// Defines the amount of force (speed) for a jump.
        /// </summary>
        [SerializeField] protected float _jumpForce;
        /// <summary>
        /// Defines the amount of force (speed) for climbing.
        /// </summary>
        [SerializeField] protected float _climbingForce;
        /// <summary>
        /// The action name for the movement controls. Needs to be defined as a Vector2D.
        /// </summary>
        [SerializeField] protected string _movementActionName;
        /// <summary>
        /// The action name for the changing the players speed.
        /// </summary>
        [SerializeField] protected string _toggleSpeedActionName;
        /// <summary>
        /// The action name for the jump control.
        /// </summary>
        [SerializeField] protected string _jumpActionName;
        /// <summary>
        /// The action name for the attack control.
        /// </summary>
        [SerializeField] protected string _attackActionName;

        /// <summary>
        /// The transform defines the position where to define if the player is grounded or not.
        /// </summary>
        [SerializeField] protected Transform _groundedTransform;
        public Transform GroundedTransform 
        { 
            get { return _groundedTransform; } 
        }

        /// <summary>
        /// The player input.
        /// </summary>
        [SerializeField] protected PlayerInput _input;
        /// <summary>
        /// Defines whether the movement should be locked or not. The <see cref="PlayerEventManager"/> also has events for this.
        /// </summary>
        protected bool _isMovementLocked;
        public bool IsMovementLocked 
        { 
            get { return _isMovementLocked; } 
            set { _isMovementLocked = value; }
        }

        /// <summary>
        /// Defines whether the climbing should be locked or not.
        /// </summary>
        protected bool _isClimbingLocked;
        public bool IsClimbingLocked
        {
            get { return _isClimbingLocked; }
            set { _isClimbingLocked = value; }
        }

        /// <summary>
        /// Defines whether the jumping should be locked or not.
        /// </summary>
        protected bool _isJumpingLocked;
        public bool IsJumpingLocked
        {
            get { return _isJumpingLocked; }
            set { _isJumpingLocked = value; }
        }

        /// <summary>
        /// The current movement state. (<see cref="MovementState.Climbing"/>, <see cref="MovementState.Moving"/>, <see cref="MovementState.Jumping"/>)
        /// <see cref="MovementState.Moving"/> does not mean that the current velocity is > 0.
        /// </summary>
        protected MovementState _currentMovementState;
        public MovementState CurrentMovementState 
        { 
            get { return _currentMovementState; } 
        }

        /// <summary>
        /// The climbing layer mask, which defines which objects should be read as climbable.
        /// </summary>
        [SerializeField] protected LayerMask _climbingMask;

        /// <summary>
        /// The jumping layer mask, which defines which objects should be read as jumpable.
        /// </summary>
        [SerializeField] protected LayerMask _jumpingMask;

        /// <summary>
        /// Defines whether the player is grounded.
        /// </summary>
        protected bool _grounded;
        public bool Grounded 
        {
            get { return _grounded; } 
        }

        /// <summary>
        /// Defines whether the animation should be counted as grounded. A usecase would be a delay between the actual <see cref="MovementBase.Grounded"/>.
        /// </summary>
        protected bool _animationGrounded;
        public bool AnimationGrounded
        {
            get { return _animationGrounded; }
        }

        /// <summary>
        /// Defines whether the player is climbing.
        /// </summary>
        protected bool _climbing;
        public bool Climbing 
        {
            get { return _climbing; }
        }

        protected bool _isToggledMoving;
        public bool IsToggledMoving
        {
            get { return _isToggledMoving; }
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

        /// <summary>
        /// Updates the current state of movement. <see cref="MovementState.Moving"/> does not mean that the current velocity is > 0!
        /// </summary>
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
            else if(!_isToggledMoving)
            {
                _currentMovementState = MovementState.Moving;
            }
            else
            {
                _currentMovementState = MovementState.ToggleMoving;
            }
        }

        /// <summary>
        /// Updates the player movement with the <paramref name="direction"/> data.
        /// </summary>
        /// <param name="direction"></param>
        public abstract void Move(Vector3 direction);
        /// <summary>
        /// Updates the player movement to execute a jump.
        /// </summary>
        public abstract void Jump();
        /// <summary>
        /// Updates the player movement with the <paramref name="direction"/> data and defined <paramref name="strength"/>.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="strength"></param>
        public abstract void MoveWithStrength(Vector3 direction, Vector3 strength);
        /// <summary>
        /// </summary>
        /// <returns>Returns the current velocity of the player.</returns>
        public abstract Vector3 GetCurrentVelocity();
    }

    public enum MovementState{
        Moving,
        ToggleMoving,
        Climbing,
        Jumping
    }
}