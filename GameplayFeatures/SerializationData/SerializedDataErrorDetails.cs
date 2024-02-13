using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    /// <summary>
    /// Non serialized data, rather data to be used on serialization errors. See <see cref="ISerializedDataContainer{T}"/> for the usage.
    /// </summary>
    public class SerializedDataErrorDetails
    {
        public bool HasErrors;
        public string ErrorDescription;
        public List<SerializedDataErrorDetails> Traced = new();
    }
}