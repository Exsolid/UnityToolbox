using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public virtual void Awake()
    {
        if(!ModuleManager.isLoaded()) StartCoroutine(waitTillMasterSceneLoaded());
        else if(!ModuleManager.registerModul(this))
        {
            throw new System.Exception(string.Format("An object of type {0} has already been registered.", this.GetType()));
        }
    }

    IEnumerator waitTillMasterSceneLoaded()
    {
        while (!ModuleManager.isLoaded())
        {
            yield return null;
        }
        if (!ModuleManager.registerModul(this))
        {
            throw new System.Exception(string.Format("An object of type {0} has already been registered.", this.GetType()));
        }
    }
}
