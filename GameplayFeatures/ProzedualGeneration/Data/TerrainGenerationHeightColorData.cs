using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
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
        public float ColorStrengthPCT;
        public string TerrainTexturePath;
        public string TerrainTextureGUID;
        [JsonIgnore]
        [NonSerialized]
        public Texture2D TerrainTexture;
        public float TextureScale;
    }
}
