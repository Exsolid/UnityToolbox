using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General;
using UnityToolbox.General.Management.Editor;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    public class TerrainGenerationHeightColorLayer: ISerializedDataContainer<TerrainGenerationHeightColorData>
    {
        private TerrainGenerationHeightColorsWindow _parent;
        private bool _rewriteErrors;

        private TerrainGenerationHeightColorData _data;
        public TerrainGenerationHeightColorData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TerrainGenerationHeightColorLayer(TerrainGenerationHeightColorsWindow parent, int currentPos)
        {
            _parent = parent;
            _data.CurrentPos = currentPos;
            _data.TerrainColor = Color.white;
            _data.TextureScale = 20;
        }

        public TerrainGenerationHeightColorLayer(TerrainGenerationHeightColorsWindow parent, int currentPos, bool isBaseLayer, string baseLayerName)
        {
            _parent = parent;
            _data.IsBaseLayer = isBaseLayer;
            _data.Name = baseLayerName;
            _data.CurrentPos = currentPos;
            _data.TerrainColor = Color.white;
            _data.TextureScale = 20;
        }

        public SerializedDataErrorDetails Deserialize(TerrainGenerationHeightColorData obj)
        {
            SerializedDataErrorDetails err = new SerializedDataErrorDetails();
            _data = obj;
            if (_data.TerrainTexturePath != null)
            {
                _data.TerrainTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.TerrainTexturePath);
                if (_data.TerrainTexture == null)
                {
                    string prevPath = _data.TerrainTexturePath;
                    try
                    {
                        _data.TerrainTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.TerrainTextureGUID);
                        _data.TerrainTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.TerrainTexture);

                        if (_data.TerrainTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.TerrainTexturePath = prevPath;
                    }
                }
            }

            if (_data.NormalTexturePath != null)
            {
                _data.NormalTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.NormalTexturePath);
                if (_data.NormalTexture == null)
                {
                    string prevPath = _data.NormalTexturePath;
                    try
                    {
                        _data.NormalTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.NormalTextureGUID);
                        _data.NormalTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.NormalTexture);

                        if (_data.NormalTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.NormalTexturePath = prevPath;
                    }
                }
            }

            if (_data.EmissionTexturePath != null)
            {
                _data.EmissionTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.EmissionTexturePath);
                if (_data.EmissionTexture == null)
                {
                    string prevPath = _data.EmissionTexturePath;
                    try
                    {
                        _data.EmissionTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.EmissionTextureGUID);
                        _data.EmissionTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.EmissionTexture);

                        if (_data.EmissionTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.EmissionTexturePath = prevPath;
                    }
                }
            }

            if (_data.MetallicTexturePath != null)
            {
                _data.MetallicTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.MetallicTexturePath);
                if (_data.MetallicTexture == null)
                {
                    string prevPath = _data.MetallicTexturePath;
                    try
                    {
                        _data.MetallicTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.MetallicTextureGUID);
                        _data.MetallicTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.MetallicTexture);

                        if (_data.MetallicTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.MetallicTexturePath = prevPath;
                    }
                }
            }

            if (_data.OcclusionTexturePath != null)
            {
                _data.OcclusionTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.OcclusionTexturePath);
                if (_data.OcclusionTexture == null)
                {
                    string prevPath = _data.OcclusionTexturePath;
                    try
                    {
                        _data.OcclusionTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.OcclusionTextureGUID);
                        _data.OcclusionTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.OcclusionTexture);

                        if (_data.OcclusionTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.OcclusionTexturePath = prevPath;
                    }
                }
            }

            if (_data.RoughnessTexturePath != null)
            {
                _data.RoughnessTexture = EditorResourceUtil.GetAssetWithResourcesPath<Texture2D>(_data.RoughnessTexturePath);
                if (_data.RoughnessTexture == null)
                {
                    string prevPath = _data.RoughnessTexturePath;
                    try
                    {
                        _data.RoughnessTexture = EditorResourceUtil.GetAssetWithGUID<Texture2D>(_data.RoughnessTextureGUID);
                        _data.RoughnessTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.RoughnessTexture);

                        if (_data.RoughnessTexture == null)
                        {
                            throw new SystemException();
                        }
                    }
                    catch
                    {
                        err.HasErrors = true;
                        err.ErrorDescription = "The asset at path: " + prevPath + " cannot be found anymore.";
                        _data.RoughnessTexturePath = prevPath;
                    }
                }
            }

            _data.TerrainColor = _data.TerrainColorData.GetColor();
            _rewriteErrors = err.HasErrors;
            return err;
        }

        public TerrainGenerationHeightColorData Serialize()
        {
            if (_data.TerrainTexture != null)
            {
                _data.TerrainTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.TerrainTexture);
                _data.TerrainTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.TerrainTexture).ToString();
            }
            else if(!_rewriteErrors)
            {
                _data.TerrainTexturePath = null;
                _data.TerrainTextureGUID = null;
            }

            if (_data.NormalTexture != null)
            {
                _data.NormalTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.NormalTexture);
                _data.NormalTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.NormalTexture).ToString();
            }
            else if (!_rewriteErrors)
            {
                _data.NormalTexturePath = null;
                _data.NormalTextureGUID = null;
            }

            if (_data.EmissionTexture != null)
            {
                _data.EmissionTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.EmissionTexture);
                _data.EmissionTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.EmissionTexture).ToString();
            }
            else if (!_rewriteErrors)
            {
                _data.EmissionTexturePath = null;
                _data.EmissionTextureGUID = null;
            }

            if (_data.MetallicTexture != null)
            {
                _data.MetallicTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.MetallicTexture);
                _data.MetallicTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.MetallicTexture).ToString();
            }
            else if (!_rewriteErrors)
            {
                _data.MetallicTexturePath = null;
                _data.MetallicTextureGUID = null;
            }

            if (_data.OcclusionTexture != null)
            {
                _data.OcclusionTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.OcclusionTexture);
                _data.OcclusionTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.OcclusionTexture).ToString();
            }
            else if (!_rewriteErrors)
            {
                _data.OcclusionTexturePath = null;
                _data.OcclusionTextureGUID = null;
            }

            if (_data.RoughnessTexture != null)
            {
                _data.RoughnessTexturePath = EditorResourceUtil.GetResourcesPathWithAsset(_data.RoughnessTexture);
                _data.RoughnessTextureGUID = EditorResourceUtil.GetGUIDOfAsset(_data.RoughnessTexture).ToString();
            }
            else if (!_rewriteErrors)
            {
                _data.RoughnessTexturePath = null;
                _data.RoughnessTextureGUID = null;
            }
            _data.TerrainColorData = new ColorData(_data.TerrainColor);
            return _data;
        }

        public void DrawDetails()
        {
            Color col = GUI.color;
            GUI.color = new Color(82f / 255f, 33f / 255f, 37f / 255f, 0.2f);
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(180));
            GUI.color = col;

            DrawHeader();

            DrawLineHorizontal();

            DrawColorDetails();

            GUILayout.EndVertical();
        }

        private void DrawColorDetails()
        {
            Color col = GUI.color;
            GUI.color = new Color(245f / 255f, 132f / 255f, 66f / 255f, 0.4f);
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(180));
            GUI.color = col;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Starting Height PCT: ");
            _data.StartingHeightPCT = EditorGUILayout.Slider(_data.StartingHeightPCT, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Blend Amount PCT: ");
            _data.BlendAmountPCT = EditorGUILayout.Slider(_data.BlendAmountPCT, 0f, 1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture Color: ");
            _data.TerrainColor = EditorGUILayout.ColorField(_data.TerrainColor, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture: ");
            Texture2D prev = _data.TerrainTexture;
            _data.TerrainTexture = (Texture2D)EditorGUILayout.ObjectField(_data.TerrainTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));       
            if (_data.TerrainTexture != null && !_data.TerrainTexture.Equals(prev))
            {
                if (!EditorResourceUtil.IsAssetValid(_data.TerrainTexture))
                {
                    _data.TerrainTexture = prev;
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("The path belonging to the terrain texture is not a \"Resource\" directory.");
                }
                else
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the terrain texture.");
                }
            }
            else if (_data.TerrainTexture == null && prev != null)
            {
                TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the terrain texture.");
            }
            GUILayout.EndHorizontal();

            if (_data.NormalEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Normal Map: ");
                prev = _data.NormalTexture;
                _data.NormalTexture = (Texture2D)EditorGUILayout.ObjectField(_data.NormalTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                if (_data.NormalTexture != null && !_data.NormalTexture.Equals(prev))
                {
                    if (!EditorResourceUtil.IsAssetValid(_data.NormalTexture))
                    {
                        _data.NormalTexture = prev;
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("The path belonging to the Normal texture is not a \"Resource\" directory.");
                    }
                    else
                    {
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Normal texture.");
                    }
                }
                else if (_data.NormalTexture == null && prev != null)
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Normal texture.");
                }
                GUILayout.EndHorizontal();
            }


            if (_data.EmissionEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Emission Map: ");
                prev = _data.EmissionTexture;
                _data.EmissionTexture = (Texture2D)EditorGUILayout.ObjectField(_data.EmissionTexture, typeof(Texture2D),
                    false, GUILayout.Width(70), GUILayout.Height(70));
                if (_data.EmissionTexture != null && !_data.EmissionTexture.Equals(prev))
                {
                    if (!EditorResourceUtil.IsAssetValid(_data.EmissionTexture))
                    {
                        _data.EmissionTexture = prev;
                        TerrainGenerationEditorEvents.Instance.UpdateStatus(
                            "The path belonging to the Emission texture is not a \"Resource\" directory.");
                    }
                    else
                    {
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Emission texture.");
                    }
                }
                else if (_data.EmissionTexture == null && prev != null)
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Emission texture.");
                }

                GUILayout.EndHorizontal();
            }

            if (_data.MetallicEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Metallic Map: ");
                prev = _data.MetallicTexture;
                _data.MetallicTexture = (Texture2D)EditorGUILayout.ObjectField(_data.MetallicTexture, typeof(Texture2D),
                    false, GUILayout.Width(70), GUILayout.Height(70));
                if (_data.MetallicTexture != null && !_data.MetallicTexture.Equals(prev))
                {
                    if (!EditorResourceUtil.IsAssetValid(_data.MetallicTexture))
                    {
                        _data.MetallicTexture = prev;
                        TerrainGenerationEditorEvents.Instance.UpdateStatus(
                            "The path belonging to the Metallic texture is not a \"Resource\" directory.");
                    }
                    else
                    {
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Metallic texture.");
                    }
                }
                else if (_data.MetallicTexture == null && prev != null)
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Metallic texture.");
                }

                GUILayout.EndHorizontal();

                if (_data.RoughnessEnabled)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Roughness Map: ");
                    prev = _data.RoughnessTexture;
                    _data.RoughnessTexture = (Texture2D)EditorGUILayout.ObjectField(_data.RoughnessTexture,
                        typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                    if (_data.RoughnessTexture != null && !_data.RoughnessTexture.Equals(prev))
                    {
                        if (!EditorResourceUtil.IsAssetValid(_data.RoughnessTexture))
                        {
                            _data.RoughnessTexture = prev;
                            TerrainGenerationEditorEvents.Instance.UpdateStatus(
                                "The path belonging to the Roughness texture is not a \"Resource\" directory.");
                        }
                        else
                        {
                            TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Roughness texture.");
                        }
                    }
                    else if (_data.RoughnessTexture == null && prev != null)
                    {
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Roughness texture.");
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Smoothness: ");
                _data.Smoothness = EditorGUILayout.Slider(_data.Smoothness, 0f, 1f, GUILayout.Width(200));
                GUILayout.EndHorizontal();
            }

            if (_data.OcclusionEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Occlusion Map: ");
                prev = _data.OcclusionTexture;
                _data.OcclusionTexture = (Texture2D)EditorGUILayout.ObjectField(_data.OcclusionTexture,
                    typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                if (_data.OcclusionTexture != null && !_data.OcclusionTexture.Equals(prev))
                {
                    if (!EditorResourceUtil.IsAssetValid(_data.OcclusionTexture))
                    {
                        _data.OcclusionTexture = prev;
                        TerrainGenerationEditorEvents.Instance.UpdateStatus(
                            "The path belonging to the Occlusion texture is not a \"Resource\" directory.");
                    }
                    else
                    {
                        TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Occlusion texture.");
                    }
                }
                else if (_data.OcclusionTexture == null && prev != null)
                {
                    TerrainGenerationEditorEvents.Instance.UpdateStatus("Updated the Occlusion texture.");
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture Scale: ");
            _data.TextureScale = EditorGUILayout.Slider(_data.TextureScale,1,255, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Layer: ");
            if (_data.IsBaseLayer)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(_data.Name);
            }
            else
            {
                _data.Name = GUILayout.TextField(_data.Name, GUILayout.Width(200));
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
                GUILayout.Label(_data.CurrentPos + "");
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
    }
}
