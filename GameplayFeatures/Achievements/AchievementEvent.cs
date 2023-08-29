using UnityToolbox.General.SubEvents;

namespace UnityToolbox.GameplayFeatures.Achievements
{
    /// <summary>
    /// An event which is used for achviement triggers.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="AchievementTrigger"/></typeparam>
    public class AchievementEvent<T> : PubSubEvent<T>
    {
    }
}
