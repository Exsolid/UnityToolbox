using System;

namespace UnityToolbox.UI.Localization
{
    /// <summary>
    /// The language which is used to indentify and serialize Localization data.
    /// </summary>
    [Serializable]
    public struct LocalizationLanguage
    {
        public string ShortName;
        public string Name;

        [NonSerialized]
        public static char DEVIDER = '_';

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(LocalizationLanguage)))
            {
                LocalizationLanguage other = (LocalizationLanguage)obj;
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
