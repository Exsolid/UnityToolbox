using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.Layered;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    public class TerrainGeneration : Module
    {
        private TerrainGenerationGenerator _generator;

        [Header("Material Settings")]
        [SerializeField] public Material TerrainMaterial;
        [SerializeField] protected bool _autoUpdate;
        [SerializeField] protected TextureSizes _textureSize;
        [SerializeField] protected TextureFormat _textureFormat;
        protected List<TerrainGenerationHeightColorData> _allHeightData;

        [Header("Terrain Settings")]
        [SerializeField] private GameObject _terrainObject;
        public Dictionary<string, TerrainGenerationData> Data;
        [SerializeField][HideInInspector] public int SelectedData;
        [SerializeField] private LayerMask _groundLayerMask;


        public List<Texture2DArray> Texture2DArrays;
        public List<Texture2D> Texture2Ds;

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

                _generator.SetData(dataToUse, _terrainObject, _groundLayerMask);
                SetHeightColors();
                UpdateMaterial();
                _generator.Mat = TerrainMaterial;
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
                data.NormalTexture = Resources.Load<Texture2D>(data.NormalTexturePath);
                data.EmissionTexture = Resources.Load<Texture2D>(data.EmissionTexturePath);
                data.OcclusionTexture = Resources.Load<Texture2D>(data.OcclusionTexturePath);
                data.MetallicTexture = Resources.Load<Texture2D>(data.MetallicTexturePath);
                data.RoughnessTexture = Resources.Load<Texture2D>(data.RoughnessTexturePath);
                data.TerrainColor = data.TerrainColorData.GetColor();
                _allHeightData[index] = data;
            }
        }

        private void UpdateMaterial()
        {
            if (!_allHeightData.Any())
            {
                return;
            }

            Texture2DArrays = new List<Texture2DArray>();
            Texture2Ds = new List<Texture2D>();

            _allHeightData = _allHeightData.OrderBy(x => x.StartingHeightPCT).ToList();

            TerrainMaterial.SetFloat("_MinHeight", _generator.GetLowestHeight());
            TerrainMaterial.SetFloat("_MaxHeight", _generator.GetHighestHeight());

            Color[] colors = new Color[_allHeightData.Count];
            float[] textureScales = new float[_allHeightData.Count];
            float[] heights = new float[_allHeightData.Count];
            float[] blends = new float[_allHeightData.Count];
            float[] smoothness = new float[_allHeightData.Count];

            Texture2D[] terrainMaps = new Texture2D[_allHeightData.Count];
            Texture2D[] normalMaps = new Texture2D[_allHeightData.Count];
            Texture2D[] emissionMaps = new Texture2D[_allHeightData.Count];
            Texture2D[] occlusionMaps = new Texture2D[_allHeightData.Count];
            Texture2D[] metallicMaps = new Texture2D[_allHeightData.Count];
            Texture2D[] roughnessMaps = new Texture2D[_allHeightData.Count];
            int count = 0;
            for (var index = 0; index < _allHeightData.Count; index++)
            {
                var type = _allHeightData[index];
                colors[count] = type.TerrainColor;

                if (type.TextureScale == 0)
                {
                    type.TextureScale = 1;
                }

                textureScales[count] = type.TextureScale;

                heights[count] = type.StartingHeightPCT;
                blends[count] = type.BlendAmountPCT;

                terrainMaps[count] = type.TerrainTexture;
                normalMaps[count] = type.NormalTexture;
                occlusionMaps[count] = type.OcclusionTexture;
                metallicMaps[count] = type.MetallicTexture;
                emissionMaps[count] = type.EmissionTexture;
                roughnessMaps[count] = type.RoughnessTexture;

                smoothness[count] = type.Smoothness;

                count++;
            }

            TerrainMaterial.SetInt("_LayerCount", _allHeightData.Count);

            TerrainMaterial.SetTexture("_BaseColors", GenerateTextureForColorArray(colors, "_BaseColors"));

            TerrainMaterial.SetTexture("_BaseStartHeights", GenerateTextureForFloatArray(heights, true, "_BaseStartHeights"));
            TerrainMaterial.SetTexture("_BaseBlends", GenerateTextureForFloatArray(blends, true,"_BaseBlends"));

            TerrainMaterial.SetTexture("_Smoothness", GenerateTextureForFloatArray(smoothness, true, "_Smoothness"));

            TerrainMaterial.SetTexture("_BaseTextureScales", GenerateTextureForFloatArrayScales(textureScales, "_BaseTextureScales"));
            TerrainMaterial.SetTexture("_BaseTextures", GenerateTextureArrayWithDefaultBase(terrainMaps, new Color(1, 1, 1, 1), "_BaseTextures"));

            LocalKeywordSpace keyWords = TerrainMaterial.shader.keywordSpace;
            
            if (_allHeightData.Count != 0 && _allHeightData[0].NormalEnabled)
            {
                TerrainMaterial.SetTexture("_BaseNormalMaps", GenerateNormalArray(normalMaps, "_BaseNormalMaps"));
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_NORMALMAP"), _allHeightData[0].NormalEnabled);
            }
            else
            {
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_NORMALMAP"), false);
            }

            if (_allHeightData.Count != 0 && _allHeightData[0].EmissionEnabled)
            {
                TerrainMaterial.SetTexture("_BaseEmissionMaps", GenerateTextureArrayWithDefaultBase(emissionMaps, new Color(0, 0, 0, 0f), "_BaseEmissionMaps"));
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_EMISSIONMAP"), _allHeightData[0].EmissionEnabled);
            }
            else
            {
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_EMISSIONMAP"), false);
            }

            if (_allHeightData.Count != 0 && _allHeightData[0].OcclusionEnabled)
            {
                TerrainMaterial.SetTexture("_BaseOcclusionMaps", GenerateTextureArrayWithDefaultBase(occlusionMaps, new Color(1, 1,1, 1f), "_BaseOcclusionMaps"));
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_OCCLUSIONMAP"), _allHeightData[0].OcclusionEnabled);
            }
            else
            {
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_OCCLUSIONMAP"), false);
            }

            if (_allHeightData.Count != 0 && _allHeightData[0].MetallicEnabled)
            {
                TerrainMaterial.SetTexture("_BaseMetallicMaps", GenerateTextureArrayWithDefaultBase(metallicMaps, new Color(0, 0, 0, 0f), "_BaseMetallicMaps"));
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_METALLICMAP"), _allHeightData[0].MetallicEnabled);
            }
            else
            {
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_METALLICMAP"), false);
            }

            if (_allHeightData.Count != 0 && _allHeightData[0].RoughnessEnabled)
            {
                TerrainMaterial.SetTexture("_BaseRoughnessMaps", GenerateTextureArrayWithDefaultBase(roughnessMaps, new Color(0, 0, 0, 0f), "_BaseRoughnessMaps"));
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_ROUGHNESSMAP"), _allHeightData[0].RoughnessEnabled);
            }
            else
            {
                TerrainMaterial.SetKeyword(keyWords.FindKeyword("_ROUGHNESSMAP"), false);
            }
        }

        private Texture2D GenerateTextureForFloatArrayScales(float[] floatArray, string textureName)
        {
            Texture2D empty = new Texture2D(floatArray.Length, 1);
            empty.filterMode = FilterMode.Point;
            for (int i = 0; i < floatArray.Length; i++)
            {
                float valueAsColor = (floatArray[i] / 255f);
                empty.SetPixel(i, 0, new Color(valueAsColor, valueAsColor, valueAsColor, valueAsColor));
            }
            empty.Apply();
            empty.name = textureName;
            Texture2Ds.Add(empty);
            return empty;
        }

        private Texture2D GenerateTextureForFloatArray(float[] floatArray, bool valuesAreZeroToOne, string textureName)
        {
            Texture2D empty = new Texture2D(floatArray.Length, 1);
            empty.filterMode = FilterMode.Point;
            for (int i = 0; i < floatArray.Length; i++)
            {
                float valueAsColor = (floatArray[i] / (valuesAreZeroToOne ? 1 : 255f));
                empty.SetPixel(i, 0, new Color(valueAsColor, valueAsColor, valueAsColor, valueAsColor));
            }
            empty.Apply();
            empty.name = textureName;
            Texture2Ds.Add(empty);
            return empty;
        }

        private Texture2D GenerateTextureForColorArray(Color[] colorArray, string textureName)
        {
            Texture2D empty = new Texture2D(colorArray.Length, 1);
            empty.filterMode = FilterMode.Point;
            for (int i = 0; i < colorArray.Length; i++)
            {
                empty.SetPixel(i, 0, colorArray[i]);
            }
            empty.Apply();
            empty.name = textureName;
            Texture2Ds.Add(empty);
            return empty;
        }

        private Texture2DArray GenerateTextureArrayWithDefaultBase(Texture2D[] textures, Color defaultColor, string textureArrayName)
        {
            Texture2DArray texture2DArray = new Texture2DArray((int)_textureSize, (int)_textureSize, textures.Length, _textureFormat, true);
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] == null)
                {
                    Texture2D empty = new Texture2D((int)_textureSize, (int)_textureSize);
                    for (int x = 0; x < (int)_textureSize; x++)
                    {
                        for (int y = 0; y < (int)_textureSize; y++)
                        {
                            empty.SetPixel(x, y, defaultColor);
                        }
                    }
                    texture2DArray.SetPixels(empty.GetPixels(), i);
                }
                else
                {
                    texture2DArray.SetPixels(textures[i].GetPixels(), i);
                }
            }
            texture2DArray.Apply();
            texture2DArray.name = textureArrayName;
            Texture2DArrays.Add(texture2DArray);
            return texture2DArray;
        }

        private Texture2DArray GenerateNormalArray(Texture2D[] textures, string textureArrayName)
        {
            Texture2DArray texture2DArray = new Texture2DArray((int)_textureSize, (int)_textureSize, textures.Length, _textureFormat, true, true);
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] == null)
                {
                    Texture2D empty = new Texture2D((int)_textureSize, (int)_textureSize, _textureFormat, true);
                    empty.filterMode = FilterMode.Trilinear;
                    empty.wrapMode = TextureWrapMode.Clamp;
                    for (int x = 0; x < (int)_textureSize; x++)
                    {
                        for (int y = 0; y < (int)_textureSize; y++)
                        {
                            empty.SetPixel(x,y,new Color(1f, 0.5f, 0.5f, 1));
                        }
                    }
                    texture2DArray.SetPixels(empty.GetPixels(), i);
                }
                else
                {
                    texture2DArray.SetPixels(textures[i].GetPixels(), i);
                }
            }
            texture2DArray.Apply();
            texture2DArray.name = textureArrayName;
            Texture2DArrays.Add(texture2DArray);
            return texture2DArray;
        }
    }
}
