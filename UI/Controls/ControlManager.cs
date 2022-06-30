using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ControlManager : Module
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string actionMapName;

    private Dictionary<string, string> initConToKey;
    private Dictionary<string, string> currentConToKey;

    public Action<string, float> valueChanged;

    void Start()
    {
        initConToKey = new Dictionary<string, string>();
        currentConToKey = new Dictionary<string, string>();

        foreach (InputAction act in controls.FindActionMap(actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                string control = bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower();
                initConToKey.Add(control, bc.path.ToLower());
                currentConToKey.Add(control, bc.path.ToLower());
            }
        }
    }

    void OnDestroy()
    {
        writeControlsToPlayerPrefs();
    }

    
    public bool setKey(string control, string path, string actionName)
    {
        control = stripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        if (currentConToKey.ContainsValue(path.ToLower())) return false;

        InputAction ac = controls.FindAction(actionName);
        ac.ChangeBindingWithPath(currentConToKey[control.ToLower()]).WithPath(path);
        currentConToKey[control.ToLower()] = path.ToLower();
        return true;
    }

    public void resetKey(string control, string actionName)
    {
        control = stripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        InputAction ac = controls.FindAction(actionName);
        ac.ChangeBindingWithPath(currentConToKey[control]).WithPath(initConToKey[control]);
        currentConToKey[control] = initConToKey[control];
    }

    public void resetAllKeys()
    {
        foreach (InputAction act in controls.FindActionMap(actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                string control = bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower();
                act.ChangeBindingWithPath(currentConToKey[control]).WithPath(initConToKey[control]);
                currentConToKey[control] = initConToKey[control];
            }
        }
    }

    public string currentValueOfControl(string control, string actionName)
    {
        control = stripToEmpty(control);
        control = control == "" ? actionName.ToLower() : control.ToLower();
        return currentConToKey[control.ToLower()];
    }

    private void writeControlsToPlayerPrefs()
    {
        PlayerPrefs.SetString(ModuleManager.GetModule<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.JSON_CONTROLS), controls.ToJson());
    }

    private string stripToEmpty(string str)
    {
        return str == null ? "" : str;
    }

    public void ValueChanged(string id, float newValue)
    {
        if (valueChanged != null)
        {
            valueChanged(id, newValue);
        }
    }
}
