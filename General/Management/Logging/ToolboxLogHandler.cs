using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityToolbox.General.Management;
using Logger = UnityToolbox.General.Management.Logging.Logger;

namespace UnityToolbox.General.Management.Logging
{
    public class ToolboxLogHandler : ILogHandler
    {
        private StreamWriter m_StreamWriter;
        private ILogHandler m_DefaultLogHandler = Debug.unityLogger.logHandler;

        private const string identityHash = "b0dc9699-7181-472b-abd4-efd617e60dab";

        private string _logPrefix;
        public ToolboxLogHandler()
        {
            if (Application.isBatchMode)
            {
                _logPrefix = "Server";
            }
            else
            {
                _logPrefix = "Client";
            }
            string filePath = Application.persistentDataPath + "/"+ _logPrefix+"Logs.txt";
            m_StreamWriter = new StreamWriter(filePath, true);
            m_StreamWriter.AutoFlush = true;
            CleanUpLogs();
            if (!Application.isEditor)
            {
                m_StreamWriter.WriteLine("[" + GetTimestamp(DateTime.Now) + "] " + Logger.BuildLog(LogLevel.INF, "ToolboxLogHandler", "--------------------------------------------LOGGING INIT--------------------------------------------"));
            }
            // Replace the default debug log handler
            Debug.unityLogger.logHandler = this;
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            CleanUpLogs();
            string log = args[0].ToString();
            if (log.Contains(identityHash))
            {
                //TODO save in game files
                m_DefaultLogHandler.LogFormat(logType, context, log.Split(identityHash).LastOrDefault(), args);
                if (!Application.isEditor)
                {
                    m_StreamWriter.WriteLine("[" + GetTimestamp(DateTime.Now) + "] " + Logger.BuildLogUncolor(log.Split(identityHash).LastOrDefault()));
                }
                return;
            }

            LogLevel level = LogLevel.ERR;
            switch (logType)
            {
                case LogType.Error:
                    level = LogLevel.ERR;
                    break;
                case LogType.Assert:
                    level = LogLevel.ERR;
                    break;
                case LogType.Warning:
                    level = LogLevel.WAR;
                    break;
                case LogType.Log:
                    level = LogLevel.INF;
                    break;
                case LogType.Exception:
                    level = LogLevel.ERR;
                    break;
            }
            if (!Application.isEditor)
            {
                //TODO edit log
                m_StreamWriter.WriteLine("[" + GetTimestamp(DateTime.Now) + "] " + Logger.BuildLog(level, context == null ? "Unknown" : context.name, log));
            }
            //TODO save in game files
            m_DefaultLogHandler.LogFormat(logType, context, Logger.BuildLogColor(level, context == null ? "Unknown" : context.name, log), args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            Exception ex = new Important(Logger.BuildLogColor(LogLevel.ERR, exception.StackTrace.Split('\r').First().Split('\\').Last().Split('.').First().Trim(), exception.Message));
            Exception nextException = exception;

            while (true)
            {
                if (nextException != null)
                {
                    ex = new Important(nextException.Message + "\n" + nextException.StackTrace, ex);
                    nextException = nextException.InnerException;
                    continue;
                }
                else
                {
                    break;
                }
            }
            m_StreamWriter.WriteLine("[" + GetTimestamp(DateTime.Now) + "] " + Logger.BuildLog(LogLevel.ERR, context == null ? "Unknown" : context.name, ex.Message));
            m_DefaultLogHandler.LogException(ex, context);
            CleanUpLogs();
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public void CleanUpLogs()
        {
            string filePath = Application.persistentDataPath + "/" + _logPrefix + "Logs.txt";
            string filePathOld = Application.persistentDataPath + "/" + _logPrefix + "Logs_prev.txt";
            long length = new System.IO.FileInfo(filePath).Length;
            if (length != 0)
            {
                length /= 1048576;
            }

            if (length > 10)
            {
                m_StreamWriter.Close();
                if (File.Exists(filePathOld))
                {
                    File.Delete(filePathOld);
                }
                System.IO.File.Move(filePath, filePathOld);
                m_StreamWriter = new StreamWriter(filePath, true);
                m_StreamWriter.AutoFlush = true;
            }
        }
    }
}