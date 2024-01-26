using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayeredLayerGround : TerrainMeshTypeLayeredLayer
    {
        private TerrainGenerationHeightColorSettings _heightColors;

        public TerrainMeshTypeLayeredLayerGround(TerrainMeshTypeLayered parent, int currentPos) : base(parent, currentPos, new TerrainMeshTypeLayeredLayerGroundData())
        {
            _heightColors = new TerrainGenerationHeightColorSettings();
        }

        public TerrainMeshTypeLayeredLayerGround(TerrainMeshTypeLayered parent, int currentPos, bool isBaseLayer, string baseLayerName) : base(parent, currentPos, isBaseLayer, baseLayerName, new TerrainMeshTypeLayeredLayerGroundData())
        {
            _heightColors = new TerrainGenerationHeightColorSettings();
        }

        protected override void DrawDetailsRest()
        {
            GUILayout.BeginVertical();
            TerrainMeshTypeLayeredLayerGroundData data = _data as TerrainMeshTypeLayeredLayerGroundData;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Cliff Height: ");
            data.Height = EditorGUILayout.FloatField(data.Height, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Noise Cliff Ground: ");
            data.NoiseCliffGround = EditorGUILayout.Slider(data.NoiseCliffGround, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Noise Cliff: ");
            data.NoiseCliff = EditorGUILayout.Slider(data.NoiseCliff, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Noise Ground: ");
            data.NoiseGround = EditorGUILayout.Slider(data.NoiseGround, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            _heightColors.DrawDetails();
            DrawLineHorizontal();
            DrawAssetPlacement();

            GUILayout.EndVertical();
        }

        protected override void DeserializeRest(TerrainMeshTypeLayeredLayerBaseData obj)
        {
            _data = obj;
            TerrainMeshTypeLayeredLayerGroundData data = _data as TerrainMeshTypeLayeredLayerGroundData;
            _heightColors.Deserialize(data.HeightData);
        }

        protected override TerrainMeshTypeLayeredLayerBaseData SerializeRest()
        {
            TerrainMeshTypeLayeredLayerGroundData data = _data as TerrainMeshTypeLayeredLayerGroundData;
            data.HeightData = _heightColors.Serialize();
            return data;
        }
    }
}
