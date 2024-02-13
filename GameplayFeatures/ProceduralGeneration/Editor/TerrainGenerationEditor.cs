using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain;
using UnityToolbox.General.Management;
using UnityToolbox.General.Management.Editor;
using UnityToolbox.General.Preferences;
using static UnityEngine.GraphicsBuffer;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor
{
    [CustomEditor(typeof(TerrainGeneration))]
    public class TerrainGenerationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TerrainGeneration gen = (TerrainGeneration)target;

            if (gen.Data == null)
            {
                gen.Data = TerrainGenerationIO.Instance.ReadData();
            }
            else if(gen.Data.Count == 0)
            {
                GUILayout.Label("Create generation data first! (UnityToolbox -> Terrain Generation)");

                if (GUILayout.Button("Refresh Data"))
                {
                    gen.Data = TerrainGenerationIO.Instance.ReadData();
                }
            }
            else
            {
                if (GUILayout.Button("Refresh Data"))
                {
                    gen.Data = TerrainGenerationIO.Instance.ReadData();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Generation data: ");
                gen.SelectedData = EditorGUILayout.Popup("", gen.SelectedData, gen.Data.Count == 0 ? new string[] { } : gen.Data.Keys.ToArray(), GUILayout.Width(180));
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Generate Terrain"))
                {
                    gen.GenerateTerrain();
                    TextAsset data = Resources.Load(ResourcesUtil.GetResourcesPath(ProjectPrefKeys.PROCEDURALGENERATIONDATAPATH) +
                                            "ProceduralGenerationData") as TextAsset;
                    string assetPath = AssetDatabase.GetAssetPath(data).Replace("ProceduralGenerationData.txt","") + "/Shader Data " + gen.Data.ElementAt(gen.SelectedData).Key + "/";

                    Directory.CreateDirectory(Application.dataPath.Replace("Assets","") + assetPath);
                    foreach (Texture2DArray texture2DArray in gen.Texture2DArrays)
                    {
                        AssetDatabase.CreateAsset(texture2DArray, assetPath + texture2DArray.name+ ".asset");
                    }
                    foreach (Texture2D texture2D in gen.Texture2Ds)
                    {
                        AssetDatabase.CreateAsset(texture2D, assetPath + texture2D.name + ".asset");
                    }
                    EditorUtility.SetDirty(gen.TerrainMaterial);
                    AssetDatabase.SaveAssetIfDirty(gen.TerrainMaterial);
                }
            }
        }
    }
}
