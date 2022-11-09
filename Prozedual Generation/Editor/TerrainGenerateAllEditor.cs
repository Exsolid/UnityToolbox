using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerateAll))]
public class TerrainGenerateAllEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainGenerateAll generator = (TerrainGenerateAll)target;

        if (GUILayout.Button("Generate Mesh And Decoration"))
        {
            generator.GenerateAll();
        }

        if (GUILayout.Button("Generate Mesh And Decoration With Anchors"))
        {
            generator.GenerateAllWithAnchors();
        }

        if (GUILayout.Button("Delete All Decoration"))
        {
            generator.DeleteAllDecoration();
        }
    }
}
