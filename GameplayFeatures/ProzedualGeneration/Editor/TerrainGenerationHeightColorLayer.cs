using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General;
using UnityToolbox.General.Management.Editor;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor
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
        }

        public TerrainGenerationHeightColorLayer(TerrainGenerationHeightColorsWindow parent, int currentPos, bool isBaseLayer, string baseLayerName)
        {
            _parent = parent;
            _data.IsBaseLayer = isBaseLayer;
            _data.Name = baseLayerName;
            _data.CurrentPos = currentPos;
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
            GUILayout.Label("Color Strength PCT: ");
            _data.ColorStrengthPCT = EditorGUILayout.Slider(_data.ColorStrengthPCT, 0f, 1f, GUILayout.Width(200));
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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture Scale: ");
            _data.TextureScale = EditorGUILayout.FloatField(_data.TextureScale, GUILayout.Width(200));
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
