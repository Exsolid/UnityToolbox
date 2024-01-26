using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
{
    public class TerrainGenerationLayeredAssetData : TerrainGenerationLayeredAssetBaseData
    {
        public string PrefabGUID;
        public string PrefabPath;
        [JsonIgnore]
        [NonSerialized]
        public GameObject Prefab;
        public bool IsAnchor;
        public bool RaycastPlacement;
    }
}