using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModuleManager : MonoBehaviour
{
    private static ModuleManager instance;
    private List<Module> moduls;
    public List<Module> Moduls { get { return moduls; } }

    private void Awake()
    {
        instance = this;
        moduls = new List<Module>();
    }

    public static bool registerModul<T>(T modul) where T : Module
    {
        if (instance.Moduls.OfType<T>().Any()) return false;
        instance.Moduls.Add(modul);
        return true;
    }

    public static T get<T>() where T : Module
    {
        return (T)instance.Moduls.Where(modul => modul.GetType() == typeof(T)).First();
    }
}
