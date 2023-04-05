using api.framework.net.Business.Contract;
using Newtonsoft.Json.Linq;
using api.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Business.BusinessLogic
{
    public class BusinessFactory 
    {
        public IBusinessGenericTransform _business { get; set; }
        public BusinessFactory(string name, Dictionary<string, string> configuration)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());
                var business = allTypes.Where(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                _business = (IBusinessGenericTransform)business.Assembly.CreateInstance(business.FullName);
                _business.configuration = configuration;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
            }
            finally
            {
                log.Exit();
            }
        }
    }
}
