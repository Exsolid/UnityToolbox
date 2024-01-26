using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityToolbox.General.Management.Editor
{
    public class EditorResourceUtil
    {
        public static GUID GetGUIDOfAsset<T>(T asset) where T : UnityEngine.Object
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (!ResourcesUtil.IsFullPathValid(path))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resource\" directory.");
            }
            return AssetDatabase.GUIDFromAssetPath(path);
        }

        public static string GetAssetPathWithGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!ResourcesUtil.IsFullPathValid(path))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resource\" directory.");
            }
            return path;
        }

        public static string GetAssetPathWithAsset<T>(T asset) where T : UnityEngine.Object
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (!ResourcesUtil.IsFullPathValid(path))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resource\" directory.");
            }
            return path;
        }

        public static T GetAssetWithGUID<T>(string guid) where T : UnityEngine.Object
        {
            string path = GetAssetPathWithGUID(guid);
            T asset = (T)Resources.Load(path);
            return asset;
        }

        public static T GetAssetWithPath<T>(string path) where T : UnityEngine.Object
        {
            if (!ResourcesUtil.IsFullPathValid(path))
            {
                throw new SystemException("The path " + path + " belonging to the object is not a \"Resource\" directory.");
            }
            T asset = (T)Resources.Load(path);
            return asset;
        }
    }
}
