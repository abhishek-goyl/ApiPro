using api.framework.net.Business.Contract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.business.BusinessLogic
{
    class TestBusiness : IBusinessGenericTransform
    {
        public Dictionary<string, string> configuration { get; set; }

        public JObject Transform(JObject response, ref JObject inputs)
        {
            // configuration[""]
            return new JObject();
        }
    }
}
