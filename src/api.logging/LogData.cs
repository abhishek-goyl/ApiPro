using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;


namespace api.logging
{
    public class LogData : ICloneable
    {
        /// <summary>
        /// Gets or Sets the time 
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// Gets or Sets the log type
        /// </summary>
        public string logType { get; set; } = LogType.summary.ToString();

        /// <summary>
        /// Gets or Sets the http verb (Method) of the request
        /// </summary>
        public string httpVerb { get; set; }

        /// <summary>
        /// Gets or Sets the request id (Unique id of request send by client)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string requestId { get; set; }

        /// <summary>
        /// Gets or Sets the trace id (Unique request id managed by the API)
        /// </summary>
        public string traceId { get; set; }

        /// <summary>
        /// Gets or Sets the request Url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Gets or Sets the request path
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string path { get; set; }

        /// <summary>
        /// Gets or Sets the request path
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string userAgent { get; set; }

        /// <summary>
        /// Gets or Sets the Http response status code
        /// </summary>
        public int httpStatus { get; set; }

        /// <summary>
        /// Gets or Sets the time take by request
        /// </summary>
        public double duration { get; set; }

        /// <summary>
        /// Gets or Sets the time take by db operations
        /// </summary>
        public double dbduration { get; set; }

        /// <summary>
        /// Gets or Sets the request client ip
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string clientIp { get; set; }

        /// <summary>
        /// Gets or Sets the server machine name 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string machineName { get; set; }

        /// <summary>
        /// Gets or Sets the server machine IP address 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string serverIP { get; set; }

        /// <summary>
        /// Gets or Sets the downstream events
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<LogEvent> events { get; set; }

        public LogData()
        {
            this.time = DateTime.UtcNow;
            PopulateRequestData();
        }

        public LogData SummaryLog()
        {
            LogData logData = (LogData)this.Clone();
            logData.logType = LogType.summary.ToString();
            return logData;
        }

        public LogData RequestLog()
        {
            LogData logData = (LogData)this.Clone();
            logData.logType = LogType.request.ToString();
            logData.events = null;
            return logData;
        }

        private void PopulateRequestData()
        {
            try
            {
                var request = HttpContext.Current.Request;
                this.httpVerb = request.HttpMethod;
                this.url = request.RawUrl;
                this.path = request.Path;
                this.clientIp = GetIp();
                this.machineName = Environment.MachineName;
                this.serverIP = GetServerIP();
                this.requestId = request.Headers[Constants.REQUEST_ID_HEADER_NAME];
                this.traceId = request.Headers[Constants.TRACE_ID_HEADER_NAME];
            }
            catch
            {
                // Supressing the exception in the logic for logging
            }
        }

        private string GetServerIP()
        {
            try
            {
                var hostName = System.Net.Dns.GetHostName();
                return System.Net.Dns.GetHostEntry(hostName).AddressList[0]?.ToString();
            }
            catch { return string.Empty; }
        }

        private string GetIp()
        {
            string ips = string.Empty;
            try
            {
                List<string> ipsList = new List<string>();
                var request = HttpContext.Current.Request;
                ipsList.Add(request.UserHostAddress);
                ipsList.Add(request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                ipsList.Add(request.ServerVariables["REMOTE_ADDR"]);
                ipsList = ipsList.Distinct().ToList<string>();
                ipsList.RemoveAll(x => string.IsNullOrEmpty(x));
                ips = string.Join(",", ipsList);
                ips = ips.Replace("::1", "127.0.0.1");
            }
            catch { }
            return ips;

        }

        public void Populate(HttpResponseMessage response)
        {
            try
            {
                List<LogEvent> events = new List<LogEvent>();
                var logs = HttpContext.Current.Request.Headers[Constants.LOG_EVENTS_HEADER_NAME];
                if (!string.IsNullOrEmpty(logs))
                {
                    events = JsonConvert.DeserializeObject<List<LogEvent>>(logs);
                }
                List<double> dbEvents = events.Where(e => e.isDBCall).Select(e => e.duration).ToList();
                this.dbduration = GetDBTime(dbEvents);
                this.events = events;
                this.duration = DateTime.UtcNow.Subtract(this.time).TotalMilliseconds;
                this.httpStatus = (int)response.StatusCode;
            }
            catch { }
        }


        private double GetDBTime(List<double> times)
        {
            double duration = 0;
            try {
                times.ForEach(d => duration += d);
            }
            catch {  }
            return duration;
        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public enum LogType
    {
        request,
        summary,
        error
    }
}
