using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadOriginalSceneForEditor : MonoBehaviour
{
    void Start()
    {
        if (!Application.isEditor)
        {
            return;
        }
        Debug.Log("Now loading scene '"+ PlayerPrefs.GetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE) +"'");
        SceneManager.LoadSceneAsync(PlayerPrefs.GetString(PlayerPrefKeys.DEBUG_ORIGINAL_SCENE));
    }
}
