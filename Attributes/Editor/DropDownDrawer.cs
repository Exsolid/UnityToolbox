using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(DropDownAttribute))]
public class DropDownDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!fieldInfo.FieldType.Equals(typeof(int)))
        {
            Debug.LogError(fieldInfo.DeclaringType+": Can only generate DropDownAttribute for Type int.");
            return;
        }
        FieldInfo[] objectFields = fieldInfo.DeclaringType.GetFields();
        var hasCollectionOfDefinedName = objectFields.Where(field => field.Name.Equals(((DropDownAttribute)attribute).VariableNameForList) && field.FieldType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)));
        if (!hasCollectionOfDefinedName.Any())
        {
            Debug.LogError(fieldInfo.DeclaringType + ": DeclaringType does not define a collection derived of type IList<> with the name " + ((DropDownAttribute)attribute).VariableNameForList);
            return;
        }
        IList list = (IList)hasCollectionOfDefinedName.FirstOrDefault().GetValue(property.serializedObject.targetObject);
        GUIContent dropDownSelect = new GUIContent(fieldInfo.Name);
        if (list == null || list.Count == 0) return;
        string[] values = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            values[i] = list[i].ToString();
        }
        try
        {
            if (fieldInfo.GetValue(property.serializedObject.targetObject) is int index)
            {
                fieldInfo.SetValue(property.serializedObject.targetObject, EditorGUILayout.Popup(dropDownSelect, index, values));
            }
        }
        catch (ArgumentException ex)
        {
            Debug.LogError(ex.GetType()+ ": The DropDown attribute requires a CustomEditor for the DeclaringType " + fieldInfo.DeclaringType + " it is being used on. (It require no functionality)");
        }
    }
}
