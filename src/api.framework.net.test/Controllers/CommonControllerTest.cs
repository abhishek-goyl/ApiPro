using api.framework.net.Controllers;
using api.framework.net.ExceptionLib;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Providers;
using api.framework.net.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace api.framework.net.Test.Controllers
{
    [TestClass]
    public class CommonControllerTest
    {
        Mock<IConfiguration> configProvider;
        Mock<ISqlDataProvider> sqlProvider;
        CommonController controller;

        [TestInitialize]
        public void TestSetup()
        {
            sqlProvider = new Mock<ISqlDataProvider>();
            configProvider = new Mock<IConfiguration>();
        }


        [TestMethod]
        [DataRow("api_with_path_data_valid_input")]
        [DataRow("api_with_query_data_valid_input")]
        [DataRow("api_with_header_data_valid_input")]
        public void IndexGET_Success(string scenario)
        {
            controller = CommonControllerHelper.Initialize(scenario, sqlProvider, configProvider);
            ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index();
            Assert.IsNotNull(res);
        }

        [TestMethod]
        [DataRow("api_with_path_data_invalid_input")]
        [DataRow("api_with_query_data_invalid_input")]
        [DataRow("api_with_header_data_invalid_input")]
        [DataRow("api_with_path_data_without_input")]
        [DataRow("api_with_query_data_without_input")]
        [DataRow("api_with_header_data_without_input")]
        [ExpectedException(typeof(BadRequestException))]
        public void IndexGET_BadRequest(string scenario)
        {
            controller = CommonControllerHelper.Initialize(scenario, sqlProvider, configProvider);
            ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index();
        }

        [TestMethod]
        [DataRow("request_with_unavailable_action")]
        [ExpectedException(typeof(NoRouteFoundException))]
        public void IndexGET_NoRouteFoundException(string scenario)
        {
            controller = CommonControllerHelper.Initialize(scenario, sqlProvider, configProvider);
            ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index();
        }

        [TestMethod]
        [DataRow("request_with_unavailable_http_method")]
        [ExpectedException(typeof(MethodNotSupportedException))]
        public void IndexGet_MethodNotSupportedException(string scenario)
        {
            controller = CommonControllerHelper.Initialize(scenario, sqlProvider, configProvider);
            ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index();
        }

        [TestMethod]
        [DataRow("request_with_unknown_error")]
        [ExpectedException(typeof(AppException))]
        public void IndexGet_Exception(string scenario)
        {
            controller = CommonControllerHelper.Initialize(scenario, sqlProvider, configProvider);
            ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index();
        }

        [TestMethod]
        [DataRow("api_with_valid_post_body")]
        public void IndexPost(string scenario)
        {
            controller = CommonControllerPostHelper.Initialize(scenario);
            //ApiSchemaProvider.LoadSchema(configProvider.Object);
            var res = controller.Index("");
            Assert.IsNotNull(res);
        }
        [TestCleanup]
        public void Cleanup()
        {
            sqlProvider = null;
            configProvider = null;
        }
    }
}
