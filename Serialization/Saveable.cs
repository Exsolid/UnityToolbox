using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

[DisallowMultipleComponent]
public abstract class Saveable : MonoBehaviour
{
    [SerializeField] [ReadOnly] private string _id = "";
    public string ID { get { return _id; } set { IDManager.RegisterID(value); IDManager.RemoveID(_id); _id = value;} }

    [SerializeField] [ReadOnly] private bool _inEditor;
    public bool InEditor { get { return _inEditor; } }

    [SerializeField] protected bool _removeFromSaveOnDelete;
    //TODO should save bool to exclude

    [SerializeField] [ReadOnly] public int PrefabID = -1;

    private void Awake()
    {
        if (_id.Equals("") && !_inEditor)
        {
            _id = IDManager.GetUniqueID();
        }
    }

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
            ResourceData objectData = new ResourceData();
            objectData.PrefabID = PrefabID;
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(objectData, ID, false);
        }

        TransformData transformData = new TransformData(transform);
        ModuleManager.GetModule<SaveGameManager>().SetDataToSave(transformData, ID, false);

        foreach (GameData gameData in SaveData())
        {
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(gameData, ID, false);
        }
    }

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

    protected abstract void LoadData(GameData data);
    protected abstract List<GameData> SaveData();
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
            Save();
        }
    }
}
