using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.ExceptionLib
{
    public class NoRouteFoundException : AppException
    {
        public NoRouteFoundException(string message, string errorCode = null, Exception ex = null) : base(message, errorCode, ex) { }

        public NoRouteFoundException(object data) : base(data) { }
    }
}
