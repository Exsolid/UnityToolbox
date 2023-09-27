using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.UI.Localisation;

namespace UnityToolbox.UI.Dialog
{
    /// <summary>
    /// The dialog node data which is used at runtime and to serialize the dialog graph.
    /// </summary>
    [Serializable]
    public class DialogNodeData
    {
        public int ID;
        public bool IsLocalized;
        public string DialogIdentifier;
        public string StateForDialogIdentifier;
        public string GamestateToComplete;
        public string AvatarReference;
        [JsonIgnore][NonSerialized] public Texture2D Avatar;
        public string Title;
        public LocalisationID TitleLocalized;
        public string Text;
        public LocalisationID TextLocalized;
        public List<string> Options;
        public List<LocalisationID> OptionsLocalized;
        public VectorData Position;
        public List<int> InputIDs;
        public List<int> OutputIDs;
    }

}