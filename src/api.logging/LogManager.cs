using Newtonsoft.Json;
using NLog;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.logging
{
    public static class LogManager
    {
        public enum LogType
        {
            request,
            summary,
            error
        }

        private static Logger RequestLogger = NLog.LogManager.GetLogger(Constants.NLOG_REQUEST_CONFIG_NAME);
        private static Logger SummaryLogger = NLog.LogManager.GetLogger(Constants.NLOG_SUMMARY_CONFIG_NAME);
        private static Logger exceptionLogger = NLog.LogManager.GetLogger(Constants.NLOG_EXCEPTION_CONFIG_NAME);


        public static void WriteLogs(HttpRequestMessage request, LogData log)
        {
            try
            {
                // fire and forget call for logging summary logs
                Task.Run(() => {
                    WriteSummaryLog(log);
                    WriteRequestLog(request, log);
                });
            }
            catch { }
        }

        public static void WriteErrors(Exception ex, LogData log)
        {
            try
            {
                Task.Run(() => WriteExceptiontLog(log, ex));
            }
            catch { }
        }

        private static Logger GetLogger(LogType type)
        {
            switch (type)
            {
                case LogType.error:
                    return exceptionLogger;
                case LogType.request:
                    return RequestLogger;
                case LogType.summary:
                default:
                    return SummaryLogger;

            }
        }


        private static bool WriteSummaryLog(LogData log)
        {
            try
            {
                Logger summaryLogger = GetLogger(LogType.summary);
                log = log.SummaryLog();
                string message = JsonConvert.SerializeObject(log);
                summaryLogger.Info(message);
                //Debug.WriteLine(message);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    WriteExceptiontLog(log, ex);
                }
                catch { }
            }
            return false;
        }

        public static void WriteRequestLog(HttpRequestMessage request, LogData log)
        {
            try
            {
                Logger requestLogger = GetLogger(LogType.request);
                log = log.RequestLog();
                log.userAgent = request.Headers.UserAgent != null ? request.Headers.UserAgent.ToString() : Constants.DEFAULT_HEADER_VALUE;
                string message = JsonConvert.SerializeObject(log);
                requestLogger.Info(message);
                //Debug.WriteLine(message);
            }
            catch (Exception ex)
            {
                try
                {
                    WriteExceptiontLog(log, ex);
                }
                catch { }
            }
        }

        public static void WriteExceptiontLog(LogData log, Exception ex)
        {
            try
            {
                Logger requestLogger = GetLogger(LogType.error);
                dynamic error = new ExpandoObject();
                error.time = log.time;
                error.traceId = log.traceId;
                error.requestId = log.requestId;
                error.message = ex.Message;
                error.excetion = ex;
                string message = JsonConvert.SerializeObject(error);
                requestLogger.Info(message);
                //Debug.WriteLine(message);
            }
            catch { }
        }
    }
}
