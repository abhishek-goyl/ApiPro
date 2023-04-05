using api.framework.net.Lib.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace api.framework.net.Test
{
    [TestClass]
    public class WebApiApplicationTest : WebApiApplication
    {
        [TestMethod]
        public void Application_Start_Test()
        {
            // cannot be mocked
        }

        [TestMethod]
        public void Application_Error_Test()
        {
            var mockSender = new Mock<object>();
            var mockArgs = new Mock<EventArgs>();
            this.Application_Error(mockSender.Object, mockArgs.Object);
        }
    }
}
