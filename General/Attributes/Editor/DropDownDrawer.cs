using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityToolbox.General.Attributes;

/// <summary>
/// Defines a [DropDown(..)] attribute, which creates a drop down menu to select from. See <see cref="DropDownAttribute"/> for the usage.
/// </summary>
[CustomPropertyDrawer(typeof(DropDownAttribute))]
public class DropDownDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var instance = new object();

        if (!fieldInfo.DeclaringType.Equals(property.serializedObject.targetObject.GetType()))
        {
            FieldInfo[] objFields = property.serializedObject.targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(property.propertyPath.Contains('['))
            {
                //Find correct object in list
                var collection = objFields.Where(
                    field => field.FieldType.GetInterfaces().Any(
                        i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) 
                        && i.GetGenericArguments().Length >= 1 && i.GetGenericArguments()[0].Equals(fieldInfo.DeclaringType)));

                int index = Int32.Parse(property.propertyPath.Split('[').Last().Split(']').First());
                IList allInstances = (IList)collection.FirstOrDefault().GetValue(property.serializedObject.targetObject);

                for (int i = 0; i < allInstances.Count; i++)
                {
                    if (i == index)
                    {
                        instance = allInstances[i];
                        break;
                    }
                }
            }
            else
            {
                FieldInfo objectOfNested = objFields.Where(field => field.FieldType.Equals(fieldInfo.DeclaringType)).FirstOrDefault();
                instance = objectOfNested.GetValue(property.serializedObject.targetObject);
            }
        }
        else
        {
            instance = property.serializedObject.targetObject;
        }
        
        if (!fieldInfo.FieldType.Equals(typeof(int)))
        {
            Debug.LogError(fieldInfo.DeclaringType+": Can only generate DropDownAttribute for Type int.");
            return;
        }

        FieldInfo[] objectFields;
        if (((DropDownAttribute)attribute).UseParentNestedForList)
        {
            objectFields = property.serializedObject.targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }
        else
        {
            objectFields = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        var hasCollectionOfDefinedName = objectFields.Where(field => field.Name.Equals(((DropDownAttribute)attribute).VariableNameForList) && field.FieldType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)));
        if (!hasCollectionOfDefinedName.Any())
        {
            Debug.LogError(fieldInfo.DeclaringType + ": DeclaringType does not define a collection derived of type IList<> with the name " + ((DropDownAttribute)attribute).VariableNameForList);
            return;
        }

        IList list = (IList)hasCollectionOfDefinedName.FirstOrDefault().GetValue(((DropDownAttribute)attribute).UseParentNestedForList ? property.serializedObject.targetObject : instance);
        GUIContent dropDownSelect = new GUIContent(fieldInfo.Name);
        if (list == null || list.Count == 0)
        {
            return;
        }

        string[] values = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            values[i] = list[i].ToString();
        }

        try
        {
            object boxed = (object) instance;
            if (fieldInfo.GetValue(boxed) is int index)
            {
                int newIndex = EditorGUI.Popup(position, ObjectNames.NicifyVariableName(fieldInfo.Name), index, values);
                fieldInfo.SetValue(boxed, newIndex);
                if (newIndex != index)
                {
                    //Make UnityEditor set scene dirty
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                    //Execute OnValidate as if a normal change happend
                    MethodInfo methodInfo = property.serializedObject.targetObject.GetType().GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(property.serializedObject.targetObject, new object[] { });
                    }
                }
            }
        }
        catch (ArgumentException ex)
        {
            Debug.LogError(ex);
        }
    }
}
