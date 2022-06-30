using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(PathFetcher))]
public abstract class Saveable : MonoBehaviour
{
    [SerializeField] private string _id = ""; //TODO readonly in editor
    public string ID { get { return _id; } set { IDManager.RegisterID(value); IDManager.RemoveID(_id); _id = value;} }

    [SerializeField] private bool _inEditor; //TODO readonly in editor
    public bool InEditor { get { return _inEditor; } }

    //TODO should save bool to exclude

    [SerializeField] protected string _path; //TODO readonly in editor
    public string Path { get { return _path; } set { _path = value; } }
    private bool subbedEvent;

    protected bool _isRunning;

    private void Awake()
    {
        _isRunning = true;
        if (_id.Equals("") && !_inEditor)
        {
            _id = IDManager.GetUniqueID();
        }
    }

    public void Save()
    {
        if (!InEditor)
        {
            ResourceData objectData = new ResourceData();
            objectData.Path = _path.Split("Resources/").LastOrDefault().Split(".prefab").FirstOrDefault();
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(objectData, ID, false);
            TransformData transformData = new TransformData(transform);
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(transformData, ID, false);
        }

        foreach (GameData gameData in SaveData())
        {
            ModuleManager.GetModule<SaveGameManager>().SetDataToSave(gameData, ID, false);
        }
    }

    public void Load()
    {
        List<GameData> data = ModuleManager.GetModule<SaveGameManager>().getGameDataForID(ID);
        foreach (GameData gameData in data)
        {
            if (gameData.GetType().Equals(typeof(TransformData)))
            {
                TransformData transformData = gameData as TransformData;
                transform.position = new Vector3(transformData.Position.x, transformData.Position.y, transformData.Position.z);
                transform.rotation = Quaternion.Euler(new Vector3(transformData.Rotation.x, transformData.Rotation.y, transformData.Rotation.z));
                transform.localScale = new Vector3(transformData.Scale.x, transformData.Scale.y, transformData.Scale.z);
            }
            LoadData(gameData);
        }
    }

    protected abstract void LoadData(GameData data);
    protected abstract List<GameData> SaveData();
    protected abstract void OnObjectDeleted();

    private void OnValidate()
    {
        if (!subbedEvent)
        {
            GetComponent<PathFetcher>().pathChanged += () => { _path = GetComponent<PathFetcher>().Path; };
            subbedEvent = true;
        }
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
        if (_isRunning)
        {
            ModuleManager.GetModule<SaveGameManager>().RemoveDataFromSave(ID);
            OnObjectDeleted();
        }
    }

    private void OnApplicationQuit()
    {
        _isRunning = false;
        Save();
    }
}
