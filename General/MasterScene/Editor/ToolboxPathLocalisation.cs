using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

/// <summary>
/// Defines the current path of the UnityToolbox within the project within the <see cref="ProjectPrefs"/>.
/// </summary>
[InitializeOnLoad]
public class ToolboxPathLocalizer
{
    static ToolboxPathLocalizer()
    {
        EditorApplication.delayCall += Init;
    }

    private static void Init()
    {
        string[] assetPath = Directory.GetDirectories(Application.dataPath, "UnityToolbox", SearchOption.AllDirectories);
        if (assetPath.Length > 0)
        {
            string path = assetPath[0].Replace(Application.dataPath, "");
            if (!path.Equals(ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH)))
            {
                ProjectPrefs.SetString(ProjectPrefKeys.UNITYTOOLBOXPATH, path);
                AssetDatabase.Refresh();

                Debug.Log("Registered UnityToolboxPath at: " + "[" + Application.dataPath + "] " + ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH));
            }
        }

        EditorApplication.delayCall -= Init;
    }
}
