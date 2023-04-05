using api.framework.net.Controllers;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;

namespace api.framework.net.Test.Controllers
{
    [TestClass]
    public class SwaggerControllerTest
    {
        Mock<IConfiguration> configProvider;
        SwaggerController controller;

        [TestInitialize]
        public void TestSetup()
        {
            configProvider = new Mock<IConfiguration>();
            configProvider.Setup<string>(m => m.GetValue<string>("swaggerFile")).Returns("Data\\valid_schema_header.json");
            controller = new SwaggerController(configProvider.Object);
        }

        [TestMethod]
        public void Index()
        {
            var res = controller.Index();
            Assert.IsNotNull(res);
        }

        [TestCleanup]
        public void Cleanup()
        {
            configProvider = null;
            controller = null;
        }


    }
}
