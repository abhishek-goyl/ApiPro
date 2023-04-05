using api.logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace api.framework.net.Handlers
{
    public class ApiLoggingHandler : DelegatingHandler
    {
        /// <summary>
        /// MessageHandler for managing the API logs
        /// </summary>
        /// <param name="request">HttpRequestMessage - request</param>
        /// <param name="cancellationToken">CancellationToken - calcel token</param>
        /// <returns></returns>
        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogData log = new LogData();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            // capture the events for the request
            log.Populate(response);
            // call for logging
            LogManager.WriteLogs(request, log);
            return response;
        }
    }
}