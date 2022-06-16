using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : Module
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string actionMapName; //Root object in controls


    private Dictionary<string, string> initConToKey;
    private Dictionary<string, string> currentConToKey;

    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey(PlayerPrefKeys.JSON_CONTROLS)) controls.LoadFromJson(PlayerPrefs.GetString(PlayerPrefKeys.JSON_CONTROLS));
        //PlayerPrefs.DeleteAll();
        initConToKey = new Dictionary<string, string>();
        currentConToKey = new Dictionary<string, string>();
        foreach (InputAction act in controls.FindActionMap(actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                initConToKey.Add(bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower(), bc.path.ToLower());
                currentConToKey.Add(bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower(), bc.path.ToLower());
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
        if (control == "") control = actionName;
        if (currentConToKey.ContainsValue(path.ToLower())) return false;

        InputAction ac = controls.FindAction(actionName);
        ac.ChangeBindingWithPath(currentConToKey[control.ToLower()]).WithPath(path);
        currentConToKey[control.ToLower()] = path.ToLower();
        return true;
    }

    public void resetKey(string control, string actionName)
    {
        control = stripToEmpty(control);
        if (control == "") control = actionName;
        InputAction ac = controls.FindAction(actionName);
        ac.ChangeBindingWithPath(currentConToKey[control.ToLower()]).WithPath(initConToKey[control.ToLower()]);
        currentConToKey[control.ToLower()] = initConToKey[control.ToLower()];
    }

    public void resetAllKeys()
    {
        foreach (InputAction act in controls.FindActionMap(actionMapName).actions)
        {
            foreach (InputBinding bc in act.bindings)
            {
                act.ChangeBindingWithPath(currentConToKey[bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower()]).WithPath(initConToKey[bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower()]);
                currentConToKey[bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower()] = initConToKey[bc.name.Equals("") ? bc.action.ToLower() : bc.name.ToLower()];
                    
            }
        }
        //InputAction ac = controls.FindAction(actionNameMoving);
        //foreach (InputBinding bc in controls.FindActionMap(actionMapName).FindAction(actionNameMoving).bindings)
        //{
        //    if (!bc.name.Equals("Controls"))
        //    {
        //        ac.ChangeBindingWithPath(currentConToKey[bc.name.ToLower()]).WithPath(initConToKey[bc.name.ToLower()]);
        //        currentConToKey[bc.name.ToLower()] = initConToKey[bc.name.ToLower()];
        //    }
        //}
    }

    public string currentValueOfControl(string control, string actionName)
    {
        control = stripToEmpty(control);
        if (control == "") control = actionName;
        return currentConToKey[control.ToLower()];
    }

    private void writeControlsToPlayerPrefs()
    {
        PlayerPrefs.SetString(ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.JSON_CONTROLS), controls.ToJson());
    }

    private string stripToEmpty(string str)
    {
        return str == null ? "" : str;
    }
}
