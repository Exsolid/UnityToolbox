using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.General.Management.Logging
{
    public class Important : Exception
    {
        public Important(string message) : base(message)
        {
        }
        public Important(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
