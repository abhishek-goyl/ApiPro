using api.framework.net.Filters;
using api.logging;
using System.Dynamic;
using System.Web.Http;

namespace api.framework.net.Controllers
{
    public class NotFoundController : ApiController
    {
        [HttpGet]
        [ApiAuthorization]
        [ApiExceptionFilter]
        public object Index()
        {
            dynamic response = new ExpandoObject();
            LogEvent log = LogEvent.Start();
            try
            {
                response.Status = 404;
                response.Error = "requested route is not supported.";
            }
            finally
            {
                log.Exit();
            }
            return response;
        }
    }
}
