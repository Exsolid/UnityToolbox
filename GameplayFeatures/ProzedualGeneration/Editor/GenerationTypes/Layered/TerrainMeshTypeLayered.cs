using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainMeshTypeLayered : TerrainMeshType
    {
        private List<TerrainMeshTypeLayeredLayer> _layers;
        private int _selectedLayer;
        private Vector2 _scrollPos;

        public TerrainMeshTypeLayered()
        {
            _layers = new List<TerrainMeshTypeLayeredLayer>();
            _layers.Add(new TerrainMeshTypeLayeredLayerGround(this, _layers.Count, true, "Base Layer"));
        }

        public override void DrawDetails()
        {
            //TODO amount of filler vertices (more detailed with more vertices)
            GUILayout.BeginVertical();
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

        public override void Deserialize(TerrainMeshTypeBaseData obj)
        {
            _layers.Clear();
            TerrainMeshTypeLayeredData layered = (TerrainMeshTypeLayeredData) obj;
            foreach (TerrainMeshTypeLayeredLayerBaseData layer in layered.Layers)
            {
                if (layer.GetType() == typeof(TerrainMeshTypeLayeredLayerGroundData))
                {
                    _layers.Insert(layer.CurrentPos, new TerrainMeshTypeLayeredLayerGround(this, layer.CurrentPos));
                    _layers[layer.CurrentPos].Deserialize(layer);
                }
                else
                {
                    //TODO create void layer
                }
            }
        }

        public override TerrainMeshTypeBaseData Serialize()
        {
            TerrainMeshTypeLayeredData layered = new TerrainMeshTypeLayeredData
            {
                Layers = new List<TerrainMeshTypeLayeredLayerBaseData>()
            };

            foreach (TerrainMeshTypeLayeredLayer layer in _layers)
            {
                layered.Layers.Insert(layer.CurrentPos, layer.Serialize());
            }
            return layered;
        }
    }
}
