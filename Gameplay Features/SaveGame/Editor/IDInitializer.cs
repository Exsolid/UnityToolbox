using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
