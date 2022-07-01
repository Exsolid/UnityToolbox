using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[InitializeOnLoad]
public class MasterSceneLoaderForEditor: MonoBehaviour
{
    static MasterSceneLoaderForEditor()
    {
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/!Scenes/Master.unity");
        if (myWantedStartScene != null)
        {
            if (EditorSceneManager.playModeStartScene != null && EditorSceneManager.playModeStartScene.Equals(myWantedStartScene)) return;
            EditorSceneManager.playModeStartScene = myWantedStartScene;
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
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/!Scenes/Master.unity");
        if (myWantedStartScene != null && EditorSceneManager.playModeStartScene != null && EditorSceneManager.playModeStartScene.Equals(myWantedStartScene))
        {
            Debug.Log("'Master' scene has been set up.");
            Debug.Log("Setting up '" + EditorSceneManager.GetActiveScene().name + "' scene for load priority.");
            PlayerPrefs.SetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE, EditorSceneManager.GetActiveScene().name);
        }
    }
}
