using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GotoSceneButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private string sceneName;

    public void OnPointerDown(PointerEventData data)
    {
        SceneManager.LoadScene(sceneName);
    }
}
