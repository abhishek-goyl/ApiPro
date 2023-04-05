using api.framework.net.Filters;
using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using api.framework.net.Lib.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.IO;
using System.Web;
using api.framework.net.Lib;
using api.framework.net.ExceptionLib;

namespace api.framework.net.Controllers
{
    public class MockController : ApiController
    {
        public IMockSqlProvider _mockProvider;
        public MockController(IConfiguration _config)
        {
            _mockProvider = new MockSqlProvider(_config);
        }

        [HttpPost]
        [ApiExceptionFilter]
        public ApiMockResponse Index(string sp, string sc)
        {
            ApiMockResponse res = new ApiMockResponse();
            LogEvent log = LogEvent.Start();
            try
            {
                string str = string.Empty;
                var httpContext = (HttpContextBase)ControllerContext.Request.Properties["MS_HttpContext"];
                httpContext.Request.InputStream.Position = 0;

                using (StreamReader streamReader = new StreamReader(httpContext.Request.InputStream))
                {
                    str = streamReader.ReadToEnd();
                }
                
                var data = JsonConvert.DeserializeObject<DataSet>(str);
                res = _mockProvider.AddStoredProcedureMock(sp, sc, data);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return res;
        }

       
        [HttpDelete]
        [ApiExceptionFilter]
        public ApiMockResponse Index(string sp, string sc, string action)
        {
            ApiMockResponse res = new ApiMockResponse();
            LogEvent log = LogEvent.Start();
            try
            {
                res = _mockProvider.DeleteStoredProcedureMock(sp, sc);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return res;
        }
    }
}
