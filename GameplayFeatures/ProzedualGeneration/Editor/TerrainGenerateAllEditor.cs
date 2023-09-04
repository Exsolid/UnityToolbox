using System.Collections;
using System.Collections.Generic;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Terrain;
using UnityEngine;
using UnityEditor;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor
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