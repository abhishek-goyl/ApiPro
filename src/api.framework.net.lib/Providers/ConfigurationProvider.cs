using api.framework.net.Lib.Contracts;
using System;
using System.Configuration;

namespace api.framework.net.Lib.Providers
{
    public class ConfigurationProvider : IConfiguration
    {
        public ConfigurationProvider()
        { }

        public T GetValue<T>(string key)
        {
            T res = default(T);
            try
            {
                var data = ConfigurationManager.AppSettings[key];
                res = (T)Convert.ChangeType(data, typeof(T));
            }
            catch { }
            return res;
        }
    }
}
