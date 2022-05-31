using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class ModuleManager : MonoBehaviour
{
    private static ModuleManager instance;
    private List<Module> moduls;
    public List<Module> Moduls { get { return moduls; } }

    private void Awake()
    {

        if (!ModuleManager.isLoaded())
        {
            DontDestroyOnLoad(this.gameObject);
        }
        instance = this;
        moduls = new List<Module>();
    }

    public static bool registerModul<T>(T module) where T : Module
    {
        if (instance.Moduls.Where(mod => mod.GetType().IsEquivalentTo(module.GetType())).Any()) return false;
        instance.Moduls.Add(module);
        Debug.Log("Registered module '" + module.GetType().Name + "'");
        return true;
    }

    public static T get<T>() where T : Module
    {
        return (T)instance.Moduls.Where(modul => modul.GetType() == typeof(T)).First();
    }

    public static bool isLoaded()
    {
        return instance != null;
    }
}
