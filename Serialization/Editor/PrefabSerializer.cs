using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
