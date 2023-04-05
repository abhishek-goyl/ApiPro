using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using api.framework.net.Lib.Providers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace api.framework.net.Lib.Test.Helpers
{
    public class RequestProviderTestHelper
    {
        public enum Scenario_getInputs
        {
            none,
            api_endpoint_with_query_input_valid,
            api_endpoint_with_path_input_valid,
            api_endpoint_with_header_input_valid,
            api_endpoint_with_body_input_valid,
            api_endpoint_with_body_file_input_valid
        }

        static Mock<IConfiguration> _mockConfigProvider;
        static Mock<ISqlDataProvider> _mockSqlProvider;

        public static RequestProvider Setup_getInputs(Scenario_getInputs scenario, Mock<ISqlDataProvider> sqlProvider, Mock<IConfiguration> configuration, ref ApiEndpoint endpoint, ref HttpControllerContext context, ref string output)
        {
            _mockConfigProvider = configuration;
            _mockSqlProvider = sqlProvider;
            switch (scenario)
            {
                case Scenario_getInputs.api_endpoint_with_header_input_valid:
                    return InitializeForHeaderData(true, true, ref endpoint, ref context, ref output);
                case Scenario_getInputs.api_endpoint_with_path_input_valid:
                    return InitializeForPathData(true, true, ref endpoint, ref context, ref output);
                case Scenario_getInputs.api_endpoint_with_query_input_valid:
                    return InitializeForQueryData(true, true, ref endpoint, ref context, ref output);
                case Scenario_getInputs.api_endpoint_with_body_input_valid:
                    return InitializeForBodyData(true, true, ref endpoint, ref context, ref output);
                default:
                    return InitializeForQueryData(true, true, ref endpoint, ref context, ref output);
            }
        }

        private static RequestProvider InitializeForBodyData(bool hasData, bool isValidData, ref ApiEndpoint endpoint, ref HttpControllerContext context, ref string output)
        {  
            string data = @"{'RootNodeId':'1'}";
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            HttpContextBaseTest webHttpContext = new HttpContextBaseTest(data);

            context = new HttpControllerContext
            {
                RouteData = new HttpRouteData(
                    route: new HttpRoute(),
                    values: new HttpRouteValueDictionary { { "id", output } }
                )
            };
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("http://localhost/v2/organization");
           
            request.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            context.Request = request;
            context.Request.Method = HttpMethod.Post;
            context.Request.Properties["MS_HttpContext"] = webHttpContext;

            endpoint = new ApiEndpoint
            {
                inputs = new List<ApiInput> { new ApiInput { name = "RootNodeId", source = ApiInputSources.body, type = ApiInputType.@string } }
            };

            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema.json");
            RequestProvider requestProvider = new RequestProvider(_mockSqlProvider.Object, _mockConfigProvider.Object);

            return requestProvider;
        }

        private static RequestProvider InitializeForPathData(bool hasData, bool isValidData, ref ApiEndpoint endpoint, ref HttpControllerContext context, ref string output)
        {
            output = "123";
            // setup refs
            endpoint = new ApiEndpoint { inputs = new List<ApiInput> { new ApiInput { source = ApiInputSources.path, name = "id" } } };
            context = new HttpControllerContext
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/v2/organization/tree?qinput=123")
                },
                RouteData = new HttpRouteData(
                    route: new HttpRoute(),
                    values: new HttpRouteValueDictionary { { "id", output } }
                )
            };

            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema.json");
            RequestProvider requestProvider = new RequestProvider(_mockSqlProvider.Object, _mockConfigProvider.Object);

            return requestProvider;
        }

        private static RequestProvider InitializeForQueryData(bool hasData, bool isValidData, ref ApiEndpoint endpoint, ref HttpControllerContext context, ref string output)
        {
            output = "123";
            // setup refs
            endpoint = new ApiEndpoint { inputs = new List<ApiInput> { new ApiInput { source = ApiInputSources.query, name = "id" } } };
            
            context = new HttpControllerContext
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/v2/organization/tree?id=" + output)
                },
                RouteData = new HttpRouteData(
                    route: new HttpRoute(),
                    values: new HttpRouteValueDictionary { }
                )
            };

            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema.json");
            RequestProvider requestProvider = new RequestProvider(_mockSqlProvider.Object, _mockConfigProvider.Object);

            return requestProvider;
        }

        private static RequestProvider InitializeForHeaderData(bool hasData, bool isValidData, ref ApiEndpoint endpoint, ref HttpControllerContext context, ref string output)
        {
            output = "123";
            // setup refs
            endpoint = new ApiEndpoint { inputs = new List<ApiInput> { new ApiInput { source = ApiInputSources.header, name = "id" } } };
            HttpRequestMessage req = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/v2/organization/tree"),

            };
            req.Headers.Add("id", output);
            context = new HttpControllerContext
            {
                Request = req,
                RouteData = new HttpRouteData(
                    route: new HttpRoute(),
                    values: new HttpRouteValueDictionary { }
                )
            };

            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema.json");
            RequestProvider requestProvider = new RequestProvider(_mockSqlProvider.Object, _mockConfigProvider.Object);

            return requestProvider;
        }


    }
}
