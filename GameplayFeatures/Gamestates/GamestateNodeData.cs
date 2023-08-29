using System;
using System.Collections.Generic;
using UnityToolbox.GameplayFeatures.Serialization_Data;

namespace UnityToolbox.GameplayFeatures.Gamestates
{
    /// <summary>
    /// The gamestate node data which is used at runtime and to serialize the gamestate graph.
    /// </summary>
    [Serializable]
    public class GamestateNodeData
    {
        public int ID;
        public string Name;
        public bool IsActive;
        public List<int> InputIDs;
        public List<int> OutputIDs;
        public VectorData Position;
    }
}
