using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A script which reads the status of the menu it is placed on. Disables movment when found active and vise versa.
    /// </summary>
    public class LockMovement : MonoBehaviour
    {
        [SerializeField] private bool _lateUpdateLocking;

        public void Awake()
        {
            GetComponentInParent<Menu>().OnActiveChanged += UpdateMovement;
            UpdateMovement(false);
        }

        private void UpdateMovement(bool active)
        {
            if (!_lateUpdateLocking)
            {
                ModuleManager.GetModule<PlayerEventManager>().LockMove(active);
            }
            else
            {
                StartCoroutine(LateUpdateUnlock(active));
            }
        }

        IEnumerator LateUpdateUnlock(bool active)
        {
            yield return new WaitForEndOfFrame();
            ModuleManager.GetModule<PlayerEventManager>().LockMove(active);
        }
    }

}