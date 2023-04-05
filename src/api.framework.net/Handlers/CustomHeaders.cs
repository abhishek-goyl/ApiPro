using Newtonsoft.Json;
using api.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Constants = api.framework.net.Lib.Constants;

namespace api.framework.net.Handlers
{
    public class CustomHeaders : DelegatingHandler
    {
        /// <summary>
        /// MessageHandler for adding custom respnse headers
        /// </summary>
        /// <param name="request">HttpRequestMessage - request</param>
        /// <param name="cancellationToken">CancellationToken - calcel token</param>
        /// <returns></returns>
        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            DateTime time = DateTime.Now;
            string requestId, traceId;
            // setting up the requets headers
            SetupRequestHeaders(out requestId, out traceId);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            // setting up custom response headers
            SetupResponseHeaders(time, requestId, traceId, request, response);
            return response;
        }

        /// <summary>
        /// Method to setup the custom response headers
        /// </summary>
        /// <param name="time">DateTime - start time of the request</param>
        /// <param name="requestId">string - unique requestid sent by the client</param>
        /// <param name="traceId">string - unique trace id managed by the API</param>
        /// <param name="response">HttpResponseMessage - response message for the api request</param>
        private void SetupResponseHeaders(DateTime time, string requestId, string traceId, HttpRequestMessage request, HttpResponseMessage response)
        {
            try
            {
                response.Headers.Add(Constants.SERVER_TIME_RESPONSE_HEADER, DateTime.Now.Subtract(time).TotalMilliseconds.ToString());
                response.Headers.Add(Constants.TRACE_ID_RESPONSE_HEADER_NAME, traceId);

                // get the response headers from request headers
                foreach (var h in request.Headers.Select(h => h.Key).Where(k => k.StartsWith("X-")))
                {
                    response.Headers.Add(h, request.Headers.GetValues(h).FirstOrDefault());
                }
            }
            catch { }
        }

        /// <summary>
        /// Method to set the custom request headers
        /// </summary>
        /// <param name="request">HttpRequestMessage - current api request message</param>
        /// <param name="requestId">string - unique requestid sent by the client</param>
        /// <param name="traceId">string - unique trace id managed by the API</param>
        private void SetupRequestHeaders(out string requestId, out string traceId)
        {
            traceId = requestId = Constants.DEFAULT_HEADER_VALUE;
            try
            {
                var request = HttpContext.Current.Request;
                requestId = request.Headers.AllKeys.Contains(Constants.REQUEST_ID_HEADER_NAME) ? request.Headers.GetValues(Constants.REQUEST_ID_HEADER_NAME).FirstOrDefault() : Constants.DEFAULT_HEADER_VALUE;
                traceId = Guid.NewGuid().ToString();
                
                List<LogEvent> events = new List<LogEvent>();
                var LogEvent = JsonConvert.SerializeObject(events);
                request.Headers.Add(api.logging.Constants.LOG_EVENTS_HEADER_NAME, LogEvent);
                request.Headers.Add(Constants.TRACE_ID_HEADER_NAME, traceId);
            }
            catch { }
        }
    }
}