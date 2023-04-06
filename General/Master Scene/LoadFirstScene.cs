using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    // Start is called before the first frame update
    void Start()
    {
        if(!Application.isEditor)SceneManager.LoadScene(_sceneName);
    }
}
