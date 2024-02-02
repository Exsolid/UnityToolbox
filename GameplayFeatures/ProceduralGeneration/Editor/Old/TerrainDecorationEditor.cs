using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
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