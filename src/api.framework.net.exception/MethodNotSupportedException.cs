using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.ExceptionLib
{
    public class MethodNotSupportedException : AppException
    {
        public MethodNotSupportedException(string message, string errorCode = null, Exception ex = null) : base(message, errorCode, ex) { }

        public MethodNotSupportedException(object data) : base(data) { }
    }
}
