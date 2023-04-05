using api.logging;
using api.framework.net.Lib.Contracts;
using System;
using System.Dynamic;
using System.Web.Http;
using Constants = api.framework.net.Lib.Constants;
using System.IO;

namespace api.framework.net.Controllers
{
    public class HealthController : ApiController
    {
        IConfiguration _config { get; set; }
        public HealthController(IConfiguration config)
        {
            this._config = config;
        }

        [HttpGet]
        public object Index()
        {
            LogEvent log = LogEvent.Start();
            dynamic res = new ExpandoObject();
            var version = string.Empty;
            try
            {
                version = getVersion();
                res.status = "healthy";   
            }
            catch(Exception ex)
            {
                res.status = "unhealthy";
                res.error = ex;
                log.LogError(ex);
            }
            finally
            {
                res.app = "Api NoCode Framework";
                res.version = version;
                log.Exit();
            }
            return res;
        }

        private string getVersion()
        {
            string version = "NA";
            try
            {
                version = _config.GetValue<string>(Constants.CONFIG_VERSION);

                var vfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.txt");
                var bfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "build.txt");

                var vtext = File.ReadAllText(vfile).Trim();
                var btext = File.ReadAllText(bfile).Trim();

                version = string.Format("{0}.{1}", vtext, btext);
            }
            catch { }
            return version;
        }
    }
}
