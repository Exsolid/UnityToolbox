using System.Collections;
using System.Collections.Generic;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Terrain;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The editor for <see cref="TerrainGenerator"/>, which adds buttons for the generation.
/// </summary>
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
