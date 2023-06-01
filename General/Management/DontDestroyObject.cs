using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    [SerializeField] private string _identifier;
    public string Identifier
    {
        get { return _identifier; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (ModuleManager.GetModule<DontDestroyManager>().GetObject(_identifier) != null)
        {
            Destroy(gameObject);
            return;
        }
        ModuleManager.GetModule<DontDestroyManager>().RegisterObject(this);
    }
}
