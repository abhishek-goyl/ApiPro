using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.ExceptionLib
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message, string errorCode = null, Exception ex = null) : base(message, errorCode, ex) { }
        public BadRequestException(object data) : base(data) { }
    }
}
