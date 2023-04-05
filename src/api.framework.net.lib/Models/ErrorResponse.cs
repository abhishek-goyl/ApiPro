using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class ErrorResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiError> errors { get; set; }
    }

    public class ApiError
    {
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string actionToBeTaken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? ignore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PostedDataProperties ExtendedProperties { get; set; }

        public static ApiError GetValidationError(ApiInputValidation validation, string message, PostedDataProperties props)
        {
            ApiError err = new ApiError();
            try
            {
                err = new ApiError
                {
                    errorCode = validation.errorCode,
                    errorMessage = message,
                    actionToBeTaken = validation.actiontobetaken
                };
                if (props != null)
                {
                    err.ExtendedProperties = new PostedDataProperties
                    {
                        index = props.index,
                        TableName = props.TableName
                    };
                }
            }
            catch { }
            return err;
        }
    }
}
