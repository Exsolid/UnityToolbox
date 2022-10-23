using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
