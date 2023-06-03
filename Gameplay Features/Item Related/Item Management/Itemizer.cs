using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

namespace UnityToolbox.Item.Management
{
    /// <summary>
    /// The core of the item managment system. It takes care of the (de)serialization and changes to all data.
    /// </summary>
    public class Itemizer
    {
        private static Itemizer _instance;
        /// <summary>
        /// The singleton instance of the Itemizer.
        /// </summary>
        public static Itemizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Itemizer();
                }
                return _instance;
            }
        }

        private bool _initialized;
        /// <summary>
        /// Is the itemizer initialized? If not, it cannot be used.
        /// </summary>
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
                if (_initialized)
                {
                    return _itemDefinitions.ToHashSet();
                }
                else
                {
                    return new HashSet<ItemDefinition>();
                }
            }
        }

        private HashSet<ItemDefinition> _faultyItemDefinitions;
        /// <summary>
        /// All defined items which are faulty.
        /// </summary>
        public HashSet<ItemDefinition> FaultyItemDefinitions
        {
            get
            {
                if (_initialized)
                {
                    return _faultyItemDefinitions.ToHashSet();
                }
                else
                {
                    return new HashSet<ItemDefinition>();
                }
            }
        }

        private HashSet<ItemScope> _itemScopes;
        /// <summary>
        /// All defined item scopes.
        /// </summary>
        public HashSet<ItemScope> ItemScopes
        {
            get 
            {
                if (_initialized)
                {
                    return _itemScopes.ToHashSet();
                }
                else
                {
                    return new HashSet<ItemScope>();
                }
            }
        }

        private ItemScope _defaultScope;
        /// <summary>
        /// The defaulte scope, set by default and is the replacement for deleted scopes.
        /// </summary>
        public ItemScope DefaultScope
        {
            get { return _defaultScope; }
        }

        /// <summary>
        /// Usage forbidden as the <see cref="Itemizer.Instance"/> should be used.
        /// </summary>
        private Itemizer()
        {
        }

        /// <summary>
        /// Tries to initialize the <see cref="Itemizer"/>.
        /// </summary>
        /// <returns>Whether initializing was successful.</returns>
        public bool Initialize()
        {
            _initialized = false;

            if (ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH).Equals(""))
            {
                return _initialized;
            }

            ReadData();

            if (_itemDefinitions == null)
            {
                _itemDefinitions = new HashSet<ItemDefinition>();
                _itemScopes = new HashSet<ItemScope>();
            }

            _defaultScope = new ItemScope();
            _defaultScope.Name = "DefaultScope";
            _itemScopes.Add(_defaultScope);

            _initialized = true;
            return _initialized;
        }

        /// <summary>
        /// Creates a new item definition and adds it to the mangement.
        /// </summary>
        /// <param name="scope">The scope of the definition."/></param>
        /// <param name="name">The name of the definition. Has to be unique in combination with the scope.</param>
        /// <param name="maxStackCount">How high this item can be stacked.</param>
        /// <param name="resourceIconPath">The path to where the icon can be found. Has to be a resources path.</param>
        /// <param name="resourcePrefabPath">The path to where the prefab can be found. Has to be a resources path.</param>
        /// <returns>The created <see cref="ItemDefinition"/>.</returns>
        /// <exception cref="ItemDefinitionException"></exception>
        public ItemDefinition AddItemDefinition(ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath, string iconGUID, string prefabGUID)
        {
            CheckInitialized();

            if (name == null || name.Equals(""))
            {
                throw new ItemDefinitionException("The name cannot be undefined.");
            }
            resourcePrefabPath = resourcePrefabPath.Split(".").First();
            resourceIconPath = resourceIconPath.Split(".").First();

            if (Resources.Load(resourcePrefabPath) == null)
            {
                throw new ItemDefinitionException("The prefab of this item must be defined from a valid \"Resources\" directory.");
            }

            if (!_itemScopes.Contains(scope))
            {
                throw new ItemDefinitionException("The given scope is not defined within the manager.");
            }

            ItemDefinition itemDefinition = new ItemDefinition()
            {
                Name = name,
                Scope = scope,
                MaxStackCount = maxStackCount,
                IconPath = resourceIconPath,
                PrefabPath = resourcePrefabPath,
                IconGUID = iconGUID,
                PrefabGUID = prefabGUID
            };

            if (!_itemDefinitions.Add(itemDefinition))
            {
                throw new ItemDefinitionException("\"" + itemDefinition.GetQualifiedName() + "\" is not unique.");
            }

            return itemDefinition;
        }

        /// <summary>
        /// Creates a new item inheriting <see cref="ItemDefinition"/> and adds it to the mangement.
        /// </summary>
        /// <typeparam name="T">A type which inherits <see cref="ItemDefinition"/>.</typeparam>
        /// <param name="scope">The scope of the definition."/></param>
        /// <param name="name">The name of the definition. Has to be unique in combination with the scope.</param>
        /// <param name="maxStackCount">How high this item can be stacked.</param>
        /// <param name="resourceIconPath">The path to where the icon can be found. Has to be a resources path.</param>
        /// <param name="resourcePrefabPath">The path to where the prefab can be found. Has to be a resources path.</param>
        /// <param name="itemFields">All additional field values defined for type <typeparamref name="T"/>.</param>
        /// <returns>The created <see cref="ItemDefinition"/>.</returns>
        /// <exception cref="ItemDefinitionException"></exception>
        public T AddInheritedItemDefinition<T>(ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath, string iconGUID, string prefabGUID, HashSet<ItemField> itemFields) where T : ItemDefinition
        {
            CheckInitialized();

            if (name == null || name.Equals(""))
            {
                throw new ItemDefinitionException("The name cannot be undefined.");
            }
            resourcePrefabPath = resourcePrefabPath.Split(".").First();
            resourceIconPath = resourceIconPath.Split(".").First();

            if (Resources.Load(resourcePrefabPath) == null)
            {
                throw new ItemDefinitionException("The prefab of this item must be defined from a valid \"Resources\" directory.");
            }

            if (!_itemScopes.Contains(scope))
            {
                throw new ItemDefinitionException("The given scope is not defined within the manager.");
            }

            FieldInfo[] allTFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            T itemDefinition = (T)Activator.CreateInstance(typeof(T));
            itemDefinition.Name = name;
            itemDefinition.Scope = scope;
            itemDefinition.MaxStackCount = maxStackCount;
            itemDefinition.IconPath = resourceIconPath;
            itemDefinition.PrefabPath = resourcePrefabPath;
            itemDefinition.IconGUID = iconGUID;
            itemDefinition.PrefabGUID = prefabGUID;

            foreach (ItemField field in itemFields)
            {
                FieldInfo referenceField = allTFields.Where(f => f.Name.Equals(field.FieldName) && f.FieldType.Equals(field.FieldType)).FirstOrDefault();
                if (referenceField == null)
                {
                    throw new ItemDefinitionException("The type " + typeof(T).Name + " does not contain the field " + field.FieldName + " of type " + field.FieldType.Name + ".");
                }
                else
                {
                    referenceField.SetValue(itemDefinition, field.GetValue());
                }
            }

            if (!_itemDefinitions.Add(itemDefinition))
            {
                throw new ItemDefinitionException("\"" + itemDefinition.GetQualifiedName() + "\" is not unique.");
            }

            return itemDefinition;
        }

        /// <summary>
        /// Removes an <see cref="ItemDefinition"/> from the mangement.
        /// </summary>
        /// <param name="itemDefinition">The <see cref="ItemDefinition"/> to be removed.</param>
        /// <exception cref="ItemDefinitionException"></exception>
        public void RemoveItemDefinition(ItemDefinition itemDefinition)
        {
            CheckInitialized();

            if (!_itemDefinitions.Remove(itemDefinition))
            {
                throw new ItemDefinitionException("\"" + itemDefinition.GetQualifiedName() + "\" does not exist.");
            }

            _faultyItemDefinitions.Remove(itemDefinition);
            ItemDefinitionEdited(itemDefinition, null);
        }

        private void InternalRemoveItemDefinition(ItemDefinition itemDefinition)
        {
            CheckInitialized();

            if (!_itemDefinitions.Remove(itemDefinition))
            {
                throw new ItemDefinitionException("\"" + itemDefinition.GetQualifiedName() + "\" does not exist.");
            }
        }

        /// <summary>
        /// Edits a given item inheriting <see cref="ItemDefinition"/> and replaces the old within the mangement.
        /// </summary>
        /// <typeparam name="T">A type which inherits <see cref="ItemDefinition"/>.</typeparam>
        /// <param name="itemDefinition">The <see cref="ItemDefinition"/> to be edited.</param>
        /// <param name="scope">The new scope of the definition."/></param>
        /// <param name="name">The new name of the definition. Has to be unique in combination with the scope.</param>
        /// <param name="maxStackCount">How high this item can be stacked.</param>
        /// <param name="resourceIconPath">The new path to where the icon can be found. Has to be a resources path.</param>
        /// <param name="resourcePrefabPath">The new path to where the prefab can be found. Has to be a resources path.</param>
        /// <param name="itemFields">All additional new field values defined for type <typeparamref name="T"/>.</param>
        /// <returns>The created <see cref="ItemDefinition"/>.</returns>
        /// <exception cref="ItemDefinitionException"></exception>
        public void EditInheritedItemDefinition<T>(ItemDefinition itemDefinition, ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath, string iconGUID, string prefabGUID, HashSet<ItemField> itemFields) where T : ItemDefinition
        {
            InternalRemoveItemDefinition(itemDefinition);
            T newItemDefinition;

            try
            {
                newItemDefinition = AddInheritedItemDefinition<T>(scope, name, maxStackCount, resourceIconPath, resourcePrefabPath, iconGUID, prefabGUID, itemFields);
            }
            catch (Exception ex)
            {
                _itemDefinitions.Add(itemDefinition);
                throw ex;
            }

            _faultyItemDefinitions.Remove(itemDefinition);
            ItemDefinitionEdited(itemDefinition, newItemDefinition);
        }

        /// <summary>
        /// Edits a given <see cref="ItemDefinition"/> and replaces the old within the mangement.
        /// </summary>
        /// <param name="itemDefinition">The <see cref="ItemDefinition"/> to be edited.</param>
        /// <param name="scope">The new scope of the definition."/></param>
        /// <param name="name">The new name of the definition. Has to be unique in combination with the scope.</param>
        /// <param name="maxStackCount">How high this item can be stacked.</param>
        /// <param name="resourceIconPath">The new path to where the icon can be found. Has to be a resources path.</param>
        /// <param name="resourcePrefabPath">The new path to where the prefab can be found. Has to be a resources path.</param>
        /// <returns>The created <see cref="ItemDefinition"/>.</returns>
        /// <exception cref="ItemDefinitionException"></exception>
        public void EditItemDefinition(ItemDefinition itemDefinition, ItemScope scope, string name, int maxStackCount, string resourceIconPath, string resourcePrefabPath, string iconGUID, string prefabGUID)
        {
            InternalRemoveItemDefinition(itemDefinition);
            ItemDefinition newItemDefinition;

            try
            {
                newItemDefinition = AddItemDefinition(scope, name, maxStackCount, resourceIconPath, resourcePrefabPath, iconGUID, prefabGUID);
            }
            catch (Exception ex)
            {
                _itemDefinitions.Add(itemDefinition);
                throw ex;
            }

            _faultyItemDefinitions.Remove(itemDefinition);

            ItemDefinitionEdited(itemDefinition, newItemDefinition);
        }

        /// <summary>
        /// Adds a new <see cref="ItemScope"/> by a given unique name.
        /// </summary>
        /// <param name="name">The unique name of the scope to create.</param>
        /// <returns>The created <see cref="ItemScope"/>.</returns>
        /// <exception cref="ItemDefinitionException"></exception>
        public ItemScope AddItemScope(string name)
        {
            CheckInitialized();

            ItemScope scope = new ItemScope()
            {
                Name = name
            };

            if (name == null || name.Equals(""))
            {
                throw new ItemDefinitionException("The name cannot be undefined.");
            }

            if (!_itemScopes.Add(scope))
            {
                throw new ItemDefinitionException("\"" + name + "\" is not unique.");
            }

            return scope;
        }

        /// <summary>
        /// Removes a given <see cref="ItemScope"/>. Replaces the given scope used for the <see cref="ItemDefinition"/> with the <see cref="DefaultScope"/>.
        /// </summary>
        /// <param name="scope">The <see cref="ItemScope"/> to remove.</param>
        /// <exception cref="ItemDefinitionException"></exception>
        public void RemoveItemScope(ItemScope scope)
        {
            CheckInitialized();

            if (_defaultScope.Equals(scope))
            {
                throw new ItemDefinitionException("Cannot make changes to the default scope.");
            }

            List<ItemDefinition> definitionsToChange = _itemDefinitions.Where(def => def.Scope.Equals(scope)).ToList();
            List<ItemDefinition> definitionsToRedo = new List<ItemDefinition>();

            try
            {
                foreach (ItemDefinition def in definitionsToChange)
                {
                    definitionsToRedo.Add(AddItemDefinition(_defaultScope, def.Name, def.MaxStackCount, def.IconPath, def.PrefabPath, def.IconGUID, def.PrefabGUID));
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
                throw new ItemDefinitionException("\"" + scope.Name + "\" does not exist.");
            }

            ItemScopeEdited(scope, null);
        }

        /// <summary>
        /// Edits a given <see cref="ItemScope"/> and replaces the old.
        /// </summary>
        /// <param name="scope">The <see cref="ItemScope"/> to edit.</param>
        /// <param name="name">The new unique name of the scope.</param>
        /// <exception cref="ItemDefinitionException"></exception>
        public void EditItemScope(ItemScope scope, string name)
        {
            CheckInitialized();

            if (_defaultScope.Equals(scope))
            {
                throw new ItemDefinitionException("Cannot make changes to the default scope.");
            }

            ItemScope newScope = AddItemScope(name);

            if (!_itemScopes.Remove(scope))
            {
                _itemScopes.Remove(newScope);
                throw new ItemDefinitionException("\"" + scope.Name + "\" does not exist.");
            }

            List<ItemDefinition> definitionsToChange = _itemDefinitions.Where(def => def.Scope.Equals(scope)).ToList();
            List<ItemDefinition> definitionsToRedo = new List<ItemDefinition>();

            try
            {
                foreach (ItemDefinition def in definitionsToChange)
                {
                    definitionsToRedo.Add(AddItemDefinition(newScope, def.Name, def.MaxStackCount, def.IconPath, def.PrefabPath, def.IconGUID, def.PrefabGUID));
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
                if (!Initialize())
                {
                    throw new SystemException("The " + nameof(Itemizer) + " cannot be initialized. Is the path set up?");
                }
            }
        }

        private void ReadData()
        {
            ReadItemData();
            ReadItemScopes();
        }

        private void ReadItemData()
        {
            HashSet<ItemDefinition> readItemData = ResourcesUtil.GetFileData<HashSet<ItemDefinition>>(ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMEITEMS);
            _faultyItemDefinitions = new HashSet<ItemDefinition>();

            if (readItemData != null)
            {
                foreach (ItemDefinition def in readItemData)
                {
                    try
                    {
                        def.Deserialize();
                    }
                    catch
                    {
                        _faultyItemDefinitions.Add(def);
                    }
                }
            }

            _itemDefinitions = readItemData;
        }

        private void ReadItemScopes()
        {
            HashSet<ItemScope> readScopes = ResourcesUtil.GetFileData<HashSet<ItemScope>>(ProjectPrefKeys.ITEMDATASAVEPATH, FILENAMESCOPES);
            _itemScopes = readScopes;
        }

        /// <summary>
        /// Checks the assembly for all classes inheriting <see cref="ItemDefinition"/>.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Invokes the <see cref="OnItemScopeEdited"/> action.
        /// </summary>
        /// <param name="oldScope"></param>
        /// <param name="newScope"></param>
        public void ItemScopeEdited(ItemScope oldScope, ItemScope newScope)
        {
            OnItemScopeEdited?.Invoke(oldScope, newScope);
        }

        /// <summary>
        /// Invokes the <see cref="OnItemDefinitionEdited"/> action.
        /// </summary>
        /// <param name="oldItemDefinition"></param>
        /// <param name="newItemDefinition"></param>
        public void ItemDefinitionEdited(ItemDefinition oldItemDefinition, ItemDefinition newItemDefinition)
        {
            OnItemDefinitionEdited?.Invoke(oldItemDefinition, newItemDefinition);
        }
    }
}
