using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityToolbox.General.Attributes;

namespace UnityToolbox.GameplayFeatures.Achievements
{
    /// <summary>
    /// The definition for an achievement.
    /// </summary>
    [CreateAssetMenu()]
    public class AchievementData : ScriptableObject
    {
        /// <summary>
        /// The achievement title.
        /// </summary>
        public string Title;

        /// <summary>
        /// The achievement description.
        /// </summary>
        public string Description;

        /// <summary>
        /// the achievment icon.
        /// </summary>
        public Texture2D Icon;

        /// <summary>
        /// The type of the defined trigger. Is set within the custom editor.
        /// </summary>
        [HideInInspector] public string OnTriggerType;
        /// <summary>
        /// The type data of the defined trigger. Is set within the custom editor and used for serialization.
        /// </summary>
        [HideInInspector] public SerializationData OnTriggerData;

        [SerializeField][HideInInspector] private List<string> _triggers = new List<string>();

        /// <summary>
        /// The selected trigger from <see cref="_triggers"/>.
        /// </summary>
        [DropDown(nameof(_triggers))] public int SelectedTrigger;

        [SerializeField][HideInInspector] private List<string> _typeMethods = new List<string>();
        /// <summary>
        /// The selected method from the defined <see cref="OnTriggerType"/>.
        /// </summary>
        [DropDown(nameof(_typeMethods))] public int OnTiggerMethod;

        /// <summary>
        /// Returns the type of the selected trigger.
        /// </summary>
        /// <returns>The <see cref="AchievementTrigger"/> type.</returns>
        public Type GetTriggerType()
        {
            return Type.GetType(_triggers[SelectedTrigger]);
        }

        /// <summary>
        /// Executes the defined method from the selected type.
        /// </summary>
        public void Execute()
        {
            MethodInfo[] methods = Type.GetType(OnTriggerType).GetMethods(BindingFlags.Public | BindingFlags.Static);
            methods[OnTiggerMethod].Invoke(null, null);
        }

        private void OnValidate()
        {
            _triggers = AchievementManager.TriggersForEditor;

            MethodInfo[] methods = Type.GetType(OnTriggerType).GetMethods(BindingFlags.Public | BindingFlags.Static);
            if (methods != null)
            {
                _typeMethods = methods.Select(m => m.Name).ToList();
            }

            if (OnTiggerMethod >= _typeMethods.Count)
            {
                OnTiggerMethod = 0;
            }

            if (SelectedTrigger >= _triggers.Count)
            {
                SelectedTrigger = 0;
            }
        }
    }
}
