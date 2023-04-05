using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;

namespace api.framework.net.Lib.Contracts
{
    public interface IBusinessProvider
    {
        object ExecuteBusinessLogic(ApiOperation operation, JObject response, ref JObject inputs);
        bool Validate(object inputs);
    }
}
