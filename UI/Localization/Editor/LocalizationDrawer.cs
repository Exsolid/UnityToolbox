using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationID))]
    public class LocalizationDrawer : PropertyDrawer
    {
        private LocalizationSelectionWindow _window;
        private SerializedProperty _property;

        private Label _locaLabel;
        private LocalizationID _drawerId;

        public LocalizationID DrawerId
        {
            get { return _drawerId; }
        }

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

            SerializedProperty nameToEdit = _property.FindPropertyRelative(nameof(LocalizationID.Name));
            SerializedProperty scopeToEdit = _property.FindPropertyRelative(nameof(LocalizationID.Scope));
            EditorGUI.LabelField(position, scopeToEdit.FindPropertyRelative(nameof(LocalizationScope.Name)).stringValue + LocalizationID.DEVIDER + nameToEdit.stringValue);

            position.x += position.width + 2;
            position.width = 18;
            position.height = 18;

            if (GUI.Button(position, ">"))
            {
                _window = LocalizationSelectionWindow.Open();
                _window.OnIDSelected += IDSelectedInspector;
            }

            EditorGUI.EndProperty();
        }

        public void IDSelectedInspector(LocalizationID ID)
        {
            if (_window != null)
            {
                _window.OnIDSelected -= IDSelectedInspector;
                _window.Close();
                fieldInfo.SetValue(_property.serializedObject.targetObject, ID);

                //Make UnityEditor set scene dirty
                EditorUtility.SetDirty(_property.serializedObject.targetObject);
                //Execute OnValidate as if a normal change happened
                MethodInfo methodInfo = _property.serializedObject.targetObject.GetType().GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(_property.serializedObject.targetObject, new object[] { });
                }
            }
        }

        public void IDSelectedUIElement(LocalizationID ID)
        {
            if (_window != null)
            {
                _drawerId = ID;
                _locaLabel.text = _drawerId.GetQualifiedName();
                _window.OnIDSelected -= IDSelectedUIElement;
                _window.Close();
            }
        }

        public VisualElement CreateVisualElement(LocalizationID id)
        {
            _drawerId = id;

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            string nameToEdit = _drawerId.Name;
            LocalizationScope scopeToEdit = _drawerId.Scope;

            _locaLabel = new Label()
            {
                text = scopeToEdit.Name + LocalizationID.DEVIDER + nameToEdit
            };
            _locaLabel.style.alignSelf = Align.FlexStart;

            container.Add(_locaLabel);

            Button button = new Button()
            {
                text = ">"
            };
            _locaLabel.style.alignSelf = Align.FlexEnd;

            container.Add(button);

            button.clicked += () =>
            {
                _window = LocalizationSelectionWindow.Open();
                _window.OnIDSelected += IDSelectedUIElement;
            };

            return container;
        }
    } 
}
