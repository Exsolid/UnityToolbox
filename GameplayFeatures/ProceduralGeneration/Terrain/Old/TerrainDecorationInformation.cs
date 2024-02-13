using System;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    /// <summary>
    /// Defines information on how an object is placed.
    /// </summary>
    [Serializable]
    public class TerrainDecorationInformation : ICloneable
    {
        [Range(0.1f, 1)] public float Weight;
        public float HeightOffset;
        public GameObject Object;
        public bool WidthPlacement;

    
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}