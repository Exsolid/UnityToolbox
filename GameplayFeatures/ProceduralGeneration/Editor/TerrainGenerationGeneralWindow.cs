using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.Items;
using UnityToolbox.GameplayFeatures.Items.Management;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes.Layered;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General;
using UnityToolbox.General.Management;
using UnityToolbox.General.Management.Editor;
using UnityToolbox.General.Preferences;
using Random = System.Random;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    public class TerrainGenerationGeneralWindow : EditorWindow
    {
        private const int GENERATION = 0;
        private const int MESH = 1;
        private const int SETTINGS = 2;

        private int _selectedGenerationType;
        private int _selectedGenerator;
        private readonly string[] _generationTypes = new[] { "Cellular Automata" };
        private string _status;

        private int _selectedMeshType;
        private readonly string[] _meshTypes = new[] { "Layered" };
        private TerrainMeshType _terrainMeshType;

        private Vector2 _generationTypeDetailsScrollPos;

        private TerrainGenerationType _terrainGenerationType;
        private int[,] _generatedExample;

        private Font _monospacedFont;
        private int _selectedTab;

        private string _currentName;
        private string _prevName;
        private Dictionary<string, TerrainGenerationData> _data;
        private TerrainGenerationData _selectedData;

        private bool _isInitilized;
        private string _assetPathInProject;

        private SerializedDataErrorDetails _errors;
        private TerrainGenerationErrorWindow _errorWindow;

        [MenuItem("UnityToolbox/Terrain Generation")]
        public static void Open()
        {
            TerrainGenerationGeneralWindow window = GetWindow<TerrainGenerationGeneralWindow>("Terrain Generation");
            window.maxSize = new Vector2(500, 700);
            window.minSize = new Vector2(500, 700);
        }

        private void InitializeWindow()
        {
            _assetPathInProject = ResourcesUtil.GetProjectPath(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH);
            _isInitilized = TerrainGenerationIO.Instance.Initialize();
            _status = "Status: -";
            _generationTypeDetailsScrollPos = Vector2.zero;
            _selectedGenerationType = 0;
            _selectedGenerator = 0;

            _generatedExample = new int[50, 10];

            _monospacedFont = AssetDatabase.LoadAssetAtPath<Font>(GetPathOfFont());
            GetData();

            if (_errors.HasErrors)
            {
                _errorWindow = TerrainGenerationErrorWindow.Open(_errors);
            }
        }

        private void Awake()
        {
            InitializeWindow();
        }

        private void OnGUI()
        {
            if (_errors.HasErrors && _errorWindow == null)
            {
                _errorWindow = TerrainGenerationErrorWindow.Open(_errors);
            }
            else if (!_errors.HasErrors && _errorWindow != null)
            {
                _errorWindow.Close();
                _errorWindow = null;
            }

            GUILayout.BeginVertical();

            DrawLineHorizontal();
            GUILayout.Label(_status);
            DrawLineHorizontal();

            if (_isInitilized)
            {
                _selectedTab = GUILayout.Toolbar(_selectedTab, new string[] { "Generation", "Mesh", "Settings" });
                DrawLineHorizontal();

                switch (_selectedTab)
                {
                    case GENERATION:
                        DrawGenerationTab();
                        DrawLineHorizontal();
                        DrawEndButtons();
                        break;
                    case MESH:
                        DrawMeshTab();
                        DrawLineHorizontal();
                        DrawEndButtons();
                        break;
                    case SETTINGS:
                        DrawSettingsTab();
                        break;
                }
            }
            else
            {
                DrawSettingsTab();
            }

            GUILayout.EndVertical();
        }

        private void DrawMeshTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mesh Type: ");
            int prev = _selectedMeshType;
            _selectedMeshType = EditorGUILayout.Popup("", _selectedMeshType, _meshTypes, GUILayout.Width(200));
            if (prev != _selectedMeshType)
            {
                switch (_selectedMeshType)
                {
                    case 0:
                        _terrainMeshType = new TerrainMeshTypeLayered();
                        break;
                    default:
                        break;
                }
            }
            GUILayout.EndHorizontal();
            DrawLineHorizontal();

            GUILayout.BeginHorizontal();
            _generationTypeDetailsScrollPos = GUILayout.BeginScrollView(_generationTypeDetailsScrollPos);
            _terrainMeshType.DrawDetails();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }


        private void DrawSettingsTab()
        {
            GUILayout.Label("To update the Procedural Generation data path, please enter a valid path below. \nIt is required that it contains \"Resources\".");
            DrawLineHorizontal();
            GUILayout.BeginHorizontal();
            Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
            GUILayout.Label(Application.dataPath + "/");
            _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                if (!ResourcesUtil.TrySetValidPath(Application.dataPath + "/" + _assetPathInProject, ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH))
                {
                    UpdateStatus("\"" + Path.GetFullPath(Application.dataPath + "/" + _assetPathInProject) + "\" is not valid.");
                }
                else
                {
                    AssetDatabase.Refresh();
                    _isInitilized = TerrainGenerationIO.Instance.Initialize();
                    GetData();
                    UpdateStatus("Path updated.");
                }
            }
        }

        private void DrawGenerationTab()
        {
            DrawGeneralDetails();

            DrawLineHorizontal();

            DrawGenerationDetails();

            DrawLineHorizontal();

            DrawExampleGen();
        }

        private void DrawExampleGen()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            string generatedAsString = "";

            for (int y = 0; y < _generatedExample.GetLength(1); y++)
            {
                for (int x = 0; x < _generatedExample.GetLength(0); x++)
                {
                    generatedAsString += _generatedExample[x, y];
                }
                generatedAsString += "\n";
            }
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.font = _monospacedFont;
            GUILayout.Label(generatedAsString, style, GUILayout.Height(160));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label(nameof(TerrainValues.Floor) + ": " + (int)TerrainValues.Floor + " | " + nameof(TerrainValues.Wall) + ": " + (int)TerrainValues.Wall);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Example With Static Size"))
            {
                _generatedExample = _terrainGenerationType.GetExampleGeneration(50, 10);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawGeneralDetails()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name of Generator: ");
            _currentName = GUILayout.TextField(_currentName, GUILayout.Width(180));

            if (GUILayout.Button("+", GUILayout.Width(18)))
            {
                if (_data.ContainsKey(_currentName))
                {
                    UpdateStatus(
                        "The generator name \"" + _currentName +
                        "\" already exists, it must be unique!");
                    _currentName = _selectedData.GeneratorName;
                }
                else
                {
                    UpdateDataNew(_currentName);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("All Generators: ");
            int prev = _selectedGenerator;
            _selectedGenerator = EditorGUILayout.Popup("", _selectedGenerator, _data.Count == 0 ? new string[]{} : _data.Keys.ToArray(), GUILayout.Width(180));
            if (prev != _selectedGenerator)
            {
                UpdateData(_selectedGenerator);
            }

            if (GUILayout.Button("-", GUILayout.Width(18)))
            {
                if (_data.Count != 1)
                {
                    _data.Remove(_selectedData.GeneratorName);
                    UpdateData(_data.First().Value);
                }
                else
                {
                    _data.Remove(_selectedData.GeneratorName);
                    UpdateDataNew("Unique Name");
                }
                //TODO sanity check
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Generation Type: ");
            prev = _selectedGenerationType;
            _selectedGenerationType = EditorGUILayout.Popup("", _selectedGenerationType, _generationTypes, GUILayout.Width(200));
            if (prev != _selectedGenerationType)
            {
                switch (_selectedGenerationType)
                {
                    case 0:
                        _terrainGenerationType = new TerrainGenerationTypeCellularAutomata();
                        break;
                    default:
                        break;
                }
            }
            GUILayout.EndHorizontal();
        }

        private void UpdateData(int dataPos)
        {
            TerrainGenerationData data = _data.ElementAt(dataPos).Value;
            UpdateData(data);
        }

        private void UpdateData(TerrainGenerationData data)
        {
            if (_selectedData != null)
            {
                _selectedData.GenerationData = _terrainGenerationType.Serialize();
                _selectedData.MeshData = _terrainMeshType.Serialize();
            }

            _selectedData = data;

            _prevName = _selectedData.GeneratorName;
            _currentName = _selectedData.GeneratorName;
            _selectedGenerator = _data.Keys.ToList().IndexOf(_selectedData.GeneratorName);

            CreateTypeSpecificTabs();

            _terrainMeshType.Deserialize(_selectedData.MeshData);
            _terrainGenerationType.Deserialize(_selectedData.GenerationData);
        }

        private void CreateTypeSpecificTabs()
        {
            if (_selectedData.MeshData.GetType() == typeof(TerrainMeshTypeLayeredData))
            {
                _terrainMeshType = new TerrainMeshTypeLayered();
            }

            if (_selectedData.GenerationData.GetType() == typeof(TerrainGenerationTypeCellularAutomataData))
            {
                _terrainGenerationType = new TerrainGenerationTypeCellularAutomata();
            }
        }

        private void UpdateDataNew(string name)
        {
            if (_selectedData != null)
            {
                _selectedData.GenerationData = _terrainGenerationType.Serialize();
                _selectedData.MeshData = _terrainMeshType.Serialize();
            }

            _selectedData = new TerrainGenerationData
            {
                GeneratorName = name
            };

            _terrainGenerationType = new TerrainGenerationTypeCellularAutomata();
            _terrainMeshType = new TerrainMeshTypeLayered();

            _selectedData.GenerationData = _terrainGenerationType.Serialize();
            _selectedData.MeshData = _terrainMeshType.Serialize();

            _data.Add(_selectedData.GeneratorName, _selectedData);
            _prevName = _selectedData.GeneratorName;
            _currentName = _selectedData.GeneratorName;
            _selectedGenerator = _data.Keys.ToList().IndexOf(_selectedData.GeneratorName);

            _terrainMeshType.Deserialize(_selectedData.MeshData);
            _terrainGenerationType.Deserialize(_selectedData.GenerationData);
        }

        private void GetData()
        {
            _errors = new SerializedDataErrorDetails();
            _data = TerrainGenerationIO.Instance.ReadData();
            if (_data.Count == 0)
            {
                try
                {
                    _terrainGenerationType = new TerrainGenerationTypeCellularAutomata();
                    _terrainMeshType = new TerrainMeshTypeLayered();
                }
                catch (Exception ex)
                {
                    UpdateStatus(ex.Message);
                }
                UpdateDataNew("Unique Name");
                return;
            }

            foreach (TerrainGenerationData dataCheck in _data.Values)
            {
                if (dataCheck.GenerationData.GetType() == typeof(TerrainGenerationTypeCellularAutomataData))
                {
                    _terrainGenerationType = new TerrainGenerationTypeCellularAutomata();
                }
                SerializedDataErrorDetails temp2 = _terrainGenerationType.Deserialize(dataCheck.GenerationData);

                if (dataCheck.MeshData.GetType() == typeof(TerrainMeshTypeLayeredData))
                {
                    _terrainMeshType = new TerrainMeshTypeLayered();
                }
                SerializedDataErrorDetails temp1 = _terrainMeshType.Deserialize(dataCheck.MeshData);
                SerializedDataErrorDetails err = new SerializedDataErrorDetails();

                err.HasErrors = temp1.HasErrors || temp2.HasErrors;
                if (err.HasErrors)
                {
                    _errors.HasErrors = true;
                    _errors.ErrorDescription = "The terrain generation contains asset errors.";
                    err.ErrorDescription = "The terrain generation " + dataCheck.GeneratorName + " contains asset errors.";
                    if (temp1.HasErrors)
                    {
                        err.Traced.Add(temp1);
                    }

                    if (temp2.HasErrors)
                    {
                        err.Traced.Add(temp2);
                    }
                    _errors.Traced.Add(err);
                }
            }

            UpdateData(_data.Values.ElementAt(_selectedGenerator));
        }

        private void DrawGenerationDetails()
        {
            _generationTypeDetailsScrollPos = GUILayout.BeginScrollView(_generationTypeDetailsScrollPos);
            _terrainGenerationType.DrawDetails();
            GUILayout.EndScrollView();
        }

        private void DrawEndButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save / Rename"))
            {
                TerrainGenerationEditorEvents.Instance.Close();
                if (SetData())
                {
                    GetData();
                    UpdateStatus("All data has been saved and reloaded.");
                }
            }

            GUILayout.EndHorizontal();
        }

        private bool SetData()
        {
            if (!_currentName.Equals(_selectedData.GeneratorName))
            {
                if (_data.ContainsKey(_currentName))
                {
                    UpdateStatus(
                        "The generator name \"" + _currentName +
                        "\" already exists, it must be unique!");
                    _currentName = _selectedData.GeneratorName;
                    return false;
                }

                _data.Remove(_selectedData.GeneratorName);
                _selectedData.GeneratorName = _currentName;
                _data.Add(_selectedData.GeneratorName, _selectedData);
                _selectedGenerator = _data.Keys.ToList().IndexOf(_selectedData.GeneratorName);
            }

            _selectedData.GenerationData = _terrainGenerationType.Serialize();
            _selectedData.MeshData = _terrainMeshType.Serialize();
            TerrainGenerationIO.Instance.WriteData(_data);
            AssetDatabase.Refresh();
            return true;
        }

        private void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public void OnAfterAssemblyReload()
        {
            InitializeWindow();
        }

        private string GetPathOfFont()
        {
            string[] paths = Directory.GetFiles(Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.UNITYTOOLBOXPATH), "monospacedUbuntu.regular.ttf", SearchOption.AllDirectories);
            if (paths.Length > 0)
            {
                return paths[0].Replace(Application.dataPath, "Assets/");
            }

            return "";
        }

        private void UpdateStatus(string status)
        {
            _status = "Status:     " + status;
            Repaint();
        }

        void OnEnable()
        {
            TerrainGenerationEditorEvents.Instance.OnUpdateStatus += UpdateStatus;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            TerrainGenerationEditorEvents.Instance.OnUpdateStatus -= UpdateStatus;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            TerrainGenerationEditorEvents.Instance.Close();
        }

        void OnDestroy()
        {
            TerrainGenerationEditorEvents.Instance.Close();
            SetData();
        }


    }
}
