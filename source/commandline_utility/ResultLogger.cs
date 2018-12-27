using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace utility
{
    static class ResultLogger
    {
        private const int InformationEventID = 1000;

        public static void LogSuccess(string message)
        {
            AddToLog(message);
        }

        public static void LogServiceGenericFault(System.ServiceModel.FaultException exception)
        {
            AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
        }

        public static void LogCommunicationIssue(System.ServiceModel.CommunicationException exception)
        {
            AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
        }

        public static void LogGenericException(Exception exception)
        {
            if (exception.Data == null)
            {
                Environment.FailFast("Process is corrupt: exception.Data is null.", exception);
            }
            else
            {
                AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
            }
        }

        private static void AddToLog(string message)
        {
            string logtext = string.Format("{0:s} [{1}] {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
            System.Diagnostics.Trace.WriteLine(logtext);
        }

        private static void AddToLog(string method, Exception exception)
        {
            int eventId = 0;
            System.Diagnostics.Trace.WriteLine(string.Format("Log method: {0} EventID: {1}. Message: {2}", method, eventId, exception.Message));
        }
    }
}