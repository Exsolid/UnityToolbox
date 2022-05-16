using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Reflection;
public class PlayerPrefKeys : Module
{
    [SerializeField] private string keyword;

    public static string JSON_CONTROLS = "JSON_CONTROLS";
    public static string MOUSE_SENSITIVITY = "MOUSE_SENSITIVITY";
    public static string SOUND_VOLUME = "SOUND_VOLUME";
    public static string MUSIC_VOLUME = "MUSIC_VOLUME";

    public static string PREV_SCENE = "PREV_SCENE";
    public static string GAMEPLAY_STATE = "GAMEPLAY_STATE";

    public override void Awake()
    {
        base.Awake();
    }

    public string getPrefereceKey(string id)
    {
        return keyword + "_" + this.GetType().GetField(id).GetValue(this);
    }
}
