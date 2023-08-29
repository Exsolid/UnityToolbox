using UnityToolbox.GameplayFeatures.SaveGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Initializes the <see cref="IDManager"/> for scene changes in the editor.
/// </summary>
[InitializeOnLoad]
public class IDInitializer: MonoBehaviour
{
    static IDInitializer()
    {
        EditorSceneManager.activeSceneChangedInEditMode += (sceneOne, sceneTwo) => 
        {
            IDManager.SceneChanged(sceneTwo); 
        };

        AssemblyReloadEvents.afterAssemblyReload += () => 
        { 
            IDManager.SceneChanged(EditorSceneManager.GetActiveScene()); 
        };
    }
}
