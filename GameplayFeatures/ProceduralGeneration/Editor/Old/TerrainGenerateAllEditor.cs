using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    /// <summary>
    /// The editor for <see cref="TerrainGenerateAll"/>, which adds buttons for the generation.
    /// </summary>
    [CustomEditor(typeof(TerrainGenerateAll))]
    public class TerrainGenerateAllEditor : UnityEditor.Editor
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
}