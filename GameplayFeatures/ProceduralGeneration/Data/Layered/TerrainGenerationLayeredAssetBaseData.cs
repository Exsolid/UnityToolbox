using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered
{
    /// <summary>
    /// The data for asset placement.
    /// </summary>
    public class TerrainGenerationLayeredAssetBaseData: ICloneable
    {
        public float HeightOffset;
        public bool PreIterate;
        public float OddsForSpawn;
        public TerrainGenerationAssetPosition Position;

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
