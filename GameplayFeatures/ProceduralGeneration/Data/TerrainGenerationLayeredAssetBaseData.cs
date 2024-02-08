using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainGenerationLayeredAssetBaseData: ICloneable
    {
        public float HeightOffset;
        public bool PreIterate;
        public float OddsForSpawn;
        public TerrainGenerationAssetPosition Position;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
