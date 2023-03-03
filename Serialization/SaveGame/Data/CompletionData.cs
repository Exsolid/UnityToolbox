using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CompletionData : GameData
{
    public Dictionary<string, bool> IDToCompletion;

    public CompletionData()
    {
        IDToCompletion = new Dictionary<string,bool>();
    }
}
