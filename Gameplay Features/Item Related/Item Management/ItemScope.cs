using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Item.Management
{
    /// <summary>
    /// The scope which is used to indentify and serialize item data.
    /// </summary>
    [Serializable]
    public class ItemScope
    {
        public string Name;

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
