using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer(typeof(LocalisationID))]
public class LocalisationDrawer : PropertyDrawer
{
    private LocalisationSelectionWindow _window;
    private SerializedProperty _property;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        _property = property;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        position.width -= 20;
        position.height = 18;

        SerializedProperty nameToEdit = _property.FindPropertyRelative(nameof(LocalisationID.Name));
        SerializedProperty scopeToEdit = _property.FindPropertyRelative(nameof(LocalisationID.Scope));
        EditorGUI.LabelField(position, scopeToEdit.FindPropertyRelative(nameof(LocalisationScope.Name)).stringValue + LocalisationID.DEVIDER + nameToEdit.stringValue);

        position.x += position.width + 2;
        position.width = 18;
        position.height = 18;

        if (GUI.Button(position, ">"))
        {
            _window = LocalisationSelectionWindow.Open();
            _window.OnIDSelected += IDSelected;
        }

        EditorGUI.EndProperty();
    }

    public void IDSelected(LocalisationID ID)
    {
        if(_window != null)
        {
            _window.OnIDSelected -= IDSelected;
            _window.Close();
            fieldInfo.SetValue(_property.serializedObject.targetObject, ID);

            //Make UnityEditor set scene dirty
            EditorUtility.SetDirty(_property.serializedObject.targetObject);
            //Execute OnValidate as if a normal change happend
            MethodInfo methodInfo = _property.serializedObject.targetObject.GetType().GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
            {
                methodInfo.Invoke(_property.serializedObject.targetObject, new object[] { });
            }
        }
    }
}