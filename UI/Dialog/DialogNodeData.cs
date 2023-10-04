using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Dialog
{
    /// <summary>
    /// The dialog node data which is used at runtime and to serialize the dialog graph.
    /// </summary>
    [Serializable]
    public class DialogNodeData
    {
        public int ID;
        public bool IsLocalizzed;
        public string DialogIdentifier;
        public string StateForDialogIdentifier;
        public string GamestateToComplete;
        public string AvatarReference;
        [JsonIgnore][NonSerialized] public Texture2D Avatar;
        public string Title;
        public LocalizationID TitleLocalizzed;
        public string Text;
        public LocalizationID TextLocalizzed;
        public List<string> Options;
        public List<LocalizationID> OptionsLocalizzed;
        public VectorData Position;
        public List<int> InputIDs;
        public List<int> OutputIDs;
    }

}