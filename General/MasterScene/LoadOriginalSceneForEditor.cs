using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.General.Management.Logging;
using Logger = UnityToolbox.General.Management.Logging.Logger;

namespace UnityToolbox.General.MasterScene
{
    /// <summary>
    /// A script placed within the master scene. It loads the scene which was opened when pressing play in editor. This is required as the master scene is always loaded first.
    /// </summary>
    public class LoadOriginalSceneForEditor : MonoBehaviour
    {
        void Start()
        {
            if (!Application.isEditor)
            {
                return;
            }
            Logger.Log(LogLevel.INF, typeof(LoadOriginalSceneForEditor), "Now loading scene '" + PlayerPrefs.GetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE) +"'");
            SceneManager.LoadSceneAsync(PlayerPrefs.GetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE));
        }
    }
}
