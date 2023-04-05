using api.framework.net.Filters;
using api.framework.net.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Http.Filters;

namespace api.framework.net.Test.Filters
{
    [TestClass]
    public class ApiExceptionFilterTest
    {
        [TestMethod]
        [DataRow("normal_exception")]
        [DataRow("exception_with_inner_exception")]
        [DataRow("AppException")]
        [DataRow("BadRequestException")]
        [DataRow("UnAuthorizedException")]
        [DataRow("MethodNotSupportedException")]
        [DataRow("NoRouteFoundException")]
        public void OnException(string scenario)
        {
            // setup 
            ExceptionScenario eScenero = (ExceptionScenario)Enum.Parse(typeof(ExceptionScenario), scenario);
            HttpActionExecutedContext context = ApiExceptionFilterHelper.Setup(eScenero);

            ApiExceptionFilter filter = new ApiExceptionFilter();

            // act
            filter.OnException(context);

            //assert
            Assert.IsTrue(true);
        }
    }
}
