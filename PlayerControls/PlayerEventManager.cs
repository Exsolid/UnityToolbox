using System;
using UnityEngine;
using UnityToolbox.General.Management;

namespace UnityToolbox.PlayerControls
{
    public class PlayerEventManager : Module
    {
        /// <summary>
        /// An event which gives updates about the current velocity and movement state of the player. <see cref="MovementState.Moving"/> does not mean the velocity is > 0!
        /// </summary>
        public event Action<Vector3, MovementState> OnMove;
        /// <summary>
        /// An event which is called once the player movement should be (un)locked.
        /// </summary>
        public event Action<bool> OnLockMove;
        /// <summary>
        /// An event which is called once the player is attacking.
        /// </summary>
        public event Action OnAttack;

        /// <summary>
        /// Should be called for each movement update of the player.
        /// </summary>
        /// <param name="currentVelocity">The players velocity</param>
        /// <param name="state">The current state.</param>
        public void Move(Vector3 currentVelocity, MovementState state)
        {
            OnMove?.Invoke(currentVelocity, state);
        }
    
        /// <summary>
        /// Should be called to (un)lock player movement.
        /// </summary>
        /// <param name="locked"></param>
        public void LockMove(bool locked)
        {
            OnLockMove?.Invoke(locked);
        }

        /// <summary>
        /// Should be called on player attacks.
        /// </summary>
        public void Attack()
        {
            OnAttack?.Invoke();
        }
    }
}
