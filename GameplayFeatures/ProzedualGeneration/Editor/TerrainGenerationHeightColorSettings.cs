using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor
{
    public class TerrainGenerationHeightColorSettings: ISerializedDataContainer<List<TerrainGenerationHeightColorData>>
    {
        private Vector2 _scrollPos;
        private List<TerrainGenerationHeightColorLayer> _layers;
        private List<TerrainGenerationHeightColorData> _layersData;

        public TerrainGenerationHeightColorSettings()
        {
            _layersData = new List<TerrainGenerationHeightColorData>();
            _layers = new List<TerrainGenerationHeightColorLayer>();
            _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count, true, "Base"));
        }

        public void DrawDetails()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.2f);
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(200));
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
                layer.DrawDetails();
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

        public void Deserialize(List<TerrainGenerationHeightColorData> data)
        {
            _layers.Clear();
            _layersData = data;
            foreach (TerrainGenerationHeightColorData layer in _layersData)
            {
                _layers.Add(new TerrainGenerationHeightColorLayer(this, _layers.Count));
                _layers.Last().Deserialize(layer);
            }
        }

        public List<TerrainGenerationHeightColorData> Serialize()
        {
            foreach (TerrainGenerationHeightColorLayer layer in _layers)
            {
                _layersData.Add(layer.Serialize());
            }

            return _layersData;
        }
    }
}
