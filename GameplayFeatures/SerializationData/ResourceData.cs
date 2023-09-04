using System;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    /// <summary>
    /// Defines a reference to the prefab which is serialized.
    /// </summary>
    [Serializable]
    public class ResourceData: GameData
    {
        public string ResourcePath;
    }
}
