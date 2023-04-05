using Newtonsoft.Json;
using System;
using System.Globalization;

namespace api.framework.net.ExceptionLib
{
    public class AppException : Exception
    {
        public string ErrorCode { get; set; }
        
        public AppException(string message, string errorCode = null, Exception ex = null) : base(message, ex) {
            this.ErrorCode = errorCode;
        }

        public AppException(object data) : base(string.Empty)
        {
            this.Data.Add("res", JsonConvert.SerializeObject(data));
        }
    }
}
