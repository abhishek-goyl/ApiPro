using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace api.framework.net.Lib.Models
{
    public class ApiEndpoint
    {
        public ApiEndpoint()
        { }

        public ApiEndpoint(ApiEndpoint endpoint)
        {
            this.tag = endpoint.tag;
            this.name = endpoint.name;
            this.summary = endpoint.summary;
            this.version = endpoint.version;
            this.scope = endpoint.scope;
            this.route = endpoint.route;
            this.auth = endpoint.auth;
            this.responses = endpoint.responses; 
            this.verb = endpoint.verb;
            this.inputs = new List<ApiInput>();
            endpoint.inputs.ForEach(i => this.inputs.Add(new ApiInput(i)));
            this.operations = endpoint.operations;
            this.route = endpoint.route;
        }


        /// <summary>
        /// Gets or Sets the tag of Swagger, which is equivalant to controller in the route
        /// </summary>
        public string tag { get; set; } = "api";

        /// <summary>
        /// Gets or Sets the name of the Endpoint, which is equivalant to action in the route
        /// </summary>
        public string name { get; set; } = new Guid().ToString();

        /// <summary>
        /// Gets or Sets the Summary of the endpoint used in the swagger 
        /// </summary>
        public string summary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the version for the endpoint default value is 'v1'
        /// </summary>
        public string version { get; set; } = "v1";

        /// <summary>
        /// Gets or Sets the scope for the endpoint
        /// </summary>
        public string scope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Http Verb for the endpoint
        /// </summary>
        public HttpVerb verb { get; set; } = HttpVerb.GET;

        /// <summary>
        /// Gets or sets the route of the endpoint post /api/{version}/{tag}/{name}
        /// </summary>
        public string route { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the path prperty
        /// </summary>
        public string path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the auth and its claims
        /// </summary>
        public List<string> auth { get; set; }

        /// <summary>
        /// Gets or Sets the list of Inputs for the endpoint
        /// </summary>
        public List<ApiInput> inputs { get; set; } = new List<ApiInput>();

        /// <summary>
        /// Gets or Sets the List of operations for the API Endpoint
        /// </summary>
        public List<ApiOperation> operations { get; set; } = new List<ApiOperation>();

        /// <summary>
        /// Gets or Sets the reference defination for the endpoint response used for swagger doc
        /// </summary>
        public Dictionary<string, ApiResponse> responses { get; set; } = new Dictionary<string, ApiResponse>();

        /// <summary>
        /// Gets or Sets the Content Type for the endpoint used for documentation (swagger)
        /// </summary>
        public List<string> produces { get; set; } = new List<string> { "application/json" };

        /// <summary>
        /// Gets or Sets the Accepts for the endpoint used for documentation (swagger)
        /// </summary>
        public List<string> consumes { get; set; } = new List<string> { "application/json" };
    }
}
