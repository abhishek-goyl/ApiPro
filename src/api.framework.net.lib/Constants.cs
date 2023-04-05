using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib
{
    public class Constants
    {
        public const string AUTHORIZATION = "Authorization";
        public const string IGNORE_AUTHORIZATION_CHECK = "iac";
        public const string LOG_EVENTS_HEADER_NAME = "logEvents";
        public const string REQUEST_ID_HEADER_NAME = "X-Correlation-Id";
        public const string TRACE_ID_HEADER_NAME = "X-TraceId";
        public const string DEFAULT_HEADER_VALUE = "NA";

        public const string SERVER_TIME_RESPONSE_HEADER = "X-Server-Time";
        public const string TRACE_ID_RESPONSE_HEADER_NAME = "X-TraceId";

        public const string NLOG_CONFIG_SUMMARY_NAME = "summary";
        public const string NLOG_CONFIG_REQUEST_NAME = "request";
        public const string NLOG_CONFIG_EXCEPTION_NAME = "error";

        public const string HTTP_CONTEXT = "MS_HttpContext";
        public const string HTTP_RESPONSE_STATUS_CODE = "HttpStatusCode";
        public const string MOCK = "mock";
        public const string MOCK_ACTION = "mock";
        public const string MOCK_DATA_STORE = "mockDataPath";
        public const string MOCK_SCENARIO = "mock_scenario";
        public const string ISSUER = "OAuthServer";

        public const string CONFIG_VERSION = "version";

        public const string PUBLIC_KEY_DB_COLUMN = "PublicKey";
        public const string PUBLIC_KEY_CACHE_EXP = "KeyCacheExp";


        public const string DEFAULT_SQL_TIMEOUT = "1800";

        public const string FORM_DATA = "multipart/form-data";
        public const string APPLICATION_JSON = "application/json";

        public const string XMLTYPE = "xml";

        public const string DEFAULT_ROWINDEX_COLUMN = "RowIndex";
        public static readonly string[] SUPPORTED_DATE_PATTERNS = {"dd/MMM/yyyy", "d/MM/yyyy" , "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy","dd/M/yy",
        "dd-MMM-yyyy","dd-MMM-yyyy", "d-MM-yyyy" , "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy","dd-M-yy","yyyy-MM-dd","MM/dd/yyyy HH:mm","MM/dd/yyyy HH:mm:ss","MM.dd.yyyy HH:mm",
        "MM-dd-yyyy H:mm","MM.dd.yyyy HH:mm:ss","d/MMM/yyyy","d-MMM-yyyy","MM-dd-yyyy","MM.dd.yyyy","d.MMM.yyyy","dd.MMM.yyyy", "d.MM.yyyy" , "dd.MM.yyyy", "d.M.yyyy",
        "dd.M.yyyy","dd.M.yy","MM/dd/yyyy","dd MMMM yyyy","MMMM dd","dd MMM yyy","yyyy MMMM","M/d/yyyy","M/d/yy","MM/dd/yy","yy/MM/dd","dd-MMM-yy","d/M/yy",
         "d-M-yy","dd/MM/yy","dd-MM-yy"};
    }
}

