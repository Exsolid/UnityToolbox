using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DialogNodeData
{
    public int ID;
    public string DialogIndentifier;
    public string CompletionToSet;
    public string AvatarReference;
    [NonSerialized] public Texture2D Avatar;
    public string Title;
    public string Text;
    public List<string> Options;
    public VectorData Position;
    public List<int> InputIDs;
    public List<int> OutputIDs;
}