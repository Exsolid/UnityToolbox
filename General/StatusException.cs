using System;

namespace UnityToolbox.General
{
    /// <summary>
    /// An exception used within tool windows to display a status.
    /// </summary>
    public class StatusException : Exception
    {
        public StatusException(string message) : base(message)
        {

        }
    } 
}
