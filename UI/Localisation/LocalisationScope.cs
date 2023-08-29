using System;

namespace UnityToolbox.UI.Localisation
{
    /// <summary>
    /// The scope which is used to indentify and serialize localisation data.
    /// </summary>
    [Serializable]
    public struct LocalisationScope
    {
        public string Name;

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(LocalisationScope)))
            {
                LocalisationScope other = (LocalisationScope)obj;
                return other.Name == Name;
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
