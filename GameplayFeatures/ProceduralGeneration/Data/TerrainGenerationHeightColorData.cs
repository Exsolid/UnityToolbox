using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public struct TerrainGenerationHeightColorData
    {
        public bool IsBaseLayer;
        public int CurrentPos;
        public string Name;
        public float StartingHeightPCT;
        public float BlendAmountPCT;
        [JsonIgnore]
        [NonSerialized]
        public Color TerrainColor;
        public ColorData TerrainColorData;
        public float TextureScale;

        public string TerrainTexturePath;
        public string TerrainTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D TerrainTexture;

        public bool NormalEnabled;
        public string NormalTexturePath;
        public string NormalTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D NormalTexture;

        public bool EmissionEnabled;
        public string EmissionTexturePath;
        public string EmissionTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D EmissionTexture;

        public bool MetallicEnabled;
        public string MetallicTexturePath;
        public string MetallicTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D MetallicTexture;

        public float Smoothness;

        public bool OcclusionEnabled;
        public string OcclusionTexturePath;
        public string OcclusionTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D OcclusionTexture;

        public bool RoughnessEnabled;
        public string RoughnessTexturePath;
        public string RoughnessTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D RoughnessTexture;
    }
}
