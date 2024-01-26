using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Enums;
using UnityToolbox.General.Management.Editor;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes.Layered
{
    public class TerrainGenerationLayeredAssetLayerSingle : TerrainGenerationLayeredAssetLayer
    {
        private TerrainGenerationLayeredAssetData _asset;

        public TerrainGenerationLayeredAssetLayerSingle(TerrainMeshTypeLayeredLayer parent) : base(parent)
        {
            _asset = new TerrainGenerationLayeredAssetData();
        }

        protected override void DrawAssets()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefab: ");
            _asset.Prefab = (GameObject)EditorGUILayout.ObjectField(_asset.Prefab, typeof(GameObject), false, GUILayout.Width(180));
            //TODO Check if resource object and move
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Is Anchor: ");
            GUILayout.FlexibleSpace();
            _asset.IsAnchor = EditorGUILayout.Toggle(_asset.IsAnchor, GUILayout.Width(20));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use Raycast Placement: ");
            GUILayout.FlexibleSpace();
            _asset.RaycastPlacement = EditorGUILayout.Toggle(_asset.RaycastPlacement, GUILayout.Width(20));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Odds for Spawn: ");
            _asset.OddsForSpawn = EditorGUILayout.Slider(_asset.OddsForSpawn, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Position on Layer: ");
            _asset.Position = (TerrainGenerationAssetPosition) EditorGUILayout.Popup((int) _asset.Position, System.Enum.GetNames(typeof(TerrainGenerationAssetPosition)), GUILayout.Width(200));
            GUILayout.EndHorizontal();

        }

        public override void Deserialize(TerrainGenerationLayeredAssetBaseData obj)
        {
            _asset = (TerrainGenerationLayeredAssetData) obj;

            if (_asset.PrefabPath != null)
            {
                try
                {
                    _asset.Prefab = EditorResourceUtil.GetAssetWithPath<GameObject>(_asset.PrefabPath);
                }
                catch
                {
                    _asset.Prefab = EditorResourceUtil.GetAssetWithGUID<GameObject>(_asset.PrefabGUID);
                    _asset.PrefabPath = EditorResourceUtil.GetAssetPathWithAsset(_asset.Prefab);
                }
            }
        }

        public override TerrainGenerationLayeredAssetBaseData Serialize()
        {
            if (_asset.Prefab != null)
            {
                _asset.PrefabPath = EditorResourceUtil.GetAssetPathWithAsset(_asset.Prefab);
                _asset.PrefabGUID = EditorResourceUtil.GetGUIDOfAsset(_asset.Prefab).ToString();
            }
            else
            {
                _asset.PrefabPath = null;
                _asset.PrefabGUID = null;
            }

            return _asset;
        }
    }
}
