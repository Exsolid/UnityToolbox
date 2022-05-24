using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GotoSceneButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private string sceneName;
    [SerializeField] private Canvas parentCanvas;

    public void OnPointerDown(PointerEventData data)
    {
        if (parentCanvas.enabled) SceneManager.LoadScene(sceneName);
    }
}
