using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/// <summary>
/// A manager which can generate unique IDs. Works in editor and runtime.
/// </summary>
public class IDManager: Module
{
    private static HashSet<string> _usedIds;
    private static string _currentSceneName;
    private static char _separator = '/';

    public override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += (sceneOne, sceneTwo) => 
        {
            SceneChanged(sceneTwo); 
        };
    }

    /// <summary>
    /// Is called when the scene changes. See <see cref="IDInitializer"/>.
    /// </summary>
    /// <param name="newScene">The new scene.</param>
    public static void SceneChanged(Scene newScene)
    {
        _currentSceneName = newScene.name;
        if (_usedIds == null)
        {
            _usedIds = new HashSet<string>();
        }
        else
        {
            _usedIds.Clear();
        }

        Saveable[] list = FindObjectsOfType<Saveable>();
        foreach (Saveable saveable in list)
        {
            _usedIds.Add(saveable.ID);
        }
    }

    /// <summary>
    /// Generates a new unique ID.
    /// </summary>
    /// <returns>A unique ID.</returns>
    public static string GetUniqueID()
    {
        if (_usedIds == null)
        {
            _usedIds = new HashSet<string>();
        } 

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

    /// <summary>
    /// Removes an ID from the existing.
    /// </summary>
    /// <param name="ID">An existing ID.</param>
    public static void RemoveID(string ID)
    {
        if (_usedIds == null)
        {
            return;
        }
        _usedIds.Remove(ID);
    }

    /// <summary>
    /// Registeres a given ID. Throws an error if the ID is already known.
    /// </summary>
    /// <param name="ID">The ID to register.</param>
    /// <exception cref="Exception"></exception>
    public static void RegisterID(string ID)
    {
        if (!_usedIds.Add(ID))
        {
            throw new Exception("The ID " + ID + " has already been registered!");
        }
    }

    /// <summary>
    /// Extracts the scene name of the ID.
    /// </summary>
    /// <param name="ID">The ID to read its scene from.</param>
    /// <returns>A scene name.</returns>
    public static string GetSceneNameOfID(string ID)
    {
        string[] split = ID.Split(_separator);
        return split.FirstOrDefault();
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
