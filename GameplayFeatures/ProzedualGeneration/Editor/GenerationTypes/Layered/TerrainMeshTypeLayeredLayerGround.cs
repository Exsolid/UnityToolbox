using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.General;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayeredLayerGround : TerrainMeshTypeLayeredLayer
    {
        private TerrainGenerationHeightColorsWindow _heightColors;

        public TerrainMeshTypeLayeredLayerGround(TerrainMeshTypeLayered parent, int currentPos) : base(parent, currentPos, new TerrainMeshTypeLayeredLayerGroundData())
        {
        }

        public TerrainMeshTypeLayeredLayerGround(TerrainMeshTypeLayered parent, int currentPos, bool isBaseLayer, string baseLayerName) : base(parent, currentPos, isBaseLayer, baseLayerName, new TerrainMeshTypeLayeredLayerGroundData())
        {
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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height Colors: ");
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                _heightColors = TerrainGenerationHeightColorsWindow.Open();
                _heightColors.Deserialize(data.HeightData);

                _heightColors.OnClose += (List<TerrainGenerationHeightColorData> heightData) =>
                {
                    TerrainMeshTypeLayeredLayerGroundData dataCurrent = _data as TerrainMeshTypeLayeredLayerGroundData;
                    dataCurrent.HeightData = heightData;
                    _heightColors = null;
                };

                _heightColors.OnUpdateStatus += (StatusException ex) => throw ex;
            }
            GUILayout.EndHorizontal();

            DrawLineHorizontal();
            DrawAssetPlacement();

            GUILayout.EndVertical();
        }

        protected override void DeserializeRest(TerrainMeshTypeLayeredLayerBaseData obj)
        {
            _data = obj;
        }

        protected override TerrainMeshTypeLayeredLayerBaseData SerializeRest()
        {
            return _data;
        }
    }
}
