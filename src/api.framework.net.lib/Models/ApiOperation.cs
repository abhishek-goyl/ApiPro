using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace api.framework.net.Lib.Models
{
    public class ApiOperation
    {
        /// <summary>
        /// Gets or Sets the sequence number for the operation
        /// </summary>
        public short order { get; set; }

        /// <summary>
        /// Gets or Sets the type of operations like sp/query/business
        /// </summary>
        public ApiOperationTypes type { get; set; }

        /// <summary>
        /// Gets or Sets the value of operation like for sp type procedure name, for query type SQL Query, for business full name of dll
        /// </summary>
        public string value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the configuration name of the connection string, only valid for db type (sp/query) operations
        /// </summary>
        public string db { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the inputs,  only valid for db type (sp/query) operations
        /// </summary>
        public List<DbInput> inputs { get; set; } = new List<DbInput>();

        /// <summary>
        /// Gets or Sets the output defination in form of JSON Schema,  only valid for db type (sp/query) operations
        /// </summary>
        public JObject output { get; set; } = new JObject();

        /// <summary>
        /// Gets or Sets the error for the operation for swagger
        /// </summary>
        public JObject errors { get; set; } = new JObject();

        /// <summary>
        /// Gets or Sets the name of the method to be executed,  only valid for business type operations
        /// </summary>
        public string method { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the extension of file (for writeFile operation)
        /// </summary>
        public string fileType { get; set; }

        /// <summary>
        /// Gets or Sets the file name (for writeFile operation)
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// Gets or Sets the configuration
        /// </summary>
        public Dictionary<string, string> config { get; }
    }
}
