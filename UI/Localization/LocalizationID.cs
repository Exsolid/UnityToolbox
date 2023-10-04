using System;

namespace UnityToolbox.UI.Localization
{
    /// <summary>
    /// An ID which is used to indentify and serialize Localization data.
    /// </summary>
    [Serializable]
    public struct LocalizationID
    {
        public LocalizationScope Scope;
        public string Name;

        [NonSerialized]
        public static char DEVIDER = '_';

        /// <summary>
        /// Creates and returns the name in syntax of <see cref="LocalizationScope.Name"/> + <see cref="DEVIDER"/> + <see cref="Name"/>
        /// </summary>
        /// <returns>A qualified name.</returns>
        public string GetQualifiedName()
        {
            return Scope.Name + DEVIDER + Name;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(LocalizationID)))
            {
                LocalizationID other = (LocalizationID)obj;
                return other.GetQualifiedName().Equals(GetQualifiedName());
            }
            else if (obj.GetType().Equals(typeof(string)))
            {
                return GetQualifiedName().Equals((string)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return GetQualifiedName().GetHashCode();
        }
    } 
}
