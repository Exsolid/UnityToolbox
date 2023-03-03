using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentIdentifier : MonoBehaviour
{
    [SerializeField] [ReadOnly] private string id = "";
    public string ID
    {
        get { return id; }
    }

    private void OnValidate()
    {
        if (id.Equals(""))
        {
            id = IDManager.GetUniqueID();
        }
    }
}
