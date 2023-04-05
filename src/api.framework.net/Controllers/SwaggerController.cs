using api.framework.net.Lib.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;
using api.framework.net.Lib.Providers;

namespace api.framework.net.Controllers
{
    public class SwaggerController : ApiController
    {
        IConfiguration _config;

        public SwaggerController(IConfiguration configProvider)
        {
            _config = configProvider;
        }

        [HttpGet]
        public object Index()
        {
            var data = string.Empty;
            if (!string.IsNullOrEmpty(ApiSchemaProvider.SwaggerContent))
            {
                data = ApiSchemaProvider.SwaggerContent;
            }
            else
            {
                var swaggerFile = _config.GetValue<string>("swaggerFile");
                swaggerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, swaggerFile);
                data = File.ReadAllText(swaggerFile);
            }
            var controller = ControllerContext.Controller.GetType().Name.ToLower().Replace("controller", string.Empty);
            var reqUrl = Request.RequestUri.OriginalString;
            var host = Request.RequestUri.Host;
            var basePath = reqUrl.Substring(0, reqUrl.ToLower().IndexOf(controller));
            data = data.Replace("{0}", host);
            data = data.Replace("{1}", basePath);
            return JObject.Parse(data);
        }
    }
}
