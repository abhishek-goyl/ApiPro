using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;


namespace api.framework.net.Lib.Models
{
    public class ApiSchema
    {
        /// <summary>
        /// Gets or Sets the name of the API for swagger documentation
        /// </summary>
        public string name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or Sets the supported schemes for the API
        /// </summary>
        public string[] schemes { get; set; }

        /// <summary>
        /// Gets or Sets the endpoints for the API
        /// </summary>
        public List<ApiEndpoint> endpoints { get; set; } = new List<ApiEndpoint>();

        /// <summary>
        /// Gets or Sets the Swagger definations
        /// </summary>
        public JObject definations { get; set; } = new JObject();
    }
}
