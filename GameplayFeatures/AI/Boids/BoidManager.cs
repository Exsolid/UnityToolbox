using System;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.AI.Boids
{
    /// <summary>
    /// Currently this manager only exists to be able send pause events to all existing boids.
    /// </summary>
    public class BoidManager : Module
    {
        /// <summary>
        /// Event for toggling all boids to be (un-)paused.
        /// </summary>
        public event Action<bool> OnPauseBoids;

        /// <summary>
        /// Toggles all boids to be (un-)paused.
        /// </summary>
        public void PauseBoids(bool paused)
        {
            OnPauseBoids?.Invoke(paused);
        }
    }
}