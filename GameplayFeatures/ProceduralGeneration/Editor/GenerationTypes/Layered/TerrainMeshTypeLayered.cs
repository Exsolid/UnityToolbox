using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayered : TerrainMeshType
    {
        private List<TerrainMeshTypeLayeredLayer> _layers;
        private TerrainMeshTypeLayeredData _data;
        private int _selectedLayer;
        private Vector2 _scrollPos;

        private bool _settingFoldout;

        public TerrainMeshTypeLayered()
        {
            Init();
        }

        private void Init()
        {
            _layers = new List<TerrainMeshTypeLayeredLayer>();
            _layers.Add(new TerrainMeshTypeLayeredLayerGround(this, _layers.Count, true, "Base Layer"));

            _data = new TerrainMeshTypeLayeredData
            {
                Layers = new List<TerrainMeshTypeLayeredLayerBaseData>()
            };

            _data.EnabledAssets = true;
        }

        public override void DrawDetails()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Size Between Vertices: ");
            _data.SizeBetweenVertices = EditorGUILayout.FloatField(Mathf.Max(0.01f, _data.SizeBetweenVertices), GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertex Multiplier: ");
            _data.VertexMultiplier = EditorGUILayout.IntField(Mathf.Max(1, _data.VertexMultiplier), GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Noise For Asset Position: ");
            _data.AssetPositionNoise = EditorGUILayout.Slider(_data.AssetPositionNoise, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            _settingFoldout = EditorGUILayout.Foldout(_settingFoldout, "General Settings To Enable");

            if (_settingFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Asset Placement:");
                GUILayout.FlexibleSpace();
                _data.EnabledAssets = EditorGUILayout.Toggle(_data.EnabledAssets, GUILayout.Width(20));
                GUILayout.EndHorizontal();

                bool prev = _data.EnabledNormal;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Normal Mapping:");
                GUILayout.FlexibleSpace();
                _data.EnabledNormal = EditorGUILayout.Toggle(_data.EnabledNormal, GUILayout.Width(20));
                if (prev != _data.EnabledNormal)
                {
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledNormal = _data.EnabledNormal;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _data.EnabledEmission;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Emission Mapping:");
                GUILayout.FlexibleSpace();
                _data.EnabledEmission = EditorGUILayout.Toggle(_data.EnabledEmission, GUILayout.Width(20));
                if (prev != _data.EnabledEmission)
                {
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledEmission = _data.EnabledEmission;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _data.EnabledOcclusion;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Occlusion Mapping:");
                GUILayout.FlexibleSpace();
                _data.EnabledOcclusion = EditorGUILayout.Toggle(_data.EnabledOcclusion, GUILayout.Width(20));
                if (prev != _data.EnabledOcclusion)
                {
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledOcclusion = _data.EnabledOcclusion;
                    }
                }
                GUILayout.EndHorizontal();

                prev = _data.EnabledMetallic;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Metallic Mapping:");
                GUILayout.FlexibleSpace();
                _data.EnabledMetallic = EditorGUILayout.Toggle(_data.EnabledMetallic, GUILayout.Width(20));
                if (prev != _data.EnabledMetallic)
                {
                    for (int i = 0; i < _layers.Count; i++)
                    {
                        ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledMetallic = _data.EnabledMetallic;
                    }

                    if (prev == true)
                    {
                        _data.EnabledRoughness = false;

                        for (int i = 0; i < _layers.Count; i++)
                        {
                            ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledRoughness = _data.EnabledRoughness;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                if (_data.EnabledMetallic)
                {
                    prev = _data.EnabledRoughness;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Enable Roughness Mapping:");
                    GUILayout.FlexibleSpace();
                    _data.EnabledRoughness = EditorGUILayout.Toggle(_data.EnabledRoughness, GUILayout.Width(20));
                    if (prev != _data.EnabledRoughness)
                    {
                        for (int i = 0; i < _layers.Count; i++)
                        {
                            ((TerrainMeshTypeLayeredLayerGround)_layers[i]).EnabledRoughness = _data.EnabledRoughness;
                        }
                    }
                    GUILayout.EndHorizontal();
                }

            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _selectedLayer = EditorGUILayout.Popup(_selectedLayer, new string[] { "Ground" }, GUILayout.Width(180));
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                switch (_selectedLayer)
                {
                    case 0:
                        _layers.Add(new TerrainMeshTypeLayeredLayerGround(this, _layers.Count));
                        break;
                }
            }
            GUILayout.EndHorizontal();
            DrawLineHorizontal();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            List<TerrainMeshTypeLayeredLayer> temp = new List<TerrainMeshTypeLayeredLayer>();
            temp.AddRange(_layers);
            foreach (TerrainMeshTypeLayeredLayer layer in temp)
            {
                layer.DrawDetails();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public bool MoveLayer(TerrainMeshTypeLayeredLayer layer, int currentPos, int goalPos)
        {
            if (currentPos >= _layers.Count || currentPos < 0 || _layers[currentPos].IsBaseLayer || goalPos < 1)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The layer could not be moved. Is the goal position at or before the base layer?");
                return false;
            }

            if (goalPos >= _layers.Count)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The layer could not be moved. The position cannot exceed " + (_layers.Count - 1));
                return false;
            }

            TerrainMeshTypeLayeredLayer moveAtGoal = _layers[goalPos];
            _layers[goalPos] = layer;
            _layers[currentPos] = moveAtGoal;

            return true;
        }

        public bool DeleteLayer(int currentPos)
        {
            if (currentPos >= _layers.Count || currentPos < 1 || _layers[currentPos].IsBaseLayer)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("The layer could not be deleted. Is it the last or base layer?");
                return false;
            }

            _layers.RemoveAt(currentPos);
            for(int i = currentPos; i < _layers.Count; i++)
            {
                _layers[i].CurrentPos--;
            }

            return true;
        }

        public override SerializedDataErrorDetails Deserialize(TerrainMeshTypeBaseData obj)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            TerrainMeshTypeLayeredData layered = (TerrainMeshTypeLayeredData)obj;
            if (layered.Layers != null && layered.Layers.Count != 0)
            {
                _data = layered;
                _layers.Clear();
                foreach (TerrainMeshTypeLayeredLayerBaseData layer in layered.Layers)
                {
                    if (layer.GetType() == typeof(TerrainMeshTypeLayeredLayerGroundData))
                    {
                        TerrainMeshTypeLayeredLayerGround gLayer =
                            new TerrainMeshTypeLayeredLayerGround(this, layer.CurrentPos);

                        gLayer.EnabledOcclusion = _data.EnabledOcclusion;
                        gLayer.EnabledRoughness = _data.EnabledRoughness;
                        gLayer.EnabledEmission = _data.EnabledEmission;
                        gLayer.EnabledNormal = _data.EnabledNormal;
                        gLayer.EnabledMetallic = _data.EnabledMetallic;

                        _layers.Insert(layer.CurrentPos, gLayer);
                        SerializedDataErrorDetails temp = _layers[layer.CurrentPos].Deserialize(layer);
                        if (temp.HasErrors)
                        {
                            err.HasErrors = true;
                            err.Traced.Add(temp);
                            err.ErrorDescription = err.Traced.Count + " mesh layers contain asset errors.";
                        }
                    }
                }

            }
            else
            {
                Init();
            }

            return err;
        }

        public override TerrainMeshTypeBaseData Serialize()
        {
            _data.Layers.Clear();
            foreach (TerrainMeshTypeLayeredLayer layer in _layers)
            {
                _data.Layers.Insert(layer.CurrentPos, layer.Serialize());
            }
            return _data;
        }
    }
}
