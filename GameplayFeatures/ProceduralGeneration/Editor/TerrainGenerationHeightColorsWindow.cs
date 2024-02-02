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

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    public class TerrainGenerationHeightColorsWindow : EditorWindow, ISerializedDataContainer<List<TerrainGenerationHeightColorData>>
    {

        private Vector2 _scrollPos;
        private List<TerrainGenerationHeightColorLayer> _layers;
        private List<TerrainGenerationHeightColorData> _layersData;

        public Action<StatusException> OnUpdateStatus;
        public Action<List<TerrainGenerationHeightColorData>> OnClose;

        private TerrainGenerationHeightColorsWindow()
        {

        }

        public static TerrainGenerationHeightColorsWindow Open()
        {
            TerrainGenerationHeightColorsWindow window =
                GetWindow<TerrainGenerationHeightColorsWindow>("Terrain Generation");
            window.maxSize = new Vector2(500, 400);
            window.minSize = new Vector2(500, 400);
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
        }

        public void OnGUI()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.4f);
            GUILayout.BeginVertical(new GUIStyle("window"));
            GUI.color = col;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height Texturing");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count));
            }
            GUILayout.EndHorizontal();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            List<TerrainGenerationHeightColorLayer> temp = new List<TerrainGenerationHeightColorLayer>();
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
            if (currentPos >= _layers.Count || currentPos < 1 || _layers[currentPos].Data.IsBaseLayer)
            {
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
            if (currentPos >= _layers.Count || currentPos < 1 || _layers[prev].Data.IsBaseLayer)
            {
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
        }

        void OnEnable()
        {
            TerrainGenerationEditorEvents.Instance.OnClose += Close;
        }

        void OnDisable()
        {
            TerrainGenerationEditorEvents.Instance.OnClose -= Close;
        }

        private void UpdateStatus(StatusException error)
        {
            OnUpdateStatus?.Invoke(error);
        }
    }
}
