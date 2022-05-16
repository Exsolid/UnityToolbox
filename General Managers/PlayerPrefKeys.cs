using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefKeys : Module
{
    [SerializeField] private string keyword = "";
    public string Keyword { get { return keyword + "_"; } }

    public readonly string JSON_CONTROLS = "Controls";
    public readonly string MOUSE_SENSITIVITY = "Mouse_Sensitivity";
    public readonly string SOUND_VOLUME = "Sound_Volume";
    public readonly string MUSIC_VOLUME = "Music_Volume";

    public readonly string PREV_SCENE = "Previous_Scene";
    public readonly string GAMEPLAY_STATE = "Gameplay_State";

    public override void Awake()
    {

        base.Awake();
    }
}
