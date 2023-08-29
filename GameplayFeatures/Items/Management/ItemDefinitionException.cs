using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.Item.Management  
{
    public class ItemDefinitionException : Exception
    {
        public ItemDefinitionException(string message) : base(message)
        {

        }
    }
}
