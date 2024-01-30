using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    public class SerializedDataErrorDetails
    {
        public bool HasErrors;
        public string ErrorDescription;
        public List<SerializedDataErrorDetails> Traced = new();
    }
}