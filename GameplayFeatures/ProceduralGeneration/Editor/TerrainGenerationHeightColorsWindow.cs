using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor;
using UnityToolbox.GameplayFeatures.SerializationData;
using System.Linq;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.General;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    public class TerrainGenerationHeightColorsWindow : EditorWindow, ISerializedDataContainer<List<TerrainGenerationHeightColorData>>
    {

        private Vector2 _scrollPos;
        private List<TerrainGenerationHeightColorLayer> _layers;
        private List<TerrainGenerationHeightColorData> _layersData;

        public Action<StatusException> OnUpdateStatus;
        public Action<List<TerrainGenerationHeightColorData>> OnClose;

        public bool EnableSettings;
        private bool _enabledNormal;
        private bool _enabledEmission;
        private bool _enabledMetallic;
        private bool _enabledOcclusion;
        private bool _enabledRoughness;

        private TerrainGenerationHeightColorsWindow()
        {
        }

        public static TerrainGenerationHeightColorsWindow Open(bool enableSettings)
        {
            TerrainGenerationHeightColorsWindow window =
                GetWindow<TerrainGenerationHeightColorsWindow>("Terrain Generation");
            window.maxSize = new Vector2(500, 400);
            window.minSize = new Vector2(500, 400);
            window.EnableSettings = enableSettings;
            return window;
        }

        private void Awake()
        {
            InitializeWindow();
        }

        public void InitializeWindow()
        {
            _layersData = new List<TerrainGenerationHeightColorData>();
            _layers = new List<TerrainGenerationHeightColorLayer>();
            _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count, true, "Base"));

            TerrainGenerationEditorEvents.Instance.OnHeightColorsOpen += Close;
            TerrainGenerationEditorEvents.Instance.OnClose += Close;
        }

        public void OnGUI()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.4f);
            GUILayout.BeginVertical(new GUIStyle("window"));
            GUI.color = col;

            List<TerrainGenerationHeightColorLayer> temp = new List<TerrainGenerationHeightColorLayer>();

            if (EnableSettings)
            {
                bool prev = _enabledNormal;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Normal Mapping:");
                GUILayout.FlexibleSpace();
                _enabledNormal = EditorGUILayout.Toggle(_enabledNormal, GUILayout.Width(20));
                if (prev != _enabledNormal)
                {
                    temp = new List<TerrainGenerationHeightColorLayer>();
                    temp.AddRange(_layers);
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        TerrainGenerationHeightColorData layer = _layers[i].Data;
                        layer.NormalEnabled = _enabledNormal;
                        _layers[i].Data = layer;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _enabledEmission;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Emission Mapping:");
                GUILayout.FlexibleSpace();
                _enabledEmission = EditorGUILayout.Toggle(_enabledEmission, GUILayout.Width(20));
                if (prev != _enabledEmission)
                {
                    temp = new List<TerrainGenerationHeightColorLayer>();
                    temp.AddRange(_layers);
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        TerrainGenerationHeightColorData layer = _layers[i].Data;
                        layer.MetallicEnabled = _enabledMetallic;
                        _layers[i].Data = layer;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _enabledMetallic;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Metallic Mapping:");
                GUILayout.FlexibleSpace();
                _enabledMetallic = EditorGUILayout.Toggle(_enabledMetallic, GUILayout.Width(20));
                if (prev != _enabledMetallic)
                {
                    temp = new List<TerrainGenerationHeightColorLayer>();
                    temp.AddRange(_layers);
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        TerrainGenerationHeightColorData layer = _layers[i].Data;
                        layer.MetallicEnabled = _enabledMetallic;
                        _layers[i].Data = layer;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _enabledOcclusion;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Occlusion Mapping:");
                GUILayout.FlexibleSpace();
                _enabledOcclusion = EditorGUILayout.Toggle(_enabledOcclusion, GUILayout.Width(20));
                if (prev != _enabledOcclusion)
                {
                    temp = new List<TerrainGenerationHeightColorLayer>();
                    temp.AddRange(_layers);
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        TerrainGenerationHeightColorData layer = _layers[i].Data;
                        layer.OcclusionEnabled = _enabledOcclusion;
                        _layers[i].Data = layer;
                    }
                }

                prev = _enabledRoughness;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Occlusion Mapping:");
                GUILayout.FlexibleSpace();
                _enabledRoughness = EditorGUILayout.Toggle(_enabledRoughness, GUILayout.Width(20));
                if (prev != _enabledRoughness)
                {
                    temp = new List<TerrainGenerationHeightColorLayer>();
                    temp.AddRange(_layers);
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        TerrainGenerationHeightColorData layer = _layers[i].Data;
                        layer.RoughnessEnabled = _enabledRoughness;
                        _layers[i].Data = layer;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height Texturing");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count));
            }
            GUILayout.EndHorizontal();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            temp = new List<TerrainGenerationHeightColorLayer>();
            temp.AddRange(_layers);
            foreach (TerrainGenerationHeightColorLayer layer in temp)
            {
                try
                {
                    layer.DrawDetails();
                }
                catch (StatusException ex)
                {
                    UpdateStatus(ex);
                }
            }
            GUILayout.FlexibleSpace();
            DrawLineHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void DefineSettings(bool enableOcclusiom, bool enableNormal, bool enableMetallic, bool enableEmission, bool enableRoughness)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                TerrainGenerationHeightColorData layer = _layers[i].Data;
                layer.OcclusionEnabled = enableOcclusiom;
                layer.RoughnessEnabled = enableRoughness;
                layer.NormalEnabled = enableNormal;
                layer.MetallicEnabled = enableMetallic;
                layer.EmissionEnabled = enableEmission;
                _layers[i].Data = layer;
            }
        }

        private void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public void DeleteLayer(int currentPos)
        {
            if (currentPos < 1 || _layers[currentPos].Data.IsBaseLayer)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The height layer could not be deleted. Is it the base layer?");
                return;
            }

            if (currentPos >= _layers.Count)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The height layer could not be deleted. The position cannot exceed: " + (_layers.Count - 1));
                return;
            }

            _layers.RemoveAt(currentPos);

            for (int i = currentPos; i < _layers.Count; i++)
            {
                TerrainGenerationHeightColorData layer = _layers[i].Data;
                layer.CurrentPos--;
                _layers[i].Data = layer;
            }
        }

        public bool MoveLayer(TerrainGenerationHeightColorLayer terrainGenerationHeightColorLayer, int prev, int currentPos)
        {
            if (currentPos < 1 || _layers[prev].Data.IsBaseLayer)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The height layer could not be moved. Is the goal position at or before the base layer?");
                return false;
            }

            if (currentPos >= _layers.Count)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The height layer could not be moved. The position cannot exceed: " + (_layers.Count - 1));
                return false;
            }

            TerrainGenerationHeightColorData toMoveOne = _layers[prev].Data;
            toMoveOne.CurrentPos = currentPos;
            TerrainGenerationHeightColorData toMoveTwo = _layers[currentPos].Data;
            toMoveTwo.CurrentPos = prev;
            _layers[prev].Data = toMoveTwo;
            _layers[currentPos].Data = toMoveOne;
            return true;
        }

        public static SerializedDataErrorDetails DummyDeserialize(List<TerrainGenerationHeightColorData> data)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            if (data != null && data.Count != 0)
            {
                foreach (TerrainGenerationHeightColorData layer in data)
                {
                    TerrainGenerationHeightColorLayer temp = new TerrainGenerationHeightColorLayer(null, 0);
                    SerializedDataErrorDetails tempErr = temp.Deserialize(layer);

                    if (tempErr.HasErrors)
                    {
                        err.HasErrors = true;
                        err.Traced.Add(tempErr);
                        err.ErrorDescription = err.Traced.Count + " height color layers have asset errors.";
                    }
                }
            }

            return err;
        }

        public SerializedDataErrorDetails Deserialize(List<TerrainGenerationHeightColorData> data)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            if (data != null && data.Count != 0)
            {
                _layers.Clear();
                _layersData = data;
                foreach (TerrainGenerationHeightColorData layer in _layersData)
                {
                    _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count));
                    SerializedDataErrorDetails temp = _layers.Last().Deserialize(layer);
                    if (temp.HasErrors)
                    {
                        err.HasErrors = true;
                        err.Traced.Add(temp);
                        err.ErrorDescription = err.Traced.Count + " height color layers have asset errors.";
                    }
                }
            }
            else
            {
                InitializeWindow();
            }

            return err;
        }

        public List<TerrainGenerationHeightColorData> Serialize()
        {
            _layersData.Clear();
            foreach (TerrainGenerationHeightColorLayer layer in _layers)
            {
                _layersData.Add(layer.Serialize());
            }

            return _layersData;
        }

        void OnDestroy()
        {
            OnClose?.Invoke(Serialize());
            TerrainGenerationEditorEvents.Instance.OnHeightColorsOpen -= Close;
            TerrainGenerationEditorEvents.Instance.OnClose -= Close;
        }

        private void UpdateStatus(StatusException error)
        {
            OnUpdateStatus?.Invoke(error);
        }
    }
}
