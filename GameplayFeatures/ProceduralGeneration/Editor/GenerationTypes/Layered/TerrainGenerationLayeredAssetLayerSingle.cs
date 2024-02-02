using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Management.Editor;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered
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
            GameObject prev = _asset.Prefab;
            _asset.Prefab = (GameObject)EditorGUILayout.ObjectField(_asset.Prefab, typeof(GameObject), false, GUILayout.Width(180));
            if (_asset.Prefab != null && !_asset.Prefab.Equals(prev))
            {
                if (!EditorResourceUtil.IsAssetValid(_asset.Prefab))
                {
                    _asset.Prefab = prev;
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("The path belonging to the prefab is not a \"Resource\" directory.");
                }
                else
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the prefab.");
                }
            }
            else if (_asset.Prefab == null && prev != null)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the prefab.");
            }
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

        public override SerializedDataErrorDetails Deserialize(TerrainGenerationLayeredAssetBaseData obj)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            _asset = (TerrainGenerationLayeredAssetData) obj;

            if (_asset.PrefabPath != null)
            {
                _asset.Prefab = EditorResourceUtil.GetAssetWithResourcesPath<GameObject>(_asset.PrefabPath);
                if (_asset.Prefab == null)
                {
                    string prevPath = _asset.PrefabPath;
                    try
                    {
                        _asset.Prefab = EditorResourceUtil.GetAssetWithGUID<GameObject>(_asset.PrefabGUID);
                        _asset.PrefabPath = EditorResourceUtil.GetResourcesPathWithAsset(_asset.Prefab);

                        if (_asset.Prefab == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _asset.PrefabPath = prevPath;
                    }
                }
            }

            _rewriteErrors = err.HasErrors;

            return err;
        }

        public override TerrainGenerationLayeredAssetBaseData Serialize()
        {
            if (_asset.Prefab != null)
            {
                _asset.PrefabPath = EditorResourceUtil.GetResourcesPathWithAsset(_asset.Prefab);
                _asset.PrefabGUID = EditorResourceUtil.GetGUIDOfAsset(_asset.Prefab).ToString();
            }
            else if(!_rewriteErrors)
            {
                _asset.PrefabPath = null;
                _asset.PrefabGUID = null;
            }

            return _asset;
        }
    }
}
