using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HeightColorTypes
{
    public string Name;
    [Range(0, 1)] public float HeightStart;
    [Range(0, 1)] public float BlendAmount;
    public Color TerrainColor;
    [Range(0, 1)] public float TerrainColorStrength;
    public Texture2D TerrainTexture;
    public float TerrainTextureScale;
}
