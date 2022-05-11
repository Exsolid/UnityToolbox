using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modul : MonoBehaviour
{
    public virtual void Awake()
    {
        if (!ModulManager.registerModul(this))
        {
            throw new System.Exception(string.Format("An object of type {0} has already been registered.", this.GetType()));
        }
    }
}
