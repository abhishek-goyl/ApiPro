using api.framework.net.Controllers;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Providers;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace api.framework.net.Test.Helpers
{
    public class CommonControllerPostHelper
    {
        static Mock<IRequestProvider> _mockRequestProvider = new Mock<IRequestProvider>(); 
        static Mock<IApiSchemaProvider> _mockSchemaProvider = new Mock<IApiSchemaProvider>();
        static Mock<ISqlDataProvider> _mockSqlProvider = new Mock<ISqlDataProvider>();
        static Mock<IConfiguration> _mockConfigProvider = new Mock<IConfiguration>();

        public enum TestScenario
        {
            none,
            api_with_valid_post_body,
            api_with_valid_post_form,
            api_with_valid_post_form_file,
        }

        public static CommonController Initialize(string scenario)
        {
            TestScenario sc = TestScenario.none;
            Enum.TryParse<TestScenario>(scenario, out sc);
            switch (sc)
            {
                case TestScenario.api_with_valid_post_body:
                    return InitializeForBody();
                default:
                    break;  
            }
            return new CommonController(_mockRequestProvider.Object, _mockSchemaProvider.Object);
        }

        private static CommonController InitializeForBody()
        {
            _mockRequestProvider.Setup<ApiEndpoint>(x => x.GetCurrentSchemaEndpoint(It.IsAny<HttpControllerContext>(), null)).Returns(new ApiEndpoint());
            _mockRequestProvider.Setup<List<ApiInput>>(x => x.getInputs(It.IsAny<ApiEndpoint>(), It.IsAny<HttpControllerContext>())).Returns(new List<ApiInput>());
            _mockRequestProvider.Setup<List<ApiError>>(x => x.validateInputs(It.IsAny<List<ApiInput>>())).Returns(new List<ApiError>());
            _mockRequestProvider.SetupGet<object>(x => x.ResponseData).Returns(new JObject());
            CommonController controller = new CommonController(_mockRequestProvider.Object, _mockSchemaProvider.Object);
            return controller;
        }
        
    }
}
