using System;
using System.IO;
using System.Web;
using System.Web.Http.Dispatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using api.framework.net.Handlers;
using api.framework.net.Test.Helpers;

namespace api.framework.net.Test.Handlers
{
    [TestClass]
    public class CustomHeadersTest : CustomHeaders
    {

        [TestMethod]
        public void SendAsync()
        {
            // setup 
            this.InnerHandler = new HttpControllerDispatcher(new System.Web.Http.HttpConfiguration());
            var context = ApiAuthorizationTestHelper.SetupAuthFilterContext(AuthorizationScenarios.with_valid_auth_header);

            // act
            var testk = this.SendAsync(context.Request, new System.Threading.CancellationToken());
        }

        [TestMethod]
        [DataRow("null")]
        [DataRow("valid")]
        public void SendAsync_withRequest(string scenario)
        {
            try
            {
                // setup 
                //validReq = validReqWithReqId = validReqWithLogData = new HttpRequest("test", "http://localhost/api/v2/organization/tree1/141", "");
                //validReqWithReqId.Headers.Add(Constants.REQUEST_ID, "test");
                //List<ApiEvent> logs = new List<ApiEvent>();
                //validReqWithLogData.Headers.Add(Constants.LOG_EVENTS_HEADER_NAME, logs.ToString());
                this.InnerHandler = new HttpControllerDispatcher(new System.Web.Http.HttpConfiguration());
                var context = ApiAuthorizationTestHelper.SetupAuthFilterContext(AuthorizationScenarios.with_valid_auth_header);
                switch (scenario)
                {
                    case "null":
                        HttpContext.Current = new HttpContext(null, new HttpResponse(new StringWriter()));
                        break;
                    case "valid":
                        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));
                        break;
                }

                // act
                var testk = this.SendAsync(context.Request, new System.Threading.CancellationToken());
            }
            catch (Exception ex)
            { }
        }
    }
}
