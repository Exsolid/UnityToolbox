using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    public class TerrainGeneration : Module
    {
        private TerrainGenerationGenerator _generator;
        private Texture2DArray texture2DArray; //Reference needed so garbage collection doesn't destroy shader texture

        [Header("Material Settings")]
        [SerializeField] protected Material _material;
        [SerializeField] protected bool _autoUpdate;
        [SerializeField] protected TextureSizes _textureSize;
        [SerializeField] protected TextureFormat _textureFormat;
        protected List<TerrainGenerationHeightColorData> _allHeightData;

        [Header("Terrain Settings")]
        [SerializeField] private GameObject _terrainObject;
        public Dictionary<string, TerrainGenerationData> Data;
        [SerializeField][HideInInspector] public int SelectedData;

        public void GenerateTerrain()
        {
            Data = TerrainGenerationIO.Instance.ReadData();
            if (Data.Count == 0)
            {
                throw new DataException("Cannot generate terrain without data.");
            }

            if (SelectedData >= Data.Count)
            {
                throw new DataException("The selected data index exceeds the data count.");
            }

            try
            {
                TerrainGenerationData dataToUse = Data.Values.ElementAt(SelectedData);

                if (dataToUse.MeshData.GetType() == typeof(TerrainMeshTypeLayeredData))
                {
                    _generator = new TerrainGenerationLayered();
                }

                _generator.SetData(dataToUse, _terrainObject);
                SetHeightColors();
                UpdateMaterial();
                _generator.Mat = _material;
                _generator.Generate();
            }
            catch (Exception ex)
            {
                throw new Exception("The terrain generation has encountered an error. Is the data still valid?", ex);
            }
        }

        private void SetHeightColors()
        {
            _allHeightData = _generator.CalculateHeights();
            for (int index = 0; index < _allHeightData.Count; index++)
            {
                TerrainGenerationHeightColorData data = _allHeightData[index];
                data.TerrainTexture = Resources.Load<Texture2D>(data.TerrainTexturePath);
                _allHeightData[index] = data;
            }
        }

        private void UpdateMaterial()
        {
            if (!_allHeightData.Any())
            {
                return;
            }

            _allHeightData = _allHeightData.OrderBy(x => x.StartingHeightPCT).ToList();

            _material.SetFloat("minHeight", 0);
            _material.SetFloat("maxHeight", _generator.GetHighestHeight());

            Color[] colors = new Color[_allHeightData.Count];
            float[] colStrengths = new float[_allHeightData.Count];
            float[] textureScales = new float[_allHeightData.Count];
            float[] heights = new float[_allHeightData.Count];
            float[] blends = new float[_allHeightData.Count];
            Texture2D[] textures = new Texture2D[_allHeightData.Count];
            int count = 0;
            for (var index = 0; index < _allHeightData.Count; index++)
            {
                var type = _allHeightData[index];
                colors[count] = type.TerrainColor;
                colStrengths[count] = type.ColorStrengthPCT;

                if (type.TextureScale == 0)
                {
                    type.TextureScale = 1;
                }

                textureScales[count] = type.TextureScale;

                heights[count] = type.StartingHeightPCT;
                blends[count] = type.BlendAmountPCT;

                if (type.TerrainTexture == null)
                {
                    return;
                }

                textures[count] = type.TerrainTexture;

                count++;
            }

            _material.SetInt("layerCount", _allHeightData.Count);

            _material.SetColorArray("baseColors", colors);
            _material.SetFloatArray("baseColorStrengths", colStrengths);

            _material.SetFloatArray("baseStartHeights", heights);
            _material.SetFloatArray("baseBlends", blends);

            _material.SetFloatArray("baseTextureScales", textureScales);
            _material.SetTexture("baseTextures", GenerateTextureArray(textures));
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            texture2DArray = new Texture2DArray((int)_textureSize, (int)_textureSize, textures.Length, _textureFormat, true);
            for (int i = 0; i < textures.Length; i++)
            {
                texture2DArray.SetPixels(textures[i].GetPixels(), i);
            }
            texture2DArray.Apply();
            return texture2DArray;
        }
    }
}
