using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// As all data of the <see cref="PrefabManager"/> is changed within OnValidate, the objects need to be SetDirty on changes. This happens here.
/// </summary>
[CustomEditor(typeof(PrefabManager))]
public class PrefabSerializer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (((PrefabManager)target).ToSerialize == null) return;
        foreach (GameObject obj in ((PrefabManager)target).ToSerialize)
        {
           if(obj != null) EditorUtility.SetDirty(obj);
        }
        ((PrefabManager)target).ToSerialize.Clear();
    }
}
