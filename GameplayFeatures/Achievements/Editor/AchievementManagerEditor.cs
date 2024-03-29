using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityToolbox.GameplayFeatures.Achievements;

namespace UnityToolbox.GameplayFeatures.Achievements.Editor
{
    /// <summary>
    /// An editor which defines the button, to manually cache all achievements.
    /// </summary>
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AchievementManager manager = (AchievementManager)target;
            if (GUILayout.Button("Update Achievement Cache"))
            {
                List<AchievementData> data = new List<AchievementData>();
                data.AddRange(
                    AssetDatabase.FindAssets("t:" + typeof(AchievementData).Name).Select(guid =>
                    (AchievementData)AssetDatabase.LoadAllAssetsAtPath
                    (AssetDatabase.GUIDToAssetPath(guid))
                    .FirstOrDefault()));
                FieldInfo info = typeof(AchievementManager).GetField("_allAchievements", BindingFlags.NonPublic | BindingFlags.Instance);
                info.SetValue(target, data);
            }
        }
    }
}

