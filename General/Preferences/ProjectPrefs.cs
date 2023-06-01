using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

/// <summary>
/// Project preferences. Just like the <see cref="PlayerPrefs"/> but within the editor. These are stored within a created resource folder.
/// </summary>
public class ProjectPrefs
{
    private static readonly string _projectSettingsPath = Path.GetFullPath(Application.dataPath + "/Resources/ProjectPrefs/");
    private static readonly string _filename = "ProjectPrefs.txt";
    private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };


    private static void InitializeFolder()
    {
        if (Directory.Exists(_projectSettingsPath))
        {
            return;
        }
        else
        {
            Directory.CreateDirectory(_projectSettingsPath);
        }
    }

    /// <summary>
    /// Saves a string value to the project prefs.
    /// </summary>
    /// <param name="key">The key, which indentifies the value. See <see cref="ProjectPrefKeys"/> for UnityToolbox internal use.</param>
    /// <param name="value">The value to save.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void SetString(string key, string value)
    {
        InitializeFolder();

        Dictionary<string, object> allExisting = new Dictionary<string, object>();
        TextAsset json = Resources.Load<TextAsset>(_projectSettingsPath.Split(Path.GetFullPath(Application.dataPath + "/Resources/")).Last() + _filename.Replace(".txt", "")) as TextAsset;
        if (json != null)
        {
            allExisting = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.text, _settings);
        }

        if (allExisting.ContainsKey(key))
        {
            if (allExisting[key].GetType().Equals(typeof(string)))
            {
                allExisting[key] = value;
            }
            else
            {
                throw new ArgumentException("The " + nameof(ProjectPrefs) + " cannot write a value of the type " + typeof(string) + " to an already existing value of type " + allExisting[key].GetType() + ".");
            }
        }
        else
        {
            allExisting.Add(key, value);
        }

        string toWrite = JsonConvert.SerializeObject(allExisting, _settings);
        File.WriteAllText(_projectSettingsPath + _filename, toWrite);
    }

    /// <summary>
    /// Reads a string value from the project prefs.
    /// </summary>
    /// <param name="key">The key, which indentifies the value. See <see cref="ProjectPrefKeys"/> for UnityToolbox internal use.</param>
    /// <returns>The read value.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetString(string key)
    {
        InitializeFolder();

        Dictionary<string, object> allExisting = new Dictionary<string, object>();
        TextAsset json = Resources.Load<TextAsset>(_projectSettingsPath.Split(Path.GetFullPath(Application.dataPath + "/Resources/")).Last() + _filename.Replace(".txt", "")) as TextAsset;
        if(json != null)
        {
            allExisting = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.text, _settings);
        }

        if (allExisting.ContainsKey(key))
        {
            if (allExisting[key].GetType().Equals(typeof(string)))
            {
                return (string) allExisting[key];
            }
            else
            {
                throw new ArgumentException("The given key returns a value of type " + allExisting[key].GetType() + " and not of type " + typeof(string) + ".");
            }
        }
        else
        {
            return "";
        }
    }

    //TODO bool int float
}
