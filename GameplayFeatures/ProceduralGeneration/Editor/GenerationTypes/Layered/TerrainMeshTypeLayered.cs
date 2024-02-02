using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayered : TerrainMeshType
    {
        private List<TerrainMeshTypeLayeredLayer> _layers;
        private TerrainMeshTypeLayeredData _data;
        private int _selectedLayer;
        private Vector2 _scrollPos;

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
        }

        public override void DrawDetails()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Filler Vertex Count: ");
            _data.FillerVertexCount = EditorGUILayout.IntField(_data.FillerVertexCount, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _selectedLayer = EditorGUILayout.Popup(_selectedLayer, new string[] { "Ground", "Void"}, GUILayout.Width(180));
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                switch (_selectedLayer)
                {
                    case 0:
                        _layers.Add(new TerrainMeshTypeLayeredLayerGround(this, _layers.Count));
                        break;
                    case 1:
                        //TODO create void layer
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
            if (currentPos >= _layers.Count || currentPos < 0 || _layers[currentPos].IsBaseLayer)
            {
                //TODO log to status
                return false;
            }

            if (layer.GetType() == typeof(TerrainMeshTypeLayeredLayerGround) && goalPos == 0)
            {
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
                //TODO log to status
                return false;
            }

            _layers.RemoveAt(currentPos);
            for(int i = currentPos; i < _layers.Count; i++)
            {
                _layers[i].CurrentPos--;
            }
            //TODO with checks for all fixed layers
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
                        _layers.Insert(layer.CurrentPos, new TerrainMeshTypeLayeredLayerGround(this, layer.CurrentPos));
                        SerializedDataErrorDetails temp = _layers[layer.CurrentPos].Deserialize(layer);
                        if (temp.HasErrors)
                        {
                            err.HasErrors = true;
                            err.Traced.Add(temp);
                            err.ErrorDescription = err.Traced.Count + " mesh layers contain asset errors.";
                        }
                    }
                    else
                    {
                        //TODO create void layer
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
