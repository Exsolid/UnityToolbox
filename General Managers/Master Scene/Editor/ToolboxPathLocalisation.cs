using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class ToolboxPathLocalizer : MonoBehaviour
{
    static ToolboxPathLocalizer()
    {
        string[] assetPath = Directory.GetDirectories(Application.dataPath, "UnityToolbox", SearchOption.AllDirectories);
        if(assetPath.Length > 0)
        {
            string path = assetPath[0].Replace(Application.dataPath, "");
            if (!path.Equals(ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH)))
            {
                ProjectPrefs.SetString(ProjectPrefKeys.UNITYTOOLBOXPATH, path);
                Debug.Log("Registered UnityToolboxPath at: " + "[" + Application.dataPath + "] " + ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH));
            }
        }
    }
}
