using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models.Enums
{
    public enum ApiInputValidationType
    {
        mandatory,
        multiExpression,
        regex,
        compare,
        range,
        length,
        type,
        custom
    }
}
