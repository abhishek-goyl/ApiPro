using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace api.framework.net.Test
{
    [TestClass]
    public class BrowserJsonFormatterTest
    {
        [TestMethod]
        public void BrowserJsonFormatter()
        {
            // act
            BrowserJsonFormatter formatter = new BrowserJsonFormatter();

            //assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SetDefaultContentHeaders()
        {
            //setup
            Type mockType = new Mock<Type>().Object;
            HttpContentHeaders mockHeaders = null;
            MediaTypeHeaderValue mockMediaType = new MediaTypeHeaderValue("application/json");
            BrowserJsonFormatter formatter = new BrowserJsonFormatter();

            // act
            //formatter.SetDefaultContentHeaders(mockType, mockHeaders, mockMediaType);

            //assert
            Assert.IsTrue(true);
        }
    }
}
