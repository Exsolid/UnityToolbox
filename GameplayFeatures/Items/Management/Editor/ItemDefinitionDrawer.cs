using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;


namespace UnityToolbox.Item.Management.Editor
{
    /// <summary>
    /// This class enables a selection field for the <see cref="ItemDefinition"/> within the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ItemDefinition))]
    public class ItemDefinitionDrawer : PropertyDrawer
    {
        private ItemDefinitionSelectionWindow _window;
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

            SerializedProperty nameToEdit = _property.FindPropertyRelative(nameof(ItemDefinition.Name));
            SerializedProperty scopeToEdit = _property.FindPropertyRelative(nameof(ItemDefinition.Scope));

            EditorGUI.LabelField(position, scopeToEdit.FindPropertyRelative(nameof(ItemScope.Name)).stringValue + ItemDefinition.DEVIDER + nameToEdit.stringValue);

            position.x += position.width + 2;
            position.width = 18;
            position.height = 18;

            if (GUI.Button(position, ">"))
            {
                _window = ItemDefinitionSelectionWindow.Open();
                _window.OnItemDefinitionSelected += ItemDefinitionSelected;
            }

            EditorGUI.EndProperty();
        }

        public void ItemDefinitionSelected(ItemDefinition itemDefinition)
        {
            if (_window != null)
            {
                _window.OnItemDefinitionSelected -= ItemDefinitionSelected;
                _window.Close();
                fieldInfo.SetValue(_property.serializedObject.targetObject, itemDefinition);

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
}
