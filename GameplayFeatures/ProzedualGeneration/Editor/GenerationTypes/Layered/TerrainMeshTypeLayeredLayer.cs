using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes.Layered
{
    public abstract class TerrainMeshTypeLayeredLayer: ISerializedDataContainer<TerrainMeshTypeLayeredLayerBaseData>
    {
        private TerrainMeshTypeLayered _parent;
        public string GeneratorName;

        protected TerrainMeshTypeLayeredLayerBaseData _data;

        public bool IsBaseLayer
        {
            get { return _data.IsBaseLayer;  }
        }

        public int CurrentPos
        {
            get { return _data.CurrentPos; }
            set { _data.CurrentPos = value; }
        }

        private List<TerrainGenerationLayeredAssetLayer> _assetPlacements;
        private Vector2 _assetScrollPos;

        private int _selectedLayer;

        protected TerrainMeshTypeLayeredLayer(TerrainMeshTypeLayered parent, int currentPos, TerrainMeshTypeLayeredLayerBaseData dataType)
        {
            _assetPlacements = new List<TerrainGenerationLayeredAssetLayer>();
            _data = dataType;
            _data.CurrentPos = currentPos;
            _parent = parent;
        }

        protected TerrainMeshTypeLayeredLayer(TerrainMeshTypeLayered parent, int currentPos, bool isBaseLayer, string baseLayerName, TerrainMeshTypeLayeredLayerBaseData dataType)
        {
            _assetPlacements = new List<TerrainGenerationLayeredAssetLayer>();
            _parent = parent;
            _data = dataType;
            _data.IsBaseLayer = isBaseLayer;
            _data.Name = baseLayerName;
            _data.CurrentPos = currentPos;
        }

        public void DrawDetails()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.4f);
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(400));
            GUI.color = col;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ground layer: ");
            if (_data.IsBaseLayer)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(_data.Name);
            }
            else
            {
                _data.Name = GUILayout.TextField(_data.Name);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int prev = _data.CurrentPos;
            if (!_data.IsBaseLayer)
            {
                _data.CurrentPos = EditorGUILayout.IntField(_data.CurrentPos, GUILayout.Width(18));
            }
            else
            {
                GUILayout.Label(_data.CurrentPos +"");
            }
            if (prev != _data.CurrentPos)
            {
                if (!_parent.MoveLayer(this, prev, _data.CurrentPos))
                {
                    _data.CurrentPos = prev;
                }
            }

            if (!_data.IsBaseLayer)
            {
                if (GUILayout.Button("-", GUILayout.Width(18)))
                {
                    _parent.DeleteLayer(_data.CurrentPos);
                }
            }
            GUILayout.EndHorizontal();

            DrawLineHorizontal();
            DrawDetailsRest();

            GUILayout.EndVertical();
        }

        protected abstract void DrawDetailsRest();

        protected void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public void DeleteAssetPlacement(TerrainGenerationLayeredAssetLayer current)
        {
            _assetPlacements.Remove(current);
        }

        public void AddAssetPlacement(TerrainGenerationLayeredAssetLayer current)
        {
            _assetPlacements.Add(current);
        }

        protected void DrawAssetPlacement()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.2f);
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(200));
            GUI.color = col;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Placement");
            GUILayout.FlexibleSpace();
            _selectedLayer = EditorGUILayout.Popup(_selectedLayer, new string[] { "Single", "Clustered" }, GUILayout.Width(180));
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                switch (_selectedLayer)
                {
                    case 0:
                        _assetPlacements.Add(new TerrainGenerationLayeredAssetLayerSingle(this));
                        break;
                    case 1:
                        _assetPlacements.Add(new TerrainGenerationLayeredAssetLayerClustered(this));
                        break;
                }
            }
            GUILayout.EndHorizontal();

            _assetScrollPos = GUILayout.BeginScrollView(_assetScrollPos);

            List<TerrainGenerationLayeredAssetLayer> temp = new List<TerrainGenerationLayeredAssetLayer>();
            temp.AddRange(_assetPlacements);
            foreach (TerrainGenerationLayeredAssetLayer layer in temp)
            {
                layer.DrawDetails();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public SerializedDataErrorDetails Deserialize(TerrainMeshTypeLayeredLayerBaseData obj)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            if (obj.AssetPlacements != null && obj.AssetPlacements.Count != 0)
            {
                foreach (TerrainGenerationLayeredAssetBaseData layer in obj.AssetPlacements)
                {
                    if (layer.GetType() == typeof(TerrainGenerationLayeredAssetData))
                    {
                        _assetPlacements.Add(new TerrainGenerationLayeredAssetLayerSingle(this));
                    }
                    else
                    {
                        _assetPlacements.Add(new TerrainGenerationLayeredAssetLayerClustered(this));
                    }
                    SerializedDataErrorDetails temp = _assetPlacements.Last().Deserialize(layer);
                    if (temp.HasErrors)
                    {
                        err.HasErrors = true;
                        err.Traced.Add(temp);
                        err.ErrorDescription = err.Traced.Count + " asset errors have been found in the asset layers.";
                    }
                }
            }
            else
            {
                _assetPlacements = new List<TerrainGenerationLayeredAssetLayer>();
            }
            err = DeserializeRest(obj, err);
            return err;
        }

        protected abstract SerializedDataErrorDetails DeserializeRest(TerrainMeshTypeLayeredLayerBaseData obj, SerializedDataErrorDetails err);

        public TerrainMeshTypeLayeredLayerBaseData Serialize()
        {
            TerrainMeshTypeLayeredLayerBaseData data = SerializeRest();
            data.AssetPlacements = new List<TerrainGenerationLayeredAssetBaseData>();
            foreach (TerrainGenerationLayeredAssetLayer layer in _assetPlacements)
            {
                data.AssetPlacements.Add(layer.Serialize());
            }
            return data;
        }

        protected abstract TerrainMeshTypeLayeredLayerBaseData SerializeRest();
    }
}
