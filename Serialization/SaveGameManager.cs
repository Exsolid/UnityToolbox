using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SaveGameManager : Module
{
    [SerializeField] private string _pathToUse;

    private readonly string _dataPath = "/SaveGameData.dat";
    private readonly string _spawnDataPath = "/SaveGameObjects.dat";
    private readonly string _completionDataPath = "/SaveGameCompletion.dat";

    private Dictionary<string,List<GameData>> _data;
    private Dictionary<string, ResourceData> _spawnData;
    private HashSet<string> _spawnedIDs;
    private CompletionData _completionInfo;

    private JsonSerializerSettings _settings;

    public Action<string, bool> OnCompletionInfoChanged;

    public override void Awake()
    {
        base.Awake();
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

        _spawnedIDs = new HashSet<string>();
        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        ReadSpawnData();
        ReadData();
        ReadCompletionData();

        if (_data == null)
        {
            _data = new Dictionary<string, List<GameData>>();
        }
        if(_spawnData == null)
        {
            _spawnData = new Dictionary<string, ResourceData>();
        } 
        if(_completionInfo == null)
        {
            _completionInfo = new CompletionData();
        }

        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        while (SceneManager.GetActiveScene().name.Equals("Master"))
        {
            yield return null;
        }

        LoadAllIntoScene();
        SceneManager.activeSceneChanged += (one, two) => 
        {
            LoadAllIntoScene();
        };

        SceneManager.sceneUnloaded += (scene) =>
        {
            _spawnedIDs.RemoveWhere(id => IDManager.GetSceneNameOfID(id).Equals(scene.name)); ;
        };
    }

    public bool GetCompletionInfo(string ID)
    {
        if (_completionInfo.IDToCompletion.ContainsKey(ID))
        {
            return _completionInfo.IDToCompletion[ID];
        }
        else return false;
    }

    public void SetCompletionInfo(string ID, bool isCompleted)
    {
        if (_completionInfo.IDToCompletion.ContainsKey(ID) && _completionInfo.IDToCompletion[ID] != isCompleted)
        {
            _completionInfo.IDToCompletion[ID] = isCompleted;
            OnCompletionInfoChanged?.Invoke(ID, isCompleted);
        }
        else if (!_completionInfo.IDToCompletion.ContainsKey(ID))
        {
            _completionInfo.IDToCompletion.Add(ID, isCompleted);
            OnCompletionInfoChanged?.Invoke(ID, isCompleted);
        }
    }

    public List<GameData> GetGameDataForID(string ID)
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
            if (!IDManager.GetSceneNameOfID(s).Equals(SceneManager.GetActiveScene().name))
            {
                continue;
            }

            _data.Remove(s);
        }
    }

    private void SpawnSaved()
    {
        foreach (KeyValuePair<string, ResourceData> data in _spawnData)
        {
            if (!IDManager.GetSceneNameOfID(data.Key).Equals(SceneManager.GetActiveScene().name))
            {
                continue;
            }

            try
            {
                GameObject obj = (GameObject)Instantiate(ModuleManager.GetModule<PrefabManager>().GetPrefabForID(data.Value.PrefabID));
                obj.GetComponent<Saveable>().ID = data.Key;
                _spawnedIDs.Add(data.Key);
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
        if (!Directory.Exists(_pathToUse))
        {
            return;
        }

        string dataToJson = JsonConvert.SerializeObject(_data, _settings);
        string spawnDataToJson = JsonConvert.SerializeObject(_spawnData, _settings);
        string completionDataToJson = JsonConvert.SerializeObject(_completionInfo, _settings);
        File.WriteAllText(_pathToUse + _dataPath, dataToJson);
        File.WriteAllText(_pathToUse + _spawnDataPath, spawnDataToJson);
        File.WriteAllText(_pathToUse + _completionDataPath, completionDataToJson);
    }

    private void ReadData()
    {
        if (!Directory.Exists(_pathToUse))
        {
            return;
        }

        if (!File.Exists(_pathToUse + _dataPath))
        {
            return;
        }

        string jsonToData = File.ReadAllText(_pathToUse + _dataPath);
        _data = JsonConvert.DeserializeObject<Dictionary<string, List<GameData>>>(jsonToData, _settings);
    }

    private void ReadSpawnData()
    {
        if (!Directory.Exists(_pathToUse))
        {
            return;
        }

        if (!File.Exists(_pathToUse + _spawnDataPath))
        {
            return;
        }

        string jsonToData = File.ReadAllText(_pathToUse + _spawnDataPath);
        _spawnData = JsonConvert.DeserializeObject<Dictionary<string, ResourceData>>(jsonToData, _settings);
    }

    private void ReadCompletionData()
    {
        if (!Directory.Exists(_pathToUse))
        {
            return;
        }

        if (!File.Exists(_pathToUse + _completionDataPath))
        {
            return;
        }

        string jsonToData = File.ReadAllText(_pathToUse + _completionDataPath);
        _completionInfo = JsonConvert.DeserializeObject<CompletionData>(jsonToData, _settings);
    }

    private void OnDestroy()
    {
        WriteData();
    }

    private byte[] Encrypt(string plainText, byte[] key, byte[] iv)
    {
        //Encoding.ASCII.GetBytes(plainText);
        //TODO checks && max memory
        byte[] cypherText;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    cypherText = ms.ToArray();
                }
            }
        }
        return cypherText;
    }

    private string Decrypt(byte[] cypherText, byte[] key, byte[] iv)
    {
        //TODO checks
        string plainText;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        plainText = sr.ReadToEnd();
                    }
                }
            }
        }
        return plainText;
    }
}
