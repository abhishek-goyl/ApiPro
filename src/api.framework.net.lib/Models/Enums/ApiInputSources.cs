using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace api.framework.net.Lib.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiInputSources
    {
        authClaim,
        query,
        path,
        header,
        body,
        form,
        none
    }
}
