using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace api.framework.net.Lib.Contracts
{
    public interface IApiSchemaProvider
    {
        ApiSchema GetSchema();
        List<ApiEndpoint> GetApiEndpointsByName(string name, string operation, string version, HttpControllerContext context = null);

        ApiEndpoint CheckHttpVerb(string method, List<ApiEndpoint> endpoints);

        Dictionary<string, List<HttpVerb>> GetAllPathsAndHttpVerbs();

        ApiEndpoint GetApiEndpointsBySwaggerPath(string path, HttpVerb verb);

        void GenerateSwaggerDocument();

        List<string> GetResponseProperties(ApiEndpoint endpoint);

        List<Tuple<string, string>> GetResponseHeaders(ApiEndpoint endpoint);

    }
}
