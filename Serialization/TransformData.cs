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
        Position = new VectorData();
        Rotation = new VectorData();
        Scale = new VectorData();
        Position.x = transform.position.x;
        Position.y = transform.position.y;
        Position.z = transform.position.z;
        Rotation.x = transform.rotation.x;
        Rotation.y = transform.rotation.y;
        Rotation.z = transform.rotation.z;
        Scale.x = transform.localScale.x;
        Scale.y = transform.localScale.y;
        Scale.z = transform.localScale.z;
    }

    public VectorData Position;
    public VectorData Rotation;
    public VectorData Scale;
}
