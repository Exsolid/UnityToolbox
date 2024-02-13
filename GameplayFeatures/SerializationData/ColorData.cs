using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.SerializationData
{
    /// <summary>
    /// The data for colors.
    /// </summary>
    public class ColorData : GameData
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public ColorData(Color color)
        {
            R = color.r;
            G = color.g;
            B = color.b;
            A = color.a;
        }

        public Color GetColor()
        {
            return new Color(R, G, B, A);
        }
    }
}
