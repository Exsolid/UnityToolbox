using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : Module
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string actionMapName; //Root object in controls
    [SerializeField] private string actionNameMoving; //Child object in controls
    [SerializeField] private string actionNameReturn; //Child object in controls
    [SerializeField] private string actionNameInteract; //Child object in controls


    private Dictionary<string, string> initConToKey;
    private Dictionary<string, string> currentConToKey;

    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey(PlayerPrefKeys.JSON_CONTROLS)) controls.LoadFromJson(PlayerPrefs.GetString(PlayerPrefKeys.JSON_CONTROLS));
        //PlayerPrefs.DeleteAll();
        initConToKey = new Dictionary<string, string>();
        currentConToKey = new Dictionary<string, string>();

        foreach (InputBinding bc in controls.FindActionMap(actionMapName).FindAction(actionNameMoving).bindings)
        {
            initConToKey.Add(bc.name.ToLower(), bc.path.ToLower());
            currentConToKey.Add(bc.name.ToLower(), bc.path.ToLower());
        }
        foreach (InputBinding bc in controls.FindActionMap(actionMapName).FindAction(actionNameReturn).bindings)
        {
            initConToKey.Add(actionNameReturn.ToLower(), bc.path.ToLower());
            currentConToKey.Add(actionNameReturn.ToLower(), bc.path.ToLower());
        }
        foreach (InputBinding bc in controls.FindActionMap(actionMapName).FindAction(actionNameInteract).bindings)
        {
            initConToKey.Add(actionNameInteract.ToLower(), bc.path.ToLower());
            currentConToKey.Add(actionNameInteract.ToLower(), bc.path.ToLower());
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
        InputAction ac = controls.FindAction(actionNameMoving);
        foreach (InputBinding bc in controls.FindActionMap(actionMapName).FindAction(actionNameMoving).bindings)
        {
            if (!bc.name.Equals("Controls"))
            {
                ac.ChangeBindingWithPath(currentConToKey[bc.name.ToLower()]).WithPath(initConToKey[bc.name.ToLower()]);
                currentConToKey[bc.name.ToLower()] = initConToKey[bc.name.ToLower()];
            }
        }
    }

    public string currentValueOfControl(string control, string actionName)
    {
        control = stripToEmpty(control);
        if (control == "") control = actionName;
        return currentConToKey[control.ToLower()];
    }

    private void writeControlsToPlayerPrefs()
    {
        PlayerPrefs.SetString(ModuleManager.get<PlayerPrefKeys>().JSON_CONTROLS, controls.ToJson());
    }

    private string stripToEmpty(string str)
    {
        return str == null ? "" : str;
    }
}
