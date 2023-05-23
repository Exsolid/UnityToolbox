using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// The scope which is used to indentify and serialize item data.
/// </summary>
[Serializable]
public class ItemScope
{
    public string Name;

    public override bool Equals(object obj)
    {
        if (obj.GetType().Equals(typeof(ItemScope)))
        {
            ItemScope other = (ItemScope)obj;
            return other.Name == Name;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
