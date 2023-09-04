using System.Collections;
using System.Collections.Generic;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Terrain;
using UnityEngine;
using UnityEditor;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor
{
    /// <summary>
    /// The editor for <see cref="TerrainDecoration"/>, which adds buttons for the generation.
    /// </summary>
    [CustomEditor(typeof(TerrainDecoration))]
    public class TerrainDecorationEditor : UnityEditor.Editor
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
}