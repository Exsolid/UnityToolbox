using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefKeys : Module
{
    public readonly string JSON_CONTROLS = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Controls";
    public readonly string MOUSE_SENSITIVITY = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Mouse_Sensitivity";
    public readonly string SOUND_VOLUME = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Sound_Volume";
    public readonly string MUSIC_VOLUME = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Music_Volume";

    public readonly string PREV_SCENE = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Previous_Scene";
    public readonly string GAMEPLAY_STATE = ModuleManager.get<PlayerPreferencesManager>().Keyword + "Gameplay_State";
}
