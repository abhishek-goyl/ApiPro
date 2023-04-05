using api.framework.net.Business.Contract;
using Newtonsoft.Json.Linq;
using api.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Business.BusinessValidations
{
    public class ValidationFactory
    {
        public IBusinessValidation _validation { get; set; }
        public ValidationFactory(string name)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());
                var validation = allTypes.Where(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                _validation = (IBusinessValidation)validation.Assembly.CreateInstance(validation.FullName);
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
