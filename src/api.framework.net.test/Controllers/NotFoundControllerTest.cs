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
    public class NotFoundControllerTest
    {
        NotFoundController controller;

        [TestInitialize]
        public void TestSetup()
        {
            controller = new NotFoundController();
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
            controller = null;
        }


    }
}
