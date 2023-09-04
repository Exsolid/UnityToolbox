using System;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    /// <summary>
    /// Defines a parent ID to be serialized.
    /// </summary>
    [Serializable]
    public class ParentData : GameData
    {
        public string ParentID;
    }
}
