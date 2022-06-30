using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft;
using System.Linq;

public class SaveGameManager : Module
{
    [SerializeField] private string _pathToUse;
    private Dictionary<string,List<GameData>> _data;
    private Dictionary<string, ResourceData> _spawnData;
    private readonly string _dataPath = "/SaveGame.dat";
    private readonly string _spawnDataPath = "/SaveGameObjects.dat";
    private Newtonsoft.Json.JsonSerializerSettings _settings;

    public void Start()
    {
        if (!Application.isEditor)
        {
            _pathToUse = Application.dataPath + "/SaveGame";
            if(!Directory.Exists(_pathToUse)) Directory.CreateDirectory(_pathToUse);
        }

        if (!Directory.Exists(_pathToUse))
        {
            _data = new Dictionary<string, List<GameData>>();
            _spawnData = new Dictionary<string, ResourceData>();
            return;
        }

        _settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };

        ReadSpawnData();
        ReadData();

        if(_data == null)_data = new Dictionary<string, List<GameData>>();
        if(_spawnData == null) _spawnData = new Dictionary<string, ResourceData>();

        LoadAllIntoScene();
    }

    public List<GameData> getGameDataForID(string ID)
    {
        List<GameData> data = new List<GameData>();
        if (_data.ContainsKey(ID)) data.AddRange(_data[ID]);
        return data;
    }

    public void SetDataToSave(GameData data, string ID, bool writeImmediate)
    {
        if (data.GetType().Equals(typeof(ResourceData)))
        {
            if (_spawnData.ContainsKey(ID)) _spawnData[ID] = (ResourceData)data;
            else _spawnData.Add(ID, (ResourceData)data);
        }
        else
        {
            if (_data.ContainsKey(ID))
            {
                List<GameData> allSaved = _data[ID];
                var contained = allSaved.Where(d => d.GetType().Equals(data.GetType()));
                if (contained.Any())
                {
                    allSaved.Remove(contained.First());
                }
                allSaved.Add(data);
                _data[ID] = allSaved;
            }
            else
            {
                _data.Add(ID, new List<GameData> { data });
            }
        }
        if (writeImmediate) WriteData();
    }

    public void RemoveDataFromSave(string ID)
    {
        _data.Remove(ID);
        _spawnData.Remove(ID);
    }

    public void LoadAllIntoScene()
    {
        SpawnSaved();
        LoadSaved();
    }

    private void LoadSaved()
    {
        List<string> checkDeleted = _data.Keys.ToList();
        foreach(Saveable save in FindObjectsOfType<Saveable>())
        {
            save.Load();
            checkDeleted.Remove(save.ID);
        }
        foreach (string s in checkDeleted)
        {
            if (!IDManager.IsIDInActiveScene(s)) continue;
            _data.Remove(s);
        }
    }

    private void SpawnSaved()
    {
        foreach (KeyValuePair<string, ResourceData> data in _spawnData)
        {
            if (!IDManager.IsIDInActiveScene(data.Key)) continue;
            try
            {
                GameObject obj = (GameObject)Instantiate(Resources.Load(data.Value.Path));
                obj.GetComponent<Saveable>().ID = data.Key;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Has the path of the Item changed?");
            }
        }
    }

    private void WriteData()
    {
        if (!Directory.Exists(_pathToUse)) return;
        string dataToJson = Newtonsoft.Json.JsonConvert.SerializeObject(_data, _settings);
        string spawnDataToJson = Newtonsoft.Json.JsonConvert.SerializeObject(_spawnData, _settings);
        File.WriteAllText(_pathToUse + _dataPath, dataToJson);
        File.WriteAllText(_pathToUse + _spawnDataPath, spawnDataToJson);
    }

    private void ReadData()
    {
        if (!Directory.Exists(_pathToUse)) return;
        if (!File.Exists(_pathToUse + _dataPath)) return;
        string jsonToData = File.ReadAllText(_pathToUse + _dataPath);
        _data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<GameData>>>(jsonToData, _settings);
    }

    private void ReadSpawnData()
    {
        if (!Directory.Exists(_pathToUse)) return;
        if (!File.Exists(_pathToUse + _spawnDataPath)) return;
        string jsonToData = File.ReadAllText(_pathToUse + _spawnDataPath);
        _spawnData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ResourceData>>(jsonToData, _settings);
    }

    private void OnDestroy()
    {
        WriteData();
    }
}
