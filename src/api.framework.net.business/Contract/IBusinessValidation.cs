using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Business.Contract
{
    public interface IBusinessValidation
    {
        bool Validate(JObject obj);
    }
}
