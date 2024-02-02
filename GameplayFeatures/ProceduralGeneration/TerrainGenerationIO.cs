using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration
{
    public class TerrainGenerationIO
    {
        private static TerrainGenerationIO _instance;
        private const string FILENAME = "ProceduralGenerationData.txt";

        private string _path;

        private bool _initialized;

        public static TerrainGenerationIO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TerrainGenerationIO();
                }

                return _instance;
            }
        }

        private TerrainGenerationIO()
        {

        }

        public void WriteData(Dictionary<string, TerrainGenerationData> data)
        {
            ResourcesUtil.WriteFile(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH, FILENAME, data);
        }

        public Dictionary<string, TerrainGenerationData> ReadData()
        {
            Dictionary<string, TerrainGenerationData> data =
                ResourcesUtil.GetFileData<Dictionary<string, TerrainGenerationData>>(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH, FILENAME);
            return data ?? new Dictionary<string, TerrainGenerationData>();
        }

        public bool Initialize()
        {
            _initialized = false;

            if (ProjectPrefs.GetString(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH).Equals("")
                || (Application.isEditor && !ResourcesUtil.IsFullPathValid(Application.dataPath + "/" + ProjectPrefs.GetString(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH))))
            {
                return _initialized;
            }
            _initialized = true;
            return _initialized;
        }
    }
}
