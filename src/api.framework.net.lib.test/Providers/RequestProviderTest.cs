using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Providers;
using api.framework.net.Lib.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http.Controllers;
using static api.framework.net.Lib.Test.Helpers.RequestProviderTestHelper;

namespace api.framework.net.Lib.Test.Providers
{
    [TestClass]
    public class RequestProviderTest
    {
        private Mock<ISqlDataProvider> _dataHelper;
        private Mock<IConfiguration> _configuration;
        private RequestProvider reqProvider;

        [TestInitialize]
        public void Setup()
        {
            
            _dataHelper = new Mock<ISqlDataProvider>();
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\schema.json");
            reqProvider = new RequestProvider(_dataHelper.Object, _configuration.Object);
        }

        [TestMethod]
        [DataRow("api_endpoint_with_query_input_valid", false)]
        [DataRow("api_endpoint_with_path_input_valid", false)]
        [DataRow("api_endpoint_with_header_input_valid", false)]
        public void getInputs(string scenario, bool isError)
        {
            string expectedOutput = string.Empty;
            // setup
            Scenario_getInputs sc = Scenario_getInputs.none;
            Enum.TryParse<Scenario_getInputs>(scenario, out sc);
            ApiEndpoint endpoint = new ApiEndpoint();
            HttpControllerContext context = new HttpControllerContext(); 
            reqProvider = RequestProviderTestHelper.Setup_getInputs(sc, _dataHelper, _configuration, ref endpoint, ref context, ref expectedOutput);

            // act
            List<ApiInput> inputs = reqProvider.getInputs(endpoint, context);

            //Assert
            if (!isError)
            {
                Assert.AreEqual(inputs.Count, endpoint.inputs.Count);
                Assert.AreEqual(inputs[0].value, expectedOutput);
            }
            else
            {
                Assert.ThrowsException<Exception>(() => reqProvider.getInputs(endpoint, context));
            }
        }

        [TestMethod]
        [DataRow("api_endpoint_with_body_input_valid", false)]
        [DataRow("api_endpoint_with_body_file_input_valid", false)]
        public void getInputs_post(string scenario, bool isError)
        {
            string expectedOutput = string.Empty;
            // setup
            Scenario_getInputs sc = Scenario_getInputs.none;
            Enum.TryParse<Scenario_getInputs>(scenario, out sc);
            ApiEndpoint endpoint = new ApiEndpoint();
            HttpControllerContext context = new HttpControllerContext();
            reqProvider = RequestProviderTestHelper.Setup_getInputs(sc, _dataHelper, _configuration, ref endpoint, ref context, ref expectedOutput);

            // act
            List<ApiInput> inputs = reqProvider.getInputs(endpoint, context);

            //Assert
            if (!isError)
            {
                Assert.AreEqual(inputs.Count, endpoint.inputs.Count);
                Assert.IsNotNull(inputs[0].value.ToJSONString(), expectedOutput.ToJSONString());
            }
            else
            {
                Assert.ThrowsException<Exception>(() => reqProvider.getInputs(endpoint, context));
            }
        }



        [TestCleanup]
        public void Cleanup()
        {
            _dataHelper = null;
            _configuration = null;
            reqProvider = null;
        }
    }
}
