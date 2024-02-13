using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration
{
    /// <summary>
    /// A singleton which reads and writes all data for the terrain generation.
    /// </summary>
    public class TerrainGenerationIO
    {
        private static TerrainGenerationIO _instance;
        private const string FILENAME = "ProceduralGenerationData.txt";

        private bool _initialized;

        /// <summary>
        /// The singleton instance
        /// </summary>
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

        /// <summary>
        /// Writes the procedural generation data.
        /// </summary>
        /// <param name="data">A dictionary with the names of each generator as keys.</param>
        public void WriteData(Dictionary<string, TerrainGenerationData> data)
        {
            if (!_initialized)
            {
                return;
            }

            ResourcesUtil.WriteFile(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH, FILENAME, data);
        }

        /// <summary>
        /// Reads the procedural generation data. 
        /// </summary>
        /// <returns>A dictionary with the names of each generator as keys.</returns>
        public Dictionary<string, TerrainGenerationData> ReadData()
        {
            if (!_initialized)
            {
                return new Dictionary<string, TerrainGenerationData>();
            }

            try
            {
                Dictionary<string, TerrainGenerationData> data =
                    ResourcesUtil.GetFileData<Dictionary<string, TerrainGenerationData>>(
                        ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH, FILENAME);
                return data ?? new Dictionary<string, TerrainGenerationData>();
            }
            catch
            {
                return new Dictionary<string, TerrainGenerationData>();
            }
        }

        /// <summary>
        /// Initializes the singleton to check if a valid path is set.
        /// </summary>
        /// <returns></returns>
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
