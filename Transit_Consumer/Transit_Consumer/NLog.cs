using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Transit_Consumer
{
    public class LoggingClass
    {
        private Logger nLoggerFileOnly = LogManager.GetLogger("LogToFileOnly");
        private Logger nLoggerConsoleFile = LogManager.GetLogger("LogToConsoleAndFile");

        public void Log(string TextValue)
        {
            //write to text file only
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, "LogToConsoleAndFile", TextValue);

            logEvent.Properties["Name"] = "Log";

            nLoggerConsoleFile.Log(logEvent);

            logEvent = null;
            TextValue = null;
        }

        public void LogToFile(string TextValue, string FileNameVal)
        {
            //write to text file only
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, "LogToFileOnly", TextValue);

            logEvent.Properties["Name"] = FileNameVal;

            nLoggerFileOnly.Log(logEvent);

            logEvent = null;
            TextValue = null;
        }

        public void LogError(Exception eErr)
        {
            //will log as an error in the error_ log file
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Error, "LogToFileOnly", eErr.ToString());

            logEvent.Properties["Name"] = "errorLog";
            nLoggerFileOnly.Log(logEvent);

            logEvent = null;
        }

        public void LogErrorText(string sErr)
        {
            //will log as an error in the error_ log file
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Error, "LogToConsoleAndFile", sErr);

            logEvent.Properties["Name"] = "Log";
            nLoggerConsoleFile.Log(logEvent);

            logEvent = null;
        }

       
    }
}
