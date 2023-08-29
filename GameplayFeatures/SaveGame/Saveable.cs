using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.Serialization_Data;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.SaveGame
{
    /// <summary>
    /// The base implementation for all gameobjects that should be saved.
    /// It is able to serialize the transform and parent data with the <see cref="SaveGameManager"/>.
    /// Requires a <see cref="PrefabManager"/> and a<see cref="IDManager"/>, if the objects are not initially found in the scene.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class Saveable : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private string _id = "";
        /// <summary>
        /// A unique ID.
        /// </summary>
        public string ID { get { return _id; } set { IDManager.RegisterID(value); IDManager.RemoveID(_id); _id = value;} }

        [SerializeField] [ReadOnly] private bool _inEditor;
        /// <summary>
        /// Whether the gameobject is found in the scene initally. Or created dynamically.
        /// </summary>
        public bool InEditor { get { return _inEditor; } }

        [SerializeField] protected bool _removeFromSaveOnDelete;
        //TODO should save bool to exclude

        /// <summary>
        /// The date which is used to serialize a reference to the <see cref="Prefab"/>.
        /// </summary>
        [ReadOnly] public ResourceData PrefabData;

        private void Awake()
        {
            if (_id.Equals("") && !_inEditor)
            {
                _id = IDManager.GetUniqueID();
            }
        }

        /// <summary>
        /// Defines all data to be serialized by the <see cref="SaveGameManager"/>.
        /// </summary>
        public void Save()
        {
            if (!InEditor)
            {
                if (gameObject.transform.parent != null)
                {
                    if (gameObject.transform.parent.GetComponent<ParentIdentifier>() == null)
                    {
                        Debug.Log("The object " + gameObject.name + " has a parent without a " + typeof(ParentIdentifier).Name + " and will be save incorrectly!");
                    }
                    else
                    {
                        ParentData parentData = new ParentData();
                        parentData.ParentID = gameObject.transform.parent.GetComponent<ParentIdentifier>().ID;
                        ModuleManager.GetModule<SaveGameManager>().SetDataToSave(parentData, ID, false);
                    }
                }

                if(PrefabData == null)
                {
                    throw new Exception("The game object " + this.name + " cannot be saved, without a prefab set.");
                }

                ModuleManager.GetModule<SaveGameManager>().SetDataToSave(PrefabData, ID, false);
            }

            TransformData transformData = new TransformData(transform);
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(transformData, ID, false);

            foreach (GameData gameData in SaveData())
            {
                ModuleManager.GetModule<SaveGameManager>().SetDataToSave(gameData, ID, false);
            }
        }

        /// <summary>
        /// Loads all data, deserialized by the <see cref="SaveGameManager"/>.
        /// </summary>
        public void Load()
        {
            List<GameData> data = ModuleManager.GetModule<SaveGameManager>().GetGameDataForID(ID);
            foreach (GameData gameData in data)
            {
                if (gameData.GetType().Equals(typeof(TransformData)))
                {
                    TransformData transformData = gameData as TransformData;
                    transform.localPosition = new Vector3(transformData.Position.x, transformData.Position.y, transformData.Position.z);
                    transform.localRotation = Quaternion.Euler(new Vector3(transformData.Rotation.x, transformData.Rotation.y, transformData.Rotation.z));
                    transform.localScale = new Vector3(transformData.Scale.x, transformData.Scale.y, transformData.Scale.z);
                }
                else if(gameData.GetType().Equals(typeof(ParentData)))
                {
                    ParentData parentData = gameData as ParentData;
                    ParentIdentifier parent = FindObjectsOfType<ParentIdentifier>().Where(pI => pI.ID.Equals(parentData.ParentID)).FirstOrDefault();
                    if (parent != null)
                    {
                        transform.SetParent(parent.transform);
                    }
                }
                LoadData(gameData);
            }
        }

        /// <summary>
        /// Is called after the base data is loaded in <see cref="Load"/>. Allows additional data to be deserialized.
        /// </summary>
        /// <param name="data">A <see cref="GameData"/> implementation, which holds data to load.</param>
        protected abstract void LoadData(GameData data);
        /// <summary>
        /// Is called on <see cref="Save"/>. Allows the definition of all additional <see cref="GameData"/> to be serialized.
        /// </summary>
        /// <returns></returns>
        protected abstract List<GameData> SaveData();
        /// <summary>
        /// A method called on deletion, that is that the object is removed from the save game and destroyed.
        /// It is not called when destroyed but saved.
        /// </summary>
        protected abstract void OnObjectDeleted();

        public void OnValidate()
        {
            if (gameObject.scene.name != null && !Application.isPlaying)
            {
                _inEditor = true;
                if(_id.Equals("")) _id = IDManager.GetUniqueID();
            }else if(gameObject.scene.name == null && !Application.isPlaying)
            {
                _inEditor = false;
                if (!_id.Equals("")) _id = "";
            }
        }

        private void OnDestroy()
        {
            //?
            if (_removeFromSaveOnDelete)
            {
                ModuleManager.GetModule<SaveGameManager>().RemoveDataFromSave(ID);
                OnObjectDeleted();
            }
            else
            {
                if (ModuleManager.ModuleRegistered<SaveGameManager>())
                {
                    Save();
                }
            }
        }
    }
}
