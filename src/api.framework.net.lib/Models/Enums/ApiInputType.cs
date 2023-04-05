using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace api.framework.net.Lib.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiInputType
    {
        @long,
        @int,
        @double,
        @string,
        date,
        datetime,
        delimited,
        @bool,
        Array
    }
}
