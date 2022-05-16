using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Reflection;
public class PlayerPrefKeys : Module
{
    [SerializeField] private string keyword = "";

    public static string JSON_CONTROLS = "Controls";
    public static string MOUSE_SENSITIVITY = "Mouse_Sensitivity";
    public static string SOUND_VOLUME = "Sound_Volume";
    public static string MUSIC_VOLUME = "Music_Volume";

    public static string PREV_SCENE = "Previous_Scene";
    public static string GAMEPLAY_STATE = "Gameplay_State";

    public override void Awake()
    {
        base.Awake();
    }

    public string getPrefereceKey(string id)
    {
        return keyword + "_" + this.GetType().GetField(id).GetValue(this);
    }
}
