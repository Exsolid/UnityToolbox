using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainGenerator meshGenerator = (TerrainGenerator)target;

        if(GUILayout.Button("Generate Mesh"))
        {
            meshGenerator.GenerateViaCellularAutomata();
        }
    }
}
