using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DbTypes
    {
        Nvarchar,
        Varchar,
        INT,
        SmallInt,
        Integer,
        Bit,
        TINYINT,
        BIGINT,
        FLOAT,
        DOUBLE,
        DECIMAL,
        DEC,
        DateTime,
        Date,
        CHAR,
        TINYTEXT,
        TEXT,
        MEDIUMTEXT,
        LONGTEXT,
        smallmoney,
        money
    }
}
