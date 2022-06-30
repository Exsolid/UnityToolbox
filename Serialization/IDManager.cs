using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class IDManager: Module
{
    private static HashSet<string> _usedIds;  //TODO readonly in editor
    private static string _currentSceneName;
    private static char _separator = '/';

    public override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += (sceneOne, sceneTwo) => { SceneChanged(sceneTwo); };
    }

    public static void SceneChanged(Scene newScene)
    {
        _currentSceneName = newScene.name;
        if (_usedIds == null) _usedIds = new HashSet<string>();
        else _usedIds.Clear();
        Saveable[] list = FindObjectsOfType<Saveable>();
        foreach (Saveable saveable in list)
        {
            _usedIds.Add(saveable.ID);
        }
    }

    public static string GetUniqueID()
    {
        if (_usedIds == null) _usedIds = new HashSet<string>();
        System.Random rand = new System.Random();
        int min;
        int max;
        if (Application.isPlaying)
        {
            min = 1000001;
            max = Int32.MaxValue;
        }
        else
        {
            min = 0;
            max = 1000000;
        }
        string newID = _currentSceneName + _separator + rand.Next(min, max);
        while (_usedIds.Contains(newID))
        {
            newID = _currentSceneName + _separator + rand.Next(min, max);
        }
        _usedIds.Add(newID);
        return newID;
    }

    public static void RemoveID(string ID)
    {
        if (_usedIds == null) return;
        _usedIds.Remove(ID);
    }

    public static void RegisterID(string ID)
    {
        if (!_usedIds.Add(ID)) throw new Exception("The ID " + ID + " has already been registered!");
    }

    public static bool IsIDInActiveScene(string ID)
    {
        string[] split = ID.Split(_separator);
        return split.FirstOrDefault().Equals(_currentSceneName);
    } 

    private static void logIDs()
    {
        Debug.Log("-----------------------------");
        foreach (string i in _usedIds)
        {
            Debug.Log(i);
        }
    }
}
