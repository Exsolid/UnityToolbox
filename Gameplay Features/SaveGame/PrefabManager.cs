using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A tool which saves all possible prefabs for the <see cref="SaveGameManager"/>. Required if new objects should be loaded into the scene on start.
/// </summary>
public class PrefabManager : Module
{
    [Header("Register new Prefab")]
    [SerializeField] private Saveable _prefabToAdd;
    [Header("Remove Prefab from register")]
    [SerializeField] private Saveable _prefabToRemove;
    [Header("Status")]
    [ReadOnly] [SerializeField] private string _status;

    [HideInInspector] [SerializeField] private int _nextID;
    [HideInInspector] [SerializeField] private List<GameObject> _prefabs;
    [HideInInspector] [SerializeField] public List<GameObject> ToSerialize;

    /// <summary>
    /// Trys to find a prefab for the given <paramref name="ID"/>.
    /// </summary>
    /// <param name="ID">An ID referencing a prefab.</param>
    /// <returns>A prefab.</returns>
    public GameObject GetPrefabForID(int ID)
    {
        return _prefabs.Where(prefab => prefab.GetComponent<Saveable>().PrefabID.Equals(ID)).FirstOrDefault();
    }

    private void OnValidate()
    {
        if (_nextID.Equals(int.MaxValue))
        {
            _status = "ID limit reached.";
            return;
        }

        if (_prefabToRemove != null)
        {
            _prefabs.Remove(_prefabToRemove.gameObject);
            _prefabToRemove.PrefabID = -1;
            ToSerialize.Add(_prefabToRemove.gameObject);
            _status = "Removed " + _prefabToRemove.gameObject.name;
            _prefabToRemove = null;
        }

        if (_prefabToAdd != null)
        {
            if (!_prefabs.Contains(_prefabToAdd.gameObject))
            {
                _prefabs.Add(_prefabToAdd.gameObject);
                _prefabToAdd.PrefabID = _nextID;
                _nextID++;
                ToSerialize.Add(_prefabToAdd.gameObject);
                _status = "Added " + _prefabToAdd.gameObject.name;
            }
            else
            {
                _status = "Item already registered.";
            }
            _prefabToAdd = null;
        }
    }
}
