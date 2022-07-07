using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathFetcher))]
public class PathInitializer : Editor, ISerializationCallbackReceiver
{
    public void OnAfterDeserialize()
    {
        PathFetcher fetcher = (PathFetcher)target;
        string newPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(fetcher.gameObject);
        if (!newPath.Equals(fetcher.Path))
        {
            fetcher.Path = newPath;
            fetcher.PathChanged();
        }
    }

    public void OnBeforeSerialize()
    {
    }

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
