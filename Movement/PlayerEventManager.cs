using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEventManager : Module
{
    public Action<Vector3, MovementState> OnMove;
    public Action<bool> OnLockMove;
    public Action OnAttack;

    public void Move(Vector3 currentVelocity, MovementState state)
    {
        if (OnMove != null)
        {
            OnMove(currentVelocity, state);
        }
    }
       
    public void LockMove(bool locked)
    {
        if (OnLockMove != null)
        {
            OnLockMove(locked);
        }
    }

    public void Attack()
    {
        if(OnAttack != null)
        {
            OnAttack();
        }
    }
}
