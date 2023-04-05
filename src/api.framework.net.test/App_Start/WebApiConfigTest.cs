using api.framework.net.Lib.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace api.framework.net.Test.App_Start
{
    [TestClass]
    public class WebApiConfigTest
    {
        [TestMethod]
        public void Register()
        {
            // setup
            var config = new Mock<HttpConfiguration>();
            var mockApiConfig = new Mock<IConfiguration>();
            mockApiConfig.Setup<string>(m => m.GetValue<string>("schemaFile")).Returns("Data\\valid_schema_default.json");
            mockApiConfig.Setup<string>(m => m.GetValue<string>("swaggerFile")).Returns("Data\\valid_schema_default.json"); 
            WebApiConfig.appConfig = mockApiConfig.Object;

            //act
            WebApiConfig.Register(config.Object);
        }
    }
}
