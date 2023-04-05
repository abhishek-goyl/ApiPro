using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Test.Providers
{
    [TestClass]
    public class BusinessProviderTest
    {
        BusinessProvider businessProvider;

        [TestInitialize]
        public void Setup()
        {
            businessProvider = new BusinessProvider();
        }


        [TestCleanup]
        public void Cleanup()
        {
            businessProvider = null;
        }
    }
}
