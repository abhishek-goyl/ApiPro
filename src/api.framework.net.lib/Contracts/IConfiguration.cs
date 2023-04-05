using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Contracts
{
    public interface IConfiguration
    {
        T GetValue<T>(string key);
    }
}
