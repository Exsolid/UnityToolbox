using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;
using UnityToolbox.General.SubEvents;

namespace UnityToolbox.GameplayFeatures.Achievements
{
    /// <summary>
    /// The manager for all Achievements. Needs to be present if Achievements are used.
    /// The manager needs to be manually updated to cache all existing achievements in the project.
    /// </summary>
    public class AchievementManager : Module, ISerializationCallbackReceiver
    {
        /// <summary>
        /// All <see cref="AchievementTrigger"/> within the project.
        /// </summary>
        public static List<string> TriggersForEditor = new List<string>();

        [SerializeField][HideInInspector] private List<string> _triggersForEditor = new List<string>();

        [ReadOnly][SerializeField] private List<AchievementData> _allAchievements;

        private void Start()
        {
            foreach (AchievementData data in _allAchievements)
            {
                ModuleManager.GetModule<EventAggregator>().GetEvent<PubSubEvent<Type>>().Subscribe
                    ((triggerType) =>
                        {
                            if (data.GetTriggerType().Equals(triggerType))
                            {
                                data.Execute();
                            }
                        }
                    );
            }
        }

        public void OnBeforeSerialize()
        {
            _triggersForEditor = typeof(AchievementTrigger).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AchievementTrigger))).Select(t => t.Name).ToList();
        }

        public void OnAfterDeserialize()
        {
            TriggersForEditor = _triggersForEditor;
        }
    }
}
