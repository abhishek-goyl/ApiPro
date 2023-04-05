using api.framework.net.ExceptionLib;
using api.framework.net.Lib;
using api.framework.net.Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace api.framework.net.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            AppException aEx = actionExecutedContext.Exception as AppException;
            HttpResponseException wEx = actionExecutedContext.Exception as HttpResponseException;
            string exceptionMessage = actionExecutedContext.Exception.Message;
            string errorCode = aEx != null ? aEx.ErrorCode : wEx?.Response.StatusCode.ToString();
            //We can log this exception message to the file or database.
            HttpStatusCode status = HttpStatusCode.ServiceUnavailable;
            switch (actionExecutedContext.Exception.GetType().Name)
            {
                case "BadRequestException":
                    status = HttpStatusCode.BadRequest;
                    break;
                case "UnAuthorizedException":
                    status = HttpStatusCode.Unauthorized;
                    break;
                case "MethodNotSupportedException":
                    status = HttpStatusCode.MethodNotAllowed;
                    break;
                case "NoRouteFoundException":
                    status = HttpStatusCode.NotFound;
                    break;
                case "AppException":
                default:
                    exceptionMessage = "Technical error occurred. Please contact system administrator.";
                    if (wEx != null) status = wEx.Response.StatusCode;
                    else status = HttpStatusCode.InternalServerError;
                    break;
            }
            var data = actionExecutedContext.Exception.Data["res"];
            var resContent = new JsonContent(new ErrorResponse
            {
                errorCode = errorCode, errorMessage = exceptionMessage 
            });
            if (data != null)
            {
                ErrorResponse res = JsonConvert.DeserializeObject<ErrorResponse>(data.TryParseString());
                resContent = string.IsNullOrEmpty(res.errorMessage)? new JsonContent(JObject.Parse(data.TryParseString())):new JsonContent(res);
            }
            var response = new HttpResponseMessage(status)
            {
                Content = resContent
            };
            actionExecutedContext.Response = response;
        }

    }
}