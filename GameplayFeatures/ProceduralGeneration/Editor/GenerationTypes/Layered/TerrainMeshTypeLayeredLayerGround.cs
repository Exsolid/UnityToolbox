using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayeredLayerGround : TerrainMeshTypeLayeredLayer
    {
        private TerrainGenerationHeightColorsWindow _heightColors;

        private bool _enabledNormals;
        private bool _enabledEmission;
        private bool _enabledMetallic;
        private bool _enabledOcclusion;
        private bool _enabledRoughness;

        public bool EnabledNormal
        {
            set
            {
                _enabledNormals = value;
                SetOptionalTextures();
            }
            get
            {
                return _enabledNormals;
            }
        }

        public bool EnabledEmission
        {
            set
            {
                _enabledEmission = value;
                SetOptionalTextures();
            }
            get
            {
                return _enabledEmission;
            }
        }

        public bool EnabledMetallic
        {
            set
            {
                _enabledMetallic = value;
                SetOptionalTextures();
            }
            get
            {
                return _enabledMetallic;
            }
        }

        public bool EnabledOcclusion
        {
            set
            {
                _enabledOcclusion = value;
                SetOptionalTextures();
            }
            get
            {
                return _enabledOcclusion;
            }
        }

        public bool EnabledRoughness
        {
            set
            {
                _enabledRoughness = value;
                SetOptionalTextures();
            }
            get
            {
                return _enabledRoughness;
            }
        }

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
            if (data.IsBaseLayer)
            {
                GUILayout.Label(data.Height + "", GUILayout.Width(200));
            }
            else
            {
                data.Height = EditorGUILayout.FloatField(data.Height, GUILayout.Width(200));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Noise Ground: ");
            data.NoiseGround = EditorGUILayout.Slider(data.NoiseGround, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height Colors: ");
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                TerrainGenerationEditorEvents.Instance.HeightColorsOpen();
                _heightColors = TerrainGenerationHeightColorsWindow.Open(false);
                _heightColors.Deserialize(data.HeightData);
                _heightColors.DefineSettings(EnabledOcclusion, EnabledNormal, EnabledMetallic, EnabledEmission, EnabledRoughness);

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

        protected override SerializedDataErrorDetails DeserializeRest(TerrainMeshTypeLayeredLayerBaseData obj, SerializedDataErrorDetails err)
        {
            _data = obj as TerrainMeshTypeLayeredLayerGroundData;
            if (obj is TerrainMeshTypeLayeredLayerGroundData data)
            {
                SerializedDataErrorDetails temp = TerrainGenerationHeightColorsWindow.DummyDeserialize(data.HeightData);
                if (temp.HasErrors)
                {
                    err.HasErrors = true;
                    err.Traced.Add(temp);
                    err.ErrorDescription = err.Traced.Count + " asset errors have been found in the asset layers.";
                }
            }

            return err;
        }

        protected override TerrainMeshTypeLayeredLayerBaseData SerializeRest()
        {
            return _data;
        }

        private void SetOptionalTextures()
        {
            if (_heightColors != null)
            {
                _heightColors.DefineSettings(EnabledOcclusion, EnabledNormal, EnabledMetallic, EnabledEmission, EnabledRoughness);
            }
            else if((_data as TerrainMeshTypeLayeredLayerGroundData).HeightData != null)
            {
                for (var index = 0; index < (_data as TerrainMeshTypeLayeredLayerGroundData).HeightData.Count; index++)
                {
                    TerrainGenerationHeightColorData heightData = (_data as TerrainMeshTypeLayeredLayerGroundData).HeightData[index];
                    heightData.OcclusionEnabled = EnabledOcclusion;
                    heightData.RoughnessEnabled = EnabledRoughness;
                    heightData.NormalEnabled = EnabledNormal;
                    heightData.MetallicEnabled = EnabledMetallic;
                    heightData.EmissionEnabled = EnabledEmission;

                    (_data as TerrainMeshTypeLayeredLayerGroundData).HeightData[index] = heightData;
                }
            }
        }
    }
}
