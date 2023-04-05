using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.logging
{
    public class Constants
    {
        public const string LOG_EVENTS_HEADER_NAME = "log";
        public const string REQUEST_ID_HEADER_NAME = "X-Correlation-Id";
        public const string TRACE_ID_HEADER_NAME = "X-TraceId";

        public const string NLOG_REQUEST_CONFIG_NAME = "request";
        public const string NLOG_SUMMARY_CONFIG_NAME = "summary";
        public const string NLOG_EXCEPTION_CONFIG_NAME = "error";

        public const string DEFAULT_HEADER_VALUE = "NA";
    }
}
