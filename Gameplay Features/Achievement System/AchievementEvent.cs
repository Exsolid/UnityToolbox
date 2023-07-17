using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.Events;

namespace UnityToolbox.Achievments
{
    /// <summary>
    /// An event which is used for achviement triggers.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="AchievementTrigger"/></typeparam>
    public class AchievementEvent<T> : PubSubEvent<T>
    {
    }
}
