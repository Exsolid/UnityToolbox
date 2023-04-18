using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Defines all data for the completion states of the game.
/// </summary>
[Serializable]
public class CompletionData : GameData
{
    public Dictionary<string, bool> IDToCompletion;

    public CompletionData()
    {
        IDToCompletion = new Dictionary<string,bool>();
    }
}
