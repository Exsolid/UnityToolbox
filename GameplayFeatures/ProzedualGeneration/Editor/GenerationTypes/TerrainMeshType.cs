using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes
{
    public abstract class TerrainMeshType: ISerializedDataContainer<TerrainMeshTypeBaseData>
    {
        public abstract void DrawDetails();

        protected void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public abstract void Deserialize(TerrainMeshTypeBaseData obj);
        public abstract TerrainMeshTypeBaseData Serialize();
    }
}
