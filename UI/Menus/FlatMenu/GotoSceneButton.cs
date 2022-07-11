using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GotoSceneButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private string _sceneName;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => 
        { 
            _isEnabled = isActive;
        };
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (_isEnabled)
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
