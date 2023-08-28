using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityToolbox.UI.Localisation
{
    /// <summary>
    /// The language which is used to indentify and serialize localisation data.
    /// </summary>
    [Serializable]
    public struct LocalisationLanguage
    {
        public string ShortName;
        public string Name;

        [NonSerialized]
        public static char DEVIDER = '_';

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(LocalisationLanguage)))
            {
                LocalisationLanguage other = (LocalisationLanguage)obj;
                return other.ShortName + DEVIDER + other.Name == ShortName + DEVIDER + Name;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    } 
}
