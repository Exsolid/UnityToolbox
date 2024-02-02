using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    [CustomEditor(typeof(TerrainGeneration))]
    public class TerrainGenerationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainGeneration gen = (TerrainGeneration)target;

            if (gen.Data == null)
            {
                gen.Data = TerrainGenerationIO.Instance.ReadData();
            }
            else if(gen.Data.Count == 0)
            {
                GUILayout.Label("Create generation data first! (UnityToolbox -> Terrain Generation)");

                if (GUILayout.Button("Refresh Data"))
                {
                    gen.Data = TerrainGenerationIO.Instance.ReadData();
                }
            }
            else
            {
                if (GUILayout.Button("Refresh Data"))
                {
                    gen.Data = TerrainGenerationIO.Instance.ReadData();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Generation data: ");
                gen.SelectedData = EditorGUILayout.Popup("", gen.SelectedData, gen.Data.Count == 0 ? new string[] { } : gen.Data.Keys.ToArray(), GUILayout.Width(180));
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Generate Terrain"))
                {
                    gen.GenerateTerrain();
                }
            }
        }
    }
}
