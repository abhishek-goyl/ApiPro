using api.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace api.framework.net
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();
            LogData log = new LogData();
            LogManager.WriteExceptiontLog(log, error);
        }
    }
}
