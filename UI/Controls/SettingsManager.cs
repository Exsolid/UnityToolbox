using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// This manager controls all relevant information about settings, that is key, volume and sensitivity controls.
/// Requires the <see cref="UIEventManager"/> and <see cref="PlayerPrefKeys"/>.
/// </summary>
public class SettingsManager : Module
{
    [SerializeField] private InputActionAsset _controls;
    [SerializeField] private string _actionMapName;

    private Dictionary<string, string> _initConToKey;
    private Dictionary<string, string> _currentConToKey;

    /// <summary>
    /// The event which is executed as soon as volumes are changed.
    /// </summary>
    public Action<AudioType, float> OnSoundValueChanged;

    /// <summary>
    /// The event which is executed as soon as the mouse sensitivity is changed.
    /// </summary>
    public Action<float> OnSenseValueChanged;

    void Start()
    {
        _initConToKey = new Dictionary<string, string>();
        _currentConToKey = new Dictionary<string, string>();

        foreach (InputAction act in _controls.FindActionMap(_actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                string control = bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower();
                _initConToKey.Add(control, bc.path.ToLower());
                _currentConToKey.Add(control, bc.path.ToLower());
            }
        }
    }

    void OnDestroy()
    {
        WriteControlsToPlayerPrefs();
    }

    /// <summary>
    /// Sets the setting for a key control.
    /// </summary>
    /// <param name="control">The control name, which specifies more information about the action. (e.g "left" or "up")</param>
    /// <param name="path">The path to the new key (e.g."<Keyboard>/W" or "<Mouse>/LeftClick)"</param>
    /// <param name="actionName">The action name (e.g. "Character Jump")</param>
    /// <returns>Returns false if the given key path is already taken.</returns>
    public bool SetKey(string control, string path, string actionName)
    {
        control = StripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        if (_currentConToKey.ContainsValue(path.ToLower())) return false;

        InputAction ac = _controls.FindAction(actionName);
        ac.ChangeBindingWithPath(_currentConToKey[control.ToLower()]).WithPath(path);
        _currentConToKey[control.ToLower()] = path.ToLower();
        return true;
    }

    /// <summary>
    /// Resets the setting for a key control to the last known setting.
    /// </summary>
    /// <param name="control">The control name, which specifies more information about the action. (e.g "left" or "up")</param>
    /// <param name="actionName">The action name (e.g. "Character Jump")</param>
    public void ResetKey(string control, string actionName)
    {
        control = StripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        InputAction ac = _controls.FindAction(actionName);
        ac.ChangeBindingWithPath(_currentConToKey[control]).WithPath(_initConToKey[control]);
        _currentConToKey[control] = _initConToKey[control];
    }

    /// <summary>
    /// Resets the settings of all controls to its last known setting.
    /// </summary>
    public void ResetAllKeys()
    {
        foreach (InputAction act in _controls.FindActionMap(_actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                string control = bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower();
                act.ChangeBindingWithPath(_currentConToKey[control]).WithPath(_initConToKey[control]);
                _currentConToKey[control] = _initConToKey[control];
            }
        }
    }

    /// <summary>
    /// Reads the current value of a given key control.
    /// </summary>
    /// <param name="control">The control name, which specifies more information about the action. (e.g "left" or "up")</param>
    /// <param name="actionName">The action name (e.g. "Character Jump")</param>
    /// <returns>Returns the string path of the current setting.</returns>
    public string CurrentValueOfControl(string control, string actionName)
    {
        control = StripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        return _currentConToKey[control.ToLower()];
    }

    /// <summary>
    /// Writes all data to the <see cref="PlayerPrefs"/>.
    /// </summary>
    private void WriteControlsToPlayerPrefs()
    {
        if (ModuleManager.ModuleRegistered<PlayerPrefKeys>())
        {
            PlayerPrefs.SetString(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.JSON_CONTROLS), _controls.ToJson());
        }
    }

    private string StripToEmpty(string str)
    {
        return str == null ? "" : str;
    }

    //TODO set to uieventmanager

    /// <summary>
    /// Executes the <see cref="SettingsManager.OnSoundValueChanged"/> event.
    /// </summary>
    /// <param name="type">The sound type. (e.g. <see cref="AudioType.Music"/> or <see cref="AudioType.Effects"/>)</param>
    /// <param name="newValue">The new value for the volume, must be between 0-1.</param>
    public void SoundValueChanged(AudioType type, float newValue)
    {
        OnSoundValueChanged?.Invoke(type, newValue);
    }

    /// <summary>
    /// Executes the <see cref="SettingsManager.OnSenseValueChanged"/> event.
    /// </summary>
    /// <param name="newValue">The new value for the mouse sensitivity, must be between 0-1.</param>
    public void SenseValueChanged(float newValue)
    {
        OnSenseValueChanged?.Invoke(newValue);
    }
}