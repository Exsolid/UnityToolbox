using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityToolbox.Item.Management;

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

        public string PrefabPath;
        public string IconPath;

        /// <summary>
        /// Loads all data from the resources with the <see cref="PrefabPath"/> & <see cref="IconPath"/>.
        /// </summary>
        public void Deserialize()
        {
            if(IconPath != null)
            {
                Icon = (Texture2D) Resources.Load(IconPath);
            }

            if(PrefabPath != null)
            {
                Prefab = (GameObject) Resources.Load(PrefabPath);
            }
            else
            {
                Debug.LogError("The deserialized " + nameof(ItemDefinition) + " \"" + Name + "\" does not contain a prefab. It cannot be instantiated this way.");
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
