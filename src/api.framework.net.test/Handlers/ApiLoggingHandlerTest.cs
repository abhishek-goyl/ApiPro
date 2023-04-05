using api.framework.net.Handlers;
using api.framework.net.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace api.framework.net.Test.Handlers
{
    [TestClass]
    public class ApiLoggingHandlerTest : ApiLoggingHandler
    {
        [TestMethod]
        public void SendAsync()
        {
            // setup 
            this.InnerHandler = new HttpControllerDispatcher(new System.Web.Http.HttpConfiguration());
            var context = ApiAuthorizationTestHelper.SetupAuthFilterContext(AuthorizationScenarios.with_valid_auth_header);

            // act
            var testk =  this.SendAsync(context.Request, new System.Threading.CancellationToken());
        }
    }
}
