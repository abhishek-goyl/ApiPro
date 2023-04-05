using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Contracts
{
    public interface ICacheProvider
    {
        void Add<T>(string key, T value, int expMinutes) where T : class;
        T Get<T>(string key, int expMinutes, Func<string, T> getItemDelegate, string par) where T : class;
    }
}
