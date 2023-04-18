using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Defines serializable vector data.
/// </summary>
[Serializable]
public class VectorData : GameData
{
    public VectorData()
    {

    }

    public VectorData(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public VectorData(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public VectorData(Vector4 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
        w = vector.w;
    }

    public float x;
    public float y;
    public float z;
    public float w;
}
