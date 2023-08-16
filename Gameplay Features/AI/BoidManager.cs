using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
