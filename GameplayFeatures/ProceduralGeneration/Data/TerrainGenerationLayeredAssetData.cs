using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainGenerationLayeredAssetData : TerrainGenerationLayeredAssetBaseData
    {
        public string PrefabGUID;
        public string PrefabPath;
        [JsonIgnore]
        [NonSerialized]
        public GameObject Prefab;
        public bool CanCollide;
    }
}