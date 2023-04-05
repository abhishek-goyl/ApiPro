using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class ApiResponse
    {
        public string ContentType { get; set; } = "application/json";
        public object ResponseData { get; set; }
        public string description { get; set; }
        public Dictionary<string, ApiHeaders> headers { get; set; } = new Dictionary<string, ApiHeaders>();
        public JObject schema { get; set; }
    }
}
