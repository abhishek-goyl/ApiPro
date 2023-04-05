using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Providers;
using api.framework.net.Lib.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using Newtonsoft.Json.Linq;

namespace api.framework.net.Lib.Test.Providers
{
    [TestClass]
   public class ExcelProviderTest
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


        [TestCleanup]
        public void Cleanup()
        {
            _dataHelper = null;
            _configuration = null;
        }
    }
}
