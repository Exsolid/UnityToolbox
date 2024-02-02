using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    /// <summary>
    /// The editor for <see cref="TerrainGenerator"/>, which adds buttons for the generation.
    /// </summary>
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGeneratorEditor : UnityEditor.Editor
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
}