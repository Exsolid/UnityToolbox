using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityToolbox.General.Management.Editor
{
    public class EditorResourceUtil
    {
        public static GUID GetGUIDOfAsset<T>(T asset) where T : UnityEngine.Object
        {
            string path = AssetDatabase.GetAssetPath(asset);

            if (!path.Contains("Resources"))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resources\" directory.");
            }
            return AssetDatabase.GUIDFromAssetPath(path);
        }

        public static string GetResourcesPathWithGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.Contains("Resources"))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resources\" directory.");
            }
            return path.Split("Resources/").Last().Split(".").First();
        }

        public static string GetResourcesPathWithAsset<T>(T asset) where T : UnityEngine.Object
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (!path.Contains("Resources"))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resources\" directory.");
            }
            return path.Split("Resources/").Last().Split(".").First();
        }

        public static T GetAssetWithGUID<T>(string guid) where T : UnityEngine.Object
        {
            string path = GetResourcesPathWithGUID(guid);
            T asset = (T)Resources.Load( path);
            return asset;
        }

        public static T GetAssetWithResourcesPath<T>(string path) where T : UnityEngine.Object
        {
            T asset = (T)Resources.Load(path);
            return asset;
        }

        public static bool IsAssetValid<T>(T asset) where T : UnityEngine.Object
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return path.Contains("Resources/");
        }
    }
}
