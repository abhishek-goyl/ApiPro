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
using System.Web.Http.Routing;

namespace api.framework.net.Test.Helpers
{
    public class CommonControllerHelper
    {
        static Mock<ISqlDataProvider> _mockSqlProvider;
        static Mock<IConfiguration> _mockConfigProvider;

        public enum TestScenario
        {
            none,
            api_with_path_data_valid_input,
            api_with_query_data_valid_input,
            api_with_header_data_valid_input,
            api_with_path_data_invalid_input,
            api_with_query_data_invalid_input,
            api_with_header_data_invalid_input,
            api_with_path_data_without_input,
            api_with_query_data_without_input,
            api_with_header_data_without_input,
            api_with_body_data_valid_input,
            api_with_form_data_valid_input,
            request_with_unavailable_action,
            request_with_unavailable_http_method,
            request_with_unknown_error
        }

        public static CommonController Initialize(string scenario, Mock<ISqlDataProvider> mockSqlProvider, Mock<IConfiguration> mockConfigProvider)
        {
            TestScenario sc = TestScenario.none;
            Enum.TryParse<TestScenario>(scenario, out sc);
            _mockSqlProvider = mockSqlProvider;
            _mockConfigProvider = mockConfigProvider;
            switch (sc)
            {
                case TestScenario.api_with_path_data_valid_input:
                    return InitializeForPathData(true, true);
                case TestScenario.api_with_path_data_invalid_input:
                    return InitializeForPathData(true, false);
                case TestScenario.api_with_path_data_without_input:
                    return InitializeForPathData(false, false);
                case TestScenario.api_with_query_data_valid_input:
                    return InitializeForQueryData(true, true);
                case TestScenario.api_with_query_data_invalid_input:
                    return InitializeForQueryData(true, false);
                case TestScenario.api_with_query_data_without_input:
                    return InitializeForQueryData(false, false);
                case TestScenario.api_with_header_data_valid_input:
                    return InitializeForHeaderData(true, true);
                case TestScenario.api_with_header_data_invalid_input:
                    return InitializeForHeaderData(true, false);
                case TestScenario.api_with_header_data_without_input:
                    return InitializeForHeaderData(false, false);
                case TestScenario.request_with_unavailable_action:
                    return InitializeForInvalidAction();
                case TestScenario.request_with_unavailable_http_method:
                    return InitializeForInvalidHttpVerb();
                case TestScenario.request_with_unknown_error:
                    return InitializeForUnknownError();
                case TestScenario.api_with_body_data_valid_input:
                    return IntializeForBodyValidInput();
                case TestScenario.api_with_form_data_valid_input:
                    return IntializeForFormValidInput();
                default:
                    break;  
            }
            return new CommonController(mockConfigProvider.Object, mockSqlProvider.Object);
        }

        private static CommonController IntializeForFormValidInput()
        {
            // Arrange
            DataSet dataset = new DataSet();
            dataset.Tables.Add(new DataTable());
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_body.json");
            _mockSqlProvider.Setup(m => m.ExecuteProcedure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(dataset);
            CommonController controller = new CommonController(_mockConfigProvider.Object,_mockSqlProvider.Object);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("http://localhost/v2/organization");

            var multipart = new MultipartFormDataContent();
            var data = new JsonContent(new { pageno = "1", pagesize = 2 });
            var stringC = new StringContent(JsonConvert.SerializeObject(data));
           
          //  HttpPostedFile p = new HttpPostedFile();
           // p.InputStream
                multipart.Add(stringC);
            multipart.Add(new ByteArrayContent(File.ReadAllBytes("C:\\Users\\shilpa.gupta\\Downloads\\KPI_Data_Import_Old.xlsx")));
            // multipart.Add(new HttpPostedFile());
            request.Content = multipart;
            controller.Request = request;
            controller.Configuration = new HttpConfiguration();

            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}",
                defaults: new { id = RouteParameter.Optional });

            controller.RequestContext.RouteData = new HttpRouteData(
               route: new HttpRoute(),
               values: new HttpRouteValueDictionary { { "tag", "organization" }, { "version", "v2" }, { "name", "tree" } }
           );         

            var claims = new List<Claim> { new Claim("RootOrganizationId", "141"), new Claim("HighestNodeUniqueId", "197_1") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            controller.RequestContext.Principal = claimsPrincipal;

            return controller;
        }

        private static CommonController IntializeForBodyValidInput()
        {
            // Arrange
            DataSet dataset = new DataSet();
            dataset.Tables.Add(new DataTable());
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_body.json");
            _mockSqlProvider.Setup(m => m.ExecuteProcedure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(dataset);
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            var data = new JsonContent(new { pageno ="1", pagesize=2 });
            var stringC = new StringContent(JsonConvert.SerializeObject(data));
            request.RequestUri = new Uri("http://localhost/v2/organization");
            request.Content = data;
            controller.Request = request;         
            controller.Configuration = new HttpConfiguration();

            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}",
                defaults: new { id = RouteParameter.Optional });

            controller.RequestContext.RouteData = new HttpRouteData(
               route: new HttpRoute(),
               values: new HttpRouteValueDictionary { { "tag", "organization" }, { "version", "v2" }, { "name", "tree" } }
           );

            var claims = new List<Claim> { new Claim("RootOrganizationId", "141"), new Claim("HighestNodeUniqueId", "197_1") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            controller.RequestContext.Principal = claimsPrincipal;

            return controller;      
        }

        private static CommonController InitializeForUnknownError()
        {
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_path.json");
            
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            var mockSchemaProvider = new Mock<IApiSchemaProvider>();
            mockSchemaProvider.Setup<object>(m => m.GetApiEndpointsByName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Throws(new Exception());
            controller._schemaProvider = mockSchemaProvider.Object;
            return controller;
        }

        private static CommonController InitializeForInvalidHttpVerb()
        {
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema_with_only_post.json");
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/v2/organization/tree/141"),
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v2/organization/tree/{id}");

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "id", "141" } }
            );

            return controller;
        }

        private static CommonController InitializeForInvalidAction()
        {
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_path.json");
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/v2/organization/tree1/141")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v2/organization/tree1/{id}");

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { {"id",  "141"},{ "tag", "organization" },{"version","v2" } } 
            );

            var claims = new List<Claim> { new Claim("RootOrganizationId", "141"), new Claim("HighestNodeUniqueId", "197_1") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            controller.RequestContext.Principal = claimsPrincipal; 
   
            return controller;
        }

        public static CommonController InitializeForPathData(bool hasData, bool isValidData)
        {
            DataSet dataset = new DataSet();
            dataset.Tables.Add(new DataTable());
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_path.json");
            _mockSqlProvider.Setup(m => m.ExecuteProcedure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(dataset);
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            controller.Request = new  HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/v2/organization/tree/141")  
                
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v2/organization/tree/{id}");

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: hasData ? new HttpRouteValueDictionary { { "id", isValidData ? "141" : "a" }, { "tag", "organization" }, { "version", "v2" },{ "name","tree"} } : new HttpRouteValueDictionary { }
            );
            var claims = new List<Claim> { new Claim("RootOrganizationId", "141"), new Claim("HighestNodeUniqueId", "197_1") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            controller.RequestContext.Principal = claimsPrincipal;
            return controller;
        }

        public static CommonController InitializeForQueryData(bool hasData, bool isValidData)
        {
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_query.json");
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(hasData ? (isValidData ? "http://localhost/api/v2/organization/tree?id=141" : "http://localhost/api/v2/organization/tree?id=a") : "http://localhost/api/v2/organization/tree")
            };
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary {  } 
            );
            return controller;
        }

        public static CommonController InitializeForHeaderData(bool hasData, bool isValidData)
        {
            _mockConfigProvider.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_header.json");
            CommonController controller = new CommonController(_mockConfigProvider.Object, _mockSqlProvider.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/v2/organization/tree")
            };
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { }
            );
            if (hasData)
            {
                controller.Request.Headers.Add("id", isValidData ? "141" : "a");
            }
            controller.Configuration = new HttpConfiguration();
            
            return controller;
        }

    }
}
