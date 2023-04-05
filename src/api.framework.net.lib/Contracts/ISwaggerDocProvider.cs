using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;

namespace api.framework.net.Lib.Contracts
{
    public interface ISwaggerDocProvider
    {
        JObject GetSwaggerDocument(ApiSchema schema);
        JObject GetBaseSwaggerDocument(ApiSchema schema);
        JArray GetSwaggerOperationTags(ApiEndpoint endpoint);
        string GetSwaggerOperationSummary(ApiEndpoint endpoint);
        JArray GetSwaggerOperationConsumes(ApiEndpoint endpoint);
        JArray GetSwaggerOperationProduces(ApiEndpoint endpoint);
        JArray GetSwaggerOperationParameters(ApiEndpoint endpoint);
        JObject GetSwaggerOperationResponses(ApiEndpoint endpoint);
        JObject GetSwaggerDefinations(ApiSchema schema);
    }
}
