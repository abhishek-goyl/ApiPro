using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Test.Helpers
{
    public class Constants
    {
        public const string AUTHORIZATION_HEADER = "Authorization";
        public const string AUTHORIZATION_MOCK_VALID_TOKEN = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        public const string REQUEST_ID = "X-RequestId";
        public const string REQUEST_ID_VALUE = "test";

        public const string LOG_EVENTS_HEADER_NAME = "logEvents";

    }
}
