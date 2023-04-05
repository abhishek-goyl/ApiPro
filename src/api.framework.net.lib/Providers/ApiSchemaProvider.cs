using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Text.RegularExpressions;

namespace api.framework.net.Lib.Providers
{
    public sealed class ApiSchemaProvider : IApiSchemaProvider
    {
        private readonly IConfiguration _configProvider;
        public static string SwaggerContent = string.Empty;

        public ApiSchemaProvider(IConfiguration configuration)
        {
            _configProvider = configuration;
        }

        private static ApiSchema _schema;

        public static void LoadSchema(IConfiguration _configProvider)
        {
            var schemaFile = _configProvider.GetValue<string>("schemaFile");
            Debug.WriteLine(schemaFile);
            schemaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, schemaFile);
            string schemaDir = Path.GetDirectoryName(schemaFile);
            
            string definationsDir = Path.Combine(schemaDir, "definations");
            DirectoryInfo dDi = new DirectoryInfo(definationsDir);

            // load schema
            string allText = File.ReadAllText(schemaFile);
            if (!string.IsNullOrEmpty(allText))
            {
                _schema = JsonConvert.DeserializeObject<ApiSchema>(allText);
            }
            List<ApiEndpoint> lstEndpoints = GetEndpoints(schemaDir);


            // load definations
            List<JObject> definations = new List<JObject>();
            if (dDi.Exists)
            {
                foreach (FileInfo fi in dDi.GetFiles("*.json"))
                {
                    string dTxt = File.ReadAllText(fi.FullName);
                    JObject def = dTxt.ToJObject();
                    definations.Add(def);
                }
            }
            definations.ForEach(d => _schema.definations.Merge(d, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            }));

            _schema.endpoints.AddRange(lstEndpoints);
        }

        private static List<ApiEndpoint> GetEndpoints(string schemaDir)
        {
            List<ApiEndpoint> lstEndpoints = new List<ApiEndpoint>();
            string endpointsDir = Path.Combine(schemaDir, "endpoints");
            DirectoryInfo opDi = new DirectoryInfo(endpointsDir);
            if (opDi.Exists)
            {
                // read all opertaions
                foreach (FileInfo fi in opDi.GetFiles("*.json"))
                {
                    string epTxt = File.ReadAllText(fi.FullName);
                    ApiEndpoint endpoint = JsonConvert.DeserializeObject<ApiEndpoint>(epTxt);
                    lstEndpoints.Add(endpoint);
                }
            }
            return lstEndpoints;
        }

        public ApiSchema Schema
        {
            get
            {
                return _schema;
            }
        }

        #region IApiSchemaProvider Members
        public ApiSchema GetSchema()
        {
            return Schema;
        }

        public List<ApiEndpoint> GetApiEndpointsByName(string name, string operation, string version, HttpControllerContext context = null)
        {
            List<ApiEndpoint> selected = new List<ApiEndpoint>();
            ApiSchema schema = GetSchema();
            selected = (from op in schema.endpoints
                        where op.tag.ToLower().Equals(name?.ToString()?.ToLower()) && op.name.ToLower().Equals(operation?.ToString()?.ToLower()) && op.version.ToLower().Equals(version?.ToString()?.ToLower())
                        select op).ToList();
            return selected;
        }

        public ApiEndpoint CheckHttpVerb(string method, List<ApiEndpoint> endpoints)
        {
            return endpoints.Find(o => o.verb.ToString().ToLower().Equals(method.ToString().ToLower()));
        }

        public Dictionary<string, List<HttpVerb>> GetAllPathsAndHttpVerbs()
        {
            ApiSchema schema = this.GetSchema();
            Dictionary<string, List<HttpVerb>> res = new Dictionary<string, List<HttpVerb>>();
            var paths = schema?.endpoints?.Select(e => e.path).Distinct().ToList();
            paths?.ForEach(p => {
                var verbs = schema?.endpoints?.Where(e => p.Equals(e.path)).Select(o => o.verb).ToList();
                res.Add(p, verbs);
            });
            return res;
        }

        public ApiEndpoint GetApiEndpointsBySwaggerPath(string path, HttpVerb verb)
        {
            List<ApiEndpoint> endpoints = _schema?.endpoints?.FindAll(e => e.path.Equals(path, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return endpoints.Find(e => e.verb.Equals(verb));
        }

        /// <summary>
        /// Method to generate the Swagger document used by swagger UI
        /// </summary>
        public void GenerateSwaggerDocument()
        {
            LogEvent log = LogEvent.Start();
            string swaggerData = string.Empty;
            try
            {
                var swaggerFile = _configProvider.GetValue<string>("swaggerFile");
                swaggerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, swaggerFile);
                ISwaggerDocProvider _swagger = new SwaggerDocProvider(this);
                var doc = _swagger.GetSwaggerDocument(this.Schema);
                swaggerData = JsonConvert.SerializeObject(doc);
                File.WriteAllText(swaggerFile, swaggerData);
            }
            catch(Exception ex)
            {
                log.LogError(ex);
                SwaggerContent = swaggerData;
            }
        }

        /// <summary>
        /// Method to get the name of swagger schema for the endpoint
        /// </summary>
        /// <param name="endpoint">ApiEndpoint - schema endpoint</param>
        /// <returns>JObject - swagger definition of 200 response</returns>
        public List<string> GetResponseProperties(ApiEndpoint endpoint)
        {
            LogEvent log = LogEvent.Start();
            List<string> response = new List<string>{ "Errors" };
            try
            {
                ApiSchema schema = GetSchema();
                var allResponses = endpoint.responses;
                var schemaRef = allResponses["200"].schema["$ref"].ToString();
                log.Debug(string.Format("response schema is: '{0}'", schemaRef));
                response.Add(schemaRef.Replace("#/definations/", ""));
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
            return response;
        }

        public List<Tuple<string, string>> GetResponseHeaders(ApiEndpoint endpoint)
        {
            LogEvent log = LogEvent.Start();
            List<Tuple<string, string>> headers = new List<Tuple<string, string>>();
            try
            {
                ApiSchema schema = GetSchema();
                var allResponses = endpoint.responses;
                var allHeaders = allResponses["200"].headers;
                if (allHeaders != null)
                {
                    foreach (var h in allHeaders)
                    {
                        headers.Add(new Tuple<string, string>(h.Key, h.Value.map));
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
            return headers;
        }

        #endregion
    }
}
