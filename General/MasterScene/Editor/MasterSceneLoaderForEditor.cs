using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using UnityToolbox.General.Preferences;

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
            Debug.Log("Setting up 'Master' scene for load priority.");
        }
        else
        {
            Debug.Log("No scene called 'Master' has been found. It is needed for scene overlapping managers.");
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
            Debug.Log("'Master' scene has been set up.");
            Debug.Log("Setting up '" + EditorSceneManager.GetActiveScene().name + "' scene for load priority.");
            PlayerPrefs.SetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE, EditorSceneManager.GetActiveScene().name);
        }
    }
}
