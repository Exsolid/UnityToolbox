using System;

[Serializable]
public struct LocalisationID
{
    public LocalisationScope Scope;
    public string Name;

    [NonSerialized]
    public static char DEVIDER = '_';

    public override bool Equals(object obj)
    {
        if (obj.GetType().Equals(typeof(LocalisationID)))
        {
            LocalisationID other = (LocalisationID)obj;
            return other.Scope.Name + DEVIDER + other.Name == Scope.Name + DEVIDER + Name;
        }
        else if (obj.GetType().Equals(typeof(string)))
        {
            return Scope.Name + DEVIDER + Name == (string) obj;
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
