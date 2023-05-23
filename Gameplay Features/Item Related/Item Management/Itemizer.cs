using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

/// <summary>
/// The core of the item managment system. It takes care of the (de)serialization and changes to all data.
/// </summary>
public class Itemizer
{
    private static Itemizer _instance;
    public static Itemizer Instance
    {
        get 
        {
            if(_instance == null)
            {
                _instance = new Itemizer();
            }
            return _instance;
        }
    }

    private bool _initialized;
    public bool Initialized
    {
        get { return _initialized; }
    }

    private string FILENAMEITEMS = "ItemData.txt";
    private string FILENAMESCOPES = "ItemScopeData.txt";

    public event Action<ItemScope, ItemScope> OnItemScopeEdited;
    public event Action<ItemDefinition, ItemDefinition> OnItemDefinitionEdited;

    private HashSet<ItemDefinition> _itemDefinitions;
    /// <summary>
    /// All defined items.
    /// </summary>
    public HashSet<ItemDefinition> ItemDefinitions
    {
        get
        {
            return _itemDefinitions.ToHashSet();
        }
    }

    private HashSet<ItemScope> _itemScopes;
    /// <summary>
    /// All defined item scopes.
    /// </summary>
    public HashSet<ItemScope> ItemScopes
    {
        get { return _itemScopes.ToHashSet(); }
    }

    private ItemScope _defaultScope;
    public ItemScope DefaultScope
    {
        get { return _defaultScope; }
    }

    public void Initialize()
    {
        _initialized = false;

        if (ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH).Equals(""))
        {
            return;
        }

        ReadDataLocally();

        if(_itemDefinitions == null)
        {
            _itemDefinitions = new HashSet<ItemDefinition>();
            _itemScopes = new HashSet<ItemScope>();
        }

        _defaultScope = new ItemScope();
        _defaultScope.Name = "DefaultScope";
        _itemScopes.Add(_defaultScope);

        _initialized = true;
    }

    public ItemDefinition AddItemDefinition(ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath)
    {
        CheckInitialized();

        if (name == null || name.Equals(""))
        {
            throw new ArgumentException("The name cannot be undefined.");
        }
        resourcePrefabPath = resourcePrefabPath.Split(".").First();
        resourceIconPath = resourceIconPath.Split(".").First();

        if (Resources.Load(resourcePrefabPath) == null)
        {
            throw new ArgumentException("The prefab of this item must be defined from a valid \"Resources\" directory.");
        }

        if (!_itemScopes.Contains(scope))
        {
            throw new ArgumentException("The given scope is not defined within the manager.");
        }

        ItemDefinition itemDefinition = new ItemDefinition()
        {
            Name = name,
            Scope = scope,
            MaxStackCount = maxStackCount,
            IconPath = resourceIconPath,
            PrefabPath = resourcePrefabPath
        };

        if (!_itemDefinitions.Add(itemDefinition))
        {
            throw new ArgumentException("\"" + itemDefinition.GetQualifiedName() + "\" is not unique.");
        }

        return itemDefinition;
    }

    public T AddInheritedItemDefinition<T>(ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath, HashSet<ItemField> itemFields) where T : ItemDefinition
    {
        CheckInitialized();

        if (name == null || name.Equals(""))
        {
            throw new ArgumentException("The name cannot be undefined.");
        }
        resourcePrefabPath = resourcePrefabPath.Split(".").First();
        resourceIconPath = resourceIconPath.Split(".").First();

        if (Resources.Load(resourcePrefabPath) == null)
        {
            throw new ArgumentException("The prefab of this item must be defined from a valid \"Resources\" directory.");
        }

        if (!_itemScopes.Contains(scope))
        {
            throw new ArgumentException("The given scope is not defined within the manager.");
        }

        FieldInfo[] allTFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        T itemDefinition = (T)Activator.CreateInstance(typeof(T));
        itemDefinition.Name = name;
        itemDefinition.Scope = scope;
        itemDefinition.MaxStackCount = maxStackCount;
        itemDefinition.IconPath = resourceIconPath;
        itemDefinition.PrefabPath = resourcePrefabPath;

        foreach (ItemField field in itemFields)
        {
            FieldInfo referenceField = allTFields.Where(f => f.Name.Equals(field.FieldName) && f.FieldType.Equals(field.FieldType)).FirstOrDefault();
            if (referenceField == null)
            {
                throw new ArgumentException("The type " + typeof(T).Name + " does not contain the field " + field.FieldName + " of type " + field.FieldType.Name + ".");
            }
            else
            {
                referenceField.SetValue(itemDefinition, field.GetValue());
            }
        }

        if (!_itemDefinitions.Add(itemDefinition))
        {
            throw new ArgumentException("\"" + itemDefinition.GetQualifiedName() + "\" is not unique.");
        }

        return itemDefinition;
    }

    public void RemoveItemDefinition(ItemDefinition itemDefinition)
    {
        CheckInitialized();

        if (!_itemDefinitions.Remove(itemDefinition))
        {
            throw new ArgumentException("\"" + itemDefinition.GetQualifiedName() + "\" does not exist.");
        }

        ItemDefinitionEdited(itemDefinition, null);
    }

    private void InternalRemoveItemDefinition(ItemDefinition itemDefinition)
    {
        CheckInitialized();

        if (!_itemDefinitions.Remove(itemDefinition))
        {
            throw new ArgumentException("\"" + itemDefinition.GetQualifiedName() + "\" does not exist.");
        }
    }

    public void EditInheritedItemDefinition<T>(ItemDefinition itemDefinition, ItemScope scope, string name, int maxStackCount, string iconPath, string prefabPath, HashSet<ItemField> itemFields) where T : ItemDefinition
    {
        InternalRemoveItemDefinition(itemDefinition);
        T newItemDefinition;

        try
        {
            newItemDefinition = AddInheritedItemDefinition<T>(scope, name, maxStackCount, iconPath, prefabPath, itemFields);
        }
        catch (Exception ex)
        {
            _itemDefinitions.Add(itemDefinition);
            throw ex;
        }

        ItemDefinitionEdited(itemDefinition, newItemDefinition);
    }

    public void EditItemDefinition(ItemDefinition itemDefinition, ItemScope scope, string name, int maxStackCount, string iconPath, string prefabPath)
    {
        InternalRemoveItemDefinition(itemDefinition);
        ItemDefinition newItemDefinition;

        try
        {
            newItemDefinition = AddItemDefinition(scope, name, maxStackCount, iconPath, prefabPath);
        }
        catch (Exception ex)
        {
            _itemDefinitions.Add(itemDefinition);
            throw ex;
        }

        ItemDefinitionEdited(itemDefinition, newItemDefinition);
    }

    public ItemScope AddItemScope(string name)
    {
        CheckInitialized();

        ItemScope scope = new ItemScope()
        {
            Name = name
        };

        if (name == null || name.Equals(""))
        {
            throw new ArgumentException("The name cannot be undefined.");
        }

        if (!_itemScopes.Add(scope))
        {
            throw new ArgumentException("\"" + name + "\" is not unique.");
        }

        return scope;
    }

    public void RemoveItemScope(ItemScope scope)
    {
        CheckInitialized();

        if (_defaultScope.Equals(scope))
        {
            throw new ArgumentException("Cannot make changes to the default scope.");
        }

        List<ItemDefinition> definitionsToChange = _itemDefinitions.Where(def => def.Scope.Equals(scope)).ToList();
        List<ItemDefinition> definitionsToRedo = new List<ItemDefinition>();

        try
        {
            foreach (ItemDefinition def in definitionsToChange)
            {
                definitionsToRedo.Add(AddItemDefinition(_defaultScope, def.Name, def.MaxStackCount, def.IconPath, def.PrefabPath));
            }

            foreach (ItemDefinition def in definitionsToChange)
            {
                InternalRemoveItemDefinition(def);
            }
        }
        catch (Exception ex)
        {
            foreach (ItemDefinition def in definitionsToRedo)
            {
                InternalRemoveItemDefinition(def);
            }
            throw ex;
        }

        if (!_itemScopes.Remove(scope))
        {
            throw new ArgumentException("\"" + scope.Name + "\" does not exist.");
        }

        ItemScopeEdited(scope, null);
    }

    public void EditItemScope(ItemScope scope, string name)
    {
        CheckInitialized();

        if (_defaultScope.Equals(scope))
        {
            throw new ArgumentException("Cannot make changes to the default scope.");
        }

        ItemScope newScope = AddItemScope(name);

        if (!_itemScopes.Remove(scope))
        {
            _itemScopes.Remove(newScope);
            throw new ArgumentException("\"" + scope.Name + "\" does not exist.");
        }

        List<ItemDefinition> definitionsToChange = _itemDefinitions.Where(def => def.Scope.Equals(scope)).ToList();
        List<ItemDefinition> definitionsToRedo = new List<ItemDefinition>();

        try
        {
            foreach (ItemDefinition def in definitionsToChange)
            {
                definitionsToRedo.Add(AddItemDefinition(newScope, def.Name, def.MaxStackCount, def.IconPath, def.PrefabPath));
            }

            foreach (ItemDefinition def in definitionsToChange)
            {
                InternalRemoveItemDefinition(def);
            }
        }
        catch (Exception ex)
        {
            foreach (ItemDefinition def in definitionsToRedo)
            {
                InternalRemoveItemDefinition(def);
            }
            throw ex;
        }

        ItemScopeEdited(scope, newScope);
    }

    private void CheckInitialized()
    {
        if (!_initialized)
        {
            Initialize();
            if (!_initialized)
            {
                throw new SystemException("The " + nameof(Itemizer) + " cannot be initialized. Is the path set up?");
            }
        }
    }

    private void ReadDataLocally()
    {
        _itemDefinitions = _instance.ReadItemData();
        _itemScopes = _instance.ReadItemScopes();
    }

    private HashSet<ItemDefinition> ReadItemData()
    {
        HashSet<ItemDefinition> readItemData = ResourcesUtil.GetFileData<HashSet<ItemDefinition>> (ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMEITEMS);

        if (readItemData != null)
        {
            foreach (ItemDefinition def in readItemData)
            {
                def.Deserialize();
            }
        }

        return readItemData;
    }

    private HashSet<ItemScope> ReadItemScopes()
    {
        HashSet<ItemScope> readScopes = ResourcesUtil.GetFileData<HashSet<ItemScope>>(ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMESCOPES);
        return readScopes;
    }

    public List<Type> GetAllInheritedItemDefinitionTypes()
    {
        List<Type> result = new List<Type>();

        foreach (Type type in
        Assembly.GetAssembly(typeof(ItemDefinition)).GetTypes()
        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ItemDefinition))))
        {
            result.Add(type);
        }

        return result;
    }

    /// <summary>
    /// Writes all data to the set resources folder.
    /// </summary>
    public void WriteData()
    {
        ResourcesUtil.WriteFile(ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMEITEMS, _itemDefinitions);
        ResourcesUtil.WriteFile(ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMESCOPES, _itemScopes);
    }

    public void ItemScopeEdited(ItemScope oldScope, ItemScope newScope)
    {
        OnItemScopeEdited?.Invoke(oldScope, newScope);
    }

    public void ItemDefinitionEdited(ItemDefinition oldItemDefinition, ItemDefinition newItemDefinition)
    {
        OnItemDefinitionEdited?.Invoke(oldItemDefinition, newItemDefinition);
    }
}
