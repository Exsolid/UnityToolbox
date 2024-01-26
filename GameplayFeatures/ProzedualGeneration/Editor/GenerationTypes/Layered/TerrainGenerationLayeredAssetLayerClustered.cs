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
    public class TerrainGenerationLayeredAssetLayerClustered : TerrainGenerationLayeredAssetLayer
    {
        private TerrainGenerationLayeredAssetClusterData _asset;

        public TerrainGenerationLayeredAssetLayerClustered(TerrainMeshTypeLayeredLayer parent) : base(parent)
        {
            _asset = new TerrainGenerationLayeredAssetClusterData();
            _asset.Assets = new List<TerrainGenerationLayeredAssetData>();
        }

        protected override void DrawAssets()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Odds for Spawn: ");
            _asset.OddsForSpawn = EditorGUILayout.Slider(_asset.OddsForSpawn, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Position on Layer: ");
            _asset.Position = (TerrainGenerationAssetPosition)EditorGUILayout.Popup((int)_asset.Position, System.Enum.GetNames(typeof(TerrainGenerationAssetPosition)), GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                _asset.Assets.Add(new TerrainGenerationLayeredAssetData());
            }
            GUILayout.EndHorizontal();

            List<TerrainGenerationLayeredAssetData> assets = _asset.Assets;
            for (int i = 0; i< _asset.Assets.Count; i++)
            {
                Color col = GUI.color;
                GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.2f);
                GUILayout.BeginVertical(new GUIStyle("window"));
                GUI.color = col;

                TerrainGenerationLayeredAssetData asset = _asset.Assets[i];

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(18)))
                {
                    assets.Remove(asset);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Prefab: ");
                asset.Prefab = (GameObject)EditorGUILayout.ObjectField(asset.Prefab, typeof(GameObject), false, GUILayout.Width(180));
                //TODO Check if resource object and move
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Is Anchor: ");
                GUILayout.FlexibleSpace();
                asset.IsAnchor = EditorGUILayout.Toggle(asset.IsAnchor, GUILayout.Width(20));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Use Raycast Placement: ");
                GUILayout.FlexibleSpace();
                asset.RaycastPlacement = EditorGUILayout.Toggle(asset.RaycastPlacement, GUILayout.Width(20));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Odds for Spawn: ");
                asset.OddsForSpawn = EditorGUILayout.Slider(asset.OddsForSpawn, 0f, 1f, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Position on Layer: ");
                asset.Position = (TerrainGenerationAssetPosition)EditorGUILayout.Popup((int)asset.Position, System.Enum.GetNames(typeof(TerrainGenerationAssetPosition)), GUILayout.Width(200));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            _asset.Assets = assets;
        }

        public override void Deserialize(TerrainGenerationLayeredAssetBaseData obj)
        {
            _asset = obj as TerrainGenerationLayeredAssetClusterData;
            List<TerrainGenerationLayeredAssetData> assets = _asset.Assets;
            for (int i = 0; i < _asset.Assets.Count; i++)
            {
                TerrainGenerationLayeredAssetData asset = _asset.Assets[i];
                if (asset.PrefabPath != null)
                {
                    try
                    {
                        asset.Prefab = EditorResourceUtil.GetAssetWithPath<GameObject>(asset.PrefabPath);
                    }
                    catch
                    {
                        asset.Prefab = EditorResourceUtil.GetAssetWithGUID<GameObject>(asset.PrefabGUID);
                        asset.PrefabPath = EditorResourceUtil.GetAssetPathWithAsset(asset.Prefab);
                    }
                }
            }
            _asset.Assets = assets;
        }

        public override TerrainGenerationLayeredAssetBaseData Serialize()
        {
            List<TerrainGenerationLayeredAssetData> assets = _asset.Assets;
            for (int i = 0; i < _asset.Assets.Count; i++)
            {
                TerrainGenerationLayeredAssetData asset = _asset.Assets[i];
                if (asset.Prefab != null)
                {
                    asset.PrefabPath = EditorResourceUtil.GetAssetPathWithAsset(asset.Prefab);
                    asset.PrefabGUID = EditorResourceUtil.GetGUIDOfAsset(asset.Prefab).ToString();
                }
                else
                {
                    asset.PrefabPath = null;
                    asset.PrefabGUID = null;
                }
            }
            _asset.Assets = assets;
            return _asset;
        }
    }
}
