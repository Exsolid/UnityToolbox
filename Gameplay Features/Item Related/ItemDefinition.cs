using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityToolbox.Item.Management;
using System.Linq;
using System.IO;

namespace UnityToolbox.Item { 
    /// <summary>
    /// The base definition for all items.
    /// </summary>
    [Serializable]
    public class ItemDefinition
    {
        [JsonIgnore]
        [NonSerialized]
        public static char DEVIDER = '_';

        public ItemScope Scope;

        public string Name;

        public int MaxStackCount;

        [JsonIgnore]
        [NonSerialized]
        public Texture2D Icon;

        [JsonIgnore]
        [NonSerialized]
        public GameObject Prefab;

        public string PrefabGUID;
        public string PrefabPath;
        public string IconGUID;
        public string IconPath;

        /// <summary>
        /// Loads all data from the resources with the <see cref="PrefabPath"/> & <see cref="IconPath"/>.
        /// </summary>
        public void Deserialize()
        {
            if(IconPath != null && !IconPath.Trim().Equals(""))
            {
                Icon = (Texture2D) Resources.Load(IconPath);
                if (Icon == null)
                {
                    string newPath = Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(IconGUID), null).Split("Resources/").Last();
                    Icon = (Texture2D)Resources.Load(newPath);
                    if(Icon != null)
                    {
                        IconPath = newPath;
                    }
                    else
                    {
                        throw new SystemException("The deserialized " + nameof(ItemDefinition) + " \"" + Name + "\" does not contain an icon. Was it moved or deleted?");
                    }
                }
            }

            if(PrefabPath != null && !PrefabPath.Trim().Equals(""))
            {
                Prefab = (GameObject) Resources.Load(PrefabPath);
                if(Prefab == null)
                {
                    string newPath = Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(PrefabGUID), null).Split("Resources/").Last();
                    Prefab = (GameObject) Resources.Load(newPath);
                    if (Prefab != null)
                    {
                        PrefabPath = newPath;
                    }
                    else
                    {
                        throw new SystemException("The deserialized " + nameof(ItemDefinition) + " \"" + Name + "\" does not contain a prefab. Was it moved or deleted?");
                    }
                }
            }
            else
            {
                throw new SystemException("The deserialized " + nameof(ItemDefinition) + " \"" + Name + "\" does not contain a prefab. It cannot be instantiated this way.");
            }
        }

        /// <summary>
        /// Creates a string based on the defined scope, devider and name of the <see cref="ItemDefinition"/>.
        /// </summary>
        /// <returns>A string representing a unique name.</returns>
        public string GetQualifiedName()
        {
            return Scope.Name + DEVIDER + Name;
        }

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType().Equals(typeof(ItemDefinition)) || obj.GetType().IsSubclassOf(typeof(ItemDefinition)))
            {
                ItemDefinition other = (ItemDefinition)obj;
                return other.GetQualifiedName().Equals(GetQualifiedName());
            }
            else if (obj.GetType().Equals(typeof(string)))
            {
                return GetQualifiedName().Equals(obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return GetQualifiedName().GetHashCode();
        }
    }
}
