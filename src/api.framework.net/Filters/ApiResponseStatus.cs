using api.framework.net.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace api.framework.net.Filters
{
    public class ApiResponseStatus : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {   
            base.OnActionExecuted(actionExecutedContext);
            var statusCode = HttpContext.Current.Items[Constants.HTTP_RESPONSE_STATUS_CODE] ?? 200;
            int code = 200;
            int.TryParse(statusCode.TryParseString(), out code);
            if (actionExecutedContext.Response != null)
            {
                actionExecutedContext.Response.StatusCode = (System.Net.HttpStatusCode)code;
            }
        }
    }
}