using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace api.framework.net.Lib.Providers
{
    public class SwaggerDocProvider : ISwaggerDocProvider
    {
        private readonly IApiSchemaProvider schemaProvider;

        public SwaggerDocProvider(IApiSchemaProvider apiSchemaProvider)
        {
            schemaProvider = apiSchemaProvider;
        }

        public JObject GetSwaggerDocument(ApiSchema schema)
        {
            JObject swaggerDoc = new JObject();
            swaggerDoc = GetBaseSwaggerDocument(schema);
            var pathAndVerbs = schemaProvider.GetAllPathsAndHttpVerbs();
            JObject paths = new JObject();
            foreach (var path in pathAndVerbs.Keys)
            {
                JObject ops = new JObject();
                foreach (var verb in pathAndVerbs[path])
                {
                    dynamic operation = new ExpandoObject();
                    var verbs = pathAndVerbs[path];
                    ApiEndpoint endpoint = schemaProvider.GetApiEndpointsBySwaggerPath(path, verbs.FirstOrDefault());
                    try
                    {
                        operation.tags = GetSwaggerOperationTags(endpoint);
                        operation.summary = GetSwaggerOperationSummary(endpoint);
                        //operation.operationId = get
                        operation.consumes = GetSwaggerOperationConsumes(endpoint);
                        operation.produces = GetSwaggerOperationProduces(endpoint);
                        operation.parameters = GetSwaggerOperationParameters(endpoint);
                        operation.responses = GetSwaggerOperationResponses(endpoint);
                    }
                    catch { }
                    var o = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(operation));
                    ops.Add(verb.ToString().ToLower(), o);
                }
                JProperty p = new JProperty(path, ops);
                paths.Add(p);
            }
            swaggerDoc.Add("paths", paths);
            swaggerDoc.Add("definitions", GetSwaggerDefinations(schema));
            return swaggerDoc.ToJObject();
        }
        public JObject GetBaseSwaggerDocument(ApiSchema schema)
        {
            dynamic data = new ExpandoObject();
            data.swagger = "2.0";
            data.info = new { version = "v1", title = schema.name };
            // placeholder for host
            data.host = "{0}";
            // placeholder for basepath
            data.basePath = "{1}";
            data.schemes = schema.schemes;
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(data));
        }

        public JObject GetSwaggerDefinations(ApiSchema schema)
        {
            var definations = schema.definations;
            if (definations != null)
            {
                List<string> allModels = definations.Properties().Select(p => p.Name).ToList();
                foreach (string res in allModels)
                {
                    List<JToken> tokens = definations.SelectTokens(string.Format("$.{0}.properties", res)).ToList();
                    foreach (JObject t in tokens)
                    {
                        List<string> data = t.Properties().Select(p => p.Name).ToList();
                        foreach (string header in data)
                        {
                            JObject h = (JObject)definations.SelectToken(string.Format("{0}.{1}", t.Path, header));
                            if (h.HasProperty("dbName"))
                            {
                                h.Property("dbName").Remove();
                            }
                        }
                    }
                }
            }
            return definations;
        }

        public JArray GetSwaggerOperationConsumes(ApiEndpoint endpoint)
        {
            return new JArray(endpoint.consumes);
        }

        public JArray GetSwaggerOperationParameters(ApiEndpoint endpoint)
        {
            JArray parameters = new JArray();
            if (endpoint.auth != null)
            {
                var p = new
                {
                    name = Constants.AUTHORIZATION,
                    @in = "header",
                    required = true,
                    type = "string",
                    format = "jwt",
                    description = "JWT bearer token"
                };
                JObject d = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(p));
                parameters.Add(d);
            }
            foreach (var par in endpoint.inputs)
            {
                bool mandatory = par.validations.FindAll(v => v.type == ApiInputValidationType.mandatory).Count > 0;
                dynamic p = new ExpandoObject();
                p.name = par.name;
                p.@in = par.source;
                p.required = mandatory;
                p.type = GetSwagerType(par.type);
                var format = GetSwagerFormat(par.type);
                if (!string.IsNullOrEmpty(format))
                {
                    p.format = format;
                }
                if (!string.IsNullOrEmpty(par.pattern))
                {
                    p.pattern = par.pattern;
                }
                if (!string.IsNullOrEmpty(par.description))
                {
                    p.description = par.description;
                }
                if (!string.IsNullOrEmpty(par.example.TryParseString()))
                {
                    p.example = par.example;
                }
                if (par.@enum != null)
                {
                    p.@enum = new List<string>(par.@enum.Keys);
                }
                JObject d = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(p));
                parameters.Add(d);
            }
            return parameters;
        }

        public JArray GetSwaggerOperationProduces(ApiEndpoint endpoint)
        {
            return new JArray(endpoint.produces);
        }

        public JObject GetSwaggerOperationResponses(ApiEndpoint endpoint)
        {
            var response = new JObject();
            if (endpoint.responses != null)
            {
                response = endpoint.responses.ToJObject();
                List<string> allResponses = response.Properties().Select(p => p.Name).ToList();
                foreach (string res in allResponses)
                {
                    List<JToken> tokens = response.SelectTokens(string.Format("$.{0}.headers", res)).ToList();
                    foreach (JObject t in tokens)
                    {
                        List<string> data = t.Properties().Select(p => p.Name).ToList();
                        foreach (string header in data)
                        {
                            JObject h = (JObject)response.SelectToken(string.Format("{0}.{1}", t.Path, header));
                            h.Property("map").Remove();
                        }
                    }
                }
            }
            return response;
        }

        public string GetSwaggerOperationSummary(ApiEndpoint endpoint)
        {
            return endpoint.summary;
        }

        public JArray GetSwaggerOperationTags(ApiEndpoint endpoint)
        {
            return new JArray(endpoint.tag);
        }


        #region Private Methods

       

        private string GetSwagerType(ApiInputType type)
        {
            switch (type)
            {
                case ApiInputType.@int:
                case ApiInputType.@long:
                    return "integer";
                case ApiInputType.delimited:
                case ApiInputType.date:
                case ApiInputType.datetime:
                    return "string";
                default:
                    return type.ToString();
            }
        }

        
        private string GetSwagerFormat(ApiInputType type)
        {
            switch (type)
            {
                case ApiInputType.date:
                case ApiInputType.datetime:
                    return "date-time";
                default:
                    return null;
            }
        }

        #endregion
    }
}
