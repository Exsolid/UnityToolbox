using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    public interface ISerializedDataContainer<T>
    {
        public abstract void Deserialize(T obj);
        public abstract T Serialize();
    }
}
