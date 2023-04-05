using api.framework.net.Lib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class DataColumn
    {
        public string name { get; set; }
        public ApiInputType type { get; set; } = ApiInputType.@string;
        public string mappingname { get; set; }
        public string pattern { get; set; }
        public List<ApiInputValidation> validations { get; set; } = new List<ApiInputValidation>();
        public string defaultValue { get; set; }
        public int mapId { get; set; }
      
        /// <summary>
        /// Gets or Sets the enum values for the input
        /// </summary>
        public Dictionary<string, int> @enum { get; set; }
    }
}
