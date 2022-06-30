using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathFetcher))]
public class PathInitializer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PathFetcher fetcher = (PathFetcher)target;
        string newPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(fetcher.gameObject);
        if (!newPath.Equals(fetcher.Path))
        {
            fetcher.Path = newPath;
            fetcher.PathChanged();
        }
    }
}
