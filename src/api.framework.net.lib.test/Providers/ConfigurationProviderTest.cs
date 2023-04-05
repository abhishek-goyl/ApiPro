using api.framework.net.Lib.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Test.Providers
{
    [TestClass]
    public class ConfigurationProviderTest
    {
        ConfigurationProvider configProvider;

        [TestInitialize]
        public void Setup()
        {
            configProvider = new ConfigurationProvider();
        }

        [TestMethod]
        [DataRow("key")]
        public void GetValue_Valid(string key)
        {
            var expectedValue = "test";
            var res = configProvider.GetValue<string>(key);
            Assert.AreEqual(res, expectedValue);
        }

        [TestMethod]
        [DataRow("InValidkey")]
        public void GetValue_InValid(string key)
        {
            var value = configProvider.GetValue<string>(key);
            Assert.IsNull(value);
        }


        [TestCleanup]
        public void Cleanup()
        {
            configProvider = null;
        }
    }
}
