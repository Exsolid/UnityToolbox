using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.UI.Dialog
{
    /// <summary>
    /// The dialog node data which is used at runtime and to serialize the dialog graph.
    /// </summary>
    [Serializable]
    public class DialogNodeData
    {
        public int ID;
        public string DialogIndentifier;
        public string StateForDialogIndentifier;
        public string GamestateToComplete;
        public string AvatarReference;
        [JsonIgnore][NonSerialized] public Texture2D Avatar;
        public string Title;
        public string Text;
        public List<string> Options;
        public VectorData Position;
        public List<int> InputIDs;
        public List<int> OutputIDs;
    }

}