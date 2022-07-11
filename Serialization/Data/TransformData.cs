using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TransformData: GameData
{

    public TransformData()
    {

    }

    public TransformData(Transform transform)
    {
        Position = new VectorData(transform.position);
        Rotation = new VectorData(transform.rotation.eulerAngles);
        Scale = new VectorData(transform.localScale);
    }

    public VectorData Position;
    public VectorData Rotation;
    public VectorData Scale;
}
