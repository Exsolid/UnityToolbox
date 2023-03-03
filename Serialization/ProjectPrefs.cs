using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ProjectPrefs
{
    private static readonly string _projectSettingsPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/ProjectPrefs.dat"));
    private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

    public static void SetString(string key, string value)
    {
        Dictionary<string, object> allExisting = new Dictionary<string, object>();
        if (File.Exists(_projectSettingsPath))
        {
            string json = File.ReadAllText(_projectSettingsPath);
            allExisting = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _settings);
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
        File.WriteAllText(_projectSettingsPath, toWrite);
    }

    public static string GetString(string key)
    {
        Dictionary<string, object> allExisting = new Dictionary<string, object>();
        if (File.Exists(_projectSettingsPath))
        {
            string json = File.ReadAllText(_projectSettingsPath);
            allExisting = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _settings);
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
