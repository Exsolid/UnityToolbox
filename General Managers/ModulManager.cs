using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModulManager : MonoBehaviour
{
    private static ModulManager instance;
    private List<Modul> moduls;
    public List<Modul> Moduls { get { return moduls; } }

    private void Awake()
    {
        instance = this;
        moduls = new List<Modul>();
    }

    public static bool registerModul<T>(T modul) where T : Modul
    {
        if (instance.Moduls.OfType<T>().Any()) return false;
        instance.Moduls.Add(modul);
        return true;
    }

    public static T get<T>() where T : Modul
    {
        return (T)instance.Moduls.Where(modul => modul.GetType() == typeof(T)).First();
    }
}
