using System.Collections;
using UnityEngine;

namespace UnityToolbox.General.Management
{
    /// <summary>
    /// The base of a module. Required if you want anything to be called from the <see cref="ModuleManager"/>.
    /// </summary>
    public class Module : MonoBehaviour
    {
        public virtual void Awake()
        {
            if(!ModuleManager.IsLoaded()) StartCoroutine(WaitTillMasterSceneLoaded());
            else if(!ModuleManager.RegisterModul(this))
            {
                throw new System.Exception(string.Format("An object of type {0} has already been registered.", this.GetType()));
            }
        }

        IEnumerator WaitTillMasterSceneLoaded()
        {
            while (!ModuleManager.IsLoaded())
            {
                yield return null;
            }

            if (!ModuleManager.RegisterModul(this))
            {
                throw new System.Exception(string.Format("An object of type {0} has already been registered.", this.GetType()));
            }
        }

        private void OnDestroy()
        {
            ModuleManager.DeregisterModul(this);
        }
    }
}
