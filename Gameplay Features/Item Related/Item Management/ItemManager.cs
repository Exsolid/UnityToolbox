using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Reflection;

public class ItemManager : Module
{
    /// <summary>
    /// Creates a new <see cref="ItemInstance"/> into the scene.
    /// </summary>
    /// <param name="position">The position of the new object.</param>
    /// <param name="rotation">The rotation of the new object.</param>
    /// <param name="scale">The scale of the new object.</param>
    /// <param name="itemDefKey">The <see cref="ItemDefinition.GetQualifiedName"/> which indentifies the <see cref="ItemDefinition"/>.</param>
    /// <returns>A new <see cref="ItemInstance"/>.</returns>
    public ItemInstance SpawnItemInstance(Vector3 position, Quaternion rotation, Vector3 scale, string itemDefKey)
    {
        if (!Itemizer.Instance.Initialized)
        {
            Debug.LogError("Cannot instaniate an item if the " + nameof(Itemizer) + " is not initialized.");
            return null;
        }

        HashSet<ItemDefinition> allItems = Itemizer.Instance.ItemDefinitions;
        ItemDefinition itemDefinition = allItems.Where(itemDef => itemDef.GetQualifiedName().Equals(itemDefKey)).FirstOrDefault();

        if (itemDefinition == null)
        {
            Debug.LogError("The item definition key \"" + itemDefKey + "\" cannot be found. Was it deleted?");
            return null;
        }

        if (itemDefinition.Prefab == null)
        {
            throw new SystemException("The required item does not have a prefab defined!");
        }

        GameObject instance = Instantiate(itemDefinition.Prefab, position, rotation);
        instance.transform.localScale = new Vector3(scale.x, scale.y, scale.z);

        ItemInstance itemInstance = instance.AddComponent<ItemInstance>();
        itemInstance.ItemName = itemDefinition.Name;
        itemInstance.PrefabData = new ResourceData()
        {
            ResourcePath = itemDefinition.PrefabPath,
        };

        if(itemDefinition.Icon != null)
        {
            itemInstance.Icon = Sprite.Create(itemDefinition.Icon, new Rect(0, 0, itemDefinition.Icon.width, itemDefinition.Icon.height), new Vector2(0.5f, 0.5f));
        }

        itemInstance.IconPath = itemDefinition.IconPath;
        itemInstance.MaxStackCount = itemDefinition.MaxStackCount;

        FieldInfo[] allFields = itemDefinition.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        HashSet<ItemField> itemFields = new HashSet<ItemField>();

        foreach (FieldInfo field in allFields)
        {
            if (field.FieldType.Equals(typeof(int)))
            {
                itemFields.Add(new ItemField(field.Name, (int) field.GetValue(itemDefinition)));
            }
            else if (field.FieldType.Equals(typeof(float)))
            {
                itemFields.Add(new ItemField(field.Name, (float)field.GetValue(itemDefinition)));
            }
            else if (field.FieldType.Equals(typeof(bool)))
            {
                itemFields.Add(new ItemField(field.Name, (bool)field.GetValue(itemDefinition)));
            }
            else if (field.FieldType.Equals(typeof(string)))
            {
                itemFields.Add(new ItemField(field.Name, (string)field.GetValue(itemDefinition)));
            }
        }

        itemInstance.ItemFields = itemFields;

        return itemInstance;
    }


}
