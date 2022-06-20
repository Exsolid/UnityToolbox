using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEventmanager : Module
{
    public Action<Vector3> move;
    public Action<bool> lockMove;
    public Action attack;

    public void Move(Vector3 currentVelocity)
    {
        if (move != null)
        {
            move(currentVelocity);
        }
    }
       
    public void LockMove(bool locked)
    {
        if (lockMove != null)
        {
            lockMove(locked);
        }
    }

    public void Attack()
    {
        if(attack != null)
        {
            attack();
        }
    }
}
