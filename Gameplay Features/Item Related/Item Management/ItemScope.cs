using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemScope : MonoBehaviour
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
        return base.GetHashCode();
    }
}
