using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbox.GameplayFeatures.Gamestates;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.SaveGame
{
    /// <summary>
    /// A manager, which is takes care of the (de)serialization of <see cref="GameObject"/>s and <see cref="GamestateData"/>.
    /// Requires a <see cref="PrefabManager"/> and <see cref="IDManager"/> for objects that are not present initially.
    /// </summary>
    public class SaveGameManager : Module
    {
        [SerializeField] private string _pathToUse;
        private string _fullPath;

        private readonly string _dataPath = "/SaveGameData.dat";
        private readonly string _spawnDataPath = "/SaveGameObjects.dat";
        private readonly string _gamestateDataPath = "/SaveGameCompletion.dat";

        private Dictionary<string, List<GameData>> _data;
        private Dictionary<string, ResourceData> _spawnData;
        private HashSet<string> _spawnedIDs;
        private HashSet<GamestateNodeData> _activeGamestates;
        /// <summary>
        /// All the active gamestates.
        /// </summary>
        public HashSet<GamestateNodeData> ActiveGamestates
        {
            get
            {
                return _activeGamestates.ToHashSet();
            }
            set
            {
                if(value != null)
                {
                    _activeGamestates = value;
                }
            }
        }
    

        private JsonSerializerSettings _settings;

        public override void Awake()
        {
            base.Awake();
            if (!Application.isEditor)
            {
                _fullPath = Application.dataPath + "/SaveGame";
                if(!Directory.Exists(_pathToUse)) Directory.CreateDirectory(_pathToUse);
            }
            else
            {
                _fullPath = Application.dataPath + _pathToUse;
            }

            if (!Directory.Exists(_fullPath))
            {
                _data = new Dictionary<string, List<GameData>>();
                _spawnData = new Dictionary<string, ResourceData>();


                if (_data == null)
                {
                    _data = new Dictionary<string, List<GameData>>();
                }
                if (_spawnData == null)
                {
                    _spawnData = new Dictionary<string, ResourceData>();
                }
                if (_activeGamestates == null)
                {
                    _activeGamestates = new HashSet<GamestateNodeData>();
                }

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
            if(_activeGamestates == null)
            {
                _activeGamestates = new HashSet<GamestateNodeData>();
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

        /// <summary>
        /// Trys to find <see cref="GameData"/> for an ID.
        /// </summary>
        /// <param name="ID">The ID of the <see cref="Saveable"/>.</param>
        /// <returns></returns>
        public List<GameData> GetGameDataForID(string ID)
        {
            List<GameData> data = new List<GameData>();
            if (_data.ContainsKey(ID)) data.AddRange(_data[ID]);
            return data;
        }

        /// <summary>
        /// Sets data to be seralized.
        /// </summary>
        /// <param name="data">The implementation of <see cref="GameData"/>, which stores the serializable data.</param>
        /// <param name="ID">The ID of the <see cref="Saveable"/>, which this data belongs to.</param>
        /// <param name="writeImmediate">Should the data be serialized immediately? Serializes ALL data.</param>
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

        /// <summary>
        /// Removes an object from the serialisation data.
        /// </summary>
        /// <param name="ID">The ID of the <see cref="Saveable"/> to be removed.</param>
        public void RemoveDataFromSave(string ID)
        {
            _data.Remove(ID);
            _spawnData.Remove(ID);
        }

        private void LoadAllIntoScene()
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
                    GameObject obj = (GameObject) Instantiate(Resources.Load(data.Value.ResourcePath));
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

        /// <summary>
        /// Resets all save game data and permanetly removes every data to be serialized.
        /// </summary>
        public void ResetAll()
        {
            List<Saveable> allSaveable = FindObjectsOfType<Saveable>().ToList();
            foreach (Saveable saveable in allSaveable)
            {
                DestroyImmediate(saveable.gameObject);
            }
            _activeGamestates = new HashSet<GamestateNodeData>();
            _data = new Dictionary<string, List<GameData>>();
            _spawnData = new Dictionary<string, ResourceData>();
        }

        private void WriteData()
        {
            if (!Directory.Exists(_fullPath))
            {
                return;
            }

            string dataToJson = JsonConvert.SerializeObject(_data, _settings);
            string spawnDataToJson = JsonConvert.SerializeObject(_spawnData, _settings);
            string completionDataToJson = JsonConvert.SerializeObject(_activeGamestates, _settings);
            File.WriteAllText(_fullPath + _dataPath, dataToJson);
            File.WriteAllText(_fullPath + _spawnDataPath, spawnDataToJson);
            File.WriteAllText(_fullPath + _gamestateDataPath, completionDataToJson);
        }

        private void ReadData()
        {
            if (!Directory.Exists(_fullPath))
            {
                return;
            }

            if (!File.Exists(_fullPath + _dataPath))
            {
                return;
            }

            string jsonToData = File.ReadAllText(_fullPath + _dataPath);
            _data = JsonConvert.DeserializeObject<Dictionary<string, List<GameData>>>(jsonToData, _settings);
        }

        private void ReadSpawnData()
        {
            if (!Directory.Exists(_fullPath))
            {
                return;
            }

            if (!File.Exists(_fullPath + _spawnDataPath))
            {
                return;
            }

            string jsonToData = File.ReadAllText(_fullPath + _spawnDataPath);
            _spawnData = JsonConvert.DeserializeObject<Dictionary<string, ResourceData>>(jsonToData, _settings);
        }

        private void ReadCompletionData()
        {
            if (!Directory.Exists(_fullPath))
            {
                return;
            }

            if (!File.Exists(_fullPath + _gamestateDataPath))
            {
                return;
            }

            string jsonToData = File.ReadAllText(_fullPath + _gamestateDataPath);
            _activeGamestates = JsonConvert.DeserializeObject<HashSet<GamestateNodeData>>(jsonToData, _settings);
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

        private void OnValidate()
        {
            if (Directory.Exists(_pathToUse) && !Path.GetFullPath(_pathToUse).Contains(Path.GetFullPath(Application.dataPath)) && !_pathToUse.Equals("Use a project path!"))
            {
                _pathToUse = "Use a project path!";
            }
            else if(Directory.Exists(_pathToUse) && Path.GetFullPath(_pathToUse).Contains(Path.GetFullPath(Application.dataPath)))
            {
                _pathToUse = Path.GetFullPath(_pathToUse).Replace(Path.GetFullPath(Application.dataPath), "");
            }
        }
    }
}
