using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public static class LogData
    {
        private static EventLog certLogger = new EventLog();
        private static EventLog serverLogger = new EventLog();

        public static void InitializeCMSEventLog()
        {
            string source = "CerificateEvents";
            string logName = "CertificateLog";

            try
            {
                if (!EventLog.SourceExists(source)) 
                {
                    EventLog.CreateEventSource(source, logName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            certLogger.Source = source;
            certLogger.Log = logName;
        }

        public static void InitializeServerEventLog()
        {
            string source = "ServerEvents";
            string logName = "ServerLog";

            try
            {
                if (!EventLog.SourceExists(source)) 
                {
                    EventLog.CreateEventSource(source, logName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            serverLogger.Source = source;
            serverLogger.Log = logName;
        }
        public static void WriteEntryCMS(string message, EventLogEntryType evntType, int eventID)
        {
            certLogger.WriteEntry(message, evntType, eventID);
        }

        public static void WriteEntryServer(string message, EventLogEntryType evntType, int eventID)
        {
            serverLogger.WriteEntry(message, evntType, eventID);
        }

    }
}
