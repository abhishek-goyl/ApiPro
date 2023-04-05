using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace api.framework.net.Lib.Test.Providers
{
    [TestClass]
    public class ApiSchemaProviderTest
    {
        Mock<IConfiguration> configProvider;
        ApiSchemaProvider schemaProvider;

        [TestInitialize]
        public void Setup()
        {
            configProvider = new Mock<IConfiguration>();
            schemaProvider = new ApiSchemaProvider(configProvider.Object);
        }
        

        [TestCleanup]
        public void Cleanup()
        {
            configProvider = null;
            schemaProvider = null;
        }
    }
}
