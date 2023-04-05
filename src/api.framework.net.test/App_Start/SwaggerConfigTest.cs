using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Test.App_Start
{
    [TestClass]
    public class SwaggerConfigTest
    {
        [TestMethod]
        public void Register()
        {
            SwaggerConfig.Register();
        }
    }
}
