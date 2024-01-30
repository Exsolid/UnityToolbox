using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Management.Editor;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor
{
    public class TerrainGenerationErrorWindow : EditorWindow
    {
        private Vector2 _errorScrollPos;
        private SerializedDataErrorDetails _errors;

        private TerrainGenerationErrorWindow()
        {
        }

        public static TerrainGenerationErrorWindow Open(SerializedDataErrorDetails errors)
        {
            TerrainGenerationErrorWindow window = GetWindow<TerrainGenerationErrorWindow>("Terrain Generation Errors");
            window.maxSize = new Vector2(600, 400);
            window.minSize = new Vector2(600, 400);
            window.InitializeWindow(errors);
            return window;
        }

        public void InitializeWindow(SerializedDataErrorDetails errors)
        {
            _errors = errors;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            _errorScrollPos = GUILayout.BeginScrollView(_errorScrollPos);

            DisplayErrorRecursive(_errors);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void DisplayErrorRecursive(SerializedDataErrorDetails errors)
        {
            GUILayout.BeginVertical(new GUIStyle("window"), GUILayout.Height(35));
            GUILayout.Label(errors.ErrorDescription);
            foreach (SerializedDataErrorDetails internalErr in errors.Traced)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("->");
                DisplayErrorRecursive(internalErr);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public void OnAfterAssemblyReload()
        {
            InitializeWindow(_errors);
        }

        void OnEnable()
        {
            TerrainGenerationEditorEvents.Instance.OnClose += Close;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            TerrainGenerationEditorEvents.Instance.OnClose -= Close;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }
    }
}
