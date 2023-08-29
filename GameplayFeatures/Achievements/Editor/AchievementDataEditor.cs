using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace UnityToolbox.Achievments
{
    /// <summary>
    /// An editor, which takes care of the script file selection.
    /// </summary>
    [CustomEditor(typeof(AchievementData))]
    public class AchievementDataEditor : Editor
    {
        private MonoScript script = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AchievementData data = (AchievementData)target;

            if (data.OnTriggerType != null && !data.OnTriggerType.Equals(""))
            {
                script = data.OnTriggerData.Deserialize() as MonoScript;
            }

            script = EditorGUILayout.ObjectField("On Trigger Class", script, typeof(MonoScript), false) as MonoScript;

            if (script != null)
            {
                data.OnTriggerType = script.GetClass().Name;
                data.OnTriggerData = script.Serialize();
                EditorUtility.SetDirty(data);
            }
        }
    }
}
