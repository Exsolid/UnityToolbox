using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using UnityToolbox.General.Preferences;
using UnityToolbox.General.Management;
using UnityToolbox.General.Management.Logging;
using Logger = UnityToolbox.General.Management.Logging.Logger;

namespace UnityToolbox.General.MasterScene.Editor
{
    /// <summary>
    /// Sets up the master scene to load initially while the game is running in editor.
    /// </summary>
    [InitializeOnLoad]
    public class MasterSceneLoaderForEditor
    {
        static MasterSceneLoaderForEditor()
        {
            string sceneGUID = AssetDatabase.FindAssets("Master t:Scene").FirstOrDefault();
            SceneAsset masterScene = null;
            if (sceneGUID != null && !sceneGUID.Equals(""))
            {
                masterScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneGUID));
            }

            if (masterScene != null)
            {
                if (EditorSceneManager.playModeStartScene != null && EditorSceneManager.playModeStartScene.Equals(masterScene)) return;
                EditorSceneManager.playModeStartScene = masterScene;
                Logger.Log(LogLevel.INF, typeof(MasterSceneLoaderForEditor), "Setting up 'Master' scene for load priority.");
            }
            else
            {
                Logger.Log(LogLevel.INF, typeof(MasterSceneLoaderForEditor), "No scene called 'Master' has been found. It is needed for scene overlapping managers.");
            }
        }

        [InitializeOnEnterPlayMode]
        static void OnEnterPlaymodeInEditor(EnterPlayModeOptions options)
        {
            string sceneGUID = AssetDatabase.FindAssets("Master t:Scene").FirstOrDefault();
            SceneAsset masterScene = null;
            if (sceneGUID != null && !sceneGUID.Equals(""))
            {
                masterScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneGUID));
            }

            if (masterScene != null && EditorSceneManager.playModeStartScene != null && EditorSceneManager.playModeStartScene.Equals(masterScene))
            {
                Logger.Log(LogLevel.INF, typeof(MasterSceneLoaderForEditor), "'Master' scene has been set up.");
                Logger.Log(LogLevel.INF, typeof(ModuleManager), "Setting up '" + EditorSceneManager.GetActiveScene().name + "' scene for load priority.");
                PlayerPrefs.SetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE, EditorSceneManager.GetActiveScene().name);
            }
        }
    }
}