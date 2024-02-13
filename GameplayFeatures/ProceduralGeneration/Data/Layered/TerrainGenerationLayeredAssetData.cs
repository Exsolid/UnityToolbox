using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered
{
    /// <summary>
    /// The data for single asset placement.
    /// </summary>
    public class TerrainGenerationLayeredAssetData : TerrainGenerationLayeredAssetBaseData
    {
        public string PrefabGUID;
        public string PrefabPath;
        [JsonIgnore]
        [NonSerialized]
        public GameObject Prefab;
        public bool CanCollide;
        public bool DisableRaycastPlacement;
    }
}