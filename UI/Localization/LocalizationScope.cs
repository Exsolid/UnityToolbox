using System;

namespace UnityToolbox.UI.Localization
{
    /// <summary>
    /// The scope which is used to indentify and serialize Localization data.
    /// </summary>
    [Serializable]
    public struct LocalizationScope
    {
        public string Name;

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(LocalizationScope)))
            {
                LocalizationScope other = (LocalizationScope)obj;
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
