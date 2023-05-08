using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System;

public class ItemManager : Module
{
    private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

    private static string FILENAMEITEMS = "ItemData.txt";
    private static string FILENAMESCOPES = "ItemScopeData.txt";

    public static event Action<ItemScope> OnScopeEdited;

    private Dictionary<string, ItemDefinition> _allItems;
    /// <summary>
    /// All defined items.
    /// </summary>
    public Dictionary<string, ItemDefinition> AllItems
    {
        get 
        { 
            return _allItems.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }

    private HashSet<string> _itemScopes;
    //All defined item scopes.
    public HashSet<string> ItemScopes
    {
        get { return _itemScopes.ToHashSet(); }
    }

    public override void Awake()
    {
        base.Awake();
        ReadDataLocally();
    }

    private void ReadDataLocally()
    {
        _allItems = ItemManager.ReadItemData();
        _itemScopes = ItemManager.ReadItemScopes();
    }

    /// <summary>
    /// Creates a new <see cref="ItemInstance"/> into the scene.
    /// </summary>
    /// <param name="position">The position of the new object.</param>
    /// <param name="rotation">The rotation of the new object.</param>
    /// <param name="scale">The scale of the new object.</param>
    /// <param name="itemDefKey">The key which indentifies the <see cref="ItemDefinition"/>.</param>
    /// <returns>A new <see cref="ItemInstance"/>.</returns>
    public ItemInstance SpawnItemInstance(Vector3 position, Quaternion rotation, Vector3 scale, string itemDefKey)
    {
        if (!_allItems.ContainsKey(itemDefKey))
        {
            return null;
        }

        GameObject prefabInstance = Instantiate(_allItems[itemDefKey].Prefab, position, rotation);
        prefabInstance.transform.localScale = new Vector3(scale.x, scale.y, scale.z);

        ItemInstance itemInstance = prefabInstance.AddComponent<ItemInstance>();
        itemInstance.ItemName = _allItems[itemDefKey].ItemName;
        itemInstance.PrefabData = new ResourceData()
        {
            ResourcePath = _allItems[itemDefKey].PrefabPath,
        };
        itemInstance.Icon = _allItems[itemDefKey].Icon;
        itemInstance.IconPath = _allItems[itemDefKey].IconPath;
        itemInstance.Stackable = _allItems[itemDefKey].Stackable;

        return itemInstance;
    }

    /// <summary>
    /// Reads all data from the set resources folder.
    /// </summary>
    /// <returns>Returns all defined items, with the key being the item name</returns>
    public static Dictionary<string, ItemDefinition> ReadItemData()
    {
        Dictionary<string, ItemDefinition> readItemData = new Dictionary<string, ItemDefinition>();
        string path = ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH);

        if (path != null && path.Contains("Resources/"))
        {
            TextAsset text = Resources.Load<TextAsset>(path.Split("Resources/").Last() + "/" + FILENAMEITEMS);
            readItemData = JsonConvert.DeserializeObject<Dictionary<string, ItemDefinition>>(text.text, _settings);

            foreach(ItemDefinition def in readItemData.Values)
            {
                def.Deserialize();
            }
        }

        return readItemData;
    }

    /// <summary>
    /// Reads all scopes from the set resources folder.
    /// </summary>
    /// <returns>Returns all defined items, with the key being the item name</returns>
    public static HashSet<string> ReadItemScopes()
    {
        HashSet<string> readScopes = new HashSet<string>();
        string path = ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH);

        if (path != null && path.Contains("Resources/"))
        {
            TextAsset text = Resources.Load<TextAsset>(path.Split("Resources/").Last() + "/" + FILENAMESCOPES);
            readScopes = JsonConvert.DeserializeObject<HashSet<string>>(text.text, _settings);
        }

        return readScopes;
    }

    /// <summary>
    /// Writes all data to the set resources folder.
    /// </summary>
    /// <param name="allItems">All items to serialize.</param>
    public static void WriteData(Dictionary<string, ItemDefinition> allItems, HashSet<string> scopes)
    {
        string path = ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH);

        if(path != null && path.Contains("Resources/"))
        {
            if (Directory.Exists(Application.dataPath + "/" + path))
            {
                string toWrite = JsonConvert.SerializeObject(allItems, _settings);
                File.WriteAllText(Application.dataPath + "/" + path + "/" + FILENAMEITEMS, toWrite);

                toWrite = JsonConvert.SerializeObject(scopes, _settings);
                File.WriteAllText(Application.dataPath + "/" + path + "/" + FILENAMESCOPES, toWrite);
            }
        }
    }

    public static void ScopeEdited(ItemScope scope)
    {
        OnScopeEdited?.Invoke(scope);
    }
}
