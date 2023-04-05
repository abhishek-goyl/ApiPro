using api.framework.net.ExceptionLib;
using api.framework.net.Lib.Models;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace api.framework.net.Test.Helpers
{
    public class ApiExceptionFilterHelper
    {
        public static HttpActionExecutedContext Setup(ExceptionScenario scenario)
        {
            HttpActionExecutedContext context = new HttpActionExecutedContext();
            switch (scenario)
            {
                case ExceptionScenario.AppException:
                    return getExceptionContext("AppException");
                case ExceptionScenario.BadRequestException:
                    return getExceptionContext("BadRequestException");
                case ExceptionScenario.exception_with_inner_exception:
                    return getExceptionContext("NestedException");
                case ExceptionScenario.MethodNotSupportedException:
                    return getExceptionContext("MethodNotSupportedException");
                case ExceptionScenario.normal_exception:
                    return getExceptionContext("Exception");
                case ExceptionScenario.NoRouteFoundException:
                    return getExceptionContext("NoRouteFoundException");
                case ExceptionScenario.UnAuthorizedException:
                    return getExceptionContext("UnAuthorizedException");
            }
            return context;
        }
        
        private static HttpActionExecutedContext getExceptionContext(string type)
        {
            HttpActionContext aContext = ApiAuthorizationTestHelper.SetupAuthFilterContext(AuthorizationScenarios.with_valid_auth_header);
            Exception ex;
            switch (type)
            {
                case "AppException":
                    ex = new AppException("test");
                    break;
                case "BadRequestException":
                    ex = new BadRequestException("test");
                    break;
                case "NestedException":
                    ex = new Exception("test", new Exception());
                    break;
                case "MethodNotSupportedException":
                    ex = new MethodNotSupportedException("test");
                    break;
                case "Exception":
                    ex = new Exception();
                    break;
                case "NoRouteFoundException":
                    ex = new NoRouteFoundException("test");
                    break;
                case "UnAuthorizedException":
                    ex = new UnAuthorizedException("test");
                    break;
                default:
                    ex = new Exception();
                    break;
            }
            HttpActionExecutedContext context = new HttpActionExecutedContext(aContext, ex);
            return context;
        }
    }

    public enum ExceptionScenario
    {
        normal_exception,
        exception_with_inner_exception,
        AppException,
        BadRequestException,
        UnAuthorizedException,
        MethodNotSupportedException,
        NoRouteFoundException
    }
}
