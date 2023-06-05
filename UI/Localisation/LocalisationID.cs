using System;

/// <summary>
/// An ID which is used to indentify and serialize localisation data.
/// </summary>
[Serializable]
public struct LocalisationID
{
    public LocalisationScope Scope;
    public string Name;

    [NonSerialized]
    public static char DEVIDER = '_';

    /// <summary>
    /// Creates and returns the name in syntax of <see cref="LocalisationScope.Name"/> + <see cref="DEVIDER"/> + <see cref="Name"/>
    /// </summary>
    /// <returns>A qualified name.</returns>
    public string GetQualifiedName()
    {
        return Scope.Name + DEVIDER + Name;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType().Equals(typeof(LocalisationID)))
        {
            LocalisationID other = (LocalisationID)obj;
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
