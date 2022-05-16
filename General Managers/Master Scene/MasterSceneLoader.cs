using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class MasterSceneLoader : MonoBehaviour
{
    [SerializeField] private string masterSceneName;
    // Start is called before the first frame update
    void Awake()
    {
        bool masterSceneLoaded = false;
        SceneManager.LoadSceneAsync(masterSceneName, LoadSceneMode.Additive);
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == masterSceneName)
            {
                masterSceneLoaded = true;
            }
        }
        if (!masterSceneLoaded)
        {
            SceneManager.LoadSceneAsync(masterSceneName, LoadSceneMode.Additive);
        }
    }
}
