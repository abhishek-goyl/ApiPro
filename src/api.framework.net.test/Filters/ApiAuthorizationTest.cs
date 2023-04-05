using api.framework.net.Filters;
using api.framework.net.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace api.framework.net.Test.Filters
{
    [TestClass]
    public class ApiAuthorizationTest
    {
        [TestMethod]
        [DataRow("with_valid_auth_header")]
        [DataRow("with_invalid_auth_header")]
        [DataRow("without_auth_header")]
        public void OnAuthorizationAsync(string scenario)
        {
            
            AuthorizationScenarios authScenario = (AuthorizationScenarios)Enum.Parse(typeof(AuthorizationScenarios), scenario);
            var context = ApiAuthorizationTestHelper.SetupAuthFilterContext(authScenario);

            ApiAuthorization filter = new ApiAuthorization();

            var res = filter.OnAuthorizationAsync(context, new System.Threading.CancellationToken());
            if (authScenario != AuthorizationScenarios.with_valid_auth_header)
            {
                Assert.IsNotNull(context.Response.Content);
            }
        }
    }
}
