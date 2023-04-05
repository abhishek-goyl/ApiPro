using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.ExceptionLib
{
    public class UnAuthorizedException : AppException
    {
        public UnAuthorizedException(string message, string errorCode = null, Exception ex = null) : base(message, errorCode, ex) { }

        public UnAuthorizedException(object data) : base(data) { }
    }
}
