using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class ModuleManager : MonoBehaviour
{
    [SerializeField] private static ModuleManager _instance;
    [SerializeField] private List<Module> _moduls;
    public List<Module> Moduls 
    { 
        get { return _moduls; } 
    }

    private void Awake()
    {
        if (!ModuleManager.IsLoaded())
        {
            DontDestroyOnLoad(this.gameObject);
        }
        _instance = this;
        _moduls = new List<Module>();
    }

    public static bool RegisterModul<T>(T module) where T : Module
    {
        bool isTypeAlreadyRegistered = _instance.Moduls.Where(mod => mod.GetType().IsEquivalentTo(module.GetType())).Any();
        if (isTypeAlreadyRegistered)
        {
            Debug.LogError("Module of type '" + module.GetType().Name + "' already registered.");
            return false;
        }
        _instance.Moduls.Add(module);
        Debug.Log("Registered module '" + module.GetType().Name + "'");
        return true;
    }

    public static void DeregisterModul<T>(T module) where T : Module
    {
        _instance.Moduls.Remove(module);
        Debug.Log("Deregistered module '" + module.GetType().Name + "'");
    }

    public static bool ModuleRegistered<T>() where T : Module
    {
        return _instance.Moduls.Where(mod => mod.GetType().IsEquivalentTo(typeof(T))).Any();
    }

    public static T GetModule<T>() where T : Module
    {
        var toReturn = _instance.Moduls.Where(modul => modul.GetType() == typeof(T));
        if (!toReturn.Any()) throw new Exception("Module of type " + typeof(T).Name + " not registered!");
        return (T)toReturn.First();
    }

    public static bool IsLoaded()
    {
        return _instance != null;
    }
}
