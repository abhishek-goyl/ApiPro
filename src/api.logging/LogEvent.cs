using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace api.logging
{
    public sealed class LogEvent
    {
        /// <summary>
        /// Gets or Sets the start time of event
        /// </summary>
        [JsonProperty]
        private DateTime start { get; set; }

        /// <summary>
        /// Gets or Sets the method name of the event
        /// </summary>
        [JsonProperty]
        private string methodName { get; set; }

        /// <summary>
        /// Gets or Sets the Full Class Name of the event
        /// </summary>
        [JsonProperty]
        private string className { get; set; }

        /// <summary>
        /// Gets or Sets the duration of event in milliseconds
        /// </summary>
        [JsonProperty]
        public double duration { get; set; }

        /// <summary>
        /// Gets or Sets the debug information for the event
        /// </summary>
        [JsonProperty]
        private List<string> details { get; set; } = new List<string>();

        [JsonProperty]
        public bool isDBCall { get; set; } = false;

        /// <summary>
        /// Gets or Sets the status of the event (true if successfull and false in case of error)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "status")]
        private bool Status { get; set; } = true;

        /// <summary>
        /// Gets or Sets the error message in case there was some error
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
        private string Error { get; set; }

        /// <summary>
        /// Private constructor so that it could not be initialized from outside of the class
        /// </summary>
        private LogEvent()
        { }

        /// <summary>
        /// Method to create the instance of ApiEvent and populate the start properties
        /// </summary>
        /// <returns></returns>
        public static LogEvent Start()
        {
            LogEvent log = new LogEvent();
            try
            {
                log.start = DateTime.UtcNow;
                log.methodName = GetCurrentMethod();
                log.className = GetCurrentClassName();
            }
            catch { }
            return log;
        }

        /// <summary>
        /// Method to populate the exit properties and update the object in state
        /// </summary>
        public void Exit(bool db = false)
        {
            try
            {
                this.isDBCall = db;
                this.duration = DateTime.UtcNow.Subtract(this.start).TotalMilliseconds;
                Store();
            }
            catch { }
        }

        /// <summary>
        /// Method to log the exception and update the object in the state
        /// </summary>
        /// <param name="ex"></param>
        public void LogError(Exception ex)
        {
            try
            {
                this.Status = false;
                this.Error = ex.Message;
                LogData log = new LogData();
                LogManager.WriteExceptiontLog(log, ex);
            }
            catch { }
        }

        /// <summary>
        /// Method to add the debug information for he event
        /// </summary>
        /// <param name="message">string - debug message</param>
        public void Debug(string message)
        {
            try
            {
                this.details.Add(JsonConvert.SerializeObject(new { time = DateTime.UtcNow, message }));
            }
            catch { }
        }



        #region Private Methods

        /// <summary>
        /// Method to get the current method name
        /// </summary>
        /// <returns>string - Method name</returns>
        private static string GetCurrentMethod()
        {
            string method = "NA";
            try
            {
                var st = new StackTrace();
                var sf = st.GetFrame(2);
                return sf.GetMethod().Name;
            }
            catch { }
            return method;
        }

        /// <summary>
        /// Method to get the current file 
        /// </summary>
        /// <returns>string - File name</returns>
        private static string GetCurrentClassName()
        {
            string file = "NA";
            try
            {
                var st = new StackTrace();
                var sf = st.GetFrame(2);
                return sf.GetMethod().DeclaringType.FullName;
            }
            catch { }
            return file;
        }

        /// <summary>
        /// Method to update the current event in the request header store
        /// </summary>
        private void Store()
        {
            List<LogEvent> events = new List<LogEvent>();
            var logs = HttpContext.Current.Request.Headers[Constants.LOG_EVENTS_HEADER_NAME];
            if (!string.IsNullOrEmpty(logs))
            {
                events = JsonConvert.DeserializeObject<List<LogEvent>>(logs);
            }
            events.Add(this);
            logs = JsonConvert.SerializeObject(events);
            HttpContext.Current.Request.Headers.Remove(Constants.LOG_EVENTS_HEADER_NAME);
            HttpContext.Current.Request.Headers.Add(Constants.LOG_EVENTS_HEADER_NAME, logs);
        }

        #endregion

    }
}
