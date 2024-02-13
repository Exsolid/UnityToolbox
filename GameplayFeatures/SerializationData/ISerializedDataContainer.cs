using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    /// <summary>
    /// An interface used to de/serialize data and give error details and fail.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializedDataContainer<T>
    {
        public abstract SerializedDataErrorDetails Deserialize(T obj);
        public abstract T Serialize();
    }
}
