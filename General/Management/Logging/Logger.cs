using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityToolbox.General.Management.Logging
{
    public class Logger
    {
        private const string identityHash = "b0dc9699-7181-472b-abd4-efd617e60dab";
        public static string CONTEXTCOLOR = "#8055ed";
        public static string ERRCOLOR = "#ba2b34";
        public static string WARCOLOR = "#c9c432";
        public static string INFCOLOR = "#32a852";
        public static void Log(LogLevel level, Type callCOntext, string message)
        {
            if (!Debug.unityLogger.logHandler.GetType().Equals(typeof(ToolboxLogHandler)))
            {
                new ToolboxLogHandler();
            }

            string logMessage = "[" + level.ToString() + "] [" + callCOntext.Name + "] " + message.Trim();
            Debug.Log(identityHash+BuildLogColor(level, callCOntext.Name, message));
        }

        public static string BuildLog(LogLevel level, string callCOntext, string message)
        {
            if (message.Contains("</color>"))
            {
                message = message.Replace("</color>", "");
            }
            while (message.Contains("<color="))
            {
                int index = message.IndexOf("<color=");
                message = message.Remove(index, "<color=".Length + WARCOLOR.Length + ">".Length);
            }
            return "[" + level.ToString() + "] [" + callCOntext + "] " + message.Trim();
        }

        public static string BuildLogUncolor(string message)
        {
            if (message.Contains("</color>"))
            {
                message = message.Replace("</color>", "");
            }
            while (message.Contains("<color="))
            {
                int index = message.IndexOf("<color=");
                message = message.Remove(index, "<color=".Length + WARCOLOR.Length + ">".Length);
            }
            return message.Trim();
        }

        public static string BuildLogColor(LogLevel level, string callCOntext, string message)
        {
            string levelColor = "";
            switch (level)
            {
                case LogLevel.INF:
                    levelColor = "#32a852";
                    break;
                case LogLevel.WAR:
                    levelColor = "#c9c432";
                    break;
                case LogLevel.ERR:
                    levelColor = "#ba2b34";
                    break;
            }
            return "[<color=" + levelColor + ">" + level.ToString() + "</color>] [<color="+ CONTEXTCOLOR + ">" + callCOntext + "</color>] " + message.Trim();
        }
    }
}
