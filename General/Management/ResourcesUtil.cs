using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.General.Management
{
    /// <summary>
    /// A utility which writes and reads data with correctly formatted paths.
    /// </summary>
    public class ResourcesUtil
    {
        /// <summary>
        /// Checks whether the path is valid and defined with a "Resources" folder.
        /// </summary>
        /// <param name="path">The local path.</param>
        /// <returns>Whether a path is a valid resources path.</returns>
        public static bool IsFullPathValid(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                return false;
            }

            if (!path.Contains("Resources"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to define a path within the <see cref="ProjectPrefs"/>. The <see cref="Application.dataPath"/> will be removed from it.
        /// </summary>
        /// <param name="path">The local path.</param>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <returns>Whether the path could be set.</returns>
        public static bool TrySetValidPath(string fullPath, string key)
        {
            if (!IsFullPathValid(fullPath))
            {
                return false;
            }

            string pathToSet = Path.GetFullPath(fullPath + "/").Replace(Path.GetFullPath(Application.dataPath), "");

            ProjectPrefs.SetString(key, pathToSet);

            return true;
        }

        /// <summary>
        /// Builds the local path from the stored path within the <see cref="ProjectPrefs"/>.
        /// </summary>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <returns>The local path.</returns>
        public static string GetLocalPath(string key)
        {
            string path = ProjectPrefs.GetString(key);

            if (path == null)
            {
                return null;
            }

            if (!path.Contains("Resources"))
            {
                throw new SystemException("The path " + Application.dataPath + "/" + path + "/" + " is not within a valid Resources directory.");
            }

            path = Path.GetFullPath(Application.dataPath + "/" + path + "/");
            return path;
        }

        /// <summary>
        /// Builds the "Resources" path from the stored path within the <see cref="ProjectPrefs"/>.
        /// </summary>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <returns>The resources path.</returns>
        public static string GetResourcesPath(string key)
        {
            string path = ProjectPrefs.GetString(key);

            if(path == null)
            {
                return null;
            }

            path = Path.GetFullPath(Application.dataPath + "/" + path + "/").Split("Resources\\").Last();
            return path;
        }

        /// <summary>
        /// Builds the project path from the stored path within the <see cref="ProjectPrefs"/>.
        /// </summary>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <returns>A project directory.</returns>
        public static string GetProjectPath(string key)
        {
            return ProjectPrefs.GetString(key);
        }

        /// <summary>
        /// Reads a <see cref="TextAsset"/> with a key for the <see cref="ProjectPrefs"/> and the filename.
        /// </summary>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The cast values of the file.</returns>
        public static T GetFileData<T>(string key, string filename)
        {
            String path = GetLocalPath(key) + filename;
            path.Replace("\\", "/");

            if (!File.Exists(path) && Application.isEditor)
            {
                return default(T);
            }
            else if(!File.Exists(path))
            {
                throw new FileNotFoundException("The file: " + GetResourcesPath(key) + filename.Replace(".txt", "") + " cannot be found.");
            }

            TextAsset textAsset = Resources.Load(path.Replace(".txt", "")) as TextAsset;
            JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        
            return JsonConvert.DeserializeObject<T>(textAsset.text, _settings);
        }

        /// <summary>
        /// Writes an object to a file and refreshes the <see cref="AssetDatabase"/>.
        /// </summary>
        /// <param name="key">The key for the <see cref="ProjectPrefs"/>.</param>
        /// <param name="filename">The filename with extension.</param>
        /// <param name="fileData">The object to write.</param>
        public static void WriteFile(string key, string filename, object fileData)
        {
            if (ProjectPrefs.GetString(key) == null)
            {
                return;
            }

            JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string data = JsonConvert.SerializeObject(fileData, _settings);
            File.WriteAllText(GetLocalPath(key) + filename, data);
        }
    }
}
