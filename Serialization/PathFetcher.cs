using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFetcher : MonoBehaviour
{
    [HideInInspector] public string Path { get { return _path; } set { _path = value; } }
    private string _path;
    public Action pathChanged;

    public void PathChanged()
    {
        if(pathChanged != null)
        {
            pathChanged();
        }
    }
}
