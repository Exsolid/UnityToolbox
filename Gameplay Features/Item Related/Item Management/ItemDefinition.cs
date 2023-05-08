using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The base definition for all items created.
/// </summary>
[Serializable]
public class ItemDefinition
{
    protected ItemScope _itemScope;
    public ItemScope ItemScope
    {
        get { return _itemScope; }
        set { _itemScope = value; }
    }

    protected string _itemName;
    public string ItemName
    {
        get { return _itemName; }
        set { _itemName = value; }
    }

    public bool Stackable;

    [NonSerialized]
    protected Sprite _icon;
    public Sprite Icon
    {
        get { return _icon; }
        set 
        {
            _icon = value;
        }
    }

    [NonSerialized]
    protected GameObject _prefab;
    public GameObject Prefab
    {
        get { return _prefab; }
        set 
        {
            _prefab = value;
        }
    }

    public string PrefabPath;
    public string IconPath;

    /// <summary>
    /// Loads all data from the resources with the <see cref="PrefabPath"/> & <see cref="IconPath"/>.
    /// </summary>
    public void Deserialize()
    {
        if(IconPath != null)
        {
            _icon = (Sprite) Resources.Load(IconPath);
        }

        if(PrefabPath != null)
        {
            _prefab = (GameObject) Resources.Load(PrefabPath);
        }
        else
        {
            Debug.LogError("The deserialized " + nameof(ItemDefinition) + " \"" + _itemName + "\" does not contain a prefab. It cannot be instantiated this way.");
        }
    }

    //private bool IsPrefabValid(GameObject newObj, GameObject oldObj)
    //{
    //    string path = AssetDatabase.GetAssetPath(newObj);
    //    if (!path.Contains("Resources/"))
    //    {
    //        Debug.LogError("The object cannot be set to values external to the resources folder.");
    //        return false;
    //    }

    //    PrefabPath = path;
    //    return true;
    //}

    //private bool IsIconValid(Sprite newObj, Sprite oldObj)
    //{
    //    string path = AssetDatabase.GetAssetPath(newObj);
    //    if (!path.Contains("Resources/"))
    //    {
    //        Debug.LogError("The object cannot be set to values external to the resources folder.");
    //        return false;
    //    }

    //    IconPath = path;
    //    return true;
    //}
}
