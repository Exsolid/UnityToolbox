using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SettingsManager : Module
{
    [SerializeField] private InputActionAsset _controls;
    [SerializeField] private string _actionMapName;

    private Dictionary<string, string> _initConToKey;
    private Dictionary<string, string> _currentConToKey;

    public Action<SoundType, float> OnSoundValueChanged;
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

    public void ResetKey(string control, string actionName)
    {
        control = StripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        InputAction ac = _controls.FindAction(actionName);
        ac.ChangeBindingWithPath(_currentConToKey[control]).WithPath(_initConToKey[control]);
        _currentConToKey[control] = _initConToKey[control];
    }

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

    public string CurrentValueOfControl(string control, string actionName)
    {
        control = StripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        return _currentConToKey[control.ToLower()];
    }

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

    public void SoundValueChanged(SoundType type, float newValue)
    {
        OnSoundValueChanged?.Invoke(type, newValue);
    }

    public void SenseValueChanged(float newValue)
    {
        OnSenseValueChanged?.Invoke(newValue);
    }
}