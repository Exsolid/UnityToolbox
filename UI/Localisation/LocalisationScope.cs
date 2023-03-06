using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
