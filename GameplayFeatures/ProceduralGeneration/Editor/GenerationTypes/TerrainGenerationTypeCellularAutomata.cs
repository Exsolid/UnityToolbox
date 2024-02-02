using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Algorithms;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes
{
    public class TerrainGenerationTypeCellularAutomata : TerrainGenerationType
    {
        public TerrainGenerationTypeCellularAutomata()
        {
            _data = new TerrainGenerationTypeCellularAutomataData();
        }

        public override void DrawDetails()
        {
            GUILayout.BeginVertical();
            TerrainGenerationTypeCellularAutomataData data = _data as TerrainGenerationTypeCellularAutomataData;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Size for X & Y: ");
            data.Size = EditorGUILayout.IntField(data.Size, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Iteration Count: ");
            data.IterationCount = EditorGUILayout.IntField(data.IterationCount, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fill Percentage: ");
            data.FillPct = EditorGUILayout.Slider(data.FillPct, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Border Size: ");
            data.BorderSize = EditorGUILayout.IntField(data.BorderSize, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public override int[,] GetExampleGeneration(int x, int y)
        {
            TerrainGenerationTypeCellularAutomataData data = _data as TerrainGenerationTypeCellularAutomataData;
            return CellularAutomata.Generate(new int[x, y], data.FillPct, data.IterationCount, data.BorderSize);
        }

        public override SerializedDataErrorDetails Deserialize(TerrainGenerationTypeBaseData obj)
        {
            _data = (TerrainGenerationTypeCellularAutomataData) obj;
            return new SerializedDataErrorDetails();
        }

        public override TerrainGenerationTypeBaseData Serialize()
        {
            return _data;
        }
    }
}
