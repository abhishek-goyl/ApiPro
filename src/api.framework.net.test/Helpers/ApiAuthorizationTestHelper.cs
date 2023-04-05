using Moq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace api.framework.net.Test.Helpers
{
    public class ApiAuthorizationTestHelper
    {
        public static HttpActionContext SetupAuthFilterContext(AuthorizationScenarios scenarios)
        {
            switch (scenarios)
            {
                case AuthorizationScenarios.with_valid_auth_header:
                    return GetContextWithOAuth();
                case AuthorizationScenarios.with_invalid_auth_header:
                    return GetContextWithOutOAuth();
                case AuthorizationScenarios.without_auth_header:
                    return GetContextWithOutOAuth();
            }
            return new HttpActionContext();
        }
        private static HttpActionContext GetContextWithOAuth()
        {
            try
            {
                HttpControllerContext cContext = new HttpControllerContext();
                cContext.Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/v2/organization/tree/141")
                };
                cContext.Request.Headers.Add(Constants.AUTHORIZATION_HEADER, Constants.AUTHORIZATION_MOCK_VALID_TOKEN);
                cContext.Request.Headers.Add(Constants.REQUEST_ID, Constants.REQUEST_ID_VALUE);
                var actionDescriptor = new Mock<HttpActionDescriptor>() { CallBase = true }.Object;
                return new HttpActionContext(cContext, actionDescriptor);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return null;
        }

        private static HttpActionContext GetContextWithOutOAuth()
        {
            try
            {
                HttpControllerContext cContext = new HttpControllerContext();
                cContext.Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/v2/organization/tree/141")
                };
                var actionDescriptor = new Mock<HttpActionDescriptor>() { CallBase = true }.Object;
                return new HttpActionContext(cContext, actionDescriptor);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return null;
        }

    }

    public enum AuthorizationScenarios
    {
        with_valid_auth_header,
        with_invalid_auth_header,
        without_auth_header
    }
}
