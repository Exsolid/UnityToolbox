using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainDecoration))]
public class TerrainDecorationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainDecoration decoGenerator = (TerrainDecoration)target;

        if(GUILayout.Button("Generate Decoration"))
        {
            decoGenerator.PlaceObjects();
        }

        if (GUILayout.Button("Delete Decoration"))
        {
            decoGenerator.DeleteObjects();
        }
    }
}
