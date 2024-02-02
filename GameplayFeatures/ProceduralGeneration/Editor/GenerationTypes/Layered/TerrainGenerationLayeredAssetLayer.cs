using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered
{
    public abstract class TerrainGenerationLayeredAssetLayer : ISerializedDataContainer<TerrainGenerationLayeredAssetBaseData>
    {
        private TerrainMeshTypeLayeredLayer _parent;
        protected bool _rewriteErrors;

        protected TerrainGenerationLayeredAssetLayer(TerrainMeshTypeLayeredLayer parent)
        {
            _parent = parent;
        }

        public void DrawDetails()
        {

            Color col = GUI.color;
            if (this.GetType() == typeof(TerrainGenerationLayeredAssetLayerClustered))
            {
                GUI.color = new Color(245f / 255f, 132f / 255f, 66f / 255f, 0.4f);
            }
            else
            {
                GUI.color = new Color(245f / 255f, 182f / 255f, 66f / 255f, 0.4f);
            }
            GUILayout.BeginVertical(new GUIStyle("window"));
            GUI.color = col;

            DrawHeader();

            GUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(18)))
            {
                _parent.DeleteAssetPlacement(this);
            }
            GUILayout.EndHorizontal();

            DrawAssets();
        }

        protected abstract void DrawAssets();

        private void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public abstract SerializedDataErrorDetails Deserialize(TerrainGenerationLayeredAssetBaseData obj);
        public abstract TerrainGenerationLayeredAssetBaseData Serialize();
    }
}
