using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class ApiMockResponse
    {
        /// <summary>
        /// Gets or Sets the status of mock operation
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Gets or Sets the success message of mock operation
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or Sets the Error Message
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        /// <summary>
        /// Gets or Sets the full path of mock file
        /// </summary>
        public string MockFile { get; set; }
    }
}
