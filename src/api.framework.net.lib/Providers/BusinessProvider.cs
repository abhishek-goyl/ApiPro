using System;
using System.IO;
using System.Linq;
using System.Reflection;
using api.framework.net.Business.BusinessLogic;
using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;

namespace api.framework.net.Lib.Providers
{
    public class BusinessProvider : IBusinessProvider
    {
        public BusinessProvider()
        { }

        public object ExecuteBusinessLogic(ApiOperation operation, JObject response, ref JObject inputs)
        {
            LogEvent log = LogEvent.Start();
            object res = new { };
            try
            {
                BusinessFactory factory = new BusinessFactory(operation.value, operation.config);
                res = factory._business.Transform(response, ref inputs);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
            return res;
        }

        public object ExecuteBusinessLogic2(ApiOperation operation, JObject response, ref JObject inputs)
        {
            var businessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"ExternalBusiness\{0}", operation.value));
            var name = Path.GetFileNameWithoutExtension(operation.value);
            var asm = Assembly.LoadFrom(businessPath);
            var type = asm.GetTypes().Where(t => t.Name.Equals(name)).FirstOrDefault();

            var inst = asm.CreateInstance(type?.FullName);
            var method = type?.GetMethod(operation.method);
            if (method != null)
            {
                return method.Invoke(inst, new object[] { response });
            }
            return response;
        }

        public bool Validate(object inputs)
        {
            throw new NotImplementedException();
        }
    }
}
